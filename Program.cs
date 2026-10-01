using System;
using System.Collections.Generic;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.CoreAudioApi;

class Program
{
    static void Main(string[] args)
    {
        var mixList = new List<ISampleProvider>();

        // Check for Preset Flag
        string presetKey = GetArg(args, "--preset", "none");
        SoundscapePreset activeProfile = new SoundscapePreset();

        if (Presets.Profiles.TryGetValue(presetKey, out var foundProfile))
        {
            activeProfile = foundProfile;
        }

        // Ramping Parameters
        string rampStartKey = GetArg(args, "--ramp-start", "none");
        string rampEndKey   = GetArg(args, "--ramp-end", "none");
        TimeSpan rampTime   = GetArgTimeSpan(args, "--ramp-time", TimeSpan.FromMinutes(15));
        float rampPitch     = GetArgHz(args, "--ramp-pitch", 180f);
        float rampVol       = GetArgVol(args, "--ramp-vol", 0.12f);

        // Standard Layer Arguments (Falling back to Preset Profile defaults)
        string bin1Key   = GetArg(args, "--binaural1", activeProfile.Bin1Key);
        float bin1Pitch  = GetArgHz(args, "--bin1-pitch", activeProfile.Bin1Pitch);
        float bin1Vol    = GetArgVol(args, "--bin1-vol", activeProfile.Bin1Vol);

        string bin2Key   = GetArg(args, "--binaural2", activeProfile.Bin2Key);
        float bin2Pitch  = GetArgHz(args, "--bin2-pitch", activeProfile.Bin2Pitch);
        float bin2Vol    = GetArgVol(args, "--bin2-vol", activeProfile.Bin2Vol);

        string isoKey    = GetArg(args, "--iso", activeProfile.IsoKey);
        float isoPitch   = GetArgHz(args, "--iso-pitch", activeProfile.IsoPitch);
        float isoVol     = GetArgVol(args, "--iso-vol", activeProfile.IsoVol);

        string noise     = GetArg(args, "--noise", activeProfile.NoiseType);
        float noiseVol   = GetArgVol(args, "--noise-vol", activeProfile.NoiseVol);

        Console.WriteLine("==================================================");
        if (!string.IsNullOrEmpty(activeProfile.Name))
        {
            Console.WriteLine($"  PRESET: {activeProfile.Name.ToUpper()}");
            Console.WriteLine($"  {activeProfile.Description}");
        }
        else
        {
            Console.WriteLine("  CUSTOM SOUNDSCAPE ENGINE");
        }
        Console.WriteLine("==================================================");

        // Dynamic Frequency Ramp Layer
        if (Presets.SubBands.TryGetValue(rampStartKey, out float startBeatHz) &&
            Presets.SubBands.TryGetValue(rampEndKey, out float endBeatHz))
        {
            var rampLayer = new RampedBinauralLayer(
                sampleRate: 44100,
                baseCarrierHz: rampPitch,
                startBeatHz: startBeatHz,
                targetBeatHz: endBeatHz,
                rampDuration: rampTime,
                amplitude: rampVol
            );
            mixList.Add(rampLayer.StereoOutput);
            Console.WriteLine($"  • Dynamic Ramp: {rampStartKey} ({startBeatHz:F1} Hz) -> {rampEndKey} ({endBeatHz:F1} Hz) over {rampTime.TotalMinutes:F1} min @ {rampPitch:F0} Hz [Vol: {rampVol * 100:F0}%]");
        }

        // Binaural Layer 1
        if (Presets.SubBands.TryGetValue(bin1Key, out float bin1Freq))
        {
            var b1 = new BinauralBeatLayer(baseFrequency: bin1Pitch, beatFrequency: bin1Freq, amplitude: bin1Vol);
            mixList.Add(b1.StereoOutput);
            Console.WriteLine($"  • Binaural 1   : {bin1Key,-10} ({bin1Freq,4:F1} Hz beat @ {bin1Pitch,5:F0} Hz base) [Vol: {bin1Vol * 100,3:F0}%]");
        }

        // Binaural Layer 2
        if (Presets.SubBands.TryGetValue(bin2Key, out float bin2Freq))
        {
            var b2 = new BinauralBeatLayer(baseFrequency: bin2Pitch, beatFrequency: bin2Freq, amplitude: bin2Vol);
            mixList.Add(b2.StereoOutput);
            Console.WriteLine($"  • Binaural 2   : {bin2Key,-10} ({bin2Freq,4:F1} Hz beat @ {bin2Pitch,5:F0} Hz base) [Vol: {bin2Vol * 100,3:F0}%]");
        }

        // Isochronic Pulse Layer
        if (Presets.SubBands.TryGetValue(isoKey, out float isoFreq))
        {
            var isoStereo = new IsochronicPulseProvider(sampleRate: 44100, carrierFrequency: isoPitch, pulseFrequency: isoFreq) 
            { 
                Amplitude = isoVol,
                PhaseOffsetRadians = 0.25f 
            };
            mixList.Add(isoStereo);
            Console.WriteLine($"  • Isochronic   : {isoKey,-10} ({isoFreq,4:F1} Hz pulse @ {isoPitch,5:F0} Hz carrier) [Vol: {isoVol * 100,3:F0}%]");
        }

        // Ambient Noise Layer
        if (noise.Equals("pink", StringComparison.OrdinalIgnoreCase))
        {
            mixList.Add(DecorrelatedNoise.CreateStereoPink(44100, amplitude: noiseVol));
            Console.WriteLine($"  • Ambient      : Pink Noise                                   [Vol: {noiseVol * 100,3:F0}%]");
        }
        else if (noise.Equals("brown", StringComparison.OrdinalIgnoreCase))
        {
            mixList.Add(DecorrelatedNoise.CreateStereoBrown(44100, amplitude: noiseVol));
            Console.WriteLine($"  • Ambient      : Brown Noise                                  [Vol: {noiseVol * 100,3:F0}%]");
        }

        if (mixList.Count == 0)
        {
            Console.WriteLine("No active audio layers selected. Exiting.");
            return;
        }

        // Master Mixer & Output
        var masterMixer = new MixingSampleProvider(mixList);

        // Pass through Soft Limiter with a 95% (-0.45 dBFS) peak safety ceiling
        var safeMasterOutput = new SoftLimiterProvider(masterMixer, ceiling: 0.95f);

        using var enumerator = new MMDeviceEnumerator();
        var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        using var outputDevice = new WasapiOut(defaultDevice, AudioClientShareMode.Shared, useEventSync: true, latency: 50);
        outputDevice.Init(safeMasterOutput);
        outputDevice.Play();

        Console.WriteLine("==================================================");
        Console.WriteLine($"Playing on: {defaultDevice.FriendlyName}");
        Console.WriteLine("\nPress Enter to stop playback.");
        Console.ReadLine();

        outputDevice.Stop();
    }

    private static string GetArg(string[] args, string flag, string defaultValue)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].Equals(flag, StringComparison.OrdinalIgnoreCase))
                return args[i + 1];
        }
        return defaultValue;
    }

    private static float GetArgHz(string[] args, string flag, float defaultValue)
    {
        string val = GetArg(args, flag, string.Empty);
        if (string.IsNullOrEmpty(val)) return defaultValue;

        if (float.TryParse(val, out float result) && result > 0)
            return result;

        return defaultValue;
    }

    private static float GetArgVol(string[] args, string flag, float defaultValue)
    {
        string val = GetArg(args, flag, string.Empty);
        if (string.IsNullOrEmpty(val)) return defaultValue;

        val = val.Replace("%", "").Trim();
        if (float.TryParse(val, out float result))
        {
            if (result > 1.0f && result <= 100.0f) result /= 100.0f;
            return Math.Clamp(result, 0.0f, 1.0f);
        }
        return defaultValue;
    }

    private static TimeSpan GetArgTimeSpan(string[] args, string flag, TimeSpan defaultValue)
    {
        string val = GetArg(args, flag, string.Empty).ToLower().Trim();
        if (string.IsNullOrEmpty(val)) return defaultValue;

        if (val.EndsWith("m") && double.TryParse(val.TrimEnd('m'), out double mins))
            return TimeSpan.FromMinutes(mins);

        if (val.EndsWith("s") && double.TryParse(val.TrimEnd('s'), out double secs))
            return TimeSpan.FromSeconds(secs);

        if (double.TryParse(val, out double directMins))
            return TimeSpan.FromMinutes(directMins);

        return defaultValue;
    }
}
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

public class BinauralBeatLayer
{
    public SineWaveProvider LeftChannel { get; }
    public SineWaveProvider RightChannel { get; }
    public MultiplexingSampleProvider StereoOutput { get; }

    public BinauralBeatLayer(float baseFrequency, float beatFrequency, float amplitude = 0.12f)
    {
        // Left Ear = Base Frequency
        LeftChannel = new SineWaveProvider(44100, baseFrequency) { Amplitude = amplitude };
        // Right Ear = Base + Target Beat Frequency
        RightChannel = new SineWaveProvider(44100, baseFrequency + beatFrequency) { Amplitude = amplitude };

        // Route Mono -> Stereo
        StereoOutput = new MultiplexingSampleProvider(
            new ISampleProvider[] { LeftChannel, RightChannel }, 
            2
        );
        StereoOutput.ConnectInputToOutput(0, 0); // Left
        StereoOutput.ConnectInputToOutput(1, 1); // Right
    }
}
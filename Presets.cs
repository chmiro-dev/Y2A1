using System;
using System.Collections.Generic;

public class SoundscapePreset
{
    public string Key { SqlString = ""; get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Dynamic Ramping Layer
    public string RampStartKey { get; set; } = "none";
    public string RampEndKey { get; set; } = "none";
    public TimeSpan RampTime { get; set; } = TimeSpan.FromMinutes(15);
    public float RampPitch { get; set; } = 180f;
    public float RampVol { get; set; } = 0.12f;

    // Static Binaural Layer 1
    public string Bin1Key { get; set; } = "none";
    public float Bin1Pitch { get; set; } = 180f;
    public float Bin1Vol { get; set; } = 0.10f;

    // Static Binaural Layer 2
    public string Bin2Key { get; set; } = "none";
    public float Bin2Pitch { get; set; } = 120f;
    public float Bin2Vol { get; set; } = 0.06f;

    // Isochronic Layer
    public string IsoKey { get; set; } = "none";
    public float IsoPitch { get; set; } = 220f;
    public float IsoVol { get; set; } = 0.06f;

    // Ambient Noise Layer
    public string NoiseType { get; set; } = "none"; // "pink", "brown", or "none"
    public float NoiseVol { get; set; } = 0.04f;
}

public static class Presets
{
    /// <summary>
    /// Case-insensitive mapping of brainwave sub-band keys to target beat frequencies (Hz).
    /// </summary>
    public static readonly Dictionary<string, float> SubBands = new(StringComparer.OrdinalIgnoreCase)
    {
        { "delta-low",   1.2f },
        { "delta-high",  3.0f },
        { "theta-low",   5.0f },
        { "theta-high",  7.0f },
        { "alpha-low",   9.0f },
        { "alpha-mid",  10.5f },
        { "alpha-high", 12.0f },
        { "beta-low",   15.0f },
        { "beta-high",  22.0f },
        { "gamma-low",  40.0f }
    };

    /// <summary>
    /// Pre-configured soundscape profiles.
    /// </summary>
    public static readonly Dictionary<string, SoundscapePreset> Profiles = new(StringComparer.OrdinalIgnoreCase)
    {
        {
            "sleep-ramp", new SoundscapePreset
            {
                Key = "sleep-ramp",
                Name = "Sleep Onset Ramp (20 Min)",
                Description = "Eases brainwave activity from Alpha flow down to deep Delta, masked with warm Brown noise.",
                RampStartKey = "alpha-low",
                RampEndKey = "delta-low",
                RampTime = TimeSpan.FromMinutes(20),
                RampPitch = 130f,
                RampVol = 0.12f,
                NoiseType = "brown",
                NoiseVol = 0.05f
            }
        },
        {
            "deep-rest", new SoundscapePreset
            {
                Key = "deep-rest",
                Name = "Deep Delta Rest",
                Description = "Dual static binaural layers tuned for deep dreamless sleep and physical recovery.",
                Bin1Key = "delta-low",
                Bin1Pitch = 110f,
                Bin1Vol = 0.12f,
                Bin2Key = "delta-high",
                Bin2Pitch = 150f,
                Bin2Vol = 0.06f,
                NoiseType = "brown",
                NoiseVol = 0.04f
            }
        },
        {
            "deep-focus", new SoundscapePreset
            {
                Key = "deep-focus",
                Name = "Deep Focus & Study",
                Description = "Alpha-Mid binaural beat combined with light Isochronic pulse and soft Pink noise for flow state.",
                Bin1Key = "alpha-mid",
                Bin1Pitch = 210f,
                Bin1Vol = 0.12f,
                IsoKey = "alpha-mid",
                IsoPitch = 315f,
                IsoVol = 0.05f,
                NoiseType = "pink",
                NoiseVol = 0.03f
            }
        },
        {
            "meditation-ramp", new SoundscapePreset
            {
                Key = "meditation-ramp",
                Name = "Meditation Descent (15 Min)",
                Description = "Smooth 15-minute descent from relaxed Alpha-High down to deep Theta meditation.",
                RampStartKey = "alpha-high",
                RampEndKey = "theta-low",
                RampTime = TimeSpan.FromMinutes(15),
                RampPitch = 160f,
                RampVol = 0.12f,
                Bin1Key = "theta-high",
                Bin1Pitch = 120f,
                Bin1Vol = 0.05f,
                NoiseType = "brown",
                NoiseVol = 0.03f
            }
        },
        {
            "cognition-boost", new SoundscapePreset
            {
                Key = "cognition-boost",
                Name = "Gamma Cognition Boost",
                Description = "High-arousal 40 Hz Gamma layer layered with Beta focus for active problem solving.",
                Bin1Key = "gamma-low",
                Bin1Pitch = 240f,
                Bin1Vol = 0.10f,
                IsoKey = "beta-low",
                IsoPitch = 320f,
                IsoVol = 0.06f,
                NoiseType = "pink",
                NoiseVol = 0.03f
            }
        }
    };
}
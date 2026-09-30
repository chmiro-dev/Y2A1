using System;
using System.Collections.Generic;

public class SoundscapePreset
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Binaural Layer 1
    public string Bin1Key { get; set; } = "none";
    public float Bin1Pitch { get; set; } = 180f;
    public float Bin1Vol { get; set; } = 0.10f;

    // Binaural Layer 2
    public string Bin2Key { get; set; } = "none";
    public float Bin2Pitch { get; set; } = 120f;
    public float Bin2Vol { get; set; } = 0.08f;

    // Isochronic Layer
    public string IsoKey { get; set; } = "none";
    public float IsoPitch { get; set; } = 220f;
    public float IsoVol { get; set; } = 0.08f;

    // Noise Layer
    public string NoiseType { get; set; } = "none";
    public float NoiseVol { get; set; } = 0.05f;
}

public static class Presets
{
    // Frequency Sub-Bands (in Hz)
    public static readonly Dictionary<string, float> SubBands = new(StringComparer.OrdinalIgnoreCase)
    {
        // Delta
        { "delta-low",   1.2f },
        { "delta-high",  3.0f },

        // Theta
        { "theta-low",   5.0f },
        { "theta-high",  7.0f },

        // Alpha
        { "alpha-low",   9.0f },
        { "alpha-mid",   10.5f },
        { "alpha-high",  12.0f },

        // Beta
        { "beta-low",    15.0f },
        { "beta-high",   22.0f },

        // Gamma
        { "gamma-low",   40.0f },
        { "gamma-mid",   55.0f },
        { "gamma-high",  80.0f }
    };

    // Pre-Calibrated Soundscape Presets
    public static readonly Dictionary<string, SoundscapePreset> Profiles = new(StringComparer.OrdinalIgnoreCase)
    {
        {
            "deep-sleep", new SoundscapePreset
            {
                Name = "Deep Sleep & Body Recovery",
                Description = "Harmonic Theta/Delta stack with boosted bass carriers and warm Brown noise.",
                Bin1Key = "theta-low", Bin1Pitch = 130f, Bin1Vol = 0.18f, // Boosted pitch & vol for bass clarity
                Bin2Key = "delta-high", Bin2Pitch = 100f, Bin2Vol = 0.15f,
                NoiseType = "brown", NoiseVol = 0.06f
            }
        },
        {
            "flow-state", new SoundscapePreset
            {
                Name = "Flow State & Creative Focus",
                Description = "Mid-Alpha entrainment with balanced 200 Hz carrier and decorrelated Pink noise.",
                Bin1Key = "alpha-mid", Bin1Pitch = 200f, Bin1Vol = 0.10f,
                NoiseType = "pink", NoiseVol = 0.04f
            }
        },
        {
            "gamma-focus", new SoundscapePreset
            {
                Name = "Peak Cognition & High Focus",
                Description = "40 Hz Gamma binaural beat paired with light Pink noise.",
                Bin1Key = "gamma-low", Bin1Pitch = 280f, Bin1Vol = 0.08f, // Softened carrier to prevent harshness
                NoiseType = "pink", NoiseVol = 0.03f
            }
        },
        {
            "meditation", new SoundscapePreset
            {
                Name = "Deep Meditation & Hypnagogia",
                Description = "Low Theta binaural beat combined with a gentle Isochronic Alpha pulse.",
                Bin1Key = "theta-low", Bin1Pitch = 150f, Bin1Vol = 0.12f,
                IsoKey = "alpha-low", IsoPitch = 210f, IsoVol = 0.06f,
                NoiseType = "brown", NoiseVol = 0.04f
            }
        }
    };
}
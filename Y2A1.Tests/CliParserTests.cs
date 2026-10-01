using System;
using Xunit;

namespace Y2A1.Tests;

public class CliParserTests
{
    [Theory]
    [InlineData("5%", 0.05f)]
    [InlineData("100%", 1.0f)]
    [InlineData("0.12", 0.12f)]
    [InlineData("150%", 1.0f)] // Clamped upper bound
    [InlineData("-10%", 0.0f)] // Clamped lower bound
    public void GetArgVol_ParsesAndClampsCorrectly(string input, float expected)
    {
        string[] args = new[] { "--vol", input };
        float result = Program.GetArgVol(args, "--vol", 0.10f);
        Assert.Equal(expected, result, precision: 4);
    }

    [Theory]
    [InlineData("15m", 15)]
    [InlineData("30s", 0.5)]
    [InlineData("10", 10)]
    public void GetArgTimeSpan_ParsesUnitsCorrectly(string input, double expectedMinutes)
    {
        string[] args = new[] { "--time", input };
        TimeSpan result = Program.GetArgTimeSpan(args, "--time", TimeSpan.Zero);
        Assert.Equal(TimeSpan.FromMinutes(expectedMinutes), result);
    }

    [Fact]
    public void BuildAudioLayers_InstantiatesPresetCorrectly()
    {
        string[] args = new[] { "--preset", "sleep-ramp" };
        var layers = Program.BuildAudioLayers(args, out var profile);

        Assert.Equal("Sleep Onset Ramp (20 Min)", profile.Name);
        Assert.True(layers.Count >= 2); // Ramped Layer + Ambient Noise Layer
    }
}
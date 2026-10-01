using System;
using Xunit;

namespace Y2A1.Tests;

public class RampedSineProviderTests
{
    private const float DefaultSampleRate = 44100f;

    [Fact]
    public void Read_FillsBufferWithCorrectSampleCount()
    {
        var provider = new RampedSineProvider(
            sampleRate: DefaultSampleRate,
            baseCarrierHz: 200f,
            startBeatHz: 10f,
            targetBeatHz: 2f,
            rampDuration: TimeSpan.FromSeconds(10),
            isRightChannel: false
        );

        float[] buffer = new float[512];
        int samplesRead = provider.Read(buffer, 0, buffer.Length);

        Assert.Equal(buffer.Length, samplesRead);
    }

    [Fact]
    public void Read_RespectsAmplitudeBoundaries()
    {
        float targetAmplitude = 0.25f;
        var provider = new RampedSineProvider(
            sampleRate: DefaultSampleRate,
            baseCarrierHz: 220f,
            startBeatHz: 12f,
            targetBeatHz: 2f,
            rampDuration: TimeSpan.FromSeconds(5),
            isRightChannel: true)
        {
            Amplitude = targetAmplitude
        };

        float[] buffer = new float[44100]; // 1 second of audio
        provider.Read(buffer, 0, buffer.Length);

        foreach (float sample in buffer)
        {
            Assert.InRange(sample, -targetAmplitude - 0.0001f, targetAmplitude + 0.0001f);
        }
    }
}
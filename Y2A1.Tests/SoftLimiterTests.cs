using System;
using NAudio.Wave;
using Xunit;

namespace Y2A1.Tests;

public class SoftLimiterTests
{
    // Mock Provider that generates intentionally clipping samples (e.g. 2.0f)
    private class OverdrivenSampleProvider : ISampleProvider
    {
        public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        public int Read(float[] buffer, int offset, int count)
        {
            for (int i = offset; i < offset + count; i++)
            {
                buffer[i] = (i % 2 == 0) ? 2.5f : -2.5f; // Harsh clipping values
            }
            return count;
        }
    }

    [Fact]
    public void Read_PreventsSamplesFromExceedingCeiling()
    {
        // Arrange
        float safetyCeiling = 0.95f;
        var overdrivenSource = new OverdrivenSampleProvider();
        var limiter = new SoftLimiterProvider(overdrivenSource, ceiling: safetyCeiling);

        float[] buffer = new float[100];

        // Act
        limiter.Read(buffer, 0, buffer.Length);

        // Assert
        foreach (float sample in buffer)
        {
            // All samples must strictly stay within [-0.95, +0.95]
            Assert.True(Math.Abs(sample) <= safetyCeiling, $"Sample {sample} exceeded safety ceiling {safetyCeiling}");
        }
    }

    [Fact]
    public void Read_LeavesNormalUnclippedSamplesUntouched()
    {
        // Arrange
        float safetyCeiling = 0.95f;
        float normalSampleValue = 0.5f;

        var normalSource = new CustomBufferSampleProvider(normalSampleValue);
        var limiter = new SoftLimiterProvider(normalSource, ceiling: safetyCeiling);

        float[] buffer = new float[10];

        // Act
        limiter.Read(buffer, 0, buffer.Length);

        // Assert
        foreach (float sample in buffer)
        {
            Assert.Equal(normalSampleValue, sample);
        }
    }

    private class CustomBufferSampleProvider : ISampleProvider
    {
        private readonly float _fillValue;
        public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        public CustomBufferSampleProvider(float fillValue) => _fillValue = fillValue;

        public int Read(float[] buffer, int offset, int count)
        {
            Array.Fill(buffer, _fillValue, offset, count);
            return count;
        }
    }
}
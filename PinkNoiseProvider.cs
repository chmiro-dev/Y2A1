using System;
using NAudio.Wave;

public class PinkNoiseProvider : ISampleProvider
{
    private readonly Random _random = new();
    private readonly float[] _rows = new float[7];
    private float _runningSum;
    private int _index;

    public WaveFormat WaveFormat { get; }
    public float Amplitude { get; set; } = 0.05f;

    public PinkNoiseProvider(int sampleRate = 44100)
    {
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);
    }

    public int Read(float[] buffer, int offset, int count)
    {
        return Read(buffer.AsSpan(offset, count));
    }

    public int Read(Span<float> buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            _index = (_index + 1) & 0x3F; // Count 0 to 63

            // Determine which row to update using trailing zeros
            int numZeros = System.Numerics.BitOperations.TrailingZeroCount(_index);
            if (numZeros < 7)
            {
                _runningSum -= _rows[numZeros];
                float newRandom = (float)(_random.NextDouble() * 2.0 - 1.0);
                _rows[numZeros] = newRandom;
                _runningSum += newRandom;
            }

            // Scale combined rows to [-1.0, 1.0]
            float output = (_runningSum / 7.0f) + (float)(_random.NextDouble() * 2.0 - 1.0) * 0.1f;
            buffer[i] = output * Amplitude;
        }

        return buffer.Length;
    }
}
using System;
using NAudio.Wave;

public class BrownNoiseProvider : ISampleProvider
{
    private readonly Random _random = new();
    private float _currentSample;
    
    public WaveFormat WaveFormat { get; }
    public float Amplitude { get; set; } = 0.08f; // Kept lower to soft-mask behind binaural beats

    public BrownNoiseProvider(int sampleRate = 44100)
    {
        // Mono format; will be expanded or centered in stereo mix
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
            // Generate white noise step between -1.0 and 1.0
            float white = (float)(_random.NextDouble() * 2.0 - 1.0);

            // Accumulate (random walk)
            _currentSample = (_currentSample + (0.02f * white)) / 1.02f;

            // Prevent extreme values and scale by amplitude
            buffer[i] = _currentSample * Amplitude;
        }

        return buffer.Length;
    }
}
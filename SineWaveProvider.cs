using System;
using NAudio.Wave;

public class SineWaveProvider : ISampleProvider
{
    private float _phase;
    public WaveFormat WaveFormat { get; }
    public float Frequency { get; set; }
    public float Amplitude { get; set; } = 0.25f;

    public SineWaveProvider(int sampleRate = 44100, float frequency = 440f)
    {
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1); // Mono
        Frequency = frequency;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        return Read(buffer.AsSpan(offset, count));
    }

    public int Read(Span<float> buffer)
    {
        int sampleRate = WaveFormat.SampleRate;

        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = Amplitude * (float)Math.Sin(_phase);
            _phase += (2 * (float)Math.PI * Frequency) / sampleRate;

            if (_phase >= 2 * Math.PI)
                _phase -= 2 * (float)Math.PI;
        }

        return buffer.Length;
    }
}
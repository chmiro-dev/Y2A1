using System;
using NAudio.Wave;

/// <summary>
/// Wraps an ISampleProvider and applies smooth tanh-based soft-clipping 
/// to prevent digital clipping distortion and protect hearing.
/// </summary>
public class SoftLimiterProvider : ISampleProvider
{
    private readonly ISampleProvider _source;
    private readonly float _ceiling;

    public WaveFormat WaveFormat => _source.WaveFormat;

    public SoftLimiterProvider(ISampleProvider source, float ceiling = 0.95f)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _ceiling = Math.Clamp(ceiling, 0.1f, 1.0f);
    }

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = _source.Read(buffer, offset, count);

        for (int i = offset; i < offset + samplesRead; i++)
        {
            float sample = buffer[i];

            // Only apply tanh shaping when sample exceeds safety threshold
            if (Math.Abs(sample) > _ceiling)
            {
                buffer[i] = (float)Math.Tanh(sample / _ceiling) * _ceiling;
            }
        }

        return samplesRead;
    }
}
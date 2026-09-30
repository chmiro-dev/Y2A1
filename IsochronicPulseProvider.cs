using System;
using NAudio.Wave;

public class IsochronicPulseProvider : ISampleProvider
{
    private float _carrierPhaseLeft;
    private float _carrierPhaseRight;
    private float _pulsePhase;

    public WaveFormat WaveFormat { get; }
    public float CarrierFrequency { get; set; }
    public float PulseFrequency { get; set; }
    public float Amplitude { get; set; } = 0.08f;
    public float PhaseOffsetRadians { get; set; } = 0.2f; // Slight phase spread between ears

    public IsochronicPulseProvider(int sampleRate = 44100, float carrierFrequency = 220f, float pulseFrequency = 10f)
    {
        // 2-channel stereo format directly
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2);
        CarrierFrequency = carrierFrequency;
        PulseFrequency = pulseFrequency;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        return Read(buffer.AsSpan(offset, count));
    }

    public int Read(Span<float> buffer)
    {
        int sampleRate = WaveFormat.SampleRate;

        // Note: Step by 2 because buffer contains interleaved [Left, Right, Left, Right...]
        for (int i = 0; i < buffer.Length; i += 2)
        {
            // 1. Calculate Pulse LFO (0.0 to 1.0)
            float pulseLfo = 0.5f * (1.0f + (float)Math.Sin(_pulsePhase));
            _pulsePhase += (2 * (float)Math.PI * PulseFrequency) / sampleRate;
            if (_pulsePhase >= 2 * Math.PI) _pulsePhase -= 2 * (float)Math.PI;

            // 2. Left Carrier Channel
            float carrierLeft = (float)Math.Sin(_carrierPhaseLeft);
            _carrierPhaseLeft += (2 * (float)Math.PI * CarrierFrequency) / sampleRate;
            if (_carrierPhaseLeft >= 2 * Math.PI) _carrierPhaseLeft -= 2 * (float)Math.PI;

            // 3. Right Carrier Channel (Slightly phase-shifted to remove center pressure)
            float carrierRight = (float)Math.Sin(_carrierPhaseRight + PhaseOffsetRadians);
            _carrierPhaseRight += (2 * (float)Math.PI * CarrierFrequency) / sampleRate;
            if (_carrierPhaseRight >= 2 * Math.PI) _carrierPhaseRight -= 2 * (float)Math.PI;

            // Interleaved stereo samples
            buffer[i]     = carrierLeft * pulseLfo * Amplitude;     // Left Ear
            buffer[i + 1] = carrierRight * pulseLfo * Amplitude;    // Right Ear
        }

        return buffer.Length;
    }
}
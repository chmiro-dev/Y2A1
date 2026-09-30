using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

public class RampedBinauralLayer
{
    public MultiplexingSampleProvider StereoOutput { get; }

    public RampedBinauralLayer(
        float sampleRate,
        float baseCarrierHz,
        float startBeatHz,
        float targetBeatHz,
        TimeSpan rampDuration,
        float amplitude)
    {
        var leftOsc = new RampedSineProvider(sampleRate, baseCarrierHz, startBeatHz, targetBeatHz, rampDuration, isRightChannel: false)
        {
            Amplitude = amplitude
        };

        var rightOsc = new RampedSineProvider(sampleRate, baseCarrierHz, startBeatHz, targetBeatHz, rampDuration, isRightChannel: true)
        {
            Amplitude = amplitude
        };

        StereoOutput = new MultiplexingSampleProvider(new ISampleProvider[] { leftOsc, rightOsc }, 2);
        StereoOutput.ConnectInputToOutput(0, 0); // Left channel
        StereoOutput.ConnectInputToOutput(1, 1); // Right channel
    }
}

public class RampedSineProvider : ISampleProvider
{
    private readonly float _sampleRate;
    private readonly float _baseCarrierHz;
    private readonly float _startBeatHz;
    private readonly float _targetBeatHz;
    private readonly long _totalRampSamples;
    private readonly bool _isRightChannel;

    private double _phase;
    private long _sampleCount;

    public WaveFormat WaveFormat { get; }
    public float Amplitude { get; set; } = 0.10f;

    public RampedSineProvider(
        float sampleRate,
        float baseCarrierHz,
        float startBeatHz,
        float targetBeatHz,
        TimeSpan rampDuration,
        bool isRightChannel)
    {
        _sampleRate = sampleRate;
        _baseCarrierHz = baseCarrierHz;
        _startBeatHz = startBeatHz;
        _targetBeatHz = targetBeatHz;
        _totalRampSamples = (long)(sampleRate * rampDuration.TotalSeconds);
        _isRightChannel = isRightChannel;

        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)sampleRate, 1);
    }

    public int Read(Span<float> buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            double progress = _totalRampSamples > 0 
                ? Math.Min(1.0, (double)_sampleCount / _totalRampSamples) 
                : 1.0;

            double currentBeatHz = _startBeatHz + (_targetBeatHz - _startBeatHz) * progress;

            double currentFreq = _isRightChannel 
                ? _baseCarrierHz + currentBeatHz 
                : _baseCarrierHz;

            _phase += 2.0 * Math.PI * currentFreq / _sampleRate;
            if (_phase >= 2.0 * Math.PI) _phase -= 2.0 * Math.PI;

            buffer[i] = (float)(Amplitude * Math.Sin(_phase));
            _sampleCount++;
        }

        return buffer.Length;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        return Read(buffer.AsSpan(offset, count));
    }
}
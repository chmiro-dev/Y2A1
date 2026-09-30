using NAudio.Wave;
using NAudio.Wave.SampleProviders;

public static class DecorrelatedNoise
{
    public static ISampleProvider CreateStereoPink(int sampleRate = 44100, float amplitude = 0.05f)
    {
        // Two distinct instances = independent random seeds
        var left = new PinkNoiseProvider(sampleRate) { Amplitude = amplitude };
        var right = new PinkNoiseProvider(sampleRate) { Amplitude = amplitude };

        var stereo = new MultiplexingSampleProvider(new ISampleProvider[] { left, right }, 2);
        stereo.ConnectInputToOutput(0, 0); // Left noise -> Left Ear
        stereo.ConnectInputToOutput(1, 1); // Right noise -> Right Ear

        return stereo;
    }

    public static ISampleProvider CreateStereoBrown(int sampleRate = 44100, float amplitude = 0.05f)
    {
        // Two distinct instances = independent random walks
        var left = new BrownNoiseProvider(sampleRate) { Amplitude = amplitude };
        var right = new BrownNoiseProvider(sampleRate) { Amplitude = amplitude };

        var stereo = new MultiplexingSampleProvider(new ISampleProvider[] { left, right }, 2);
        stereo.ConnectInputToOutput(0, 0); // Left noise -> Left Ear
        stereo.ConnectInputToOutput(1, 1); // Right noise -> Right Ear

        return stereo;
    }
}
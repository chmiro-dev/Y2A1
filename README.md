# Binaural Soundscape & Entrainment Engine

A high-performance C# / NAudio console application designed for custom audio generation and brainwave entrainment. The engine generates real-time, high-fidelity stereo audio with support for static and dynamic binaural beats, isochronic pulses, and decorrelated ambient noise.

---

## Features

* **Dynamic Frequency Ramping:** Smoothly interpolate binaural frequencies over time (e.g., sliding from active Alpha states down to deep Delta sleep over a set duration).
* **Multi-Layer Audio Synthesis:** Mix multiple independent layers simultaneously:
* Up to two static binaural beat layers.
* One dynamic ramped binaural beat layer.
* One isochronic pulse layer with phase offset control.
* Decorrelated stereo ambient noise layers (Pink or Brown noise).



* **Perceptual Loudness Calibration:** Tuned carrier pitches to account for human equal-loudness contours (Fletcher-Munson effect).
* **Low-Latency WASAPI Output:** Real-time audio processing driven by NAudio.
* **Flexible Command-Line Interface:** Fully configurable via command-line flags.

---

## Prerequisites

* **Operating System:** Windows 10/11 (uses WASAPI audio output).
* **SDK:** [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or higher.
* **Audio Output:** Stereo headphones or ear-buds (required for binaural beat perception).

---

## Quick Start

1. **Clone the Repository:**
```bash
git clone https://github.com/your-username/your-repo-name.git
cd your-repo-name

```


2. **Build the Project:**
```bash
dotnet build

```


3. **Run a Default Soundscape:**
```bash
dotnet run

```



---

## Usage & CLI Options

The engine accepts flags to configure soundscape layers, frequencies, durations, and volume levels. Volumes can be specified as percentages (e.g., `5%`) or decimals (e.g., `0.05`).

### Frequency Sub-Bands

You can use sub-band names directly in CLI arguments:

| Band Key | Beat Frequency (Hz) | Associated State |
| --- | --- | --- |
| `delta-low` | 1.2 Hz | Deep restorative sleep |
| `delta-high` | 3.0 Hz | Deep sleep, dreamless rest |
| `theta-low` | 5.0 Hz | Deep meditation, hypnagogia |
| `theta-high` | 7.0 Hz | Relaxation, REM sleep |
| `alpha-low` | 9.0 Hz | Light relaxation, calm focus |
| `alpha-mid` | 10.5 Hz | Flow state, creative visualization |
| `alpha-high` | 12.0 Hz | Alert relaxation |
| `beta-low` | 15.0 Hz | Active thinking, focus |
| `beta-high` | 22.0 Hz | High arousal, problem solving |
| `gamma-low` | 40.0 Hz | Peak cognition, information processing |

---

### Command-Line Arguments Reference

#### Dynamic Ramp Layer

* `--ramp-start <band>`: Starting frequency sub-band (e.g., `alpha-mid`).
* `--ramp-end <band>`: Target frequency sub-band (e.g., `delta-low`).
* `--ramp-time <duration>`: Duration of the frequency slide (e.g., `15m`, `30s`, `120s`).
* `--ramp-pitch <Hz>`: Carrier frequency pitch in Hz (Default: `180`).
* `--ramp-vol <vol>`: Layer gain level (Default: `12%`).

#### Static Binaural Layers

* `--binaural1 <band>`: Sub-band for Binaural Layer 1.
* `--bin1-pitch <Hz>`: Carrier frequency in Hz (Default: `180`).
* `--bin1-vol <vol>`: Gain level (Default: `10%`).
* `--binaural2 <band>`: Sub-band for Binaural Layer 2.
* `--bin2-pitch <Hz>`: Carrier frequency in Hz (Default: `120`).
* `--bin2-vol <vol>`: Gain level (Default: `8%`).

#### Isochronic Layer

* `--iso <band>`: Sub-band for Isochronic pulse.
* `--iso-pitch <Hz>`: Pulse carrier frequency in Hz (Default: `220`).
* `--iso-vol <vol>`: Gain level (Default: `8%`).

#### Ambient Noise Layer

* `--noise <type>`: Ambient noise type (`pink` or `brown`).
* `--noise-vol <vol>`: Gain level (Default: `5%`).

---

## Usage Examples

### 1. Sleep Descent Ramp (15-Minute Alpha to Delta Drop)

Gently drags brainwave activity down from relaxed awareness into deep sleep over 15 minutes, layered with soft brown noise:

```bash
dotnet run -- --ramp-start alpha-mid --ramp-end delta-low --ramp-time 15m --ramp-pitch 140 --noise brown --noise-vol 5%

```

### 2. High-Focus Alpha/Gamma Blend

Combines a static 40 Hz Gamma binaural layer with pink noise for deep focus:

```bash
dotnet run -- --binaural1 gamma-low --bin1-pitch 240 --bin1-vol 8% --noise pink --noise-vol 3%

```

### 3. Quick Test Ramp (30-Second Beta to Theta)

Demonstrates the frequency slide mechanism in a short time frame:

```bash
dotnet run -- --ramp-start beta-low --ramp-end theta-low --ramp-time 30s --ramp-pitch 200 --noise pink --noise-vol 3%

```

---

## License

This project is licensed under the [MIT License](LICENSE) - see the LICENSE file for details.
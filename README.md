# REPO SoundBoard Mod

## Description
REPO SoundBoard Mod is a customizable soundboard application that allows you to play your own sound buttons with ease.

## Features
- Add and play custom sound buttons.
- Hotkey support for quick sound playback.
- Supports `.wav` audio files.
- Adjustable (local) volume for each sound button.

## Usage
Sound buttons can be configured using a JSON configuration file named `soundboard-config.json` located in the game's directory.

### Locating the Game Directory
You can find the game directory by navigating to:
`Steam > R.E.P.O > Settings > Manage > Search Local Files`

### Configuration File Structure
Below is an example of the `soundboard-config.json` file structure. Make sure to keep it a valid JSON Syntax.

#### Important Notes:
- The `Path` to the audio file has to be absolute (meaning it has to start from C:\\).
- The `Volume` is a value from 0 to 1 and adjusts the volume only for you.
- For the names of `Keys` refer to [Unity KeyCode documentation](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/KeyCode.html).

```json
{
    "SoundBoard": {
        "StopHotkey": {
            "Keys": ["H"]
        },
        "SoundButtons": [
            {
                "Path": "C:\\Path\\To\\Your\\Clip.wav",
                "Volume": 0.7,
                "Hotkey": {
                    "Keys": ["LeftControl", "Alpha1"]
                }
            },
            {
                "Path": "C:\\WhereEver\\OtherClip.wav",
                "Volume": 0.4,
                "Hotkey": {
                    "Keys": ["Keypad0"]
                }
            }
        ]
    }
}
```

## Converting to `.wav` Format
If your sound file is not in `.wav` format, you can convert it using the following methods:

### Online Tools
Use online converters to easily transform `.mp3` or other formats into `.wav`.

### Local Conversion
To convert locally, follow these steps:

1. Download and install `ffmpeg` from [ffmpeg.org](https://www.ffmpeg.org/download.html).
2. Open a terminal window (press `Win + R`, type `cmd`, and press Enter).
3. Navigate to the directory containing your sound file:
   ```bash
   cd C:\path\to\soundbutton\
   ```
4. Convert the file using the following command:
   ```bash
   ffmpeg -i <inputfile.mp3> <outputfile.wav>
   ```
   Replace `<inputfile.mp3>` with your file name and `<outputfile.wav>` with the desired output name.
5. The `.wav` file will be created in the same directory.

## Troubleshooting
- Ensure the config file exists in your game folder and has the correct name.
- Ensure the file paths in the configuration file are absolute (starting from C:\\) and correctly formatted.
- Verify that the `.wav` files are not corrupted and can be played in other media players.
- Check that the keys for the hotkey have the correct names.

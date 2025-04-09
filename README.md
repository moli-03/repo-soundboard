# REPO SoundBoard Mod

A customizable soundboard mod for REPO that lets you play custom sounds through proximity voice chat using hotkeys.

## Table of Contents
- [REPO SoundBoard Mod](#repo-soundboard-mod)
	- [Table of Contents](#table-of-contents)
	- [Quick Start](#quick-start)
	- [Features](#features)
		- [Core Features](#core-features)
		- [Supported Formats](#supported-formats)
		- [Planned Features](#planned-features)
	- [Installation](#installation)
		- [Config Location](#config-location)
		- [Configuration File Structure](#configuration-file-structure)
			- [Important Notes](#important-notes)
	- [Video Format Support](#video-format-support)
	- [Troubleshooting](#troubleshooting)
		- [Common Issues](#common-issues)
		- [Need Help?](#need-help)

## Quick Start
1. Install the mod via Thunderstore
2. Install [ffmpeg](https://www.ffmpeg.org/download.html) if you plan to use video files
3. Create/edit `Moli.REPOSoundBoard.json` in your BepInEx config folder
4. Add your sound buttons with hotkeys
5. Use the configured hotkeys in-game to play sounds

## Features
### Core Features
- Custom sound button support with hotkey bindings
- Per-sound volume control (locally)
- Proximity-based audio playback
- Global stop hotkey

### Supported Formats
- **Audio**: `.wav`, `.mp3`
- **Video** (requires ffmpeg): `.aiff`, `.avi`, `.mov`, `.mp4`, `.ogg`, `.webm`

### Planned Features
- [x] More format support
- [ ] In-game UI for sound board management
- [ ] Remote volume control
- [ ] Automatic cleanup of cached video audio

## Installation
1. Install via Thunderstore App or manually place in BepInEx/plugins
2. For video support: Install [ffmpeg](https://www.ffmpeg.org/download.html)
3. Launch game once to generate config file
4. Configure your soundboard in `Moli.REPOSoundBoard.json`

### Config Location
- Thunderstore: `Thunderstore App > R.E.P.O > (profile) > Edit config`
- Manual: `%appdata%\Thunderstore Mod Manager\DataFolder\REPO\profiles\(profile)\BepInEx\config`

### Configuration File Structure
Below is an example of the `Moli.REPOSoundBoard.json` file structure.

#### Important Notes
- The `Path` to the file has to be absolute (= start from e.g. C:\\ or D:\\) and the double \\\\ are required in the path.
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
                "Path": "C:\\WhereEver\\OtherClip.mp4",
                "Volume": 0.4,
                "Hotkey": {
                    "Keys": ["Keypad0"]
                }
            }
        ]
    }
}
```

## Video Format Support
When using video files as sound sources:
- Files are automatically converted to `.wav` format
- Converted files are cached as `audio_<clip_name>.wav` in the same directory as the video.
- Conversion happens only once per file
- Delete the cached `.wav` file manually if you update the source video

## Troubleshooting
### Common Issues
- Config file missing or misnamed
- Incorrect file paths (must be absolute, using double backslashes)
- Corrupted audio/video files
- Incorrect hotkey names (check [Unity KeyCode reference](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/KeyCode.html))

### Need Help?
Open an issue on our [GitHub repository](https://github.com/moli-03/repo-soundboard/issues) for:
- Bug reports
- Feature requests
- General support

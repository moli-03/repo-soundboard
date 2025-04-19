# REPO SoundBoard Mod

A customizable soundboard mod for REPO that lets you play custom sounds through proximity voice chat using hotkeys.

## Table of Contents
- [REPO SoundBoard Mod](#repo-soundboard-mod)
	- [Table of Contents](#table-of-contents)
	- [Quick Start](#quick-start)
	- [Features](#features)
      - [Supported Formats](#supported-formats)
    - [Changing UI Hotkey](#changing-ui-hotkey)
	- [Troubleshooting](#troubleshooting)
		- [Need Help?](#need-help)

## Quick Start
1. Install the mod via Thunderstore
2. Install [ffmpeg](https://www.ffmpeg.org/download.html) if you plan to use video files
3. Start the game and press `F4` to open the sound board menu (key changeable in the config file)
4. Add your sound buttons with hotkeys
5. Make sure to save them on the left as the auto save could not work with crashes
6. Use the configured hotkeys in-game to play sounds

## Features
- Custom sound button support with hotkey bindings
- Per-sound volume control
- Simple UI to manage sound buttons
- Enable/Disable sound buttons and the entire sound board

### Supported Formats
- **Audio**: `.wav`, `.mp3`, `.aiff`
- **Video** (requires ffmpeg): `.avi`, `.mov`, `.mp4`, `.ogg`, `.webm`

## Changing UI Hotkey
If you want to change the hotkey to toggle the UI you can do that in the config file directly.
You can find the config file in thunderstore under `R.E.P.O > (your profile) > Edit config` and there
choose the `BepInEx/config/Moli.REPOSoundboard.json` file and edit the hotkey there.

## Need Help?
Open an issue on our [GitHub repository](https://github.com/moli-03/repo-soundboard/issues) for:
- Bug reports
- Feature requests
- General support

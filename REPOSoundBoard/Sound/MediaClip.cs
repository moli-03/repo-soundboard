using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace REPOSoundBoard.Sound
{

    public class MediaClip
    {
        private readonly string _source;
        public bool IsLoaded { get; private set; }
        public bool FailedToLoad { get; private set; }
        public AudioClip AudioClip { get; private set; }
        private static readonly string[] VideoExtensions = { ".mp4", ".webm", ".avi", ".mov", ".ogg" };
        private string _videoAudioPath;

        public MediaClip(string source)
        {
            this._source = source;
            this.IsLoaded = false;
            this.FailedToLoad = false;
            
            // Unique path in case a video has to be converted
            _videoAudioPath = Path.Combine(Path.GetDirectoryName(this._source), $"audio_{Path.GetFileName(_source)}.wav");
        }

        private static AudioType GetAudioTypeFromExtension(string file)
        {
            string extension = Path.GetExtension(file).ToLowerInvariant();
            return extension switch
            {
                ".wav" => AudioType.WAV,
                ".mp3" => AudioType.MPEG,
                ".aiff" => AudioType.AIFF,
                _ => AudioType.UNKNOWN
            };
        }

        private bool IsVideoFile()
        {
            var extension = Path.GetExtension(_source).ToLowerInvariant();
            return VideoExtensions.Contains(extension);
        }

        public IEnumerator Load()
        {
            if (IsVideoFile())
            {
                yield return LoadVideo();
            }
            else
            {
                yield return LoadAudio($"file:///{_source}");
            }
        }

        private IEnumerator LoadVideo()
        {
            if (!File.Exists(_videoAudioPath))
            {
                REPOSoundBoard.Instance.LOG.LogInfo($"Found video. Extracting audio to {_videoAudioPath}...");
                if (!AudioExtractor.ExtractAudioFromVideo(this._source, _videoAudioPath))
                {
                    this.IsLoaded = false;
                    this.FailedToLoad = true;
                    yield break;
                }
                REPOSoundBoard.Instance.LOG.LogInfo($"Audio successfully extracted");
            }
            
            yield return LoadAudio($"file:///{_videoAudioPath}");
        }
        
        private IEnumerator LoadAudio(string source)
        {
            AudioType audioType = GetAudioTypeFromExtension(source);

            if (audioType == AudioType.UNKNOWN)
            {
                REPOSoundBoard.Instance.LOG.LogError($"Unsupported media format for file: {_source}");
                yield break;
            }

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(source, audioType))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    var audioClip = DownloadHandlerAudioClip.GetContent(www);
                    audioClip.name = Path.GetFileName(this._source);
                    var convertedClip = SoundConverter.ConvertStereoToMono(audioClip);

					if (convertedClip == null) {
                    	this.IsLoaded = false;
                    	this.FailedToLoad = true;
						yield break;
					}

					this.AudioClip = convertedClip;
                    this.IsLoaded = true;
                    this.FailedToLoad = false;
                }
                else
                {
                    REPOSoundBoard.Instance.LOG.LogError("Failed to load media: " + www.error + ". Path: " + source);
                    this.IsLoaded = false;
                    this.FailedToLoad = true;
                }
            }
        }
    }
}
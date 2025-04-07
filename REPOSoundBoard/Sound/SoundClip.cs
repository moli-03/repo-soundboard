using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace REPOSoundBoard.Sound
{
    public class SoundClip
    {
        private readonly string _source;
        public bool IsLoaded { get; private set; }
        public AudioClip AudioClip { get; private set; }
        
        public SoundClip(string source)
        {
            this._source = source;
            this.IsLoaded = false;

            REPOSoundBoard.Instance.StartCoroutine(this.Load());
        }

        private IEnumerator Load()
        {
            string source = $"file:///{this._source}";
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(source, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    var audioClip = DownloadHandlerAudioClip.GetContent(www);
                    this.AudioClip = SoundConverter.ConvertStereoToMono(audioClip);
                    this.IsLoaded = true;
                }
                else
                {
                    REPOSoundBoard.Instance.LOG.LogError("Failed to load sound button: " + www.error + ". Path of the sound button: " + this._source);
                }
            }
        }
    }
}
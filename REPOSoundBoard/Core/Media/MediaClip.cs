using System;
using System.Collections;
using System.IO;
using REPOSoundBoard.Core.Exceptions;
using REPOSoundBoard.Core.Media.Converter;
using UnityEngine;
using UnityEngine.Networking;

namespace REPOSoundBoard.Core.Media
{
    public class MediaClip
    {
        public enum MediaClipState
        {
            Idle,
            Converting,
            Converted,
            FailedToConvert,
            Loading,
            Loaded,
            FailedToLoad
        }

        public MediaClipState State;
        public string StateMessage = string.Empty;
        public string OriginalPath;
        public AudioClip AudioClip;
        
        private readonly string _cachePath;
        private readonly string _cacheFileName;
        private bool _isConverted;
        
        
        public MediaClip(string path)
        {
            this.State = MediaClipState.Idle;
            this.StateMessage = "Waiting for instructions...";
            this.OriginalPath = path;
            this._cacheFileName = CacheFileHelper.GetCacheFileName(path, ".wav");
            this._cachePath = CacheFileHelper.GetFullCachePath(this._cacheFileName);
            this._isConverted = CacheFileHelper.ExistsInCache(this._cacheFileName);
        }

        public IEnumerator Load()
        {
            if (this.State == MediaClipState.FailedToConvert || this.State == MediaClipState.FailedToLoad || this.State == MediaClipState.Loaded)
            {
                yield break;
            }
            
            // Check if the file even exists
            if (!File.Exists(this.OriginalPath))
            {
                this.State = MediaClipState.FailedToLoad;
                this.StateMessage = "Failed to load. Could not find file.";
                yield break;
            }
            
            // Convert it
            if (!this._isConverted)
            {
                yield return this.Convert();
                
                // If that failed we give up :)
                if (!this._isConverted)
                {
                    yield break;
                }
            }
            
            this.State = MediaClipState.Loading;
            this.StateMessage = "Loading clip...";
            
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(this._cachePath, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    this.AudioClip = DownloadHandlerAudioClip.GetContent(www);
                    this.State = MediaClipState.Loaded;
                    this.StateMessage = "Sound button loaded successfully.";
                }
                else
                {
                    REPOSoundBoard.Logger.LogError("Failed to load media: " + www.error + ". Path: " + this.OriginalPath + ". Cache Path: " + this._cachePath);
                    this.State = MediaClipState.FailedToLoad;
                    this.StateMessage = "Failed to load. Check logs for more information.";
                }
            }
        }

        public IEnumerator Convert()
        {
            this.State = MediaClipState.Converting;
            var converter = MediaConverterFactory.GetConverterForFile(this.OriginalPath);

            if (converter == null)
            {
                this.State = MediaClipState.FailedToConvert;
                this.StateMessage = "Failed to convert to .wav. Invalid file format.";
                REPOSoundBoard.Logger.LogWarning($"Unsupported file format: {this.OriginalPath}");
                yield break;
            }

            // Make sure the cache dir exists
            CacheFileHelper.EnsureCacheDirectoryExists();
            
            try
            {
                converter.Convert(this.OriginalPath, this._cachePath, ConversionOptions.Default);
                this.State = MediaClipState.Converted;
                this.StateMessage = "Successfully converted to .wav. Waiting to load...";
                
                this._isConverted = true;
            }
            catch (AudioConversionException ex)
            {
                this.State = MediaClipState.FailedToConvert;
                this.StateMessage = ex.Message;
                REPOSoundBoard.Logger.LogError($"Failed to convert to .wav. Error {ex.Message}. File: {this.OriginalPath}");
            }
            catch (Exception ex)
            {
                this.State = MediaClipState.FailedToConvert;
                this.StateMessage = $"Failed to convert to .wav. Check logs for more information.";
                REPOSoundBoard.Logger.LogError($"Failed to convert to .wav. Error {ex.Message}. File: {this.OriginalPath}");
            }
        }
    }
}
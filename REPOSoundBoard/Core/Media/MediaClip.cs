﻿using System;
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
            Created,
            FailedToLoad,
            Converting,
            FailedToConvert,
            Loaded
        }

        public MediaClipState State;
        public string OriginalPath;
        public AudioClip AudioClip;
        
        private readonly string _cachePath;
        private bool _isConverted;
        
        
        public MediaClip(string path)
        {
            this.State = MediaClipState.Created;
            this.OriginalPath = path;
            this._cachePath = CacheFileHelper.GetCacheFilePath(path);
            this._isConverted = CacheFileHelper.ExistsInCache(this.OriginalPath);
        }

        public IEnumerator Load()
        {
            if (this.State == MediaClipState.FailedToConvert || this.State == MediaClipState.FailedToLoad)
            {
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
            
            
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(this._cachePath, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    this.AudioClip = DownloadHandlerAudioClip.GetContent(www);
                    this.State = MediaClipState.Loaded;
                }
                else
                {
                    REPOSoundBoard.Logger.LogError("Failed to load media: " + www.error + ". Path: " + this.OriginalPath + ". Cache Path: " + this._cachePath);
                    this.State = MediaClipState.FailedToLoad;
                }
            }
        }

        public IEnumerator Convert()
        {
            var converter = MediaConverterFactory.GetConverterForFile(this.OriginalPath);

            if (converter == null)
            {
                this.State = MediaClipState.FailedToConvert;
                REPOSoundBoard.Logger.LogWarning($"Unsupported file format: {this.OriginalPath}");
                yield break;
            }

            try
            {
                this.State = MediaClipState.Converting;
                CacheFileHelper.EnsureCacheDirectoryExists();
                converter.Convert(this.OriginalPath, this._cachePath, ConversionOptions.Default);
                
                REPOSoundBoard.Logger.LogInfo($"Converted and cached {this.OriginalPath} to {this._cachePath}");
                this._isConverted = true;
            }
            catch (AudioConversionException ex)
            {
                this.State = MediaClipState.FailedToConvert;
                REPOSoundBoard.Logger.LogError(
                    $"Failed to convert to .wav. Error {ex.Message}. File: {this.OriginalPath}");
            }
            catch (Exception ex)
            {
                this.State = MediaClipState.FailedToConvert;
                REPOSoundBoard.Logger.LogError(
                    $"Failed to convert to .wav. Error {ex.Message}. File: {this.OriginalPath}");
            }
        }
    }
}
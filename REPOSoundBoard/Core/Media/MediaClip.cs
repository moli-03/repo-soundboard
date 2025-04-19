using System.Collections;
using REPOSoundBoard.Core.Exceptions;
using REPOSoundBoard.Core.Media.Converter;

namespace REPOSoundBoard.Core.Media
{
	using System;
	using System.IO;
	using System.Threading;
	using UnityEngine;
	using UnityEngine.Networking;

	public class MediaClip
	{
    	#region Enums
    
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
    
    	#endregion
    
    	#region Public Properties
    
    	public MediaClipState State { get; private set; }
    	public string StateMessage { get; private set; }
    	public string OriginalPath { get; private set; }
    	public AudioClip AudioClip { get; private set; }
    	public bool IsReady => State == MediaClipState.Loaded;
    	public bool HasError => State == MediaClipState.FailedToConvert || State == MediaClipState.FailedToLoad;
    
    	#endregion
    
    	#region Private Fields
    
    	private readonly string _cachePath;
    	private readonly string _cacheFileName;
    	private bool _isConverted;
    	private bool _conversionComplete;
    	private Exception _conversionException;
    
    	#endregion
    
    	#region Constructor
    
    	public MediaClip(string path)
    	{
        	OriginalPath = path;
        	_cacheFileName = CacheFileHelper.GetCacheFileName(path, ".wav");
        	_cachePath = CacheFileHelper.GetFullCachePath(_cacheFileName);
        	_isConverted = CacheFileHelper.ExistsInCache(_cacheFileName);
        
        	SetState(MediaClipState.Idle, "Waiting for instructions...");
    	}
    
    	#endregion
    
    	#region Public Methods
	    
	    
	    public void DeleteCacheFile()
	    {
		    CacheFileHelper.DeleteFromCache(this._cacheFileName);
		    SetState(MediaClipState.Idle, "Cache file deleted. Waiting for conversion to start...");
	    }
    
    	/// <summary>
    	/// Loads the audio clip, converting it first if necessary
    	/// </summary>
    	public IEnumerator Load()
    	{
        	// Skip if already in a terminal state
        	if (HasError || State == MediaClipState.Loaded)
        	{
            	yield break;
        	}
        
        	// Check if the file exists
        	if (!File.Exists(OriginalPath))
        	{
            	SetState(MediaClipState.FailedToLoad, "Failed to load. Could not find file.");
            	yield break;
        	}
        
        	// Convert it if needed
        	if (!_isConverted)
        	{
            	yield return Convert();
            
            	// If conversion failed, abort
            	if (!_isConverted)
            	{
                	yield break;
            	}
        	}
        
        	// Load the audio clip
        	yield return LoadAudioClip();
    	}
    
    	#endregion
    
    	#region Private Methods
    
    	/// <summary>
    	/// Sets the state and message for this media clip
    	/// </summary>
    	private void SetState(MediaClipState state, string message)
    	{
        	State = state;
        	StateMessage = message;
    	}
    
    	/// <summary>
    	/// Converts the original file to a WAV format using a background thread
    	/// </summary>
    	private IEnumerator Convert()
    	{
        	SetState(MediaClipState.Converting, "Converting media file...");
        	_conversionComplete = false;
        	_conversionException = null;
        
        	// Start the conversion in a separate thread
        	ThreadPool.QueueUserWorkItem(state => 
        	{
            	try
            	{
                	var converter = MediaConverterFactory.GetConverterForFile(OriginalPath);
                
                	if (converter == null)
                	{
                    	// We'll handle this case on the main thread
                    	return;
                	}
                
                	// Make sure the cache directory exists
                	CacheFileHelper.EnsureCacheDirectoryExists();
                
                	// Perform the conversion on the background thread
                	converter.Convert(OriginalPath, _cachePath, ConversionOptions.Default);
                
                	// Mark as successfully converted
                	_isConverted = true;
            	}
            	catch (Exception ex)
            	{
                	// Store the exception to be handled on the main thread
                	_conversionException = ex;
            	}
            	finally
            	{
                	// Signal that background work is complete
                	_conversionComplete = true;
            	}
        	});
        
        	// Wait for the background thread to complete
        	while (!_conversionComplete)
        	{
            	yield return null;
        	}
        
        	// Now back on the main thread, update states based on results
        	if (_conversionException != null)
        	{
            	string errorMessage = _conversionException is AudioConversionException 
                	? _conversionException.Message 
                	: "Failed to convert to .wav. Check logs for more information.";
                
            	SetState(MediaClipState.FailedToConvert, errorMessage);
            
            	string logMessage = $"Failed to convert to .wav. Error {_conversionException.Message}. File: {OriginalPath}";
            	REPOSoundBoard.Logger.LogError(logMessage);
            
            	yield break;
        	}
        
        	// Check if converter was available
        	if (MediaConverterFactory.GetConverterForFile(OriginalPath) == null)
        	{
            	SetState(MediaClipState.FailedToConvert, "Failed to convert to .wav. Invalid file format.");
            	REPOSoundBoard.Logger.LogWarning($"Unsupported file format: {OriginalPath}");
            	yield break;
        	}
        
        	// Success path
        	SetState(MediaClipState.Converted, "Successfully converted to .wav. Waiting to load...");
    	}
    
    	/// <summary>
    	/// Loads the converted audio clip
    	/// </summary>
    	private IEnumerator LoadAudioClip()
    	{
        	SetState(MediaClipState.Loading, "Loading clip...");
        
        	using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(_cachePath, AudioType.WAV))
        	{
            	yield return www.SendWebRequest();
            
            	if (www.result == UnityWebRequest.Result.Success)
            	{
                	AudioClip = DownloadHandlerAudioClip.GetContent(www);
                	SetState(MediaClipState.Loaded, "Sound button loaded successfully.");
            	}
            	else
            	{
                	string errorMessage = $"Failed to load media: {www.error}. Path: {OriginalPath}. Cache Path: {_cachePath}";
                	REPOSoundBoard.Logger.LogError(errorMessage);
                	SetState(MediaClipState.FailedToLoad, "Failed to load. Check logs for more information.");
            	}
        	}
    	}
    
    	#endregion
	}
}
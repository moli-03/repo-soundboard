using System;
using UnityEngine;

namespace REPOSoundBoard.Sound
{
    public class SoundConverter
    {
        public static AudioClip ConvertStereoToMono(AudioClip stereoClip)
        {
            // Check if the clip is already mono, return it if so
            if (stereoClip.channels == 1)
            {
                return stereoClip;
            }
    
            // Get stereo clip data
            float[] stereoData = new float[stereoClip.samples * stereoClip.channels];
            stereoClip.GetData(stereoData, 0);
    
            // Create mono data array (half the size of stereo)
            int monoSamples = stereoClip.samples;
            float[] monoData = new float[monoSamples];
    
            // Average the stereo channels to create mono
            for (int i = 0; i < monoSamples; i++)
            {
                // Average the left and right channels (stereoData[i*2] and stereoData[i*2+1])
                monoData[i] = (stereoData[i * 2] + stereoData[i * 2 + 1]) * 0.5f;
            }
    
            // Create a new mono AudioClip
            try
            {
                AudioClip monoClip = AudioClip.Create(
                    stereoClip.name + " (Mono)",
                    monoSamples,
                    1, // 1 channel for mono
                    stereoClip.frequency,
                    false // Not 3D
                );

                // Set the mono data into the new clip
                monoClip.SetData(monoData, 0);

                return monoClip;
            }
            catch (Exception e)
            {
                REPOSoundBoard.Instance.LOG.LogWarning($"Failed to convert clip {stereoClip.name} to mono. Error: {e.Message}");
                return null;
            }
        }
    }
}
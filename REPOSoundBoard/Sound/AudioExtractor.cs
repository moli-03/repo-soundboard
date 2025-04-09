using System;
using System.Diagnostics;

namespace REPOSoundBoard.Sound
{
    public static class AudioExtractor
    {

        private static bool IsFfmpegInstalled()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        return false;
                    }
                    
                    process.WaitForExit(2000); // Wait up to 2 seconds
                    return process.ExitCode == 0 || process.ExitCode == 1; // 1 is common for -version
                }
            }
            catch
            {
                return false;
            } 
        }


        public static bool ExtractAudioFromVideo(string videoPath, string outputAudioPath)
        {
            if (!IsFfmpegInstalled())
            {
                REPOSoundBoard.Instance.LOG.LogError("Cannot extract audio from video: ffmpeg is not installed. You can download ffmpeg from https://www.ffmpeg.org/download.html");
                return false;
            }
            
            
            // Create process to run FFmpeg
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{videoPath}\" -vn -acodec pcm_s16le -ar 48000 -ac 1 \"{outputAudioPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch (Exception e)
            {
                REPOSoundBoard.Instance.LOG.LogError($"Failed to extract audio with ffmpeg: {e.Message}");
                return false;
            }
        }
    }
}
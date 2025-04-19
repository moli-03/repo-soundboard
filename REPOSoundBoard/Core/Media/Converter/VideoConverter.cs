using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using REPOSoundBoard.Core.Exceptions;

namespace REPOSoundBoard.Core.Media.Converter
{
    public class VideoConverter : IMediaConverter
    {
        private static readonly string[] _supportedExtensions = { ".mp4", ".webm", ".avi", ".mov", ".ogg" };
        private static bool? _isFfmpegInstalled;
        private static TimeSpan _ffmpegTimeout = TimeSpan.FromSeconds(30);
        
        public static bool IsCompatible(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            string extension = Path.GetExtension(path)?.ToLowerInvariant();
            return !string.IsNullOrEmpty(extension) && _supportedExtensions.Contains(extension);
        }
        
        private string BuildFfmpegArguments(string sourcePath, string targetPath, ConversionOptions options)
        {
            return $"-i \"{sourcePath}\" -vn -acodec {options.AudioCodec} -ar {options.SampleRate} -ac {options.ChannelCount} \"{targetPath}\"";
        }

        public void Convert(string sourcePath, string targetPath, ConversionOptions options)
        {
            if (!IsFfmpegInstalled())
            {
                throw new AudioConversionException("Failed to convert video file. ffmpeg is not installed.");
            }
            
            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException($"Source file {sourcePath} not found");
            }
            
            RunFfmpegProcess(sourcePath, targetPath, options);
        }
        
        private void RunFfmpegProcess(string sourcePath, string targetPath, ConversionOptions options)
        {
            string arguments = BuildFfmpegArguments(sourcePath, targetPath, options);
    
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
    
            using (Process process = new Process { StartInfo = startInfo })
            {
                StringBuilder errorOutput = new StringBuilder();
                process.ErrorDataReceived += (sender, e) => 
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        errorOutput.AppendLine(e.Data);
                    }
                };
    
                if (!process.Start())
                {
                    throw new AudioConversionException("Failed to start FFmpeg process");
                }
    
                process.BeginErrorReadLine();
                process.WaitForExit((int)_ffmpegTimeout.TotalMilliseconds);
    
                if (!process.HasExited)
                {
                    try { process.Kill(); } catch { /* Ignore errors on kill */ }
                    throw new AudioConversionException("FFmpeg process timed out");
                }
                
                if (process.ExitCode != 0)
                {
                    string errorMessage = errorOutput.ToString();
                    REPOSoundBoard.Logger.LogError($"FFmpeg exited with error code {process.ExitCode}. Error: {errorMessage}");
                    throw new AudioConversionException($"FFmpeg failed with exit code {process.ExitCode}: {errorMessage}");
                }
    
                // Verify the output file was created
                if (!File.Exists(targetPath))
                {
                    throw new AudioConversionException("FFmpeg completed but the output file was not created");
                }
            }
        }
        
        
        private static bool IsFfmpegInstalled()
        {
            if (!_isFfmpegInstalled.HasValue)
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo()
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
                            _isFfmpegInstalled = false;
                        }
                        else
                        {
                            process.WaitForExit(2000); // Wait up to 2 seconds
                            _isFfmpegInstalled = process.ExitCode == 0 || process.ExitCode == 1; // 1 is common for -version
                        }
                    }
                }
                catch
                {
                    _isFfmpegInstalled = false;
                }
            }

            return _isFfmpegInstalled.Value;
        }
    }
}
using System.IO;
using NAudio.Wave;

namespace REPOSoundBoard.Core.Media.Converter
{
    public class WavConverter : IMediaConverter
    {
        public static bool IsCompatible(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            string extension = Path.GetExtension(path)?.ToLowerInvariant();
            return !string.IsNullOrEmpty(extension) && extension == ".wav";
        }

        public void Convert(string sourcePath, string targetPath, ConversionOptions options)
        {
            using (var reader = new AudioFileReader(sourcePath))
            {
                // Create a resampler if needed
                var resampler = new MediaFoundationResampler(reader, new WaveFormat(options.SampleRate, options.BitsPerSample, options.ChannelCount));
                
                // Set resampler quality (0 = best quality)
                resampler.ResamplerQuality = 60;

                // Write to output file
                WaveFileWriter.CreateWaveFile(targetPath, resampler);
            }
        }
    }
}
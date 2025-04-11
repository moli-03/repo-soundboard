namespace REPOSoundBoard.Core.Media.Converter
{
    public static class MediaConverterFactory
    {

        public static IMediaConverter GetConverterForFile(string path)
        {
            if (WavConverter.IsCompatible(path))
            {
                return new WavConverter();
            }

            if (Mp3Converter.IsCompatible(path))
            {
                return new Mp3Converter();
            }

            if (VideoConverter.IsCompatible(path))
            {
                return new VideoConverter();
            }

            return null;
        }
    }
}
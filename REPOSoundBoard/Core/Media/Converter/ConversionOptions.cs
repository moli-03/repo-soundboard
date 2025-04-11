namespace REPOSoundBoard.Core.Media.Converter
{
    public struct ConversionOptions
    {
        public int SampleRate;
        public int ChannelCount;
        public int BitsPerSample;
        public string AudioCodec;

        public static ConversionOptions Default = new ConversionOptions()
        {
            SampleRate = 48000,
            ChannelCount = 1,
            BitsPerSample = 16,
            AudioCodec = "pcm_s16le",
        };
    }
}
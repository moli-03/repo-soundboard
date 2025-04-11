using System;

namespace REPOSoundBoard.Core.Exceptions
{
    public class AudioConversionException : Exception
    {
        public AudioConversionException(string message) : base(message) { }
    }
}
using System.Threading.Tasks;

namespace REPOSoundBoard.Core.Media.Converter
{
    public interface IMediaConverter
    {
        static bool IsCompatible(string path)
        {
            return false;
        }
        
        void Convert(string sourcePath, string targetPath, ConversionOptions options);
    }
}
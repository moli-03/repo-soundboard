using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BepInEx;

namespace REPOSoundBoard.Core.Media
{
    public static class CacheFileHelper
    {
        private static readonly string CacheDirectory = Path.Combine(Paths.PluginPath, $"Moli-REPOSoundBoard-{REPOSoundBoard.VERSION}/_cache");

        public static string GetFullCachePath(string cacheFileName)
        {
            return Path.Combine(CacheDirectory, cacheFileName);
        }

        public static bool ExistsInCache(string cacheFileName)
        {
            return File.Exists(GetFullCachePath(cacheFileName));
        }

        public static string GetCacheFilePath(string originalFilePath, string cacheExtension = ".cache")
        {
            return Path.Combine(CacheDirectory, GetCacheFileName(originalFilePath, cacheExtension));
        }

        public static void EnsureCacheDirectoryExists()
        {
            if (!Directory.Exists(CacheDirectory))
            {
                Directory.CreateDirectory(CacheDirectory);
            }
        }
        
        public static string GetCacheFileName(string originalFilePath, string cacheExtension = ".cache")
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] pathBytes = Encoding.UTF8.GetBytes(originalFilePath);
                byte[] hashBytes = sha256.ComputeHash(pathBytes);

                // Convert to hex string
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                return hash + cacheExtension;
            }
        }
    }
    
}
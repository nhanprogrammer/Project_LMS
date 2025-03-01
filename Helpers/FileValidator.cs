using System;
using System.IO;
using System.Linq;

namespace Project_LMS.Helpers
{
    public static class FileValidator
    {
        public static bool IsValidFileSize(long fileSize, long maxSize) => fileSize > 0 && fileSize <= maxSize;
        
        public static bool HasValidExtension(string fileName, string[] allowedExtensions)
        {
            if (string.IsNullOrWhiteSpace(fileName) || allowedExtensions == null || allowedExtensions.Length == 0)
                return false;

            string extension = Path.GetExtension(fileName)?.ToLower();
            return !string.IsNullOrEmpty(extension) && allowedExtensions.Contains(extension);
        }
        
        public static bool FileExists(string filePath) => File.Exists(filePath);
        
        public static bool IsEmptyFile(string filePath) => File.Exists(filePath) && new FileInfo(filePath).Length == 0;
        
        public static bool IsValidMimeType(string filePath, string[] allowedMimeTypes)
        {
            if (!File.Exists(filePath) || allowedMimeTypes == null || allowedMimeTypes.Length == 0)
                return false;

            try
            {
                string mimeType = GetMimeType(filePath);
                return allowedMimeTypes.Contains(mimeType);
            }
            catch
            {
                return false;
            }
        }
        
        private static string GetMimeType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);
            return key?.GetValue("Content Type") as string ?? "application/octet-stream";
        }
    }
}


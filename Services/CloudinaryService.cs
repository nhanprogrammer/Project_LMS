using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Project_LMS.Configurations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Project_LMS.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(string base64String);
        Task<string> UploadExcelAsync(string base64String);
        Task<string> UploadDocAsync(string base64String);
        Task<string> UploadDocxAsync(string base64String);
        Task<string> UploadPowerPointAsync(string base64String);
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new Account
            {
                Cloud = config.Value.CloudName,
                ApiKey = config.Value.ApiKey,
                ApiSecret = config.Value.ApiSecret
            };
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(string base64String)
        {
            return await UploadFileAsync(base64String, "jpg", "images", "images_up");
        }

        public async Task<string> UploadExcelAsync(string base64String)
        {
            return await UploadFileAsync(base64String, "xlsx", "files", "files_up");
        }

        public async Task<string> UploadDocAsync(string base64String)
        {
            return await UploadFileAsync(base64String, "doc", "files", "files_up");
        }

        public async Task<string> UploadDocxAsync(string base64String)
        {
            return await UploadFileAsync(base64String, "docx", "files", "files_up");
        }

        public async Task<string> UploadPowerPointAsync(string base64String)
        {
            return await UploadFileAsync(base64String, "pptx", "files", "files_up");
        }

        private async Task<string> UploadFileAsync(string base64String, string fileExtension, string folder, string uploadPreset)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                throw new ArgumentException("Base64 string cannot be null or empty");
            }

            string base64Data = base64String;
            if (base64String.Contains(","))
            {
                var parts = base64String.Split(',');
                if (parts.Length < 2)
                {
                    throw new ArgumentException("Invalid Base64 data URL format");
                }
                base64Data = parts[1];
            }

            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Invalid Base64 string: " + ex.Message);
            }

            using var ms = new MemoryStream(fileBytes);
            var uniqueFileName = $"{Guid.NewGuid()}.{fileExtension}";

            var uploadParams = new AutoUploadParams
            {
                File = new FileDescription(uniqueFileName, ms),
                Folder = folder,
                UploadPreset = uploadPreset
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Failed to upload file: {uploadResult.Error?.Message}");
            }

            return uploadResult.SecureUrl.AbsoluteUri;
        }
    }
}
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
        Task DeleteFileByUrlAsync(string url);
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

        public async Task DeleteFileByUrlAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL không được để trống hoặc null.");
            }

            var resourceType = url.Contains("/raw/upload/") ? "raw" : "image";
            Console.WriteLine($"ResourceType: {resourceType}");

            var publicId = GetPublicIdFromUrl(url, resourceType);
            Console.WriteLine($"Public ID: {publicId}");

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = resourceType == "raw" ? ResourceType.Raw : ResourceType.Image
            };

            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Result != "ok")
            {
                var errorMessage = deletionResult.Error != null ? deletionResult.Error.Message : "Không có thông tin lỗi chi tiết.";
                if (errorMessage.Contains("not found"))
                {
                    Console.WriteLine("File không tồn tại trên Cloudinary, bỏ qua.");
                    return;
                }
                throw new Exception($"Xóa file thất bại: {errorMessage}");
            }
        }

        private string GetPublicIdFromUrl(string url, string resourceType)
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/');

            // Tìm vị trí của "upload" trong URL
            var uploadIndex = Array.IndexOf(segments, "upload");

            // Lấy các đoạn sau "upload", bỏ qua version
            var publicIdSegments = segments.Skip(uploadIndex + 2);
            var publicIdWithExtension = string.Join("/", publicIdSegments);

            // Xử lý public_id dựa trên ResourceType
            string publicId;
            if (resourceType == "raw")
            {
                // Với file raw (xlsx, docx, doc, v.v.), giữ nguyên đuôi file
                publicId = publicIdWithExtension;
            }
            else
            {
                // Với file image (jpg, png, v.v.), bỏ đuôi file
                publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);
                var folder = string.Join("/", publicIdSegments.Take(publicIdSegments.Count() - 1));
                publicId = string.IsNullOrEmpty(folder) ? publicId : $"{folder}/{publicId}";
            }

            return publicId;
        }
    }
}
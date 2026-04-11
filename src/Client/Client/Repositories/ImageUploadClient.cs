using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class ImageUploadClient : IImageUploadClient
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
        };

        public ImageUploadClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UploadAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Image file not found.", filePath);
            }

            var contentType = GetImageContentType(filePath)
                ?? throw new InvalidOperationException("Only image files (jpg, jpeg, png, gif, webp) are allowed.");

            using var form = new MultipartFormDataContent();
            var bytes = await File.ReadAllBytesAsync(filePath).ConfigureAwait(false);
            using var fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            form.Add(fileContent, "image", Path.GetFileName(filePath));

            var uploadUri = new Uri("http://localhost:5000/api/upload");
            var response = await _httpClient.PostAsync(uploadUri, form).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpRequestException($"Image upload failed with status {(int)response.StatusCode}: {error}");
            }

            var payload = await response.Content.ReadFromJsonAsync<UploadImageResponse>(JsonOptions).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(payload?.url))
            {
                throw new InvalidOperationException("Upload response does not contain image url.");
            }

            return payload.url;
        }

        private static string? GetImageContentType(string filePath)
        {
            var ext = Path.GetExtension(filePath)?.ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => null
            };
        }

        private class UploadImageResponse
        {
            public string? url { get; set; }
            public string? publicId { get; set; }
        }
    }
}

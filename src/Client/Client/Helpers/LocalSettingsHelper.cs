using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Helpers
{
    public static class LocalSettingsHelper
    {
        private static readonly string SettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "POSClientSettings");
        private static readonly string SettingsFile = Path.Combine(SettingsFolder, "serverconfig.json");

        private class ServerConfig
        {
            public string Host { get; set; } = "localhost";
            public string Port { get; set; } = "5000";
        }

        private static ServerConfig GetConfig()
        {
            if (File.Exists(SettingsFile))
            {
                string json = File.ReadAllText(SettingsFile);
                return JsonSerializer.Deserialize<ServerConfig>(json) ?? new ServerConfig();
            }
            return new ServerConfig();
        }

        public static string GetServerHost() => GetConfig().Host;
        public static string GetServerPort() => GetConfig().Port;

        public static void SaveServerConfig(string host, string port)
        {
            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
            }

            var config = new ServerConfig { Host = host, Port = port };
            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(config));
        }

        public static string GetGraphQLEndpoint(string host, string port)
        {
            return $"http://{host}:{port}/graphql";
        }

        public static async Task<(bool isSuccess, string message)> TestConnectionAsync(string host, string port)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                var requestBody = new { query = "query { health { status database } }" }; // Yêu cầu trả về status và database
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(GetGraphQLEndpoint(host, port), content);

                // Bước 1: Kiểm tra xem server có phản hồi HTTP 200 OK không
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    using var jsonDoc = JsonDocument.Parse(responseString);
                    var root = jsonDoc.RootElement;

    
                    if (root.TryGetProperty("errors", out _))
                    {
                        return (false, "Máy chủ hoạt động nhưng có lỗi truy vấn dữ liệu.");
                    }

                    if (root.TryGetProperty("data", out var dataElement) &&
                        dataElement.TryGetProperty("health", out var healthElement) &&
                        healthElement.TryGetProperty("status", out var statusElement))
                    {
                        if (statusElement.GetString() == "ok")
                        {
                            return (true, "Kết nối máy chủ và Database thành công!");
                        }
                    }

                    return (false, "Server phản hồi nhưng sai định dạng dữ liệu.");
                }

                return (false, $"Máy chủ phản hồi lỗi: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể kết nối. Kiểm tra lại IP/Port.");
            }
        }
    }
}
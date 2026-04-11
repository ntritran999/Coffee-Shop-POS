using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class GeminiService
    {
        private readonly Kernel _kernel;

        public GeminiService()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddGoogleAIGeminiChatCompletion(
                modelId: "gemma-3-4b-it",
                apiKey: App.Configuration["GEMINI_API_KEY"]
            );
            _kernel = builder.Build();
        }

        public async Task<string> AskAsync(string prompt)
        {
            int maxRetries = 2; // Giảm số lần retry xuống để tránh spam thêm
            int delayMilliseconds = 3000;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var result = await _kernel.InvokePromptAsync(prompt);
                    return result.ToString();
                }
                catch (Exception ex)
                {
                    // Bắt lỗi 429 - Vượt quá giới hạn Request
                    if (ex.Message.Contains("429") || ex.Message.Contains("Too Many Requests"))
                    {
                        // Trả ngay thông báo ra UI để nhân viên thu ngân/người dùng biết đường chờ đợi
                        return "Hệ thống AI đang nhận quá nhiều yêu cầu. Vui lòng đợi khoảng 1 phút rồi thử lại nhé!";
                    }

                    // Bắt lỗi 503 - Server Google bận
                    if (ex.Message.Contains("503") || ex.Message.Contains("Service Unavailable"))
                    {
                        if (i == maxRetries - 1) return "Hệ thống AI hiện đang bảo trì hoặc quá tải. Vui lòng thử lại sau.";
                        await Task.Delay(delayMilliseconds);
                        continue;
                    }

                    // Nếu là lỗi khác
                    return $"Lỗi kết nối AI: Vui lòng kiểm tra lại mạng hoặc API Key.";
                }
            }
            return "Hệ thống AI hiện không phản hồi.";
        }
    }
}

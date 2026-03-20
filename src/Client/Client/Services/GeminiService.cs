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
                modelId: "gemini-2.5-flash",
                apiKey: "AIzaSyA7rOmKWhatypnzyUnWpxjuyeKmxiVP4W0"
            );
            _kernel = builder.Build();
        }

        public async Task<string> AskAsync(string prompt)
        {
            try
            {
                var result = await _kernel.InvokePromptAsync(prompt);
                return result.ToString();
            }
            catch (Exception ex)
            {
                return $"Lỗi kết nối: {ex.Message}";
            }
        }
    }
}

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LLM
{
    public class LLMService
    {
        private Kernel _kernel;

        public LLMService()
        {
            var builder = Kernel.CreateBuilder();

            builder.AddGoogleAIGeminiChatCompletion(
                modelId: "gemini-2.5-flash",
                apiKey: "SERCET_KEY"
            );

            _kernel = builder.Build();
        }


        public async Task<string> AskAsync(string prompt)
        {
            var result = await _kernel.InvokePromptAsync(prompt);
            return result.ToString();
        }
    }
}

using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class ChatViewModel : ObservableObject
    {
        private readonly GeminiService _geminiService;

        public ObservableCollection<ChatMessage> Messages { get; } = new();

        public IAsyncRelayCommand SendMessageCommand { get; }

        private string? _inputText;
        public string? InputText
        {
            get => _inputText;
            set => SetProperty(ref _inputText, value); // Quan trọng: Phải có public set và SetProperty
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value); // Để UI nhận biết khi nào đang load
        }

        public ChatViewModel()
        {
            _geminiService = new GeminiService();
            SendMessageCommand = new AsyncRelayCommand(SendMessageAsync);

            // Tin nhắn chào mừng
            Messages.Add(new ChatMessage
            {
                Text = "Chào bạn! Tôi là trợ lý AI của POS Cà Phê. Tôi có thể giúp gì cho bạn?",
                IsUser = false,
                Time = DateTime.Now.ToString("HH:mm")
            });
        }

        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(InputText) || IsBusy) return;

            var userText = InputText;

            // 1. Thêm tin nhắn của User vào danh sách
            Messages.Add(new ChatMessage
            {
                Text = userText,
                IsUser = true,
                Time = DateTime.Now.ToString("HH:mm")
            });

            // 2. Xóa ô nhập liệu
            InputText = string.Empty;

            // 3. Gọi AI
            IsBusy = true;
            try
            {
                var response = await _geminiService.AskAsync(userText);
                Messages.Add(new ChatMessage
                {
                    Text = response,
                    IsUser = false,
                    Time = DateTime.Now.ToString("HH:mm")
                });
            }
            catch (Exception ex)
            {
                Messages.Add(new ChatMessage
                {
                    Text = "Lỗi: " + ex.Message,
                    IsUser = false,
                    Time = DateTime.Now.ToString("HH:mm")
                });
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

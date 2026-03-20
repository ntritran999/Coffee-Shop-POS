using Client.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Views.Selector
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? UserTemplate { get; set; }
        public DataTemplate? AiTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            if (item is ChatMessage message)
            {
                return message.IsUser ? UserTemplate : AiTemplate;
            }
            return null;
        }
    }
}

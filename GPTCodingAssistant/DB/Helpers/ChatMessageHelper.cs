using OpenAI_API.Chat;

namespace GPTCodingAssistant.DB.Helpers;

public static class ChatMessageHelper
{
    public static int ToDB(this ChatMessageRole role)
    {
#pragma warning disable CS8846 // switch 表达式不会处理属于其输入类型的所有可能值(它并非详尽无遗)。
        return role switch
        {
            var _ when role == ChatMessageRole.System => 0,
            var _ when role == ChatMessageRole.User => 1,
            var _ when role == ChatMessageRole.Assistant => 2,
        };
#pragma warning restore CS8846 // switch 表达式不会处理属于其输入类型的所有可能值(它并非详尽无遗)。
    }

    public static ChatMessageRole ToRole(int role)
    {
        switch (role)
        {
            case 0:
                return ChatMessageRole.System;
            case 1:
                return ChatMessageRole.User;
            case 2:
                return ChatMessageRole.Assistant;
            default:
                throw new ArgumentOutOfRangeException(nameof(role));
        }
    }

    public static OpenAI_API.Chat.ChatMessage ToAI(this ChatMessage message)
    {
        return new OpenAI_API.Chat.ChatMessage
        {
            Content = message.Message,
            Role = ToRole(message.Role),
        };
    }
}

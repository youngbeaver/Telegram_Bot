using SearchFriend.Logger;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace SearchFriend.Entity
{
    public class Bot
    {
        private static TelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;

        public async static Task InitBotClient(string key)
        {
            _botClient = new TelegramBotClient(key);

            if (_botClient == null)
            {
                Console.WriteLine("Произошла ошибка при включении бота");
                Environment.Exit(1);
            }

            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                },
            };

            using var cts = new CancellationTokenSource();

            _botClient.StartReceiving(Update.UpdateHandler, Update.ErrorHandler, _receiverOptions, cts.Token);

            Log.WriteLog(MessageHead.BotStatus, "Бот запущен");
        }

        public static async Task SendAsyncMessageFromUser(string message, long chatId) => await _botClient.SendMessage(chatId, message);
    }
}

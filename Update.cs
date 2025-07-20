using SearchFriend.Command;
using SearchFriend.Entity;
using SearchFriend.Logger;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SearchFriend
{
    public class Update
    {

        public static async Task UpdateHandler(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case Telegram.Bot.Types.Enums.UpdateType.Message:
                        HandlerMessage(update.Message);
                        break;

                    default:
                        Log.WriteLog(MessageHead.NewDefaultMessage, $"Пользователь {update.Message.From.Username} ({update.Message.From.Id}) отправил неподдерживаемый тип");
                        await Bot.SendAsyncMessageFromUser("Я не понимаю вас :(", update.Message.Chat.Id);
                        break;
                }
            }
            catch (Exception ex) 
            {
                Log.WriteError(ex.Message, ex.StackTrace);
            }
        }

        public static async Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {

        }

        private static async Task HandlerMessage(Message message)
        {
            if (String.IsNullOrEmpty(message.Text))
            {
                Log.WriteLog(MessageHead.NewDefaultMessage, $"Пользователь {message.From.Username} ({message.From.Id}) отправил неподдерживаемый тип");
                await Bot.SendAsyncMessageFromUser("Я не понимаю вас :(", message.Chat.Id);
                return;
            }

            switch (message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Text:
                    if (message.Text[0] == '/')
                        CommandHandler.HandlerCommand(message.Text, message);
                        break;

                default:
                    Log.WriteLog(MessageHead.NewDefaultMessage, $"Пользователь {message.From.Username} ({message.From.Id}) отправил неподдерживаемый тип");
                    await Bot.SendAsyncMessageFromUser("Я не понимаю вас :(", message.Chat.Id);
                    break;
            }
        }
    }
}

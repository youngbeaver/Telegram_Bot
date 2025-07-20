using System.Data;
using System.Security.Cryptography;
using System.Text;
using MySqlConnector;
using SearchFriend.Database;
using SearchFriend.Entity;
using SearchFriend.Logger;
using Telegram.Bot.Types;

namespace SearchFriend.Command
{
    public class CommandHandler
    {
        private static readonly List<Entity.Command> _commandList = new List<Entity.Command>()
        {
            new Entity.Command("/start", "Start bot", UserMode.Guest, _handlerStart),
            new Entity.Command("/help", "Help for user", UserMode.Guest, _handlerHelp),
            new Entity.Command("/aboutme", "Get info about me", UserMode.User, _handlerAboutMe),
            new Entity.Command("/createkey", "Create Registration key", UserMode.Moderator, _handlerCreateRegistrationKey),
            new Entity.Command("/deleteaccount", "Delete user account", UserMode.Administator, _handlerDeleteUserAccount),
        };

        public static async Task HandlerCommand(string command, Message message)
        {
            string[] args = command.Split(" ");
            Entity.Command inputCommand = _commandList.Find(command => command.Name == args[0]);

            bool access = await _checkAccessLevel(message, inputCommand);

            if (access == true)
                await inputCommand.Action.Invoke(message, args);              
        }

        private static async Task<bool> _checkAccessLevel(Message message, Entity.Command command) 
        {
            if (command == null)
            {
                await Bot.SendAsyncMessageFromUser("Мне неизвестная такая команда :(", message.From.Id);
                return false;
            }

            if (command.Access == UserMode.Guest)
                return true;

            if (Users.IsUserRegistered(message.From.Id) == true || Users.IsUserRegistered(message.From.Username) == true)
            {
                if (command.Access > Users.GetUserMode(message.From.Username))
                {
                    await Bot.SendAsyncMessageFromUser("Ваш уровень привелегий слишком низкий.", message.From.Id);
                    return false;
                }
            }
            else
            {
                await Bot.SendAsyncMessageFromUser("Для этой команды требуется регистрация.", message.From.Id);
                return false;
            }

            return true;
        }

        private static async Task _handlerStart(Message message, string[] args)
        {
            if (args.Length == 0 || args[1] == null)
            {
                await Bot.SendAsyncMessageFromUser("Вы неверно указали ключ для активации аккаунта", message.From.Id);
                return;
            }

            string registrationKey = args[1].ToString();

            if (Users.IsUserRegistered(message.From.Id) == true || Users.IsUserRegistered(message.From.Username) == true)
            {
                await Bot.SendAsyncMessageFromUser("Вы уже зарегистрированы в сервисе, меню для использования - /menu", message.From.Id);
                return;
            }

            using MySqlCommand command = new MySqlCommand("SELECT `reg_key` FROM `registration_key` WHERE `reg_key`=@registrationKey");
            command.Parameters.AddWithValue("@registrationKey", registrationKey);
            DataTable resultQuary = await MySQL.QuaryReadAsync(command);

            if (resultQuary == null || resultQuary.Rows.Count == 0)
            {
                await Bot.SendAsyncMessageFromUser("Такого ключа не существует", message.From.Id);
                return;
            }

            bool result = await Users.CreateAccount(message);

            if (result == false)
            {
                await Bot.SendAsyncMessageFromUser("Произошла неизвестная ошибка при создании аккаунта", message.From.Id);
                Log.WriteError("Произошла ошибка во время активации аккаунта пользователем", null);
                return;
            }

            await Bot.SendAsyncMessageFromUser("Вы успешно активировали ключ, ваша учетная запись авторизирована", message.From.Id);
            Log.WriteLog(MessageHead.AccountAction, $"Пользователь {message.From.Username} успешно создал свой аккаунт");

            using MySqlCommand commandForDeleteKey = new MySqlCommand("DELETE FROM `registration_key` WHERE `reg_key`=@registrationKey");
            command.Parameters.AddWithValue("@registrationKey", registrationKey);
            await MySQL.QuaryAsync(command);
        }

        private static async Task _handlerCreateRegistrationKey(Message message, string[]? args)
        {
            DateTime dateTime = DateTime.Now;
    
            using MD5 mD5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(dateTime.ToString());
            byte[] hash = mD5.ComputeHash(inputBytes);

            using MySqlCommand cmd = new MySqlCommand("INSERT INTO `registration_key` (`reg_key`) VALUES (@registrationKey)");
            cmd.Parameters.AddWithValue("@registrationKey", Convert.ToHexString(hash));
            await MySQL.QuaryAsync(cmd);

            await Bot.SendAsyncMessageFromUser($"Вы успешно создали ключ для регистрации, можете его переслать: {Convert.ToHexString(hash)}", message.From.Id);
        }

        private static async Task _handlerAboutMe(Message message, string[]? args)
        {

        }

        private static async Task _handlerHelp(Message message, string[]? args)
        {
            UserMode accessUserLevel = Users.GetUserMode(message.From.Username);
            string[] aviableCommand = _commandList.Where(command => command.Access == accessUserLevel).Select(comm => comm.Name).ToArray();
            string[] aviableCommandDescription = _commandList.Where(command => command.Access == accessUserLevel).Select(comm => comm.Description).ToArray();
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < aviableCommand.Length; i++) 
            {
                stringBuilder.Append($"{aviableCommand[i]} - {aviableCommandDescription[i]} \n");
            }

            await Bot.SendAsyncMessageFromUser($"Вам доступны команды: \n {stringBuilder.ToString()}", message.From.Id);
        }

        private static async Task _handlerDeleteUserAccount(Message message, string[]? args)
        {
            if (args[1] == null)
            {
                await Bot.SendAsyncMessageFromUser("Укажите после команды username пользователя", message.From.Id);
                return;
            }

            bool deleteResult = await Users.DeleteUserAccount(message, message.From.Username);

            if (deleteResult == true)
                await Bot.SendAsyncMessageFromUser("Вы успешно удалили аккаунт пользователя. Теперь он потерял доступ по прежнему ключу.", message.From.Id);
        }
    }
}

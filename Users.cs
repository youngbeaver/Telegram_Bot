using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using SearchFriend.Database;
using SearchFriend.Entity;
using SearchFriend.Logger;
using Telegram.Bot.Types;

namespace SearchFriend
{
    public class Users
    {
        private static List<Entity.User> _users = new List<Entity.User>();
        public static async Task InitUsers()
        {
            using MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM `users`");
            DataTable table = await Database.MySQL.QuaryReadAsync(mySqlCommand);

            if (table.Rows.Count <= 0)
            {
                Log.WriteLog(MessageHead.BotStatus, "Отсутствуют данные в таблице users");
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                var user = new Entity.User(
                    Convert.ToInt32(row["id"]),
                    row["first_and_lastname"].ToString(),
                    row["username"].ToString(),
                    Convert.ToInt64(row["telegram_id"]),
                    row["display_name"].ToString() ?? "",
                    Convert.ToUInt16(row["display_age"]),
                    row["display_city"].ToString() ?? "",
                    (UserMode)Enum.Parse(typeof(UserMode), row["user_mode"].ToString())
                );

                _users.Add(user);
            }

            Log.WriteLog(MessageHead.BotStatus, $"Подгружено {_users.Count} пользователей.");
        }

        public static async Task<bool> CreateAccount(Message message)
        {
            string firstAndLastname = $"{message.From.FirstName} {message.From.LastName}";

            try
            {
                using MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO `users` (first_and_lastname, username, telegram_id, user_mode) VALUES (@firstAndLastname, @username, @telegramId, @userMode); SELECT LAST_INSERT_ID();");
                mySqlCommand.Parameters.AddWithValue("@firstAndLastname", firstAndLastname);
                mySqlCommand.Parameters.AddWithValue("@username", message.From.Username);
                mySqlCommand.Parameters.AddWithValue("@telegramId", message.From.Id);
                mySqlCommand.Parameters.AddWithValue("@userMode", UserMode.User);

                DataTable result = await Database.MySQL.QuaryReadAsync(mySqlCommand);

                if (result == null || result.Rows.Count == 0)
                    return false;

                Console.WriteLine(result.Rows[0][0]);

                var user = new Entity.User(Convert.ToInt32(result.Rows[0][0]), firstAndLastname, message.From.Username, message.From.Id);
                _users.Add(user);
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.Message, ex?.StackTrace ?? "");
                return false;
            }

            return true;
        }

        public static async Task<bool> DeleteUserAccount(Message message, string username)
        {
            Entity.User user = _users.Find(user => user.Username == username);

            if (user == null)
                return false;

            _users.Remove(user);

            using MySqlCommand mySqlCommand = new MySqlCommand("DELETE FROM `users` WHERE `username`=@username");
            mySqlCommand.Parameters.AddWithValue("@username", username);
            await MySQL.QuaryAsync(mySqlCommand);

            return true;
        }

        public static bool IsUserRegistered(string username) => _users.Any(us => us.Username == username);
        public static bool IsUserRegistered(long chatId) => _users.Any(us => us.TelegramId == chatId);
        public static UserMode GetUserMode(string username) => _users.Find(us => us.Username == username)?.UserMode ?? UserMode.Guest;
    }
}

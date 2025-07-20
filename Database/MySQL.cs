using System.Data;
using MySql.Data.MySqlClient;
using MySqlConnector;
using SearchFriend.Logger;
using MySqlCommand = MySqlConnector.MySqlCommand;
using MySqlConnection = MySqlConnector.MySqlConnection;
using MySqlDataReader = MySqlConnector.MySqlDataReader;

namespace SearchFriend.Database
{
    public class MySQL
    {
        private static readonly string _connection = "server=localhost;uid=root;password=;database=search_friend_bot";

        public static async Task QuaryAsync(MySqlCommand command)
        {
            try
            {
                if (command == null)
                {
                    Log.WriteError("Отсутствует аргумент переданный в QuaryAsync", null);
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(_connection))
                {
                    command.Connection = connection;
                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex) 
            {
                Log.WriteError(ex.Message, ex.StackTrace);
            }
        }

        public static async Task<DataTable?> QuaryReadAsync(MySqlCommand command)
        {
            DataTable table = new DataTable();

            try
            {
                if (command == null)
                {
                    Log.WriteError("Отсутствует аргумент переданный в QuaryReadAsync", null);
                    return null;
                }

                using (MySqlConnection connection = new MySqlConnection(_connection))
                {
                    command.Connection = connection;
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync()) 
                    {
                        table.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.Message, ex.StackTrace);
                return null;
            }

            return table;
        }
    }
}

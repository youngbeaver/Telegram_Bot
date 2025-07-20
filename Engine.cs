using SearchFriend;
using SearchFriend.Entity;
using SearchFriend.Logger;

class Engine
{
    public static async Task Main(string[] args)
    {
        Log.InitLogs();
        await Users.InitUsers();
        await Bot.InitBotClient("YourBotToken");
        await Task.Delay(-1);
    }
}
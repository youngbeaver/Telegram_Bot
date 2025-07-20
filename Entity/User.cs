
namespace SearchFriend.Entity
{
    public class User
    {
        public int Id { get; private set; }
        public string FirstAndLastname { get; private set; }
        public string Username { get; private set; }
        public long TelegramId { get; private set; }
        public string DisplayName { get; set; }
        public ushort DisplayAge { get; set; }
        public string DisplayCity { get; set; }
        public UserMode UserMode { get; private set; }


        public User(int id, string firstAndLastname, string username, long telegramId, string displayName, ushort displayAge, string displayCity, UserMode userMode)
        {
            Id = id;
            FirstAndLastname = firstAndLastname;
            Username = username;
            TelegramId = telegramId;
            DisplayName = displayName;
            DisplayAge = displayAge;
            DisplayCity = displayCity;
            UserMode = userMode;
        }

        public User(int id, string firstAndLastname, string username, long telegramId) 
        {
            Id = id;
            FirstAndLastname = firstAndLastname;
            Username = username;
            TelegramId = telegramId;
            UserMode = UserMode.User;
        }
    }

    public enum UserMode
    {
        Guest,
        User,
        Moderator,
        Administator
    }
}

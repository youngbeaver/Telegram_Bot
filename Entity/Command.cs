using System;
using Telegram.Bot.Types;

namespace SearchFriend.Entity
{
    public class Command
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public UserMode Access { get; private set; }
        public Func<Message, string[]?, Task> Action { get; private set; }

        public Command(string name, string description, UserMode access, Func<Message, string[]?, Task> action)
        {
            Name = name;
            Description = description;
            Access = access;
            Action = action;
        }
    }
}

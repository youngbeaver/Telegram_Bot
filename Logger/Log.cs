namespace SearchFriend.Logger
{
    public class Log
    {
        /*
         * Последний название после "/" должно содержать *НазваниеФайла*.txt 
         */
        private static readonly string pathExceptionLogFile = @"Logger/ExceptionLogs.txt";
        private static readonly string pathAllLogFile = @"Logger/DefaultLogs.txt";

        public static void InitLogs()
        {
            if (!Directory.Exists(pathAllLogFile))
                Directory.CreateDirectory(Path.GetDirectoryName(pathAllLogFile));
        }

        /// <summary>
        /// Лог исключительно для ошибок
        /// </summary>
        /// <param name="message">Текст ошибки</param>
        /// <param name="trace">Подробный трэйс ошибки</param>
        public static void WriteError(string message, string trace)
        {
            string text = $"DateTime: [{DateTime.UtcNow}] MessageBody: {message} | MessageTrace: {trace}" + Environment.NewLine;

            if (!File.Exists(pathExceptionLogFile))
                File.WriteAllText(pathExceptionLogFile, text);

            Console.WriteLine(text);
        }

        /// <summary>
        /// Общий лог
        /// </summary>
        /// <param name="messageHead">Тип лога</param>
        /// <param name="messageBody">Содержание лога</param>
        public static void WriteLog(MessageHead messageHead, string messageBody)
        {
            string text = $"DateTime: [{DateTime.UtcNow}] MessageHead: {messageHead} | MessageBody: {messageBody}" + Environment.NewLine;

            if (!File.Exists(pathAllLogFile))
                File.WriteAllText(pathAllLogFile, text);

            Console.WriteLine(text);
        }
    }

    public enum MessageHead
    {
        Unknown = 0,
        BotStatus,
        NewDefaultMessage,
        AccountAction
    }
}

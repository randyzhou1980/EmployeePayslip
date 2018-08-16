using System;

namespace LoggerService
{
    public class ConsoleService: ILogService
    {
        public void LogInfo(string log)
        {
            Console.WriteLine(log);
        }

        public void LogError(Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        public void LogError(string error)
        {
            Console.WriteLine($"Error: {error}");
        }
    }
}

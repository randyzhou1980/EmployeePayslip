using System;

namespace LoggerService
{
    public interface ILogService
    {
        void LogInfo(string log);
        void LogError(Exception ex);
        void LogError(string error);
    }
}

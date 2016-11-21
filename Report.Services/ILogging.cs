using System;

namespace Report.Services
{
    public interface ILogging
    {
        void LogException(Exception ex, string logFilePath);
    }
}

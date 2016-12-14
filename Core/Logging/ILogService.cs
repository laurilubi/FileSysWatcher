using System;
using System.Collections.Generic;

namespace Core.Logging
{
    public interface ILogService
    {
        void ErrorLog(string logMessage);

        void WarningLog(string logMessage);

        void VerboseLog(string logMessage);

        void DebugLog(string logMessage);

        void PerformanceLog(string logMessage);

        void WriteToLog(string logMessage, LogLevel logLevel);

        void DeleteOldLogs(TimeSpan maxAge);

        void Register(List<string> report);

        void Unregister(List<string> report);
    }
}

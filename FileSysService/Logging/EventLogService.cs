using System;
using System.Diagnostics;
using Core.Logging;

namespace FileSysService.Logging
{
    public class EventLogService : BaseLogService, ILogService
    {
        private EventLog eventLog;

        public EventLogService(EventLog eventLog)
        {
            this.eventLog = eventLog;
        }

        public void ErrorLog(string logMessage)
        {
            WriteToLog(logMessage, LogLevel.Error);
        }

        public void WarningLog(string logMessage)
        {
            WriteToLog(logMessage, LogLevel.Warning);
        }

        public void VerboseLog(string logMessage)
        {
            WriteToLog(logMessage, LogLevel.Verbose);
        }

        public void DebugLog(string logMessage)
        {
            WriteToLog(logMessage, LogLevel.Debug);
        }

        public void PerformanceLog(string logMessage)
        {
            WriteToLog(logMessage, LogLevel.Performance);
        }

        public virtual void WriteToLog(string logMessage, LogLevel logLevel)
        {
            var line = string.Format("{0} {1} {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), logLevel, logMessage);
            eventLog.WriteEntry(line);
            PublishToSubscribers(line);
        }

        public void DeleteOldLogs(TimeSpan maxAge)
        { }
    }
}

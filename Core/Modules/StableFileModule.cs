using System;
using System.IO;
using FileSysWatcher.Core;

namespace FileSysWatcher.Modules
{
    public class StableFileModule : ModuleBase
    {
        public const string EXECUTION_COMMAND = "none||StableFileModule";

        private readonly TimeSpan minimumStableTime;
        private readonly bool deletedCountsAsStable;

        public StableFileModule(Section section, Settings settings, string settingsPrefix)
            : base(section, settings, settingsPrefix)
        {
            minimumStableTime = Util.GetTimespanOrDefault(ReadModuleSetting("MinimumStableTime"), new TimeSpan());
            deletedCountsAsStable = Util.GetBoolOrDefault(ReadModuleSetting("DeletedCountsAsStable", true), false);
        }

        public override ExecuteCommandResult Execute(DelayedEvent delayedEvent)
        {
            var result = new ExecuteCommandResult();
            if (minimumStableTime.TotalMilliseconds == 0) return result;

            var filename = delayedEvent.FileSystemEvent.FullPath;
            if (!File.Exists(filename))
            {
                result.ExitCode = deletedCountsAsStable ? 0 : 1;
                return result;
            }
            if (File.GetLastWriteTime(filename) > DateTime.Now - minimumStableTime)
            {
                result.ExitCode = 2;
                return result;
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using FileSysWatcher.Core;

namespace FileSysWatcher.Modules
{
    public class Section
    {
        public const int DEFAULT_EXIT_CODE = 0;

        private readonly string preConditionCommand;
        private readonly int preConditionExpectedExitCode;
        private readonly string command;
        private readonly int expectedExitCode;
        private readonly string folder;
        private readonly string filenameMatchRegex;
        private readonly Regex filenameMatchRegexObj;
        private readonly TimeSpan eventDelay;
        private readonly Settings settings;
        private FileSystemWatcher fsWatcher;

        public string SectionName { get; private set; }

        public Dictionary<string, DelayedEvent> DelayedEvents { get; private set; }

        public Section(string sectionName, Settings settings)
        {
            SectionName = sectionName;
            this.settings = settings;
            DelayedEvents = new Dictionary<string, DelayedEvent>();

            folder = Util.GetStringOrDefault(ReadSetting("Folder"), null);
            filenameMatchRegex = Util.GetStringOrDefault(ReadSetting("FilenameMatchRegex"), null);
            if (filenameMatchRegex != null)
                filenameMatchRegexObj = new Regex(filenameMatchRegex, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            eventDelay = Util.GetTimespanOrDefault(ReadSetting("EventDelay"), new TimeSpan(0));
            preConditionCommand = Util.GetStringOrDefault(ReadSetting("PreConditionCommand"), null);
            preConditionExpectedExitCode = Util.GetIntOrDefault(ReadSetting("PreConditionExpectedExitCode"), DEFAULT_EXIT_CODE);
            command = Util.GetStringOrDefault(ReadSetting("Command"), null);
            expectedExitCode = Util.GetIntOrDefault(ReadSetting("ExpectedExitCode"), DEFAULT_EXIT_CODE);

            InitializeWatcher(false);
        }

        private void InitializeWatcher(bool reInitialize)
        {
            // todo: if (!reInitialize) callEventOnAllFiles()
            fsWatcher = new FileSystemWatcher(folder);
            fsWatcher.IncludeSubdirectories = true;
            fsWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes
                | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess
                | NotifyFilters.CreationTime | NotifyFilters.Security;
            fsWatcher.Deleted += EventHandler;
            fsWatcher.Renamed += EventHandler;
            fsWatcher.Changed += EventHandler;
            fsWatcher.Created += EventHandler;
            fsWatcher.EnableRaisingEvents = true;
        }

        private void EventHandler(object sender, FileSystemEventArgs e)
        {
            try
            {
                var delayStr = eventDelay.TotalMilliseconds == 0
                    ? "no delay"
                    : string.Format("will be delayed for {0}", eventDelay);
                if (e.ChangeType == WatcherChangeTypes.Created)
                    Util.WriteLog(string.Format("Created {0}, {1}", e.FullPath, delayStr));
                else if (e.ChangeType == WatcherChangeTypes.Changed)
                    Util.WriteLog(string.Format("Changed {0}, {1}", e.FullPath, delayStr));
                else if (e.ChangeType == WatcherChangeTypes.Deleted)
                    Util.WriteLog(string.Format("Deleted {0}, {1}", e.FullPath, delayStr));
                else if (e.ChangeType == WatcherChangeTypes.Renamed)
                    Util.WriteLog(string.Format("Renamed {0} to {1}, {2}", ((RenamedEventArgs)e).OldFullPath, e.FullPath, delayStr));

                if (filenameMatchRegexObj != null && !filenameMatchRegexObj.IsMatch(e.FullPath))
                {
                    Util.WriteLog(string.Format("File {0} does not match regex {1}. Skipping the file.", e.FullPath, filenameMatchRegex));
                    return;
                }

                // make sure that timer object gets inserted into DelayedEvent
                var msDelay = eventDelay.TotalMilliseconds < 10
                    ? 10
                    : (int)eventDelay.TotalMilliseconds;

                DelayedEvent delayedEvent;
                if (DelayedEvents.ContainsKey(e.FullPath))
                {
                    Util.WriteLog(string.Format("Delaying existing event for {0}", e.FullPath));
                    delayedEvent = DelayedEvents[e.FullPath];
                    delayedEvent.FileSystemEvent = e;
                    delayedEvent.Timer.Change(msDelay, Timeout.Infinite);
                    return;
                }

                // new event
                delayedEvent = new DelayedEvent();
                DelayedEvents.Add(e.FullPath, delayedEvent);
                delayedEvent.FileSystemEvent = e;
                var timer = new Timer(EventHandlerTimerCallback, delayedEvent, msDelay, Timeout.Infinite);
                delayedEvent.Timer = timer;
            }
            catch (Exception ex)
            {
                Util.LogError(ex);
            }
        }

        private void EventHandlerTimerCallback(object state)
        {
            try
            {
                var delayedEvent = (DelayedEvent)state;
                var e = delayedEvent.FileSystemEvent;
                var delayStr = eventDelay.TotalMilliseconds == 0
                    ? "was not delayed"
                    : string.Format("was delayed for {0}", eventDelay);
                if (e.ChangeType == WatcherChangeTypes.Created)
                    Util.WriteLog(string.Format("Acting on created {0}, {1}", e.FullPath, delayStr));
                else if (e.ChangeType == WatcherChangeTypes.Changed)
                    Util.WriteLog(string.Format("Acting on changed {0}, {1}", e.FullPath, delayStr));
                else if (e.ChangeType == WatcherChangeTypes.Deleted)
                    Util.WriteLog(string.Format("Acting on deleted {0}, {1}", e.FullPath, delayStr));
                else if (e.ChangeType == WatcherChangeTypes.Renamed)
                    Util.WriteLog(string.Format("Acting on renamed {0} to {1}, {2}", ((RenamedEventArgs)e).OldFullPath, e.FullPath, delayStr));

                if (delayedEvent.Timer != null) delayedEvent.Timer.Dispose();
                DelayedEvents.Remove(e.FullPath);
                if (StableFileCondition(delayedEvent))
                    if (PreCondition(delayedEvent))
                        Execute(delayedEvent);
            }
            catch (Exception ex)
            {
                Util.LogError(ex);
            }
        }

        public bool StableFileCondition(DelayedEvent delayedEvent)
        {
            var commandResult = ModuleTools.ExecuteCommand(StableFileModule.EXECUTION_COMMAND, delayedEvent, this, settings, ModuleBase.PREFIX_PRECONDITION);
            if (commandResult.Error != null)
            {
                Util.LogError(commandResult.Error);
                return false;
            }
            Util.WriteLog(string.Format("ExitCode {0}, expected {1}", commandResult.ExitCode, preConditionExpectedExitCode));
            return commandResult.ExitCode == preConditionExpectedExitCode;
        }

        public bool PreCondition(DelayedEvent delayedEvent)
        {
            var commandResult = ModuleTools.ExecuteCommand(preConditionCommand, delayedEvent, this, settings, ModuleBase.PREFIX_PRECONDITION);
            if (commandResult.Error != null)
            {
                Util.LogError(commandResult.Error);
                return false;
            }
            Util.WriteLog(string.Format("ExitCode {0}, expected {1}", commandResult.ExitCode, preConditionExpectedExitCode));
            return commandResult.ExitCode == preConditionExpectedExitCode;
        }

        public void Execute(DelayedEvent delayedEvent)
        {
            var commandResult = ModuleTools.ExecuteCommand(command, delayedEvent, this, settings, ModuleBase.PREFIX_MODULE);
            if (commandResult.Error != null)
                throw commandResult.Error;
            if (commandResult.ExitCode != expectedExitCode)
            {
                Util.WriteLog(string.Format("Command {0} did not exit correctly, exit code {1} (expected {2}).",
                    command, commandResult.ExitCode, expectedExitCode));
                foreach (var line in commandResult.Lines)
                    Util.WriteLog(line);
            }
        }

        protected string ReadSetting(string key)
        {
            string value = settings[key];
            if (value == null) throw new Exception(string.Format("Missing configuration value for key {0} for section {1} of type {2}.",
                key, SectionName, SectionName));
            return value;
        }
    }
}

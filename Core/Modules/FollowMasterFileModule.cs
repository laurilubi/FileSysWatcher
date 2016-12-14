using System.IO;
using FileSysWatcher.Core;

namespace FileSysWatcher.Modules.Lubi
{
    public class FollowMasterFileModule : ModuleBase
    {
        public const string EXECUTION_COMMAND = "none||FollowMasterFileModule";

        private readonly string followerFilenameFormat;
        private readonly bool allowAutoDelete;
        private readonly bool createFolder;

        public FollowMasterFileModule(Section section, Settings settings, string settingsPrefix)
            : base(section, settings, settingsPrefix)
        {
            followerFilenameFormat = ReadModuleSetting("FollowerFilenameFormat");
            allowAutoDelete = Util.GetBoolOrDefault(ReadModuleSetting("AllowAutoDelete", true), false);
            createFolder = Util.GetBoolOrDefault(ReadModuleSetting("CreateFolder", true), true);
        }

        public override ExecuteCommandResult Execute(DelayedEvent delayedEvent)
        {
            var masterFilename = delayedEvent.FileSystemEvent.FullPath;
            var followerFilename = ModuleTools.FormatCommand(followerFilenameFormat, masterFilename);
            Util.WriteLog(string.Format("FollowMasterFileModule.Execute() followerFilename = {0}", followerFilename));

            var result = new ExecuteCommandResult();

            // master file exists, follower file exists and is newer than original file - the condition is false
            if (File.Exists(masterFilename) && File.Exists(followerFilename) && File.GetLastWriteTime(followerFilename) >= File.GetLastWriteTime(masterFilename))
            {
                result.ExitCode = 1;
                Util.WriteLog(string.Format("FollowMasterFileModule.Execute() ExitCode = {0}", result.ExitCode));
                return result;
            }

            // master file does not exist, follower file does not exist
            if (!File.Exists(masterFilename) && !File.Exists(followerFilename))
            {
                result.ExitCode = 2;
                Util.WriteLog(string.Format("FollowMasterFileModule.Execute() ExitCode = {0}", result.ExitCode));
                return result;
            }

            if (allowAutoDelete)
            {
                File.Delete(followerFilename);
                Util.WriteLog(string.Format("FollowMasterFileModule.Execute() Deleted follower {0}", followerFilename));
                result.ExitCode = 3;
                Util.WriteLog(string.Format("FollowMasterFileModule.Execute() ExitCode = {0}", result.ExitCode));
                return result;
            }

            if (createFolder)
            {
                var fileInfo = new FileInfo(followerFilename);
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            Util.WriteLog(string.Format("FollowMasterFileModule.Execute() ExitCode = {0}", result.ExitCode));
            return result;
        }
    }
}

using System.IO;

namespace FileSysWatcher.Modules.Lubi
{
    public class CompressedVideoConditionModule : ModuleBase
    {
        public CompressedVideoConditionModule(Section section, Settings settings, string settingsPrefix)
            : base(section, settings, settingsPrefix)
        {
        }

        public override ExecuteCommandResult Execute(DelayedEvent delayedEvent)
        {
            var filename = delayedEvent.FileSystemEvent.FullPath;
            var parts = filename.Split('\\');
            parts[parts.Length - 1] = string.Format("preview_{0}", parts[parts.Length - 1]);
            var previewFilename = string.Join("\\", parts);

            var result = new ExecuteCommandResult();

            // original file exists, preview file exists and is newer than original file - the condition is false
            if (File.Exists(filename) && File.Exists(previewFilename) && File.GetLastWriteTime(previewFilename) >= File.GetLastWriteTime(filename))
            {
                result.ExitCode = 1;
                return result;
            }

            // preview file exists but not the original

            return result;
        }
    }
}

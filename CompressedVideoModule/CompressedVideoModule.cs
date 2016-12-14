using System;
using System.IO;
using FileSysWatcher.Core;

namespace FileSysWatcher.Modules.Lubi
{
    public class CompressedVideoModule : ModuleBase
    {
        private readonly string compressionCommand;
        private readonly int expectedExitCode;

        public CompressedVideoModule(Section section, Settings settings, string settingsPrefix)
            : base(section, settings, settingsPrefix)
        {
            compressionCommand = ReadModuleSetting("Module_CompressionCommand");
            if (compressionCommand.StartsWith("|")) compressionCommand = compressionCommand.Substring(1);
            expectedExitCode = Util.GetIntOrDefault(ReadModuleSetting("ExpectedExitCode"), Section.DEFAULT_EXIT_CODE);
        }

        public override ExecuteCommandResult Execute(DelayedEvent delayedEvent)
        {
            var filename = delayedEvent.FileSystemEvent.FullPath;
            var parts = filename.Split('\\');
            parts[parts.Length - 1] = string.Format("preview_{0}", parts[parts.Length - 1]);
            var previewFilename = string.Join("\\", parts);

            var result = new ExecuteCommandResult();
            if (!File.Exists(filename))
            {
                if (File.Exists(previewFilename))
                    File.Delete(previewFilename);
                return result;
            }

            var resolvedCompressionCommand = compressionCommand.Replace("[previewFilename]", previewFilename);
            result = ModuleTools.ExecuteShellCommandSync(resolvedCompressionCommand, filename);
            if (result.Error != null)
                throw result.Error;
            if (result.ExitCode != expectedExitCode)
            {
                Util.WriteLog(string.Format("Command {0} did not exit correctly, exit code {1} (expected {2}).",
                    resolvedCompressionCommand, result.ExitCode, expectedExitCode));
                foreach (var line in result.Lines)
                    Util.WriteLog(line);
            }
            return result;
        }
    }
}

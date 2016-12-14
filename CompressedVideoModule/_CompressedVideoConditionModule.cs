using System;
using System.IO;

namespace FileSysWatcher.Modules.Lubi
{
    //public class CompressedVideoConditionModule : ModuleBase
    //{
    //    public CompressedVideoConditionModule(Section section, Settings settings)
    //        : base(section, settings)
    //    {
    //    }

    //    public override ExecuteCommandResult Execute(string filename)
    //    {
    //        var parts = filename.Split('\\');
    //        parts[parts.Length - 1] = "preview_" + parts[parts.Length - 1];
    //        var previewFilename = string.Join("\\", parts);

    //        var result = new ExecuteCommandResult();

    //        // preview file exists and is newer than original file - the condition fails
    //        if (File.Exists(previewFilename) && File.GetLastWriteTime(previewFilename) >= File.GetLastWriteTime(filename))
    //        {
    //            result.ExitCode = 1;
    //            return result;
    //        }

    //        return result;
    //    }
    //}
}

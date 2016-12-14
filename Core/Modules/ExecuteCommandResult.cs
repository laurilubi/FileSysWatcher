using System;
using System.Collections.Generic;

namespace FileSysWatcher.Modules
{
    public class ExecuteCommandResult
    {
        public bool IsShellCommand { get; set; }

        public List<string> Lines { get; private set; }

        public int ExitCode { get; set; }

        public Exception Error { get; set; }

        public ExecuteCommandResult()
        {
            Lines = new List<string>();
        }
    }
}
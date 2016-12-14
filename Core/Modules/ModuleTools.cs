using System;
using System.Diagnostics;
using System.Reflection;

namespace FileSysWatcher.Modules
{
    public static class ModuleTools
    {
        public const string SHELL_COMMAND_PREFIX = "|";
        public const char INTERNAL_COMMAND_SEPARATOR = '|';

        public static string FormatCommand(string format, string filename)
        {
            var posSlash = filename.LastIndexOfAny(new[] { '/', '\\' });

            var result = format;
            result = result.Replace("{file}", filename);

            var noExtPosDot = filename.LastIndexOf('.');
            var noExt = noExtPosDot == -1 || noExtPosDot < posSlash
                ? filename
                : filename.Substring(0, noExtPosDot);
            result = result.Replace("{fileNoExt}", noExt);

            var folder = posSlash == -1
                ? string.Empty
                : filename.Substring(0, posSlash);
            result = result.Replace("{folder}", folder);
            return result;
        }

        public static ExecuteCommandResult ExecuteCommand(string command, DelayedEvent delayedEvent, Section section, Settings settings, string settingsPrefix)
        {
            var filename = delayedEvent.FileSystemEvent.FullPath;
            if (command.StartsWith(SHELL_COMMAND_PREFIX))
                return ExecuteShellCommandSync(command.Substring(1), filename);
            try
            {
                var tmp = command.Split(INTERNAL_COMMAND_SEPARATOR);
                var assemblyFile = tmp[0];
                var subNamespace = tmp[1];
                var className = tmp[2];

                Type myType;
                if (assemblyFile == "none")
                {
                    myType = Type.GetType(string.Format("FileSysWatcher.Modules.{0}", className));
                }
                else
                {
                    // Use the file name to load the assembly into the current 
                    // application domain.
                    var a = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + assemblyFile);
                    // Get the type to use.
                    myType = a.GetType(string.Format("FileSysWatcher.Modules.{0}.{1}", subNamespace, className));
                }
                // Get the method to call.
                var mymethod = myType.GetMethod("Execute");
                // Create an instance.
                Object obj = Activator.CreateInstance(myType, section, settings, settingsPrefix);
                Debug.Assert(obj.GetType() == typeof(ModuleBase));
                // Execute the method.
                var parameters = new object[] { delayedEvent };
                object result = mymethod.Invoke(obj, parameters);
                Debug.Assert(result.GetType() == typeof(ExecuteCommandResult));
                return (ExecuteCommandResult)result;
            }
            catch (Exception ex)
            {
                var result = new ExecuteCommandResult();
                result.Error = ex.InnerException ?? ex;
                return result;
            }
        }

        /// <summary>
        /// Executes a shell command synchronously.
        /// </summary>
        /// <param name="command">format command</param>
        /// <param name="filename"> </param>
        /// <returns>string, as output of the command.</returns>
        public static ExecuteCommandResult ExecuteShellCommandSync(string command, string filename)
        {
            var result = new ExecuteCommandResult { IsShellCommand = true };
            try
            {
                command = FormatCommand(command, filename);
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                var procStartInfo = new ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.RedirectStandardError = true;
                procStartInfo.UseShellExecute = false;

                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;

                // Now we create a process, assign its ProcessStartInfo and start it
                var proc = new Process { StartInfo = procStartInfo };
                proc.Start();

                // read the results
                string errors = proc.StandardError.ReadToEnd();
                result.Lines.AddRange(errors.Split(new[] { '\r', '\n' }));
                string output = proc.StandardOutput.ReadToEnd();
                result.Lines.AddRange(output.Split(new[] { '\r', '\n' }));
                result.ExitCode = proc.ExitCode;
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }
            return result;
        }
    }
}

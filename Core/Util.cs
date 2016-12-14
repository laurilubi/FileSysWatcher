using System;
using System.Configuration;
using System.IO;

namespace FileSysWatcher.Core
{
    public static class Util
    {
        public static string logFileName;
        public static string errorLogFileName;

        static Util()
        {
            logFileName = GetSetting("LOGFILENAME");
            errorLogFileName = GetSetting("ERRORLOGFILENAME");

            IsDirectoryPresent(GetDirectoryNameFromPath(logFileName), true);
            IsDirectoryPresent(GetDirectoryNameFromPath(errorLogFileName), true);
        }

        /// <summary>
        /// Gets The Directory Path from the FilePath
        /// </summary>
        public static string GetDirectoryNameFromPath(string filepath)
        {
            var posLastSlash = filepath.LastIndexOf('\\');
            var direcoryPath = filepath.Substring(0, posLastSlash);
            return direcoryPath;
        }

        public static bool IsDirectoryPresent(string directory, bool create)
        {
            try
            {
                if (Directory.Exists(directory)) return true;
                if (!create) return false;
                Directory.CreateDirectory(directory);
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Gets Values From The Config File.
        /// </summary>
        public static string GetSetting(string val)
        {
            return ConfigurationManager.AppSettings[val];
        }

        public static void WriteLog(string message)
        {
            if (!IsDirectoryPresent(GetDirectoryNameFromPath(logFileName), true)) return;
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                fs = new FileStream(logFileName, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.WriteLine(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }

        public static void LogError(Exception sourceException)
        {
            if (!IsDirectoryPresent(GetDirectoryNameFromPath(errorLogFileName), true)) return;
            try { WriteLog(sourceException.Message); }
            catch { }

            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(errorLogFileName, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.WriteLine("==================================================================");
                sw.WriteLine("ERROR OCCURED AT:   " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.WriteLine("ERROR OCCOURED IN:  " + sourceException.Source);
                sw.WriteLine("ERROR DESCRPTION:   " + sourceException.Message);
                sw.WriteLine("ERROR DESCRPTION:   " + sourceException.StackTrace);
                sw.WriteLine("==================================================================");
                sw.WriteLine("");
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }

        public static int GetInt(string value)
        {
            int result;
            if (!int.TryParse(value, out result)) throw new Exception(string.Format("Value {0} cannot be parsed as int.", value));
            return result;
        }

        public static int GetIntOrDefault(string value, int defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return GetInt(value);
        }

        public static string GetStringOrDefault(string value, string defaultValue)
        {
            return value ?? defaultValue;
        }

        public static TimeSpan GetTimespan(string value)
        {
            TimeSpan result;
            if (!TimeSpan.TryParse(value, out result)) throw new Exception(string.Format("Value {0} cannot be parsed as timespan.", value));
            return result;
        }

        public static TimeSpan GetTimespanOrDefault(string value, TimeSpan defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return GetTimespan(value);
        }

        public static bool GetBool(string value)
        {
            bool result;
            if (!bool.TryParse(value, out result)) throw new Exception(string.Format("Value {0} cannot be parsed as boolean.", value));
            return result;
        }

        public static bool GetBoolOrDefault(string value, bool defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return GetBool(value);
        }
    }
}

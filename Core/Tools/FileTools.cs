using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Core.Tools
{
    public static class FileTools
    {
        public static void DeleteOldFiles(string folder, int maxDays) { DeleteOldFiles(folder, DateTime.Now.AddDays(-maxDays)); }
        public static void DeleteOldFiles(string folder, TimeSpan maxAge) { DeleteOldFiles(folder, DateTime.Now - maxAge); }
        public static void DeleteOldFiles(string folder, DateTime minimumDate)
        {
            var dir = new DirectoryInfo(folder);
            foreach (var fileInfo in dir.GetFiles().Where(_ => _.LastWriteTime < minimumDate))
            {
                TryDelete(fileInfo.FullName);
            }
        }

        public static void DeleteOldFolders(string folder, int maxDays) { DeleteOldFolders(folder, DateTime.Now.AddDays(-maxDays)); }
        public static void DeleteOldFolders(string folder, TimeSpan maxAge) { DeleteOldFolders(folder, DateTime.Now - maxAge); }
        public static void DeleteOldFolders(string folder, DateTime minimumDate)
        {
            var dir = new DirectoryInfo(folder);
            foreach (var subFolder in dir.GetDirectories())
                DeleteOldFolders(subFolder.FullName, minimumDate);
            DeleteOldFiles(folder, minimumDate);

            if (!dir.GetDirectories().Any() && !dir.GetFiles().Any())
                TryDelete(folder);
        }

        public static long GetSize(string folder) { return GetSize(new DirectoryInfo(folder)); }
        public static long GetSize(DirectoryInfo folderInfo)
        {
            long totalSize = 0;
            foreach (FileInfo fileInfo in folderInfo.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                totalSize += fileInfo.Length;
            }
            return totalSize;
        }

        public static void GuaranteeFolder(string folder)
        {
            if (Directory.Exists(folder)) return;
            Directory.CreateDirectory(folder);
        }

        public static string Normalize(this string path)
        {
            var output = path.Replace('/', '\\');
            char prevChar = ' ';
            var sb = new StringBuilder();
            var pos = 0;
            foreach (var ch in output)
            {
                if (pos > 1 && ch == '\\' && ch == prevChar) continue;
                sb.Append(ch);
                prevChar = ch;
                pos++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Lowercase, no dots.
        /// </summary>
        public static string GetExtension(this string filepath)
        {
            var extension = Path.GetExtension(filepath) ?? "";
            extension = extension.ToLower();
            extension = extension.Replace(".", "");
            return extension;
        }

        public static string ChangeExtension(this string filepath, string newExtension)
        {
            var extension = filepath.GetExtension();
            var newFilePath = filepath.Substring(0, filepath.Length - extension.Length).TrimEnd('.');
            if (!string.IsNullOrEmpty(newExtension))
                newFilePath += "." + newExtension.ToLower();
            return newFilePath;
        }

        public static bool TryDelete(string fileOrFolderPath)
        {
            try
            {
                var attr = File.GetAttributes(fileOrFolderPath);
                if (attr.HasFlag(FileAttributes.Directory))
                    Directory.Delete(fileOrFolderPath);
                else
                    File.Delete(fileOrFolderPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ResolveDotDot(this string path)
        {
            var folders = path.Split('\\');
            var result = new List<string>();
            for (var i = folders.Length - 1; i >= 0; i--)
            {
                if (folders[i] == "..")
                {
                    i--;
                    continue;
                }
                result.Insert(0, folders[i]);
            }
            return result.CommaSeparate(_ => _, "\\");
        }

        public static string NormalizeAndResolveDotDot(string path)
        {
            return ResolveDotDot(Normalize(path));
        }

        private static readonly Regex NonAlphaNumericRgx = new Regex(@"[^a-z0-9.]+", RegexOptions.Compiled);
        public static string ToWebFriendly(string url)
        {
            var cleaned = url.ToLower();
            cleaned = NonAlphaNumericRgx.Replace(cleaned, "_");
            return cleaned;
        }

        public static bool IsValidFilPath(string filePath)
        {
            Regex containsABadCharacter = new Regex("["
                  + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");
            if (containsABadCharacter.IsMatch(filePath)) { return false; };

            // other checks for UNC, drive-path format, etc

            return true;
        }

        public static string RemoveInvalidFileChars(string filename, string replaceString = "")
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(filename, replaceString);
        }

        private static readonly string[] Suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; // type long runs out around EB
        public static string DisplaySize(long size)
        {
            if (size == 0)
                return "0" + Suffixes[0];
            long bytes = Math.Abs(size);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            int decimalCount = Math.Max(0, place - 2);
            double num = Math.Round(bytes / Math.Pow(1024, place), decimalCount);
            return (Math.Sign(size) * num).ToString() + Suffixes[place];
        }

        // Returns a unique temporary file name, and creates a 0-byte file by that
        // name on disk.
        public static string GetTempFileName(string extension = "tmp")
        {
            var folder = Path.GetTempPath();
            string filePath;
            do
            {
                filePath = Path.Combine(folder, "tmp_" + StringTools.GetRandomString(10, true) + "." + extension);
            } while (File.Exists(filePath));

            File.WriteAllText(filePath, "");
            return filePath;
        }

        public static bool EqualByBytes(FileInfo first, FileInfo second)
        {
            const int bytesToRead = sizeof(long);

            if (first.Length != second.Length)
                return false;

            var iterations = (int)Math.Ceiling((double)first.Length / bytesToRead);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[bytesToRead];
                byte[] two = new byte[bytesToRead];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, bytesToRead);
                    fs2.Read(two, 0, bytesToRead);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }

        public static void AssertNotDotDot(string fileName)
        {
            AssertTools.Assert(!fileName.Contains(".."), string.Format("File name cannot include 2 dots. File name={0}", fileName));
        }

        public static int GetBounceOff(int initialDelay, int counter = 0)
        {
            var power = (int)Math.Pow(2, counter);
            var random = new Random();
            return random.Next(0, initialDelay * power);
        }

        public static TResult ExecuteWithRetry<TException, TResult>(int retryCount, int initialDelay, Func<TResult> func) where TException : Exception
        {
            for (var i = 0; i < retryCount - 1; i++)
            {
                try
                {
                    return func();
                }
                catch (TException)
                {
                    Thread.Sleep(GetBounceOff(initialDelay, i));
                }
            }
            return func();
        }

        public static void ExecuteWithRetry<TException>(int retryCount, int initialDelay, Action action) where TException : Exception
        {
            for (var i = 0; i < retryCount - 1; i++)
            {
                try
                {
                    action();
                }
                catch (TException)
                {
                    Thread.Sleep(GetBounceOff(initialDelay, i));
                }
            }
            action();
        }
    }
}

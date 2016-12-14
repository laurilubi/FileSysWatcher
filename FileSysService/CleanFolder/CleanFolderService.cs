using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Core;
using Core.Logging;
using Core.Tools;

namespace FileSysService.CleanFolder
{
    public class CleanFolderService : IExecutableService
    {
        private readonly ILogService logService;
        private readonly List<CleanFolderDeleteEntry> deleteEntries;

        public CleanFolderService(ILogService logService, XmlNode cleanFolderNode)
        {
            this.logService = logService;
            deleteEntries = GetConfigDeleteEntries(cleanFolderNode);
        }

        private List<CleanFolderDeleteEntry> GetConfigDeleteEntries(XmlNode cleanFolderNode)
        {
            var ret = new List<CleanFolderDeleteEntry>();
            if (cleanFolderNode == null) return ret;
            var deleteNodes = cleanFolderNode.SelectNodes("Delete");
            if (deleteNodes == null) return ret;

            foreach (XmlElement deleteNode in deleteNodes)
            {
                var deleteEntry = new CleanFolderDeleteEntry();
                deleteEntry.FolderPath = deleteNode.GetAttribute("folder");
                var maxAgeDays = NumberTools.GetIntOrDefault(deleteNode.GetAttribute("maxAge"), 31);
                deleteEntry.MaxAge = new TimeSpan(maxAgeDays, 0, 0, 0);
                deleteEntry.IsActive = BoolTools.GetBoolOrDefault(deleteNode.GetAttribute("active"));
                ret.Add(deleteEntry);
            }
            return ret;
        }

        public void Execute()
        {
            foreach (var deleteEntry in deleteEntries)
                CheckFolder(deleteEntry, 0);
        }

        public void CheckFolder(CleanFolderDeleteEntry deleteEntry, int level)
        {
            if (level == 0)
            {
                logService.VerboseLog(string.Format("{0}Checking folder {1} for files not used after {2}.", deleteEntry.WhatIfPrefix, deleteEntry.FolderPath, DateTime.Now - deleteEntry.MaxAge));
                WriteWarningFile(deleteEntry);
            }

            var folder = new DirectoryInfo(deleteEntry.FolderPath);
            if (!folder.Exists)
                return;

            var subFolders = folder.GetDirectories();
            foreach (var subFolder in subFolders)
            {
                var subEntry = new CleanFolderDeleteEntry
                {
                    FolderPath = subFolder.FullName,
                    MaxAge = deleteEntry.MaxAge,
                    IsActive = deleteEntry.IsActive
                };
                try
                {
                    CheckFolder(subEntry, level + 1);
                }
                catch (IOException ex)
                {
                    logService.ErrorLog(ex.ToString());
                }
            }

            var files = folder.GetFiles();
            foreach (var file in files)
            {
                if (!IsFileObsolete(file, deleteEntry.MaxAge)) continue;
                try
                {
                    if (deleteEntry.IsActive)
                        file.Delete();
                    logService.VerboseLog(string.Format("{0}Deleted file {1}", deleteEntry.WhatIfPrefix, file.FullName));
                }
                catch (IOException ex)
                {
                    logService.ErrorLog(ex.ToString());
                }
            }

            if (level == 0 || !deleteEntry.IsActive) return;
            if (!IsFileObsolete(folder, deleteEntry.MaxAge)) return;

            subFolders = folder.GetDirectories();
            files = folder.GetFiles();
            if (subFolders.Length == 0 && files.Length == 0)
            {
                FileTools.TryDelete(folder.FullName);
                logService.VerboseLog("Deleted folder " + folder.FullName);
            }
        }

        private void WriteWarningFile(CleanFolderDeleteEntry deleteEntry)
        {
            var folderInfo = new DirectoryInfo(deleteEntry.FolderPath);
            foreach (var fileInfo in folderInfo.GetFiles("_WARNING - *.txt"))
            {
                if (!fileInfo.Name.Contains("Files in this folder and subfolders will be deleted after ")) continue;
                FileTools.TryDelete(fileInfo.FullName);
            }

            var message = string.Format("Files in this folder and subfolders will be deleted after {0} days.", deleteEntry.MaxAge.TotalDays);
            var fileName = string.Format("_WARNING - {0}.txt", message);
            var filePath = Path.Combine(deleteEntry.FolderPath, fileName);
            File.WriteAllText(filePath, deleteEntry.WhatIfPrefix + message);
        }

        private bool IsFileObsolete(FileSystemInfo fileInfo, TimeSpan maxAge)
        {
            var minDate = DateTime.Now - maxAge;
            if (fileInfo.CreationTime > minDate) return false;
            if (fileInfo.LastAccessTime > minDate) return false;
            if (fileInfo.LastWriteTime > minDate) return false;
            return true;
        }
    }
}

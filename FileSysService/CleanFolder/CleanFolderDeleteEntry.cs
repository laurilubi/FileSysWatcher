using System;

namespace FileSysService.CleanFolder
{
    public class CleanFolderDeleteEntry
    {
        public string FolderPath { get; set; }

        public TimeSpan MaxAge { get; set; }

        public bool IsActive { get; set; }

        public string WhatIfPrefix => IsActive ? "" : "WHATIF: ";
    }
}

﻿
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace ServerBackupUtility.Services
{
    public class ArchiveService : IArchiveService
    {
        private readonly string _folderPaths = ConfigurationManager.AppSettings["FolderPaths"];
        private readonly string _archivePath = ConfigurationManager.AppSettings["ArchivePath"];

        public void CreateArchives()
        {
            LogService.LogEvent("Reading Web Folder Paths");

            string[] folderPaths = _folderPaths.Split(new [] {'|'}, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                foreach (var folderPath in folderPaths)
                {
                    IEnumerable<String> directoryPaths = Directory.EnumerateDirectories(folderPath, "*", SearchOption.TopDirectoryOnly);

                    foreach (var directoryPath in directoryPaths)
                    {
                        int index = directoryPath.Trim().LastIndexOf('\\');
                        string directoryName = directoryPath.Trim().Substring(index);

                        LogService.LogEvent("Creating Archive: " + directoryName);

                        if (File.Exists(_archivePath + directoryName + ".zip"))
                        {
                            File.Delete(_archivePath + directoryName + ".zip");
                            Thread.Sleep(1000);
                        }

                        ZipFile.CreateFromDirectory(directoryPath, _archivePath + directoryName + ".zip", CompressionLevel.Optimal, true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: ArchiveService.CreateArchivesAsync - " + ex.Message);
            }

            LogService.LogEvent("Finishing Archive Backup Process");
        }
    }
}

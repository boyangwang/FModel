﻿using csharp_wick;
using FModel.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace FModel
{
    static class CreateBackup
    {
        public static void CreateBackupList()
        {
            PakExtractor extractor = null;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < ThePak.mainPaksList.Count; i++)
            {
                try
                {
                    extractor = new PakExtractor(Settings.Default.PAKsPath + "\\" + ThePak.mainPaksList[i].thePak, Settings.Default.AESKey);
                }
                catch (Exception)
                {
                    new UpdateMyConsole("0x" + Settings.Default.AESKey + " doesn't work with the main paks.", Color.Red, true).AppendToConsole();
                    extractor.Dispose();
                    break;
                }

                string[] CurrentUsedPakLines = extractor.GetFileList().ToArray();
                if (CurrentUsedPakLines != null)
                {
                    string mountPoint = extractor.GetMountPoint();
                    for (int ii = 0; ii < CurrentUsedPakLines.Length; ii++)
                    {
                        CurrentUsedPakLines[ii] = mountPoint.Substring(9) + CurrentUsedPakLines[ii];

                        sb.Append(CurrentUsedPakLines[ii] + "\n");
                    }
                    new UpdateMyState(".PAK mount point: " + mountPoint.Substring(9), "Waiting").ChangeProcessState();
                }
                extractor.Dispose();
            }

            for (int i = 0; i < ThePak.dynamicPaksList.Count; i++)
            {
                string pakName = DynamicKeysManager.AESEntries.Where(x => x.thePak == ThePak.dynamicPaksList[i].thePak).Select(x => x.thePak).FirstOrDefault();
                string pakKey = DynamicKeysManager.AESEntries.Where(x => x.thePak == ThePak.dynamicPaksList[i].thePak).Select(x => x.theKey).FirstOrDefault();

                if (!string.IsNullOrEmpty(pakName) && !string.IsNullOrEmpty(pakKey))
                {
                    try
                    {
                        extractor = new PakExtractor(Settings.Default.PAKsPath + "\\" + pakName, pakKey);
                    }
                    catch (Exception)
                    {
                        new UpdateMyConsole("0x" + pakKey + " doesn't work with " + ThePak.dynamicPaksList[i].thePak, Color.Red, true).AppendToConsole();
                        extractor.Dispose();
                        continue;
                    }

                    string[] CurrentUsedPakLines = extractor.GetFileList().ToArray();
                    if (CurrentUsedPakLines != null)
                    {
                        string mountPoint = extractor.GetMountPoint();
                        for (int ii = 0; ii < CurrentUsedPakLines.Length; ii++)
                        {
                            CurrentUsedPakLines[ii] = mountPoint.Substring(9) + CurrentUsedPakLines[ii];

                            sb.Append(CurrentUsedPakLines[ii] + "\n");
                        }
                        new UpdateMyConsole("Backing up ", Color.Black).AppendToConsole();
                        new UpdateMyConsole(ThePak.dynamicPaksList[i].thePak, Color.DarkRed, true).AppendToConsole();
                    }
                    extractor.Dispose();
                }
            }

            File.WriteAllText(App.DefaultOutputPath + "\\Backup" + Checking.BackupFileName, sb.ToString()); //File will always exist so we check the file size instead
            if (new System.IO.FileInfo(App.DefaultOutputPath + "\\Backup" + Checking.BackupFileName).Length > 0)
            {
                new UpdateMyState("\\Backup" + Checking.BackupFileName + " successfully created", "Success").ChangeProcessState();
            }
            else
            {
                File.Delete(App.DefaultOutputPath + "\\Backup" + Checking.BackupFileName);
                new UpdateMyState("Can't create " + Checking.BackupFileName.Substring(1), "Error").ChangeProcessState();
            }
        }
    }
}
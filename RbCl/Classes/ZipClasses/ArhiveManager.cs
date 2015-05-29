using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Config_classes;
using System.Threading;

namespace ZippArhiveHelper
{
    public enum ArhivStates { Begin=0, Arhing = 1, Ready = 2,Error=-1 };
    public class ArhiveManager
    {
        public static Action<string> LogEvent;

        public static ArhivStates _ArchiveState = ArhivStates.Begin;
        public static ArhivStates ArchiveState
        {
            get
            {
                return _ArchiveState;
            }
            set
            {
                _ArchiveState = value;
            }
        }
        
        public static FileInfo CreateArchiveFile(List<FileInfo> file_list, string zipFileName, string ArchiveFolder)
        {
            ArchiveState = ArhivStates.Arhing;
            Log("Начинаем создание архива");
            // получить файлы для архивации
            string release_folder_str = ArchiveFolder;
            DirectoryInfo dir = new DirectoryInfo(release_folder_str); 
            
            
            if (file_list == null || file_list.Count == 0) throw new Exception("Нет файлов в директории релиза!");

            string z_path = Path.Combine(ArchiveFolder, zipFileName);// zipFileName;

            if (File.Exists(z_path))
            {
                File.Delete(z_path);
            }

            ZipOutputStream zipStream = new ZipOutputStream(File.Create(z_path));

            file_list.ForEach(a =>
            {
                ZipEntry zipEntry = new ZipEntry(a.Name);
                zipStream.PutNextEntry(zipEntry);
                byte[] buffer = File.ReadAllBytes(a.FullName);
                zipStream.Write(buffer, 0, buffer.Length);
                zipStream.CloseEntry();
            });

            zipStream.Close();            
            
            Log("Архив создан, file: "+z_path);
            return new FileInfo(z_path);
        }

        

        private static void Log(string message)
        {
            if (LogEvent!=null)
                LogEvent(message);
        }

        internal static FileInfo CreateArchiveFile(List<FileInfo> files, string ZreportName, DirectoryInfo dest_dir)
        {
            return CreateArchiveFile(files, ZreportName, dest_dir.FullName);
        }
    }
}

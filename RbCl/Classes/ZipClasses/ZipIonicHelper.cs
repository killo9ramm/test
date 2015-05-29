using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using RBServer.Debug_classes;
using RBClient.Classes;
using Ionic.Zip;

namespace Debug_classes
{
    class ZipIonicHelper
    {
        public delegate void MessageEventHandler(object o, MessageEventArgs e);
        public static NLogDelegate LogEvent;
        
        private static void Log(string message)
        {
            if (null != LogEvent)
            {
                LogEvent(message);
            }
        }
        private static void Log(string message,Exception exp)
        {
            if (null != LogEvent)
            {
                LogEvent(message+" exception: "+exp.Message);
            }
        }
        private static void Log(Exception exp)
        {
            if (null != LogEvent)
            {
                LogEvent(exp.Message);
            }
        }

        /// <summary>
        /// Разархивирует файл в указанную директорию
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sourceDir">папка назначения</param>
        public static void Exctract(string file,DirectoryInfo sourceDir)
        {
            try
            {
                sourceDir = sourceDir.FullName.CreateOrReturnDirectory();
                using (ZipFile _zip = ZipFile.Read(file, Encoding.GetEncoding("cp866")))
                {
                    foreach (ZipEntry e in _zip)
                    {
                        FileInfo _fi = new FileInfo(e.FileName);
                        using (FileStream fs = new FileStream(Path.Combine(sourceDir.FullName,_fi.Name), FileMode.OpenOrCreate))
                        {
                            e.Extract(fs);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log("Exception: " + exp, exp);
            }
        }

        /// <summary>
        /// Получает имена файлов в архиве
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<string> GetZipEntryFileNames(string file)
        {
            List<string> fileNames = new List<string>();
            try
            {
                
                using (ZipFile _zip = ZipFile.Read(file, Encoding.GetEncoding("cp866")))
                {
                    foreach (ZipEntry e in _zip)
                    {
                        fileNames.Add(e.FileName);
                    }
                }
            }
            catch (Exception exp)
            {
                Log("Exception: " + exp, exp);
            }
            return fileNames;
        }

        /// <summary>
        /// Разархивирует файл в указанную директорию
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sourceDir">папка назначения</param>
        public static List<FileInfo> Exctract(FileInfo file, DirectoryInfo sourceDir)
        {
            List<FileInfo> fileList = new List<FileInfo>();
            try
            {
                sourceDir = sourceDir.FullName.CreateOrReturnDirectory();
                using (ZipFile _zip = ZipFile.Read(file.FullName, Encoding.GetEncoding("cp866")))
                {
                    foreach (ZipEntry e in _zip)
                    {
                        if (e.FileName.IndexOf("Thumbs.db") == -1)
                        {
                            FileInfo _fi = new FileInfo(e.FileName);
                            string filePath = Path.Combine(sourceDir.FullName, _fi.Name);
                            filePath.DeleteFileIfExist();
                            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                            {
                                e.Extract(fs);
                                fileList.Add(new FileInfo(filePath));
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log("Exception: " + exp, exp);
            }

            return fileList;
        }


        //private static FileInfo CreateArchiveFile()
        //{
            

        //    string extension = "-----";

        //    Root.Log("Начинаем создание архива");
        //    // получить файлы для архивации
        //    string release_folder_str = ConfigClass.GetProperty("RBClientReleaseFolder").ToString();

        //    DirectoryInfo dir = new DirectoryInfo(release_folder_str);
        //    List<FileInfo> file_list = dir.GetFiles().ToList();

        //    if (file_list == null || file_list.Count == 0) throw new Exception("Нет файлов в директории релиза!");


        //    string zipFileName = ConfigClass.GetProperty("RBClientArchiveName").ToString();
        //    string z_path = Path.Combine(TempFolder.FullName, zipFileName);// zipFileName;

        //    if (File.Exists(z_path))
        //    {
        //        File.Delete(z_path);
        //    }

        //    ZipOutputStream zipStream = new ZipOutputStream(File.Create(z_path));


        //    file_list.ForEach(a =>
        //    {
        //        ZipEntry zipEntry = new ZipEntry();
        //        zipEntry.fi
        //        zipStream.1PutNextEntry(zipEntry);
        //        byte[] buffer = File.ReadAllBytes(a.FullName);
        //        zipStream.Write(buffer, 0, buffer.Length);
        //        zipStream.CloseEntry();
        //    });

        //    zipStream.Close();

        //    Root.Log("Архив создан, file: " + z_path);
        //    return new FileInfo(z_path);
        //}

        /// <summary>
        /// разархивировать файл
        /// </summary>
        /// <param name="fileName">имя файла архива</param>
        /// <param name="directory_name">папка куда разархивировать</param>
        //public static void UnZipFileVoid(string fileName, string directory_name)
        //{
        //    try
        //    {
        //        System.IO.DirectoryInfo dir = FileSystemHelper.makeFolder(directory_name);

        //        FileStream fileStreamIn = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //        ZipInputStream zipInStream = new ZipInputStream(fileStreamIn);
        //        ZipEntry entry = zipInStream.GetNextEntry();

        //        while (null != entry)
        //        {
        //            FileStream fileStreamOut = new FileStream(Path.Combine(directory_name, entry.Name), FileMode.Create, FileAccess.Write);
        //            int size;
        //            byte[] buffer = new byte[4096];
        //            do
        //            {
        //                size = zipInStream.Read(buffer, 0, buffer.Length);
        //                fileStreamOut.Write(buffer, 0, size);
        //            } while (size > 0);

        //            fileStreamOut.Close();
        //            entry = zipInStream.GetNextEntry();
        //        }


        //        zipInStream.Close();
        //        fileStreamIn.Close();
        //    }
        //    catch (Exception exp)
        //    {
        //        Log("Не удалось разархивировать " + fileName + " error: " + exp.Message);
        //        throw exp;
        //    }
        //}

        /// <summary>
        /// разархивировать файл
        /// </summary>
        /// <param name="fileName">имя файла архива</param>
        /// <param name="directory_name">папка куда разархивировать</param>
        //public static List<FileInfo> UnZipFile(string fileName, string directory_name)
        //{
        //    try
        //    {
        //        List<FileInfo> file_list = new List<FileInfo>();
        //        System.IO.DirectoryInfo dir = FileSystemHelper.makeFolder(directory_name);

        //        FileStream fileStreamIn = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //        ZipInputStream zipInStream = new ZipInputStream(fileStreamIn);
        //        ZipEntry entry = zipInStream.GetNextEntry();

        //        while (null != entry)
        //        {
        //            string file_path=Path.Combine(directory_name, entry.Name);
        //            FileStream fileStreamOut = new FileStream(file_path, FileMode.Create, FileAccess.Write);
        //            int size;
        //            byte[] buffer = new byte[4096];
        //            do
        //            {
        //                size = zipInStream.Read(buffer, 0, buffer.Length);
        //                fileStreamOut.Write(buffer, 0, size);
        //            } while (size > 0);

        //            fileStreamOut.Close();
        //            file_list.Add(new FileInfo(file_path));
        //            entry = zipInStream.GetNextEntry();
        //        }


        //        zipInStream.Close();
        //        fileStreamIn.Close();
        //        return file_list;
        //    }
        //    catch (Exception exp)
        //    {
        //        Log("Не удалось разархивировать " + fileName + " error: " + exp.Message);
        //        throw exp;
        //    }
        //}
    }
}

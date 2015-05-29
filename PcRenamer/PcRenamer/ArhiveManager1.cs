using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Config_classes;
using System.Threading;
using RBClient;
using RBClient.Classes;

namespace RBClientUpdateApplication.Updation
{
    public enum ArhivStates { Begin=0, Arhing = 1, Ready = 2,Error=-1 };
    public class ArhiveManager
    {

        public static FileInfo Archive=null;

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
        
        

        public static DirectoryInfo TempFolder;
        //сделать темповую директорию
        public static void CreateTempFolder()
        {
            MDIParentMain.Log("Создаем темповую директорию " + StaticConstants.TEMP_FOLDER);

            DirectoryInfo dir = new DirectoryInfo(StaticConstants.TEMP_FOLDER);
            if (dir.Exists)
            {
                dir.Delete(true);
            }

            TempFolder = Directory.CreateDirectory(StaticConstants.TEMP_FOLDER);
            MDIParentMain.Log("Темповая директория создана, dir: " + TempFolder.FullName);
        }

        public static FileInfo CreateArchiveFile(string _zipFileName,string release_folder,string dest_folder)
        {
            ArchiveState = ArhivStates.Arhing;
            MDIParentMain.Log("Начинаем создание архива");
            // получить файлы для архивации
            string release_folder_str = release_folder;

            DirectoryInfo dir = new DirectoryInfo(release_folder_str); 
            List<FileInfo> file_list=dir.GetFiles().ToList();
            
            if (file_list == null || file_list.Count == 0) throw new Exception("Нет файлов в директории релиза!");


            string zipFileName = _zipFileName;


            string z_path = Path.Combine(dest_folder, zipFileName);// zipFileName;

            if (File.Exists(z_path))
            {
                File.Delete(z_path);
            }

            ZipOutputStream zipStream = new ZipOutputStream(File.Create(z_path));

            Stack<FileInfo> files_toArhive = DirExplore(release_folder_str);

            files_toArhive.ToList<FileInfo>().ForEach(a=>{
             
                string entryName = a.FullName.Remove(0, release_folder_str.Length);
                ZipEntry zipEntry = new ZipEntry(entryName);
                zipStream.PutNextEntry(zipEntry);
                byte[] buffer = File.ReadAllBytes(a.FullName);
                zipStream.Write(buffer, 0, buffer.Length);
                zipStream.CloseEntry();

            });

            zipStream.Close();            
            
            MDIParentMain.Log("Архив создан, file: "+z_path);
            return new FileInfo(z_path);
        }


        static private Stack<FileInfo> DirExplore(string stSrcDirPath)
        {
            try
            {
                Stack<DirectoryInfo> stackDirs = new Stack<DirectoryInfo>();
                Stack<FileInfo> stackPaths = new Stack<FileInfo>();

                DirectoryInfo dd = new DirectoryInfo(Path.GetFullPath(stSrcDirPath));

                stackDirs.Push(dd);
                while (stackDirs.Count > 0)
                {
                    DirectoryInfo currentDir = (DirectoryInfo)stackDirs.Pop();

                    try
                    {
                        //Process .\files
                        foreach (FileInfo fileInfo in currentDir.GetFiles())
                        {
                            stackPaths.Push(fileInfo);
                        }

                        //Process Subdirectories
                        foreach (DirectoryInfo diNext in currentDir.GetDirectories())
                            stackDirs.Push(diNext);
                    }
                    catch (Exception)
                    {//Might be a system directory
                    }
                }
                return stackPaths;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //сделать архив
        //public static void CreateArhiveThread()
        //{
        //    MDIParentMain.Log("Создаем поток архивации");
        //    Thread thread = new Thread(new ThreadStart(CreateArhive));
        //    thread.Name = "Create_archive_thread";
        //    thread.Start();
        //}

        //public static void CreateArhive()
        //{
        //    try
        //    {
        //        ArchiveState = ArhivStates.Begin;

        //        //создать темповую директорию
        //        CreateTempFolder();


        //        //сделать архив
        //        FileInfo arhiv = CreateArchiveFile();

        //        //созранить файл
        //        ArchiveState = ArhivStates.Ready;
        //        Archive=arhiv;

        //    }
        //    catch (Exception ex)
        //    {
        //        ArchiveState = ArhivStates.Error;
        //        MDIParentMain.Log(ex, "Не удалось создать архив!");
        //    }
        //}

        //очистить темп директорию
        private static void Release()
        {
            if (TempFolder != null)
            {
                TempFolder.Delete(true);
            }
        }

        private static void Log(string message)
        {
            MDIParentMain.Log(message);
        }
    }
}

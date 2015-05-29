using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Config_classes;
using System.Threading;

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
            Root.Log("Создаем темповую директорию "+ Root.TEMP_FOLDER_NAME);

            DirectoryInfo dir = new DirectoryInfo(Root.TEMP_FOLDER_NAME);
            if (dir.Exists)
            {
                dir.Delete(true);
            }

            TempFolder = Directory.CreateDirectory(Root.TEMP_FOLDER_NAME);
            Root.Log("Темповая директория создана, dir: " + TempFolder.FullName);
        }

        public static FileInfo CreateArchiveFile()
        {
            ArchiveState = ArhivStates.Arhing;
            Root.Log("Начинаем создание архива");
            // получить файлы для архивации
            string release_folder_str = ConfigClass.GetProperty("RBClientReleaseFolder").ToString();
            DirectoryInfo dir = new DirectoryInfo(release_folder_str); 
            List<FileInfo> file_list=dir.GetFiles().ToList();
            
            if (file_list == null || file_list.Count == 0) throw new Exception("Нет файлов в директории релиза!");


            string zipFileName = ConfigClass.GetProperty("RBClientArchiveName").ToString();
            string z_path = Path.Combine(TempFolder.FullName, zipFileName);// zipFileName;

            if (File.Exists(z_path))
            {
                File.Delete(z_path);
            }

            ZipOutputStream zipStream = new ZipOutputStream(File.Create(z_path));
            

            //file_list.ForEach(a =>
            //{
            //    ZipEntry zipEntry = new ZipEntry(a.Name);
            //    zipStream.PutNextEntry(zipEntry);
            //    byte[] buffer = File.ReadAllBytes(a.FullName);
            //    zipStream.Write(buffer, 0, buffer.Length);
            //    zipStream.CloseEntry();
            //});

            Stack<FileInfo> files_toArhive = DirExplore(release_folder_str);

            files_toArhive.ToList<FileInfo>().ForEach(a=>{
                //FileStream fs = new FileStream(a.FullName,FileMode.Open);
                //byte[] buffer = new byte[fs.Length];
                //fs.Read(buffer, 0, buffer.Length);
                //string entryName = a.FullName.Remove(0, release_folder_str.Length + 1);
                //ZipEntry ze = new ZipEntry(entryName
                string entryName = a.FullName.Remove(0, release_folder_str.Length + 1);
                ZipEntry zipEntry = new ZipEntry(entryName);
                zipStream.PutNextEntry(zipEntry);
                byte[] buffer = File.ReadAllBytes(a.FullName);
                zipStream.Write(buffer, 0, buffer.Length);
                zipStream.CloseEntry();

            });

            zipStream.Close();            
            
            Root.Log("Архив создан, file: "+z_path);
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
        public static void CreateArhiveThread()
        {
            Root.Log("Создаем поток архивации");
            Thread thread = new Thread(new ThreadStart(CreateArhive));
			thread.Name = "Create_archive_thread";
			thread.Start();
        }

        public static void CreateArhive()
        {
            try
            {
                ArchiveState = ArhivStates.Begin;

                //создать темповую директорию
                CreateTempFolder();


                //сделать архив
                FileInfo arhiv = CreateArchiveFile();

                //созранить файл
                ArchiveState = ArhivStates.Ready;
                Archive=arhiv;

            }
            catch (Exception ex)
            {
                ArchiveState = ArhivStates.Error;
                Root.Log(ex, "Не удалось создать архив!");
            }
        }

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
            Root.Log(message);
        }
    }
}

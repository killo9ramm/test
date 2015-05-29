using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RBServer.Debug_classes;

namespace Debug_classes
{
    public class FileSystemHelper
    {
        public delegate void MessageEventHandler(object o, MessageEventArgs e);
        public static event MessageEventHandler LogEvent;
        public static event MessageEventHandler TraceEvent;
        private static void Log(string message)
        {
            if (null != LogEvent)
            {
                MessageEventArgs mess = new MessageEventArgs(message);
                LogEvent(null, mess);
            }
        }
        private static void Trace(string message)
        {
            if (null != TraceEvent)
            {
                MessageEventArgs mess = new MessageEventArgs(message);
                TraceEvent(null, mess);
            }
        }

        /// <summary>
        /// Ищет указанный файл в папке rootDir
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="rootDir"></param>
        /// <returns></returns>
        public static FileInfo findFile(string fileName, DirectoryInfo rootDir)
        {
            try
            {
                FileInfo file = rootDir.GetFiles(fileName)[0];
                return file;
            }
            catch (Exception exp)
            {
                Log("Не удалось найти файл " + fileName + " error: " + exp.Message);
                throw exp;
            }
        }

        /// <summary>
        /// Ищет указанную директорию
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static DirectoryInfo findFolder(string folderName)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                DirectoryInfo inbox_folder = dir.GetDirectories(folderName)[0];
                return inbox_folder;

            }
            catch (Exception exp)
            {
                Log("Не удалось найти директорию " + folderName + " error: " + exp.Message);
                throw exp;
            }
        }

        /// <summary>
        /// создаем или освобождаем директорию
        /// </summary>
        /// <param name="folderName">имя создаваемой папки</param>
        /// <returns></returns>
        public static System.IO.DirectoryInfo makeFolder(string folderName)
        {
            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(folderName);
                if (dir.Exists)
                {
                    dir.Delete(true);
                }

                dir.Create();
                return dir;
            }
            catch (Exception exp)
            {
                Log("Не удалось создать директорию " + folderName + " error " + exp.Message);
                return null;
            }
        }


        /// <summary>
        /// заменяет файлы
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destFileName"></param>
        public static void MoveWithReplace(string sourceFileName, string destFileName)
        {
            try
            {
                //first, delete target file if exists, as File.Move() does not support overwrite
                if (File.Exists(destFileName))
                {
                    File.Delete(destFileName);
                }
                File.Move(sourceFileName, destFileName);
            }catch(Exception exp){
                Log("Не удалось переместить файл " + sourceFileName + " в " + destFileName + " error " + exp.Message);
                throw exp;
            }
        }

        /// <summary>
        /// заменяет файлы в папке
        /// </summary>
        /// <param name="file_list"></param>
        /// <param name="inboxFolder"></param>
        public static void ReplaseFiles(List<FileInfo> file_list, DirectoryInfo Folder)
        {
            string source_path = "";
            string dest_path = "";

            try
            {
                foreach(FileInfo fileIn in file_list)
                {
                    source_path = fileIn.FullName;

                    string folders="";
                    string s_path = source_path.Replace(Path.Combine(Folder.FullName,"temp"), "");

                    string f_name = DebugPanel.getFileName(s_path, ref folders, Folder.FullName);

                    //dest_path = Path.Combine(Folder.FullName, s_path);
                    dest_path = Folder.FullName+s_path;

                    MoveWithReplace(source_path, dest_path);
                }
            }
            catch (Exception exp)
            {
                Log("Не скопировать файл "+source_path +" в " + dest_path + " error " + exp.Message);
            }
        }
    }
}

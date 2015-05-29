using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using RBServer.Debug_classes;

namespace Debug_classes
{
    class ZipHelper
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
        /// разархивировать файл
        /// </summary>
        /// <param name="fileName">имя файла архива</param>
        /// <param name="directory_name">папка куда разархивировать</param>
        public static void UnZipFileVoid(string fileName, string directory_name)
        {
            try
            {
                System.IO.DirectoryInfo dir = FileSystemHelper.makeFolder(directory_name);

                FileStream fileStreamIn = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                ZipInputStream zipInStream = new ZipInputStream(fileStreamIn);
                ZipEntry entry = zipInStream.GetNextEntry();

                while (null != entry)
                {
                    FileStream fileStreamOut = new FileStream(Path.Combine(directory_name, entry.Name), FileMode.Create, FileAccess.Write);
                    int size;
                    byte[] buffer = new byte[4096];
                    do
                    {
                        size = zipInStream.Read(buffer, 0, buffer.Length);
                        fileStreamOut.Write(buffer, 0, size);
                    } while (size > 0);

                    fileStreamOut.Close();
                    entry = zipInStream.GetNextEntry();
                }


                zipInStream.Close();
                fileStreamIn.Close();
            }
            catch (Exception exp)
            {
                Log("Не удалось разархивировать " + fileName + " error: " + exp.Message);
                throw exp;
            }
        }

        /// <summary>
        /// разархивировать файл
        /// </summary>
        /// <param name="fileName">имя файла архива</param>
        /// <param name="directory_name">папка куда разархивировать</param>
        public static List<FileInfo> UnZipFile(string fileName, string directory_name)
        {
            try
            {
                List<FileInfo> file_list = new List<FileInfo>();
                System.IO.DirectoryInfo dir = FileSystemHelper.makeFolder(directory_name);

                FileStream fileStreamIn = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                ZipInputStream zipInStream = new ZipInputStream(fileStreamIn);
                ZipEntry entry = zipInStream.GetNextEntry();

                while (null != entry)
                {
                    string file_path=Path.Combine(directory_name, entry.Name);
                    FileStream fileStreamOut = new FileStream(file_path, FileMode.Create, FileAccess.Write);
                    int size;
                    byte[] buffer = new byte[4096];
                    do
                    {
                        size = zipInStream.Read(buffer, 0, buffer.Length);
                        fileStreamOut.Write(buffer, 0, size);
                    } while (size > 0);

                    fileStreamOut.Close();
                    file_list.Add(new FileInfo(file_path));
                    entry = zipInStream.GetNextEntry();
                }


                zipInStream.Close();
                fileStreamIn.Close();
                return file_list;
            }
            catch (Exception exp)
            {
                Log("Не удалось разархивировать " + fileName + " error: " + exp.Message);
                throw exp;
            }
        }
    }
}

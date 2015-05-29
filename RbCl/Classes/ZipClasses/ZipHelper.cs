using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using RBServer.Debug_classes;
using ICSharpCode.SharpZipLib.Core;

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
                    if (entry.IsDirectory) { entry = zipInStream.GetNextEntry(); continue; }
                    string folders = "";
                    DebugPanel.getFileName(entry.Name, ref folders, directory_name);

                    string file_path=Path.Combine(directory_name, entry.Name).Replace("/",@"\");

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


        public static void ExtractZipFile(string archiveFilenameIn, string password, string outFolder)
        {
            ZipFile zf = null;
            try
            {
                FileStream fs = System.IO.File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);
                if (!String.IsNullOrEmpty(password))
                {
                    zf.Password = password;     // AES encrypted entries are handled automatically
                }
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = System.IO.File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }
    }
}

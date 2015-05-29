using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBServer.Debug_classes;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Debug_classes;
using ProcessServiceWorker;
using System.ComponentModel;

namespace RBClientUpdate
{
    class Root
    {
        private const string INBOXFOLDER = "Inbox";
        private const string RBFILE = "RBClient.exe";

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

       



        public static void Operate_Zip(BackgroundWorker sender, DoWorkEventArgs e)
        {
            try
            {
                //проверить и убить процесс
                ProcessWorker.StopProcess(RBFILE.Substring(0,RBFILE.IndexOf('.')));
                sender.ReportProgress(10);

                //найти файл в папке инбокс
                DirectoryInfo inboxFolder = FileSystemHelper.findFolder(INBOXFOLDER);
                sender.ReportProgress(20);

                FileInfo fileName = FileSystemHelper.findFile(RBFILE, inboxFolder);
                sender.ReportProgress(30);

                //создать темповую директорию
                DirectoryInfo dir = FileSystemHelper.makeFolder("temp");
                sender.ReportProgress(40);

                //разархивировать файлы
                List<FileInfo> file_list = ZipHelper.UnZipFile(fileName.FullName, dir.FullName);
                sender.ReportProgress(50);

                //заменить файлы
                FileSystemHelper.ReplaseFiles(file_list,new DirectoryInfo(Directory.GetCurrentDirectory()));
                sender.ReportProgress(60);

                //очистить папку
                dir.Delete(true);
                sender.ReportProgress(70);

                //очистить файл
#if(!DEBUG)
                fileName.Delete();
                sender.ReportProgress(100);
#endif

            }
            catch(Exception exp)
            {
                Log("Обновление не удалось!!! error: " + exp.Message);
            }
        }
    }
}

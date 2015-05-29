using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace RBServer.Debug_classes
{
    internal class FileInfo
    {
        internal static event EventHandler MoveEvent;
        internal static event EventHandler DeleteEvent;

        private FileInfo()
        {
        }

        private System.IO.FileInfo file;

        internal FileInfo(System.IO.FileInfo fi)
        {
            file = fi;
        }

        internal FileInfo(string path)
        {
            file = new System.IO.FileInfo(path);
        }

        internal string Name
        {
            get
            {
                return file.Name;
            }
        }

        internal bool Exists
        {
            get
            {
                return file.Exists;
            }
        }

        internal long Length
        {
            get
            {
                return file.Length;
            }
        }

        internal DateTime CreationTime
        {
            get
            {
                return file.CreationTime;
            }
        }

        internal string FullName
        {
            get
            {
                return file.FullName;
            }
        }

        internal void Delete()
        {
            try
            {
                if (DeleteEvent != null)
                {
                    DeleteEvent(new ArrayList() { file }, null);
                }
                file.Delete();
            }catch(Exception ex){
                string message = "Не удалось удалить файл " + file.FullName + " ошибка " + ex.Message;
                DebugPanel.OnFatalErrorOccured(ex, message);
            }
        }

        internal void MoveTo(string destination)
        {
            try
            {
                if (MoveEvent != null)
                {
                    MoveEvent(new ArrayList() { file, destination }, null);
                }
                file.MoveTo(destination);
            }catch(Exception ex)
            {
                string message= "Не удалось переместить файл " + file.FullName + " в "+destination + " ошибка "+ex.Message;
                DebugPanel.OnFatalErrorOccured(ex,message);
            }
        }

    }

   

   
}

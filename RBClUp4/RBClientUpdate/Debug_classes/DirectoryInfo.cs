using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBServer.Debug_classes
{
    internal class DirectoryInfo
    {
        private DirectoryInfo()
        {
        }

        private System.IO.DirectoryInfo dir;
        private List<FileInfo> flist;
        private List<DirectoryInfo> dlist;

        internal DirectoryInfo(string path)
        {
            dir = new System.IO.DirectoryInfo(path);
            flist = new List<FileInfo>();
            dlist = new List<DirectoryInfo>();
        }

        internal string FullName
        {
            get
            {
                return dir.FullName;
            }
        }

        internal string Name
        {
            get
            {
                return dir.Name;
            }
        }

        internal List<FileInfo> GetFiles()
        {
            flist = new List<FileInfo>();
            try
            {
                    dir.GetFiles().ToList().ForEach(a => flist.Add(new FileInfo(a)));
            }catch(Exception ex)
            {
                string message="Нет файлов в папке "+dir.FullName+" Exception "+ex.Message;
                DebugPanel.Log(message);
                //DebugPanel.OnFatalErrorOccured(message);
            }
            return flist;
        }

        internal List<DirectoryInfo> GetDirectories()
        {
            dlist = new List<DirectoryInfo>();
            try
            {
                dir.GetDirectories().ToList().ForEach(a => dlist.Add(new DirectoryInfo(a.FullName)));
            }
            catch (Exception ex)
            {
                string message="Нет папок в папке " + dir.FullName + " Exception " + ex.Message;
                DebugPanel.Log(message);
                DebugPanel.OnFatalErrorOccured(message);
            }
            return dlist;
        }
    }
}

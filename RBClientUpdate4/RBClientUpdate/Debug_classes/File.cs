using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace RBServer.Debug_classes
{
    internal class File
    {
        internal static event EventHandler CopyEvent;                
        internal static event EventHandler MoveEvent;
        internal static event EventHandler DeleteEvent;

        internal static void Copy(string source,string destination,bool overwrite)
        {
            try
            {
                System.IO.File.Copy(source, destination, overwrite);
                if (CopyEvent != null)
                {
                    CopyEvent(new ArrayList() { source, destination, overwrite }, null);
                }
            }catch(Exception ex){
                string message = "Не удалось копировать файл " + source + " в " + destination + " ошибка " + ex.Message;
                DebugPanel.OnFatalErrorOccured(ex, message);
            }
        }

        internal static void Copy(string source, string destination)
        {
            try
            {
                System.IO.File.Copy(source, destination);
                if (CopyEvent != null)
                {
                    CopyEvent(new ArrayList() { source }, null);
                }
            }catch(Exception ex){
                string message = "Не удалось копировать файл " + source + " в " + destination + " ошибка " + ex.Message;
                DebugPanel.OnFatalErrorOccured(ex, message);
            }
        }

        internal static string ReadAllText(string source,Encoding enc)
        {
            return System.IO.File.ReadAllText(source,enc);
        }

        internal static bool Exists(string source)
        {
            return System.IO.File.Exists(source);
        }

        internal static void Delete(string source)
        {
            try
            {
                if (DeleteEvent != null)
                {
                    DeleteEvent(new ArrayList() { source }, null);
                }
                System.IO.File.Delete(source);
            }catch(Exception ex){
                string message = "Не удалось удалить файл " + source + " ошибка " + ex.Message;
                DebugPanel.OnFatalErrorOccured(ex, message);
            }
        }

        internal static void Move(string source, string destination)
        {
            try
            {
                if (MoveEvent != null)
                {
                    MoveEvent(new ArrayList() { source, destination }, null);
                }
                System.IO.File.Move(source, destination);
            }catch(Exception ex){
                string message = "Не удалось переместить файл " + source + " в " + destination + " ошибка " + ex.Message;
                DebugPanel.OnFatalErrorOccured(ex, message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net.Mail;
using System.Net;
using RBClient;
using RBClient.Classes;

namespace RBServer.Debug_classes
{
    public class MessageEventArgs : EventArgs
    {
        public string Message="";
        public object Object;
        public MessageEventArgs(string message)
        {
            Message = message;
        }
        public MessageEventArgs(){}

        public override string ToString()
        {
            string st = "";
            if (Message != "")
            {
                st +=" Message:"+ Message+"\r\n";
            }

            if (Object != null)
            {
                st += " Object:" + Object.ToString() + "\r\n";
            }
            return st;
        }
    }
    #region trash

    public delegate void MessageEventHandler(object o, MessageEventArgs e);
    internal partial class DebugPanel
    {
        internal static event EventHandler FileCopied;
        internal static event EventHandler FileMoved;
        internal static event EventHandler FileDeleted;

        
        internal static event MessageEventHandler FatalErrorOccured;
        internal static event MessageEventHandler ErrorOccured;

        internal static void OnFatalErrorOccured(object obj,string message)
        {
            Log("FatalError: " + message + "object " + obj.ToString());
            MessageEventArgs ce = new MessageEventArgs(message);
            if (FatalErrorOccured!=null)
            {
                FatalErrorOccured(obj, ce);
            }
            
        }
        internal static void OnFatalErrorOccured(string message)
        {
            Log("FatalError: " + message);
            MessageEventArgs ce = new MessageEventArgs(message);
            if (FatalErrorOccured != null)
            {
                FatalErrorOccured(null, ce);
            }

        }
        internal static void OnFatalErrorOccured(object obj)
        {
            Log("FatalError: " + "object " + obj.ToString());
            MessageEventArgs ce = new MessageEventArgs();
            if (FatalErrorOccured != null)
            {
                FatalErrorOccured(obj, ce);
            }

        }

        internal static void OnErrorOccured(object obj, string message)
        {
            Log("Error: " + message+ "object "+obj.ToString());
            MessageEventArgs ce = new MessageEventArgs(message);
            if (ErrorOccured != null)
            {
                ErrorOccured(obj, ce);
            }

        }
        internal static void OnErrorOccured(string message)
        {
            Log("Error: " + message);
            MessageEventArgs ce = new MessageEventArgs(message);
            if (ErrorOccured != null)
            {
                ErrorOccured(null, ce);
            }

        }
        internal static void OnErrorOccured(object obj)
        {
            Log("Error: " +"object " + obj.ToString());
            MessageEventArgs ce = new MessageEventArgs();
            if (ErrorOccured != null)
            {
                ErrorOccured(obj, ce);
            }

        }


        private static Regex fname_regx = new Regex(@"[a-z_A-Z10-9а-яА-я.() -]*[.][a-zA-Z0-9]+");
        private static List<string> Remove_symb = new List<string>() { ":" };
        private static List<string> Replace_symb = new List<string>() { "\\","/" };
        private static string Replace_value = @"\";

        public static string getFileName(string fullpath, ref string folders, string back_folder_path)
        {
            Remove_symb.ForEach(s => fullpath = fullpath.Replace(s, ""));
            Replace_symb.ForEach(s => fullpath = fullpath.Replace(s, Replace_value));
            //if (fullpath.IndexOf("/") != -1)
            //{
            //    fullpath = fullpath.reSubstring(0, fullpath.IndexOf("/"));
            //}
            List<string> folders_list = fullpath.Split(new string[] { Replace_value }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();


            string filename = fname_regx.Match(fullpath).Value;
            folders_list.Remove(filename);

            System.IO.DirectoryInfo parent_dir = checkFolderExist(back_folder_path);

            folders = CreateFolderStructure(folders_list, parent_dir);

            return filename;
        }

        public static string CreateFolderStructure(List<string> folder_list, System.IO.DirectoryInfo parent_dir)
        {
            System.IO.DirectoryInfo child_dir;

            if (folder_list.Count == 0)
            {
                return parent_dir.FullName;
            }
            string nf = folder_list.First<string>();

            string new_folder = System.IO.Path.Combine(parent_dir.FullName, nf);

            child_dir = checkFolderExist(new_folder);

            folder_list.Remove(nf);
            return CreateFolderStructure(folder_list, child_dir);
        }

        public static System.IO.DirectoryInfo checkFolderExist(string ch_path)
        {
            System.IO.DirectoryInfo dir = null;
            try
            {
                if (System.IO.Directory.Exists(ch_path))
                {
                    dir = new System.IO.DirectoryInfo(ch_path);
                }
                else
                {
                    dir = System.IO.Directory.CreateDirectory(ch_path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не могу получить доступ к директории " + ch_path);
                throw new Exception();
            }
            return dir;
        }

        private static string log_path;

        internal static void Log(string message)
        {
            try
            {
                MDIParentMain.Log(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Debug_Panel.log error " + ex.Message);
            }
        }

        private static Regex method_Name = new Regex(@"^[a-z_A-Z0-9]+");

        internal static string InMethod(int from_frame, int length)
        {
            string mess = "---------------(техническая информация)\r\n";

            List<StackFrame> sfl = new StackTrace().GetFrames().ToList<StackFrame>();
            int j = sfl.Count;
            if (from_frame + length < sfl.Count)
            {
                j = from_frame + length;
            }
            for (int i = from_frame; i < j; i++)
            {
                mess += method_Name.Match(sfl[i].ToString()) + "  ";
            }
            return mess;
        }


        public static void SaveEmailToDisk(System.IO.DirectoryInfo child_dir, MailMessage mm)
        {
            try
            {
                if (mm != null)
                {
                    Log("Сохраняем сообщение на диск " + mm.Subject);
                    System.IO.FileStream fs = new System.IO.FileStream(DateTime.Now.ToString() + mm.Subject + ".eml",
                        System.IO.FileMode.OpenOrCreate
                        , System.IO.FileAccess.ReadWrite);
                    var a = mm.RawMessage();
                    a.WriteTo(fs);
                    fs.Close();
                    a.Close();
                }
            }
            catch (Exception ex)
            {
                Log("Не получилось сохранить сообщение на диск " + ex.Message);
            }
        }
    }

    #endregion
}

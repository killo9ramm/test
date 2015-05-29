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


//        private static string _back_folder_path = @"C:\RBSERV_PRODUCTION";
//        public static string back_folder_path
//        {
//            get
//            {
//                return _back_folder_path;
//            }
//            set
//            {
//                _back_folder_path = value;
//            }
//        }

//        public static string admin_email = "krigel.s@teremok.ru";

        private static string log_path;

//        private static bool _on=false;
//        internal static bool IsON
//        {
//            set
//            {
//                if (_on != value)
//                {
//                    if (value)
//                    {

//                        back_folder_path = checkFolderExist(back_folder_path).FullName;

//                        File.MoveEvent += CreateDelMoveFolderTree;
//                        FileInfo.MoveEvent += CreateDelMoveFolderTree;
//                        File.DeleteEvent += CreateDelMoveFolderTree;
//                        FileInfo.DeleteEvent += CreateDelMoveFolderTree;
//                        Log("Дебаг включен!");
//                    }
//                    else
//                    {
//                        File.MoveEvent -= CreateDelMoveFolderTree;
//                        FileInfo.MoveEvent -= CreateDelMoveFolderTree;
//                        File.DeleteEvent -= CreateDelMoveFolderTree;
//                        FileInfo.DeleteEvent -= CreateDelMoveFolderTree;
//                        Log("Дебаг выключен!");
//                    }
//                    _on = value;
//                }
//            }
//        }

//        private static bool _smon = false;
//        internal static bool SendAdminMessageIsON
//        {
//            set
//            {
//                if (_smon != value)
//                {
//                    if (value)
//                    {
//                        ErrorOccured += SendErrorMessageToAdmin;
//                        FatalErrorOccured += SendErrorMessageToAdmin;
//                        Log("Отсылка сервисных сообщений включена!");
//                    }
//                    else
//                    {
//                        ErrorOccured -= SendErrorMessageToAdmin;
//                        FatalErrorOccured -= SendErrorMessageToAdmin;
//                        Log("Отсылка сервисных сообщений выключена!");
//                    }
//                    _smon = value;
//                }
//            }
//        }


//        static DebugPanel()
//        {
//            log_path = "rbs_log_temp.txt";
//        }

//        private static FileInfo logfile;

        internal static void Log(string message)
        {
            try
            {
                DateTime dt = DateTime.Now;
                string method = InMethod(4, 7);

                string mess = dt.ToString() + " --- " + message + " --- " + " in " + method + "\r\n";
                System.IO.File.AppendAllText(log_path, mess);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Debug_Panel.log error " + ex.Message);
            }
        }

        private static Regex method_Name = new Regex(@"^[a-z_A-Z0-9]+");

//        internal static void GetLocalVars(StackFrame stf)
//        {
//            var frame = stf;
//            var methodName = frame.GetMethod().Name;
//            //var properties = this.GetType().GetProperties();
//            //var fields = this.GetType().GetFields(); // public fields
//            //// for example:
//            //foreach (var prop in properties)
//            //{
//            //    var value = prop.GetValue(this, null);
//            //}
//            //foreach (var field in fields)
//            //{
//            //    var value = field.GetValue(this);
//            //}
//            //foreach (string key in Session)
//            //{
//            //    var value = Session[key];
//            //}
//        }

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

//        #region filesystem routine
//        internal static void CreateDelMoveFolderTree(object sender,EventArgs e)
//        {
//            ArrayList list=sender as ArrayList;

//            if (sender == null)
//            {
//                Log("не удалось определить тип sender " + sender.GetType().Name);
//                throw new Exception("не удалось определить тип sender " + sender.GetType().Name);
//            }
//            else
//            {
//               CreateFileCopy(list[0]);
//            }
//        }

//        private static void CreateFileCopy(object obj)
//        {
//            if (obj is string) { CreateFileCopy(obj as string); }
//            if (obj is System.IO.FileInfo) { CreateFileCopy(obj as System.IO.FileInfo); }
//        }

//        private static void CreateFileCopy(System.IO.FileInfo path)
//        {
//            CreateFileCopy(path.FullName);
//        }
        
//        private static void CreateFileCopy(string path)
//        {
//            string folders = "";
//            string fileName=getFileName(path,ref folders);
//            System.IO.File.Copy(path, System.IO.Path.Combine(folders,fileName),true);
//            Log("Сделана копия " + fileName + " путь " + folders);
//        }

//        private static Regex fname_regx = new Regex(@"[a-z_A-Z10-9а-яА-я.]*[.][a-zA-Z0-9]+");
//        private static List<string> Remove_symb = new List<string>() {":"};
//        private static List<string> Replace_symb = new List<string>() {"\\"};
//        private static string Replace_value = @"\";

//        private static string getFileName(string fullpath, ref string folders)
//        {
//            Remove_symb.ForEach(s => fullpath=fullpath.Replace(s, ""));
//            Replace_symb.ForEach(s => fullpath=fullpath.Replace(s, Replace_value));

//            List<string> folders_list=fullpath.Split(new string[] { Replace_value },StringSplitOptions.RemoveEmptyEntries).ToList<string>();


//            string filename = fname_regx.Match(fullpath).Value;
//            folders_list.Remove(filename);

//            System.IO.DirectoryInfo parent_dir=checkFolderExist(back_folder_path);

//            folders = CreateFolderStructure(folders_list, parent_dir);
           
//            return filename;
//        }

//        private static System.IO.DirectoryInfo checkFolderExist(string ch_path)
//        {
//            System.IO.DirectoryInfo dir=null;
//            try
//            {
//                if (System.IO.Directory.Exists(ch_path))
//                {
//                    dir = new System.IO.DirectoryInfo(ch_path);
//                }
//                else
//                {
//                    dir = System.IO.Directory.CreateDirectory(ch_path);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Не могу получить доступ к директории " + ch_path);
//                throw new Exception();
//            }
//            return dir;
//        }

//        private static string CreateFolderStructure(List<string> folder_list,System.IO.DirectoryInfo parent_dir)
//        {
//            System.IO.DirectoryInfo child_dir;

//            if (folder_list.Count == 0)
//            {
//                return parent_dir.FullName;
//            }
//            string nf=folder_list.First<string>();

//            string new_folder =System.IO.Path.Combine(parent_dir.FullName,nf);

//            child_dir = checkFolderExist(new_folder);

//            folder_list.Remove(nf);
//            return CreateFolderStructure(folder_list, child_dir);
//        }
//#endregion

//        #region mailsend
        
//        public static SmtpClient ConnectToMailClient(string smtpServer, string smtpLogin, string smtpPassword, string domain)
//        {
//            SmtpClient client = new SmtpClient(smtpServer);
//            client.Credentials = new NetworkCredential(smtpLogin, smtpPassword, domain);
            
//            return client;
//        }
//        public static MailMessage PrepareMessageToSend(string From,string To, string Subject, string Message, bool isHTML)
//        {
//            MailMessage mm = new MailMessage(From, To, Subject, Message);
//            mm.IsBodyHtml = isHTML;

//            #region attachment commented
//            //Attachment attachData = new Attachment(@"\\mskapp\RB_server\RBServer\teremok_reestr_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".csv");
//            //mm.Attachments.Add(attachData);
//            #endregion

//            return mm;
//        }
//        public static bool SendMessage(SmtpClient client, MailMessage mm, SendCompletedEventHandler SendCompletedCallback)
//        {
//            bool result = false;
//            try
//            {
//                client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
//                client.SendAsync(mm, null);/*для асинхронной отправки. При этом в @Page указываем Async="true". Иначе используем client.Send(mm);*/
//                result = true;
//            }
//            catch (Exception Ex)
//            {
//                Log("Ошибка отправки Email " + Ex.Message);
//                result = false;
//            }
            
//            return result;
//        }
//        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
//        {
//            String token = (string)e.UserState;

//            if (e.Cancelled)
//            {
//                Log("[{0}] Send canceled." + token);
//            }
//            if (e.Error != null)
//            {
//                Log(token + " " + e.Error.ToString());
//            }
//            else
//            {
//                Log("Message sent.");
//            }
//        }

//        internal static void SendErrorMessageToAdmin(object sender,EventArgs ex)
//        {
//            SendErrorMessageToAdmin(ex);
//        }

//        internal static void SendErrorMessageToAdmin(EventArgs ex)
//        {
//            CConfig _conf = new CConfig();
//            string message=ex.ToString();
//            string subject = message.Substring(0, message.Trim().IndexOf(' ')+1);
//            SendMail(_conf, admin_email, subject, ex.ToString(), false, "msk");
//        }

//        public static void SendMail(string smtp_server, string smtp_Login, string smtp_pass, string smtp_domain, string From, string To, string Subject, string Message, bool isHTML)
//        {
//            try
//            {
//                SmtpClient client = ConnectToMailClient(smtp_server, smtp_Login, smtp_pass, smtp_domain);
//                MailMessage mm = PrepareMessageToSend(From, To, Subject, Message, isHTML);
//                SendMessage(client, mm, SendCompletedCallback);
//            }
//            catch (Exception Ex)
//            {
//                Log("Ошибка создания Email " + Ex.Message);
//            }
//        }

////        internal static void SendMail(CConfig _conf, string To, string Subject, string Message, bool isHTML,string domain)
////        {
////            try
////            {
////                string From = _conf.m_send_from;
////                string smtpLogin = _conf.m_smtp_login; if (string.IsNullOrEmpty(From)) From = smtpLogin;

////                SmtpClient client = ConnectToMailClient(_conf.m_smtp_server, smtpLogin, _conf.m_smtp_pass, domain);
////                MailMessage mm = PrepareMessageToSend(From, To, Subject, Message, isHTML);
////                SendMessage(client, mm, SendCompletedCallback);
////#if(!DEB)
////                SendMessage(client, mm, SendCompletedCallback);
////#endif
////            }
////            catch (Exception Ex)
////            {
////                Log("Ошибка создания Email " + Ex.Message);
////            }
////        }

//        #endregion


    }

    #endregion
}

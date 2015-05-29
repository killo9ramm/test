using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Mail;
using RBClient.Classes.CustomClasses;
using System.Net;
using RBServer.Debug_classes;

namespace RBClient.Classes
{
    partial class ErrorNotifications
    {
        public static void Log(string message)
        {
            MDIParentMain.Log(message);
        }

        public static void Log(Exception ex,string message)
        {
            MDIParentMain.Log(ex,message);
        }

        public static void SendMail(string To, string Subject, string Message)
        {
            try
            {
                ThreadPool.QueueUserWorkItem((a) =>
                {
                    SendMailSync(To, Subject, Message, false, "");
                });
            }
            catch (Exception ex)
            {
                Log(ex, "SendMail error");
            }
        }

        public static string SendMailSync(string To, string Subject, string Message, bool isHTML, string attachment_filename)
        {
            Log("Пытаемся отправить email: " + Subject);
            string From = StaticConstants.RBINNER_CONFIG.GetProperty<string>("send_from", "server_arm@teremok.ru");
            string ret = string.Empty;
            string smtpServer = StaticConstants.RBINNER_CONFIG.GetProperty<string>("smtp_server", "msk.teremok.ru");
            SmtpClient client = new SmtpClient(smtpServer);
            client.Port = StaticConstants.RBINNER_CONFIG.GetProperty<int>("send_port", 2525);
            string smtpLogin = StaticConstants.RBINNER_CONFIG.GetProperty<string>("smtp_login", "server_arm");
            if (string.IsNullOrEmpty(From))
                From = smtpLogin;
            string domain = "msk";
            string smtpPassword = PasswordDecoder.decode_string(
                StaticConstants.RBINNER_CONFIG.GetProperty<string>("smtp_pass", "Ⴔ먐맾먋먍먒ミミ"));

            client.Credentials = new NetworkCredential(smtpLogin, smtpPassword, domain);

            
            try
            {
                MailMessage mm = new MailMessage(From, To, Subject, Message);

                
                if(StaticConstants.RBINNER_CONFIG.GetProperty<bool>("save_eml_to_disk",false))
                {
                    DebugPanel.SaveEmailToDisk(new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory())
                        ,mm);
                }
                

                if (attachment_filename != "")
                {
                    mm.Attachments.Add(new Attachment(attachment_filename));
                }

                mm.IsBodyHtml = isHTML;
                mm.SubjectEncoding = System.Text.Encoding.UTF8;
                mm.BodyEncoding = System.Text.UTF8Encoding.UTF8;


#if(!DEB)
                      client.Send(mm);
#endif
            }

            #region catches
            catch (FormatException fEx)
            {
                ret = "Ошибка FormatException: " + fEx.Message;
                if (fEx.InnerException != null)
                {
                    ret += "\r\n InnerException:" + fEx.InnerException.Message;
                }
            }
            catch (ArgumentException argEx)
            {
                ret = "Ошибка ArgumentException: " + argEx.Message;
                if (argEx.InnerException != null)
                {
                    ret += "\r\n InnerException:" + argEx.InnerException.Message;
                }
            }
            catch (SmtpFailedRecipientsException faildRecipientEx)
            {
                ret = "Ошибка SmtpFailedRecipientsException: " + faildRecipientEx.Message;
                if (faildRecipientEx.InnerException != null)
                {
                    ret += "\r\n InnerException:" + faildRecipientEx.InnerException.Message;
                }
            }
            catch (SmtpException smtpEx)
            {
                ret = "Ошибка SmtpException: " + smtpEx.Message;
                if (smtpEx.InnerException != null)
                {
                    ret += "\r\n InnerException:" + smtpEx.InnerException.Message;
                }
            }
            catch (InvalidOperationException operationEx)
            {
                ret = "Ошибка InvalidOperationException: " + operationEx.Message;
                if (operationEx.InnerException != null)
                {
                    ret += "\r\n InnerException:" + operationEx.InnerException.Message;
                }
            }
            catch (Exception Ex)
            {
                ret = "Ошибка Exception: " + Ex.Message;
                if (Ex.InnerException != null)
                {
                    ret += "\r\n InnerException:" + Ex.InnerException.Message;
                }
            }
            #endregion
            finally
            {
                if (ret != String.Empty)
                {
                    Log(ret);
                }
                else
                {
                    Log("Отправили email: " + Subject);
                }
            }

            return string.Empty;
        }
    }
}

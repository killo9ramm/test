using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using System.ComponentModel;
using System.Net;

namespace smtpTester
{
    class Class1
    {
        static void Main1(string[] args)
        {
            Console.WriteLine("maxIter sllep_time To");

            string testline = Console.ReadLine();

            string[] parms = testline.Split(' ');

            int maxIter = int.Parse(parms[0]);

            int sleep = int.Parse(parms[1]);

            WaitHandle[] wh_list = new WaitHandle[maxIter];
            MMAil[] mm_list = new MMAil[maxIter];
            

            for (int i = 0; i < maxIter; i++)
            {
                    int j = i;
                    Guid guid = Guid.NewGuid();
                    Console.WriteLine(j + ") Начинаем отправку пиьсма ");
                    MMAil mail = new MMAil();
                    mail.guid = j.ToString();
                    mail.Send(parms[2]);
                    mm_list[j] = mail;   
                
                wh_list[i]=mail.ewh;
                Thread.Sleep(sleep);
            }

            Console.WriteLine("Ожидаем окончания отправки всех сообщений...");
            WaitHandle.WaitAll(wh_list);

            Console.WriteLine("Отправленных: " + mm_list.Where(a => a.sended).Count());
            Console.WriteLine("Ошибок: " + mm_list.Where(a => a.error).Count());

            foreach (MMAil ma in mm_list.Where(a => a.error))
            {
                Console.WriteLine(ma.guid + ") Не отправлено: " + ma.Error);
            }
            Console.ReadKey();
        }

        class MMAil
        {
            public EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
            public MailMessage mm;
            public string guid;
            public bool sending;
            public bool sended;
            public bool error;
            public string Error;

            public string GetMessage()
            {
                string _message = "<html><body><table border=1>";
                _message += "<tr>";
                _message += "<td>Ресторан </td>";
                _message += "<td align=center>Привет</td>";
                _message += "<tr>";
                _message += "<td>Дата создания </td>";
                _message += "<td align=center>Привет</td>";
                _message += "<tr>";
                _message += "<td align=center>Дата подготовки к отправке </td>";
                _message += "<td align=center>Привет</td>";
                _message += "</table></body></html>";
                _message += "<html><body><table border=0>";
                _message += "</table></body></html>";
                return _message;
            }

            public void Send(string To)
            {
                SendMail(To, "Test", GetMessage(), true);
            }

            private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
            {
                // Get the unique identifier for this asynchronous operation.
                String token = (string)e.UserState;

                if (e.Cancelled)
                {
                    //   Console.WriteLine("[{0}] Send canceled.", token);
                    Error = "[{0}] Send canceled." + token;
                    error = true;
                }
                if (e.Error != null)
                {
                    // Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
                    Error = "[{0}] {1} " + token + " " + e.Error.ToString();
                    error = true;
                }
                else
                {
                    //   Console.WriteLine("Message sent.");
                    sended = true;
                }

                   ewh.Set();
            }

            public void SendMail(string To, string Subject, string Message, bool isHTML)
            {
                string From = "server_arm@teremok.ru";
                string ret = string.Empty;

                string smtpServer = "mskexch01";
                SmtpClient client = new SmtpClient(smtpServer);
                string smtpLogin = "server_arm";



                string domain = "msk";
                string smtpPassword = "Qwerty11";

                client.Credentials = new NetworkCredential(smtpLogin, smtpPassword, domain);
                try
                {

                    MailMessage mm = new MailMessage(From, To, Subject, Message);
                    mm.IsBodyHtml = isHTML;
                   client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                //    client.Send(mm);
                client.SendAsync(mm, null);/*для асинхронной отправки. При этом в @Page указываем Async="true". Иначе используем client.Send(mm);*/
                  //  sended = true;
                }

                #region catches
                catch (Exception Ex)
                {
                    error = true;
                    ret = "Ошибка: " + Ex.Message;
                    Error = ret;
                        ewh.Set();
                }
                finally
                {

                }
                #endregion
            }
        }
    }
}


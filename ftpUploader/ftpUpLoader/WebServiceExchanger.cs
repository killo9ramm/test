using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CustomLogger;
using Models;
using ftpUpLoader.ArmServices;
using RBClient.Classes.CustomClasses;
using Config_classes;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;


namespace ftpUpLoader
{
    class WebServiceExchanger : LoggerBase
    {
        public static bool CustomCertificateValidatior(object sender,
    X509Certificate certificate, X509Chain chain,
    SslPolicyErrors policyErrors)
        {
            return true;
        }

        private static Service1 _WebService1 = null;
        public static Service1 WebServiceSystem
        {
            get
            {
                if (_WebService1 == null)
                {
                    string web_url = ConfigClass.GetProperty<string>("service_webservice_url",
                        @"https://msk.teremok.ru:8095/armservice/Service1.asmx");

                    _WebService1 = new Service1();

                    _WebService1.Url = web_url;


                    string login = ConfigClass.GetProperty<string>("service_webservice_login",
                        "arm_services");

                    string passw = PasswordDecoder.decode_string(ConfigClass.GetProperty<string>("service_webservice_password",
                        "먆먀ャႴ먎먄맿ュႧႪユႵ먋먏먐Ⴝ"));

                    if (login != "")
                    {
                        _WebService1.Credentials = new NetworkCredential(login, passw);
                    }

                    string validbs = ConfigClass.GetProperty<string>("service_webservice_validation_by_sert",
                        "0");

                    if (validbs == "0")
                    {
                        ServicePointManager.ServerCertificateValidationCallback = null;
                        ServicePointManager.ServerCertificateValidationCallback +=
                        new RemoteCertificateValidationCallback(CustomCertificateValidatior);
                    }

#if(DEB)

                    //_WebService1.Proxy = new WebProxy("mskisa01.msk.teremok.biz", 8080);
                    //var cr = new NetworkCredential("arm_services", "mg5Qukf7DG8RrvwZ", "msk");
                    //_WebService1.Proxy.Credentials = cr;

#endif
                }
                return _WebService1;
            }
            set
            {
                _WebService1 = value;
            }
        }

        public void SendTreportInfo(FileInfo trep_file)
        {
            try
            {
                DateTime dt = DateTime.Now;
                t_Kkm kkm = Program.KKM;
                ReportHelper rph = new ReportHelper() { LogEvent=Log};
                string text = System.IO.File.ReadAllText(trep_file.FullName, Encoding.GetEncoding(1251));
                string kkm_name = rph.GetKkmName(text);

                kkm.kkm_name = kkm_name;

                rph.GetLastCheckInfo(text, ref kkm.last_check_datetime, ref kkm.last_check_num);

                rph.CheckKKmWorkedToDay(DateTime.Now, kkm);

                kkm.last_treport = trep_file.Name;
                kkm.lasttime_online = dt;
                Program.KKM = kkm;
                
                new CustomAction(o =>
                    {
                        string st = WebServiceSystem.SendKKmZInfo(ConfigClass.GetProperty("terem_name").ToString(),
                            kkm.kkm_name,dt, kkm.workedToDay, kkm.lasttime_online);

                        Log("kkm_UpdateTReport " + st);
                        if (st.IndexOf("1") == -1) throw new Exception("kkm_UpdateTReport package not delivered");
                    }, null) { Timeout = 1000 }.Start();

            }
            catch (Exception ex)
            {
                Log(ex, "Ошибка в kkm_UpdateTReport не удалось обновить kkm_name");
            }
        }

        /// <summary>
        /// Обновить статус кассы
        /// </summary>
        /// <param name="kkmin"></param>
        /// <param name="kkmout"></param>
        /// <param name="online"></param>
        public void kkm_UpdateOnlineState(string kkmin, string kkmout, bool online)
        {
            try
            {
                t_Kkm kkm = Program.KKM;
                if (online)
                    kkm.lasttime_online = DateTime.Now;

                if (kkm.in_folder != kkmin)
                    kkm.in_folder = kkmin;

                kkm.online = online;

                CheckKKmWorkedToDay(DateTime.Now, kkm);

                new CustomAction(o =>
                {
                    string st = WebServiceSystem.SendKKmZInfo(ConfigClass.GetProperty("terem_name").ToString(),
                        kkm.kkm_name, DateTime.Now, kkm.workedToDay, kkm.lasttime_online);
                    if (st.IndexOf("1") == -1) throw new Exception("kkm_UpdateOnlineState package not delivered");
                }, null) { Timeout = 1000 }.Start();


            }
            catch (Exception ex)
            {
                Log(ex, "Ошибка в kkm_UpdateOnlineState не удалось обновить kkm_name");
            }
        }

        private void CheckKKmWorkedToDay(DateTime ToDay, t_Kkm kkm)
        {
            if (kkm.last_check_datetime.Date == ToDay.Date)
            {
                kkm.workedToDay = true;
            }
            else
            {
                kkm.workedToDay = false;
            }
        }
       
        /// <summary>
        /// Подтверждение что z-отчет отправлен
        /// </summary>
        /// <param name="zname">имя z-отчета</param>
        /// <param name="ToDay">текущая дата</param>
        internal void kkm_ConfirmZReportSended(FileInfo zrep_file, DateTime ToDay)
        {
            try
            {
                string[] z_file = GetZInfoFromZname(zrep_file.Name);
                if (z_file == null) return;
                new CustomAction(o =>
                {

                    var result_str = WebServiceSystem.ConfirmZreportSended
                        (ConfigClass.GetProperty("terem_name").ToString(), z_file[3], ToDay, z_file[1], true);
                    if (result_str.IndexOf("1") == -1) throw new Exception("kkm_ConfirmZReportSended package not delivered");

                }, null) { Timeout = 1000 }.Start();

            }
            catch (Exception ex)
            {
                Log(ex, "Ошибка в kkm_ConfirmZReportSended");
            }
        }
        private static string[] GetZInfoFromZname(string zrep_file)
        {
            string regg = ConfigClass.GetProperty<string>("z_report_name_reg"
                , @"(\S+?)_(X(\d{6})(\S+?)[.](\S+?))[.]zip");
            Regex reg = new Regex(regg);
            if (reg.IsMatch(zrep_file))
            {
                string[] st = new string[5];
                Match m = reg.Match(zrep_file);
                st[0] = m.Groups[1].Value;
                st[1] = m.Groups[2].Value;
                st[2] = m.Groups[3].Value;
                st[3] = m.Groups[4].Value;
                st[4] = m.Groups[5].Value;
                //1: ATRIU
                //2: X140911999.071
                //3: 140911
                //4: 999
                //5: 071
                return st;
            }
            return null;
        }
    }
}

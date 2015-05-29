using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes;

namespace RBClient
{
    static class CParam
    {
        
        /// <summary>
        /// Папка приложения
        /// </summary>

        public static string AppFolder = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "").Replace("\\bin\\Release", "");
        //public static string AppFolder = Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "").Replace("\\bin\\Release", "");

        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        /// 
        

        

        public static string ConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;Data Source=" + AppFolder + "\\Data\\RBA.accdb";

       // public static string ConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;Data Source=" + "H:\\trash\\leninsk\\RBA.accdb";
        
        //public static string ConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;Data Source=" + " H:\\trash\\avtoz\\RBA.accdb";
        //  public static string ConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;Data Source=H:\\trash\\spisan\\semen" + "\\RBA.accdb";
        //public static string ConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;Data Source=C:\\git\\RBClient\\RBClient\\bin\\x86\\Debug\\Data\\RBA.accdb";
      //  public static string ConnString = "Provider=Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=False;Data Source=C:\\git\\RBClient\\RBClient\\bin\\x86\\Debug\\Data\\RBA.accdb";
        
        /// <summary>
        /// Строка подключения к папке img
        /// </summary>
        public static string Img_string = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\Img", "").Replace("\\bin\\Release\\Img", "");


        /// <summary>
        /// Текущая версия приложения
        /// </summary>
        public static string AppVer = "";//GetParam(1);

        /// <summary>
        /// Код ресторана
        /// </summary>
        private static string _TeremokId = ""; //GetParam(2);
        public static string TeremokId
        {
            get
            {
                return _TeremokId;
            }
            set
            {
                _TeremokId = value;
                StaticConstants.Teremok_ID = _TeremokId;    
            }
        }

        /// <summary>
        /// Код ресторана
        /// </summary>
        private static string _MainTeremokId = ""; //GetParam(2);
        public static string MainTeremokId
        {
            get
            {
                return _MainTeremokId;
            }
            set
            {
                _MainTeremokId = value;
                StaticConstants.Main_Teremok_ID = _MainTeremokId;
            }
        }

        /// <summary>
        /// Город (1 - СПБ, 2 - Москва)
        /// </summary>
        public static int AppCity = 0; //Convert.ToInt32(GetParam(14));

        /// <summary>
        /// ФТП сервер (основной)
        /// </summary>
        public static string FtpServer1 = ""; //GetParam(6);

        /// <summary>
        /// ФТП порт (основной)
        /// </summary>
        public static string FtpPort1 = ""; //GetParam(7);

        /// <summary>
        /// ФТП логин (основной)
        /// </summary>
        public static string FtpLogin1 = ""; //GetParam(10);

        /// <summary>
        /// ФТП пароль (основной)
        /// </summary>
        public static string FtpPass1 = ""; //GetParam(11);

        /// <summary>
        /// ФТП сервер (резервный)
        /// </summary>
        public static string FtpServer2 = ""; //GetParam(6);

        /// <summary>
        /// ФТП порт (резервный)
        /// </summary>
        public static string FtpPort2 = ""; //GetParam(7);

        /// <summary>
        /// ФТП логин (резервный)
        /// </summary>
        public static string FtpLogin2 = ""; //GetParam(10);

        /// <summary>
        /// ФТП пароль (резервный)
        /// </summary>
        public static string FtpPass2 = ""; //GetParam(11);

        /// <summary>
        /// ФТП сервер (основной)
        /// </summary>
        public static string FtpServer3 = ""; //GetParam(29);

        /// <summary>
        /// ФТП порт (основной)
        /// </summary>
        public static string FtpPort3 = ""; //GetParam(30);

        /// <summary>
        /// ФТП логин (основной)
        /// </summary>
        public static string FtpLogin3 = ""; //GetParam(31);

        /// <summary>
        /// ФТП пароль (основной)
        /// </summary>
        public static string FtpPass3 = ""; //GetParam(32);

        /// <summary>
        /// ФТП сервер (основной)
        /// </summary>
        public static string FtpServer4 = ""; //GetParam(33);

        /// <summary>
        /// ФТП порт (основной)
        /// </summary>
        public static string FtpPort4 = ""; //GetParam(34);

        /// <summary>
        /// ФТП логин (основной)
        /// </summary>
        public static string FtpLogin4 = ""; //GetParam(35);

        /// <summary>
        /// ФТП пароль (основной)
        /// </summary>
        public static string FtpPass4 = ""; //GetParam(36);

        /// <summary>
        /// Таймер обновления (режим при отправке)
        /// </summary>
        public static int TimerSending = 0; //Convert.ToInt32(GetParam(3));

        /// <summary>
        /// Таймер обновления (режим опроса сервера)
        /// </summary>
        public static int TimerExchange = 0; //Convert.ToInt32(GetParam(4));

        /// <summary>
        /// Таймер обновления (режим опроса касс на предмет наличия Z-отчетов)
        /// </summary>
        public static int TimerKKM = 0; //Convert.ToInt32(GetParam(5));

        /// <summary>
        /// Индиктор - нужно ли загружать отчеты по продажам или нет 0 - не нужно, 1 - нужно
        /// </summary>
        public static int LoadReportToDataBase = 1; //Convert.ToInt32(GetParam(23));

        /// <summary>
        /// касса 1 IN
        /// </summary>
        public static string Kkm1In = ""; //GetParam(15);
        public static string kkm1_dep = ""; //GetParam(15);

        /// <summary>
        /// касса 1 OUT
        /// </summary>
        public static string Kkm1Out = ""; //GetParam(16);

        /// <summary>
        /// касса 2 IN
        /// </summary>
        public static string Kkm2In = ""; //GetParam(17);
        public static string kkm2_dep = "";

        /// <summary>
        /// касса 2 OUT
        /// </summary>
        public static string Kkm2Out = ""; //GetParam(18);

        /// <summary>
        /// касса 3 IN
        /// </summary>
        public static string Kkm3In = ""; //GetParam(19);
        public static string kkm3_dep = "";

        /// <summary>
        /// касса 3 OUT
        /// </summary>
        public static string Kkm3Out = ""; //GetParam(20);

        /// <summary>
        /// касса 4 IN
        /// </summary>
        public static string Kkm4In = ""; //GetParam(21);
        public static string kkm4_dep = "";

        /// <summary>
        /// касса 4 OUT
        /// </summary>
        public static string Kkm4Out = ""; //GetParam(22);

        /// <summary>
        /// касса 5 IN
        /// </summary>
        public static string Kkm5In = ""; //GetParam(21);
        public static string kkm5_dep = "";

        /// <summary>
        /// касса 5 OUT
        /// </summary>
        public static string Kkm5Out = ""; //GetParam(22);

        /// <summary>
        /// Emark 1 
        /// </summary>
        public static string emark_Kkm1 = ""; //GetParam(25);

        /// <summary>
        /// Emark 2
        /// </summary>
        public static string emark_Kkm2 = ""; //GetParam(26);

        /// <summary>
        /// Emark 3
        /// </summary>
        public static string emark_Kkm3 = ""; //GetParam(27);

        /// <summary>
        /// Emark 4
        /// </summary>
        public static string emark_Kkm4 = ""; 
        public static string OPOSLog = ""; 
        public static string InpasLog = ""; 
        public static string POSLog = ""; 
        public static string SmartLog = ""; 

                
        //private static string GetParam(int ConfId)
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command;
        //    string _str_command;

        //    try
        //    {
        //        // формируем комме
        //        _conn = new OleDbConnection(ConnString);
        //        _conn.Open();
        //        // создать новый документ
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;
        //        _str_command = "SELECT conf_value FROM t_Conf WHERE conf_id = @conf_id";
        //        _command.CommandText = _str_command;
        //        _command.Parameters.AddWithValue("@conf_id", ConfId.ToString());
        //        return _command.ExecuteScalar().ToString();
        //    }
        //    catch (Exception _exp)
        //    {
        //        throw _exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        public static string CurrGettedParamName="";
        public static string GetParam(List<t_Conf> parameters,string param_name)
        {
            CurrGettedParamName=param_name;
            t_Conf tconf=parameters.WhereFirst<t_Conf>(a => a.conf_param == param_name);
            if (tconf == null)
            {
                MDIParentMain.Log("Нет параметра " + param_name);
                return "";
            }
            return tconf.conf_value;
        }

        public static t_Conf GetTaram(List<t_Conf> parameters, string param_name)
        {
            CurrGettedParamName = param_name;
            t_Conf tconf = parameters.WhereFirst<t_Conf>(a => a.conf_param == param_name);
            if (tconf == null)
            {
                MDIParentMain.Log("Нет параметра " + param_name);
                return null;
            }
            return tconf;
        }

        public static t_Conf IdGetTaram(List<t_Conf> parameters, int param_id)
        {
            CurrGettedParamName = param_id.ToString();
            t_Conf tconf = parameters.WhereFirst<t_Conf>(a => a.conf_id == param_id);
            if (tconf == null)
            {
                MDIParentMain.Log("Нет параметра " + param_id);
                return null;
            }
            return tconf;
        }

        public static void Init()
        {
            try
            {
                List<t_Conf> p = new t_Conf().Select<t_Conf>();
                AppVer = GetParam(p,"ver");
                TeremokId = GetParam(p, "teremok_id");
                MainTeremokId = TeremokId;
                AppCity = Convert.ToInt32(GetParam(p, "dep"));

                FtpServer1 = GetParam(p, "ftp_server1");
                FtpPort1 = GetParam(p, "ftp_port1");
                FtpLogin1 = GetParam(p, "ftp_login1");
                FtpPass1 = GetParam(p, "ftp_pass1");

                FtpServer2 = GetParam(p, "ftp_server2");
                FtpPort2 = GetParam(p, "ftp_port2");
                FtpLogin2 = GetParam(p, "ftp_login2");
                FtpPass2 = GetParam(p, "ftp_pass2");

                FtpServer3 = GetParam(p, "ftp_server3");
                FtpPort3 = GetParam(p, "ftp_port3");
                FtpLogin3 = GetParam(p, "ftp_login3");
                FtpPass3 = GetParam(p, "ftp_pass3");
                
                FtpServer4 = GetParam(p, "ftp_server4");
                FtpPort4 = GetParam(p, "ftp_port4");
                FtpLogin4 =GetParam(p, "ftp_login4");
                FtpPass4 = GetParam(p, "ftp_pass4");

                TimerSending =  Convert.ToInt32(GetParam(p, "timer_exh"));
                TimerExchange =  Convert.ToInt32(GetParam(p, "timer_inbox"));
                TimerKKM =  Convert.ToInt32(GetParam(p, "timer_report"));
                LoadReportToDataBase =  Convert.ToInt32(GetParam(p, "load_report"));

                Kkm1In = GetParam(p, "folder_kkm1_in");
                kkm1_dep = GetTaram(p, "folder_kkm1_in").conf_dep;
                Kkm1Out = GetParam(p, "folder_kkm1_out");

                Kkm2In = GetParam(p, "folder_kkm2_in");
                kkm2_dep = GetTaram(p, "folder_kkm2_in").conf_dep;
                Kkm2Out = GetParam(p, "folder_kkm2_out");

                Kkm3In = GetParam(p, "folder_kkm3_in");
                kkm3_dep = GetTaram(p, "folder_kkm3_in").conf_dep;
                Kkm3Out = GetParam(p, "folder_kkm3_out");

                Kkm4In = GetParam(p, "folder_kkm4_in");
                kkm4_dep = GetTaram(p, "folder_kkm4_in").conf_dep;
                Kkm4Out = GetParam(p, "folder_kkm4_out");

                if (AppCity == 1)
                {
                    Kkm5In = IdGetTaram(p, 37).conf_value;   
                    kkm5_dep = IdGetTaram(p, 37).conf_dep;
                    Kkm5Out = IdGetTaram(p, 38).conf_value;
                }
                
                emark_Kkm1 = GetParam(p, "folder_emark_kkm1");
                emark_Kkm2 = GetParam(p, "folder_emark_kkm2");
                emark_Kkm3 = GetParam(p, "folder_emark_kkm3");
                emark_Kkm4 = GetParam(p, "folder_emark_kkm4");

                if (AppVer == "4.0.0.18")
                {
                    OPOSLog = GetTaram(p, "conf_OPOSLog").conf_value;
                    InpasLog = GetTaram(p, "conf_InpasLog").conf_value;
                    POSLog = GetTaram(p, "conf_POSLog").conf_value;
                    SmartLog = GetTaram(p, "conf_SmartLog").conf_value;
                }  
            }
            catch (Exception _exp)
            {
                MDIParentMain.Log(_exp, "Не удалось инициализировать параметры " + CurrGettedParamName + " err" + _exp.Message);
                MDIParentMain.Log("Не удалось инициализировать параметры" + CurrGettedParamName + " err" + _exp.Message);
                throw _exp;
            }
            finally
            {
                
            }
        }
    }
}

// Type: RBServer.CConfig
// Assembly: RBServer, Version=1.0.0.56, Culture=neutral, PublicKeyToken=null
// MVID: D6C94F23-CE38-43FB-B524-257857437EBF
// Assembly location: G:\RRepo\myproject\RBServer\RBServer.exe

using RBServer.Debug_classes;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace RBServer
{
  public class CConfig
  {
    private static string _filename = AppDomain.CurrentDomain.BaseDirectory + "\\RBConfig.xml";
    private string _email_rbs_err = "krigel.s@teremok.ru";
    private string _email_noRefNum = "krigel.s@teremok.ru";
    private string _update_process = "";
    private string _updation_folder = "Update";
    private string _rbs_err_svyaznoy_ftp = "krigel.s@teremok.ru";
    private string _rbs_err_svyaznoy_csv = "karasev.p@teremok.ru, krigel.s@teremok.ru";
    private string _rbs_alarm_restoran = "krigel.s@teremok.ru,upravrest.msk@teremok.ru";
    private string _rbs_err_zreport = "krigel.s@teremok.ru,chekomazova.e@teremok.ru,kolova.i@teremok.ru,soboleva.i@teremok.ru";
    private string _rbs_err_goods = "krigel.s@teremok.ru,operator.s@teremok.ru";
    private string _rbs_err_return = "krigel.s@teremok.ru,chekomazova.e@teremok.ru,kolova.i@teremok.ru,soboleva.i@teremok.ru,obraztsova.e@teremok.ru";
    private string _rbs_mess_inbox = "krigel.s@teremok.ru,ceh@teremok.ru,operatorprc@teremok.ru,krovosheeva.i@teremok.ru,makarova.o@teremok.ru";
    private string _rbs_mess_z = "krigel.s@teremok.ru";
    private string _ftp_svyaznoy_address = "ftp://ftp.sclub.ru/reestrs/";
    private string _ftp_svyaznoy_username = "teremok";
    private string _ftp_svyaznoy_password = "tPKZPisT";
    private string _rbs_intern_degugger_folder = "C:\\RBSERV_PRODUCTION";
    private string _rbs_intern_degugger_folder_size = "C:\\RBSERV_PRODUCTION";
    private int _sql_timeout = 300;
    private string _svyaznoy_upload_day = "1";
    private string _svyaznoy_upload_time = "20:30";
    private int _svyaznoy_upload_sending_time_min = 10;
    public int m_timer;
    public string m_folder_1c_in;
    public string m_folder_1c_out;
    public string m_folder_arch;
    public string m_folder_log;
    public string m_folder_log_kkm;
    public string m_folder_1c_order;
    public string m_folder_1c_order_out;
    public string m_folder_Z;
    public string m_folder_Z_M;
    public string m_folder_ftp;
    public string m_folder_ftp_M;
    public string m_folder_load;
    public string m_connstring;
    public string m_folder_terminal;
    public string m_order1_email;
    public string m_order2_email;
    public string m_inv_email;
    public string m_spis_email;
    public string m_dinner_email;
    public string m_error_email;
    public string m_smena_email;
    public string m_smtp_server;
    public string m_smtp_login;
    public string m_smtp_pass;
    public string m_send_from;
    public string m_dep;
    public string m_zakaz_time;
    public string m_invoice_time;
    public string m_invoice_time2;
    public string m_return_time;
    public string m_app_folder;
    public string m_ftp_server;
    public int m_ftp_port;
    public string m_ftp_login;
    public string m_ftp_pass;
    public string m_ftp_folder_name;
    public string m_ftp_server_sb;
    public int m_ftp_port_sb;
    public string m_ftp_login_sb;
    public string m_ftp_pass_sb;
    public string m_ftp_folder_name_sb;
    public string m_folderRSB;
    public string m_folderRSBarhiv;
    private bool _rbs_intern_degugger_state;
    private bool _rbs_paused;
    private XmlReader _xmlreader;
    private XDocument _doc;

    public string email_rbs_err
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_err");
        if (str != "")
          this._email_rbs_err = str;
        return this._email_rbs_err;
      }
    }

    public string email_noRefNum
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_no_rn");
        if (str != "")
          this._email_noRefNum = str;
        return this._email_noRefNum;
      }
    }

    public string update_process
    {
      get
      {
        string str = this.returnParamName("parm", "name", "update_process");
        if (str != "")
          this._update_process = str;
        return this._update_process;
      }
    }

    public string updation_folder
    {
      get
      {
        string str = this.returnParamName("parm", "name", "updation_folder");
        if (str != "")
          this._updation_folder = str;
        return this._updation_folder;
      }
    }


    private string _rbs_zfile_report = "krigel.s@teremok.ru";
    public string email_rbs_zfile_report
    {
        get
        {
            string str = this.returnParamName("parm", "name", "rbs_zfile_report");
            if (str != "")
                this._rbs_zfile_report = str;
            return this._rbs_zfile_report;
        }
    }

    private string _rbs_spb_zfile_report = "krigel.s@teremok.ru";
    public string email_rbs_spb_zfile_report
    {
        get
        {
            string str = this.returnParamName("parm", "name", "rbs_spb_zfile_report");
            if (str != "")
                this._rbs_spb_zfile_report = str;
            return this._rbs_spb_zfile_report;
        }
    }


    private int _hour_zfile_report_send_spb = 11;
    public int hour_zfile_report_send_spb
    {
        get
        {
            string str = this.returnParamName("parm", "name", "hour_zfile_report_send_spb");
            if (str != "")
                int.TryParse(str, out this._hour_zfile_report_send_spb);
            return this._hour_zfile_report_send_spb;
        }
    }

    private int _hour_zfile_report_send = 11;
    public int hour_zfile_report_send
    {
        get
        {
            string str = this.returnParamName("parm", "name", "hour_zfile_report_send");
            if (str != "")
                int.TryParse(str, out this._hour_zfile_report_send);
            return this._hour_zfile_report_send;
        }
    }

    private int _next_day_check_hour_zfile_report = 11;
    public int next_day_check_hour_zfile_report
    {
        get
        {
            string str = this.returnParamName("parm", "name", "next_day_check_hour_zfile_report");
            if (str != "")
                int.TryParse(str, out this._next_day_check_hour_zfile_report);
            return this._next_day_check_hour_zfile_report;
        }
    }

    private int _from_day_check_hour_zfile_report = 17;
    public int from_day_check_hour_zfile_report
    {
        get
        {
            string str = this.returnParamName("parm", "name", "from_day_check_hour_zfile_report");
            if (str != "")
                int.TryParse(str, out this._from_day_check_hour_zfile_report);
            return this._from_day_check_hour_zfile_report;
        }
    }

    private int _kkm_start_work_hour_zfile_report = 9;
    public int kkm_start_work_hour_zfile_report
    {
        get
        {
            string str = this.returnParamName("parm", "name", "kkm_start_work_hour_zfile_report");
            if (str != "")
                int.TryParse(str, out this._kkm_start_work_hour_zfile_report);
            return this._kkm_start_work_hour_zfile_report;
        }
    }


      

    public string email_rbs_err_svyaznoy_ftp
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_err_svyaznoy_ftp");
        if (str != "")
          this._rbs_err_svyaznoy_ftp = str;
        return this._rbs_err_svyaznoy_ftp;
      }
    }

    public string email_rbs_err_svyaznoy_csv
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_err_svyaznoy_csv");
        if (str != "")
          this._rbs_err_svyaznoy_csv = str;
        return this._rbs_err_svyaznoy_csv;
      }
    }

    public string email_rbs_alarm_restoran
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_alarm_restoran");
        if (str != "")
          this._rbs_alarm_restoran = str;
        return this._rbs_alarm_restoran;
      }
    }

    public string email_rbs_err_zreport
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_err_zreport");
        if (str != "")
          this._rbs_err_zreport = str;
        return this._rbs_err_zreport;
      }
    }

    public string email_rbs_err_goods
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_err_goods");
        if (str != "")
          this._rbs_err_goods = str;
        return this._rbs_err_goods;
      }
    }

    public string email_rbs_err_return
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_err_return");
        if (str != "")
          this._rbs_err_return = str;
        return this._rbs_err_return;
      }
    }

    public string email_rbs_mess_inbox
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_mess_inbox");
        if (str != "")
          this._rbs_mess_inbox = str;
        return this._rbs_mess_inbox;
      }
    }

    private bool _enable_email_send_flag = true;
    public bool enable_email_send_flag
    {
        get
        {
            string str = this.returnParamName("parm", "name", "enable_email_send_flag");
            if (str != "")
                bool.TryParse(str, out this._enable_email_send_flag);
            return this._enable_email_send_flag;
        }
    }

    public string email_rbs_mess_z
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_mess_z");
        if (str != "")
          this._rbs_mess_z = str;
        return this._rbs_mess_z;
      }
    }

    public string ftp_svyaznoy_address
    {
      get
      {
        string str = this.returnParamName("parm", "name", "ftp_svyaznoy_address");
        if (str != "")
          this._ftp_svyaznoy_address = str;
        return this._ftp_svyaznoy_address;
      }
    }

    public string ftp_svyaznoy_username
    {
      get
      {
        string str = this.returnParamName("parm", "name", "ftp_svyaznoy_username");
        if (str != "")
          this._ftp_svyaznoy_username = str;
        return this._ftp_svyaznoy_username;
      }
    }

    public string ftp_svyaznoy_password
    {
      get
      {
        string str = this.returnParamName("parm", "name", "ftp_svyaznoy_password");
        if (str != "")
          this._ftp_svyaznoy_password = str;
        return this._ftp_svyaznoy_password;
      }
    }

    public bool rbs_intern_degugger_state
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_intern_degugger_state");
        if (str != "")
          bool.TryParse(str, out this._rbs_intern_degugger_state);
        return this._rbs_intern_degugger_state;
      }
    }

    public string rbs_intern_degugger_folder
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_intern_degugger_folder");
        if (str != "")
          this._rbs_intern_degugger_folder = str;
        return this._rbs_intern_degugger_folder;
      }
    }

    public string rbs_intern_degugger_folder_size
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_intern_degugger_folder_size");
        if (str != "")
          this._rbs_intern_degugger_folder_size = str;
        return this._rbs_intern_degugger_folder_size;
      }
    }

    private string _remove_backup_files_count = "1000";
    public string remove_backup_files_count
    {
        get
        {
            string str = this.returnParamName("parm", "name", "remove_backup_files_count");
            if (str != "")
                this._remove_backup_files_count = str;
            return this._remove_backup_files_count;
        }
    }



    public bool rbs_paused
    {
      get
      {
        string str = this.returnParamName("parm", "name", "rbs_paused");
        if (str != "")
          bool.TryParse(str, out this._rbs_paused);
        return this._rbs_paused;
      }
    }

    public int sql_timeout
    {
      get
      {
        string s = this.returnParamName("parm", "name", "sql_timeout");
        if (s != "")
          int.TryParse(s, out this._sql_timeout);
        return this._sql_timeout;
      }
    }

    public string svyaznoy_upload_day
    {
      get
      {
        string str = this.returnParamName("parm", "name", "svyaznoy_upload_day");
        if (str != "")
          this._svyaznoy_upload_day = str;
        return this._svyaznoy_upload_day;
      }
    }

    public string svyaznoy_upload_time
    {
      get
      {
        string str = this.returnParamName("parm", "name", "svyaznoy_upload_time");
        if (str != "")
          this._svyaznoy_upload_time = str;
        return this._svyaznoy_upload_time;
      }
    }

    public int svyaznoy_upload_sending_time_min
    {
      get
      {
        string s = this.returnParamName("parm", "name", "svyaznoy_upload_sending_time_min");
        if (s != "")
          int.TryParse(s, out this._svyaznoy_upload_sending_time_min);
        if (this._svyaznoy_upload_sending_time_min == 0)
          this._svyaznoy_upload_sending_time_min = 10;
        return this._svyaznoy_upload_sending_time_min;
      }
    }

    static CConfig()
    {
    }

    public CConfig()
    {
      XmlReader xmlReader = (XmlReader) null;
      try
      {
        xmlReader = XmlReader.Create(CConfig._filename);
        this._doc = XDocument.Load(CConfig._filename);
        foreach (XElement xelement in this._doc.Root.Elements())
        {
          switch (xelement.Attribute((XName) "name").Value)
          {
            case "timer":
              this.m_timer = Convert.ToInt32(xelement.Value);
              continue;
            case "app_folder":
              this.m_app_folder = xelement.Value;
              continue;
            case "folder_1c_in":
              this.m_folder_1c_in = xelement.Value;
              continue;
            case "folder_arch":
              this.m_folder_arch = xelement.Value;
              continue;
            case "folder_1c_out":
              this.m_folder_1c_out = xelement.Value;
              continue;
            case "folder_1c_order":
              this.m_folder_1c_order = xelement.Value;
              continue;
            case "folder_1c_order_out":
              this.m_folder_1c_order_out = xelement.Value;
              continue;
            case "folder_log":
              this.m_folder_log = xelement.Value;
              continue;
            case "folder_log_kkm":
              this.m_folder_log_kkm = xelement.Value;
              continue;
            case "folder_Z":
              this.m_folder_Z = xelement.Value;
              continue;
            case "folder_Z_M":
              this.m_folder_Z_M = xelement.Value;
              continue;
            case "folder_ftp":
              this.m_folder_ftp = xelement.Value;
              continue;
            case "folder_ftp_M":
              this.m_folder_ftp_M = xelement.Value;
              continue;
            case "connstring":
              this.m_connstring = xelement.Value;
              continue;
            case "order1_email":
              this.m_order1_email = xelement.Value;
              continue;
            case "order2_email":
              this.m_order2_email = xelement.Value;
              continue;
            case "error_email":
              this.m_error_email = xelement.Value;
              continue;
            case "inv_email":
              this.m_inv_email = xelement.Value;
              continue;
            case "spis_email":
              this.m_spis_email = xelement.Value;
              continue;
            case "dinner_email":
              this.m_dinner_email = xelement.Value;
              continue;
            case "smena_email":
              this.m_smena_email = xelement.Value;
              continue;
            case "smtp_server":
              this.m_smtp_server = xelement.Value;
              continue;
            case "smtp_login":
              this.m_smtp_login = xelement.Value;
              continue;
            case "smtp_pass":
              this.m_smtp_pass = xelement.Value;
              continue;
            case "send_from":
              this.m_send_from = xelement.Value;
              continue;
            case "dep":
              this.m_dep = xelement.Value;
              continue;
            case "zakaz_time":
              this.m_zakaz_time = xelement.Value;
              continue;
            case "invoice_time":
              this.m_invoice_time = xelement.Value;
              continue;
            case "invoice_time2":
              this.m_invoice_time2 = xelement.Value;
              continue;
            case "return_time":
              this.m_return_time = xelement.Value;
              continue;
            case "ftp_server":
              this.m_ftp_server = xelement.Value;
              continue;
            case "ftp_port":
              this.m_ftp_port = Convert.ToInt32(xelement.Value);
              continue;
            case "ftp_login":
              this.m_ftp_login = xelement.Value;
              continue;
            case "ftp_pass":
              this.m_ftp_pass = xelement.Value;
              continue;
            case "ftp_folder_name":
              this.m_ftp_folder_name = xelement.Value;
              continue;
            case "ftp_server_sb":
              this.m_ftp_server_sb = xelement.Value;
              continue;
            case "ftp_port_sb":
              this.m_ftp_port_sb = Convert.ToInt32(xelement.Value);
              continue;
            case "ftp_login_sb":
              this.m_ftp_login_sb = xelement.Value;
              continue;
            case "ftp_pass_sb":
              this.m_ftp_pass_sb = xelement.Value;
              continue;
            case "ftp_folder_name_sb":
              this.m_ftp_folder_name_sb = xelement.Value;
              continue;
            case "folder_load":
              this.m_folder_load = xelement.Value;
              continue;
            case "folder_terminal":
              this.m_folder_terminal = xelement.Value;
              continue;
            case "folderRSB":
              this.m_folderRSB = xelement.Value;
              continue;
            case "folderRSBarhiv":
              this.m_folderRSBarhiv = xelement.Value;
              continue;
            default:
              continue;
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        if (xmlReader != null)
          xmlReader.Close();
      }
    }

    private string returnParamName(string elementName, string parameterName, string value)
    {
      try
      {
        this._doc = (XDocument) null;
        this._doc = XDocument.Load(CConfig._filename);
        if (this._doc == null)
          return "";
        else
          return Enumerable.First<XElement>(Enumerable.Where<XElement>(this._doc.Descendants((XName) elementName), (Func<XElement, bool>) (c => c.Attribute((XName) parameterName).Value == value))).Value;
      }
      catch (Exception ex)
      {
        DebugPanel.Log("Ошибка разбора файла конфигурации <" + elementName + " " + parameterName + "=" + value + "> error= " + ex.Message);
        Console.WriteLine("Ошибка разбора файла конфигурации <" + elementName + " " + parameterName + "=" + value + ">");
      }
      return "";
    }

    public string SetParam(string elementName, string parameterName, string value, string new_value)
    {
      try
      {
        this._doc = (XDocument) null;
        this._doc = XDocument.Load(CConfig._filename);
        if (this._doc == null)
          return "";
        Enumerable.First<XElement>(Enumerable.Where<XElement>(this._doc.Descendants((XName) elementName), (Func<XElement, bool>) (c => c.Attribute((XName) parameterName).Value == value))).Value = new_value;
        this._doc.Save(CConfig._filename);
      }
      catch (Exception ex)
      {
        DebugPanel.Log("Ошибка сохранения параметра конфигурации <" + elementName + " " + parameterName + "=" + value + "> new_value= " + new_value + " error= " + ex.Message);
        Console.WriteLine("Ошибка сохранения параметра конфигурации <" + elementName + " " + parameterName + "=" + value + "> new_value= " + new_value + " error= " + ex.Message);
      }
      return "";
    }
  }
}

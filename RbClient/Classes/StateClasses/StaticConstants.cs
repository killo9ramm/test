using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.ru.teremok.msk;
using System.Net;
using System.Threading;
using System.Data;
using RBClient.Classes.InternalClasses.Models;
using System.Net.Security;
using Config_classes;
using System.IO;
using RBClient.ArmServices;
using RBClient.Classes.CustomClasses;
using RBClient.Classes.WebServiceClasses;

namespace RBClient.Classes
{
    class StaticConstants
    {
        #region webservice exchange
        public static IWebServiceExchanger WebServiceExchanger = RBClient.Classes.WebServiceExchanger.Instance;
        #endregion

        #region Ftp Exchange
        public static string CurrentFtpLogin;
        public static string CurrentFtpPassword;
        #endregion

        #region Plan Smen Block

        public static Thread WebServiceExchangeThread = null;

        private static string _tabel_Opened_Date_str;
        public static string Tabel_Opened_Date_str
        {
            get
            {
                return _tabel_Opened_Date_str;
            }
        }

        public static DateTime Tabel_Opened_Date;

        public static DateTime? RbClient_Started_time = null;

        private static TimeSpan? _started_Service_Exchange_Interval = null;
        public static TimeSpan Started_Service_Exchange_Interval
        {
            get
            {
                if (_started_Service_Exchange_Interval == null)
                {
                    if (CParam.AppVer == "1")
                    {
                        _started_Service_Exchange_Interval = new TimeSpan(0, 0, 5);
                    }
                    else
                    {
                        //_started_Service_Exchange_Interval = new TimeSpan(0, 30, 0);
                        _started_Service_Exchange_Interval = new TimeSpan(0, 0, 5);
                    }
                }
                return (TimeSpan)_started_Service_Exchange_Interval;
            }
        }

        public static DateTime? Web_Service_Last_Exchange_Date = null;

        private static TimeSpan _web_Service_Exchange_Interval = new TimeSpan(4, 0, 0);
        public static TimeSpan Web_Service_Exchange_Interval
        {
            get
            {
                int interval_seconds = CellHelper.GetConfValue<int>("web_service_exchange_timeout", 600);
                return new TimeSpan(0, 0, (int)interval_seconds);   //интервал обмена 10 минут
            }
            set
            {
                _web_Service_Exchange_Interval = value;
            }
        }


        public static bool IsSyncProcess = false;
        public static string Web_Service_Url = "";
        public static bool IsDocumentTabelBlocked = false;
        public static bool IsTabelOpened = false;


        private static ARMWeb _WebService_msk = null;
        public static ARMWeb WebService_msk
        {
            get
            {
                if (_WebService_msk == null)
                {

                    _WebService_msk = new ARMWeb();
                    _WebService_msk.Url = "https://msk.teremok.ru:8094/operuchet82_ARM/ws/ARMWeb/?wsdl";

                    _WebService_msk.Credentials = new NetworkCredential("_iteremok_", "Ge6!))~nC5@0");

                    t_Conf conf = new t_Conf().SelectFirst<t_Conf>("conf_param='validation_by_cert'");
                    if (conf.conf_value == "0")
                    {
                        ServicePointManager.ServerCertificateValidationCallback = null;
                        ServicePointManager.ServerCertificateValidationCallback +=
                        new RemoteCertificateValidationCallback(MDIParentMain.CustomCertificateValidatior);
                    }

#if(DEB)

                    _WebService_msk.Proxy = new WebProxy("mskisa01.msk.teremok.biz", 8080);
                    var cr = new NetworkCredential("arm_services", "mg5Qukf7DG8RrvwZ", "msk");
                    _WebService_msk.Proxy.Credentials = cr;

#endif
                }
                return _WebService_msk;
            }
            set
            {
                _WebService_msk = value;
            }
        }


        private static ARMWeb _WebService = null;
        public static ARMWeb WebService
        {
            get
            {
                if (_WebService == null)
                {

                    t_Conf conf_url = new t_Conf().SelectFirst<t_Conf>("conf_param='web_service_url'");
                    if (conf_url.conf_value == "null") return null;

                    _WebService = new ARMWeb();

                    if (conf_url.conf_value != "")
                    {
                        _WebService.Url = conf_url.conf_value;
                    }
                    _WebService.Credentials = new NetworkCredential("iteremok", "Ge6!))~nC5@0");

                    t_Conf conf = new t_Conf().SelectFirst<t_Conf>("conf_param='validation_by_cert'");
                    if (conf.conf_value == "0")
                    {
                        ServicePointManager.ServerCertificateValidationCallback = null;
                        ServicePointManager.ServerCertificateValidationCallback +=
                        new RemoteCertificateValidationCallback(MDIParentMain.CustomCertificateValidatior);
                    }

//#if(DEB)

                    _WebService.Proxy = new WebProxy("mskisa01.msk.teremok.biz", 8080);
                    var cr = new NetworkCredential("arm_services", "mg5Qukf7DG8RrvwZ", "msk");
                    _WebService.Proxy.Credentials = cr;

//#endif
                }
                return _WebService;
            }
            set
            {
                _WebService = value;
            }
        }



        private static Service1 _WebService1 = null;
        public static Service1 WebServiceSystem
        {
            get
            {
                if (_WebService1 == null)
                {
                    string web_url = RBINNER_CONFIG.GetProperty<string>("service_webservice_url",
                        @"https://msk.teremok.ru:8095/armservice/Service1.asmx");

                    _WebService1 = new Service1();
                    _WebService1.Timeout = RBINNER_CONFIG.GetProperty<int>("service_webservice_request_timeout", 300000);
                    _WebService1.UserAgent = StaticConstants.Main_Teremok_1cName;
                    _WebService1.Url = web_url;


                    string login = RBINNER_CONFIG.GetProperty<string>("service_webservice_login",
                        "arm_services");

                    string passw = PasswordDecoder.decode_string(RBINNER_CONFIG.GetProperty<string>("service_webservice_password",
                        "먆먀ャႴ먎먄맿ュႧႪユႵ먋먏먐Ⴝ"));

                    if (login != "")
                    {
                        _WebService1.Credentials = new NetworkCredential(login, passw);
                    }

                    string validbs = RBINNER_CONFIG.GetProperty<string>("service_webservice_validation_by_sert",
                        "0");

                    if (validbs == "0")
                    {
                        ServicePointManager.ServerCertificateValidationCallback = null;
                        ServicePointManager.ServerCertificateValidationCallback +=
                        new RemoteCertificateValidationCallback(MDIParentMain.CustomCertificateValidatior);
                    }

#if(DEB)

                    _WebService1.Proxy = new WebProxy("mskisa01.msk.teremok.biz", 8080);
                    var cr = new NetworkCredential("arm_services", "mg5Qukf7DG8RrvwZ", "msk");
                    _WebService1.Proxy.Credentials = cr;

#endif
                }
                return _WebService1;
            }
            set
            {
                _WebService1 = value;
            }
        }
        #endregion


        #region Main Info Block
        private static ConfigClass _inner_config;
        public static ConfigClass RBINNER_CONFIG
        {
            get
            {
                if (_inner_config != null) return _inner_config;

                FileInfo config =
                    RbClientGlobalStaticMethods.ReturnConfig(StaticConstants.INNER_CONFIG, false);
                if (config != null)
                {
                    _inner_config = new ConfigClass(config.FullName);
                }
                return _inner_config;
            }

        }

        #region mainteremok

        private static string _main_teremok_id = "";
        public static string Main_Teremok_ID
        {
            get
            {
                return _main_teremok_id;
            }
            set
            {
                _main_teremok_id = value;
                _main_teremok_id_int = int.Parse(_main_teremok_id);

                t_Teremok terem = new t_Teremok().SelectFirst<t_Teremok>("teremok_id=" + _main_teremok_id);
                if (terem != null)
                {
                    _main_teremok_id_string = terem.teremok_name;
                    _main_teremok_1c_string = terem.teremok_1C;
                }
            }
        }

        private static string _main_teremok_1c_string = "";
        public static string Main_Teremok_1cName
        {
            get
            {
                return _main_teremok_1c_string;
            }
        }

        private static string _main_teremok_id_string = "";
        public static string Main_Teremok_Name
        {
            get
            {
                return _main_teremok_id_string;
            }
        }
        private static int _main_teremok_id_int = 0;
        public static int Main_Teremok_ID_int
        {
            get
            {
                return _main_teremok_id_int;
            }
        }


        #endregion


        private static string _teremok_id = "";
        public static string Teremok_ID
        {
            get
            {
                return _teremok_id;
            }
            set
            {
                _teremok_id = value;
                _teremok_id_int = int.Parse(_teremok_id);

                t_Teremok terem = new t_Teremok().SelectFirst<t_Teremok>("teremok_id=" + _teremok_id);
                if (terem != null)
                {
                    Teremok_Name = terem.teremok_name;
                    Teremok_1cName = terem.teremok_1C;
                }
            }
        }

        private static string _teremok_1c_string = "";
        public static string Teremok_1cName
        {
            get
            {
                return _teremok_1c_string;
            }
            set
            {
                _teremok_1c_string = value;
            }
        }

        private static string _teremok_id_string = "";
        public static string Teremok_Name
        {
            get
            {
                return _teremok_id_string;
            }
            set
            {
                _teremok_id_string = value;
            }
        }

        private static int _teremok_id_int = 0;
        public static int Teremok_ID_int
        {
            get
            {
                return _teremok_id_int;
            }
            set
            {
                _teremok_id_int = value;
                _teremok_id = value.ToString();
            }
        }

        private static int _current_teremok_id_int = 0;
        public static int Current_Teremok_ID_int
        {
            get
            {
                return _current_teremok_id_int;
            }
            set
            {
                _current_teremok_id_int = value;
            }
        }

        public static DataGridView FormDocGrid = null;
        private static MDIParentMain _MainWindow = null;
        public static MDIParentMain MainWindow
        {
            get
            {
                return _MainWindow;
            }
            set
            {
                _MainWindow = value;
            }
        }

        private static CBData _cbd = null;
        public static CBData CBData
        {
            get
            {
                if (_cbd == null) _cbd = new CBData();
                return _cbd;
            }
        }

        private static CUpdateHelper _upd = null;
        public static CUpdateHelper UpdateHelper
        {
            get
            {
                if (_upd == null)
                {
                    _upd = new CUpdateHelper();
                }
                return _upd;
            }
            set
            {
                _upd = value;
            }
        }

        /// <summary>
        /// обновить главный грид
        /// </summary>
        public static void MainGridUpdate()
        {
            if (null != MainWindow)
            {
                if (MainWindow.InvokeRequired)
                {
                    MainWindow.Invoke(new MethodInvoker(delegate { MainWindow.RefreshDoc(); }));
                }
                else
                {
                    MainWindow.RefreshDoc();
                }


                #region trash
                //foreach (Form _f in MainWindow.MdiChildren)
                //{
                //    if (_f.Name == "FormDoc")
                //    {
                //        FormDoc _fd = (FormDoc)_f;
                //        if (_fd.InvokeRequired)
                //        {
                //            _fd.Invoke(new MethodInvoker(delegate { _fd.IninData(); }));
                //        }
                //        else
                //        {
                //            _fd.IninData();
                //        } 

                //        break;
                //    }
                //}
                #endregion
            }
        }

        public static void UpdateMainInterface(Control control, Action<Control> act)
        {
            if (control != null)
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(new MethodInvoker(delegate { act(control); }));
                }
                else
                {
                    act(control);
                }
            }
        }

        public static bool CheckIfFormIsOpen(Type form_type)
        {
            Form[] _forms = MainWindow.MdiChildren;
            bool _f_opened = false;

            foreach (Form _f in _forms)
            {
                if (_f.GetType() == form_type)
                {
                    _f.Activate();
                    _f_opened = true;
                    break;
                }
            }
            return _f_opened;
        }

        public static void FormAllEnabled()
        {
            Form[] _forms = MainWindow.MdiChildren;
            bool _f_opened = false;

            foreach (Form _f in _forms)
            {
                _f.Enabled = true;
            }
        }

        public static void FormOnlyEnabled(Type form_type)
        {
            Form[] _forms = MainWindow.MdiChildren;
            bool _f_opened = false;

            foreach (Form _f in _forms)
            {
                if (_f.GetType() == form_type)
                {
                    continue;
                }
                _f.Enabled = false;
            }
        }



        /// <summary>
        /// Возвращаем значение из конфига базы
        /// </summary>
        /// <typeparam name="T">тип параметра</typeparam>
        /// <param name="parameter_name">имя параметра</param>
        /// <param name="default_value">значение по умолчанию</param>
        /// <returns></returns>
        public static T ReturnConfValue<T>(string parameter_name, T default_value)
        {
            T val = default_value;
            object o = CellHelper.GetConfValue(parameter_name);

            if (o != null)
            {
                try
                {
                    val = (T)Convert.ChangeType(o, typeof(T));
                    return val;
                }
                catch (Exception e)
                { return default_value; }
            }
            return default_value;
        }

        public static DataTable _FullNomenclatureList = null;
        public static bool GetNomesThreadWorking = false;
        public static DataTable FullNomenclatureList
        {
            get
            {
                if (!GetNomesThreadWorking)
                {
                    return _FullNomenclatureList;
                }
                else
                {
                    int i = 0;
                    while (GetNomesThreadWorking && i < 40)
                    {
                        Thread.Sleep(500);
                        i++;
                    }
                    return _FullNomenclatureList;
                }
            }
            set
            {
                _FullNomenclatureList = value;
            }
        }

        /// <summary>
        /// флаг, если тру то в данное время отправляются z-отчеты
        /// </summary>
        public static bool m_sending_z_report = false;

        #endregion

        #region global string constants
        public const string TREPORT_FOLDER = "TReport";
        public const int TREPORT_FOLDER_SIZE = 100000000;//100mb

        public const string POSDISPLAY_FOLDER = "posdisplay";
        public const string POSDISPLAY_CONFIG = "rbclientposconfig.xml";
        public const string CONFIGS_FOLDER = "Configs";
        public const string POS_CONFIG_KEY = "folder_tokkm_emark";
        public const string EMARK_FOLDER_NAME = "Emark";

        public const string INNER_CONFIG = "rbclientinnerconfig.xml";

        public const string ADVIMAGE_CONFIG = "rbclientadvimageconfig.xml";
        public const string ADV_IMAGE_CONFIG_KEY = "adv_image_emark_path";

        public const string ADVVIDEO_CONFIG = "rbclientadvvideoconfig.xml";
        public const string ADV_VIDEO_CONFIG_KEY = "adv_video_emark_path";

        public const string Z_BACK_FOLDER = "ztemp";
        public const string TEMP_FOLDER = "temp";
        public const int Z_BACK_FOLDER_SIZE = 50000000;

        public const string OUTBOX_FOLDER = "outbox";

        public const string LOCAL_KILLER_DIRECTORY = @"prkiller";
        public const string KILLER_PATH = @"prkiller\ProcessKillerLauncher.exe";
        public const string KILLER_CONFIG_NAME = @"Config.xml";
        public const string KILLER_INC_NAME = "ProcessKillerLauncher.exe.lnk";

        public const string KILLER_STOP_FLAG_NAME = "killer_flag";
        public const string KILLER_START_FLAG_NAME = "starter_flag";
        public const string KILLER_START_FILE_PATH = "starter_name";
        public const string KILLER_PROCESS_NAME = "process_name";
        public const string KILLER_TIMEOUT_MS_VALUE = "1000";
        public const string KILLER_TIMEOUT_MS_NAME = "timeout_ms";

        public static string KILLER_OLD_TIMEOUT_PERIOD = "1000";

        public const string AUTO_LOAD_PATH_XP = @"Documents and Settings\All Users\Главное меню\Программы\Автозагрузка";

        public const string DATA_FOLDER = "Data";
        public const int DEFAULT_DB_BACK_COUNT = 3;

        public const string INSTALL_PACKAGE_FOLDER = "InstallPackages";
        public const string INSTALL_PACKAGE_CONFIG = "config.xml";
        public const string TRASH_FOLDER = "Trash";

        public const string ADV_DOC_FOLDER = "adv_documents";
        public const string EDUC_VIDEO_FOLDER = "video";
        public const string ADV_VIDEO_FOLDER = "ADV_VIDEO";
        public const string INNER_DOCS_FOLDER = "help";

        #endregion



        public static WebTaskManager WebTaskManager
        {
            get
            {
                return _MainWindow.WebTaskSheduler;
            }
        }

        private static WebService1cManager _WebService1cManager;
        public static WebService1cManager WebService1cManager
        {
            get
            {
                if (_WebService1cManager == null)
                {
                    _WebService1cManager = new WebService1cManager();
                }
                return _WebService1cManager;
            }
        }
    }
}

#define RBCDEBUG
using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using Ftp;
using Ionic.Zip;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using DevComponents.WinForms;
using DevComponents.DotNetBar;
using iTuner;
using RBClient.ru.teremok.msk;
using System.Net;
using Common.Logging;
using RBClient.ModalWindows;
using RBClient.Classes;
using RBClient.Classes.WindowsProgress;
using System.Text.RegularExpressions;
using killolib;
using Config_classes;
using RBClient.Classes.CustomClasses;
using RBClient.Classes.WindowMarochOtch;
using RBClient.Classes.InternalClasses.Models;
using RBClient.WPF.UserControls;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using RBClient.Classes.CustomWindows;
using ZreportWork;
using System.Runtime.Remoting.Messaging;
using ftpUpLoader;
using System.Runtime.InteropServices;
using RBClient.WinForms;
using RBClient.Classes.DocumentClasses;
using RBClient.Classes.WindowAddElement;
using System.Reflection;
using Classes;


//using RBClient.ru.teremok.spb;



namespace RBClient
{

    public partial class MDIParentMain : Form
    {
        public static ILog log = LogManager.GetCurrentClassLogger();

        private DevComponents.DotNetBar.RibbonControl ribbonControl1;
        private DevComponents.DotNetBar.RibbonTabItem ribbonTabItem1;
        private DevComponents.DotNetBar.RibbonPanel ribbonPanel1;
        private DevComponents.DotNetBar.RibbonBar ribbonBar4;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_Order1;
        private DevComponents.DotNetBar.RibbonPanel ribbonPanel2;
        private DevComponents.DotNetBar.RibbonBar ribbonBar2;
        private DevComponents.DotNetBar.RibbonTabItem ribbonTabItem2;
        private DevComponents.DotNetBar.ButtonItem buttonItem7;
        private DevComponents.DotNetBar.ButtonItem buttonItem6;
        private DevComponents.DotNetBar.ButtonItem buttonItem30;
        private DevComponents.DotNetBar.RibbonPanel ribbonPanel3;
        private DevComponents.DotNetBar.RibbonBar ribbonBar1;
        private DevComponents.DotNetBar.RibbonTabItem ribbonTabItem3;
        private DevComponents.DotNetBar.SuperTooltip superTooltip1;
        private DevComponents.DotNetBar.Bar bar1;
        private DevComponents.DotNetBar.ButtonItem toolStripStatusLabel_Transf;
        private DevComponents.DotNetBar.ButtonItem toolStripStatusLabel_KKM1;
        private DevComponents.DotNetBar.ButtonItem toolStripStatusLabel_KKM2;
        private DevComponents.DotNetBar.ButtonItem toolStripStatusLabel_KKM3;
        private DevComponents.DotNetBar.ButtonItem toolStripStatusLabel_KKM4;
        public DevComponents.DotNetBar.ProgressBarItem progressBarItem1;
        public DevComponents.DotNetBar.LabelItem Label1;
        private IContainer components;

        public int m_teremok_id = 0;
        public string m_title = "";
        private bool m_error = false; // ���� ������ ������, ����� ��������� ��������� TRUE        

        private string m_Info = "����� �� ����������"; // ���������� ��� ������������
        private bool m_thread_working = false; // ���� ����� ������ �� FTP
        private bool m_thread_work = false;

        public bool m_working_zreport = false; // ���� ������� ��������� Z-report
        private bool m_need_refresh_docjournal = false; // ����� ���������� ������
        private string m_kkm = null;

        private bool m_need_update_menu = false; // ����� �������� ����
        private bool m_update_ARM = false;
        private int m_ip_chanel = 0; // 1 - ��������, 2 - ��������� ����, 3 - ��������� ����
        private bool m_just_started = true; // ��������� ������ ��� ��������                
        private bool m_logfile_flag = false; // ���� ��������� ��� ����� (������������ ��� ������� �������)
        private bool m_open_log = false; // ���� �������� ����������� �� �������� (������������ ��� ������� �������)
        private bool m_cash_log = false; // ���� �������� 
        private bool m_cash_send_log = false; // ���� �������� 
        public string c_hash;
        string log_file = "kkm";

        // ����� ���
        public bool m_kkm1_online = false;
        public bool m_kkm2_online = false;
        public bool m_kkm3_online = false;
        public bool m_kkm4_online = false;
        public bool m_kkm5_online = false;

        private Timer timer_Exchange;
        private Timer timer_CheckInbox;
        private Timer timer_ZReport;
        private Timer timer_ExchIndicator;
        private Timer timer_TReport;
        public BackgroundWorker backgroundWorker_Inbox;
        private BackgroundWorker backgroundWorker_ZReport;
        private BackgroundWorker backgroundWorker_TReport;
        private BackgroundWorker backgroundWorker_CheckKKM;
        private BackgroundWorker backgroundWorker_Info;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_Invent;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_Transfer;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_Trans;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_Util;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_Spis2;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_Encashment;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_Help;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_Inv2;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_InvInv;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_water;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_electricity;
        private DevComponents.DotNetBar.RibbonPanel ribbonPanel4;
        private DevComponents.DotNetBar.RibbonBar ribbonBar3;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_water1;
        private DevComponents.DotNetBar.RibbonTabItem ribbonTabItem4;
        private DevComponents.DotNetBar.ButtonItem buttonItem1;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_electricity1;
        private DevComponents.Editors.DateTimeAdv.MonthCalendarAdv monthCalendarAdv1;
        public DevComponents.DotNetBar.LabelX labelinfo;
        private Timer time_Info;
        private DevComponents.DotNetBar.ButtonItem buttonExpanded;
        private DevComponents.DotNetBar.ButtonItem buttonItemzakazapb;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_OrderWeek;
        private DevComponents.DotNetBar.RibbonPanel ribbonPanel5;
        private DevComponents.DotNetBar.RibbonBar ribbonBar5;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_SalesReport;
        private DevComponents.DotNetBar.RibbonTabItem ribbonTabItem5;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_Invent_morning;
        private DevComponents.DotNetBar.ButtonItem toolStripButton_water2;
        private Timer timer_WalkKkm;
        private DevComponents.DotNetBar.ButtonItem toolStripStatusLabel_KKM5;
        private ButtonItem toolStripButton_Order2;
        private ButtonItem toolStripButton_Order29;
        public ButtonItem toolStripButton_Order30;
        private ButtonItem buttonItemknowledge;
        private ButtonItem buttonItemDetector;
        private BackgroundWorker backgroundWorker_WalkKkm;
        private Office2007StartButton office2007StartButton1;
        private ItemContainer itemContainer1;
        private ItemContainer itemContainer2;
        private ItemContainer itemContainer3;
        private ButtonItem buttonItem8;
        private ButtonItem buttonItem9;
        private ButtonItem buttonItem10;
        private ButtonItem buttonItem11;
        private ButtonItem buttonItem12;
        private ButtonItem buttonItem13;
        private GalleryContainer galleryContainer1;
        private LabelItem labelItem8;
        private ButtonItem buttonItem14;
        private ButtonItem buttonItem15;
        private ButtonItem buttonItem16;
        private ButtonItem buttonItem17;
        private ItemContainer itemContainer4;
        private ButtonItem buttonItem18;
        private ButtonItem buttonItem19;
        private StyleManager styleManager1;
        private RibbonPanel ribbonPanel6;
        private RibbonBar ribbonBar6;
        public ButtonItem button_Mark;
        public ButtonItem button_MarkNM;
        private RibbonTabItem tab;
        private ButtonX buttonX1;

        private UsbManager manager;

        ProgressWorker pw;

        public MDIParentMain()
        {
            try
            {
#if(DEB)
            //removeOldWebServiceTask(10);
            //ztemp_temp_processing();



            //CParam.Init();
            //Former_Static_Variables_Init();
            //startWebServiceExchange(null);
            //TSerializationHipDelete();
            //WindowStarter.�������_�����_�_�����_����(DateTime.Now);
            //return;



            //WebTaskSheduler = new WebTaskManager() { LogEvent = Log, Name = "WebTaskSheduler" };
            //WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "SetTeremokVersion",
            //                   new object[] { StaticConstants.Main_Teremok_1cName, "123" },
            //                   99);


            ////WebTaskSheduler.StartService();
            //return;

            //removeOldData();
            //System.Threading.Thread.Sleep(1200000);
           // removeOldWebServiceTask(7);
            
            //ParseDoc();
            //backgroundWorker_ZReport_DoWork(null,null);
            //startWebServiceExchange();

          //  t_WebServiceTask tw = new t_WebServiceTask().Select<t_WebServiceTask>().First();

            //string path1 = @"C:\Out\otchet";
            //var byt = Serializer.BinarySerialize(new object[] { "testres","999","0",path1,
            //    DateTime.Now,new DirectoryInfo(path1).GetFiles()});

            //t_WebServiceTask task = new t_WebServiceTask() {taskdate=DateTime.Now, taskparams = byt};
            //task.CreateOle();

            //t_test test = new t_test() { testole=new byte[3]{0,1,3} };
            //test.CreateOle();


            //t_WebServiceTask twt = new t_WebServiceTask().Select<t_WebServiceTask>("id=25").First();
            //twt.failed = true;
            //twt.UpdateOle();


          //  string path = @"C:\Out\otchet";
//            StaticConstants.WebServiceSystem.RecieveLogFilesList(

         //   UpdateClass.kkm_LogOutFiles("testres", 999, "1", path);

          //  WebTaskManager webtm = new WebTaskManager() { LogEvent = Log };
         //   webtm.StartService();

         //   return;

            //webtm.EnqeueTask("WebServiceSystem", 0, "RecieveLogFilesList", new object[] { "testres",999,"0",path,
            //    DateTime.Now,new DirectoryInfo(path).GetFiles().
            //Select(a=>new ArmServices.Fileinfo{ filename = a.Name, length = a.Length}).ToArray()}, 1);
            //return;
            //TestManager testman = new TestManager() {LogEvent=Log, Timeout=1000};

            //testman.EnqeueTask(1);
            //System.Threading.Thread.Sleep(100);
            //testman.EnqeueTask(2);
            //System.Threading.Thread.Sleep(100);
            //testman.EnqeueTask(3);
            //System.Threading.Thread.Sleep(100);
            //testman.EnqeueTask(4);
            //System.Threading.Thread.Sleep(100);

            //System.Threading.Thread.Sleep(2000);

            //testman.EnqeueTask(5);
            //System.Threading.Thread.Sleep(100);
            //testman.EnqeueTask(6);
            //System.Threading.Thread.Sleep(100);
            //testman.EnqeueTask(7);

            //System.Threading.Thread.Sleep(6000);
            //testman.EnqeueTask(8);
            //System.Threading.Thread.Sleep(5000);
            //System.Threading.Thread.Sleep(2000);

            //try
            //{
                
            //        UpdateClass.kkm_LogOutFiles(StaticConstants.Main_Teremok_1cName+"1", 999, "5", @"C:\out\otchet");
                
            //}catch
            //{
            //    Log("tIMEOUT");
            //}
            //return;
            //DirectoryInfo z_back_dir = RbClientGlobalStaticMethods.GetDirectory(StaticConstants.Z_BACK_FOLDER);

            //List<t_ZReport> z_list_sended = new t_ZReport().Select<t_ZReport>();
            //IEnumerable<string> z_list_sended_str = z_list_sended.Select(a => a.z_file);

            //ZreportMaker zmaker = new ZreportMaker();
            //zmaker.LogEvent = MDIParentMain.Log;

            //Func<DirectoryInfo, IEnumerable<FileInfo>> get_missing_x_files = (wdir) =>
            //{
            //    IEnumerable<FileInfo> _back_z_files = wdir.GetFiles("X*");
            //    IEnumerable<FileInfo> _back_z_files_missing = _back_z_files.Where(a => !z_list_sended_str.Contains(a.Name));
            //    return _back_z_files_missing;
            //};
            
            //Operate_Out_Z_Folder(z_back_dir, get_missing_x_files, zmaker,@"C:\Out\Otchet");

            
            //t_Doc doc = new t_Doc().SelectFirst<t_Doc>("doc_id=37");
            //WindowsController.��������_�_���������_������_��_������_�����(doc);


            //FormUpdate up = new FormUpdate();
            //up.ShowDialog();
        //    return;
            //string file_name="uptime.log";
            //FtpHelperClass.CheckIfFileExist("a1.teremok.ru", "333", ref file_name, "ftpteremokspb", "NthtvjrCG,2", "/KASHI/in", StringComparison.OrdinalIgnoreCase);

           //UpdateClass.kkm_LogOutFiles("testres", 999, "5", @"H:\FTP\kassa\2\C\Out\Otchet");

           //UpdateClass.kkm_LogOutFiles("testres", 999, "5", @"H:\FTP\kassa\2\C\Out\Otchet");

           // CParam.Init();
           // StaticConstants.MainWindow = this;
           // StaticConstants.MainWindow.m_kkm1_online = true;
           //// backgroundWorker_TReport_DoWork(this, null);
            //backgroundWorker_ZReport_DoWork(this, null);
           // backgroundWorker_Inbox_DoWork(this, null);

          //FileInfo zf=new FileInfo(@"G:\RRepo\myproject\RBClient\ztemp\temp\atriu_X14052791.423.zip");
          //StaticConstants.WebServiceSystem.RecieveZReport("test",900,DateTime.Now,zf.Name,200,File.ReadAllBytes(zf.FullName));



          //  BackgroundWorker bkw = new BackgroundWorker();
          //  BackgroundWorker bg = (BackgroundWorker)bkw;

          //  RunWorkerCompletedEventHandler rwe =(s, e) =>
          //          {
          //              int k = 3;
          //              k=k+1;
          //          };
          //  bg.RunWorkerCompleted += rwe;
          //  bg.RunWorkerCompleted += (s, e) =>
          //  {

          //  }; 
          //  object key1=new object();

          //  object key = bg.GetType().GetField("runWorkerCompletedKey", BindingFlags.Static | BindingFlags.NonPublic);
          //  object key2 = typeof(BackgroundWorker).GetField("runWorkerCompletedKey", BindingFlags.Static| BindingFlags.NonPublic);

          //  object key3 = bkw.GetType().GetField("doWorkKey", BindingFlags.Static | BindingFlags.NonPublic);
          //  object key4 = bkw.GetType().GetField("progressChangedKey", BindingFlags.Static | BindingFlags.NonPublic);
          //  object key5 = bkw.GetType().GetField("runWorkerCompletedKey", BindingFlags.Static | BindingFlags.NonPublic);

          //  PropertyInfo oo = bg.GetType().BaseType.GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
            

          //  var comp = (Component)bg;
          //  EventHandlerList o1 = (EventHandlerList)oo.GetValue(comp, null);
          ////  o1.AddHandler(key1,rwe);
          //  bg.RunWorkerAsync();
          //  MethodInfo find = o1.GetType().GetMethod("Find", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
          //  object o2 = find.Invoke(o1, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { key }, null);
          //  object o4 = find.Invoke(o1,new object[] { key2 });
          //  object o34 = find.Invoke(o1, new object[] { key3 });
          //  object o44 = find.Invoke(o1, new object[] { key4 });
          //  object o54 = find.Invoke(o1, new object[] { key5 });
            
            

            //FtpHelperClass.SendFileOnServer("devpc", "332", new FileInfo(@"H:\FTP\disk_c\ARM\FTP\grval\in\grval_X141016241.146.zip"),
            //    "ftp_user_1", "123456aA", @"/gval1/in");
            //CParam.Init();
            //string st = StaticConstants.WebServiceSystem.SendKKmZInfo(StaticConstants.Teremok_1cName,
            //                   "999", DateTime.Now, true, DateTime.Now);
            //startWebServiceExchange(0);
#endif

                log.Trace("��������� ���");
                log.Trace("��������� 1�� �������");
                CheckOnly1Instance();
                log.Trace("���������");
#if(STARTTEST)
                log.Trace("�������������� � ��������� progresswin");
                pw = new ProgressWorker("AppStart", "", global::RBClient.Properties.Resources.splash);
                pw.Start();
                pw.ReportProgress(10);
                log.Trace("���������");


#endif
                log.Trace("�������� ������������� ����");
                InitializeComponent();
                log.Trace("���������");

#if(STARTTEST)
                pw.ReportProgress(20);

#endif
                log.Trace("������������� ����������");
                CParam.Init();
                log.Trace("���������");

                //�������� ����������
                log.Trace("�������� ��������� ����������");
                CheckRbClientUpdate();
                log.Trace("���������");

                if (CParam.AppCity != 1)//���� ������ �� �����
                {
                    toolStripButton_Order29.Visible = false; //����� ����������
                }

                #region ����������� ���� ������
#if(!DEB)

                if (StaticConstants.UpdateHelper.IsVersion("5.6.5.5") && CParam.AppCity == 1)
                {
                    Log("���� ����� ������ "
                            + StaticConstants.RBINNER_CONFIG.GetProperty<int>("doc_delete_month_period", 3) + " ������� - ���� �� ��������");
                    AsyncTaskAction1 asta = new AsyncTaskAction1((o) =>
                    {
                        while (ReturnOldZDocs(StaticConstants.RBINNER_CONFIG.GetProperty<int>("doc_delete_month_period", 3)).NotNullOrEmpty())
                        {
                            Log("���� �������� ������ ������ "
                                + StaticConstants.RBINNER_CONFIG.GetProperty<int>("doc_delete_month_period", 3) + " ������� - 30 ���");
                            System.Threading.Thread.Sleep(30000);
                        }

                        log.Trace("�������� ����������� ����");
                        log.Trace("������� ������ ������");
                        DeleteOldBackups();
                        log.Trace("�������� ����������� ����");
                        OptimizeDataBase();
                        log.Trace("���������");
                    }, null).Start();
                }
                else
                {
                    log.Trace("�������� ����������� ����");
                    log.Trace("������� ������ ������");
                    DeleteOldBackups();
                    log.Trace("�������� ����������� ����");
                    OptimizeDataBase();
                    log.Trace("���������");
                }
#endif

                #endregion


#if(STARTTEST)
                pw.ReportProgress(35);
#endif
            }
            catch (Exception ex)
            {
                Log(ex, "MdiParentMainError");
                WpfCustomMessageBox.Show("��������� ��������� ������ � MdiParentMainError!!! ��������� � ����������� ����������!!!");
            }
        }

        public void CheckRbClientUpdate()
        {
            DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder + "\\inbox\\");
            foreach (FileInfo _file in _dir.GetFiles("*.exe"))
            {
                if (_file.Name.ToLower() == "rbclient.exe")
                {
                    try
                    {
                        RbClientUpdate();
                    }catch(Exception ex)
                    {
                        Log("cannot update arm " + ex.Message);
                    }
                }
            }
        }

        public void RbClientUpdate()
        {
            Process process = new Process();
            process.StartInfo.FileName = CParam.AppFolder + "\\RBClientUpdate.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            this.Close();
        }

        public void DeleteOldBackups()
        {
            log.Trace("�������� ������� ������ ������");
            int default_dback_count=0;
            DirectoryInfo local_db_dir = 
                RbClientGlobalStaticMethods.GetDirectory(StaticConstants.DATA_FOLDER);

            List<FileInfo> files_backs = new List<FileInfo>();
            List<FileInfo> files = local_db_dir.GetFiles("RBA.accdb_*_backup").ToList();
            files.Sort((a,b)=>a.CreationTime.CompareTo(b.CreationTime));

            if (CellHelper.GetConfValue("db_backups_count") != null)
            {
                if (!int.TryParse(CellHelper.GetConfValue("db_backups_count").ToString(), out default_dback_count))
                    default_dback_count = StaticConstants.DEFAULT_DB_BACK_COUNT;
            }
            else
            {
                default_dback_count = StaticConstants.DEFAULT_DB_BACK_COUNT;
            }

            if (files.Count <= default_dback_count + 2)
            {
                log.Trace("������ �� ������� - ������� ���� ������");
                return;
            }

            log.Trace("�������� �������� �������");
            files_backs.Add(files.First());
            files_backs.Add(files.Last());

            default_dback_count -= 2;
            int promej = files.Count / (default_dback_count + 1);

            for (int i = 1; i <= default_dback_count; i++)
            {
                files_backs.Add(files[i + i * promej - 1]);
            }

            files.ForEach(a =>
            {
                if (!files_backs.Contains(a))
                {
                    a.Delete();
                    log.Trace("������� ���� "+a.FullName);
                }

            });
        }

        public void OptimizeDataBase()
        {
            if (!StaticConstants.UpdateHelper.IsNewerVersion("5.5.7.9"))
            {
                try
                {
                    log.Trace("��������� ������ ���� ������");
                    t_Conf tc = new t_Conf().SelectFirst<t_Conf>("conf_param='max_database_size_mb'");
                    long db_size_int = long.Parse(tc.conf_value)*1000;
                    AccessDbFunctions acdf = new AccessDbFunctions() {LogEvent=Log};


                    FileInfo fi = acdf.returnCurrentDataBase(CParam.ConnString);
                    
                   
                   long curr_db_len=(fi.Length/1024);
                   if (curr_db_len > db_size_int)
                   {

                       log.Trace("������ ���� ������ ������ " + db_size_int.ToString() + "�� ������ �����������");
                       //���� ������ ���������� ������� ���������� �����������
                       log.Trace("������ ������� ���� ������");
                       new CustomAction(o =>
                       {
                           CompactDataBase(fi, acdf);
                       }, null) { Timeout=2000 }.Start();

                       log.Trace("������� �������");
                       log.Trace("�������� ����� ������ ����");
                       //��������� ����� ���� ��� ����� ������ 
                       fi = acdf.returnCurrentDataBase(CParam.ConnString);
                       curr_db_len = (fi.Length / 1024);
                       if (curr_db_len > db_size_int)
                       {
                           log.Trace("������ ���� ����� ������ " + db_size_int.ToString()+"��");
                           log.Trace("������ ����� ���� ����� ��������� ������");
                           string backupName = fi.FullName + "_" + DateTime.Now.ToShortDateString() + "_backup";
                           if (acdf.BackUpDataBase(fi.FullName, backupName, false))
                           {
                               log.Trace("������� ������");
                               removeOldData();
                           }
                       }

                   }
                    
                }
                catch (Exception ex)
                {
                    Log(ex, "�� ������� ������� ����������� ���� ������");
                }
            }
        }

        private void removeOldWebServiceTask(int days)
        {
            DateTime dt = DateTime.Now.AddDays(-days);
            removeOldDocs<t_WebServiceTask>("succed=-1 AND taskdate<" + SqlWorker.ReturnDate(dt));
        }


        private void removeOldDocs<T>(string query)
            where T : ModelClass, new()
        {
                List<T> docs = new T().Select<T>(query);
                docs.ForEach(a =>
                {
                    try
                    {
                        a.Delete();
                    }
                    catch (Exception ex)
                    {
                        Log(String.Format("removeOldDocs type {0} id {1} error {2}", 
                            typeof(T).Name.ToString(),
                            StaticHelperClass.ReturnClassItemValue(a,0),
                            ex.Message));
                    }
                });
                Log(String.Format("��������� ������� ����� type {0} count {1} ",typeof(T).Name.ToString(),docs.Count));
        }

        private void removeOldDocs(int month,int count)
        {            
           DateTime dt = DateTime.Now.AddMonths(-month);
            
           List<t_Doc> docs = new t_Doc().Select<t_Doc>(String.Format("doc_datetime<" + SqlWorker.ReturnDate(dt))).Take(count).ToList();

           Log("������ ������� ����� "+docs.Count);
           t_Doc aa = null;
           try
           {
               docs.ForEach(a =>
               {
                   
                   aa = a;
                   if (a.doc_type_id != 5)
                   {
                       a.Join<t_TaskExchange>("task_doc_id=" + a.doc_id);
                       a.Join<t_Order2ProdDetails>("opd_doc_id=" + a.doc_id);
                       a.Join<t_Utilization>("util_doc_id=" + a.doc_id);
                       a.Join<t_MarkItems>("mark_doc_id=" + a.doc_id);
                       a.Join<t_Marotch>("doc_id=" + a.doc_id);
                       a.Join<t_Invent>("inv_doc_id=" + a.doc_id);
                       a.Join<t_Dinner>("doc_id=" + a.doc_id);
                       a.Join<t_MenuPrice>("mp_doc_id=" + a.doc_id);
                       if (a.JoinTables.Keys.Contains(typeof(t_Dinner)))
                       {
                           a.JoinTables[typeof(t_Dinner)].ToList().ForEach(b =>
                           {
                               b.Join<t_Dinner2t_Menu>("Dinner_id=" + ((t_Dinner)b).id);
                           });
                       }
                   }
                   else//if (a.doc_type_id == 5)
                   {
                       a.Join<t_TaskExchange>("task_doc_id=" + a.doc_id);
                       a.Join<t_Order2ProdDetails>("opd_doc_id=" + a.doc_id);
                       a.Join<t_ZReport>("z_doc_id=" + a.doc_id);
                       if (a.JoinTables.Keys.Contains(typeof(t_ZReport)))
                       {
                           
                           a.JoinTables[typeof(t_ZReport)].ToList().ForEach(b =>
                           {
                               b.Join<t_Check>("check_z_id=" + ((t_ZReport)b).z_id);
                               if (b.JoinTables.Keys.Contains(typeof(t_Check)))
                               {
                                   b.JoinTables[typeof(t_Check)].ToList().ForEach(c =>
                                   {
                                       c.Join<t_CheckItem>("ch_check_id=" + ((t_Check)c).check_id);
                                   });
                               }
                               delete_zReport_from_disc(b);
                           });
                       }
                   }

               });

               //docs.ForEach(a => a.RecursiveDelete());
               //docs.ForEach(a => a.RecursiveDelete());
               docs.ForEach(a => { a.DeleteJoinTablesMass(); a.Delete(); });

               Log("��������� ������� ����� " + docs.Count);
           }catch(Exception ex)
           {
           }
    }

        private void delete_zReport_from_disc(ModelClass z_report)
        {
            if (z_report is t_ZReport)
            {
                DirectoryInfo z_back_dir = RbClientGlobalStaticMethods.GetDirectory(StaticConstants.Z_BACK_FOLDER);
                DirectoryInfo z_temp_dir = z_back_dir.GetDecendantDirectory(StaticConstants.TEMP_FOLDER);
                
                //�������� �������
                //new CustomAction(o =>
                //{
                //    foreach(FileInfo z_file in z_temp_dir.GetFiles("*" + ((t_ZReport)z_report).z_file))
                //    {
                //        z_file.Delete();
                //    }
                //}, null).Start();

                ZreportMaker zmaker = new ZreportMaker();
                zmaker.LogEvent = Log;

                new CustomAction(o =>
                {
                    foreach (FileInfo z_back_file in z_back_dir.GetFiles("*" + ((t_ZReport)z_report).z_file)
                        .Where(a=>a.Name.IndexOf("D")==-1))
                    {
                        List<FileInfo> files_to_z = zmaker.findAnotherFilesOnX(z_back_file);
                        foreach (FileInfo _f in zmaker.findAnotherFilesOnX(z_back_file))
                        {
                            _f.Delete();
                            Log("������� ���� " + _f.FullName);
                        }
                    }
                }, null).Start();
            }
        }

        private void removeOldData()
        {
            AsyncTaskAction2 act = new AsyncTaskAction2(
            () =>
            {
                Log("�������� ������");
                TDocsDelete();
                TWebServiceTaskDelete();
                TSerializationHipDelete();
                Log("�������� ���������");

            }) { LogEvent = Log };
            act.Start();
        }

        private void TSerializationHipDelete()
        {
            int val = StaticConstants.RBINNER_CONFIG.GetProperty("doc_delete_serializhip_day_period", 15);
            DateTime dt = DateTime.Now.AddDays(-val);
            string query=String.Format("isvalid=0 AND  date_added<{0}",SqlWorker.ReturnDate(dt));
            int portion = StaticConstants.RBINNER_CONFIG.GetProperty("doc_delete_portion_quantity", 150);

            StaticConstants.CBData.DeleteRecordsFromDB<t_SerializationHip>(query, portion);
        }

        private void TWebServiceTaskDelete()
        {
            int days = StaticConstants.RBINNER_CONFIG.GetProperty("doc_delete_ttask_day_period", 15);
            removeOldWebServiceTask(days);
        }

        private void TDocsDelete()
        {
            int month = StaticConstants.RBINNER_CONFIG.GetProperty("doc_delete_month_period", 3);

            DateTime dt = DateTime.Now.AddMonths(-month);
            List<t_Doc> docs = new t_Doc().Select<t_Doc>(String.Format("doc_datetime<" + SqlWorker.ReturnDate(dt))).ToList();

            while (docs.Count > 0)
            {
                int docs_count = StaticConstants.RBINNER_CONFIG.GetProperty("doc_delete_portion_quantity", 150);
                removeOldDocs(month, docs_count);
                docs = new t_Doc().Select<t_Doc>(String.Format("doc_datetime<" + SqlWorker.ReturnDate(dt))).ToList();
            }
        }

        private void CompactDataBase(FileInfo fi,AccessDbFunctions acdf)
        {
            
            string backupName = fi.FullName + "_backup_last";

            if (acdf.BackUpDataBase(fi.FullName, backupName, false, true))
            {
                log.Trace("������� ����� "+fi.FullName);
                string temp_ext = "temp_base_ext";
                FileInfo fi_new = new FileInfo(fi.FullName + temp_ext);
                if (acdf.CompactDataBase(fi.FullName, fi_new.FullName))
                {
                    if (fi_new.Exists) fi.Delete();
                    else throw new Exception("��� �������� ���� " + fi_new.FullName);
                    fi_new.MoveTo(fi.FullName);
                }
                else
                {
                    throw new Exception("�� ���������� ������� �������");
                }
            }
            else
            {
                log.Trace("�� ���������� ������� ����� " + fi.FullName);
            }
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MDIParentMain));
            this.ribbonControl1 = new DevComponents.DotNetBar.RibbonControl();
            this.ribbonPanel1 = new DevComponents.DotNetBar.RibbonPanel();
            this.labelinfo = new DevComponents.DotNetBar.LabelX();
            this.monthCalendarAdv1 = new DevComponents.Editors.DateTimeAdv.MonthCalendarAdv();
            this.ribbonBar4 = new DevComponents.DotNetBar.RibbonBar();
            this.toolStripButton_Order1 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Order29 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItemzakazapb = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Invent = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Invent_morning = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Transfer = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Trans = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Util = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Spis2 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Encashment = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Order30 = new DevComponents.DotNetBar.ButtonItem();
            this.ribbonPanel3 = new DevComponents.DotNetBar.RibbonPanel();
            this.ribbonBar1 = new DevComponents.DotNetBar.RibbonBar();
            this.buttonItemDetector = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItemknowledge = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem1 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Help = new DevComponents.DotNetBar.ButtonItem();
            this.ribbonPanel6 = new DevComponents.DotNetBar.RibbonPanel();
            this.ribbonBar6 = new DevComponents.DotNetBar.RibbonBar();
            this.button_Mark = new DevComponents.DotNetBar.ButtonItem();
            this.button_MarkNM = new DevComponents.DotNetBar.ButtonItem();
            this.ribbonPanel5 = new DevComponents.DotNetBar.RibbonPanel();
            this.ribbonBar5 = new DevComponents.DotNetBar.RibbonBar();
            this.toolStripButton_SalesReport = new DevComponents.DotNetBar.ButtonItem();
            this.ribbonPanel4 = new DevComponents.DotNetBar.RibbonPanel();
            this.ribbonBar3 = new DevComponents.DotNetBar.RibbonBar();
            this.toolStripButton_water1 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_water2 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_electricity1 = new DevComponents.DotNetBar.ButtonItem();
            this.ribbonPanel2 = new DevComponents.DotNetBar.RibbonPanel();
            this.ribbonBar2 = new DevComponents.DotNetBar.RibbonBar();
            this.toolStripButton_Order2 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_OrderWeek = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_Inv2 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_InvInv = new DevComponents.DotNetBar.ButtonItem();
            this.ribbonTabItem1 = new DevComponents.DotNetBar.RibbonTabItem();
            this.ribbonTabItem2 = new DevComponents.DotNetBar.RibbonTabItem();
            this.ribbonTabItem4 = new DevComponents.DotNetBar.RibbonTabItem();
            this.ribbonTabItem5 = new DevComponents.DotNetBar.RibbonTabItem();
            this.tab = new DevComponents.DotNetBar.RibbonTabItem();
            this.buttonItem6 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem30 = new DevComponents.DotNetBar.ButtonItem();
            this.ribbonTabItem3 = new DevComponents.DotNetBar.RibbonTabItem();
            this.buttonExpanded = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem7 = new DevComponents.DotNetBar.ButtonItem();
            this.superTooltip1 = new DevComponents.DotNetBar.SuperTooltip();
            this.bar1 = new DevComponents.DotNetBar.Bar();
            this.toolStripStatusLabel_Transf = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripStatusLabel_KKM1 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripStatusLabel_KKM2 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripStatusLabel_KKM3 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripStatusLabel_KKM4 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripStatusLabel_KKM5 = new DevComponents.DotNetBar.ButtonItem();
            this.progressBarItem1 = new DevComponents.DotNetBar.ProgressBarItem();
            this.Label1 = new DevComponents.DotNetBar.LabelItem();
            this.timer_Exchange = new System.Windows.Forms.Timer(this.components);
            this.timer_CheckInbox = new System.Windows.Forms.Timer(this.components);
            this.timer_ZReport = new System.Windows.Forms.Timer(this.components);
            this.timer_ExchIndicator = new System.Windows.Forms.Timer(this.components);
            this.timer_TReport = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker_Inbox = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker_ZReport = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker_TReport = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker_CheckKKM = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker_Info = new BackgroundWorker();
            this.time_Info = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker_WalkKkm = new System.ComponentModel.BackgroundWorker();
            this.timer_WalkKkm = new System.Windows.Forms.Timer(this.components);
            this.office2007StartButton1 = new DevComponents.DotNetBar.Office2007StartButton();
            this.itemContainer1 = new DevComponents.DotNetBar.ItemContainer();
            this.itemContainer2 = new DevComponents.DotNetBar.ItemContainer();
            this.itemContainer3 = new DevComponents.DotNetBar.ItemContainer();
            this.buttonItem8 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem9 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem10 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem11 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem12 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem13 = new DevComponents.DotNetBar.ButtonItem();
            this.galleryContainer1 = new DevComponents.DotNetBar.GalleryContainer();
            this.labelItem8 = new DevComponents.DotNetBar.LabelItem();
            this.buttonItem14 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem15 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem16 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem17 = new DevComponents.DotNetBar.ButtonItem();
            this.itemContainer4 = new DevComponents.DotNetBar.ItemContainer();
            this.buttonItem18 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem19 = new DevComponents.DotNetBar.ButtonItem();
            this.styleManager1 = new DevComponents.DotNetBar.StyleManager(this.components);
            this.toolStripButton_water = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripButton_electricity = new DevComponents.DotNetBar.ButtonItem();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            this.ribbonControl1.SuspendLayout();
            this.ribbonPanel1.SuspendLayout();
            this.ribbonPanel3.SuspendLayout();
            this.ribbonPanel6.SuspendLayout();
            this.ribbonPanel5.SuspendLayout();
            this.ribbonPanel4.SuspendLayout();
            this.ribbonPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            // 
            // 
            // 
            this.ribbonControl1.BackgroundStyle.Class = "";
            this.ribbonControl1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonControl1.CanCustomize = false;
            this.ribbonControl1.Controls.Add(this.ribbonPanel1);
            this.ribbonControl1.Controls.Add(this.ribbonPanel3);
            this.ribbonControl1.Controls.Add(this.ribbonPanel6);
            this.ribbonControl1.Controls.Add(this.ribbonPanel5);
            this.ribbonControl1.Controls.Add(this.ribbonPanel4);
            this.ribbonControl1.Controls.Add(this.ribbonPanel2);
            this.ribbonControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ribbonControl1.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.ribbonTabItem1,
            this.ribbonTabItem2,
            this.ribbonTabItem4,
            this.ribbonTabItem5,
            this.tab,
            this.buttonItem6,
            this.buttonItem30,
            this.ribbonTabItem3,
            this.buttonExpanded});
            this.ribbonControl1.KeyTipsFont = new System.Drawing.Font("Tahoma", 7F);
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.ribbonControl1.QuickToolbarItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItem7});
            this.ribbonControl1.Size = new System.Drawing.Size(1105, 155);
            this.ribbonControl1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonControl1.SystemText.MaximizeRibbonText = "&Maximize the Ribbon";
            this.ribbonControl1.SystemText.MinimizeRibbonText = "Mi&nimize the Ribbon";
            this.ribbonControl1.SystemText.QatAddItemText = "&Add to Quick Access Toolbar";
            this.ribbonControl1.SystemText.QatCustomizeMenuLabel = "<b>Customize Quick Access Toolbar</b>";
            this.ribbonControl1.SystemText.QatCustomizeText = "&Customize Quick Access Toolbar...";
            this.ribbonControl1.SystemText.QatDialogAddButton = "&Add >>";
            this.ribbonControl1.SystemText.QatDialogCancelButton = "Cancel";
            this.ribbonControl1.SystemText.QatDialogCaption = "Customize Quick Access Toolbar";
            this.ribbonControl1.SystemText.QatDialogCategoriesLabel = "&Choose commands from:";
            this.ribbonControl1.SystemText.QatDialogOkButton = "OK";
            this.ribbonControl1.SystemText.QatDialogPlacementCheckbox = "&Place Quick Access Toolbar below the Ribbon";
            this.ribbonControl1.SystemText.QatDialogRemoveButton = "&Remove";
            this.ribbonControl1.SystemText.QatPlaceAboveRibbonText = "&Place Quick Access Toolbar above the Ribbon";
            this.ribbonControl1.SystemText.QatPlaceBelowRibbonText = "&Place Quick Access Toolbar below the Ribbon";
            this.ribbonControl1.SystemText.QatRemoveItemText = "&Remove from Quick Access Toolbar";
            this.ribbonControl1.TabGroupHeight = 14;
            this.ribbonControl1.TabIndex = 0;
            this.ribbonControl1.Text = "ribbonControl1";
            this.ribbonControl1.Click += new System.EventHandler(this.ribbonControl1_Click);
            // 
            // ribbonPanel1
            // 
            this.ribbonPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ribbonPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            //this.ribbonPanel1.Controls.Add(this.labelinfo);
            this.Controls.Add(this.labelinfo);

            this.ribbonPanel1.Controls.Add(this.monthCalendarAdv1);
            this.ribbonPanel1.Controls.Add(this.ribbonBar4);
            this.ribbonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ribbonPanel1.Location = new System.Drawing.Point(0, 26);
            this.ribbonPanel1.Name = "ribbonPanel1";
            this.ribbonPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.ribbonPanel1.Size = new System.Drawing.Size(1105, 127);
            // 
            // 
            // 
            this.ribbonPanel1.Style.Class = "";
            this.ribbonPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel1.StyleMouseDown.Class = "";
            this.ribbonPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel1.StyleMouseOver.Class = "";
            this.ribbonPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonPanel1.TabIndex = 1;
            this.ribbonPanel1.Visible = true;
            // 
            // labelinfo
            // 
            //this.labelinfo.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelinfo.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Dot;
            this.labelinfo.BackgroundStyle.BorderBottomColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarCaptionInactiveText;
            this.labelinfo.BackgroundStyle.BorderBottomWidth = 1;
            this.labelinfo.BackgroundStyle.BorderColor = System.Drawing.SystemColors.ControlText;
            this.labelinfo.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Dot;
            this.labelinfo.BackgroundStyle.BorderLeftWidth = 1;
            this.labelinfo.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Dot;
            this.labelinfo.BackgroundStyle.BorderRightWidth = 1;
            this.labelinfo.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Dot;
            this.labelinfo.BackgroundStyle.BorderTopWidth = 1;
            this.labelinfo.BackgroundStyle.Class = "";
            this.labelinfo.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelinfo.ForeColor = System.Drawing.Color.Red;
            this.labelinfo.Image = ((System.Drawing.Image)(resources.GetObject("labelinfo.Image")));
            this.labelinfo.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.labelinfo.Location = new System.Drawing.Point(724, 6);
            this.labelinfo.Name = "labelinfo";
            this.labelinfo.Size = new System.Drawing.Size(198, 114);
            this.labelinfo.TabIndex = 3;
            this.labelinfo.Text = "�������!";
            this.labelinfo.TextAlignment = System.Drawing.StringAlignment.Center;

            this.labelinfo.Visible = false;
            this.labelinfo.WordWrap = true;
            
            
            // 
            // monthCalendarAdv1
            // 
            this.monthCalendarAdv1.AnnuallyMarkedDates = new System.DateTime[0];
            this.monthCalendarAdv1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.monthCalendarAdv1.BackgroundStyle.BackColor = System.Drawing.Color.Transparent;
            this.monthCalendarAdv1.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Double;
            this.monthCalendarAdv1.BackgroundStyle.BorderBottomWidth = 1;
            this.monthCalendarAdv1.BackgroundStyle.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.monthCalendarAdv1.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Double;
            this.monthCalendarAdv1.BackgroundStyle.BorderLeftWidth = 1;
            this.monthCalendarAdv1.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Double;
            this.monthCalendarAdv1.BackgroundStyle.BorderRightWidth = 1;
            this.monthCalendarAdv1.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Double;
            this.monthCalendarAdv1.BackgroundStyle.BorderTopWidth = 1;
            this.monthCalendarAdv1.BackgroundStyle.Class = "";
            this.monthCalendarAdv1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.monthCalendarAdv1.Colors.Selection.IsBold = true;
            this.monthCalendarAdv1.Colors.Weekend.TextColor = System.Drawing.Color.Red;
            // 
            // 
            // 
            this.monthCalendarAdv1.CommandsBackgroundStyle.BorderColor = System.Drawing.Color.Transparent;
            this.monthCalendarAdv1.CommandsBackgroundStyle.Class = "";
            this.monthCalendarAdv1.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.monthCalendarAdv1.ContainerControlProcessDialogKey = true;
            this.monthCalendarAdv1.DaySize = new System.Drawing.Size(24, 13);
            this.monthCalendarAdv1.DisplayMonth = new System.DateTime(2012, 10, 26, 0, 0, 0, 0);
            this.monthCalendarAdv1.FirstDayOfWeek = System.DayOfWeek.Monday;
            this.monthCalendarAdv1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
            this.monthCalendarAdv1.Location = new System.Drawing.Point(926, 6);
            this.monthCalendarAdv1.MarkedDates = new System.DateTime[0];
            this.monthCalendarAdv1.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            this.monthCalendarAdv1.MinDate = new System.DateTime(1999, 1, 1, 0, 0, 0, 0);
            this.monthCalendarAdv1.MonthlyMarkedDates = new System.DateTime[0];
            this.monthCalendarAdv1.Name = "monthCalendarAdv1";
            // 
            // 
            // 
            this.monthCalendarAdv1.NavigationBackgroundStyle.Class = "";
            this.monthCalendarAdv1.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.monthCalendarAdv1.Size = new System.Drawing.Size(170, 114);
            this.monthCalendarAdv1.TabIndex = 0;
            this.monthCalendarAdv1.TwoLetterDayName = false;
            this.monthCalendarAdv1.WeekendDaysSelectable = false;
            this.monthCalendarAdv1.WeeklyMarkedDays = new System.DayOfWeek[0];
            // 
            // ribbonBar4
            // 
            this.ribbonBar4.AutoOverflowEnabled = true;
            // 
            // 
            // 
            this.ribbonBar4.BackgroundMouseOverStyle.Class = "";
            this.ribbonBar4.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar4.BackgroundStyle.Class = "";
            this.ribbonBar4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonBar4.ContainerControlProcessDialogKey = true;
            this.ribbonBar4.Dock = System.Windows.Forms.DockStyle.Left;
            this.ribbonBar4.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.toolStripButton_Order1,
            this.toolStripButton_Order29,
            this.buttonItemzakazapb,
            this.toolStripButton_Invent,
            this.toolStripButton_Invent_morning,
            this.toolStripButton_Transfer,
            this.toolStripButton_Trans,
            this.toolStripButton_Util,
            this.toolStripButton_Spis2,
            this.toolStripButton_Encashment,
            this.toolStripButton_Order30});
            this.ribbonBar4.Location = new System.Drawing.Point(0, 0);
            this.ribbonBar4.Name = "ribbonBar4";
            this.ribbonBar4.Size = new System.Drawing.Size(818, 126);
            this.ribbonBar4.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonBar4.TabIndex = 2;
            this.ribbonBar4.Text = "���������";
            // 
            // 
            // 
            this.ribbonBar4.TitleStyle.Class = "";
            this.ribbonBar4.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar4.TitleStyleMouseOver.Class = "";
            this.ribbonBar4.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // toolStripButton_Order1
            // 
            this.toolStripButton_Order1.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Order1.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Order1.HotFontBold = true;
            this.toolStripButton_Order1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Order1.Image")));
            this.toolStripButton_Order1.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Order1.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Order1.Name = "toolStripButton_Order1";
            this.toolStripButton_Order1.SubItemsExpandWidth = 14;
            this.toolStripButton_Order1.Text = "�����";
            this.toolStripButton_Order1.Click += new System.EventHandler(this.toolStripButton_Order1_Click);
            // 
            // toolStripButton_Order29
            // 
            this.toolStripButton_Order29.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Order29.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Order29.HotFontBold = true;
            this.toolStripButton_Order29.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Order29.Image")));
            this.toolStripButton_Order29.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Order29.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Order29.Name = "toolStripButton_Order29";
            this.toolStripButton_Order29.SubItemsExpandWidth = 14;
            this.toolStripButton_Order29.Text = "�����.����������";
            this.toolStripButton_Order29.Click += new System.EventHandler(this.toolStripButton_Order29_Click);
            // 
            // buttonItemzakazapb
            // 
            this.buttonItemzakazapb.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.buttonItemzakazapb.FixedSize = new System.Drawing.Size(100, 80);
            this.buttonItemzakazapb.HotFontBold = true;
            this.buttonItemzakazapb.Image = ((System.Drawing.Image)(resources.GetObject("buttonItemzakazapb.Image")));
            this.buttonItemzakazapb.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.buttonItemzakazapb.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.buttonItemzakazapb.Name = "buttonItemzakazapb";
            this.buttonItemzakazapb.SubItemsExpandWidth = 14;
            this.buttonItemzakazapb.Text = "����� ���. ��������";
            this.buttonItemzakazapb.Visible = false;
            this.buttonItemzakazapb.Click += new System.EventHandler(this.toolStripButton_Order2_Click);
            // 
            // toolStripButton_Invent
            // 
            this.toolStripButton_Invent.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Invent.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Invent.HotFontBold = true;
            this.toolStripButton_Invent.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Invent.Image")));
            this.toolStripButton_Invent.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Invent.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Invent.Name = "toolStripButton_Invent";
            this.toolStripButton_Invent.SubItemsExpandWidth = 14;
            this.toolStripButton_Invent.Text = "�������";
            this.toolStripButton_Invent.Click += new System.EventHandler(this.toolStripButton_Invent_Click);
            // 
            // toolStripButton_Invent_morning
            // 
            this.toolStripButton_Invent_morning.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Invent_morning.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Invent_morning.HotFontBold = true;
            this.toolStripButton_Invent_morning.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Invent_morning.Image")));
            this.toolStripButton_Invent_morning.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Invent_morning.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Invent_morning.Name = "toolStripButton_Invent_morning";
            this.toolStripButton_Invent_morning.SubItemsExpandWidth = 14;
            this.toolStripButton_Invent_morning.Text = "������� ����";
            this.toolStripButton_Invent_morning.Visible = false;
            this.toolStripButton_Invent_morning.Click += new System.EventHandler(this.toolStripButton_Invent_morning_Click);
            // 
            // toolStripButton_Transfer
            // 
            this.toolStripButton_Transfer.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Transfer.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Transfer.HotFontBold = true;
            this.toolStripButton_Transfer.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Transfer.Image")));
            this.toolStripButton_Transfer.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Transfer.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Transfer.Name = "toolStripButton_Transfer";
            this.toolStripButton_Transfer.SubItemsExpandWidth = 14;
            this.toolStripButton_Transfer.Text = "�������";
            this.toolStripButton_Transfer.Click += new System.EventHandler(this.toolStripButton_Transfer_Click);
            // 
            // toolStripButton_Trans
            // 
            this.toolStripButton_Trans.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Trans.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Trans.HotFontBold = true;
            this.toolStripButton_Trans.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Trans.Image")));
            this.toolStripButton_Trans.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Trans.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Trans.Name = "toolStripButton_Trans";
            this.toolStripButton_Trans.SubItemsExpandWidth = 14;
            this.toolStripButton_Trans.Text = "�����������";
            this.toolStripButton_Trans.Click += new System.EventHandler(this.toolStripButton_Trans_Click);
            // 
            // toolStripButton_Util
            // 
            this.toolStripButton_Util.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Util.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Util.HotFontBold = true;
            this.toolStripButton_Util.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Util.Image")));
            this.toolStripButton_Util.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Util.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Util.Name = "toolStripButton_Util";
            this.toolStripButton_Util.SubItemsExpandWidth = 14;
            this.toolStripButton_Util.Text = "�������� ��";
            this.toolStripButton_Util.Click += new System.EventHandler(this.toolStripButton_Util_Click);
            // 
            // toolStripButton_Spis2
            // 
            this.toolStripButton_Spis2.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Spis2.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Spis2.HotFontBold = true;
            this.toolStripButton_Spis2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Spis2.Image")));
            this.toolStripButton_Spis2.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Spis2.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Spis2.Name = "toolStripButton_Spis2";
            this.toolStripButton_Spis2.SubItemsExpandWidth = 14;
            this.toolStripButton_Spis2.Text = "�������� ��";
            this.toolStripButton_Spis2.Click += new System.EventHandler(this.toolStripButton_Spis2_Click);
            // 
            // toolStripButton_Encashment
            // 
            this.toolStripButton_Encashment.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Encashment.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Encashment.HotFontBold = true;
            this.toolStripButton_Encashment.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Encashment.Image")));
            this.toolStripButton_Encashment.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Encashment.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Encashment.Name = "toolStripButton_Encashment";
            this.toolStripButton_Encashment.SubItemsExpandWidth = 14;
            this.toolStripButton_Encashment.Text = "����������";
            this.toolStripButton_Encashment.Click += new System.EventHandler(this.toolStripButton_Encashment_Click);
            // 
            // toolStripButton_Order30
            // 
            this.toolStripButton_Order30.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Order30.FixedSize = new System.Drawing.Size(120, 80);
            this.toolStripButton_Order30.HotFontBold = true;
            this.toolStripButton_Order30.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Order30.Image")));
            this.toolStripButton_Order30.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Order30.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Order30.Name = "toolStripButton_Order30";
            this.toolStripButton_Order30.SubItemsExpandWidth = 14;
            this.toolStripButton_Order30.Text = "����� � �����";
            this.toolStripButton_Order30.Click += new System.EventHandler(this.toolStripButton_Order30_Click);
            // 
            // ribbonPanel3
            // 
            this.ribbonPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonPanel3.Controls.Add(this.ribbonBar1);
            this.ribbonPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ribbonPanel3.Location = new System.Drawing.Point(0, 26);
            this.ribbonPanel3.Name = "ribbonPanel3";
            this.ribbonPanel3.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ribbonPanel3.Size = new System.Drawing.Size(1105, 127);
            // 
            // 
            // 
            this.ribbonPanel3.Style.Class = "";
            this.ribbonPanel3.Style.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel3.StyleMouseDown.Class = "";
            this.ribbonPanel3.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel3.StyleMouseOver.Class = "";
            this.ribbonPanel3.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonPanel3.TabIndex = 3;
            this.ribbonPanel3.Visible = false;
            // 
            // ribbonBar1
            // 
            this.ribbonBar1.AutoOverflowEnabled = true;
            // 
            // 
            // 
            this.ribbonBar1.BackgroundMouseOverStyle.Class = "";
            this.ribbonBar1.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar1.BackgroundStyle.Class = "";
            this.ribbonBar1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonBar1.ContainerControlProcessDialogKey = true;
            this.ribbonBar1.Dock = System.Windows.Forms.DockStyle.Left;
            this.ribbonBar1.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItemDetector,
            this.buttonItemknowledge,
            this.buttonItem1,
            this.toolStripButton_Help});
            this.ribbonBar1.Location = new System.Drawing.Point(3, 0);
            this.ribbonBar1.Name = "ribbonBar1";
            this.ribbonBar1.Size = new System.Drawing.Size(583, 124);
            this.ribbonBar1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonBar1.TabIndex = 0;
            this.ribbonBar1.Text = "���������";
            // 
            // 
            // 
            this.ribbonBar1.TitleStyle.Class = "";
            this.ribbonBar1.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar1.TitleStyleMouseOver.Class = "";
            this.ribbonBar1.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // buttonItemDetector
            // 
            this.buttonItemDetector.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.buttonItemDetector.FixedSize = new System.Drawing.Size(100, 80);
            this.buttonItemDetector.HotFontBold = true;
            this.buttonItemDetector.Image = ((System.Drawing.Image)(resources.GetObject("buttonItemDetector.Image")));
            this.buttonItemDetector.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.buttonItemDetector.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.buttonItemDetector.Name = "buttonItemDetector";
            this.buttonItemDetector.SubItemsExpandWidth = 14;
            this.buttonItemDetector.Text = "��������� ��������";
            this.buttonItemDetector.Click += new System.EventHandler(this.buttonItemDetector_Click);
            // 
            // buttonItemknowledge
            // 
            this.buttonItemknowledge.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.buttonItemknowledge.FixedSize = new System.Drawing.Size(100, 80);
            this.buttonItemknowledge.HotFontBold = true;
            this.buttonItemknowledge.Image = ((System.Drawing.Image)(resources.GetObject("buttonItemknowledge.Image")));
            this.buttonItemknowledge.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.buttonItemknowledge.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.buttonItemknowledge.Name = "buttonItemknowledge";
            this.buttonItemknowledge.SubItemsExpandWidth = 14;
            this.buttonItemknowledge.Text = "�����";
            this.buttonItemknowledge.Click += new System.EventHandler(this.buttonItemknowledge_Click);
            // 
            // buttonItem1
            // 
            this.buttonItem1.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.buttonItem1.FixedSize = new System.Drawing.Size(100, 80);
            this.buttonItem1.HotFontBold = true;
            this.buttonItem1.Image = ((System.Drawing.Image)(resources.GetObject("buttonItem1.Image")));
            this.buttonItem1.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.buttonItem1.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.buttonItem1.Name = "buttonItem1";
            this.buttonItem1.SubItemsExpandWidth = 14;
            this.buttonItem1.Text = "������\r\n����������";
            this.buttonItem1.Click += new System.EventHandler(this.toolStripButton_Reestr_Click);
            // 
            // toolStripButton_Help
            // 
            this.toolStripButton_Help.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Help.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Help.HotFontBold = true;
            this.toolStripButton_Help.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Help.Image")));
            this.toolStripButton_Help.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Help.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Help.Name = "toolStripButton_Help";
            this.toolStripButton_Help.SubItemsExpandWidth = 14;
            this.toolStripButton_Help.Text = "��� �������� � ���";
            this.toolStripButton_Help.Click += new System.EventHandler(this.toolStripButton_Help_Click);
            // 
            // ribbonPanel6
            // 
            this.ribbonPanel6.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonPanel6.Controls.Add(this.ribbonBar6);
            this.ribbonPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ribbonPanel6.Location = new System.Drawing.Point(0, 26);
            this.ribbonPanel6.Name = "ribbonPanel6";
            this.ribbonPanel6.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ribbonPanel6.Size = new System.Drawing.Size(1105, 127);
            // 
            // 
            // 
            this.ribbonPanel6.Style.Class = "";
            this.ribbonPanel6.Style.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel6.StyleMouseDown.Class = "";
            this.ribbonPanel6.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel6.StyleMouseOver.Class = "";
            this.ribbonPanel6.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonPanel6.TabIndex = 6;
            this.ribbonPanel6.Visible = false;
            // 
            // ribbonBar6
            // 
            this.ribbonBar6.AutoOverflowEnabled = true;
            // 
            // 
            // 
            this.ribbonBar6.BackgroundMouseOverStyle.Class = "";
            this.ribbonBar6.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar6.BackgroundStyle.Class = "";
            this.ribbonBar6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonBar6.ContainerControlProcessDialogKey = true;
            this.ribbonBar6.Dock = System.Windows.Forms.DockStyle.Left;
            this.ribbonBar6.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.button_Mark,
            this.button_MarkNM});
            this.ribbonBar6.Location = new System.Drawing.Point(3, 0);
            this.ribbonBar6.Name = "ribbonBar6";
            this.ribbonBar6.Size = new System.Drawing.Size(111, 124);
            this.ribbonBar6.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonBar6.TabIndex = 2;
            this.ribbonBar6.Text = "���������";
            // 
            // 
            // 
            this.ribbonBar6.TitleStyle.Class = "";
            this.ribbonBar6.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar6.TitleStyleMouseOver.Class = "";
            this.ribbonBar6.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // button_Mark
            // 
            this.button_Mark.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.button_Mark.FixedSize = new System.Drawing.Size(140, 80);
            this.button_Mark.HotFontBold = true;
            this.button_Mark.Image = ((System.Drawing.Image)(resources.GetObject("button_Mark.Image")));
            this.button_Mark.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.button_Mark.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.button_Mark.Name = "button_Mark";
            this.button_Mark.PopupFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, 
                System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Mark.SubItemsExpandWidth = 14;
            this.button_Mark.Text = "������ �������� ������";
            this.button_Mark.Click += new System.EventHandler(this.button_Mark_Click);
            // 
            // button_MarkNM
            // 
            this.button_MarkNM.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.button_MarkNM.FixedSize = new System.Drawing.Size(140, 80);
            this.button_MarkNM.HotFontBold = true;
            this.button_MarkNM.Image = ((System.Drawing.Image)(resources.GetObject("button_MarkNM.Image")));
            this.button_MarkNM.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.button_MarkNM.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.button_MarkNM.Name = "button_MarkNM";
            this.button_MarkNM.PopupFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_MarkNM.SubItemsExpandWidth = 14;
            this.button_MarkNM.Text = "������ ���������� ������";
            this.button_MarkNM.Click += new System.EventHandler(this.button_Mark_ClickNM);
            // 
            // ribbonPanel5
            // 
            this.ribbonPanel5.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonPanel5.Controls.Add(this.ribbonBar5);
            this.ribbonPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ribbonPanel5.Location = new System.Drawing.Point(0, 26);
            this.ribbonPanel5.Name = "ribbonPanel5";
            this.ribbonPanel5.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            //this.ribbonPanel5.Size = new System.Drawing.Size(1105, 127);
            // 
            // 
            // 
            this.ribbonPanel5.Style.Class = "";
            this.ribbonPanel5.Style.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel5.StyleMouseDown.Class = "";
            this.ribbonPanel5.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel5.StyleMouseOver.Class = "";
            this.ribbonPanel5.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonPanel5.TabIndex = 5;
            this.ribbonPanel5.Visible = false;
            // 
            // ribbonBar5
            // 
            this.ribbonBar5.AutoOverflowEnabled = true;
            // 
            // 
            // 
            this.ribbonBar5.BackgroundMouseOverStyle.Class = "";
            this.ribbonBar5.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar5.BackgroundStyle.Class = "";
            this.ribbonBar5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonBar5.ContainerControlProcessDialogKey = true;
            this.ribbonBar5.Dock = System.Windows.Forms.DockStyle.Left;
            this.ribbonBar5.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.toolStripButton_SalesReport});
            this.ribbonBar5.Location = new System.Drawing.Point(3, 0);
            this.ribbonBar5.Name = "ribbonBar5";
            this.ribbonBar5.Size = new System.Drawing.Size(111, 124);
            this.ribbonBar5.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonBar5.TabIndex = 1;
            this.ribbonBar5.Text = "���������";
            // 
            // 
            // 
            this.ribbonBar5.TitleStyle.Class = "";
            this.ribbonBar5.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar5.TitleStyleMouseOver.Class = "";
            this.ribbonBar5.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // toolStripButton_SalesReport
            // 
            this.toolStripButton_SalesReport.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_SalesReport.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_SalesReport.HotFontBold = true;
            this.toolStripButton_SalesReport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_SalesReport.Image")));
            this.toolStripButton_SalesReport.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_SalesReport.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_SalesReport.Name = "toolStripButton_SalesReport";
            this.toolStripButton_SalesReport.SubItemsExpandWidth = 14;
            this.toolStripButton_SalesReport.Text = "����� \r\n�������";
            this.toolStripButton_SalesReport.Click += new System.EventHandler(this.toolStripButton_SalesReport_Click);
            // 
            // ribbonPanel4
            // 
            this.ribbonPanel4.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonPanel4.Controls.Add(this.ribbonBar3);
            this.ribbonPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ribbonPanel4.Location = new System.Drawing.Point(0, 26);
            this.ribbonPanel4.Name = "ribbonPanel4";
            this.ribbonPanel4.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ribbonPanel4.Size = new System.Drawing.Size(1105, 127);
            // 
            // 
            // 
            this.ribbonPanel4.Style.Class = "";
            this.ribbonPanel4.Style.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel4.StyleMouseDown.Class = "";
            this.ribbonPanel4.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel4.StyleMouseOver.Class = "";
            this.ribbonPanel4.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonPanel4.TabIndex = 4;
            this.ribbonPanel4.Visible = false;
            // 
            // ribbonBar3
            // 
            this.ribbonBar3.AutoOverflowEnabled = true;
            // 
            // 
            // 
            this.ribbonBar3.BackgroundMouseOverStyle.Class = "";
            this.ribbonBar3.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar3.BackgroundStyle.Class = "";
            this.ribbonBar3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonBar3.ContainerControlProcessDialogKey = true;
            this.ribbonBar3.Dock = System.Windows.Forms.DockStyle.Left;
            this.ribbonBar3.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.toolStripButton_water1,
            this.toolStripButton_water2,
            this.toolStripButton_electricity1});
            this.ribbonBar3.Location = new System.Drawing.Point(3, 0);
            this.ribbonBar3.Name = "ribbonBar3";
            this.ribbonBar3.Size = new System.Drawing.Size(310, 124);
            this.ribbonBar3.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonBar3.TabIndex = 1;
            this.ribbonBar3.Text = "���������";
            // 
            // 
            // 
            this.ribbonBar3.TitleStyle.Class = "";
            this.ribbonBar3.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar3.TitleStyleMouseOver.Class = "";
            this.ribbonBar3.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // toolStripButton_water1
            // 
            this.toolStripButton_water1.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_water1.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_water1.HotFontBold = true;
            this.toolStripButton_water1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_water1.Image")));
            this.toolStripButton_water1.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_water1.Name = "toolStripButton_water1";
            this.toolStripButton_water1.SubItemsExpandWidth = 14;
            this.toolStripButton_water1.Text = "�������\r\n���. ����";
            this.toolStripButton_water1.Click += new System.EventHandler(this.toolStripButton_water_Click);
            // 
            // toolStripButton_water2
            // 
            this.toolStripButton_water2.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_water2.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_water2.HotFontBold = true;
            this.toolStripButton_water2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_water2.Image")));
            this.toolStripButton_water2.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_water2.Name = "toolStripButton_water2";
            this.toolStripButton_water2.SubItemsExpandWidth = 14;
            this.toolStripButton_water2.Text = "�������\r\n���. ����";
            this.toolStripButton_water2.Click += new System.EventHandler(this.toolStripButton_water2_Click);
            // 
            // toolStripButton_electricity1
            // 
            this.toolStripButton_electricity1.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_electricity1.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_electricity1.HotFontBold = true;
            this.toolStripButton_electricity1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_electricity1.Image")));
            this.toolStripButton_electricity1.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_electricity1.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_electricity1.Name = "toolStripButton_electricity1";
            this.toolStripButton_electricity1.SubItemsExpandWidth = 14;
            this.toolStripButton_electricity1.Text = "�������\r\n��.";
            this.toolStripButton_electricity1.Click += new System.EventHandler(this.toolStripButton_electricity_Click);
            // 
            // ribbonPanel2
            // 
            this.ribbonPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonPanel2.Controls.Add(this.ribbonBar2);
            this.ribbonPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ribbonPanel2.Location = new System.Drawing.Point(0, 26);
            this.ribbonPanel2.Name = "ribbonPanel2";
            this.ribbonPanel2.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ribbonPanel2.Size = new System.Drawing.Size(1105, 127);
            // 
            // 
            // 
            this.ribbonPanel2.Style.Class = "";
            this.ribbonPanel2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel2.StyleMouseDown.Class = "";
            this.ribbonPanel2.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonPanel2.StyleMouseOver.Class = "";
            this.ribbonPanel2.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonPanel2.TabIndex = 2;
            this.ribbonPanel2.Visible = false;
            // 
            // ribbonBar2
            // 
            this.ribbonBar2.AutoOverflowEnabled = true;
            // 
            // 
            // 
            this.ribbonBar2.BackgroundMouseOverStyle.Class = "";
            this.ribbonBar2.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar2.BackgroundStyle.Class = "";
            this.ribbonBar2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ribbonBar2.ContainerControlProcessDialogKey = true;
            this.ribbonBar2.Dock = System.Windows.Forms.DockStyle.Left;
            this.ribbonBar2.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.toolStripButton_Order2,
            this.toolStripButton_OrderWeek,
            this.toolStripButton_Inv2,
            this.toolStripButton_InvInv});
            this.ribbonBar2.Location = new System.Drawing.Point(3, 0);
            this.ribbonBar2.Name = "ribbonBar2";
            this.ribbonBar2.Size = new System.Drawing.Size(516, 124);
            this.ribbonBar2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.ribbonBar2.TabIndex = 0;
            this.ribbonBar2.Text = "���������";
            // 
            // 
            // 
            this.ribbonBar2.TitleStyle.Class = "";
            this.ribbonBar2.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.ribbonBar2.TitleStyleMouseOver.Class = "";
            this.ribbonBar2.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // toolStripButton_Order2
            // 
            this.toolStripButton_Order2.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Order2.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Order2.HotFontBold = true;
            this.toolStripButton_Order2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Order2.Image")));
            this.toolStripButton_Order2.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Order2.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Order2.Name = "toolStripButton_Order2";
            this.toolStripButton_Order2.SubItemsExpandWidth = 14;
            this.toolStripButton_Order2.Text = "����� ���. ��������";
            this.toolStripButton_Order2.Click += new System.EventHandler(this.toolStripButton_Order2_Click);
            // 
            // toolStripButton_OrderWeek
            // 
            this.toolStripButton_OrderWeek.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_OrderWeek.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_OrderWeek.HotFontBold = true;
            this.toolStripButton_OrderWeek.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_OrderWeek.Image")));
            this.toolStripButton_OrderWeek.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_OrderWeek.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_OrderWeek.Name = "toolStripButton_OrderWeek";
            this.toolStripButton_OrderWeek.SubItemsExpandWidth = 14;
            this.toolStripButton_OrderWeek.Text = "����� ���. ��������";
            this.toolStripButton_OrderWeek.Visible = false;
            this.toolStripButton_OrderWeek.Click += new System.EventHandler(this.toolStripButton_OrderWeek_Click);
            // 
            // toolStripButton_Inv2
            // 
            this.toolStripButton_Inv2.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_Inv2.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_Inv2.HotFontBold = true;
            this.toolStripButton_Inv2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Inv2.Image")));
            this.toolStripButton_Inv2.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_Inv2.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_Inv2.Name = "toolStripButton_Inv2";
            this.toolStripButton_Inv2.SubItemsExpandWidth = 14;
            this.toolStripButton_Inv2.Text = "�����������  \r\n�������";
            this.toolStripButton_Inv2.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton_InvInv
            // 
            this.toolStripButton_InvInv.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_InvInv.FixedSize = new System.Drawing.Size(100, 80);
            this.toolStripButton_InvInv.HotFontBold = true;
            this.toolStripButton_InvInv.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_InvInv.Image")));
            this.toolStripButton_InvInv.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_InvInv.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_InvInv.Name = "toolStripButton_InvInv";
            this.toolStripButton_InvInv.SubItemsExpandWidth = 14;
            this.toolStripButton_InvInv.Text = "������� ���������";
            this.toolStripButton_InvInv.Click += new System.EventHandler(this.toolStripButton_InvInv_Click);
            // 
            // ribbonTabItem1
            // 
            this.ribbonTabItem1.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.ribbonTabItem1.Checked = true;
            this.ribbonTabItem1.HotFontBold = true;
            this.ribbonTabItem1.Image = ((System.Drawing.Image)(resources.GetObject("ribbonTabItem1.Image")));
            this.ribbonTabItem1.Name = "ribbonTabItem1";
            this.ribbonTabItem1.Panel = this.ribbonPanel1;
            this.ribbonTabItem1.Shape = new DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2, 2, 14, 2);
            this.ribbonTabItem1.Text = " ����������";
            // 
            // ribbonTabItem2
            // 
            this.ribbonTabItem2.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.ribbonTabItem2.HotFontBold = true;
            this.ribbonTabItem2.Image = ((System.Drawing.Image)(resources.GetObject("ribbonTabItem2.Image")));
            this.ribbonTabItem2.Name = "ribbonTabItem2";
            this.ribbonTabItem2.Panel = this.ribbonPanel2;
            this.ribbonTabItem2.Text = " ���������";
            // 
            // ribbonTabItem4
            // 
            this.ribbonTabItem4.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.ribbonTabItem4.HotFontBold = true;
            this.ribbonTabItem4.Image = ((System.Drawing.Image)(resources.GetObject("ribbonTabItem4.Image")));
            this.ribbonTabItem4.Name = "ribbonTabItem4";
            this.ribbonTabItem4.Panel = this.ribbonPanel4;
            this.ribbonTabItem4.Text = " ��������";
            // 
            // ribbonTabItem5
            // 
            this.ribbonTabItem5.AutoCheckOnClick = true;
            this.ribbonTabItem5.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.ribbonTabItem5.Image = ((System.Drawing.Image)(resources.GetObject("ribbonTabItem5.Image")));
            this.ribbonTabItem5.Name = "ribbonTabItem5";
            this.ribbonTabItem5.Panel = this.ribbonPanel5;
            this.ribbonTabItem5.Text = " ������";
            // 
            // tab
            // 
            this.tab.Name = "tab";
            this.tab.Panel = this.ribbonPanel6;
            this.tab.Text = "������";
            // 
            // buttonItem6
            // 
            this.buttonItem6.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem6.HotFontBold = true;
            this.buttonItem6.Image = ((System.Drawing.Image)(resources.GetObject("buttonItem6.Image")));
            this.buttonItem6.Name = "buttonItem6";
            this.buttonItem6.Text = " ��������� � ��������";
            this.buttonItem6.Click += new System.EventHandler(this.buttonItem6_Click);
            // 
            // buttonItem30
            // 
            this.buttonItem30.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem30.HotFontBold = true;
            this.buttonItem30.Name = "buttonItem30";
            this.buttonItem30.Text = " �������� �����������";
            this.buttonItem30.Click += new System.EventHandler(this.buttonItem30_Click);
            // 
            // ribbonTabItem3
            // 
            this.ribbonTabItem3.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.ribbonTabItem3.HotFontBold = true;
            this.ribbonTabItem3.Image = ((System.Drawing.Image)(resources.GetObject("ribbonTabItem3.Image")));
            this.ribbonTabItem3.Name = "ribbonTabItem3";
            this.ribbonTabItem3.Panel = this.ribbonPanel3;
            this.ribbonTabItem3.Text = " ����������";
            // 
            // buttonExpanded
            // 
            this.buttonExpanded.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonExpanded.Image = ((System.Drawing.Image)(resources.GetObject("buttonExpanded.Image")));
            this.buttonExpanded.Name = "buttonExpanded";
            this.buttonExpanded.Text = "  �������� ������";
            this.buttonExpanded.Click += new System.EventHandler(this.buttonItem2_Click);
            // 
            // buttonItem7
            // 
            this.buttonItem7.Name = "buttonItem7";
            this.buttonItem7.Text = "buttonItem7";
            // 
            // bar1
            // 
            this.bar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bar1.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.toolStripStatusLabel_Transf,
            this.toolStripStatusLabel_KKM1,
            this.toolStripStatusLabel_KKM2,
            this.toolStripStatusLabel_KKM3,
            this.toolStripStatusLabel_KKM4,
            this.toolStripStatusLabel_KKM5,
            this.progressBarItem1,
            this.Label1});
            this.bar1.Location = new System.Drawing.Point(0, 490);
            this.bar1.Name = "bar1";
            this.bar1.Size = new System.Drawing.Size(1105, 25);
            this.bar1.Stretch = true;
            this.bar1.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.bar1.TabIndex = 4;
            this.bar1.TabStop = false;
            this.bar1.Text = "bar1";
            // 
            // toolStripStatusLabel_Transf
            // 
            this.toolStripStatusLabel_Transf.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStatusLabel_Transf.Image")));
            this.toolStripStatusLabel_Transf.Name = "toolStripStatusLabel_Transf";
            this.toolStripStatusLabel_Transf.Text = "buttonItem5";
            this.toolStripStatusLabel_Transf.Click += new System.EventHandler(this.toolStripStatusLabel_Transf_Click);
            // 
            // toolStripStatusLabel_KKM1
            // 
            this.toolStripStatusLabel_KKM1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStatusLabel_KKM1.Image")));
            this.toolStripStatusLabel_KKM1.Name = "toolStripStatusLabel_KKM1";
            this.toolStripStatusLabel_KKM1.Text = "buttonItem21";
            // 
            // toolStripStatusLabel_KKM2
            // 
            this.toolStripStatusLabel_KKM2.AutoCollapseOnClick = false;
            this.toolStripStatusLabel_KKM2.Enabled = false;
            this.toolStripStatusLabel_KKM2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStatusLabel_KKM2.Image")));
            this.toolStripStatusLabel_KKM2.Name = "toolStripStatusLabel_KKM2";
            this.toolStripStatusLabel_KKM2.Text = "buttonItem22";
            // 
            // toolStripStatusLabel_KKM3
            // 
            this.toolStripStatusLabel_KKM3.Enabled = false;
            this.toolStripStatusLabel_KKM3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStatusLabel_KKM3.Image")));
            this.toolStripStatusLabel_KKM3.Name = "toolStripStatusLabel_KKM3";
            this.toolStripStatusLabel_KKM3.Text = "buttonItem23";
            // 
            // toolStripStatusLabel_KKM4
            // 
            this.toolStripStatusLabel_KKM4.Enabled = false;
            this.toolStripStatusLabel_KKM4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStatusLabel_KKM4.Image")));
            this.toolStripStatusLabel_KKM4.Name = "toolStripStatusLabel_KKM4";
            this.toolStripStatusLabel_KKM4.Text = "buttonItem24";
            // 
            // toolStripStatusLabel_KKM5
            // 
            this.toolStripStatusLabel_KKM5.Enabled = false;
            this.toolStripStatusLabel_KKM5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStatusLabel_KKM5.Image")));
            this.toolStripStatusLabel_KKM5.Name = "toolStripStatusLabel_KKM5";
            this.toolStripStatusLabel_KKM5.Text = "buttonItem24";
            // 
            // progressBarItem1
            // 
            // 
            // 
            // 
            this.progressBarItem1.BackStyle.Class = "";
            this.progressBarItem1.BackStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.progressBarItem1.ChunkGradientAngle = 0F;
            this.progressBarItem1.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            this.progressBarItem1.Name = "progressBarItem1";
            this.progressBarItem1.RecentlyUsed = false;
            this.progressBarItem1.Width = 120;
            // 
            // Label1
            // 
            this.Label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label1.Image = ((System.Drawing.Image)(resources.GetObject("Label1.Image")));
            this.Label1.Name = "Label1";
            // 
            // timer_Exchange
            // 
            this.timer_Exchange.Interval = 100000;
            this.timer_Exchange.Tick += new System.EventHandler(this.timer_Exchange_Tick);
            // 
            // timer_CheckInbox
            // 
            this.timer_CheckInbox.Interval = 100000;
            this.timer_CheckInbox.Tick += new System.EventHandler(this.timer_CheckInbox_Tick);
            // 
            // timer_ZReport
            // 
            this.timer_ZReport.Interval = 5000;
            this.timer_ZReport.Tick += new System.EventHandler(this.timer_ZReport_Tick);
            // 
            // timer_ExchIndicator
            // 
            this.timer_ExchIndicator.Tick += new System.EventHandler(this.timer_ExchIndicator_Tick);
            // 
            // timer_TReport
            // 
            this.timer_TReport.Interval = 3600000;
            this.timer_TReport.Tick += new System.EventHandler(this.timer_TReport_Tick);
            // 
            // backgroundWorker_Inbox
            // 
            this.backgroundWorker_Inbox.WorkerReportsProgress = true;
            this.backgroundWorker_Inbox.WorkerSupportsCancellation = true;
            this.backgroundWorker_Inbox.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_Inbox_DoWork);
            this.backgroundWorker_Inbox.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_Inbox_ProgressChanged);
            this.backgroundWorker_Inbox.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_Inbox_RunWorkerCompleted);
            // 
            // backgroundWorker_ZReport
            // 
            this.backgroundWorker_ZReport.WorkerSupportsCancellation = true;
            this.backgroundWorker_ZReport.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_ZReport_DoWork);
            // 
            // backgroundWorker_TReport
            // 
            this.backgroundWorker_TReport.WorkerSupportsCancellation = true;
            this.backgroundWorker_TReport.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_TReport_DoWork);
            // 
            // backgroundWorker_CheckKKM
            // 
            this.backgroundWorker_CheckKKM.WorkerSupportsCancellation = true;
            this.backgroundWorker_CheckKKM.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_CheckKKM_DoWork);
            // 
            // backgroundWorker_Info
            // 
            this.backgroundWorker_Info.WorkerSupportsCancellation = true;
            this.backgroundWorker_Info.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_Info_DoWork);
            // 
            // time_Info
            // 
            this.time_Info.Interval = 5000;
            this.time_Info.Tick += new System.EventHandler(this.timer_Info_Tick);
            // 
            // backgroundWorker_WalkKkm
            // 
            this.backgroundWorker_WalkKkm.WorkerSupportsCancellation = true;
            this.backgroundWorker_WalkKkm.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_WalkKkm_DoWork);
            // 
            // timer_WalkKkm
            // 
            this.timer_WalkKkm.Interval = 1000;
            //this.timer_WalkKkm.Tick += new System.EventHandler(this.timer_WalkKkm_Tick);
            this.timer_WalkKkm.Tick += new System.EventHandler(this.timer_WalkKkm_Tick);
            // 
            // office2007StartButton1
            // 
            this.office2007StartButton1.AutoExpandOnClick = true;
            this.office2007StartButton1.CanCustomize = false;
            this.office2007StartButton1.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.Image;
            this.office2007StartButton1.ImageFixedSize = new System.Drawing.Size(16, 16);
            this.office2007StartButton1.ImagePaddingHorizontal = 0;
            this.office2007StartButton1.ImagePaddingVertical = 0;
            this.office2007StartButton1.Name = "office2007StartButton1";
            this.office2007StartButton1.ShowSubItems = false;
            this.office2007StartButton1.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.itemContainer1});
            this.office2007StartButton1.Text = "&File";
            // 
            // itemContainer1
            // 
            // 
            // 
            // 
            this.itemContainer1.BackgroundStyle.Class = "RibbonFileMenuContainer";
            this.itemContainer1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.itemContainer1.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.itemContainer1.Name = "itemContainer1";
            this.itemContainer1.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.itemContainer2,
            this.itemContainer4});
            // 
            // itemContainer2
            // 
            // 
            // 
            // 
            this.itemContainer2.BackgroundStyle.Class = "RibbonFileMenuTwoColumnContainer";
            this.itemContainer2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.itemContainer2.ItemSpacing = 0;
            this.itemContainer2.Name = "itemContainer2";
            this.itemContainer2.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.itemContainer3,
            this.galleryContainer1});
            // 
            // itemContainer3
            // 
            // 
            // 
            // 
            this.itemContainer3.BackgroundStyle.Class = "RibbonFileMenuColumnOneContainer";
            this.itemContainer3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.itemContainer3.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.itemContainer3.MinimumSize = new System.Drawing.Size(120, 0);
            this.itemContainer3.Name = "itemContainer3";
            this.itemContainer3.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItem8,
            this.buttonItem9,
            this.buttonItem10,
            this.buttonItem11,
            this.buttonItem12,
            this.buttonItem13});
            // 
            // buttonItem8
            // 
            this.buttonItem8.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem8.Name = "buttonItem8";
            this.buttonItem8.SubItemsExpandWidth = 24;
            this.buttonItem8.Text = "&New";
            // 
            // buttonItem9
            // 
            this.buttonItem9.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem9.Name = "buttonItem9";
            this.buttonItem9.SubItemsExpandWidth = 24;
            this.buttonItem9.Text = "&Open...";
            // 
            // buttonItem10
            // 
            this.buttonItem10.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem10.Name = "buttonItem10";
            this.buttonItem10.SubItemsExpandWidth = 24;
            this.buttonItem10.Text = "&Save...";
            // 
            // buttonItem11
            // 
            this.buttonItem11.BeginGroup = true;
            this.buttonItem11.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem11.Name = "buttonItem11";
            this.buttonItem11.SubItemsExpandWidth = 24;
            this.buttonItem11.Text = "S&hare...";
            // 
            // buttonItem12
            // 
            this.buttonItem12.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem12.Name = "buttonItem12";
            this.buttonItem12.SubItemsExpandWidth = 24;
            this.buttonItem12.Text = "&Print...";
            // 
            // buttonItem13
            // 
            this.buttonItem13.BeginGroup = true;
            this.buttonItem13.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem13.Name = "buttonItem13";
            this.buttonItem13.SubItemsExpandWidth = 24;
            this.buttonItem13.Text = "&Close";
            // 
            // galleryContainer1
            // 
            // 
            // 
            // 
            this.galleryContainer1.BackgroundStyle.Class = "RibbonFileMenuColumnTwoContainer";
            this.galleryContainer1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.galleryContainer1.EnableGalleryPopup = false;
            this.galleryContainer1.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.galleryContainer1.MinimumSize = new System.Drawing.Size(180, 240);
            this.galleryContainer1.MultiLine = false;
            this.galleryContainer1.Name = "galleryContainer1";
            this.galleryContainer1.PopupUsesStandardScrollbars = false;
            this.galleryContainer1.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.labelItem8,
            this.buttonItem14,
            this.buttonItem15,
            this.buttonItem16,
            this.buttonItem17});
            // 
            // labelItem8
            // 
            this.labelItem8.BorderSide = DevComponents.DotNetBar.eBorderSide.Bottom;
            this.labelItem8.BorderType = DevComponents.DotNetBar.eBorderType.Etched;
            this.labelItem8.CanCustomize = false;
            this.labelItem8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelItem8.Name = "labelItem8";
            this.labelItem8.PaddingBottom = 2;
            this.labelItem8.PaddingTop = 2;
            this.labelItem8.Stretch = true;
            this.labelItem8.Text = "Recent Documents";
            // 
            // buttonItem14
            // 
            this.buttonItem14.Name = "buttonItem14";
            this.buttonItem14.Text = "&1. Short News 5-7.rtf";
            // 
            // buttonItem15
            // 
            this.buttonItem15.Name = "buttonItem15";
            this.buttonItem15.Text = "&2. Prospect Email.rtf";
            // 
            // buttonItem16
            // 
            this.buttonItem16.Name = "buttonItem16";
            this.buttonItem16.Text = "&3. Customer Email.rtf";
            // 
            // buttonItem17
            // 
            this.buttonItem17.Name = "buttonItem17";
            this.buttonItem17.Text = "&4. example.rtf";
            // 
            // itemContainer4
            // 
            // 
            // 
            // 
            this.itemContainer4.BackgroundStyle.Class = "RibbonFileMenuBottomContainer";
            this.itemContainer4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.itemContainer4.HorizontalItemAlignment = DevComponents.DotNetBar.eHorizontalItemsAlignment.Right;
            this.itemContainer4.Name = "itemContainer4";
            this.itemContainer4.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItem18,
            this.buttonItem19});
            // 
            // buttonItem18
            // 
            this.buttonItem18.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem18.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonItem18.Name = "buttonItem18";
            this.buttonItem18.SubItemsExpandWidth = 24;
            this.buttonItem18.Text = "Opt&ions";
            // 
            // buttonItem19
            // 
            this.buttonItem19.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem19.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonItem19.Name = "buttonItem19";
            this.buttonItem19.SubItemsExpandWidth = 24;
            this.buttonItem19.Text = "E&xit";
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue;
            // 
            // toolStripButton_water
            // 
            this.toolStripButton_water.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_water.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_water.Image")));
            this.toolStripButton_water.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_water.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_water.Name = "toolStripButton_water";
            this.toolStripButton_water.SubItemsExpandWidth = 14;
            this.toolStripButton_water.Text = "�������� ����";
            this.toolStripButton_water.Click += new System.EventHandler(this.toolStripButton_water_Click);
            // 
            // toolStripButton_electricity
            // 
            this.toolStripButton_electricity.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton_electricity.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_electricity.Image")));
            this.toolStripButton_electricity.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.toolStripButton_electricity.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top;
            this.toolStripButton_electricity.Name = "toolStripButton_electricity";
            this.toolStripButton_electricity.SubItemsExpandWidth = 14;
            this.toolStripButton_electricity.Text = "������� �������������";
            this.toolStripButton_electricity.Click += new System.EventHandler(this.toolStripButton_electricity_Click);
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.Location = new System.Drawing.Point(0, 0);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.TabIndex = 0;
            // 
            // MDIParentMain
            // 
           // this.AutoScaleBaseSize = new System.Drawing.Size(6, 16);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(217)))), ((int)(((byte)(247)))));
           // this.ClientSize = new System.Drawing.Size(1105, 515);
            this.Controls.Add(this.bar1);
            this.Controls.Add(this.ribbonControl1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "MDIParentMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "��� ���������� ����������� (������ 5.0.0.1)";
            this.Activated += new System.EventHandler(this.MDIParentMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MDIParentMain_FormClosing);
            this.Load += new System.EventHandler(this.MDIParentMain_Load);
            this.Resize += new System.EventHandler(this.MDIParentMain_Resize);
            this.ribbonControl1.ResumeLayout(false);
            this.ribbonControl1.PerformLayout();
            this.ribbonPanel1.ResumeLayout(false);
            this.ribbonPanel3.ResumeLayout(false);
            this.ribbonPanel6.ResumeLayout(false);
            this.ribbonPanel5.ResumeLayout(false);
            this.ribbonPanel4.ResumeLayout(false);
            this.ribbonPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        [STAThread]
        static void Main()
        {

#if(STARTTEST)
            log.Info("����� � main");
#endif
            try
            {

                Application.ThreadException +=
               new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException +=
                    new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                Application.Run(new MDIParentMain());


            }catch(Exception ex)
            {
                log.Error("��������� ������ � main ex: "+ex.Message,ex);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ErrorNotifications.Notificate_error("Application_ThreadException", e.Exception.Message);
            Log(e.Exception, "Application_ThreadException");
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                ErrorNotifications.Notificate_error("CurrentDomain_UnhandledException",
                    ((Exception)e.ExceptionObject).Message);
                Log(e.ExceptionObject as Exception, "Application_ThreadException");
            }
            else
            {
                ErrorNotifications.Notificate_error("CurrentDomain_UnhandledException", e.ExceptionObject.ToString());
                Log("Application_ThreadException "+e.ExceptionObject.ToString());
            }
            
        }

        public static void ExecuteTryAction(Action act)
        {
            new TryAction((o) => { act(); }, null) { LogEventt = Log }.Start();
        }
        public static void ExecuteTryAction(Action act,Action catchAct)
        {
            new TryAction((o) => { act(); }, null, catchAct) { LogEventt = Log }.Start();
        }

        AsyncWorker AsyncWorker_Inbox;
        internal WebTaskManager WebTaskSheduler;

        private void timers_start()
        {
            // ���������� ��������� �������
            timer_Exchange.Interval = CParam.TimerSending;

            AsyncWorker_Inbox = new AsyncWorker(new Action(Inbox_exchange_method)) { LogEvent = MDIParentMain.Log, Name = "Inbox" };
            WebTaskSheduler = new WebTaskManager() { LogEvent = Log, Name = "WebTaskSheduler" };

            timer_CheckInbox.Interval = CParam.TimerExchange;
            timer_ZReport.Interval = CParam.TimerKKM;

            time_Info.Interval = StaticConstants.RBINNER_CONFIG.GetProperty<int>
                ("timer_Info_interval", 15000);
            
            //time_Info.Start();


           // this.timer_CheckInbox.Start();
            //this.timer_Exchange.Start();
            

            object t_rep_interval = CellHelper.GetConfValue("t_report_interval_ms");
            if (t_rep_interval != null)
            {
                int int_t_rep_interval = 0;
                if (int.TryParse(t_rep_interval.ToString(), out int_t_rep_interval))
                {
                    this.timer_TReport.Interval = int_t_rep_interval;
                }
            }

            //this.timer_TReport.Start();
            
            this.timer_WalkKkm.Interval = StaticConstants.RBINNER_CONFIG.GetProperty<int>
                ("timer_WalkKkm_interval", 60000);
           // this.timer_WalkKkm.Start();

            //this.timer_ZReport.Start();

            #region label info
            
            this.labelinfo.MouseEnter += (s, e) =>
            {
                this.labelinfo.Visible = false;
            };
            #endregion
        }

        private bool is_timers_working()
        {
            return timer_Exchange.Enabled &&
                    timer_CheckInbox.Enabled &&
                    time_Info.Enabled &&
                    timer_TReport.Enabled &&
                    timer_WalkKkm.Enabled &&
                    timer_ZReport.Enabled;
        }

        private void timers_pause()
        {
            StaticHelperClass.ExecuteInvokeRequiredDelegate_WinForm(this, (o) =>
                {
                    Log("timers_pause");
                    stop_timer(o.timer_Exchange);
                    stop_timer(o.timer_CheckInbox);
                    stop_timer(o.time_Info);
                    stop_timer(o.timer_TReport);
                    stop_timer(o.timer_WalkKkm);
                    stop_timer(o.timer_ZReport);

                    o.toolStripStatusLabel_Transf.Enabled = false;
                });
        }

        private void timers_resume()
        {
            try
            {
                StaticHelperClass.ExecuteInvokeRequiredDelegate_WinForm(this, (o) =>
                {
                    Log("timers_resume");
                    start_timer(o.timer_Exchange);
                    start_timer(o.timer_CheckInbox);
                    start_timer(o.time_Info);
                    start_timer(o.timer_TReport);
                    start_timer(o.timer_WalkKkm);
                    start_timer(o.timer_ZReport);
                    o.toolStripStatusLabel_Transf.Enabled = true;
                    Log("timers_resume completed");

                });
            }catch(Exception ex)
            {
                Log(ex, "timers_resume error");
            }
        }

        private void stop_timer(Timer tmr)
        {
            if (tmr.Enabled)
                tmr.Stop();
        }
        private void start_timer(Timer tmr)
        {
            if (!tmr.Enabled)
                tmr.Start();
        }


        private void test_method()
        {
            CUpdateHelper cupd = new CUpdateHelper();

            #region ���������� ������ �� 5.4.0.0.

            // cupd.ExecuteCommand("UPDATE t_Conf SET conf_value='5.4.0.0' WHERE conf_id=1");        //���������� ������


            #region ���������� �� ������������

            //cupd.ExecuteCommand("DELETE urr_name, urr_id FROM t_UtilReasonRef");          //�������� ������ �������


            //cupd.ExecuteCommand("ALTER TABLE t_UtilReasonRef ADD COLUMN nome_include text(255)");
            //cupd.ExecuteCommand("ALTER TABLE t_UtilReasonRef ADD COLUMN doc_type text(255)");      //���������� ����������� ��������   
            //cupd.ExecuteCommand("ALTER TABLE t_UtilReasonRef ADD COLUMN id_1c text(40)");         //���������� ����������� ��������
        

           //cupd.ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_name) VALUES('����������. ������� �������� ��')");      // ���������� ������ ���� ���������
           //cupd.ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_name) VALUES('����������. ������� �������� ��')");      // ���������� ������ ���� ���������

            #endregion

            #region ���������� �������� �������� �� ������

            #endregion

            #endregion
        }


        public DialogResult EnterPasswordWindow()
        {
            PasswordForm pw = new PasswordForm();
            int k = (int)pw.ShowDialog();
            if ((DialogResult)k == DialogResult.OK)
            {
                return DialogResult.OK;
            }
            return DialogResult.Cancel;
        }

        public const string AdminPassword="123456789";

        private void keyDownEventHandler(object sender, KeyEventArgs e)
        {   
            if (e.KeyCode==Keys.O)
            {
                if (Control.ModifierKeys==Keys.Control)
                {
                    //��������� ���� ����
                    if (EnterPasswordWindow() == DialogResult.OK)
                    {
                        SettingsWindowShow();
                    }
                }
            }
            //if (e.KeyCode == Keys.T)
            //{
            //    if (Control.ModifierKeys == Keys.Control)
            //    {
            //        WindowStarter.PlanSmenOpen();
            //    }
            //}
            //if (e.KeyCode == Keys.V)
            //{
            //    try
            //    {
            //        if (Control.ModifierKeys == Keys.Control)
            //        {
            //            MainProgressReport.Instance.ReportProgress("��������� ��������� ���������� �����",
            //                50);
            //            Web_UpdateSpravValues_WithConfirm(StaticConstants.WebService, "GetTrainingVideoSD", "ConfirmTrainingVideoSD",
            //                new object[] { Convert.ToInt32(CParam.TeremokId), 0 },
            //             o =>
            //             {
            //                 if (UpdateClass.UpdationList.Where(a => a.UpdationType.Equals(o.GetType())).Count() == 0)
            //                 {
            //                     UpdateClass updcls = new UpdateClass() { LogEvent = Log, UpdationType = o.GetType() };
            //                     Log("WebService �������� ���������� ��������� ���������� �����");
            //                     UpdateClass.UpdationList.Add(updcls);
            //                     updcls.����������������������(o);
            //                 }
            //                 else
            //                 {
            //                     Log("���������� ���������� ����� �� ���������! ��� ��� ����!");
            //                 }
            //             });
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log(ex,"������ ���������� �����");
            //    }
            //}
        }


        private void RbClientUpdateToCurrentVersion()
        {
            CUpdateHelper cupd = StaticConstants.UpdateHelper;

            if (CParam.AppCity == 1) //�����
            {
                //������� ����� ���� ������ ���� ������� ������ ����
                if (cupd.IsNewerVersion("5.3.0.0"))
                {
                    Regex reg = new Regex("Data Source=(.*)");
                    string dataBase_path = reg.Match(CParam.ConnString).Groups[1].Value;
                    if (File.Exists(dataBase_path))
                    {
                        File.Copy(dataBase_path, dataBase_path + "_before_updation_reserve_copy", true);
                    }
                }

                if (CParam.AppVer == "5.5.1.0")//������ ����� � ������ 5510
                {
                    cupd.ExecuteCommand("UPDATE t_Conf SET conf_value =  'spbfs01.teremok-spb.local' WHERE conf_param='ftp_server1' ");
                    cupd.ExecuteCommand("UPDATE t_Conf SET conf_value =  '' WHERE conf_param='ftp_server2' ");
                    cupd.ExecuteCommand("UPDATE t_Conf SET conf_value =  '' WHERE conf_param='ftp_server3' ");
                    cupd.ExecuteCommand("UPDATE t_Conf SET conf_value =  '' WHERE conf_param='ftp_server4' ");
                    cupd.ExecuteCommand("UPDATE t_Conf SET conf_value =  '21' WHERE conf_param='ftp_port1' ");
                }

                t_Conf conf_web_service = new t_Conf().SelectFirst<t_Conf>("conf_param='web_service_url'");
                if (conf_web_service != null)
                {
                    if (conf_web_service.conf_value != "https://web.spb.teremok.ru/ARMWeb/ws/ARMWeb")
                    {
                        conf_web_service.conf_value = "https://web.spb.teremok.ru/ARMWeb/ws/ARMWeb";
                        conf_web_service.Update();
                    }
                }
            }

            if (cupd.IsNewerVersion("5.6.0.0"))
            {
                cupd.Update_4_0_0_18();
                cupd.Update_5_3_0_0();
                cupd.Update_5_4_0_0();
                cupd.Update_5_4_0_1();
                cupd.Update_5_4_0_3();
                cupd.Update_5_4_4_0();
                cupd.Update_5_4_4_3();
                cupd.Update_5_4_5_0();
                cupd.Update_5_4_6_0();
                cupd.Update_5_4_7_0();
                cupd.Update_5_4_8_0();
                cupd.Update_5_4_9_0();
                cupd.Update_5_5_0_0();
                cupd.Update_5_5_1_0();
                cupd.Update_5_5_2_0();
                cupd.Update_5_5_3_0();
                cupd.Update_5_5_4_0();
                cupd.Update_5_5_5_0();
                cupd.Update_5_5_6_0();
                cupd.Update_5_5_7_0();
                cupd.Update_5_5_7_1();
                cupd.Update_5_5_8_0();
                cupd.Update_5_5_9_0();
                cupd.Update_5_6_0_0();
            }

            if (cupd.IsNewerVersion("5.6.4.0"))
            {
                cupd.Update_5_6_1_1();
                cupd.Update_5_6_2_0();
                cupd.Update_5_6_3_0();
                cupd.Update_5_6_4_0();
            }

            if (cupd.IsNewerVersion("5.6.5.8"))
            {
                cupd.Update_5_6_4_1();
                SortDocsBySendTypes();
                cupd.Update_5_6_4_3();
                cupd.Update_5_6_4_4();
                insertNewNotificationsToDb();

                cupd.Update_5_6_4_6();
                cupd.Update_5_6_4_8();
                cupd.Update_5_6_4_9();
                cupd.Update_5_6_5_0();
                cupd.Update_5_6_5_1();
                cupd.Update_5_6_5_2();
                cupd.Update_5_6_5_3();

                insertNewNotificationsZReportToDb();

                cupd.Update_5_6_5_5();
                cupd.Update_5_6_5_6();
                cupd.Update_5_6_5_7();
                cupd.Update_5_6_5_8();
            }
            if (cupd.IsNewerVersion("5.6.6.30"))
            {
                cupd.Update_5_6_5_9();
                cupd.Update_5_6_6_0();
                cupd.Update_5_6_6_1();
                cupd.Update_5_6_6_2();
                cupd.Update_5_6_6_21();
                cupd.Update_5_6_6_22();
                cupd.Update_5_6_6_23();
                cupd.Update_5_6_6_24();
                cupd.Update_5_6_6_25();
                cupd.Update_5_6_6_26();
                cupd.Update_5_6_6_27();
                cupd.Update_5_6_6_28();
                cupd.Update_5_6_6_29();
                cupd.Update_5_6_6_30();
            }
            cupd.Update_5_6_6_31();
            cupd.Update_5_6_6_32();
        }

        /// <summary>
        /// ���������� ����������� ���������� ��������� �� ���� ���������
        /// </summary>
        private void Former_Static_Variables_Init()
        {
            //��������� ������ �� ���� � ����������� �����
            StaticConstants.MainWindow = this;


            //�������� ������������ � ��������� ������ �� ���������
            object web_service_url_obj = CellHelper.GetConfValue("web_service_url");
            if (null != web_service_url_obj && web_service_url_obj.ToString() != "") StaticConstants.Web_Service_Url = web_service_url_obj.ToString();
        }

        //private void test_method_emploee()
        //{
        //    string filename = "emploee.bin";
        //    List<Employee> emp = null;
        //    CBData _data = new CBData();


        //    if (!File.Exists(filename))
        //    {
        //        ARMWeb systemService = new ARMWeb();
        //        systemService.Credentials = new NetworkCredential("iteremok", "Ge6!))~nC5@0");

        //        int rec = 0;

        //        Employee[] Employee = systemService.GetEmployee(Convert.ToInt32(CParam.TeremokId), ref rec);
        //        List<Employee> EmployeeLoad_list = new List<Employee>();

        //        emp = Employee.Where(a => a.Work == true).ToList();

        //        ArrayList arr = new ArrayList();
        //        emp.ForEach(a => { arr.Add(a); });
        //        Serializer.binary_write(arr, filename);
        //    }
        //    else
        //    {
        //        ArrayList arr = Serializer.binary_get(filename);
        //        emp = arr.OfType<Employee>().ToList<Employee>();
        //    }

        //    foreach (var info in emp)
        //    {
        //        if (info.JobFunctionName == null)
        //        {
        //            info.JobFunctionName = " ";
        //            info.JobFunctionGuid = " ";
        //        }
        //        log.Debug("ImportEmployee(" + info.Guid + ", " + info.Name + ", " + info.JobFunctionName + ", " + info.JobFunctionGuid + ", " + info.Work + ")");


        //        string _str_command;
        //        OleDbCommand _command = null;
        //        int _doc_id;
        //        //_data.ImportEmployee(_conn, info.Guid.ToString(), info.Name.ToString(), info.JobFunctionName.ToString(), info.JobFunctionGuid.ToString(), info.Work);

        //        string Guid = info.Guid;
        //        string Name = info.Name;
        //        string functionName = info.JobFunctionName;
        //        string functionGuid = info.JobFunctionGuid;
        //        string workField = info.Work;

        //        try
        //        {
        //            _str_command = "SELECT COUNT(*) FROM t_Employee WHERE employee_1C = @employee_1C";
        //            _command = new OleDbCommand();
        //            _command.Connection = _conn;
        //            _command.CommandText = _str_command;
        //            _command.Parameters.Clear();
        //            _command.Parameters.AddWithValue("@employee_1C", Guid);

        //            _doc_id = Convert.ToInt32(_command.ExecuteScalar());
        //            if (_doc_id == 0)
        //            {
        //                _str_command = "INSERT INTO t_Employee (employee_name, employee_1C, employee_FunctionName, employee_FunctionGuid, employee_WorkField) VALUES(@employee_name, @employee_1C, @employee_FunctionName, @employee_FunctionGuid, @employee_WorkField)";
        //                _command.Parameters.Clear();
        //                _command.Parameters.AddWithValue("@employee_name", Name);
        //                _command.Parameters.AddWithValue("@employee_1C", Guid);
        //                _command.Parameters.AddWithValue("@employee_FunctionName", functionName);
        //                _command.Parameters.AddWithValue("@employee_FunctionGuid", functionGuid);
        //                _command.Parameters.AddWithValue("@employee_WorkField", workField);
        //                _command.CommandText = _str_command;
        //                _command.ExecuteNonQuery();
        //            }
        //            else
        //            {
        //                _str_command = "UPDATE t_Employee SET employee_name = @employee_name, employee_FunctionName = @employee_FunctionName, employee_FunctionGuid = @employee_FunctionGuid, employee_WorkField = @employee_WorkField  WHERE employee_1C = '" + Guid + "'";
        //                _command.Parameters.Clear();
        //                _command.Parameters.AddWithValue("@employee_name", Name);
        //                _command.Parameters.AddWithValue("@employee_FunctionName", functionName);
        //                _command.Parameters.AddWithValue("@employee_FunctionGuid", functionGuid);
        //                _command.Parameters.AddWithValue("@employee_WorkField", workField);
        //                _command.CommandText = _str_command;
        //                _command.ExecuteNonQuery();
        //            }
        //        }
        //        catch (Exception _exp)
        //        {
        //            throw _exp;
        //        }

        //    }


        //    //OleDbConnection _conn = null;
        //    //_conn = new OleDbConnection(CParam.ConnString);
        //    //_conn.Open();

        //    //_conn.Close();
        //    //_conn.Dispose();
        //}

        private void MDIParentMain_Load(object sender, EventArgs e)
        {

            try
            {
#if(STARTTEST)
                log.Info("mdiParentMain_load start");
#endif
                //���������������� ���������� ���������
                Former_Static_Variables_Init();
                //��������� �������

                timers_start();
                log.Trace("��������� �� ����� �� ����������");
                RbClientUpdateToCurrentVersion();
                log.Trace("���������");

                //���������� ����������� ���������� ��������� �� ���� ���������
                log.Trace("����������� ����������� ����������");

                StaticConstants.RbClient_Started_time = DateTime.Now;
                log.Trace("���������");
#if(STARTTEST)
                pw.ReportProgress(40);
                log.Trace("mdiParentMain_load check updation ended");
#endif
                log.Trace("�������������� � ��������� �������");
                //���������� ������� �� ������� ������ CTRL+O
                this.KeyPreview = true;
                this.KeyUp += keyDownEventHandler;


#if(DEB)

                // ���������� ��������� �������
                //timer_Exchange.Interval = CParam.TimerSending;
                //timer_CheckInbox.Interval = 1000;// CParam.TimerExchange;
                ////timer_ZReport.Interval = CParam.TimerKKM;

                ////this.time_Info.Start();
                //this.timer_CheckInbox.Start();
                ////this.timer_Exchange.Start();
                ////this.timer_ExchIndicator.Start();
               // AsyncWorker_Inbox = new AsyncWorker(new Action(Inbox_exchange_method)) { LogEvent = MDIParentMain.Log, Name = "Inbox" };
                //object t_rep_interval = CellHelper.GetConfValue("t_report_interval_ms");
                //if (t_rep_interval != null)
                //{
                //    int int_t_rep_interval = 0;
                //    if (int.TryParse(t_rep_interval.ToString(), out int_t_rep_interval))
                //    {
                //        this.timer_TReport.Interval = int_t_rep_interval;
                //    }
                //}

                //this.timer_TReport.Start();

                //this.timer_WalkKkm.Interval = StaticConstants.RBINNER_CONFIG.GetProperty<int>
                  //  ("timer_WalkKkm_interval", 60000);
                //this.timer_WalkKkm.Start();

                //this.timer_ZReport.Start();
#endif




                //log.Trace("������� �������� usb");
                //manager = new UsbManager();
                //manager.StateChanged += new UsbStateChangedEventHandler(DoStateChanged);
                //log.Trace("���������");

#if(STARTTEST)
                pw.ReportProgress(50);
#endif
                monthCalendarAdv1.DisplayMonth = DateTime.Today;

                pw.ReportProgress(60);

                pw.ReportProgress(70);


                log.Trace("�������������� ��������� Cparam");
                CParam.Init();
                log.Trace("���������");

                pw.ReportProgress(75);

                log.Trace("������������� ������� ����");
                ConfForm(); // ������������ �������� ������������� �������� ����
                log.Trace("���������");

                log.Trace("��������� ���� �� �����������");
                monthCalendar();
                log.Trace("���������");

                log.Trace("��������� � ��������� ���� ���� ��������");
                MakeOpenFile();
                log.Trace("���������");

                // ������� ������ ����������

                pw.ReportProgress(80);

                log.Trace("��������� ������� �����");
                FormDoc _form = new FormDoc();
                _form.Dock = DockStyle.Fill;
                _form.MdiParent = this;
                _form.m_teremok_id = m_teremok_id;
                log.Trace("���������� ������� �����");
                _form.Show();
                log.Trace("���������");


                pw.ReportProgress(90);
                //���������� ������ ����� �������� ���������
                log.Trace("��������� ����������� ������ ������������");
                RbClientGlobalStaticMethods.LogEvent = Log;

#if(DEB)
           //     MethodsAfterLoadRoutine();
#else
                AsyncMethodsAfterLoadRoutine();
#endif


                log.Trace("���������");
                pw.ReportProgress(100);

                pw.Stop();

                log.Trace("��������� MDIParent_Main load");
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0002", exp.Message, true);
            }
        }

        private void SortDocsBySendTypes()
        {
            if (StaticConstants.UpdateHelper.IsNewerVersion("5.6.4.2"))
            {
                SortDocsBySendType();
            }
        }



        private void insertNewNotificationsToDb()
        {
            if (StaticConstants.UpdateHelper.IsNewerVersion("5.6.4.5"))
            {
                #region monthcalendar notifications
                new t_InfoMessage()
                {
                    timefrom = new DateTime(2014, 1, 1, 19, 0, 0),
                    doc_type_id = 1,
                    department = CParam.AppCity.ToString(),
                    type=0,
                    description="�� ���������� ������� �� ������ �������� ������� ����",
                    priority = 2,
                    message = "��������� �������  �����! \n �� �������� ��������� ������� �� 20:00 \n  ���������� �����!"
                }.CreateOle();

                new t_InfoMessage()
                {
                    dayofweek=7,
                    doc_type_id = 2,
                    department = CParam.AppCity.ToString(),
                    type = 0,
                    description = "�� ���������� ������� �� ������ �������� ������� ����",
                    message = "��������� �������  �����! \n �� �������� ��������� ������� �� 13:00 \n  ����� �� ������ ��������!"
                }.CreateOle();

                new t_InfoMessage()
                {
                    timefrom = new DateTime(2014, 1, 1, 20, 0, 0),
                    doc_type_id = 10,
                    lastmonthday=true,
                    department = CParam.AppCity.ToString(),
                    type = 0,
                    priority=8,
                    description = "�� ���������� ������� �� ������ �������� ������� ����",
                    message = labelinfo.Text = "��������� �������  �����! \n �� �������� ��������� ������� \n ����������� �������!",
                }.CreateOle();

                new t_InfoMessage()
                {
                    timefrom = new DateTime(2014, 1, 1, 20, 0, 0),
                    doc_type_id = 10,
                    dayofmounth=15,                    
                    department = CParam.AppCity.ToString(),
                    type = 0,
                    priority=8,
                    description = "�� ���������� ������� �� ������ �������� ������� ����",
                    message = labelinfo.Text = "��������� �������  �����! \n �� �������� ��������� ������� \n ����������� �������!",
                }.CreateOle();

                new t_InfoMessage()
                {
                    timefrom = new DateTime(2014, 1, 1, 22, 0, 0),
                    doc_type_id = 5,
                    department = CParam.AppCity.ToString(),
                    type = 0,
                    description = "�� ���������� ������� �� ������ �������� ������� ����",
                    priority = 3,
                    message = "��������� �������  �����! \n ��������� �������� Z-�������!"
                }.CreateOle();

                new t_InfoMessage()
                {
                    timefrom = new DateTime(2014, 1, 1, 19, 0, 0),
                    doc_type_id = 1,
                    department = "1",
                    type = 0,
                    description = "�� ���������� ������� �� ������ �������� ������� ����",
                    priority = 3,
                    message = "��������� �������  �����! \n ������ �� ����� � ������������ \n ���������� ��������� �� 21:00"
                }.CreateOle();

                new t_InfoMessage()
                {
                    timefrom = new DateTime(2014, 1, 1, 19, 0, 0),
                    doc_type_id = 2,
                    department = "1",
                    type = 0,
                    description = "�� ���������� ������� �� ������ �������� ������� ����",
                    priority = 2,
                    message = "��������� �������  �����! \n ������ �� ����� � ������������ \n ���������� ��������� �� 21:00"
                }.CreateOle();

                new t_InfoMessage()
                {
                    timefrom = new DateTime(2014, 1, 1, 22, 0, 0),
                    doc_type_id = 5,
                    department = "1",
                    type = 0,
                    description = "�� ���������� ������� �� ������ �������� ������� ����",
                    priority = 9,
                    message = "��������� �������  �����! \n ��������� �������� Z-�������!"
                }.CreateOle();
#endregion

                #region template notifications
                new t_InfoMessage()
                {
                    doc_type_id = 1,
                    department = "1",
                    type = 1,
                    description = "����������� ��� ���������� �������",
                    message = "������ �� ��������. ���������� � ����������� ����� (sklad.spb@teremok.ru) ��� �������� �������."
                }.CreateOle();
                new t_InfoMessage()
                {
                    doc_type_id = 29,
                    department = "1",
                    type = 1,
                    description = "����������� ��� ���������� �������",
                    message = "������ �� ��������. ���������� � ����������� ����� (sklad.spb@teremok.ru) ��� �������� �������."
                }.CreateOle();
                new t_InfoMessage()
                {
                    doc_type_id = 3,
                    department = "1",
                    type = 1,
                    description = "����������� ��� ���������� �������",
                    message = "������ �� ��������. ���������� � ����������� ������ ��� (���������� �������������) (oper.spb@teremok.ru) ��� �������� �������."
                }.CreateOle();

                new t_InfoMessage()
                {
                    doc_type_id = 9,
                    department = "1",
                    type = 1,
                    description = "����������� ��� ���������� �������",
                    message = "������ �� ��������. ���������� � ����������� ������ ��� (���������� �������������) (oper.spb@teremok.ru) ��� �������� �������."
                }.CreateOle();

                new t_InfoMessage()
                {
                    doc_type_id = 13,
                    department = "1",
                    type = 1,
                    description = "����������� ��� ���������� �������",
                    message = "������ �� ��������. ���������� � ����������� ������ ��� (���������� �������������) (oper.spb@teremok.ru) ��� �������� �������."
                }.CreateOle();
                new t_InfoMessage()
                {
                    doc_type_id = 16,
                    department = "1",
                    type = 1,
                    description = "����������� ��� ���������� �������",
                    message = "������ �� ��������. ���������� � ����������� ����� (sklad.spb@teremok.ru) ��� �������� �������."
                }.CreateOle();
                new t_InfoMessage()
                {
                    doc_type_id = 19,
                    department = "1",
                    type = 1,
                    description = "����������� ��� ���������� �������",
                    message = "������ �� ��������. ���������� � ����������� ������ ���  (DDSDept@teremok.ru) ��� �������� �������."
                }.CreateOle();
                new t_InfoMessage()
                {
                    department = "1",
                    type = 1,
                    description = "����������� ��� ���������� �������",
                    message = "������ �� ��������. ���������� � �� ����� (support.spb@teremok.ru) ��� �������� �������."
                }.CreateOle();

                new t_InfoMessage()
                {
                    department = "2",
                    type = 1,
                    description = "����������� ��� ���������� �������",
                    message = "������ �� ��������. ���������� � �� ����� (support.msk@teremok.ru) ��� �������� �������."
                }.CreateOle();
                #endregion

                StaticConstants.UpdateHelper.SetVersion("5.6.4.5");
            }
        }

        private void insertNewNotificationsZReportToDb()
        {
            if (StaticConstants.UpdateHelper.IsVersion("5.6.5.3"))
            {
                new t_InfoMessage()
                {
                    doc_type_id = 5,
                    department = CParam.AppCity.ToString(),
                    type = 3,
                    description = "�� ���������� ������� �� ��������� �������� ������� ����",
                    priority = 99,
                    message = "��������� �������  �����! \n ���������� ������� ����� �� ����� {0} � ��������� Z-�����!"
                }.CreateOle();

                new t_InfoMessage()
                {
                    doc_type_id = 5,
                    department = CParam.AppCity.ToString(),
                    type = 4,
                    description = "���� ����������� ���������� �� �������",
                    priority = 98,
                    message = "��������� �������  �����! \n �� ����� {0} ������ �������! \n"+
                    " ���������� ��������� ��� ��������, ��� ������� � ��� ��-3!"
                }.CreateOle();

                StaticConstants.UpdateHelper.SetVersion("5.6.5.4");
            }
        }

        private void SortDocsBySendType()
        {
            try
            {
                List<int> xml_list = new List<int>(){1,2,3,4,5,9,10,13,14,15,16,17,18,19,
                21,23,24,26,27,29};
                List<int> web_list=new List<int>(){28,37};
                List<int> not_send_list=new List<int>(){7,8,22,30,31,32,34,35,36,38};

                List<t_DocTypeRef> list_doc_types = new t_DocTypeRef().Select<t_DocTypeRef>("sendtype_type Is Null");
                list_doc_types.ForEach(a =>
                {
                    if (xml_list.Contains(a.doctype_id))
                    {
                        a.sendtype_type = 1;
                        a.Update();
                    }
                    if (web_list.Contains(a.doctype_id))
                    {
                        a.sendtype_type = 2;
                        a.Update();
                    }
                    if (not_send_list.Contains(a.doctype_id))
                    {
                        a.sendtype_type = 0;
                        a.Update();
                    }
                });

                StaticConstants.UpdateHelper.SetVersion("5.6.4.2");
            }
            catch(Exception ex)
            {
                Log(ex, "�� ������� ������������� ���������");
            }
        }

        private void AsyncMethodsAfterLoadRoutine()
        {
            AsyncTaskAction2 asyncaction = new AsyncTaskAction2(() => { MethodsAfterLoadRoutine(); }) {LogEvent=Log};
#if(!DEB)
            asyncaction.Start(); 
#else
            asyncaction.Start(); 
#endif
        }

        private void MethodsAfterLoadRoutine()
        {
            try
            {
                DisableGui();

                // ��� ������� ����� ��������� ������ �� ������� 
                m_just_started = false;

                checkDisableControls();

                Inbox_exchange_method();

                EnableGui();
                //�������� ����� ��� ����
                backgroundWorker_CheckKKM_DoWork(null, null);

                backgroundWorker_ZReport_DoWork(null, null);

                backgroundWorker_TReport_DoWork(null, null);


#if(!DEB)
                #region disabled                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             //���������� ���������� �������� �������� ����������
                //if (StaticConstants.UpdateHelper.IsVersion("5.6.5.5") && CParam.AppCity == 1)
                //{
                //    delete_OldzReports(StaticConstants.RBINNER_CONFIG.GetProperty<int>("doc_delete_month_period", 3));
                //}

                #region copyPrKillersToKKms
                //Action copyPrKillersToKKms = () =>
                //{
                //    try
                //    {
                //        CUpdateHelper cupd = new CUpdateHelper();
                //        if (cupd.IsNewerVersion("5.6.1.0"))
                //        {
                //            Log("�������� ������������� �� prkiller �����");
                //            List<string> emark_folder_pathes = RbClientGlobalStaticMethods.ReturnPosRootFolders();

                //            Log("kkms count=" + emark_folder_pathes.Count);
                //            if (emark_folder_pathes.Count > 0)
                //            {
                //                UpdateClass updc = new UpdateClass() { LogEvent = Log };
                //                emark_folder_pathes.ForEach(a =>
                //                {
                //                    Log("working with " + a);
                //                    updc.copyPrKillerOnKKm(a);
                //                });
                //            }
                //            cupd.SetVersion("5.6.1.0");
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Log(ex, "�� ������� ������ �������� �� �����");
                //    }
                //};
                #endregion
                // copyPrKillersToKKms.Invoke();
                #endregion
               
                log_processing_routine();
                //removeOldWebServiceTask(7);
                //timers_resume();
                
#endif
                
                
            }
            catch (Exception ex)
            {
                Log(ex, "AsyncMethodsAfterLoadRoutine error");
            }
            finally
            {
                timers_resume();
            }
        }

        private void EnableGui()
        {
            StaticHelperClass.ExecuteInvokeRequiredDelegate_WinForm<MDIParentMain>(this, (o) =>
            {
                EnableExchangeButtons();
            });
        }

        private void DisableGui()
        {
            StaticHelperClass.ExecuteInvokeRequiredDelegate_WinForm<MDIParentMain>(this, (o) =>
            {
                DisableExchangeButtons();
            });
        }

        private void DisableExchangeButtons()
        {
            StaticHelperClass.SetControlAvailability(this, "buttonItem6", false);
            StaticHelperClass.SetControlAvailability(this, "buttonItem30", false);
        }

        private void EnableExchangeButtons()
        {
            StaticHelperClass.SetControlAvailability(this, "buttonItem6", true);
            StaticHelperClass.SetControlAvailability(this, "buttonItem30", true);
        }

        private void checkDisableControls()
        {
            var ctrls=StaticConstants.RBINNER_CONFIG.GetProperties("blocked_control");
            foreach (var ctrl in ctrls)
            {

                object o = StaticHelperClass.ReturnClassItemValue(this, ctrl.ToString(),
                    BindingFlags.Instance|BindingFlags.NonPublic);

                if (o is Control)
                {
                    ((Control)o).Enabled = false;
                }

                if (o is BaseItem)
                {
                    ((BaseItem)o).Enabled = false;
                }
                //var a = this.FindControl(ctrl.ToString());
                //if (a != null)
                //{
                //    ((Control)a).Enabled = false;
                //}
            }
            
        }

        public List<FileInfo> ReturnOldDocs(List<string> directories,DateTime lastDate)
        {
            List<FileInfo> flist = new List<FileInfo>();
            directories.ForEach(a =>
            {
                List<FileInfo> _flist = new DirectoryInfo(a).GetFiles().Where(b => b.LastWriteTime < lastDate).ToList();
                if (_flist.NotNullOrEmpty())
                {
                    flist.AddRange(_flist);
                }
            });
            return flist;
        }

        public List<FileInfo> ReturnOldZDocs(int mounthCountBack)
        {
            DateTime lastDate = DateTime.Now.AddMonths(-mounthCountBack);
            List<string> outPathes = new List<string>();
            List<string> _ls = RbClientGlobalStaticMethods.ReturnKKmOutPathes(false);

            if (_ls.NotNullOrEmpty()) outPathes.AddRange(_ls);

                outPathes.Add(RbClientGlobalStaticMethods.GetDirectory(StaticConstants.Z_BACK_FOLDER).FullName);
            if (outPathes == null && outPathes.Count == 0)
            {
                MDIParentMain.Log("outPathes �����. ��� ���� � ����!");
                return null;
            }

            return ReturnOldDocs(outPathes, lastDate);
        }

        private void delete_OldzReports(int monthCountBack)
        {
            Log("�������� ������� z-������ ����� " + monthCountBack + " �������");
            DateTime lastDate = DateTime.Now.AddMonths(-monthCountBack);
            List<string> outPathes = new List<string>();
            List<string> _ls = RbClientGlobalStaticMethods.ReturnKKmOutPathes(false);

            if (_ls.NotNullOrEmpty()) outPathes.AddRange(_ls);
            
            outPathes.Add(RbClientGlobalStaticMethods.GetDirectory(StaticConstants.Z_BACK_FOLDER).FullName);
            if (outPathes == null && outPathes.Count == 0)
            {
                MDIParentMain.Log("outPathes �����. ��� ���� � ����!");
                return;
            }

            List<FileInfo> oldZFiles = ReturnOldDocs(outPathes, lastDate);
            oldZFiles.ForEach(a =>
            {
                new CustomAction((o) =>
                {
                    a.Delete();
                    Log(a.FullName + " ������");
                }, null).Start();
            });
        }

        private void DoStateChanged(UsbStateChangedEventArgs e)
        {
            try
            {
                if (e.State.ToString() == "Added")
                {
                    opdeWindow(e.Disk.Name.ToString());
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                throw (exp);
            }
        }

        private void opdeWindow(string drive)
        {
            Form[] _forms = this.MdiChildren;

            foreach (Form _f in _forms)
            {
                if (_f.Name != "FormDoc")
                {
                    _f.Close();
                }
            }

            FormFlash _form = new FormFlash();

            _form.m_drive = drive;
            _form.ShowDialog();

            if (_form.m_update)
            {
                this.Close();
                return;
            }

            // �������� �������
            foreach (Form _f in _forms)
            {
                if (_f.Name == "FormDoc")
                {
                    FormDoc _fd = (FormDoc)_f;
                    _fd.IninData();
                    break;
                }
            }
        }

        private void CheckOnly1Instance()
        {
#if(!DEB)
            // �������� ������ ���� ��������� ���������
                System.Diagnostics.Process[] prc = System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName);

                if (prc.Length > 1)
                {   
                    Log("������� ��������� ������ �����");
                    log.Info("������� ��������� ������ �����");
                    MessageBox.Show("� ������ ���������� ��� ������� ���. ���� �� ��� �� ������ �� ������, �� ������������� ��������� ��� ��������� ��������� ����� ���� ������� ����������!");
                    this.Close();        
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    return;
                }
#endif
        }

        private void DeleteOldReports()
        {
            CBData _db = new CBData();
            _db.DeleteOldReports();
        }

        private void ConfForm()
        {
            try
            {

                // ���������� ������� ����
                this.SetBounds(4, 4, 1110, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 80);
                m_teremok_id = Convert.ToInt32(CParam.TeremokId);

                CBData _data = new CBData();
                this.Text = _data.TeremokName(m_teremok_id) + m_title;

                // ��������� ��������� ����
                toolStripStatusLabel_KKM1.Enabled = false;
                toolStripStatusLabel_KKM2.Enabled = false;
                toolStripStatusLabel_KKM3.Enabled = false;
                toolStripStatusLabel_KKM4.Enabled = false;
                toolStripStatusLabel_KKM5.Enabled = false;

                m_title = " (��� ���������� ���������� ������ " + CParam.AppVer + ")";

                // ��������� ���������� � ������������ �� �������
                if (CParam.AppCity == 1)
                {
                    buttonItemzakazapb.Text = "����� �����";
                    toolStripButton_Util.Text = "��������";
                    toolStripButton_Spis2.Visible = false;
                    buttonItemzakazapb.Visible = false;
                }
                else
                {
                    buttonItemzakazapb.Visible = false;
                    toolStripButton_OrderWeek.Visible = false;
                    ribbonTabItem5.Visible = false;

#if(DEB)

                    //buttonItemzakazapb.Visible = true;
                    //toolStripButton_OrderWeek.Visible = true;
                    //ribbonTabItem5.Visible = true;

                    //buttonItemzakazapb.Text = "����� ����� ���";
                    //toolStripButton_Util.Text = "�������� ���";
                    //toolStripButton_Spis2.Visible = true;
                    //buttonItemzakazapb.Visible = true;

#endif
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0003", exp.Message, true);
            }
        }

        public void all_Childs_and_close(Form parent)
        {
            List<Form> child_forms=parent.MdiChildren.OfType<Form>().ToList();
            child_forms.ForEach(a => a.Close());
        }

        public void MDIParentMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ���������� �� FTP
            ARMWeb systemService = StaticConstants.WebService;
            try
            {
                all_Childs_and_close(this);

                Log("��������� �������");
                log.Info("��������� �������");
                MakeCloseFile();
                Exchangeclose();
              //  systemService.PutOpenCloseARM(Convert.ToInt32(CParam.TeremokId), DateTime.Now, true);

                // ��������� ��� ������
                backgroundWorker_Inbox.CancelAsync();
                backgroundWorker_CheckKKM.CancelAsync();
                //backgroundWorker_WalkKkm.CancelAsync();
                backgroundWorker_TReport.CancelAsync();
                backgroundWorker_ZReport.CancelAsync();

                if (m_thread_working)
                {
                    if (MessageBox.Show("��� ������������ ������� � ������. �����������, ��� ������������� ������ �������.", "�����������", MessageBoxButtons.YesNoCancel) != System.Windows.Forms.DialogResult.Yes)
                    {
                        Log("������� ������� ��� � ������ ������");
                        log.Info("������� ������� ��� � ������ ������");
                        e.Cancel = true;
                        return;
                    }
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0004", exp.Message, true);
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        // ������ ���� � 10 ���
        private void timer_Exchange_Tick(object sender, EventArgs e)
        {
            try
            {
                #region is update arm
                if (m_update_ARM)
                {
                    if (!m_thread_working)
                    {
                        timer_Exchange.Enabled = false;
                        timer_Exchange.Stop();

                        AsyncTaskAction1 async = new AsyncTaskAction1((o) =>
                        {
                            FormUpdate up = new FormUpdate();
                            up.ShowDialog();

                        }, null).Start();
                        
                    }
                }
                #endregion


                // ���� ��������� � ��������?                
                //CBData _data = new CBData();
                //if (_data.GetDocID2Send() != 0)
                //{
                //    Log("���� ��������� �� ��������");
                //    if (IsWorkerEnabled(backgroundWorker_Inbox))
                //        backgroundWorker_Inbox.RunWorkerAsync();
                //}

                //if (!m_thread_working)
                //    toolStripStatusLabel_Transf.Enabled = false;
                //else
                //    toolStripStatusLabel_Transf.Enabled = true;

            }
            catch (InvalidOperationException exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                // �������� �����, ����������
                ShowMessage("MD0005", "���� �����... ", false);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0006", exp.Message, true);
            }
        }

        /// <summary>
        /// ������ ��� �������������� � ������� ���������� ��� � 1 ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_ExchIndicator_Tick(object sender, EventArgs e)
        {
            return;
            // �������� ������
            if (m_need_refresh_docjournal || m_need_update_menu)
            {
                RefreshDoc();
                m_need_update_menu = false;
                m_need_refresh_docjournal = false;
            }

            // ��������� ������
            if (!m_thread_working)
                toolStripStatusLabel_Transf.Enabled = false;
            else
                toolStripStatusLabel_Transf.Enabled = true;
        }



        private void backgroundWorker_Info_DoWork(object sender, DoWorkEventArgs e)
        {
            NotifyUser();
        }

        private void NotifyUser()
        {
            monthCalendar();

            if (labelinfo.ForeColor == Color.Red)
            {
                labelinfo.ForeColor = Color.Black;
            }
            else
            {
                labelinfo.ForeColor = Color.Red;
            }
        }
        private void timer_Info_Tick(object sender, EventArgs e)
        {
            if (IsWorkerEnabled(backgroundWorker_Info))
            {
                backgroundWorker_Info.RunWorkerAsync();
            }
        }

        #region BUTTONS

        public int CreateOrReturnOrderDB(int type_order, int teremok2_id)
        {
            int _doc_id;
            // ������� ����� �����
            CBData _b = new CBData();
            // ���������, ���� �� ��� ������ �� ������� ����, ���� ����, ��������� ���
            if (teremok2_id == 0) // ��� �� �����������
            {
                _doc_id = _b.OrderCheck(m_teremok_id, type_order);
                if (_doc_id == 0)                    // ������� �����                
                    _doc_id = _b.OrderAdd(type_order, m_teremok_id, 0);
            }
            else
            {
                // ���������
                _doc_id = _b.OrderAdd(type_order, m_teremok_id, teremok2_id);
            }
            return _doc_id;
        }

        private void OpenOrder(int type_order, int teremok2_id)
        {
            #region good
           

            Cursor.Current = Cursors.WaitCursor;

            // ���������, ������� �� ��� ����             
            Form[] _forms = this.MdiChildren;
            bool _f_opened = false;

            try
            {
                foreach (Form _f in _forms)
                {
                    if (_f.Name == "FormGrid" || _f is FormHost )
                    {
                        _f.Activate();
                        _f_opened = true;
                        break;
                    }
                }

                if (!_f_opened)
                {
#endregion
                    
                    int _doc_id;

                    // ������� ����� �����
                    CBData _b = new CBData();
                    // ���������, ���� �� ��� ������ �� ������� ����, ���� ����, ��������� ���
                    if (teremok2_id == 0) // ��� �� �����������
                    {
                        _doc_id = _b.OrderCheck(m_teremok_id, type_order);
                        if (_doc_id == 0)                    // ������� �����                
                            _doc_id = _b.OrderAdd(type_order, m_teremok_id, 0);
                    }
                    else
                    {
                        // ���������
                        _doc_id = _b.OrderAdd(type_order, m_teremok_id, teremok2_id);
                    }

                    if (_doc_id == 0)
                    {
                        //�������� ������ �����������, 
                        string message = "��������! �� �������� ������. �������� � ������ �� ���������.";
                        t_InfoMessage notif = GlobalNotifications.Instance.FindFirstNotificationFilteredByCity("type=1 AND doc_type_id=" + type_order);
                        if (notif == null)
                        {
                            notif = GlobalNotifications.Instance.FindFirstNotificationFilteredByCity("type=1 AND doc_type_id=0");
                            if (notif != null)
                                message = notif.message;
                        }

                        using (new CenterWinDialog(this))
                        {
                            MessageBox.Show(message);
                        }
                        Log("�� �������� ������ ��� " + type_order.ToString());
                        log.Info("�� �������� ������ ��� " + type_order.ToString());
                        return;
                    }

                    // �������� ������
                    foreach (Form _f in _forms)
                    {
                        if (_f.Name == "FormDoc")
                        {
                            FormDoc _fd = (FormDoc)_f;
                            _fd.IninData();
                            break;
                        }
                    }

                    Log("������ �������� ��� " + type_order.ToString() + " id " + _doc_id.ToString());
                    log.Info("������ �������� ��� " + type_order.ToString() + " id " + _doc_id.ToString());

                    // ������� �����
                    FormGrid _form = new FormGrid();
                    _form.MdiParent = this;
                    _form.m_doc_id = _doc_id;
                    _form.m_doc_type_id = type_order;
                    _form.m_teremok_id = m_teremok_id;
                    _form.Show();
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0007", exp.Message, true);
            }
        }

        private void OpenCounter(int type_order, int id_counter)
        {
            Cursor.Current = Cursors.WaitCursor;

            // ���������, ������� �� ��� ����             
            Form[] _forms = this.MdiChildren;
            bool _f_opened = false;

            try
            {
                foreach (Form _f in _forms)
                {
                    if (_f.Name == "FormGrid")
                    {
                        _f.Activate();
                        _f_opened = true;
                        break;
                    }
                }

                if (!_f_opened)
                {
                    // ������� ����� �����
                    CBData _b = new CBData();
                    int _doc_id;



                    // ���������, ���� �� ��� ������ �� ������� ����, ���� ����, ��������� ���
                    _doc_id = _b.OrderCheck(m_teremok_id, type_order);
                    if (_doc_id == 0)                    // ������� �����                
                        _doc_id = _b.OrderAdd(type_order, m_teremok_id, 0);

                    if (_doc_id == 0)
                    {
                        MessageBox.Show("��������! �� �������� ������. �������� � ������ �� ���������.");
                        Log("�� �������� ������ ��� " + type_order.ToString());
                        log.Info("�� �������� ������ ��� " + type_order.ToString());
                        return;
                    }

                    // �������� ������
                    foreach (Form _f in _forms)
                    {
                        if (_f.Name == "FormDoc")
                        {
                            FormDoc _fd = (FormDoc)_f;
                            _fd.IninData();
                            break;
                        }
                    }

                    Log("������ �������� ��� " + type_order.ToString() + " id " + _doc_id.ToString());
                    log.Info("������ �������� ��� " + type_order.ToString() + " id " + _doc_id.ToString());

                    // ������� �����
                    FormGrid _form = new FormGrid();
                    _form.MdiParent = this;
                    _form.m_doc_id = _doc_id;
                    _form.m_doc_type_id = type_order;
                    _form.m_teremok_id = m_teremok_id;
                    _form.Show();
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0007", exp.Message, true);
            }
        }

        // ����� � ������������
        private void toolStripButton_Order1_Click(object sender, EventArgs e)
        {
            OpenOrder(1, 0);
        }
        //�����.����������
        private void toolStripButton_Order29_Click(object sender, EventArgs e)
        {
            OpenOrder(29, 0);
        }


        //����� � �����
        private void toolStripButton_Order30_Click(object sender, EventArgs e)
        {
            WindowStarter.�������_�����_�_�����_����(DateTime.Now);
        }
        // ����� �� �����
        private void toolStripButton_Order2_Click(object sender, EventArgs e)
        {
            OpenOrder(2, 0);
        }

        // �������
        private void toolStripButton_Invent_Click(object sender, EventArgs e)
        {
            OpenOrder(3, 0);
        }

        // ��������
        private void toolStripButton_Util_Click(object sender, EventArgs e)
        {
            OpenOrder(9, 0);
        }

        private void toolStripButton_Spis2_Click(object sender, EventArgs e)
        {
            OpenOrder(13, 0);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenOrder(10, 0);
        }

        private void toolStripButton_InvInv_Click(object sender, EventArgs e)
        {
            OpenOrder(14, 0);
        }

        private void toolStripButton_OrderWeek_Click(object sender, EventArgs e)
        {
            OpenOrder(17, 0);
        }

        private void toolStripButton_OrderMonth_Click(object sender, EventArgs e)
        {
            OpenOrder(18, 0);
        }

        // ��������� ��������� � ����
        private void toolStripButton_Exchange_Click(object sender, EventArgs e)
        {
            Log("������� ����� ������");
            log.Info("������� ����� ������");
            // ��������� ��� �������� ��������� ����� �������
            Form[] _forms = this.MdiChildren;
            try
            {
                foreach (Form _f in _forms)
                {
                    if (_f.Name != "FormDoc")
                    {
                        _f.Close();
                    }
                }

                FormExchange _form = new FormExchange();
                //_form.m_teremok_id = m_teremok_id;
                _form.ShowDialog();

                if (_form.m_update)
                {
                    this.Close();
                    return;
                }

                // �������� �������
                //Form[] _forms = this.MdiChildren;
                foreach (Form _f in _forms)
                {
                    if (_f.Name == "FormDoc")
                    {
                        FormDoc _fd = (FormDoc)_f;
                        _fd.IninData();
                        break;
                    }
                }
                timer_CheckInbox_Tick(null, null);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0009", exp.Message, true);
            }
        }

        // ��������� - �����������
        private void toolStripButton_Transfer_Click(object sender, EventArgs e)
        {

            WindowStarter.��������������(DateTime.Now);

            //WindowStarter.OpenOrCreateOrder(16);
        }

        private void buttonItemknowledge_Click(object sender, EventArgs e)
        {
            FormDBVideo _form = new FormDBVideo();
            _form.MdiParent = this;
            _form.Show();
        }

        // ���� �������
        private void toolStripButton_Help_Click(object sender, EventArgs e)
        {
            try
            {
                Log("������ ���� �������");
                log.Info("������ ���� �������");
                FormHelp _form = new FormHelp();
                _form.MdiParent = this;
                _form.Show();
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0012", exp.Message, true);
            }
        }

        private void toolStripButton_Reestr_Click(object sender, EventArgs e)
        {
            try
            {
                WindowStarter.������������������(this);

                //FormDBReestr _form = new FormDBReestr();
                //_form.MdiParent = this;
                //_form.Show();
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0012", exp.Message, true);
            }
        }

        private void toolStripButton_Survey_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    FormSurvey _form = new FormSurvey();
            //    _form.MdiParent = this;
            //    _form.Show();
            //}
            //catch (Exception exp)
            //{
            //    ShowMessage("MD0048", exp.Message, true);
            //}
        }

        private void toolStripButton_SalesReport_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    Cursor.Current = Cursors.WaitCursor;

            //    GetTReport();

            //    FormSalesReport _form = new FormSalesReport();
            //    _form.MdiParent = this;
            //    _form.Show();
            //}
            //catch (Exception exp)
            //{
            //    ShowMessage("MD0048", exp.Message, true);
            //}
        }


        //// ���� �������
        //private void toolStripButton_Help_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Log("������ ���� �������");
        //        FormHelp _form = new FormHelp();
        //        _form.MdiParent = this;
        //        _form.Show();
        //    }
        //    catch (Exception exp)
        //    {
        //        ShowMessage("MD0012", exp.Message, true);
        //    }
        //}

        #endregion

      
        // �������� ������� � ����� FormDoc
        public void RefreshDoc()
        {
            Form[] _forms = this.MdiChildren;
            try
            {
                foreach (Form _f in _forms)
                {
                    if (_f.Name == "FormDoc")
                    {
                        FormDoc _fd = (FormDoc)_f;
                        _fd.IninData();
                        if (_fd.WindowState != FormWindowState.Normal) _fd.WindowState = FormWindowState.Normal;
                        break;
                    }
                }
                m_need_refresh_docjournal = false;
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0018", exp.Message, true);
            }
        }

        public void ChangeTeremok(int teremok_id, string teremok_name)
        {
            try
            {
                Log("������� ������������� " + teremok_name);
                log.Info("������� ������������� " + teremok_name);
                this.Text = teremok_name + m_title;
                m_teremok_id = teremok_id;

            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0019", exp.Message, true);
            }
        }

        // ��������� �������� ����������
        public void ParseDoc()
        {
            log.Debug("ParseDoc started");
            string[] _s;
            ArrayList _array = new ArrayList();
            char[] _separator = ":".ToCharArray();
            string _teremok_id = "";

            try
            {

                CTransfer _transfer = new CTransfer();
                CBData _data = new CBData();

                DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder + "\\inbox\\");

                foreach (FileInfo _file in _dir.GetFiles())
                {
                    log.Debug("parseDoc: _file='" + _file + "'");
                    if (!_file.Name.EndsWith(".tmp"))
                    {
                        checkIfSqlInstrExist();

                        if (_file.Name.EndsWith(".dll"))
                        {

                            if (_file.Name.Substring(_file.Name.Length - 3, 3) == "dll")
                            {
                                log.Debug("DllLoad(" + _file + ")");
                                DllLoad(_file);
                            }
                        }

                        if (_file.Name == "RBClient.exe" || _file.Name == "rbclient.exe")
                        {
                            log.Debug("setting m_update_ARM = true");
                            m_update_ARM = true;
                            return;
                        }


                        if (_file.Name == "RBClientUpdate.exe")
                        {
                            UpdaterLoad(_file);
                        }

                        // �������� �����
                        if (_file.Name.EndsWith(".WAB") || _file.Name.ToLower().EndsWith(".wab"))
                        {
                            Log("������� ���� �������� �����");
                            log.Info("������� ���� �������� �����");
                            MailLoad(_file);
                        }

                        if (_file.Name.EndsWith(".zip"))
                        {
                            string adv_video_mask = "ADV_VIDEO";
                            // ���� ������������
                            if (_file.Name.IndexOf(adv_video_mask) != -1)
                            {
                                Log("������� ���� ��������� ����������");
                                log.Info("������� ���� ��������� ����������");
                                
                                //���������� ����� �������
                                VideoUpdateClass.OperateAdvArchive(_file);
                                //VideoUpdateClass.OperateAdvArchive(new FileInfo(@"G:\RRepo\myproject\RBClient\Inbox\0000000000_ADV_VIDEO.zip"));
                            }

                            string adv_image_mask = "ADV_IMAGE";
                            // ���� �����������
                            if (_file.Name.IndexOf(adv_image_mask) != -1)
                            {
                                Log("������� ���� ��������� ����������");
                                log.Info("������� ���� ��������� ����������");

                                //���������� ����� �������
                                ImageUpdateClass.OperateAdvArchive(_file);
                            }


                            string video_mask = "EDUC_VIDEO";
                            // ���� �����������
                            if (_file.Name.IndexOf(video_mask) != -1)
                            {
                                Log("������� ���� ���������� �����");
                                log.Info("������� ���� ���������� �����");

                                //���������� ����� �������
                                VideoUpdateClass.OperateEducArchiveDoc(_file);
                            }

                            string pos_mask = "POSDISPLAY";
                            // ���� �����������
                            if (_file.Name.IndexOf(pos_mask,StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                Log("������� ����� POSDISPLAY");
                                log.Info("������� ���� ���������� �����");

                                Action act = () =>
                                {
                                    UpdateClass updcls = new UpdateClass() { LogEvent = Log };

                                    new CustomAction((o) =>
                                    {
                                        updcls.PosDisplayUnArchive(_file);
                                    }, null) {MaxTries=10}.Start();

                                    updcls.PosDisplayLoadOnKKm();
                                    _file.Delete();
                                    VideoUpdateClass.sendVideoFilesToKkm();
                                    ImageUpdateClass.sendVideoFilesToKkm();
                                };
                                act.BeginInvoke(null,null);
                            }

                            string install_mask = "INSTALLPACKAGE";
                            // ���� ������������� ������
                            if (_file.Name.IndexOf(install_mask) != -1)
                            {
                                log.Info("������� ���� ������������� ������" + _file.Name);

                                AsyncTaskAction1 asyncTask = new AsyncTaskAction1((o) =>
                                {
                                    UpdateClass updcls = new UpdateClass() { LogEvent = Log };
                                    if (UpdateClass.OperatingObject == null )
                                    {
                                        Log("�������� ��������� ��������� ������������� ������ " + ((FileInfo)o).Name);
                                        updcls.�����������������������������������((FileInfo)o);
                                    }
                                    else
                                    {
                                        Log("������ �� ���������� �.�. UpdateClass ��� ������������ ������ ���� " + ((FileInfo)o).Name);
                                    }
                                }, _file).Start();
                            }

                            string install_kkm_mask = "KKMPACKAGE";
                            if (_file.Name.IndexOf(install_kkm_mask) != -1)
                            {
                                log.Info("������� ���� ������������� ������ ��� ����" + _file.Name);

                                AsyncTaskAction1 asyncTask = new AsyncTaskAction1((o) =>
                                {
                                    UpdateClass updcls = new UpdateClass() { LogEvent = Log };
                                    if (UpdateClass.OperatingObject == null)
                                    {
                                        Log("�������� ��������� ��������� ������������� ������ " + ((FileInfo)o).Name);
                                        updcls.������������������������������������������((FileInfo)o);
                                    }
                                    else
                                    {
                                        Log("������ �� ���������� �.�. UpdateClass ��� ������������ ������ ���� " + ((FileInfo)o).Name);
                                    }
                                }, _file).Start();
                            }

                            // ���� ����
                            if (_file.Name.Substring(11, 4) == "menu")
                            {
                                Log("������� ���� ����");
                                log.Info("������� ���� ����");
                                MenuLoad(_file, _file.Name.Substring(16, 2));
                            }

                            // ���� ����
                            if (_file.Name.Substring(11, 4) == "card")
                            {
                                Log("������� ���� ��������");
                                log.Info("������� ���� ��������");
                                CardLoad(_file);
                            }

                            // ���� ����
                            if (_file.Name.Substring(6, 5) == "emark")
                            {

                                string[] _name;
                                _name = _file.Name.Split('_', '.');
                                Log("�������� ������ �������" + _name[2]);
                                log.Info("�������� ������ �������" + _name[2]);
                                EmarkLoad(_file, _name[2]);
                            }

                            // ���� ����
                            if (_file.Name.Substring(11, 4) == "help")
                            {
                                Log("������� ���� ��������");
                                log.Info("������� ���� ��������");
                                HelpLoad(_file);
                            }
                        }

                        if (_file.Name.Substring(_file.Name.Length - 3, 3) == "xml")
                        {
                            _separator = "_".ToCharArray();
                            _s = _file.Name.Split(_separator);
                            log.Debug("xml: _s1=" + _s[1] + ", _teremok_id=" + _teremok_id);
                            if (_s[1] != "7-0")
                                _teremok_id = _data.GetTeremokIDBy1C(_s[0]);

                            StaticConstants.Teremok_ID = _teremok_id;

                            switch (_s[1])
                            {
                                case "4-24": // ���������� �������� �������������
                                    Log("������� ���������� �������� �������������");
                                    log.Info("������� ���������� �������� ������������");
                                    _transfer.InsertSpravElick(_file, _s[1], "����� ������ �������� ��.");
                                    StaticConstants.MainGridUpdate();
                                    break;

                                case "39": // ���������� ������� �������� ��
                                    Log("������� ���������� �������� ����������� ����������");
                                    log.Info("������� ���������� �������� ����������� ����������");
                                    _transfer.InsertGroupRefrence(_file, _s[1]);
                                    break;

                                case "31": // ���������� ������� �������� ��
                                    Log("������� ���������� ������� �������� ��");
                                    log.Info("������� ���������� ������� �������� ��");
                                    _transfer.InsertSpravSpisanie(_file,_s[1]);
                                    break;

                                case "32": // ���������� ������� �������� ��
                                    Log("������� ���������� ������� �������� c�");
                                    log.Info("������� ���������� ������� �������� c�");
                                    _transfer.InsertSpravSpisanie(_file,_s[1]);

                                    break;

                                case "4-0": // ���������� �������������
                                    Log("������� ���� �������������");
                                    log.Info("������� ���� �������������");
                                    _transfer.LoadTeremok(_file.FullName);
                                    break;
                                case "4-1": // ���������� ������������, ������������
                                    Log("������� ������ 4-1");
                                    log.Info("������� ������ 4-1");
                                    _transfer.LoadNomenclature(_file.FullName, 1, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-29": // ���������� ������������, ����� ����������
                                    Log("������� ������ 4-29");
                                    log.Info("������� ������ 4-29");
                                    _transfer.LoadNomenclature(_file.FullName, 29, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-2": // ���������� ������������, �����
                                    Log("������� ������ 4-2");
                                    log.Info("������� ������ 4-2");
                                    _transfer.LoadNomenclature(_file.FullName, 2, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-3": // ������������ ��� ��������������
                                    Log("������� ������ 4-3");
                                    log.Info("������� ������ 4-3");
                                    _transfer.LoadNomenclature(_file.FullName, 3, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-6": // ������������ ��� �������� (�������)
                                    Log("������� ������ 4-4");
                                    log.Info("������� ������ 4-4");
                                    _transfer.LoadNomenclature(_file.FullName, 6, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-10": // ������������ ��� 10������� ��������
                                    Log("������� ������ 4-10");
                                    log.Info("������� ������ 4-10");
                                    _transfer.LoadNomenclature(_file.FullName, 10, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-9": // ������������ ��� �� ��������
                                    Log("������� ������ 4-9");
                                    log.Info("������� ������ 4-9");
                                    _transfer.LoadNomenclature(_file.FullName, 9, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-13": // ������������ ��� �������� � �
                                    Log("������� ������ 4-13");
                                    log.Info("������� ������ 4-13");
                                    _transfer.LoadNomenclature(_file.FullName, 13, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-14": // ������������ ��� �������� ���������
                                    Log("������� ������ 4-14");
                                    log.Info("������� ������ 4-14");
                                    _transfer.LoadNomenclature(_file.FullName, 14, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-16": // ������������ ��� ���������
                                    Log("������� ������ 4-16");
                                    log.Info("������� ������ 4-16");
                                    _transfer.LoadNomenclature(_file.FullName, 16, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-17": // ������������ ��� ������ ����������
                                    Log("������� ������ 4-17");
                                    log.Info("������� ������ 4-17");
                                    _transfer.LoadNomenclature(_file.FullName, 17, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-18": // ������������ ��� ������ ���������
                                    Log("������� ������ 4-18");
                                    log.Info("������� ������ 4-18");
                                    _transfer.LoadNomenclature(_file.FullName, 18, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-19": // ������������ ���������
                                    Log("������� ������ 4-19");
                                    log.Info("������� ������ 4-19");
                                    _transfer.LoadNomenclature(_file.FullName, 19, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-20": // ��������� ��������
                                    Log("������� ������ 4-20");
                                    log.Info("������� ������ 4-20");
                                    _transfer.LoadRetReason(_file.FullName);
                                    break;
                                case "4-21": // ������������ ��� ���������
                                    Log("������� ������ 4-21");
                                    log.Info("������� ������ 4-21");
                                    _transfer.LoadNomenclature(_file.FullName, 21, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-23":
                                    Log("������� ������ 4-23");
                                    log.Info("������� ������ 4-23");

                                    _transfer.InsertSpravElick(_file, _s[1], "����� ������ �������� ��.");

                                    _transfer.LoadNomenclature(_file.FullName, 23, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));

                                    break;
                                case "4-25":
                                    Log("������� ������ 4-25");
                                    log.Info("������� ������ 4-25");
                                    _transfer.LoadNomenclature(_file.FullName, 25, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;
                                case "4-27":
                                    Log("������� ������ 4-27");
                                    log.Info("������� ������ 4-27");
                                    _transfer.LoadNomenclature(_file.FullName, 27, Convert.ToInt32(_teremok_id),
                                        Convert.ToInt32(_s[2]));
                                    break;

                                case "21": // �������� �����������
                                    Log("�������� ����. �����������");
                                    log.Info("�������� ����. �����������");
                                    _transfer.LoadTransfer(_file.FullName, Convert.ToInt32(_teremok_id));
                                    break;

                                case "15": // �������� �����������
                                    Log("�������� ����. �����������");
                                    log.Info("�������� ����. �����������");
                                    _transfer.LoadTransfer(_file.FullName, Convert.ToInt32(_teremok_id));
                                    break;
                                case "5-0": // ���������� �������������
                                    Log("������� ���� �������� ����");
                                    log.Info("������� ���� �������� ����");
                                    _transfer.LoadCash(_file.FullName);
                                    break;
                                case "7-0": // ���������� video
                                    Log("������� ������ ���� ������.");
                                    log.Info("������� ������ ���� ������.");
                                    _transfer.LoadVideo(_file.FullName);
                                    break;
                            }
                            m_need_refresh_docjournal = true;
                            StaticConstants.MainGridUpdate();
                        }
                    }
                }

                RbClientGlobalStaticMethods.AfterExchangeRoutine();
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0021", exp.Message, true);
            }
            finally
            {
                log.Debug("ParseDoc finished");
            }
        }


        #region FTP_EXCHANGE

        private void GetFileResuming(FtpSession m_client, String localPath, String name)
        {
            log.Debug("GetFileResuming [localPath='" + localPath + "', name='" + name + "'");
            try
            {
                FtpFile ftpFile = m_client.CurrentDirectory.FindFile(name);
                if (ftpFile != null)
                {
                    long offset = 0;
                    string tmpName = localPath + ".tmp";
                    log.Debug("tmpName='" + tmpName + "'");

                    Boolean needToTransfer = true;
                    // ��������� ���� �� ������ ����
                    FileInfo fiOld = new FileInfo(localPath);
                    if (fiOld.Exists)
                    {
                        if (fiOld.Length < ftpFile.Size)
                        {
                            fiOld.Delete();
                        }
                        else
                        {
                            needToTransfer = false;
                        }
                    }
                    else
                    {
                        log.Debug("fiOld is not exist");
                    }

                    if (needToTransfer)
                    {
                        // ��������� ���� �� ��������� ����
                        FileInfo fiTmp = new FileInfo(tmpName);
                        if (fiTmp.Exists)
                        {
                            offset = fiTmp.Length;
                        }
                        else
                        {
                            log.Debug("1. fiTmp is not exist");
                        }
                        if (offset < ftpFile.Size)
                        {
                            m_client.CurrentDirectory.GetFile(tmpName, name, offset);
                        }
                        // ������� ��������� ����
                        fiTmp = new FileInfo(tmpName);
                        if (fiTmp.Exists)
                        {
                            // ��������������� � ���������� ���
                            fiTmp.MoveTo(localPath);
                        }
                        else
                        {
                            log.Debug("2. fiTmp is not exist");
                        }
                    }
                    else
                    {
                        log.Debug("needToTransfer==false");
                    }

#if(REMFTP)
                    //�������� ���� ���� ���� � ������� �� ������� �� ���
                    FileInfo local_file=new FileInfo(localPath);
                    if (ftpFile.Name == local_file.Name && ftpFile.Size == local_file.Length)
                    {
                        //m_client.CurrentDirectory.RemoveFile(ftpFile.Name);
                    }
#endif
                }
                else
                {
                    log.Debug("ftpFile==null");
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                throw exp;
            }
        }

        private void SendFileResuming(FtpSession m_client, FileInfo _fi)
        {
            log.Debug("SendFileResuming [_fi=" + _fi + "]");
            try
            {
                long offset = 0;
                string tmpName = _fi.Name + ".tmp";

                Boolean needToTransfer = true;
                // ��������� ���� �� ������ ����
                FtpFile oldFtpFile = m_client.CurrentDirectory.FindFile(_fi.Name);
                if (oldFtpFile != null)
                {
                    if (oldFtpFile.Size < _fi.Length)
                    {
                        m_client.CurrentDirectory.RemoveItem(oldFtpFile);
                    }
                    else
                    {
                        needToTransfer = false;
                    }
                }
                else
                {
                    
                    log.Debug("oldFtpFile==null");
                }
                if (needToTransfer)
                {
                    // ��������� ���� �� ��������� ����

                    FtpFile ftpFile = m_client.CurrentDirectory.FindFile(tmpName);
                    if (ftpFile != null)
                    {
                        // ���������� � ����� ���������
                        offset = ftpFile.Size;
                    }
                    else
                    {
                        log.Debug("1. ftpFile==null");
                    }
                    if (offset < _fi.Length)
                    {
                        m_client.CurrentDirectory.PutFile(_fi.FullName, tmpName, offset);
                    }
                    // ������� ��������� ����
                    m_client.CurrentDirectory = m_client.CurrentDirectory;

                    ftpFile = m_client.CurrentDirectory.FindFile(tmpName);
                    if (ftpFile != null)
                    {
                        // ��������������� � ���������� ���
                        m_client.CurrentDirectory.RenameSubitem(ftpFile, _fi.Name);
                    }
                    else
                    {
                        log.Debug("2. ftpFile==null");
                    }
                }
                else
                {
                    log.Debug("needToTransfer==false");
                }
            }
            catch (Exception exp)
            {
                log.Error("Transfer Exception: " + exp, exp);
                Log(exp.ToString());
                throw exp;
            }
        }

        public void Exchange(string _drive)
        {
            log.Debug("Exchange [_drive=" + _drive + "]");
            string _teremok_folder = "";
            string _teremok_id;
            //string _drive;

            try
            {
                checkIfSqlInstrExist();  //�������� ���������� sql


                CBData _data = new CBData();

                DataTable _table_folders;
                DataTable _table_doc;

                _table_folders = _data.GetTeremokFolders();
                log.Debug("_table_folders.Rows.Count=" + _table_folders.Rows.Count);
                foreach (DataRow _row in _table_folders.Rows)
                {
                    _teremok_folder = _row[0].ToString();
                    log.Debug("_teremok_folder='" + _teremok_folder + "'");
                    if (_teremok_folder != "")
                    {

                        _teremok_id = _data.GetTeremokIDBy1C(_teremok_folder);
                        _table_doc = _data.GetDoc2SendExch(_teremok_id);

                        // ��������� ��������� � ��������
                        log.Debug("_table_doc.Rows.Count=" + _table_doc.Rows.Count);

                        foreach (DataRow _row_doc in _table_doc.Rows)
                        {
                            SendFile2Flesh(Convert.ToInt32(_row_doc[0]));
                            Log("��������� ���� ���" + _row_doc[0].ToString());
                            log.Info("��������� ���� ���" + _row_doc[0].ToString());
                        }

                        if (!Directory.Exists(_drive + "\\inbox\\"))
                            Directory.CreateDirectory(_drive + "\\inbox\\");
                        DirectoryInfo _dir = new DirectoryInfo(_drive + "\\inbox\\");
                        foreach (FileInfo _file in _dir.GetFiles())
                        {
                            log.Debug("_dir: _file='" + _file + "'");
                            if (!Directory.Exists(CParam.AppFolder + "\\inbox\\"))
                                Directory.CreateDirectory(CParam.AppFolder + "\\inbox\\");
                            File.Move(_file.FullName, CParam.AppFolder + "\\inbox\\" + _file.Name);
                            File.Delete(_drive + "\\inbox\\" + _file.Name);
                        }

                        ParseDoc();
                        

                        if (!Directory.Exists(_drive + "\\img\\"))
                            Directory.CreateDirectory(_drive + "\\img\\");
                        DirectoryInfo _dir2 = new DirectoryInfo(_drive + "\\img\\");
                        foreach (FileInfo _file in _dir2.GetFiles())
                        {
                            log.Debug("_dir2: _file='" + _file + "'");
                            if (!Directory.Exists(CParam.AppFolder + "\\img\\"))
                                Directory.CreateDirectory(CParam.AppFolder + "\\img\\");
                            File.Copy(_file.FullName, CParam.AppFolder + "\\img\\" + _file.Name, true);
                        }

                        Label1.Text = "����� � ����-������ ��������";
                    }
                    else
                    {
                        log.Debug("_teremok_folder==''");
                    }
                }
            }
            catch (FtpException exp)
            {
                log.Error("Exception: " + exp.Message, exp);
            }
            finally
            {
                
            }
        }

        private void checkIfSqlInstrExist()
        {
            DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder + "\\inbox\\");

            foreach (FileInfo _file in _dir.GetFiles("*.sql"))
            {
                if (_file.Name.EndsWith(".sql"))
                {
                    Log("������� ���� ���������� sql");
                    Log(File.ReadAllText(_file.FullName));
                    List<string> sql_queries = File.ReadAllLines(_file.FullName).ToList();
                    sql_queries.ForEach(a =>
                    {
                        SqlWorker.ExecuteQuerySafe(a);
                    });
                    _file.Delete();
                }
            }
        }

        private void FTPServer(int ftp_ip_num)
        {
            FtpSession m_client = new FtpSession();
            try
            {
                try
                {
                    //if (m_working_zreport)
                    //{   
                    //    StaticHelperClass.ExecuteInvokeRequiredDelegate_WinForm<MDIParentMain>(this, (oo) => { ((MDIParentMain)oo).buttonItem6.Enabled = false; });
                    //    new CustomAction((o) =>
                    //    {
                    //        if (m_working_zreport) throw new Exception();
                    //    }, null) { Timeout = 9000}.Start();

                    //    StaticHelperClass.ExecuteInvokeRequiredDelegate_WinForm<MDIParentMain>(this, (oo) => { ((MDIParentMain)oo).buttonItem6.Enabled = true; });
                    //}
                    if (m_ip_chanel == 0)
                    {
                        m_client.Server = CParam.FtpServer1;
                        m_client.Port = Convert.ToInt32(CParam.FtpPort1);
                        m_client.Connect(CParam.FtpLogin1, CParam.FtpPass1);
                        StaticConstants.CurrentFtpLogin = CParam.FtpLogin1;
                        StaticConstants.CurrentFtpPassword = CParam.FtpPass1;
                        ExchangeFTP(m_client);
                        return;
                    }
                }
                catch (FtpException exp)
                {
                    log.Error("Exception: " + exp.Message, exp);
                    m_error = true;
                }
                catch (Exception exp)
                {
                    log.Error("Exception: " + exp.Message, exp);
                    m_error = true;
                }
                try
                {
                    if (m_error)
                    {
                        m_client.Server = CParam.FtpServer2;
                        m_client.Port = Convert.ToInt32(CParam.FtpPort2);
                        m_client.Connect(CParam.FtpLogin2, CParam.FtpPass2);
                        StaticConstants.CurrentFtpLogin = CParam.FtpLogin2;
                        StaticConstants.CurrentFtpPassword = CParam.FtpPass2;
                        
                        ExchangeFTP(m_client);
                        return;
                    }
                }
                catch (FtpException exp)
                {
                    log.Error("Exception: " + exp.Message, exp);
                    m_error = true;
                }
                catch (Exception exp)
                {
                    log.Error("Exception: " + exp.Message, exp);
                    m_error = true;
                }
                try
                {
                    if (m_error)
                    {
                        m_client.Server = CParam.FtpServer3;
                        m_client.Port = Convert.ToInt32(CParam.FtpPort3);
                        m_client.Connect(CParam.FtpLogin3, CParam.FtpPass3);
                        StaticConstants.CurrentFtpLogin = CParam.FtpLogin3;
                        StaticConstants.CurrentFtpPassword = CParam.FtpPass3;
                        ExchangeFTP(m_client);
                        return;
                    }
                }
                catch (FtpException exp)
                {
                    log.Error("Exception: " + exp.Message, exp);
                    m_error = true;
                }
                catch (Exception exp)
                {
                    log.Error("Exception: " + exp.Message, exp);
                    m_error = true;
                }
                try
                {
                    if (m_error)
                    {
                        m_client.Server = CParam.FtpServer4;
                        m_client.Port = Convert.ToInt32(CParam.FtpPort4);
                        m_client.Connect(CParam.FtpLogin4, CParam.FtpPass4);
                        StaticConstants.CurrentFtpLogin = CParam.FtpLogin4;
                        StaticConstants.CurrentFtpPassword = CParam.FtpPass4;
                        ExchangeFTP(m_client);
                        return;
                    }
                }
                catch (FtpException exp)
                {
                    log.Error("Exception: " + exp.Message, exp);
                    m_error = true;
                }
                catch (Exception exp)
                {
                    log.Error("Exception: " + exp.Message, exp);
                    m_error = true;
                }

            }
            catch (FtpException exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                m_error = true;
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                m_error = true;
            }
            finally
            {
                try
                {
                    if (m_client != null)
                    {
                        m_client.Close();
                        m_client = null;
                    }
                }catch(Exception exp)
                {
                    log.Error("Exception: " + exp.Message, exp);
                }
            }
        }

        private void ExchangeFTP(FtpSession m_client)
        {
            log.Debug("ExchangeFTP started");
            string _teremok_folder = "";
            string _teremok_id;
            

            try
            {
                checkIfSqlInstrExist();

                CBData _data = new CBData();
                

                DataTable _table_folders;
                DataTable _table_doc;
                List<t_WebServiceTask> _table_doc_web=null;

                

                _table_folders = _data.GetTeremokFolders();
                log.Debug("_table_folders.Rows.Count=" + _table_folders.Rows.Count);

                bool isMainResto = false;

                foreach (DataRow _row in _table_folders.Rows)  //�������� �� ������� �������
                {

                    _teremok_folder = _row[0].ToString();
                    isMainResto = IsMainResto(_teremok_folder);

                    #region sending
                    

                    log.Debug("_teremok_folder='" + _teremok_folder + "'");
                    if (_teremok_folder != "")
                    {

                        _teremok_id = _data.GetTeremokIDBy1C(_teremok_folder);
                        Log("������������ � FTP " + _teremok_folder);
                        log.Info("������������ � FTP " + _teremok_folder);
                        Label1.Text = "������������ � FTP";

                         backgroundWorker_Inbox.ReportProgress(50);

                        #region ����������� �� ���


                         Ftp.FtpDirectory rest_dir = m_client.CurrentDirectory.FindSubdirectory(_teremok_folder.ToLower());

                         if (null != rest_dir)
                         {
                             Log("rest_dir " + _teremok_folder + " is Null on ftp");
                             m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(_teremok_folder.ToLower());
                         }
                         else
                         {
                             m_client.CurrentDirectory.CreateSubdir(_teremok_folder.ToLower());

                             m_client.CurrentDirectory.Refresh();
                             //Ftp.FtpDirectory rest_dir1 = m_client.CurrentDirectory.FindSubdirectory(_teremok_folder.ToLower());
                             m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(_teremok_folder.ToLower());
                         }


                         Ftp.FtpDirectory in_dir = m_client.CurrentDirectory.FindSubdirectory("in");
                         if (null != in_dir)
                         {
                             m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory("in");
                         }
                         else
                         {
                             m_client.CurrentDirectory.CreateSubdir("in");
                             m_client.CurrentDirectory.Refresh();
                             m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory("in");
                         }

                        //m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory("in");

                        #endregion


                         if (StaticConstants.UpdateHelper.IsNewerVersion("5.6.4.2"))
                         {
                             _table_doc = _data.GetDoc2SendExch(_teremok_id);
                         }
                         else
                         {
                             //_table_doc_web = _data.GetDoc2SendExch(_teremok_id, 2);
                             _table_doc_web = StaticConstants.WebService1cManager.GetCurrentTasks();
                             _table_doc = _data.GetDoc2SendExch(_teremok_id,1);
                         }

                         if (StaticConstants.UpdateHelper.IsNewerVersion("5.6.4.2"))
                         {
                             #region old exchange

                             log.Debug("_table_doc.Rows.Count=" + _table_doc.Rows.Count);
                             // ��������� ��������� � ��������
                             foreach (DataRow _row_doc in _table_doc.Rows)
                             {
                                 int doc_type = _data.ReturnDocIDFromTask(Convert.ToInt32(_row_doc[0]));
                                 log.Debug("doc_type=" + doc_type);
                                 if (doc_type != 28)
                                 {
                                     SendFile(m_client, _teremok_folder, Convert.ToInt32(_row_doc[0]));
                                     Log("��������� ���� ���" + _row_doc[0].ToString());
                                     log.Info("��������� ���� ���" + _row_doc[0].ToString());
                                     m_need_refresh_docjournal = true;
                                     StaticConstants.MainGridUpdate();
                                 }
                                 else
                                 {
                                     try
                                     {

                                         ARMWeb systemService = StaticConstants.WebService;

                                         BlokDoc _bk = new BlokDoc { Date = DateTime.Now, ID = _data.returnDocGuid(Convert.ToInt32(_row_doc[0])) };
                                         BlokResult _bkr = systemService.IsBlocked(Convert.ToInt32(CParam.TeremokId), _bk, false);


                                         WorkEmployee[] arrWe = new WorkEmployee[_data.CountMark(Convert.ToInt32(_row_doc[0]))];
                                         string docGuid = _data.returnDocGuid(Convert.ToInt32(_row_doc[0]));
                                         log.Debug("docGuid=" + docGuid);
                                         DataTable all = _data.allMark(Convert.ToInt32(_row_doc[0]));
                                         log.Debug("arrWe.Length=" + arrWe.Length);
                                         FormMark2 _mr = new FormMark2();
                                         for (int a = 0; a < arrWe.Length; a++)
                                         {
                                             WorkEmployee we = new WorkEmployee { Name = all.Rows[a].ItemArray[0].ToString(), Responsibility = all.Rows[a].ItemArray[1].ToString(), ArrayDayWork = _mr.myDayArr2(a, Convert.ToInt32(_row_doc[0])) };
                                             log.Debug("WorkEmployee.name=" + we.Name);
                                             arrWe[a] = we;
                                         }

                                         PlanDocument pd = new PlanDocument { Date = DateTime.Today, ID = docGuid.ToString(), ArrayWorkEmployee = arrWe };
                                         string result = systemService.PutDocument(Convert.ToInt32(CParam.TeremokId), pd);
                                         log.Debug("result=" + result);
                                         if (result == "1")
                                         {
                                             _data.TaskUpdateState(Convert.ToInt32(_row_doc[0]), _data.GetStatusID(null, doc_type, 3), "���������� ", true);
                                             Log("��������� ���� ���" + _row_doc[0].ToString());
                                             log.Info("��������� ���� ���" + _row_doc[0].ToString());
                                             m_need_refresh_docjournal = true;
                                             StaticConstants.MainGridUpdate();
                                         }
                                     }
                                     catch (Exception ex)
                                     {
                                         log.Error("�� ���� ��������� �������� �� webservice ", ex);
                                     }
                                 }
                             }

                             #endregion
                         }
                         else
                         {
                             if (_table_doc != null && _table_doc.Rows.Count > 0)
                             {
                                 log.Debug("_table_doc.Rows.Count=" + _table_doc.Rows.Count);
                                 foreach (DataRow _row_doc in _table_doc.Rows)
                                 {
                                     int docId=Convert.ToInt32(_row_doc[0]);
                                     int doc_type = _data.ReturnDocIDFromTask(docId);

                                     try
                                     {
                                         log.Debug("doc_type=" + doc_type);
                                         SendFile(m_client, _teremok_folder, docId);
                                         Log("��������� ���� ���" + docId);
                                         log.Info("��������� ���� ���" + docId.ToString());
                                         m_need_refresh_docjournal = true;
                                         StaticConstants.MainGridUpdate();
                                     }
                                     catch (Exception ex)
                                     {
                                         Log(ex, "SendFile exception. �� ������� ��������� ���� ID="+docId+" Type="+doc_type);
                                     }
                                 }
                             }
                             if (_table_doc_web.NotNullOrEmpty())
                             {
                                 log.Debug("_table_doc_web count=" + _table_doc_web.Count());

                                 foreach(var wtask in _table_doc_web)
                                 {
                                     StaticConstants.WebService1cManager.Execute1Task(wtask);
                                 }

                                 //List<t_TaskExchangeWeb> web_tasks = new t_TaskExchangeWeb().Select<t_TaskExchangeWeb>("teremok_id=" + StaticConstants.Current_Teremok_ID_int);
                                 //if (web_tasks != null && web_tasks.Count > 0)
                                 //{
                                 //    StaticConstants.WebServiceExchanger.SendDocuments(StaticConstants.Current_Teremok_ID_int);
                                 //}

                             }
                         }

                        // �������� ���������� � ������� ����������
                        if (_teremok_folder == _data.GetTeremo1CByID(Convert.ToInt32(CParam.TeremokId)))
                        {
                            log.Debug("uptime.log: found _teremok_folder for id " + CParam.TeremokId);
                            FileInfo _fi = new FileInfo(CParam.AppFolder + "\\uptime.log");
                            SendFileResuming(m_client, _fi);
                        }

                        if (!m_open_log)
                        {
                            if (_teremok_folder == _data.GetTeremo1CByID(Convert.ToInt32(CParam.TeremokId)))
                            {
                                log.Debug("open.log: found _teremok_folder for id " + CParam.TeremokId);
                                FileInfo _fi = new FileInfo(CParam.AppFolder + "\\open.log");
                                SendFileResuming(m_client, _fi);
                                m_open_log = true;
                            }
                        }
                        else
                        {
                            log.Debug("m_open_log==true");
                        }

                        // �������� ���-����
                        if (!m_logfile_flag)
                        {
                            if (_teremok_folder == _data.GetTeremo1CByID(Convert.ToInt32(CParam.TeremokId)))
                            {
                                log.Debug("sendlogfile: found _teremok_folder for id " + CParam.TeremokId);
                                SendLogFile(m_client, _teremok_folder);
                                m_logfile_flag = true;
                            }
                        }
                        else
                        {
                            log.Debug("m_logfile_flag==true");
                        }

                #endregion

                        m_client.CurrentDirectory = m_client.CurrentDirectory.Parent;
                        m_client.CurrentDirectory = m_client.CurrentDirectory.Parent;

                        m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(_teremok_folder.ToLower());
//                        m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory("out");
                        m_client.CurrentDirectory = FindOrCreateFtpDirectory(m_client,"out");

                        List<String> names = new List<string>();
                        VbEnumableCollection items = m_client.CurrentDirectory.Files;
                        log.Debug("items.Count=" + items.Count());

                        //�������� ����� ��� ��������� �������, ������� � ������� ����� ������ � ���
                        string adv_mask = "ADV_VIDEO";


                        List<Regex> reg_list = new List<Regex>();

                        if (isMainResto)
                        {
                            reg_list = StaticConstants.RBINNER_CONFIG.GetPropertiesList<Regex>("stop_file_name");
                        }
                        else
                        {
                            reg_list = StaticConstants.RBINNER_CONFIG.GetPropertiesList<Regex>("slave_resto_stop_file_name");
                        }

                        foreach (FtpFile item in items)
                            {
                                if (item.IsFile)
                                {
                                    if (reg_list != null)
                                    {
                                        bool flag = false;
                                        foreach (Regex reg in reg_list)
                                        {
                                            if (reg.IsMatch(item.Name))
                                            {
                                                flag = true;
                                                Log("��������������� ���� �� ��� " + item.Name);
                                                break;
                                            }
                                        }
                                        if (flag) continue;
                                    }

                                    if (item.Size > 5000000)
                                    {
                                        if (DownloadHelper.Logg == null) DownloadHelper.Logg += log.Error;

                                        string message = "���� ���������� ������...";

                                        if (item.Name.IndexOf("VIDEO") != -1)
                                        {
                                            message = "���� ���������� ��������� �������...";
                                        }
                                        if (item.Name.IndexOf("IMAGE") != -1)
                                        {
                                            message = "���� ���������� ��������� ��������...";
                                        }
                                        if (item.Name.IndexOf("EDUC_VIDEO") != -1)
                                        {
                                            message = "���� ���������� ��������� �������...";
                                            Log("���� "+item.Name+" ������������ "+item.FullPath);
                                        }

                                        DownloadHelper.
                                            AsyncDownloadFromFtp(message, item.Name,_teremok_folder, 
                                                        CParam.FtpServer1, Convert.ToInt32(CParam.FtpPort1),
                                                        CParam.FtpLogin1, CParam.FtpPass1);

                                    }
                                    else
                                    {
                                        Log("�������� ���� " + item.Name);
                                        log.Info("�������� ���� " + item.Name);
                                        GetFileResuming(m_client, CParam.AppFolder + "\\inbox\\" + item.Name, item.Name);
                                        names.Add(item.Name);
                                    }
                                }
                                else
                                {
                                    log.Debug("item is not a file '" + item + "'");
                                }
                            }

                        log.Debug("names.Count=" + names.Count());
                        foreach (String name in names)
                        {
                            Log("������ �� FTP ���� " + name);
                            log.Info("������ �� FTP ���� " + name);
                            m_client.CurrentDirectory.RemoveFile(name);
                        }

                        // ������������ ���������
                        ParseDoc();


                        // ������ � Inbox
                        DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder + "\\inbox\\");
                        foreach (FileInfo _file in _dir.GetFiles())
                        {
                            log.Debug("File in inbox: " + _file);
                            if (_file.Name.ToLower() != "rbclient.exe" 
                                && _file.Name.IndexOf("_ADV_")==-1 
                                && _file.Extension.IndexOf("tmp")==-1)
                            {
                                File.Delete(_file.FullName);
                                Log("������ � Inbox " + _file.Name);
                                log.Info("������ � Inbox " + _file.Name);
                            }
                        }
                        // ������� �� ��������� �������                    
                        m_client.CurrentDirectory = m_client.CurrentDirectory.Parent;
                        m_client.CurrentDirectory = m_client.CurrentDirectory.Parent;
                    }
                }
                toolStripStatusLabel_Transf.Enabled = false;
                m_thread_working = false;
                Label1.Text = "����� ��������";
                ShowMessage("����� �������� ", DateTime.Now.ToLongTimeString(), false);
                m_error = false;
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                MainProgressReport.Instance.ReportProgress("������ ������!", 100);
                throw(exp);
                ShowMessage("MD0034", exp.ToString(), true);
            }
            finally
            {
                m_thread_working = false;
                log.Debug("ExchangeFTP finished");
                StaticConstants.MainGridUpdate();
            }
        }

        private FtpDirectory FindOrCreateFtpDirectory(FtpSession m_client,string subdirname)
        {
            Ftp.FtpDirectory in_dir = m_client.CurrentDirectory.FindSubdirectory(subdirname);
            if (null != in_dir)
            {
                return in_dir;
            }
            else
            {
                m_client.CurrentDirectory.CreateSubdir(subdirname);
                m_client.CurrentDirectory.Refresh();
                in_dir = m_client.CurrentDirectory.FindSubdirectory(subdirname);
                return in_dir;
            }
        }

        public static bool IsMainResto(string _teremok_folder)
        {
            if (StaticConstants.Main_Teremok_1cName.Equals(_teremok_folder, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Exchangeclose()
        {
            log.Debug("Exchangeclose started");
            string _teremok_folder = "";
            string _teremok_id;

            FtpSession m_client = new FtpSession();
            try
            {
                CBData _data = new CBData();

                m_client.Server = CParam.FtpServer1;
                m_client.Port = Convert.ToInt32(CParam.FtpPort1);
                m_client.Connect(CParam.FtpLogin1, CParam.FtpPass1);

                DataTable _table_folders;

                _table_folders = _data.GetTeremokFolders();
                log.Debug("_table_folders.Rows.Count=" + _table_folders.Rows.Count);
                foreach (DataRow _row in _table_folders.Rows)
                {
                    _teremok_folder = _row[0].ToString();
                    log.Debug("_teremok_folder='" + _teremok_folder + "'");
                    if (_teremok_folder != "")
                    {
                        _teremok_id = _data.GetTeremokIDBy1C(_teremok_folder);
                        Log("������������ � FTP " + _teremok_folder);
                        log.Info("������������ � FTP " + _teremok_folder);
                        m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(_teremok_folder.ToLower());
                        m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory("in");

                        if (_teremok_folder == _data.GetTeremo1CByID(Convert.ToInt32(CParam.TeremokId)))
                        {
                            log.Debug("close.log: found _teremok_folder for id " + CParam.TeremokId);
                            FileInfo _fi = new FileInfo(CParam.AppFolder + "\\close.log");
                            SendFileResuming(m_client, _fi);
                            File.Delete(CParam.AppFolder + "\\close.log");
                        }
                    }
                }
            }
            catch (FtpException exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0034", "��� ����� �� FTP", true);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0035", "��� ����� � ������.", true);
            }
            finally
            {
                if (m_client != null)
                {
                    m_client.Close();
                    m_client = null;
                }
            }
        }


        #endregion


        private void SendFile(FtpSession m_client, string _teremok_folder, int doc_id)
        {
            log.Debug("SendFile [_teremok_folder='" + _teremok_folder + ", doc_id='" + doc_id + "']");
            DataTable _dt_item;
            int _type_doc;
            int _teremok_id;
            string _file_name;
            DateTime _datetime;
            FileInfo _fi;
            int _type_doc2 = 0;

            try
            {

                CBData _data = new CBData();
                _teremok_id = _data.GetTeremokIDByDocID(doc_id);

                Label1.Text = "�������������� �������� � ��������";
                backgroundWorker_Inbox.ReportProgress(70);


                // ��������� ��� ���������
                _type_doc = _data.GetTypeDoc(doc_id);
                _type_doc2 = _type_doc;
                _datetime = _data.GetDateDoc(doc_id);
                log.Debug("_type_doc='" + _type_doc + "', _datetime='" + _datetime + "'");

                // ���� ��� �� Z-�����
                if (_type_doc != 5)
                {
                    // ����������� ������� � ��������
                    if (_type_doc == 9 || _type_doc == 13)
                    {
                        _dt_item = _data.ExportOrder913(doc_id, false);
                    }
                    else
                    {
                        _dt_item = _data.ExportOrder(doc_id, false);
                    }
                    // ��� ����� <��� �������>_<��� ���������>_<����>_<id ���������>
                    _file_name = "";

                    if (_type_doc == 1 || _type_doc == 2 || _type_doc == 17 || _type_doc == 18 || _type_doc == 29) // ���� ��� ������, �� ���������� ���� ���� � ���� ���������                    
                        _file_name = _data.GetTeremokFolder(_teremok_id) + "_" + _type_doc.ToString() + "_" + FormatFileDate(_data.GetDateDoc(doc_id), 1) + "_" + doc_id + ".xml";

                    if (_type_doc == 23)
                        _file_name = _data.GetTeremokFolder(_teremok_id) + "_23-3_" + FormatFileDate(_data.GetDateDoc(doc_id), 0) + "_" + doc_id + ".xml";

                    if (_type_doc == 24)
                        _file_name = _data.GetTeremokFolder(_teremok_id) + "_23-1_" + FormatFileDate(_data.GetDateDoc(doc_id), 0) + "_" + doc_id + ".xml";

                    if (_type_doc == 26)
                        _file_name = _data.GetTeremokFolder(_teremok_id) + "_23-2_" + FormatFileDate(_data.GetDateDoc(doc_id), 0) + "_" + doc_id + ".xml";

                    if (_type_doc == 9 || _type_doc == 13)
                        _file_name = _data.GetTeremokFolder(_teremok_id) + "_" + _type_doc.ToString() + "_" + FormatFileDate(_data.GetDateDoc(doc_id), 0) + "_" + doc_id + ".xml";

                    if (_type_doc != 13 && _type_doc != 9 && _type_doc != 1 && _type_doc != 2 && _type_doc != 17 && _type_doc != 18 && _type_doc != 23 && _type_doc != 24 && _type_doc != 26 && _type_doc != 29)
                        _file_name = _data.GetTeremokFolder(_teremok_id) + "_" + _type_doc.ToString() + "_" + FormatFileDate(_data.GetDateDoc(doc_id), 0) + "_" + doc_id + ".xml";


                    _fi = new FileInfo(CParam.AppFolder + "\\Outbox\\" + _file_name);
                    // ������������ ����� � ��������
                    WriteXML(_dt_item, CParam.AppFolder + "\\Outbox\\" + _file_name, doc_id, _type_doc);
                    SendFileResuming(m_client, _fi);
                    _data.TaskUpdateState(doc_id, _data.GetStatusID(null, _type_doc, 3), "���������� ", true);
                    File.Delete(_fi.FullName);
                }
                else
                {
                    // Z-������ ��������� � ����� Outbox, ������� ������� Outbox �� ����� X � �������� � ����
                    CZReportHelper _zreport = new CZReportHelper();
                    _file_name = _data.GetTeremokFolder(_teremok_id) + "_" + _zreport.GetZReportFileName(doc_id); // +"_" + doc_id;
                    _fi = new FileInfo(CParam.AppFolder + "\\Outbox\\" + _file_name + ".zip");

                    if (File.Exists(_fi.FullName))
                    {
                        SendFileResuming(m_client, _fi);

                        #region check fileftp sended
          
                        
                            Log("��������� ��������� �� ���� �� ��� " + _fi.Name);

                            string fname=_fi.Name;
                            var flag = FtpHelperClass.CheckIfFileExist(m_client.ControlChannel.Server,
                                m_client.ControlChannel.Port.ToString(), ref fname, StaticConstants.CurrentFtpLogin,
                                StaticConstants.CurrentFtpPassword,
                                String.Format("/{0}/in",_teremok_folder), StringComparison.OrdinalIgnoreCase,Log);

                            if (!flag)
                            {
                                string message = "����� " + _fi.Name + " �� ��� ���";

                                Log(message);
                                ErrorNotifications.Notificate_error("Error ftp transfer", "�������� �� ���������� �� ��� ���� " + _fi.Name);
                                throw new Exception("Z-report doesn't sended");
                            }
                            else
                            {
                                _data.TaskUpdateState(doc_id, 12, "���������� ", true);
                                UpdateClass.kkm_ConfirmZReportSended(_fi, DateTime.Now);
                                File.Delete(_fi.FullName);
                            }
                            

                            //FtpUploader uploader = new FtpUploader(m_client.ControlChannel.Server, 
                            //m_client.ControlChannel.Port.ToString()
                            //    , StaticConstants.CurrentFtpLogin, StaticConstants.CurrentFtpPassword);

                            //uploader.LogEvent += MDIParentMain.Log;

                            //bool flag = uploader.CheckFileExist(_fi, m_client.RootDirectory.FullName, m_client.CurrentDirectory.FullName, true);

                            //if (!flag)
                            //{
                            //    string message = "����� " + _fi.Name + " �� ��� ���";
                            //    Log(message);

                            //    ErrorNotifications.Notificate_error("Error ftp transfer", "�������� �� ���������� �� ��� ���� "+_fi.Name);
                            //}


                        
                            //FtpFile ftpFile = m_client.CurrentDirectory.FindFile(_fi.Name);
                            
                            //long len1 = _fi.Length;
                            //long len2 = ftpFile.Size;

                            //if (len1 != len2)
                            //{
                            //    //m_client.CurrentDirectory.RemoveFile(_fi.Name);
                            //    return;
                            //}
                        #endregion

                        
                    }
                    else
                    {
                        //  �� ����� �� �������� ����� ������� ���, ���������� ������� ������ � ��� � ����
                        _zreport.ZReportDelete(doc_id);
                    }
                }
                Label1.Text = "�������� ���������";
                backgroundWorker_Inbox.ReportProgress(85);

            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
                throw exp;
            }
        }

        public void SendFile2Flesh(int doc_id)
        {
            DataTable _dt_item;
            int _type_doc;
            int _teremok_id;
            string _file_name;
            DateTime _datetime;
            FileInfo _fi;
            int _type_doc2 = 0;

            try
            {

                CBData _data = new CBData();
                _teremok_id = _data.GetTeremokIDByDocID(doc_id);

                Label1.Text = "�������������� �������� � ��������";
                backgroundWorker_Inbox.ReportProgress(70);

                // ��������� ��� ���������
                _type_doc = _data.GetTypeDoc(doc_id);
                _type_doc2 = _type_doc;
                _datetime = _data.GetDateDoc(doc_id);

                // ���� ��� �� Z-�����
                if (_type_doc != 5)
                {
                    // ����������� ������� � ��������
                    _dt_item = _data.ExportOrder(doc_id, false);

                    // ��� ����� <��� �������>_<��� ���������>_<����>_<id ���������>
                    if (_type_doc == 1 || _type_doc == 2 || _type_doc == 17 || _type_doc == 18) // ���� ��� ������, �� ���������� ���� ���� � ���� ���������
                        _file_name = _data.GetTeremokFolder(_teremok_id) + "_" + _type_doc.ToString() + "_" + FormatFileDate(_data.GetDateDoc(doc_id), 1) + "_" + doc_id + ".xml";
                    else
                        _file_name = _data.GetTeremokFolder(_teremok_id) + "_" + _type_doc.ToString() + "_" + FormatFileDate(_data.GetDateDoc(doc_id), 0) + "_" + doc_id + ".xml";

                    _fi = new FileInfo(CParam.AppFolder + "\\Outbox\\" + _file_name);
                    // ������������ ����� � ��������
                    WriteXML(_dt_item, CParam.AppFolder + "\\Outbox\\" + _file_name, doc_id, _type_doc);
                    //SendFileResuming(m_client, _fi);
                    _data.TaskUpdateState(doc_id, _data.GetStatusID(null, _type_doc, 3), "���������� �� ����- ����� ", true);
                    //File.Delete(_fi.FullName);
                }
                else
                {
                    // Z-������ ��������� � ����� Outbox, ������� ������� Outbox �� ����� X � �������� � ����
                    CZReportHelper _zreport = new CZReportHelper();
                    _file_name = _data.GetTeremokFolder(_teremok_id) + "_" + _zreport.GetZReportFileName(doc_id); // +"_" + doc_id;
                    _fi = new FileInfo(CParam.AppFolder + "\\Outbox\\" + _file_name + ".zip");

                    if (File.Exists(_fi.FullName))
                    {
                        //SendFileResuming(m_client, _fi);
                        _data.TaskUpdateState(doc_id, 12, "���������� ", true);
                        //File.Delete(_fi.FullName);
                    }
                    else
                    {
                        //  �� ����� �� �������� ����� ������� ���, ���������� ������� ������ � ��� � ����
                        _zreport.ZReportDelete(doc_id);
                    }
                }
                Label1.Text = "�������� ���������";
                backgroundWorker_Inbox.ReportProgress(85);

            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        private void start_log_processing_routine()
        {
            AsyncTaskAction2 asta = new AsyncTaskAction2(() =>
            {
                log_processing_routine();
            }){LogEvent=Log};
            asta.Start();

            //System.Threading.Thread thread = new System.Threading.Thread(log_processing_routine);
            //thread.Name = "ThreadLogRoutine";
            //thread.Start();
        }

        #region log processing

        private DateTime? parseDate(Regex reg, string file_name)
        {
            DateTime? result = null;
            try
            {
                string[] starr = new string[] { reg.Match(file_name).Groups[1].Value, reg.Match(file_name).Groups[3].Value, reg.Match(file_name).Groups[4].Value };
                result = DateTime.Parse(String.Format("{0}-{1}-{2}", starr[0], starr[1], starr[2]));

            }
            catch (Exception ex)
            {
                MDIParentMain.log.Error("������ �������� ����� ����� " + file_name, ex);
            }
            return result;
        }

        private DateTime parseDate(FileInfo file_name)
        {
            Regex kkm_log_file_date_regx = new Regex(@"((\d){4}).*?(\d{1,2}).*?(\d{1,2})");
            DateTime? a_date = parseDate(kkm_log_file_date_regx, file_name.Name);

            if (a_date == null) return file_name.CreationTime;

            return (DateTime)a_date;
        }

        private void kkm_log_processing()
        {
            //��� ��� ���������
            Regex kkm_log_file_regx = new Regex(@"(?i:kkm)_(\d+)_(\d+)_(\d+)_(\d+)_(\d+)[.]log");
            Regex kkm_log_file_date_regx = new Regex(@"((\d){4}).*?(\d{1,2}).*?(\d{1,2})");
            string kkm_folder = "kkm_log";
            //�������� ����� ��� ��� � ����� kkm_log �� ����
            List<FileInfo> fi_list = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("kkm*.log").ToList<FileInfo>();
            fi_list.ForEach(a => Debug_classes.File.Move(a.FullName, Path.Combine(kkm_folder, a.Name)));

            fi_list = new DirectoryInfo(kkm_folder).GetFiles("kkm*.log").ToList<FileInfo>();


            Comparison<FileInfo> compar_fi_date = (a, b) =>
            {
                DateTime? a_date = parseDate(kkm_log_file_date_regx, a.Name);
                DateTime? b_date = parseDate(kkm_log_file_date_regx, b.Name);
                if (a_date == null || b_date == null) return a.CreationTime.CompareTo(b.CreationTime);
                return ((DateTime)a_date).CompareTo((DateTime)b_date);
            };

            fi_list.Sort(compar_fi_date);

            DateTime now_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            fi_list = fi_list.Where(a => parseDate(a) < now_date).ToList();
            //���� ���� ����� ����������� ������ �� ���������� �� 

            List<FileInfo> files_toArchive = new List<FileInfo>() { };
            int Month = -1;
            fi_list.ForEach(a =>
            {
                DateTime a_dat = parseDate(a);
                if (Month == -1)
                {
                    files_toArchive.Add(a);
                    Month = a_dat.Month;
                    return;
                }
                if (Month != a_dat.Month)
                {
                    //��������� ����� � �����
                    using (ZipFile _zip = new ZipFile())
                    {
                        DateTime date = parseDate(files_toArchive[0]);
                        string _z_file = Path.Combine(kkm_folder, "kkm_" + date.Month + "." + date.Year + "_archive.zip");
                        _zip.AddFiles(files_toArchive.Select(b => Path.Combine(kkm_folder, b.Name)));
                        _zip.Save(_z_file);
                        files_toArchive.ForEach(c => c.Delete());
                    }
                    //�������� ������
                    files_toArchive.Clear();
                    Month = a_dat.Month;
                }
                files_toArchive.Add(a);

            });
        }

        private void RBClient_log_processing()
        {
            int rbclient_log_days_left = 10;
            FileInfo config = RbClientGlobalStaticMethods.ReturnConfig(StaticConstants.INNER_CONFIG, false);
            if (config != null)
            {
                //����� ���� �� �������
                using (ConfigClass inner_config = new ConfigClass(config.FullName))
                {
                    rbclient_log_days_left = inner_config.GetProperty("rbclient_log_days_left", 10);
                }
            }
            
            string rbclient_Log_folder = "Log";
            DateTime now_date = DateTime.Now.AddDays(-rbclient_log_days_left);

            List<FileInfo> files_toArchive = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), rbclient_Log_folder)).GetFiles("*RBClient.log").ToList<FileInfo>();

            files_toArchive=files_toArchive.Where(a => parseDate(a) < now_date).ToList();

            files_toArchive.Sort((a, b) => parseDate(a).CompareTo(parseDate(b)));
            if (files_toArchive.Count == 0) return;

            using (ZipFile _zip = new ZipFile())
            {
                DateTime date = parseDate(files_toArchive[0]);
                string _z_file = Path.Combine(rbclient_Log_folder, "Rbclient_" + parseDate(files_toArchive.First<FileInfo>()).ToShortDateString() + "-" 
                    + parseDate(files_toArchive.Last<FileInfo>()).ToShortDateString() + "_archive.zip");
                _zip.AddFiles(files_toArchive.Select(b => Path.Combine(rbclient_Log_folder, b.Name)));
                _zip.Save(_z_file);
                files_toArchive.ForEach(c => c.Delete());
            }

        }

        private void nlog_processing()
        {
            string rbclient_Log_folder = "Log";

            List<FileInfo> files_toArchive = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("RBClient.nlog*")
                .ToList<FileInfo>().Where(a=>a.Name!="RBClient.nlog").ToList();

            files_toArchive.Sort((a, b) => a.Name.CompareTo(b.Name));

            if (files_toArchive.Count == 0) return;
            using (ZipFile _zip = new ZipFile())
            {
                string _z_file = Path.Combine(rbclient_Log_folder, "Rbclient_" +files_toArchive.First<FileInfo>().CreationTime.ToShortDateString()+"-"
                    +files_toArchive.Last<FileInfo>().LastWriteTime.ToShortDateString() + "nlog_archive.zip");
                _zip.AddFiles(files_toArchive.Select(b =>  b.Name));
                _zip.Save(_z_file);
                files_toArchive.ForEach(c => c.Delete());
            }

        }

        private void zipus_log_processing()
        {
            FileInfo config = RbClientGlobalStaticMethods.ReturnConfig(StaticConstants.INNER_CONFIG, false);
            if (config != null)
            {
                //����� ���� �� �������
                using (ConfigClass inner_config = new ConfigClass(config.FullName))
                {
                    int rbclient_zipus_days_left = inner_config.GetProperty("rbclient_zipus_days_left", 10);

                    ClearFolderLog(RbClientGlobalStaticMethods.GetDirectory(StaticConstants.OUTBOX_FOLDER),
                        new TimeSpan(rbclient_zipus_days_left, 0, 0, 0), "*.log.zipus");
                }
            }
        }

        private void ztemp_processing()
        {
            var _left = StaticConstants.RBINNER_CONFIG.GetProperty("rbclient_ztemp_month_left", 4);

            ClearFolderLog(RbClientGlobalStaticMethods.GetDirectory(StaticConstants.Z_BACK_FOLDER),
                DateTime.Now.AddMonths(-_left), "*.*");
        }
        private void ztemp_temp_processing()
        {
            var _left = StaticConstants.RBINNER_CONFIG.GetProperty("rbclient_ztemp_month_left", 4)*3;

            var tempf = RbClientGlobalStaticMethods.GetDirectory(StaticConstants.Z_BACK_FOLDER).FindDirectory(StaticConstants.TEMP_FOLDER,false);
            if (tempf != null)
            {
                ClearFolderLog(tempf,
                    DateTime.Now.AddMonths(-_left), "*.*");
            }
        }
        #endregion

        private void log_processing_routine()
        {
            try
            {
                kkm_log_processing();

                RBClient_log_processing();

                //��������� nlog
                nlog_processing();

                zipus_log_processing();
                //������� zipus outbox ztemp
                ztemp_processing();

                ztemp_temp_processing();
                
            }catch(Exception exp){
                MDIParentMain.log.Error("log_processing_routine error "+exp.Message,exp);
            }
        }


        private static void ClearFolderLog(DirectoryInfo dir,TimeSpan time_left,string files_pattern)
        {
            List<FileInfo> list=dir.GetFiles(files_pattern).Where(a => a.CreationTime.Date < DateTime.Now.Date - time_left)
                .ToList();
                list.ForEach(a=>a.Delete());
        }

        private static void ClearFolderLog(DirectoryInfo dir, DateTime lastDate, string files_pattern)
        {
            List<FileInfo> list = dir.GetFiles(files_pattern).Where(a => a.CreationTime.Date < lastDate)
                .ToList();
            list.ForEach(a => a.Delete());
        }

        private void SendLogFile(FtpSession m_client, string _teremok_folder)
        {
            FileInfo _fi;
            string _file = CParam.AppFolder + "\\RBClient.log";
            CheckLog();

            //start_log_processing_routine();

            string _z_file = CParam.AppFolder + "\\Outbox\\" + DateTime.Today.Year.ToString() + "_" +
                 DateTime.Today.Month.ToString() + "_" +
                 DateTime.Today.Day.ToString() + "_" + "RBClient.log.zipus";

            try
            {
                if (File.Exists(_z_file))
                    File.Delete(_z_file);

                using (ZipFile _zip = new ZipFile())
                {
                    _zip.AddFile(_file);
                    _zip.Save(_z_file);
                }

                // ������������ ����� � ��������
                _fi = new FileInfo(_z_file);
              //  SendFileResuming(m_client, _fi);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        //private void updateKashevarFilter(DirectoryInfo dir)
        //{
        //     //����� ������ ����� �� �����
        //     object posDisplayVideoRegex = CellHelper.GetConfValue("folder_kkm_videoName_Mask");
        //     Regex reg=new Regex(posDisplayVideoRegex.ToString());
        //     List<FileInfo> posDisplays_fi=dir.GetFiles().ToList<FileInfo>().Where(a => reg.IsMatch(a.Name)).ToList();

        //     //����������� ����� � ����� �����������
        //     try
        //     {
        //         List<string> kkm_folders_lst = new List<string>(){CellHelper.GetConfValue("folder_kkm1_video").ToString(),CellHelper.GetConfValue("folder_kkm1_video").ToString()
        //         ,CellHelper.GetConfValue("folder_kkm1_video").ToString(),CellHelper.GetConfValue("folder_kkm1_video").ToString()};

        //         ThreadCopyClass.Reset();
        //         ThreadCopyClass.ThreadCopy(posDisplays_fi, kkm_folders_lst[0]);

        //         //kkm_folders_lst.ForEach(a =>
        //         //    {
        //         //        ThreadCopyClass.ThreadCopy(posDisplays_fi, a);
        //         //    });
        //     }catch(Exception exp)
        //     {
        //         Log(exp,"�� ������� ������� ������ �����������");
        //     }
        //}

        // �������� ����
        public void MenuLoad(FileInfo file_menu, string menu_dep)
        {
            string _nome_1c;
            string _nome_name;
            string _menu_date;
            int _menu_doc_id;

            try
            {
                #region old
                CBData _data = new CBData();
                // ������������� ������ � ����������� ����� Menu
                if (!Directory.Exists(CParam.AppFolder + "\\Menu\\" + menu_dep))
                    Directory.CreateDirectory(CParam.AppFolder + "\\Menu\\" + menu_dep);

                // ���������� ��������� � ����
                CMenuHelper _menu = new CMenuHelper();
                _menu_date = file_menu.Name.Substring(26, 2) + "/" + file_menu.Name.Substring(24, 2) + "/" + file_menu.Name.Substring(20, 4);
                _menu_doc_id = _menu.MenuDoc(CParam.TeremokId, _menu_date, menu_dep);

                // ������ ������ ����, ���� ����
                #endregion

                string dir_to_Unzip_path = CParam.AppFolder + "\\Menu\\" + menu_dep + "\\" + _menu_doc_id.ToString();
                DirectoryInfo dir_to_Unzip_di = new DirectoryInfo(dir_to_Unzip_path);

                #region unzip
                if (!dir_to_Unzip_di.Exists)
                    dir_to_Unzip_di.Create();
                    //Directory.CreateDirectory(CParam.AppFolder + "\\Menu\\" + menu_dep + "\\" + _menu_doc_id.ToString());
                else
                {
                    DirectoryInfo _dir_in = new DirectoryInfo(CParam.AppFolder + "\\Menu\\" + menu_dep + "\\" + _menu_doc_id.ToString());
                    foreach (FileInfo _file in _dir_in.GetFiles())
                    {
                        File.Delete(_file.FullName);
                    }
                }

                using (ZipFile _zip = ZipFile.Read(file_menu.FullName))
                {
                    foreach (ZipEntry e in _zip)
                    {
                        FileInfo _fi = new FileInfo(e.FileName);
                        using (FileStream fs = new FileStream(CParam.AppFolder + "\\Menu\\" + menu_dep + "\\" + _menu_doc_id.ToString() + "\\" + _fi.Name, FileMode.Create))
                        {
                            e.Extract(fs);
                        }
                    }
                }
                #endregion
 
                //����� ����� ����������� �� ���������� ��������
                //updateKashevarFilter(dir_to_Unzip_di);
               
                // ������� � ���� � ���� ��� ����������
                if (CParam.LoadReportToDataBase == 1)
                {
                    Log("��������� ����");
                    FileInfo m_file_menu = null; // ���� ����        
                    String name = file_menu.Name.Substring(19);
                    name = name.Substring(0, name.Length - 4);

                    m_file_menu = new FileInfo(CParam.AppFolder + "\\Menu\\" + menu_dep + "\\" + _menu_doc_id.ToString() + "\\" + name+".txt");
                    Log("���� ���� "+ m_file_menu.FullName);


                    FileInfo lun = null;
                    if (CheckLunchFile(dir_to_Unzip_di, ref lun))
                    {
                        m_file_menu = lun;
                    }

                    
                    UpdateMenuTable(m_file_menu);
                }
                m_need_update_menu = true;
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        private bool CheckLunchFile(DirectoryInfo dir_to_Unzip_di, ref FileInfo lun)
        {
            string lunches_txt = StaticConstants.RBINNER_CONFIG.GetProperty<string>("lunches_menu_file", "lunches.txt");
            FileInfo lu = dir_to_Unzip_di.FindFile(lunches_txt, false);
            if (lu != null)
            {
                lun = lu;
                return true;
            }
            return false;
        }

        private static void UpdateMenuTable(FileInfo m_file_menu)
        {
            StreamReader _sr = new StreamReader(m_file_menu.FullName, System.Text.Encoding.GetEncoding(1251));
            string _line;
            Regex menu_reg = new Regex(@".+?(\d{13})(.+?)(\d+[.]\d{2})");
            List<t_Menu> t_men_list = new List<t_Menu>();

            // ������� ������� ����
            while ((_line = _sr.ReadLine()) != null)
            {
                if (menu_reg.IsMatch(_line))
                {
                    Match m = menu_reg.Match(_line);

                    t_Menu menu_item = new t_Menu();
                    menu_item.menu_nome_1C = m.Groups[1].Value;
                    menu_item.menu_nome = m.Groups[2].Value.Trim();
                    menu_item.price = 0;
                    decimal.TryParse(m.Groups[3].Value.Replace(".", ","), out menu_item.price);
                    t_men_list.Add(menu_item);
                }
            }

            // ������� ����
            _sr.Close();
            _sr.Dispose();
            _sr = null;

            Log("��������� ���������� ����");
            new t_Menu().UpdateSprav<t_Menu>(t_men_list, a => a.menu_nome_1C, a => a.menu_nome_1C + a.menu_nome + a.price.ToString());
            Log("��������");
        }

        /// <summary>
        /// �������� ������ ��� Emark
        /// </summary>
        /// <param name="file_menu"></param>
        public void EmarkLoad(FileInfo file_emark, string buildNumber)
        {
            int _doc_id;
            string _emark_date;

            try
            {
                // ������������� ������ � ����������� ����� Emark
                if (!Directory.Exists(CParam.AppFolder + "\\Emark\\" + buildNumber))
                    Directory.CreateDirectory(CParam.AppFolder + "\\Emark\\" + buildNumber);

                //// ������ ������
                //DirectoryInfo _dir_in = new DirectoryInfo(CParam.AppFolder + "\\Emark\\");
                //foreach (FileInfo _file in _dir_in.GetFiles())
                //{
                //    File.Delete(_file.FullName);
                //}

                using (ZipFile _zip = ZipFile.Read(file_emark.FullName))
                {
                    foreach (ZipEntry e in _zip)
                    {
                        FileInfo _fi = new FileInfo(e.FileName);
                        using (FileStream fs = new FileStream(CParam.AppFolder + "\\Emark\\" + buildNumber + "\\" + _fi.Name, FileMode.Create))
                        {
                            e.Extract(fs);
                        }
                    }
                }

                CMenuHelper _menu = new CMenuHelper();
                _emark_date = AttachZeroToDate(DateTime.Today.Year) + "/" + AttachZeroToDate(DateTime.Today.Month) + "/" + AttachZeroToDate(DateTime.Today.Day);
                _doc_id = _menu.Emark(CParam.TeremokId, _emark_date, buildNumber); // �������� ��������                

                m_need_update_menu = true;
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        /// <summary>
        /// �������� ���� �����������.
        /// </summary>
        /// <param name="file_menu"></param>
        public void CardLoad(FileInfo file_menu)
        {
            string _card_date;
            int _doc_id;

            try
            {
                // ������������� ������ � ����������� ����� Card
                if (!Directory.Exists(CParam.AppFolder + "\\Card"))
                    Directory.CreateDirectory(CParam.AppFolder + "\\Card");

                // ������ ������ Card
                DirectoryInfo _dir_in = new DirectoryInfo(CParam.AppFolder + "\\Card\\");
                foreach (FileInfo _file in _dir_in.GetFiles())
                {
                    File.Delete(_file.FullName);
                }

                using (ZipFile _zip = ZipFile.Read(file_menu.FullName))
                {
                    foreach (ZipEntry e in _zip)
                    {
                        FileInfo _fi = new FileInfo(e.FileName);
                        using (FileStream fs = new FileStream(CParam.AppFolder + "\\Card\\" + _fi.Name, FileMode.Create))
                        {
                            e.Extract(fs);
                        }
                    }
                }

                CMenuHelper _menu = new CMenuHelper();
                _card_date = file_menu.Name.Substring(23, 2) + "/" + file_menu.Name.Substring(21, 2) + "/" + file_menu.Name.Substring(17, 4);
                _doc_id = _menu.CardDoc(CParam.TeremokId, _card_date); // �������� ��������

                // ������� ��� � �����
                CTransfer _transfer = new CTransfer();

                // � ����� ������� �� ���� ���������� Card                
                _dir_in = new DirectoryInfo(CParam.AppFolder + "\\Card\\");


       //�������� �������� �� �����
                List<FileInfo> cards_file_list=_dir_in.GetFiles().ToList();
                t_Doc cards_doc = new t_Doc().SelectFirst<t_Doc>("doc_id="+_doc_id);

                UpdateCardsToKkm(cards_file_list, cards_doc);


//#if(!DEB)
//                foreach (FileInfo _file in _dir_in.GetFiles())
//                {
//                    // �������� � �����

//                    if (m_kkm1_online)

//                        _transfer.CopyCardToKKM(1, _file.FullName, _file.Name, false);

//                    if (m_kkm2_online)

//                        _transfer.CopyCardToKKM(2, _file.FullName, _file.Name, false);
 
//                    if (m_kkm3_online)

//                        _transfer.CopyCardToKKM(3, _file.FullName, _file.Name, false);

//                    if (m_kkm4_online)

//                        _transfer.CopyCardToKKM(4, _file.FullName, _file.Name, false);

//                    if (m_kkm5_online)

//                        _transfer.CopyCardToKKM(5, _file.FullName, _file.Name, false);
//                }

//                 // ���� �������, ������ ������� �����������
//                CBData _data = new CBData();
//                _data.DocUpdateState(_doc_id, 14, " - ��������� � �����: ");

//                m_need_update_menu = true;
//#endif

            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }


        /// <summary>
        /// �������� ����� �� ������
        /// </summary>
        /// <param name="cards_file_list">������ ������</param>
        /// <param name="cards_doc">�������� ��������</param>
        public void UpdateCardsToKkm(List<FileInfo> cards_file_list, t_Doc cards_doc)
        {
            if (cards_doc == null)
            {
                Log("Cards_doc is null");
                WpfCustomMessageBox.Show("�� ���������� ������� �������� ����");
            }
            //����������� �������

            PassObject load_cards_on_kkms = (o) =>
            {
                //�������� ������ ��������� + �������� ����
                cards_doc.doc_desc = "���� ���������� �������� �� ������...."; cards_doc.Update(); StaticConstants.MainGridUpdate();


                //�������� ���� ��� ����
                List<string> kkmInList = RbClientGlobalStaticMethods.ReturnKKmInPathes();

                if (kkmInList == null)
                {
                    WpfCustomMessageBox.Show("��� ��������� ����");
                    return;
                }
                else
                {
                    kkmInList.ForEach(a =>
                    {
                        new CustomAction((b) =>         //�������� ����� ������-������
                        {
                            //�������� ������ ��������� + �������� ����

                            string path_to_kkkm = b.ToString();
                            cards_file_list.ForEach(c => c.CopyTo(Path.Combine(path_to_kkkm,
                                c.Name), true));
                            cards_doc.doc_desc += " ����� " + path_to_kkkm + " ���������!"; cards_doc.Update(); StaticConstants.MainGridUpdate();
                        }, a).Start();
                    });
                }
            };

            Action<object> loads_kkm_ended = (o) =>
            {
                cards_doc.doc_desc = cards_doc.doc_desc.Replace("���� ���������� �������� �� ������....", "���������� �������� ���������!");
                t_DocStatusRef dsr = new t_DocStatusRef().SelectFirst<t_DocStatusRef>("doctype_id=4 AND statustype_id=3");
                if (dsr != null)
                {
                    cards_doc.doc_status_id = dsr.docstatusref_id;
                }
                cards_doc.Update(); StaticConstants.MainGridUpdate();
                //�������� ���� �� ���� �����������
            };

            object State = null;
            load_cards_on_kkms.BeginInvoke(null, new AsyncCallback(loads_kkm_ended), State);

        }

        private string AttachZeroToDate(int c)
        {
            if (c < 10)
                return "0" + c.ToString();
            else
                return c.ToString();
        }

        /// <summary>
        /// �������� ������ ��� Emark
        /// </summary>
        /// <param name="file_menu"></param>
        public void DllLoad(FileInfo file_menu)
        {
            try
            {
                File.Move(file_menu.FullName, CParam.AppFolder + "\\" + file_menu);
                File.Delete(file_menu.FullName);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        public void UpdaterLoad(FileInfo file)
        {
            try
            {
                File.Copy(file.FullName, CParam.AppFolder + "\\" + file, true);
                File.Delete(file.FullName);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        public void HelpLoad(FileInfo file_menu)
        {
            DateTime _help_date;
            int _doc_id;

            try
            {
                // ������������� ������ � ����������� ����� Help
                if (!Directory.Exists(CParam.AppFolder + "\\Help"))
                    Directory.CreateDirectory(CParam.AppFolder + "\\Help");

                // ������ ������ Help
                DirectoryInfo _dir_in = new DirectoryInfo(CParam.AppFolder + "\\Help\\");
                foreach (FileInfo _file in _dir_in.GetFiles())
                {
                    File.Delete(_file.FullName);
                }

                using (ZipFile _zip = ZipFile.Read(file_menu.FullName))
                {
                    foreach (ZipEntry e in _zip)
                    {
                        FileInfo _fi = new FileInfo(e.FileName);
                        using (FileStream fs = new FileStream(CParam.AppFolder + "\\Help\\" + _fi.Name, FileMode.Create))
                        {
                            e.Extract(fs);
                        }
                    }
                }
                CBData _data = new CBData();
                _help_date = DateTime.Now.Date;
                _doc_id = _data.HelpDoc(CParam.TeremokId, _help_date); // �������� ��������
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        public void MailLoad(FileInfo file)
        {
            try
            {
                //������� ���� �� ����.
                //File.Copy(file.FullName, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\AddressBook\\" + file.Name, true);
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\AddressBook\\" + Environment.UserName + ".WAB");
                File.Move(file.FullName, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\AddressBook\\" + Environment.UserName + ".WAB");
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
                Log(exp.Message.ToString());
            }
        }

        public void EmarkUpdate(int doc_id)
        {
            int _count_updated_kkm = 0;
            int _f = 0;
            string buildNumber;

            try
            {
                CBData _dt = new CBData();
                //kkm_dep = _dt.GetDep_kkm(_conn, doc_id);
                buildNumber = _dt.GetDep_kkm(doc_id);
                {
                    CTransfer _transfer = new CTransfer();
                    DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder + "\\Emark\\" + buildNumber);
                    foreach (FileInfo _file in _dir.GetFiles())
                    {
                        if (_file.Name.EndsWith(".jpg"))
                        {
                            if (m_kkm1_online)
                            {
                                _transfer.CopyToEmark(1, _file.FullName, _file.Name, true);
                                _count_updated_kkm = 1;
                            }
                            if (m_kkm2_online)
                            {
                                _transfer.CopyToEmark(2, _file.FullName, _file.Name, true);
                                _count_updated_kkm = 2;
                            }
                            if (m_kkm3_online)
                            {
                                _transfer.CopyToEmark(3, _file.FullName, _file.Name, true);
                                _count_updated_kkm = 3;
                            }
                            if (m_kkm4_online)
                            {
                                _transfer.CopyToEmark(4, _file.FullName, _file.Name, true);
                                _count_updated_kkm = 4;
                            }
                            if (m_kkm5_online)
                            {
                                _transfer.CopyToEmark(5, _file.FullName, _file.Name, true);
                                _count_updated_kkm = 5;
                            }
                        }
                        if (_file.Name.EndsWith(".xml"))
                        {
                            if (m_kkm1_online)
                                _transfer.CopyToEmarkxml(1, _file.FullName, _file.Name, true);
                            if (m_kkm2_online)
                                _transfer.CopyToEmarkxml(2, _file.FullName, _file.Name, true);
                            if (m_kkm3_online)
                                _transfer.CopyToEmarkxml(3, _file.FullName, _file.Name, true);
                            if (m_kkm4_online)
                                _transfer.CopyToEmarkxml(4, _file.FullName, _file.Name, true);
                            if (m_kkm5_online)
                                _transfer.CopyToEmarkxml(5, _file.FullName, _file.Name, true);
                        }

                        if (_f == 0)
                            _f = _count_updated_kkm;
                    }
                    if (_count_updated_kkm == 0)
                    {
                        MessageBox.Show("����� �� �������� ��� ����������");
                        Log("����� �� �������� ��� ����������");
                        log.Info("����� �� �������� ��� ����������");
                    }
                    else
                    {
                        // ������ ������ ����. 
                        CBData _data = new CBData();
                        _data.DocUpdateState(doc_id, 79, "��������� � ��� ������ " + buildNumber + " ");
                        MessageBox.Show("��������� ���� �� " + _f.ToString() + " ������(�)");
                        Log("��������� ���� �� " + _f.ToString() + " ������(�)");
                        log.Info("��������� ���� �� " + _f.ToString() + " ������(�)");
                    }
                }
            }

            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }


        private void CopyFilesUpdatedMenu(int menuNum, FileInfo[] Files, CTransfer _transfer, string kkm_dep)
        {
            foreach (FileInfo _file in Files)
            {
                Servicelog("���� ������� ���� " + _file.Name + " � ����� " + menuNum);
                _transfer.CopyToKKM(menuNum, _file.FullName, _file.Name, kkm_dep, false);
            }
        }

        public void Servicelog(string message)
        {
            Log(message);
            log.Info(message);
        }


        class Kashevar
        {
            public FileInfo filename;
            public int index;
            public string destinationPath;
        }

        private void updateKashevarFilter(DirectoryInfo dir)
        {
            //����������� ����� � ����� �����������
            try
            {
                //����� ������ ���������
                object kash_config_path = CellHelper.GetConfValue("KashevarConfig");

                //��������� ������ ���������
                using (ConfigClass kash_config = new ConfigClass(kash_config_path.ToString()))
                {
                    #region ��������� ������

                    object regex_o=kash_config.GetProperty("folder_kkm_videoName_Mask");
                    Regex reg = new Regex(regex_o.ToString());

                    List<Kashevar> kashevar_lst = new List<Kashevar>();
                    
                    //������� ������ ������ � ���������� ������ ���������
                    dir.GetFiles().ToList<FileInfo>().ForEach(a =>
                    {
                        bool flag=reg.IsMatch(a.Name);
                        if(flag){
                            kashevar_lst.Add(new Kashevar() { filename = a, index = int.Parse(reg.Match(a.Name).Groups[1].Value)});
                        }
                    });
                    

                    //�������� ��� ��������
                    Dictionary<string,object> prop_dict=kash_config.GetAllProperties();

                    kashevar_lst.ForEach(a => 
                    {
                        string key="folder_blinopek" + a.index;
                        if (prop_dict.Keys.Contains(key))
                        {
                            a.destinationPath = prop_dict[key].ToString();
                        }
                    });

                    #endregion

                    #region ����������� ������



                    

                    ThreadCopyClass.Reset();

                    //ThreadCopyClass.ThreadCopy(kashevar_lst[0].filename, kashevar_lst[0].destinationPath);

                    kashevar_lst.ForEach(a =>
                    {
                        ThreadCopyClass.ThreadCopy(a.filename, a.destinationPath);
                    });

                    ThreadCopyClass.Disposed();

                    #endregion

                    //kkm_folders_lst.ForEach(a =>
                    //    {
                    //        ThreadCopyClass.ThreadCopy(posDisplays_fi, a);
                    //    });
                }
            }
            catch (Exception exp)
            {
                Log(exp, "�� ������� �������� ������� ��� ��������� ����������");
            }
        }

        public void UpdateMenu(int doc_id)
        {
            int _count_updated_kkm = 0;
            int _f = 0;
            string kkm_dep;

            try
            {
                CBData _dt = new CBData();
                kkm_dep = _dt.GetDep_kkm(doc_id);
                {
                    CTransfer _transfer = new CTransfer();
                    DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder + "\\Menu\\" + kkm_dep + "\\" + doc_id.ToString());
                    
                    //������������� ���������� ������?
                    //����� ����� � ���������� ���������
#if(KASHEVAR)
                    updateKashevarFilter(_dir);
#endif


                    FileInfo[] Files = SplitFilesFromMenu(_dir.GetFiles());

                        // �������� � �����
                        if (m_kkm1_online)
                        {
                            Servicelog("������� ���������� ���� ����� 1");
                            CopyFilesUpdatedMenu(1, Files, _transfer, kkm_dep);
                            _count_updated_kkm++;
                            Servicelog("���� ��������� � ����� 1");
                            
                        }
                        if (m_kkm2_online)
                        {
                            Servicelog("������� ���������� ���� ����� 2");
                            CopyFilesUpdatedMenu(2, Files, _transfer, kkm_dep);
                            _count_updated_kkm++;
                            Servicelog("���� ��������� � ����� 2");
                        }
                        if (m_kkm3_online)
                        {
                            Servicelog("������� ���������� ���� ����� 3");
                            CopyFilesUpdatedMenu(3, Files, _transfer, kkm_dep);
                            _count_updated_kkm++;
                            Servicelog("���� ��������� � ����� 3");
                        }
                        if (m_kkm4_online)
                        {
                            Servicelog("������� ���������� ���� ����� 4");
                            CopyFilesUpdatedMenu(4, Files, _transfer, kkm_dep);
                            _count_updated_kkm++;
                            Servicelog("���� ��������� � ����� 4");
                        }
                        if (m_kkm5_online)
                        {
                            Servicelog("������� ���������� ���� ����� 5");
                            CopyFilesUpdatedMenu(5, Files, _transfer, kkm_dep);
                            _count_updated_kkm++;
                            Servicelog("���� ��������� � ����� 5");
                        }

                        if (_f == 0)
                            _f = _count_updated_kkm;
                    
                    if (_count_updated_kkm == 0)
                    {
                        MessageBox.Show("����� �� �������� ��� ����������");
                        Log("����� �� �������� ��� ����������");
                        log.Info("����� �� �������� ��� ����������");
                    }
                    else
                    {
                        // ������ ������ ����. 
                        _dt.DocUpdateState(doc_id, 15, "��������� � ��� ");
                        MessageBox.Show("��������� ���� �� " + _f.ToString() + " ������(�)");
                        Log("��������� ���� �� " + _f.ToString() + " ������(�)");
                        log.Info("��������� ���� �� " + _f.ToString() + " ������(�)");
                    }
                }
            }

            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        private FileInfo[] SplitFilesFromMenu(FileInfo[] Files)
        {
            string lunches_txt = StaticConstants.RBINNER_CONFIG.GetProperty<string>("lunches_menu_file", "lunches.txt");
            var temp =Files.ToList();
            temp.RemoveAll(a => a.Name.Equals(lunches_txt, StringComparison.OrdinalIgnoreCase));
            return temp.ToArray();
        }

        public void WorkerRun(BackgroundWorker bgw)
        {
            if (IsWorkerEnabled(bgw))
            {
                bgw.RunWorkerAsync();
            }
        }

        public bool IsWorkerEnabled(BackgroundWorker bgw)
        {

            //if (!CheckAndExecuteFromTaskQueue())
            //    return false;

            if(bgw.Equals(backgroundWorker_Inbox))
            {
                if (!(backgroundWorker_Inbox.IsBusy))// && !(backgroundWorker_CheckKKM.IsBusy))
                    return true;
                else
                {
                    log.Warn("backgroundWorker_Inbox ����� � �� ����� ���������� ������");
                    return false;
                }
            }

            if (bgw.Equals(backgroundWorker_TReport))
            {
                if (backgroundWorker_TReport.IsBusy)
                {
                    log.Warn("backgroundWorker_TReport ����� � �� ����� ���������� ������");
                    return false;
                }
                else
                    return true;
            }

            if (bgw.Equals(backgroundWorker_CheckKKM))
            {
                //if (!(backgroundWorker_Inbox.IsBusy) && !(backgroundWorker_CheckKKM.IsBusy))
                if (!(backgroundWorker_ZReport.IsBusy) && !(backgroundWorker_CheckKKM.IsBusy))
                {
                    return true;
                }
                else
                {
                    log.Warn("backgroundWorker_CheckKKM ����� � �� ����� ���������� ������");
                    return false;
                }
            }

            if (bgw.Equals(backgroundWorker_Info))
            {
                if (!backgroundWorker_Info.IsBusy)
                {
                    return true;
                }
                else
                {
                    return false;
                    log.Warn("backgroundWorker_Info ����� � �� ����� ���������� ������");
                }
            }

            if (bgw.Equals(backgroundWorker_ZReport))
            {
                if (!backgroundWorker_ZReport.IsBusy)
                {
                    return true;
                }
                else
                {
                    log.Warn("backgroundWorker_ZReport ����� � �� ����� ���������� ������");
                    return false;
                }
            }
            
            return false;
        }

        

       

        // ������
        public void timer_CheckInbox_Tick(object sender, EventArgs e)
        {
            // ��������� ������
            try
            {
                // ��������� � ������ �����
                //WorkerRun(backgroundWorker_Inbox);

                AsyncWorker_Inbox.Run();
                
            }
            catch (InvalidOperationException exp)
            {
                // �������� �����, ����������                
                Log("!!! ������ m_working=" + m_thread_working.ToString());
                log.Info("!!! ������ m_working=" + m_thread_working.ToString());
                log.Error("Exception: " + exp.Message, exp);
            }
            catch (Exception exp)
            {
                Log("!!! ������ m_working=" + m_thread_working.ToString() + " " + exp.Message);
                log.Info("!!! ������ m_working=" + m_thread_working.ToString() + " " + exp.Message);
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        public void VideoLoad(FileInfo file)
        {
            try
            {
                CBData _data = new CBData();
                // ������������� ������ � ����������� ����� Menu
                if (!Directory.Exists(CParam.AppFolder + "\\Temp\\"))
                    Directory.CreateDirectory(CParam.AppFolder + "\\Temp\\");

                using (ZipFile _zip = ZipFile.Read(file.FullName, Encoding.GetEncoding("cp866")))
                {
                    foreach (ZipEntry e in _zip)
                    {
                        FileInfo _fi = new FileInfo(e.FileName);
                        using (FileStream fs = new FileStream(CParam.AppFolder + "\\Temp\\" + _fi.Name, FileMode.OpenOrCreate))
                        {
                            e.Extract(fs);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        public void WalkDirTree(DirectoryInfo root)
        {
            ArrayList _array = new ArrayList();

            try
            {
                _array.Add(root);
                foreach (var dirInfo in root.GetDirectories())
                    WalkDirTree(dirInfo);
                DirectoryInfo _dir_sf = new DirectoryInfo(root.FullName);
                foreach (FileInfo _file in _dir_sf.GetFiles())
                {
                    if (_file.Name.ToLower().EndsWith("avi"))
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\video\\"))
                        {
                            Directory.CreateDirectory(CParam.AppFolder + "\\video\\");
                        }
                        File.Copy(_file.FullName, CParam.AppFolder + "\\video\\" + _file.Name, true);
                    }
                    if (_file.Name.ToLower().EndsWith("bmp") || _file.Name.ToLower().EndsWith("png"))
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\img\\"))
                        {
                            Directory.CreateDirectory(CParam.AppFolder + "\\img\\");
                        }
                        File.Copy(_file.FullName, CParam.AppFolder + "\\img\\" + _file.Name, true);
                    }
                    if (_file.Name.ToLower().EndsWith("mht"))
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\Help\\"))
                        {
                            Directory.CreateDirectory(CParam.AppFolder + "\\Help\\");
                        }
                        File.Copy(_file.FullName, CParam.AppFolder + "\\Help\\" + _file.Name, true);
                    }
                    if (_file.Name.ToLower().EndsWith("dll"))
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\"))
                        {
                            Directory.CreateDirectory(CParam.AppFolder + "\\");
                        }
                        File.Copy(_file.FullName, CParam.AppFolder + "\\" + _file.Name, true);
                    }
                    if (!_file.Name.ToLower().EndsWith("dll") || !_file.Name.ToLower().EndsWith("mht") || !_file.Name.ToLower().EndsWith("bmp") || !_file.Name.ToLower().EndsWith("png") || !_file.Name.ToLower().EndsWith("avi")) // ���� ��� ������� � ����. 
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\Inbox\\"))
                        {
                            Directory.CreateDirectory(CParam.AppFolder + "\\Inbox\\");
                        }
                        File.Copy(_file.FullName, CParam.AppFolder + "\\Inbox\\" + _file.Name, true);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                throw (exp);
            }
        }

        private void Web_GetDepartmentSchedule(ARMWeb systemService, CBData _data)
        {
            try
            {
                {
                    //�������� ������ ������ ����������.
                    log.Debug("�������� ������ ������ ����������.");
                    DateTime _dt = Convert.ToDateTime(DateTime.Today.Year + "-" + AttachZeroToDate(DateTime.Today.Month) + "-01 0:00:00");
                    Schedule[] ptr = systemService.GetDepartmentSchedule(Convert.ToInt32(CParam.TeremokId), _dt);
                    if (ptr.Count() != 0) SqlWorker.ExecuteQuerySafe("DELETE FROM t_WorkTeremok;");
                    int day = 0;
                    log.Debug("ptr.Length=" + ptr.Length);
                    foreach (var info in ptr)
                    {
                        day++;
                        _data.ImportWorkTeremok(Convert.ToDateTime(info.EndTime), Convert.ToDateTime(info.StartTime), CParam.TeremokId, DateTime.Today.Month, DateTime.Today.Year, day);
                    }
                }
            }
            catch (Exception exp)
            {
                Log("������ � ������� GetDepartmentSchedule");
                log.Info("������ � ������� GetDepartmentSchedule");
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        private void Web_GetResponsibility(ARMWeb systemService, CBData _data,ref int rec)
        {
            try
            {
                //�������� ������ ������������.

                log.Debug("�������� ������ ������������.");
                OleDbConnection _conn = null;
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                
                rec = 0;

                Responsibility[] _res = systemService.GetResponsibilities(Convert.ToInt32(CParam.TeremokId), ref rec);
                if (_res.Count() > 0) _data.ClearRespon();
                log.Debug("_res.Length=" + _res.Length);
                foreach (var info in _res)
                {
                    log.Debug("ImportResponsibility(" + info.Guid + ", " + info.Name + ")");
                    _data.ImportResponsibility(info.Guid.ToString(), info.Name.ToString());
                }
                Responsibility[] _resrec = systemService.GetResponsibilities(Convert.ToInt32(CParam.TeremokId), ref rec);

            }
            catch (Exception exp)
            {
                Log("������ � ������� GetResponsibility");
                log.Info("������ � ������� GetResponsibility");
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        private void Web_GetShiftTypes(ARMWeb systemService, CBData _data, ref int rec)
        {
            try
            {
                //�������� ������ ������������.

                OleDbConnection _conn = null;
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                //_data.ClearRespon();
                rec = 0;

                ShiftType[] _st = systemService.GetShiftTypes(Convert.ToInt32(CParam.TeremokId), ref rec);
                if (_st.Count() > 0) SqlWorker.ExecuteQuerySafe("DELETE FROM t_ShiftType;");
                log.Debug("_st.Length=" + _st.Length);
                foreach (var info in _st)
                {
                    log.Debug("ImportShiftType(" + info.Guid + ", " + info.Name + ", " + info.Value + ", " + info.Color + ")");
                    _data.ImportShiftType(_conn, info.Guid.ToString(), info.Name.ToString(), info.Value.ToString(), info.Color.ToString());
                }
                ShiftType[] _str = systemService.GetShiftTypes(Convert.ToInt32(CParam.TeremokId), ref rec);

            }
            catch (Exception exp)
            {
                Log("������ � ������� GetShiftTypes");
                log.Info("������ � ������� GetShiftTypes");
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        private void Web_GetEmployee(ARMWeb systemService, CBData _data, ref int rec)
        {
            try
            {
                //"GetEmployee" //�������� ������ ����������.

                log.Debug("�������� ������ ����������.");
                OleDbConnection _conn = null;
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                rec = 0;
                
                Employee[] Employee = systemService.GetEmployees(Convert.ToInt32(CParam.TeremokId), ref rec);
                List<Employee> EmployeeLoad_list = new List<Employee>();


                List<Employee> emp = Employee.Where(a => a.Working == true).ToList();

                log.Debug("Employee.Length=" + emp.Count);
                foreach (var info in emp)
                {
                    if (StaticConstants.IsTabelOpened)
                    {
                        while (StaticConstants.IsTabelOpened)
                        {
                            System.Threading.Thread.Sleep(10000);
                        }
                    }

                    if (info.JobFunctionName == null)
                    {
                        info.JobFunctionName = " ";
                        info.JobFunctionGuid = " ";
                    }
                    log.Debug("ImportEmployee(" + info.Guid + ", " + info.Name + ", " 
                        + info.JobFunctionName + ", " + info.JobFunctionGuid + ", " + info.Working + ")");
                    _data.ImportEmployee(_conn, info.Guid.ToString(), info.Name.ToString(),
                        info.JobFunctionName.ToString(), info.JobFunctionGuid.ToString(), info.Working);
                    EmployeeLoad_list.Add(info);
                }

                Employee[] EmployeeLoad = EmployeeLoad_list.ToArray();


                _conn.Dispose();
                _conn.Close();

            }
            catch (Exception exp)
            {
                Log("������ � ������� GetEmployee");
                log.Info("������ � ������� GetEmployee");
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        private void Web_GetArrayShifts(ARMWeb systemService, CBData _data, ref int rec)
        {
            try
            {
                //"GetArrayShifts" //�������� ������ �������� ����.

                log.Debug("�������� ������ �������� ����.");
                // �������� ������ ����� �������.
                

                OleDbConnection _conn = null;
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                rec = 0;

                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//                PatternShifts[] ptr = systemService.GetArrayShifts(Convert.ToInt32(CParam.TeremokId), ref rec);
//#if(DEB)
//                //rec = 0;
//                //PatternShifts[] ptrptr = systemService.GetArrayShifts(149, ref rec);
//#endif
//                log.Debug("_ptr.Length=" + ptr.Length);
//                if (ptr.Count() > 1)
//                {
//                    log.Debug("������� ������� ����");    
//                    _data.ClearButtonTemplate();
//                    foreach (var info in ptr)
//                    {
//                        if (info.Time.ToString() != "0")
//                        {
//                            log.Debug("ImportButtonTemplate(" + info.Guid + ", " + info.Name + ", " + info.Value + " + " + info.Time + ", " + info.Color + ", " + info.Time + ", " + info.IsUsed + ", " + info.SmenaType + ", " + info.ButtonOrder + ")");
//                            _data.ImportButtonTemplate(_conn, info.Guid.ToString(), info.Name.ToString(), info.Value.ToString() + info.Time.ToString(), info.Color.ToString(), info.Time.ToString(), info.IsUsed, info.SmenaType.ToString(), info.ButtonOrder);
//                        }
//                        else
//                        {
//                            log.Debug("ImportButtonTemplate(" + info.Guid + ", " + info.Name + ", " + info.Value + ", " + info.Color + ", " + info.Time + ", " + info.IsUsed + ", " + info.SmenaType + ", " + info.ButtonOrder + ")");
//                            _data.ImportButtonTemplate(_conn, info.Guid.ToString(), info.Name.ToString(), info.Value.ToString(), info.Color.ToString(), info.Time.ToString(), info.IsUsed, info.SmenaType.ToString(), info.ButtonOrder);
//                        }
//                    }
//                    PatternShifts[] ptrLoad = systemService.GetArrayShifts(Convert.ToInt32(CParam.TeremokId), ref rec);
//                }
                _conn.Dispose();
                _conn.Close();

            }
            catch (Exception exp)
            {
                Log("������ � ������� GetArrayShifts");
                log.Info("������ � ������� GetArrayShifts");
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        private void Web_GetDepatments(ARMWeb systemService, CBData _data, ref int rec)
        {
            try
            {
                //"GetDepatments" //�������� ������ �������������.

                log.Debug("�������� ������ �������������.");

               // t_PropValue code = new t_PropValue().SelectFirst<t_PropValue>("Prop_name='"++"'");

                OleDbConnection _conn = null;
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                rec = 0;

                Department[] dep = systemService.GetDepartments(Convert.ToInt32(CParam.TeremokId), ref rec);
                log.Debug("dep.Length=" + dep.Length);



                foreach (var info in dep)
                {
                    log.Debug("ImportDepart(" + info.Id + ", " + info.Code + ", " + info.Name + ", " + info.Type + ", " + info.Guid + ")");
                    _data.ImportDepart(_conn, info.Id.ToString(), info.Code, info.Name, info.Type, info.Guid);
                }
                Department[] depLoad = systemService.GetDepartments(Convert.ToInt32(CParam.TeremokId), ref rec);

                _conn.Dispose();
                _conn.Close();

            }
            catch (Exception exp)
            {
                Log("������ � ������� GetDepatments");
                log.Info("������ � ������� GetDepatments");
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        /// <summary>
        /// ��������� ����������� ��� �� ���������� �������� ��� ����������
        /// </summary>
        /// <param name="webservice">���������</param>
        /// <param name="web_func_name">��� �������</param>
        /// <param name="param_s">���������</param>
        /// <param name="UpdationMethod">����� ���������� � ������������</param>
        private void Web_UpdateSpravValuesWithOutCode(ARMWeb webservice, string web_func_name, object[] param_s, Action<object> UpdationMethod)// where T : ModelClass, new()
        {
            try
            {
                log.Debug("�������� ������ " + web_func_name);
                object results = StaticHelperClass.CallMethod<ARMWeb>(webservice, web_func_name, param_s);
                if (results != null)
                {
                    UpdationMethod(results);
                }
            }
            catch (Exception exp)
            {
                Log(exp, "������ � ������� " + web_func_name);
            }
        }

        /// <summary>
        /// ������� ��������� ����������� ���������� �� ���� �� ����������
        /// </summary>
        /// <typeparam name="T">��� ������� �����������</typeparam>
        /// <param name="webservice"></param>
        /// <param name="web_func_name"></param>
        /// <param name="param_s"></param>
        /// <param name="UpdationMethod"></param>
        private void Web_UpdateSpravValues(ARMWeb webservice,string web_func_name,object[] param_s,Action<object> UpdationMethod)// where T : ModelClass, new()
        {
            try
            {
                log.Debug("�������� ������ "+web_func_name);
                t_PropValue code = new t_PropValue().SelectFirst<t_PropValue>("Prop_name='" + web_func_name + "'");
                if (code == null)
                {
                    code = new t_PropValue();
                    code.prop_name = web_func_name;
                    code.prop_value = "0";
                    code.Create();
                }

                if (param_s[1] == null)
                {
                    int code_int = int.Parse(code.prop_value); param_s[1] = code_int;
                }


                //object results = StaticHelperClass.CallMethod<ARMWeb>(webservice, web_func_name, new object[2]{164, 0});// param_s);
                object results = StaticHelperClass.CallMethod<ARMWeb>(webservice, web_func_name, param_s);

                if (WebIsNotNull(results))
                {
                    UpdationMethod(results);
                    code.prop_value = ((int)param_s[1]).ToString();
                    code.Update();
                }
            }
            catch (Exception exp)
            {
                Log(exp,"������ � ������� " + web_func_name);
            }
        }

             

        private void Web_UpdateSpravValues_WithConfirm(ARMWeb webservice, string web_func_name,string web_confirm_name, object[] param_s,
            Action<object> UpdationMethod)// where T : ModelClass, new()
        {
            try
            {
                log.Debug("�������� ������ " + web_func_name);
                t_PropValue code = new t_PropValue().SelectFirst<t_PropValue>("Prop_name='" + web_func_name + "'");
                if (code == null)
                {
                    code = new t_PropValue();
                    code.prop_name = web_func_name;
                    code.prop_value = "0";
                    code.Create();
                }

                if (param_s[1] == null)
                {
                    int code_int = int.Parse(code.prop_value); param_s[1] = code_int;
                }
                if(param_s[1].Equals(0))
                {
                    code.prop_value = "0";
                    code.Update();
                }

                log.Debug(web_func_name + " code= " + code.prop_value);

                object results = StaticHelperClass.CallMethod<ARMWeb>(webservice, web_func_name, param_s);
                Log(String.Format("webservice func name is:{0} value is:{1} ",web_func_name,Serializer.JsonSerialize(results)));

                if (WebIsNotNull(results))
                {
                    log.Debug(web_func_name + " results is not null starting updationmethod");
                    UpdationMethod(results);
                    code.prop_value = ((int)param_s[1]).ToString();
                    Web_ConfirmExchangePerformed(webservice, web_confirm_name, param_s, code);
                }
                else
                {
                    log.Debug(web_func_name + " results is null results:"+Serializer.JsonSerialize(results));
                }
            }
            catch (Exception exp)
            {
                Log(exp, "������ � ������� " + web_func_name);
            }
        }

        private void Web_ConfirmExchangePerformed(ARMWeb webservice, string web_func_name, object[] param_s, t_PropValue code)
        {
            try
            {
                code.Update();
                object results = StaticHelperClass.CallMethod<ARMWeb>(webservice, web_func_name, param_s);
            }
            catch (Exception ex)
            {
                Log(ex, "�� ������� �������� �� �������� ���������� " + web_func_name);
            }
        }

        public void thread_Sleep_Recursive(Action<object> _threadMethod,object _object,int timeout)
        {
            System.Threading.Thread.Sleep(timeout);
            _threadMethod(_object);
        }


        private void Web_GetEmployee_threadMethod(object arr_list1)
        {
            try{
                if (StaticConstants.IsTabelOpened)
                {
                    thread_Sleep_Recursive(Web_GetEmployee_threadMethod,arr_list1,5000);
                    return;
                }
                ArrayList arr_list = arr_list1 as ArrayList;
                int rec=0;
         
                Web_GetEmployee((ARMWeb)arr_list[0], (CBData)arr_list[1], ref rec); //�������� ������ �����������.
                log.Info("WebSrv Web_GetEmployee Complete");
            }catch(Exception exp)
            {
                log.Error("WebSrv Web_GetEmployee parse error. Exception: " + exp, exp);
            }
        }

        public static bool CustomCertificateValidatior(object sender,
    X509Certificate certificate, X509Chain chain,
    SslPolicyErrors policyErrors)
        {
            // anything goes!
            return true;

            // PS: you could put your own validation logic here, 
            // through accessing the certificate properties:
            // var publicKey = certificate.GetPublicKey();

        }


        private void startWebServiceExchange()
        {
            startWebServiceExchange(null);
            return;
            StaticConstants.IsSyncProcess = true;

            CBData _data = new CBData();

            ARMWeb systemService = StaticConstants.WebService; if (systemService == null) return;
#if(!DEB)
            Label1.Text = "����������� � �������";
#endif
            log.Debug("����������� � ������� (" + systemService.Url + ")");
            

            try
            {                
#if(!DEB)
                Label1.Text = "������� ������ �� �������";
#endif

                string[] push = systemService.Pull(Convert.ToInt32(CParam.TeremokId));

                log.Debug("push.Length=" + push.Length);

                
#if(DEB)
                //int rec = 0;
                //object docs=systemService.GetDocsSD(103, ref rec);
#endif
                
                MainProgressReport.Instance.ReportProgress("�������� ���������� ������������ �� WebService", 0);
                List<string> pushList = push.ToList();

                foreach (var item in push)
                {
                    log.Debug("item.Name=" + item);

                    if (item == "GetTrainingVideoSD")
                    {
                        #region GetTrainingVideoSD
                        MainProgressReport.Instance.ReportProgress("��������� ��������� ���������� �����", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues_WithConfirm(systemService, "GetTrainingVideoSD", "ConfirmTrainingVideoSD", new object[] { Convert.ToInt32(CParam.TeremokId), null },
                         o =>
                         {
                             if (UpdateClass.UpdationList.Where(a => a.UpdationType.Equals(o.GetType())).Count() == 0)
                             {
                                 UpdateClass updcls = new UpdateClass() { LogEvent = Log, UpdationType = o.GetType() };
                                 Log("�������� ���������� ��������� ���������� �����");
                                 UpdateClass.UpdationList.Add(updcls);
                                 updcls.����������������������(o);
                             }
                             else
                             {
                                 Log("���������� ���������� ����� �� ���������! ��� ��� ����!");
                             }
                         });
                        #endregion
                    }

                    //systemService.GetShiftPatterns
                    if (item == "GetShiftTypes")
                    {
                        #region GetShiftTypes
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"���� ����\"", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues(systemService, "GetShiftTypes", new object[] { Convert.ToInt32(CParam.TeremokId), null },
                         (o) =>  //����� ���������� �����������
                         {
                             if (o is ShiftType[])
                             {
                                 IEnumerable<ShiftType> __list = (o as ShiftType[]);
                                 Log("WebService " + item + " list.Count" + __list.Count());
                                 IEnumerable<ModelClass> _list = __list
                                     .Select<ShiftType, ModelClass>((a, i) =>
                                     RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                 new t_ShiftType().UpdateSpravAddOrUpdate<t_ShiftType>(_list.OfType<t_ShiftType>().ToList(),
                                     b => b.type_guid, b => b.type_guid + b.type_name + b.type_value + b.type_color+b.deleted);
                             }
                         });
                        #endregion
                    }
                    if (item == "GetEmployees")
                    {
                        #region GetEmployees
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"����������\"", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues(systemService, "GetEmployees", new object[] { Convert.ToInt32(CParam.TeremokId), null },
                           (o) =>  //����� ���������� �����������
                           {
                               if (o is Employee[])
                               {
                                   IEnumerable<Employee> __list = (o as Employee[]);
                                   Log("WebService " + item + " list.Count" + __list.Count());
                                   IEnumerable<ModelClass> _list = __list
                                       .Select<Employee, ModelClass>((a, i) =>
                                       RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                   new t_Employee().UpdateSpravAddOrUpdate<t_Employee>(_list.OfType<t_Employee>().ToList(),
                                        b => b.employee_1C, b => b.employee_1C+ b.employee_name + b.employee_FunctionName
                                       + b.employee_FunctionGuid + b.employee_WorkField+b.deleted);
                               }
                           });
                        #endregion
                    }

                    if (item == "GetDepartments")
                    {
                        #region GetDepartments
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"�������\"", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues(systemService, "GetDepartments", new object[] { Convert.ToInt32(CParam.TeremokId), null },
                           (o) =>  //����� ���������� �����������
                           {                               
                               if (o is Department[])
                               {
                                   IEnumerable<Department> __list = (o as Department[]);
                                   Log("WebService " + item + " list.Count" + __list.Count());
                                   IEnumerable<ModelClass> _list = __list
                                       .Select<Department, ModelClass>((a, i) =>
                                       RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                   //��������� ������� �������
                                   List<int> terem_list = new t_Teremok().Select<t_Teremok>("teremok_current=-1").Select(a=>a.teremok_id)
                                       .OfType<int>().ToList();

                                   new t_Teremok().UpdateSpravAddOrUpdate<t_Teremok>(_list.OfType<t_Teremok>().ToList(),
                                        b => b.teremok_id.ToString(), b => b.teremok_id.ToString() + b.teremok_1C.ToString() + b.teremok_name+b.deleted);

                                   //��������� ������� �������
                                   if (terem_list.NotNullOrEmpty())
                                   {
                                       terem_list.ForEach(a =>
                                       {
                                           t_Teremok ter = new t_Teremok().SelectFirst<t_Teremok>("teremok_id="+a);
                                           if (ter != null)
                                           {
                                               ter.teremok_current = true;
                                               ter.Update();
                                           }
                                       });
                                   }

                               }
                           });

                        #endregion
                    }

                    if (item == "GetDepartmentSchedule")
                    {
                        #region  GetDepartmentSchedule
                        DateTime dt = Convert.ToDateTime(DateTime.Today.Year + "-" + AttachZeroToDate(DateTime.Today.Month) + "-01 0:00:00");
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"���������� �����\"", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValuesWithOutCode(systemService, "GetDepartmentSchedule",
                           new object[] { Convert.ToInt32(CParam.TeremokId), dt },
                                 (o) =>  //����� ���������� �����������
                                 {
                                     //systemService.GetDepartments
                                     if (o is Schedule[])
                                     {
                                         List<t_WorkTeremok> _list =
                                             (o as Schedule[]).ToList().Select<Schedule, t_WorkTeremok>((a, i) =>
                                             (t_WorkTeremok)RbClientGlobalStaticMethods.ConvertFromWeb(a)).ToList();
                                         Log("WebService " + item + " list.Count" + _list.Count());
                                         new t_WorkTeremok().UpdateSpravAddOrUpdate<t_WorkTeremok>(_list, 
                                             b => b.teremok_id + b.teremok_day + b.teremok_month + b.teremok_year,
                                             b => b.getHashCode(new string[] { "id" }));
                                     }
                                 });
                        #endregion
                    }

                    if (item == "GetShiftPatterns")
                    {
                        #region getshiftpatterns
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"���� ������\"", (100 / push.Length) * pushList.IndexOf(item));
                            Web_UpdateSpravValues(systemService, "GetShiftPatterns", new object[] { Convert.ToInt32(CParam.TeremokId), null },
                            (o) =>  //����� ���������� �����������
                            {
                                //systemService.GetDepartments
                                if (o is ShiftPattern[])
                                {
                                    IEnumerable<ShiftPattern> __list = (o as ShiftPattern[]);
                                    Log("WebService " + item + " list.Count" + __list.Count());
                                    IEnumerable<ModelClass> _list = __list
                                        .Select<ShiftPattern, ModelClass>((a, i) =>
                                        RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                    new t_ButtonTemplate().UpdateSpravAddOrUpdate<t_ButtonTemplate>(_list.OfType<t_ButtonTemplate>().ToList(),
                                         b => b.btn_guid, b => b.getHashCode(new string[] { "id" }));
                                }
                            });
                        #endregion
                    }

                    if (item == "GetResponsibilities")
                    {
                        #region GetResponsibilities
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"�����������\"", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues(systemService, "GetResponsibilities", new object[] { Convert.ToInt32(CParam.TeremokId), null },
                          (o) =>  //����� ���������� �����������
                          {
                              if (o is Responsibility[])
                              {
                                  IEnumerable<Responsibility> __list = (o as Responsibility[]);
                                  Log("WebService " + item + " list.Count" + __list.Count());
                                  IEnumerable<ModelClass> _list = __list
                                      .Select<Responsibility, ModelClass>((a, i) =>
                                      RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                  new t_Responsibility().UpdateSpravAddOrUpdate<t_Responsibility>(_list.OfType<t_Responsibility>().ToList(),
                                       b => b.res_guid, b => b.res_guid + b.res_name+b.deleted);

                                  //IEnumerable<Responsibility> __list = (o as Responsibility[]);

                                  //IEnumerable<ModelClass> _list = __list.Where(a => !a.Deleted)
                                  //    .Select<Responsibility, ModelClass>((a, i) =>
                                  //    RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                  //new t_Responsibility().UpdateSpravAddOrUpdate<t_Responsibility>(_list.OfType<t_Responsibility>().ToList(),
                                  //     b => b.res_guid, b => b.res_guid + b.res_name + b.deleted);

                                  //__list.Where(a => a.Deleted).Select<Responsibility, ModelClass>((a, i) =>
                                  //    RbClientGlobalStaticMethods.ConvertFromWeb(a)).ToList().ForEach(a => a.Delete());
                              }
                          });
                        #endregion
                    }
                }
                MainProgressReport.Instance.ReportProgress("���������� ������������ ���������!", 100);
            }
            catch (Exception exp)
            {
                Log("!!! ��� ������ �� ��������");
                Log(exp.ToString());
                log.Info("!!! ��� ������ �� �������� (" + systemService.Url + ")");
                log.Error("Exception: " + exp.Message, exp);
            }
            finally
            {
                StaticConstants.IsSyncProcess = false;
            }
        }

        private void startWebServiceExchange(object code)
        {
            StaticConstants.IsSyncProcess = true;
            CBData _data = new CBData();
            ARMWeb systemService = StaticConstants.WebService; if (systemService == null) return;
#if(!DEB)
            Label1.Text = "����������� � �������";
#endif
            log.Debug("����������� � ������� (" + systemService.Url + ")");
            
            try
            {
#if(!DEB)
                Label1.Text = "������� ������ �� �������";
#endif


                string[] push=null;

#if(DEB)
                //push = systemService.Pull(303);
                //     int ii = 0;
                //var d = StaticConstants.WebService.GetResponsibilities(112, ref ii);
                push = new string[] { "GetTrainingVideoSD" };
            //    push = systemService.Pull(Convert.ToInt32(CParam.TeremokId));
                //      Docs dc = StaticConstants.WebService.GetPromotionalVideoSD(110, ref ii);
                //ii = 0;
                // Responsibility[] sh=StaticConstants.WebService.GetResponsibilities(112,ref ii);
#else
                

                if (code != null)
                {
                    push = new string[] {"GetPromotionalVideoSD","GetTrainingVideoSD","GetDocumentsSD", "GetShiftTypes", "GetEmployees", "GetDepartments",
                    "GetDepartmentSchedule","GetShiftPatterns","GetResponsibilities"};
                }else
                {
                    push=systemService.Pull(Convert.ToInt32(CParam.TeremokId));
                    log.Debug("push.Length=" + push.Length);
                }

                if (!push.NotNullOrEmpty())
                {
                    Log("push array is null!!!" + push.Length);
                    MainProgressReport.Instance.ReportProgress("����� �� WebService ��������!", 100);
                    throw new Exception("push array is null!!!");
                }

                MainProgressReport.Instance.ReportProgress("�������� ���������� ������������ �� WebService", 0);
#endif


                push = new string[] { "GetTrainingVideoSD" };



                List<string> pushList=push.ToList();

                Log("push is " + Serializer.JsonSerialize(push));

                foreach (var item in push)
                {
                    log.Debug("item.Name=" + item);

                    if (item == "GetPromotionalVideoSD")
                    {
                        #region GetPromotionalVideoSD
                        MainProgressReport.Instance.ReportProgress("��������� ��������� ��������� ����������",
                            (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues_WithConfirm(systemService, "GetPromotionalVideoSD", "ConfirmPromotionalVideoSD",
                            new object[] { Convert.ToInt32(CParam.TeremokId), code },
                         o =>
                         {
                             UpdateClass updcls=UpdateClass.CreateUpdateTaskStr("GetPromotionalVideoSD");

                             if (updcls!=null)
                             {
                                 Log("WebService �������� ���������� ��������� ��������� ����������");   
                                 updcls.��������������������������(o);
                                 updcls.Dispose();
                             }
                             else
                             {
                                 Log("���������� ��������� ���������� �� ���������! ��� ��� ����!");
                                 throw new Exception("���������� ��������� ���������� �� ���������! ��� ��� ����!");
                             }
                         });
                        #endregion
                    }

                    if (item == "GetDocumentsSD")
                    {
                        #region GetDocumentsSD
                        MainProgressReport.Instance.ReportProgress("��������� ��������� ���������� ����������",
                            (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues_WithConfirm(systemService, "GetDocumentsSD", "ConfirmDocumentsSD",
                            new object[] { Convert.ToInt32(CParam.TeremokId), code },
                         o =>
                         {
                             UpdateClass updcls = UpdateClass.CreateUpdateTaskStr("GetDocumentsSD");
                             if (updcls != null)
                             {
                                 Log("WebService �������� ���������� ��������� ���������� ����������");
                                 updcls.���������������������������(o);
                                 updcls.Dispose();
                             }
                             else
                             {
                                 Log("���������� ���������� ���������� �� ���������! ��� ��� ����!");
                                 throw new Exception("���������� ���������� ���������� �� ���������! ��� ��� ����!");
                             }
                         });
                        #endregion
                    }

                    if (item == "GetTrainingVideoSD")
                    {
                        #region GetTrainingVideoSD
                        MainProgressReport.Instance.ReportProgress("��������� ��������� ���������� �����",
                            (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues_WithConfirm(systemService, "GetTrainingVideoSD", "ConfirmTrainingVideoSD",
                            new object[] { Convert.ToInt32(CParam.TeremokId), code },
                         o =>
                         {
                             UpdateClass updcls = UpdateClass.CreateUpdateTaskStr("GetTrainingVideoSD");
                             if (updcls != null)
                             {
                                 Log("WebService �������� ���������� ��������� ���������� �����");
                                 updcls.����������������������(o);
                                 updcls.Dispose();
                             }
                             else
                             {
                                 Log("���������� ���������� ����� �� ���������! ��� ��� ����!");
                                 throw new Exception("���������� ���������� ����� �� ���������! ��� ��� ����!");
                             }
                         });
                        #endregion
                    }

                    if (item == "GetShiftTypes")
                    {
                        #region GetShiftTypes
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"���� ����\"", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues(systemService, "GetShiftTypes", new object[] { Convert.ToInt32(CParam.TeremokId), code },
                         (o) =>  //����� ���������� �����������
                         {
                             if (o is ShiftType[])
                             {
                                 IEnumerable<ShiftType> __list = (o as ShiftType[]);

                                 Log("WebService " + item + " list.Count" + __list.Count());

                                 IEnumerable<ModelClass> _list = __list
                                     .Select<ShiftType, ModelClass>((a, i) =>
                                     RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                 new t_ShiftType().UpdateSpravAddOrUpdate<t_ShiftType>(_list.OfType<t_ShiftType>().ToList(),
                                     b => b.type_guid, b => b.type_guid + b.type_name + b.type_value + b.type_color+b.deleted);
                             }
                         });
                        #endregion
                    }

                    if (item == "GetEmployees")
                    {
                        #region GetEmployees
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"����������\"", (100 / push.Length) * pushList.IndexOf(item));
                   
                        Web_UpdateSpravValues(systemService, "GetEmployees", new object[] { Convert.ToInt32(CParam.TeremokId), code },
                           (o) =>  //����� ���������� �����������
                           {
                               if (o is Employee[])
                               {
                                   IEnumerable<Employee> __list = (o as Employee[]);
                                   Log("WebService " + item + " list.Count" + __list.Count());
                                   IEnumerable<ModelClass> _list = __list
                                       .Select<Employee, ModelClass>((a, i) =>
                                       RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                   new t_Employee().UpdateSpravAddOrUpdate<t_Employee>(_list.OfType<t_Employee>().ToList(),
                                        b => b.employee_1C, b => b.employee_1C + b.employee_name + b.employee_FunctionName
                                       + b.employee_FunctionGuid + b.employee_WorkField+b.deleted);

                               }
                           });
                    #endregion
                    }
                    if (item == "GetDepartments")
                    {
                        #region GetDepartments
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"�������\"", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues(systemService, "GetDepartments", new object[] { Convert.ToInt32(CParam.TeremokId), code },
                           (o) =>  //����� ���������� �����������
                           {
                               if (o is Department[])
                               {
                                   IEnumerable<Department> __list = (o as Department[]);
                                   Log("WebService " + item + " list.Count" + __list.Count());
                                   IEnumerable<ModelClass> _list = __list
                                       .Select<Department, ModelClass>((a, i) =>
                                       RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                   //��������� ������� �������
                                   List<int> terem_list = new t_Teremok().Select<t_Teremok>("teremok_current=-1").Select(a => a.teremok_id)
                                       .OfType<int>().ToList();

                                   new t_Teremok().UpdateSpravAddOrUpdate<t_Teremok>(_list.OfType<t_Teremok>().ToList(),
                                        b => b.teremok_id.ToString(), b => b.teremok_id.ToString() + b.teremok_1C.ToString() + b.teremok_name+b.deleted);

                                   //��������� ������� �������
                                   if (terem_list.NotNullOrEmpty())
                                   {
                                       terem_list.ForEach(a =>
                                       {
                                           t_Teremok ter = new t_Teremok().SelectFirst<t_Teremok>("teremok_id=" + a);
                                           if (ter != null)
                                           {
                                               ter.teremok_current = true;
                                               ter.Update();
                                           }
                                       });
                                   }



                               }
                           });

                        #endregion
                    }

                    if (item == "GetDepartmentSchedule")
                    {
                        #region  GetDepartmentSchedule
                        DateTime dt = Convert.ToDateTime(DateTime.Today.Year + "-" + AttachZeroToDate(DateTime.Today.Month) + "-01 0:00:00");
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"���������� �����\"", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValuesWithOutCode(systemService, "GetDepartmentSchedule",
                           new object[] { Convert.ToInt32(CParam.TeremokId), dt },
                                 (o) =>  //����� ���������� �����������
                                 {
                                     //systemService.GetDepartments
                                     if (o is Schedule[])
                                     {
                                         List<t_WorkTeremok> _list =
                                             (o as Schedule[]).ToList().Select<Schedule, t_WorkTeremok>((a, i) =>
                                             (t_WorkTeremok)RbClientGlobalStaticMethods.ConvertFromWeb(a)).ToList();
                                         Log("WebService " + item + " list.Count" + _list.Count());
                                         new t_WorkTeremok().UpdateSpravAddOrUpdate<t_WorkTeremok>(_list,
                                             b => b.teremok_id + b.teremok_day + b.teremok_month + b.teremok_year,
                                             b => b.getHashCode(new string[] { "id" }));
                                     }

                                     //UpdateClass updcls=UpdateClass.CreateUpdateTask(o);
                                     //if (updcls == null) throw new Exception("");


                                 });
                        #endregion
                    }

                    if (item == "GetShiftPatterns")
                    {
                        #region getshiftpatterns
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"���� ������\"", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues(systemService, "GetShiftPatterns", new object[] { Convert.ToInt32(CParam.TeremokId), code },
                        (o) =>  //����� ���������� �����������
                        {
                            if (o is ShiftPattern[])
                            {
                                IEnumerable<ShiftPattern> __list = (o as ShiftPattern[]);
                                Log("WebService " + item + " list.Count" + __list.Count());
                                IEnumerable<ModelClass> _list = __list
                                    .Select<ShiftPattern, ModelClass>((a, i) =>
                                    RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                new t_ButtonTemplate().UpdateSpravAddOrUpdate<t_ButtonTemplate>(_list.OfType<t_ButtonTemplate>().ToList(),
                                     b => b.btn_guid, b => b.getHashCode(new string[] { "id" }));

                            }
                        });
                        #endregion
                    }

                    if (item == "GetResponsibilities")
                    {
                        #region GetResponsibilities
                        MainProgressReport.Instance.ReportProgress("��������� ���������� \"�����������\"", (100 / push.Length) * pushList.IndexOf(item));
                        Web_UpdateSpravValues(systemService, "GetResponsibilities", new object[] { Convert.ToInt32(CParam.TeremokId), code },
                          (o) =>  //����� ���������� �����������
                          {
                              if (o is Responsibility[])
                              {
                                  IEnumerable<Responsibility> __list = (o as Responsibility[]);
                                  Log("WebService " + item + " list.Count" + __list.Count());
                                  IEnumerable<ModelClass> _list = __list
                                      .Select<Responsibility, ModelClass>((a, i) =>
                                      RbClientGlobalStaticMethods.ConvertFromWeb(a));

                                  new t_Responsibility().UpdateSpravAddOrUpdate<t_Responsibility>(_list.OfType<t_Responsibility>().ToList(),
                                       b => b.res_guid, b => b.res_guid + b.res_name + b.deleted);

                                  
                                  List<ModelClass> _1=new List<ModelClass>();
                                  foreach (var a in __list.Where(oo => oo.Deleted != true))
                                  {
                                      if (a.AllowedShiftType.NotNullOrEmpty())
                                      {
                                          _1.AddRange(a.AllowedShiftType
                                         .Select<string, ModelClass>((b, i) => new t_Shifts_Allowed() { res_guid = a.Guid, shift_guid = b }));
                                      }
                                  }

                                  new t_Shifts_Allowed().UpdateSprav<t_Shifts_Allowed>(_1.OfType<t_Shifts_Allowed>().ToList()
                                      , b => b.res_guid + b.shift_guid,
                                      b => b.res_guid + b.shift_guid);


                                  //DeleteFromSlaveTable<t_Shifts_Allowed>(new t_Responsibility().Select<t_Responsibility>("deleted=-1")
                                  //    .OfType<ModelClass>(), oo => 
                                  //    String.Format("res_guid='{0}'",((t_Responsibility)oo).res_guid));

                                  //SyncWebDBLists<Responsibility, t_Shifts_Allowed>(
                                  //   __list, oo => oo.Deleted != true, a => String.Format("res_guid='{0}'", a.Guid),
                                  //   a => a.AllowedShiftType, a => a.shift_guid, a => new t_Shifts_Allowed() { res_guid = a.Guid },
                                  //   (t, a) =>
                                  //   {
                                  //       t.shift_guid = a.ToString();
                                  //       return t;
                                  //   });
                              }
                          });
                        #endregion
                    }
                }
                MainProgressReport.Instance.ReportProgress("���������� ������������ ���������!", 100);
            }
            catch (Exception exp)
            {
                Log("!!! ��� ������ �� ��������");
                Log(exp.ToString());
                log.Info("!!! ��� ������ �� �������� (" + systemService.Url + ")");
                log.Error("Exception: " + exp.Message, exp);

            }
            finally
            {
                StaticConstants.IsSyncProcess = false;
            }
        }


        //objetc - Responsibility
        //oo => oo.Deleted != true  deleted condition
        //T Responsibility
        //U t_Shifts_Allowed
        //ConvertFromWebCondition   String.Format("res_guid='{0}'", a.Guid)
        //Func<T,IEnumerable<object>> a.AllowedShiftType
        //Func<U,object> GetTableRowId b.shift_guid
        //CreateItemFromWeb - t_Shifts_Allowed tsh = new t_Shifts_Allowed() { res_guid = a.Guid, ;
        //UpdateItemFromWebListItem shift_guid = b }

        /// <summary>
        /// ������������� ������� � ����������� ������� � �����������
        /// ����������� ������� ���������� � ����� �� ������� �������� �� ������ �������� ���������
        /// </summary>
        /// <typeparam name="T">��� ������� ���������</typeparam>
        /// <typeparam name="U">��� ����������� �������</typeparam>
        /// <param name="WebList">������ ��� �������</param>
        /// <param name="FilterCondition">����������� �������</param>
        /// <param name="ConvertFromWebCondition">������� ��������� � ������������ �������� � ����</param>
        /// <param name="GetInnerWebList">������� ��������� ����������� ������</param>
        /// <param name="GetWebObjectFromTable">������� ��������� ������� ���������� �� ������� �������</param>
        /// <param name="CreateItemFromWeb">������� �������� ������ ������� �� ���-�������</param>
        /// <param name="UpdateItemFromWebListItem">������� ���������� ������ ������� �� ������ ��. ������ ���-�������</param>
        private static void SyncWebDBLists<T, U>(IEnumerable<T> WebList, Func<T, bool> FilterCondition,
            Func<T,string> ConvertFromWebCondition,Func<T,IEnumerable<object>> GetInnerWebList,
            Func<U,object> GetWebObjectFromTable,Func<T,U> CreateItemFromWeb,Func<U,object,U> UpdateItemFromWebListItem)
            where T:new()
            where U:ModelClass,new()
        {
            foreach (var a in WebList.Where(o=>FilterCondition(o)))
            {
                var lsshguid = new U().Select<U>(ConvertFromWebCondition(a));
                if (lsshguid.NotNullOrEmpty())
                {
                    foreach (var b in lsshguid)
                    {
                        if (!GetInnerWebList(a).Contains(GetWebObjectFromTable(b))) b.Delete();
                    }
                }

                if (GetInnerWebList(a).NotNullOrEmpty())
                {
                    foreach (var b in GetInnerWebList(a).Distinct())
                    {
                        if (lsshguid.NotNullOrEmpty())
                        {
                            var ls = lsshguid.Select(c => GetWebObjectFromTable(c));
                            if (ls.NotNullOrEmpty())
                            {
                                if (!ls.Contains(b))
                                {
                                    U tsh = CreateItemFromWeb(a);
                                    tsh = UpdateItemFromWebListItem(tsh,b);
                                    tsh.Create();

                                }
                            }
                            else
                            {

                                U tsh = CreateItemFromWeb(a);
                                tsh = UpdateItemFromWebListItem(tsh, b);
                                tsh.Create();
                            }
                        }
                        else
                        {

                            U tsh = CreateItemFromWeb(a);
                            tsh = UpdateItemFromWebListItem(tsh, b);
                            tsh.Create();
                        }
                    }
                }
            }
        }


        private static void DeleteFromSlaveTable<T>(IEnumerable<ModelClass> deleted_list,Func<ModelClass,string> condition)
            where T: ModelClass,new()
        {
            if (deleted_list.NotNullOrEmpty())
            {
                foreach (var a in deleted_list)
                {
                    var lsshguid = new T().Select<T>(condition(a));
                    if (lsshguid.NotNullOrEmpty())
                    {
                        foreach (var b in lsshguid)
                        {
                            b.Delete();
                        }
                    }
                }
            }
        }

        private bool WebIsNotNull(object o)
        {
            #region ShiftType
            if (o is ShiftType[])
            {
                if (((ShiftType[])o).NotNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion

            #region Employee
            if (o is Employee[])
            {
                if (((Employee[])o).NotNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion

            #region Department
            if (o is Department[])
            {
                if (((Department[])o).NotNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion

            #region Employee
            if (o is Employee[])
            {
                if (((Employee[])o).NotNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion

            #region Schedule
            if (o is Schedule[])
            {
                if (((Schedule[])o).NotNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion

            #region ShiftPattern
            if (o is ShiftPattern[])
            {
                if (((ShiftPattern[])o).NotNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion

            #region Schedule
            if (o is Responsibility[])
            {
                if (((Responsibility[])o).NotNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion

            #region Docs
            if (o is Docs)
            {
                if (((Docs)o).Type!=null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion

            if (o == null)
                return false;
            else
                return true;
            


        }   

        private void askWebService()
        {
#if(!DEB)
            if (StaticConstants.Web_Service_Last_Exchange_Date != null)
            {
                TimeSpan fromStarted_ts = DateTime.Now - (DateTime)StaticConstants.RbClient_Started_time;

                TimeSpan ts = DateTime.Now - (DateTime)StaticConstants.Web_Service_Last_Exchange_Date;

                if (fromStarted_ts < StaticConstants.Started_Service_Exchange_Interval)
                {
                    log.Info("���������� ���������� �� ����������,�.�. �� �������� �������� ����� �������");
                    return;
                }

                if (ts < StaticConstants.Web_Service_Exchange_Interval)
                {
                    log.Info("���������� ���������� �� ����������,�.�. �� �������� �������� ���������");
                    return;
                }
            }
            else
            {
                StaticConstants.Web_Service_Last_Exchange_Date = DateTime.Now;
            }
#endif

            if (StaticConstants.WebServiceExchangeThread == null)
            {
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(startWebServiceExchange));
                thread.IsBackground = true;
                thread.Name = "ThreadWebServiceExchange";
                StaticConstants.WebServiceExchangeThread = thread;
                thread.Start();
            }
            else
            {
                if (StaticConstants.WebServiceExchangeThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(startWebServiceExchange));
                    thread.IsBackground = true;
                    thread.Name = "ThreadWebServiceExchange";
                    StaticConstants.WebServiceExchangeThread = thread;
                    thread.Start();
                }
            }
        }

        
        private void backgroundWorker_Inbox_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.AutoResetEvent inbox_autoreset = new System.Threading.AutoResetEvent(false);
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                inbox_autoreset.Set();
                Inbox_exchange_method();
            })) { IsBackground = true };
            thread.Start();
            inbox_autoreset.WaitOne();
            while (thread.IsAlive)
            {
                System.Threading.Thread.Sleep(2000);
                if (backgroundWorker_Inbox.CancellationPending)
                {
                    if (thread.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        try
                        {
                            Log("backgroundWorker_Inbox_DoWork ���������� ����� backgroundWorker_Inbox_Thread");
                            thread.Abort();
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                    break;
                }
            }
        }

        private void ReportProgressBgw(BackgroundWorker bgw,int progress)
        {
            if (bgw != null)
            {
                bgw.ReportProgress(progress);
            }
        }

        private void Inbox_exchange_method()
        {
            log.Debug("Inbox_DoWork started");
            try
            {
                MakeUptimeFile();
                ReportProgressBgw(backgroundWorker_Inbox,25);
                m_thread_working = true;

                FTPServer(m_ip_chanel);

                if (!m_error)
                {
                    ReportProgressBgw(backgroundWorker_Inbox, 100);
                    Label1.Text = "����� ��������";
                    log.Debug("Inbox_DoWork ����� �������� ��� ������");
                }
                if (m_error)
                {
                    ReportProgressBgw(backgroundWorker_Inbox, 100);
                    Label1.Text = "���� ������";
                    log.Debug("Inbox_DoWork ���� ������ ftp");
                    //backgroundWorker_Inbox.CancelAsync();   
                }

                m_thread_working = false;

                //                #if(!DEB && WEBSRV)   
                log.Debug("Inbox_DoWork �������� ����� � �����������");

//#if(!DEB)
                askWebService();  // ���������� webservice � ������ ������ � ����
//#endif

                log.Debug("Inbox_DoWork ����� � ����������� ��������");
                // #endif    

            }
            catch (System.Threading.ThreadAbortException ex)
            {
                Label1.Text = "����� ��������";
                log.Debug("Inbox_DoWork ����� �������� ����� ������� ThreadAbortException");
            }
            catch (Exception exp)
            {
                Log("!!! ������ m_working=" + m_thread_working.ToString() + "������ " + exp.ToString());
                log.Info("!!! ������ m_working=" + m_thread_working.ToString() + "������ " + exp.ToString());
                log.Error("Exception: " + exp.Message, exp);
                Label1.Text = "���� ������";
            }
            finally
            {
                log.Debug("backgroundWorker_Inbox_DoWork finished");
                m_thread_working = false;
                StaticConstants.MainGridUpdate();
            }
        }





        private void  backgroundWorker_ZReport_DoWork(object sender, DoWorkEventArgs e)
        {
            log.Debug("ZReport_DoWork started");
            // �������� 
            try
            {
                m_working_zreport = true;
               
               //����� ���� �� z-����� ��� ��� ��������

                Log("ZReport_DoWork ��������� ����� ����� z-�������");
                //��������� ����� ����� z-�������
                DirectoryInfo z_back_dir=RbClientGlobalStaticMethods.GetDirectory(StaticConstants.Z_BACK_FOLDER);
                DirectoryInfo z_temp_dir = z_back_dir.GetDecendantDirectory(StaticConstants.TEMP_FOLDER);
                List<t_ZReport> z_list_sended=new t_ZReport().Select<t_ZReport>();

                IEnumerable<string> z_list_sended_str = z_list_sended.Select(a => a.z_file);

                Func<DirectoryInfo,IEnumerable<FileInfo>> get_missing_x_files=(wdir)=>
                {
                    IEnumerable<FileInfo> _back_z_files = wdir.GetFiles("X*");
                    IEnumerable<FileInfo> _back_z_files_missing = _back_z_files.Where(a => !z_list_sended_str.Contains(a.Name));
                    return _back_z_files_missing;
                };

                List<FileInfo> x_files_missing = new List<FileInfo>();

                ZreportMaker zmaker = new ZreportMaker();
                zmaker.LogEvent = MDIParentMain.Log;

                //��������� ����� ����
#if(!DEB)
                Log("ZReport_DoWork ��������� ����� ����");
                List<string> outPathes = RbClientGlobalStaticMethods.ReturnKKmOutPathes(false);
                if (outPathes == null && outPathes.Count==0)
                {
                    MDIParentMain.Log("ZReport_DoWork outPathes �����. ��� ���� � ����! Stop!");
                    return;
                }

                outPathes.ForEach(a =>
                {
                    Operate_Out_Z_Folder(z_back_dir, get_missing_x_files, zmaker, a);
                });
#endif

                x_files_missing.AddRange(get_missing_x_files(z_back_dir));

                t_Teremok teremok = new t_Teremok().SelectFirst<t_Teremok>("teremok_id=" + StaticConstants.Main_Teremok_ID);

                Log("ZReport_DoWork �������� ��������� z-�������");
                int doc_id=0;
                if (x_files_missing.Count > 0)
                {
                    CZReportHelper zreporthelper = new CZReportHelper();

                    DirectoryInfo dir_outbox = RbClientGlobalStaticMethods.GetDirectory(StaticConstants.OUTBOX_FOLDER);

                    x_files_missing.ForEach(a=>{
                        Log("ZReport_DoWork ������������ ���� "+a.Name);
                        new CustomAction(o_o =>
                            {
                                try
                                {
                                    string num_name = a.Name.Substring(1);

                                    Log("ZReport_DoWork ������������ ����� " + a.Name);
                                    //�������� ������ ����� ��� z-������
                                    List<FileInfo> files_to_z = zmaker.prepareZpackage(a);
                                    Log("ZReport_DoWork ������� ����� " + a.Name);
                                    //������� z-������
                                    FileInfo zrep = zmaker.makeZreport(files_to_z, teremok.teremok_1C + "_X" + num_name + ".zip", z_temp_dir);
                                    Log("ZReport_DoWork ������� z-����� " + zrep.Name);

                                    //�������� � outbox
                                    if (zrep != null)
                                    {
                                        zrep.CopyTo(Path.Combine(dir_outbox.FullName, zrep.Name), true);
                                        Log("ZReport_DoWork ����������� � outbox " + zrep.Name);
                                    }
                                    //���������� x_otchet
                                    Log("ZReport_DoWork ������ ����� " + a.Name);
                                    doc_id=zreporthelper.ZReportParse(a);
                                }
                                catch (Exception ex)
                                {
                                    Log(ex, "ZReport_DoWork ������ �������� z_������ " + a.FullName);
                                }
                            }, null) { Timeout=1000 }.Start();
                    });
                }
                
                if (doc_id != 0)
                {
                    StaticConstants.MainGridUpdate();
                    AsyncWorker_Inbox.RunAnyWay();
                    //WorkerRun(backgroundWorker_Inbox);
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
            finally
            {
                log.Debug("backgroundWorker_ZReport_DoWork finished");
                m_working_zreport = false;
                StaticConstants.m_sending_z_report = false;
            }
        }

        private static void Operate_Out_Z_Folder(DirectoryInfo z_back_dir, Func<DirectoryInfo, IEnumerable<FileInfo>> get_missing_x_files, ZreportMaker zmaker, string OutPath)
        {
            var kkmoutdir = new DirectoryInfo(OutPath);

            IEnumerable<FileInfo> x_f_miss_kkm = get_missing_x_files(kkmoutdir);

            if (x_f_miss_kkm.Count() > 0)
            {

                //���������� ���������� ����� ����� ������ ������
                Log("ZReport_DoWork ���� ����� �����");
                int kkm_num = UpdateClass.GetKkmName(x_f_miss_kkm.First());
                Log("ZReport_DoWork ����� ����� " + kkm_num);


                ClearOutDirectory(kkmoutdir);
                
#if(!DEB)
                if (kkm_num != -1)
                {
                    Log("ZReport_DoWork ���������� ���������� ����� �� ����� �������");
                    UpdateClass.kkm_LogOutFilesIn(StaticConstants.Main_Teremok_1cName, kkm_num, "5", OutPath);
                    Log("ZReport_DoWork ���������� ���������� ����� �� ����� ������� ���������");
                }


                foreach (var b in x_f_miss_kkm)
                {
                    //����������� �� �������������� ������
                    Log("ZReport_DoWork ����������� �� �������������� ������ " + b.Name);
                    UpdateClass.kkm_UpdateZReport(OutPath, b);
                }
#endif

                x_f_miss_kkm.ToList().ForEach(b =>
                {
                    //�������� ������ ����� ��� z-������
                    Log("ZReport_DoWork �������� ������ ����� ��� z-������ " + b.Name);
                    List<FileInfo> files_to_z = zmaker.prepareZpackage(b);

                    Log("ZReport_DoWork ���������� ����� z-������ � ����� " + z_back_dir.FullName);
                    files_to_z.ForEach(c =>
                    {
                        c.MoveWithReplase(z_back_dir.FullName);
                        Log("ZReport_DoWork ���������� " + c.Name);
                    });
                });

                //���������� ���������� ����� ����� ����� �������
#if(!DEB)
                if (kkm_num != -1)
                {
                    Log("ZReport_DoWork ���������� ���������� ����� ����� ����� �������");
                    UpdateClass.kkm_LogOutFilesOut(StaticConstants.Main_Teremok_1cName, kkm_num, "5", OutPath);
                    Log("ZReport_DoWork ���������� ���������� ����� ����� ����� ������� ���������");
                }
#endif
            }
        }

        private static void ClearOutDirectory(DirectoryInfo kkmoutdir)
        {
            int _left = StaticConstants.RBINNER_CONFIG.GetProperty("rbclient_kkm_out_days_left", 90);
            ClearFolderLog(kkmoutdir,
                DateTime.Now.AddDays(-_left), "*.*");
        }

        // Z-�����
        private void timer_ZReport_Tick(object sender, EventArgs e)
        {
            try
            {
                if (IsWorkerEnabled(backgroundWorker_ZReport))
                {
                    log.Debug("timer_ZReport_Tick �������� backgroundWorker_ZReport");
                    backgroundWorker_ZReport.RunWorkerAsync();
                }
                else
                {
                    log.Debug("backgroundWorker_ZReport is busy");
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
            
        }

        private void CheckZReport()
        {
            try
            {
                if (m_kkm1_online)
                    ZReportSendItem(CParam.Kkm1Out);
                if (m_kkm2_online)
                    ZReportSendItem(CParam.Kkm2Out);
                if (m_kkm3_online)
                    ZReportSendItem(CParam.Kkm3Out);
                if (m_kkm4_online)
                    ZReportSendItem(CParam.Kkm4Out);
                if (m_kkm5_online)
                    ZReportSendItem(CParam.Kkm5Out);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }


        public void CheckZReportFlesh()
        {
            try
            {
                if (m_kkm1_online)
                    ZReportSendItemToFlesh(CParam.Kkm1Out);
                if (m_kkm2_online)
                    ZReportSendItemToFlesh(CParam.Kkm2Out);
                if (m_kkm3_online)
                    ZReportSendItemToFlesh(CParam.Kkm3Out);
                if (m_kkm4_online)
                    ZReportSendItemToFlesh(CParam.Kkm4Out);
                if (m_kkm5_online)
                    ZReportSendItemToFlesh(CParam.Kkm5Out);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        private void ZReportSendItem(string kkm_folder_in)
        {
            bool _is_Y = false;
            bool _is_D = false;
            DateTime _create_datetime;
            //������� �������� �����
            try
            {
                // Log("�������� Z-������ � �� ���� " + kkm_folder_in);
                CBData _data = new CBData();
                CZReportHelper _zreport = new CZReportHelper();

                string _teremok_folder = _data.GetTeremokFolder(Convert.ToInt32(CParam.TeremokId));

                DirectoryInfo _dir = new DirectoryInfo(kkm_folder_in);
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    // ��������� ����� ������ 31 ����?
                    _create_datetime = _file.CreationTime.AddDays(31);
                    if (_create_datetime <= DateTime.Now)
                    {
                        Log("��������� ���� ������ 31 ���� " + _file.FullName);
                        log.Info("��������� ���� ������ 31 ���� " + _file.FullName);
                        File.Delete(_file.FullName);
                    }
                    else
                    {
                        // ��������� - ��� X-�����?
                        if (_file.Name.Substring(0, 1).ToLower() == "x")
                        {
                            // ���������, ��������� �� ������ ����
                            if (!_zreport.IsZReportSent(_file.Name))
                            {
                                // ���������� ��� �������� � Outbox X-�����
                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + _file.Name))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + _file.Name);
                                //else
                                File.Copy(_file.FullName, CParam.AppFolder + "\\outbox\\" + _file.Name);

                                // ���������� D �����
                                string _name = _file.Name.Substring(1, _file.Name.Length - 1);
                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + "DX" + _name))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + "DX" + _name);

                                if (File.Exists(kkm_folder_in + "\\" + "DX" + _name))
                                {
                                    _is_D = true;
                                    File.Copy(kkm_folder_in + "\\" + "DX" + _name, CParam.AppFolder + "\\outbox\\" + "DX" + _name);
                                }


                                // ���������� Y �����                                
                                string _name_Y = _name;
                                // � Y ������ ���� ���������� - �������� ���� ����� ����� �����. ���������� ����������
                                // ��� ��������.
                                // ������ ��� ����� ����� � �����
                                if (!File.Exists(kkm_folder_in + "\\Y" + _name_Y))
                                {
                                    // ������������ ��������, ������� ����
                                    _name_Y = _name.Insert(6, "0");
                                }
                                // �������� ��� ���
                                if (File.Exists(kkm_folder_in + "\\Y" + _name_Y))
                                {
                                    if (File.Exists(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y))
                                        File.Delete(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);

                                    _is_Y = true;
                                    File.Copy(kkm_folder_in + "\\" + "Y" + _name_Y, CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);


                                } // Y ����� ���, ������ ����� ���� ��� �����
                                else
                                {
                                    _name_Y = _name_Y.Insert(6, "0");
                                    // �������� ��� ���
                                    if (File.Exists(kkm_folder_in + "\\Y" + _name_Y))
                                    {
                                        if (File.Exists(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y))
                                            File.Delete(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);

                                        _is_Y = true;
                                        File.Copy(kkm_folder_in + "\\" + "Y" + _name_Y, CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);


                                    } // Y ����� ������� ���
                                }

                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + _teremok_folder + "_X" + _name + ".zip"))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + _teremok_folder + "_X" + _name + ".zip");

                                using (ZipFile _zip = new ZipFile())
                                {
                                    _zip.AddFile(CParam.AppFolder + "\\outbox\\" + "X" + _name);
                                    if (_is_D)
                                    {
                                        _zip.AddFile(CParam.AppFolder + "\\outbox\\" + "DX" + _name);
                                    }
                                    if (_is_Y)
                                    {
                                        _zip.AddFile(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);
                                    }
                                    _zip.Save(CParam.AppFolder + "\\outbox\\" + _teremok_folder + "_X" + _name + ".zip");
                                }

                                // ������� ��� �����
                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + _file.Name))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + _file.Name);
                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + "DX" + _name))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + "DX" + _name);
                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);

                                // ����� ����� 
                                _zreport.ZReportParse(_file);
                                walkToLogs();

                                //������ ����
                                _zreport.ChekItemDelete(DateTime.Today.AddDays(-2));

                                _is_D = false;
                                _is_Y = false;
                                // ��������� ������                        
                                m_need_refresh_docjournal = true;
                                StaticConstants.MainGridUpdate();
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Log("������ " + exp.Message);
                // throw exp;
            }
        }

        public void ZReportSendItemToFlesh(string kkm_folder_in)
        {
            bool _is_Y = false;
            bool _is_D = false;

            DateTime _create_datetime;
            //������� �������� �����
            try
            {
                // Log("�������� Z-������ � �� ���� " + kkm_folder_in);
                CBData _data = new CBData();
                CZReportHelper _zreport = new CZReportHelper();

                string _teremok_folder = _data.GetTeremokFolder(Convert.ToInt32(CParam.TeremokId));

                DirectoryInfo _dir = new DirectoryInfo(kkm_folder_in);
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    // ��������� ����� ������ 50 ����?
                    _create_datetime = _file.CreationTime.AddDays(50);
                    if (_create_datetime <= DateTime.Now)
                    {
                        // ������� ������ � ���� ������!
                        // �����
                        Log("��������� ���� ������ 50 ���� " + _file.FullName);
                        log.Info("��������� ���� ������ 50 ���� " + _file.FullName);
                        // ������� ����
                        File.Delete(_file.FullName);
                    }
                    else
                    {
                        // ��������� - ��� X-�����?
                        if (_file.Name.Substring(0, 1).ToLower() == "x")
                        {
                            // ���������, ��������� �� ������ ����
                            //if (!_zreport.IsZReportSent(_file.Name))
                            {
                                // ���������� ��� �������� � Outbox X-�����
                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + _file.Name))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + _file.Name);
                                //else
                                File.Copy(_file.FullName, CParam.AppFolder + "\\outbox\\" + _file.Name);

                                // ���������� D �����
                                string _name = _file.Name.Substring(1, _file.Name.Length - 1);
                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + "DX" + _name))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + "DX" + _name);

                                if (File.Exists(kkm_folder_in + "\\" + "DX" + _name))
                                {
                                    _is_D = true;
                                    File.Copy(kkm_folder_in + "\\" + "DX" + _name, CParam.AppFolder + "\\outbox\\" + "DX" + _name);
                                }


                                // ���������� Y �����                                
                                string _name_Y = _name;
                                // � Y ������ ���� ���������� - �������� ���� ����� ����� �����. ���������� ����������
                                // ��� ��������.
                                // ������ ��� ����� ����� � �����
                                if (!File.Exists(kkm_folder_in + "\\Y" + _name_Y))
                                {
                                    // ������������ ��������, ������� ����
                                    _name_Y = _name.Insert(6, "0");
                                }
                                // �������� ��� ���
                                if (File.Exists(kkm_folder_in + "\\Y" + _name_Y))
                                {
                                    if (File.Exists(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y))
                                        File.Delete(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);

                                    _is_Y = true;
                                    File.Copy(kkm_folder_in + "\\" + "Y" + _name_Y, CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);


                                } // Y ����� ���, ������ ����� ���� ��� �����
                                else
                                {
                                    _name_Y = _name_Y.Insert(6, "0");
                                    // �������� ��� ���
                                    if (File.Exists(kkm_folder_in + "\\Y" + _name_Y))
                                    {
                                        if (File.Exists(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y))
                                            File.Delete(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);

                                        _is_Y = true;
                                        File.Copy(kkm_folder_in + "\\" + "Y" + _name_Y, CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);


                                    } // Y ����� ������� ���
                                }

                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + _teremok_folder + "_X" + _name + ".zip"))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + _teremok_folder + "_X" + _name + ".zip");

                                using (ZipFile _zip = new ZipFile())
                                {
                                    _zip.AddFile(CParam.AppFolder + "\\outbox\\" + "X" + _name);
                                    if (_is_D)
                                    {
                                        _zip.AddFile(CParam.AppFolder + "\\outbox\\" + "DX" + _name);
                                    }
                                    if (_is_Y)
                                    {
                                        _zip.AddFile(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);
                                    }
                                    _zip.Save(CParam.AppFolder + "\\outbox\\" + _teremok_folder + "_X" + _name + ".zip");
                                }

                                // ������� ��� �����
                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + _file.Name))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + _file.Name);
                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + "DX" + _name))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + "DX" + _name);
                                if (File.Exists(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y))
                                    File.Delete(CParam.AppFolder + "\\outbox\\" + "Y" + _name_Y);

                                // ����� �����                                
                                _zreport.ZReportParse(_file);

                                _is_D = false;
                                _is_Y = false;
                                // ��������� ������                        
                                StaticConstants.MainGridUpdate();
                                m_need_refresh_docjournal = true;
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Log("������ " + exp.Message);
                throw exp;
            }
        }


        private void UpdateKkmStatusOnWebService(string kkm_in,string kkm_out,bool kkm_online)
        {
            t_Kkm kkm = new t_Kkm().SelectFirst<t_Kkm>(String.Format("out_folder='{0}'", kkm_out));
            if (kkm != null && !String.IsNullOrEmpty(kkm.kkm_name))
            {
                UpdateClass.kkm_UpdateOnlineState(kkm_in, kkm_out, kkm_online);
            }
        }

        /// <summary>
        /// ��������� ����������� ����
        /// </summary>
        private void CheckKKM()
        {
            
            try
            {
                if (CParam.Kkm1Out != "" || CParam.Kkm1In != "")
                {
                    if (Directory.Exists(CParam.Kkm1Out) && Directory.Exists(CParam.Kkm1In))
                    {
                        m_kkm1_online = true;
                    }
                    else
                    {
                        Log("����� 1 �� ��������!");
                        log.Info("����� 1 �� ��������!");
                        m_kkm1_online = false;
                    }

                    UpdateKkmStatusOnWebService(CParam.Kkm1In, CParam.Kkm1Out, m_kkm1_online);
                }

#if(TREPDEB)
                return;
#endif
                if (CParam.Kkm2Out != "" || CParam.Kkm2In != "")
                {
                    if (Directory.Exists(CParam.Kkm2Out) && Directory.Exists(CParam.Kkm2In))
                    {
                        m_kkm2_online = true;
                    }
                    else
                    {
                        Log("����� 2 �� ��������!");
                        log.Info("����� 2 �� ��������!");
                        m_kkm2_online = false;
                    }
                    UpdateKkmStatusOnWebService(CParam.Kkm2In, CParam.Kkm2Out, m_kkm2_online);
                }
                if (CParam.Kkm3Out != "" || CParam.Kkm3In != "")
                {
                    if (Directory.Exists(CParam.Kkm3Out) && Directory.Exists(CParam.Kkm3In))
                    {
                        m_kkm3_online = true;
                    }
                    else
                    {
                        Log("����� 3 �� ��������!");
                        log.Info("����� 3 �� ��������!");
                        m_kkm3_online = false;
                    }
                    UpdateKkmStatusOnWebService(CParam.Kkm3In, CParam.Kkm3Out, m_kkm3_online);
                }
                if (CParam.Kkm4Out != "" || CParam.Kkm4In != "")
                {
                    if (Directory.Exists(CParam.Kkm4Out) && Directory.Exists(CParam.Kkm4In))
                    {
                        m_kkm4_online = true;
                    }
                    else
                    {
                        Log("����� 4 �� ��������!");
                        log.Info("����� 4 �� ��������!");
                        m_kkm4_online = false;
                    }
                    UpdateKkmStatusOnWebService(CParam.Kkm4In, CParam.Kkm4Out, m_kkm4_online);
                }
                if (CParam.Kkm5Out != "" || CParam.Kkm5In != "")
                {
                    if (Directory.Exists(CParam.Kkm5Out) && Directory.Exists(CParam.Kkm5In))
                    {
                        m_kkm5_online = true;
                    }
                    else
                    {
                        Log("����� 5 �� ��������!");
                        log.Info("����� 5 �� ��������!");
                        m_kkm5_online = false;
                    }
                    UpdateKkmStatusOnWebService(CParam.Kkm5In, CParam.Kkm5Out, m_kkm5_online);
                }

            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        private void ShowKKMStatus()
        {
            try
            {

                if (m_kkm1_online)
                {
                    toolStripStatusLabel_KKM1.Enabled = true;
                }
                if (m_kkm2_online)
                {
                    toolStripStatusLabel_KKM2.Enabled = true;
                }
                if (m_kkm3_online)
                {
                    toolStripStatusLabel_KKM3.Enabled = true;
                }
                if (m_kkm4_online)
                {
                    toolStripStatusLabel_KKM4.Enabled = true;
                }
                if (m_kkm5_online)
                {
                    toolStripStatusLabel_KKM5.Enabled = true;
                }

            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        #region UTILS FUNC

        private void Exp(string message)
        {
            m_thread_working = false; // ������������� �������, ��� �� ����� ���� ������ ������
            Log(message);
        }

        // ���������� ���� ����
        private string FormatFileDate(DateTime dt, int add_days)
        {
            try
            {
                DateTime _dt = dt.AddDays(add_days);
                return AttachZeroToDate(_dt.Year) + "_" + AttachZeroToDate(_dt.Month) + "_" + AttachZeroToDate(_dt.Day);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                throw exp;
            }
        }

        // ��������� �������� ����
        protected void WriteXML(DataTable dt, string file_name, int doc_id, int type)
        {
            string _line="";
            int teremok_id;
            TextWriter m_file = null;

            CBData _data = new CBData();
            try
            {
                m_file = new StreamWriter(file_name);

                switch (type)
                {
                    case 15:
                        _line = "<DocumentElement code-1C=\"" + _data.GetDocCode1C(doc_id) + "\">";
                        break;
                    case 19:
                        // �������� ������� ���� � �����
#if(!INKASS)
                        _line = "<DocumentElement time=\"" + _data.GetDateInkass(doc_id) + "\">";
#endif
                        break;
                    case 21:
                        // ������� ��� �������
                        teremok_id = _data.GetTeremok2Doc(doc_id);
                        _line = "<DocumentElement teremok2_code=\"" + _data.GetTeremo1CByID(teremok_id) + "\">";
                        break;
                    default:
                        _line = "<DocumentElement>";
                        break;
                }
                if (type == 15)
                {
                    m_file.WriteLine(_line);
                    m_file.WriteLine("<o2p time=\"" + _data.GetDateInkass(doc_id) + "\" />");
                    m_file.WriteLine("<o2p timecreate=\"" + _data.GetDateCreate(doc_id) + "\" />");
                    // ��������� ������
                    foreach (DataRow _row in dt.Rows)
                    {
                        _line = "<o2p c=\"";
                        _line += _row[0].ToString();
                        _line += "\" q=\"";
                        _line += CUtilHelper.ParceAmount(_row[1].ToString(), 0);
                        _line += "\" i=\"";
                        _line += _row[2].ToString();
                        _line += "\" dv=\"";
                        _line += CUtilHelper.ParceAmount(_row[3].ToString(), 0);
                        _line += "\" d=\"";
                        _line += _row[4].ToString();
                        _line += "\" g=\"";
                        _line += _row[5].ToString();
                        _line += "\" di=\"";
                        _line += CUtilHelper.ParceAmount(_row[6].ToString(), 0);

                        t_Nomenclature t_m = new t_Nomenclature().SelectFirst<t_Nomenclature>
                                  ("nome_1C='" + _row[0].ToString() + "'");

                        if (t_m != null)
                        {
                            _line += "\" nn=\"";
                            _line += t_m.nome_name.Replace("\"", "&quot;");

                            _line += "\" ne=\"";
                            _line += t_m.nome_ed;
                        }

                        _line += "\" />";

                        m_file.WriteLine(_line);
                    }
                    _line = "</DocumentElement>";
                    m_file.WriteLine(_line);
                }
                if (CParam.AppCity == 2)
                {
                    if (type == 9 || type== 13)
                    {
                        {
                            m_file.WriteLine(_line);
                            m_file.WriteLine("<o2p time=\"" + _data.GetDateInkass(doc_id) + "\" />");
                            m_file.WriteLine("<o2p timecreate=\"" + _data.GetDateCreate(doc_id) + "\" />");
                            // ��������� ������
                            foreach (DataRow _row in dt.Rows)
                            {
                                _line = "<o2p c=\"";
                                _line += _row[0].ToString();

                                _line += "\" q=\"";
                                _line += CUtilHelper.ParceAmount(_row[7].ToString(), 0);

                                _line += "\" i=\"";
                                _line += _row[2].ToString();

                                _line += "\" dv=\"";
                                _line += CUtilHelper.ParceAmount(_row[3].ToString(), 0);

                                _line += "\" d=\"";
                                _line += _row[4].ToString();

                                _line += "\" g=\"";
                                _line += _row[5].ToString();

                                _line += "\" di=\"";
                                _line += CUtilHelper.ParceAmount(_row[6].ToString(), 0);

                                _line += "\" ps=\"";
                                _line += CUtilHelper.ParceAmount(_row[8].ToString(), 0);

                                _line += "\" />";

                                m_file.WriteLine(_line);
                            }
                            _line = "</DocumentElement>";
                            m_file.WriteLine(_line);
                        }
                    }

#if(INKASS)
                    if (type == 19)
                    {
                        var o=OrderClass.CreateOrderClass(doc_id);
                        o.CreateDetails(false);
                        m_file.Write(o.WriteXml());
                    }

                    if (type != 15 && type != 9 && type != 13 && type != 19)
#else
                        if (type != 15 && type != 9 && type != 13)
#endif
                    {
                        m_file.WriteLine(_line);
                        m_file.WriteLine("<o2p time=\"" + _data.GetDateInkass(doc_id) + "\" />");
                        m_file.WriteLine("<o2p timecreate=\"" + _data.GetDateCreate(doc_id) + "\" />");
                        // ��������� ������


                        foreach (DataRow _row in dt.Rows)
                        {
                            _line = "<o2p c=\"";
                            _line += _row[0].ToString();

                            _line += "\" q=\"";
                            _line += CUtilHelper.ParceAmount(_row[7].ToString(), 0);

                            _line += "\" i=\"";
                            _line += _row[2].ToString();

                            _line += "\" dv=\"";
                            _line += CUtilHelper.ParceAmount(_row[3].ToString(), 0);

                            _line += "\" d=\"";
                            _line += _row[4].ToString();

                            _line += "\" g=\"";
                            _line += _row[5].ToString();

                            _line += "\" di=\"";
                            _line += CUtilHelper.ParceAmount(_row[6].ToString(), 0);


                            t_Nomenclature t_m = new t_Nomenclature().SelectFirst<t_Nomenclature>
                              ("nome_1C='" + _row[0].ToString() + "'");

                            if (t_m != null)
                            {
                                _line += "\" nn=\"";
                                _line += t_m.nome_name.Replace("\"", "&quot;");

                                _line += "\" ne=\"";
                                _line += t_m.nome_ed;
                            }

                            _line += "\" />";

                            m_file.WriteLine(_line);
                        }
                        _line = "</DocumentElement>";
                        m_file.WriteLine(_line);
                    }
                }
                if (CParam.AppCity == 1)
                {
                    if (type == 9 || type == 13)
                    {
                        {
                            m_file.WriteLine(_line);
                            m_file.WriteLine("<o2p time=\"" + _data.GetDateInkass(doc_id) + "\" />");
                            m_file.WriteLine("<o2p timecreate=\"" + _data.GetDateCreate(doc_id) + "\" />");
                            // ��������� ������
                            foreach (DataRow _row in dt.Rows)
                            {
                                _line = "<o2p c=\"";
                                _line += _row[0].ToString();

                                _line += "\" q=\"";
                                _line += CUtilHelper.ParceAmount(_row[7].ToString(), 0);

                                _line += "\" i=\"";
                                _line += _row[2].ToString();

                                _line += "\" dv=\"";
                                _line += CUtilHelper.ParceAmount(_row[3].ToString(), 0);

                                _line += "\" d=\"";
                                _line += _row[4].ToString();

                                _line += "\" g=\"";
                                _line += _row[5].ToString();

                                _line += "\" di=\"";
                                _line += CUtilHelper.ParceAmount(_row[6].ToString(), 0);

                                _line += "\" ps=\"";
                                _line += CUtilHelper.ParceAmount(_row[8].ToString(), 0);

                                _line += "\" />";

                                m_file.WriteLine(_line);
                            }
                            _line = "</DocumentElement>";
                            m_file.WriteLine(_line);
                        }
                    }
                    if (type != 15 && type != 9 && type != 13)
                    {

                        m_file.WriteLine(_line);
                        m_file.WriteLine("<o2p time=\"" + _data.GetDateInkass(doc_id) + "\" />");
                        m_file.WriteLine("<o2p timecreate=\"" + _data.GetDateCreate(doc_id) + "\" />");
                        // ��������� ������
                        foreach (DataRow _row in dt.Rows)
                        {
                            _line = "<o2p c=\"";
                            _line += _row[0].ToString();

                            _line += "\" q=\"";
                            _line += CUtilHelper.ParceAmount(_row[1].ToString(), 0);

                            _line += "\" i=\"";
                            _line += _row[2].ToString();

                            _line += "\" dv=\"";
                            _line += CUtilHelper.ParceAmount(_row[3].ToString(), 0);

                            _line += "\" d=\"";
                            _line += _row[4].ToString();

                            _line += "\" g=\"";
                            _line += _row[5].ToString();

                            _line += "\" di=\"";
                            _line += CUtilHelper.ParceAmount(_row[6].ToString(), 0);

                            t_Nomenclature t_m = new t_Nomenclature().SelectFirst<t_Nomenclature>
                                   ("nome_1C='" + _row[0].ToString()+"'");
                            if (t_m != null)
                            {
                                _line += "\" nn=\"";
                                _line += t_m.nome_name.Replace("\"", "&quot;");

                                _line += "\" ne=\"";
                                _line += t_m.nome_ed;
                            }

                            _line += "\" />";

                            m_file.WriteLine(_line);
                        }
                        _line = "</DocumentElement>";
                        m_file.WriteLine(_line);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
            finally
            {
                if (m_file != null)
                    m_file.Close();
            }
        }

        //private void DeleteContenOutboxFolder()
        //{
        //    try
        //    {

        //        DirectoryInfo _dir_in = new DirectoryInfo(CParam.AppFolder + "\\outbox\\");
        //        foreach (FileInfo _file in _dir_in.GetFiles())
        //        {
        //            File.Delete(_file.FullName);
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        ShowMessage("MD0043", exp.Message, true);
        //    }
        //}
        #endregion


        

        // ��������� ���� ��������
        private void toolStripStatusLabel_Transf_Click(object sender, EventArgs e)
        {
            // ������
            //if (MessageBox.Show("������� ������:", "������", MessageBoxButtons.OKCancel)
            if (EnterPasswordWindow() == DialogResult.OK)
            {
                SettingsWindowShow();
            }
            
        }

        private void SettingsWindowShow()
        {
            Log("������� ���� ��������");
            log.Info("������� ���� ��������");

            // ���������, ������� �� ��� ����             
            Form[] _forms = this.MdiChildren;
            bool _f_opened = false;

            try
            {
                foreach (Form _f in _forms)
                {
                    if (_f.Name == "FormParam")
                    {
                        _f.Activate();
                        _f_opened = true;
                        break;
                    }
                }

                if (!_f_opened)
                {
                    FormParam _form = new FormParam();
                    _form.MdiParent = this;
                    _form.Show();
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        // ��������� T-������
        private void TReportPush()
        {
            try
            {
                if (File.Exists(CParam.AppFolder + "\\Data\\0.dat"))
                {
                    if (m_kkm1_online)
                    {
                        if (!File.Exists(CParam.Kkm1In + "\\0.dat"))
                            File.Copy(CParam.AppFolder + "\\Data\\0.dat", CParam.Kkm1In + "\\0.dat");
                        System.Threading.Thread.Sleep(6000); // ����� 2 ������� �� �������� ������ �������
                    }
                    if (m_kkm2_online)
                    {
                        if (!File.Exists(CParam.Kkm2In + "\\0.dat"))
                            File.Copy(CParam.AppFolder + "\\Data\\0.dat", CParam.Kkm2In + "\\0.dat");
                        System.Threading.Thread.Sleep(6000); // ����� 2 ������� �� �������� ������ �������
                    }
                    if (m_kkm3_online)
                    {
                        if (!File.Exists(CParam.Kkm3In + "\\0.dat"))
                            File.Copy(CParam.AppFolder + "\\Data\\0.dat", CParam.Kkm3In + "\\0.dat");
                        System.Threading.Thread.Sleep(6000); // ����� 2 ������� �� �������� ������ �������
                    }
                    if (m_kkm4_online)
                    {
                        if (!File.Exists(CParam.Kkm4In + "\\0.dat"))
                            File.Copy(CParam.AppFolder + "\\Data\\0.dat", CParam.Kkm4In + "\\0.dat");
                        System.Threading.Thread.Sleep(6000); // ����� 2 ������� �� �������� ������ �������
                    }
                    if (m_kkm5_online)
                    {
                        if (!File.Exists(CParam.Kkm5In + "\\0.dat"))
                            File.Copy(CParam.AppFolder + "\\Data\\0.dat", CParam.Kkm5In + "\\0.dat");
                        System.Threading.Thread.Sleep(6000); // ����� 2 ������� �� �������� ������ �������
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }


        // �������� ������������� �����
        private void backgroundWorker_TReport_DoWork(object sender, DoWorkEventArgs e)
        {
            
            //#region test
            //#if(DEB)

            //������ ��������
            log.Debug("backgroundWorker_TReport_DoWork started");
            List<FileInfo> t_reps = RbClientGlobalStaticMethods.GetTReports(
                RbClientGlobalStaticMethods.ReturnKKmInPathes(),
                RbClientGlobalStaticMethods.ReturnKKmOutPathes(), true);

            DirectoryInfo tdir = RbClientGlobalStaticMethods.GetDirectory(StaticConstants.TREPORT_FOLDER);
            tdir.DeleteOldFilesInDir(StaticConstants.TREPORT_FOLDER_SIZE);
                

            if (t_reps != null)
            {
                //������� ����� ��� ���

                t_reps.ForEach(a =>
                {
                    new CustomAction((o) =>
                    {
                        a.MoveWithReplase(tdir.FullName);
                    },null).Start();
                });
            }

            //#endif
            //#endregion
        }

        // �������� ������������� �����
        private void timer_TReport_Tick(object sender, EventArgs e)
        {
            try
            {
                if (IsWorkerEnabled(backgroundWorker_TReport))
                {
                    backgroundWorker_TReport.RunWorkerAsync();
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        // ������������� �����
        private void TReportGet(string kkm_folder_out)
        {
            try
            {
                DirectoryInfo _dir = new DirectoryInfo(kkm_folder_out);
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    if (_file.Name.Substring(0, 1) == "T")
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\TReport\\"))
                            Directory.CreateDirectory(CParam.AppFolder + "\\TReport\\");
                        File.Copy(_file.FullName, CParam.AppFolder + "\\TReport\\" + _file.Name, true);
                        File.Delete(_file.FullName);
                        Log("�������� T-����� � ����� " + _file.Name);
                        log.Info("�������� T-����� � ����� " + _file.Name);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                throw exp;
            }
        }

        private void backgroundWorker_CheckKKM_DoWork(object sender, DoWorkEventArgs e)
        {
                m_thread_work = true;
                log.Debug("backgroundWorker_CheckKKM_DoWork started");
                log.Debug("CheckKKM");
                CheckKKM();
                StaticHelperClass.ExecuteInvokeRequiredDelegate_WinForm<MDIParentMain>
                    (this, (oo) => { ((MDIParentMain)oo).ShowKKMStatus(); });
                log.Debug("backgroundWorker_CheckKKM_DoWork finished");
                m_thread_work = false;
        }

        // ������ ��������� � ����
        public bool UpdateNewVer()
        {
            try
            {
                // ������� ���������� ���������
                CUpdateHelper _update = new CUpdateHelper();

                //_update.Update_4_0_0_11();
                //_update.Update_4_0_0_12();
                //_update.Update_4_0_0_15();
                //_update.Update_4_0_0_16();
               // _update.Update_4_0_0_17();
                //_update.Update_4_0_0_18();
                //_update.Update_4_0_0_19();
                //_update.Update_4_0_0_20();
                _update.Update_5_4_0_0();

                if (CParam.AppCity == 1)
                {
                    _update.DeleteTeremok();
                }
                if (CParam.AppCity == 2)
                {
                    _update.NewTeremok();
                }
                return false;
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
                return false;
            }
        }

        private void backgroundWorker_Inbox_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // ������ ������ ���������
        }

        private void toolStripButton_Encashment_Click(object sender, EventArgs e)
        {
            #if(INKASS)
                        WindowStarter.�����������������(DateTime.Now);
            #else
                        OpenOrder(19, 0);
            #endif
        }

        /// <summary>
        /// ������� ���� � ������� �������� ��� ������ � �����
        /// </summary>
        private void MakeUptimeFile()
        {
            TextWriter m_file = null;
            try
            {
                m_file = new StreamWriter(CParam.AppFolder + "\\uptime.log");
                
                m_file.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
                //t_Teremok t_terem = new t_Teremok().SelectFirst<t_Teremok>("teremok_current=TRUE");

                //m_file.WriteLine("teremok_id="+t_terem.teremok_id);
                //m_file.WriteLine("teremok_1C=" + t_terem.teremok_1C);
                //m_file.WriteLine("teremok_name=" + t_terem.teremok_name);

                //t_Conf t_conf = new t_Conf().SelectFirst<t_Conf>("conf_param='ver'");
                //m_file.WriteLine("version="+t_conf.conf_value);

            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0111", exp.Message, true);
            }
            finally
            {
                if (m_file != null)
                    m_file.Close();
            }
        }

        private void MakeOpenFile()
        {
            TextWriter m_file = null;

            try
            {
                m_file = new StreamWriter(CParam.AppFolder + "\\open.log");
                m_file.WriteLine(DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
                t_Teremok t_terem = new t_Teremok().SelectFirst<t_Teremok>("teremok_current=TRUE");

                m_file.WriteLine("teremok_id=" + t_terem.teremok_id);
                m_file.WriteLine("teremok_1C=" + t_terem.teremok_1C);
                m_file.WriteLine("teremok_name=" + t_terem.teremok_name);

                t_Conf t_conf = new t_Conf().SelectFirst<t_Conf>("conf_param='ver'");
                m_file.WriteLine("version=" + t_conf.conf_value);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0112", exp.Message, true);
            }
            finally
            {
                if (m_file != null)
                    m_file.Close();
            }
        }

        private void MakeCloseFile()
        {
            TextWriter m_file = null;

            try
            {
                m_file = new StreamWriter(CParam.AppFolder + "\\close.log");
                m_file.WriteLine(DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0112", exp.Message, true);
            }
            finally
            {
                if (m_file != null)
                    m_file.Close();
            }
        }

        // ������ ���������
        
        public static void Log(Exception exp,string message)
        {
            lock (log)
            {
                log.Error(message + " error:" + exp.Message, exp);
            }
        }

        // ������ ���������
        public static void Log(string message)
        {
            lock (log)
            {
                log.Debug(message);
            }
        }

        // ������ ���������
        //public static void Log(string message)
        //{

        //    TextWriter m_file = null;
        //    try
        //    {
        //        log.Info(message);
        //        m_file = new StreamWriter("RBClient.log", true, System.Text.Encoding.GetEncoding(1251));
        //        m_file.WriteLine(DateTime.Today.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " > " + message + "  " + new StackTrace(1).ToString().Substring(0, 70));

        //    }
        //    catch (Exception exp)
        //    {
        //        log.Error("Exception: " + exp.Message, exp);
        //        // ������ �� ������
        //    }
        //    finally
        //    {
        //        if (m_file != null)
        //            m_file.Close();
        //    }
        //}

        private void CheckLog()
        {
            FileInfo _fi;
            try
            {
                if (!File.Exists(CParam.AppFolder + "\\RBClient.log"))
                    return;
                _fi = new FileInfo(CParam.AppFolder + "\\RBClient.log");
                if (_fi.Length > 1000000)
                {
                    if (!Directory.Exists(CParam.AppFolder + "\\Log"))
                        Directory.CreateDirectory(CParam.AppFolder + "\\Log");

                    File.Move(CParam.AppFolder + "\\RBClient.log", CParam.AppFolder + "\\Log\\" +
                        DateTime.Today.Year.ToString() + "_" +
                            DateTime.Today.Month.ToString() + "_" +
                                DateTime.Today.Day.ToString() + "_RBClient.log");
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                // ������ �� �����
            }
        }

        private void ShowMessage(string exp_num, string message, bool error)
        {
            m_Info = message;
            Log(message + " ���: " + exp_num);
            if (error)
                m_thread_working = false;
        }

        private void toolStripButton_Trans_Click(object sender, EventArgs e)
        {
            try
            {
                // �������� ��� ���������
                FormChooseTeremok _ft = new FormChooseTeremok();
                _ft.ShowDialog();
                int _teremok2_id = _ft.m_teremok_id;
                if (_teremok2_id == 0)
                    return;

                OpenOrder(21, _teremok2_id);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0011", exp.Message, true);
            }
        }

        private void backgroundWorker_Inbox_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StaticConstants.UpdateMainInterface(StaticConstants.MainWindow, a => ((MDIParentMain)a).progressBarItem1.Value
                = e.ProgressPercentage);
        }

        private void toolStripButton_flesh_Click(object sender, EventArgs e)
        {
            try
            {
                FormFlash _form = new FormFlash();
                _form.MdiParent = this;
                _form.ShowDialog();
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0048", exp.Message, true);
            }
        }

        private void buttonItem30_Click(object sender, EventArgs e)
        {
            Action act =new Action(()=>{
                            StaticConstants.UpdateMainInterface(StaticConstants.MainWindow, a => ((MDIParentMain)a).buttonItem30.Enabled = false);
                            startWebServiceExchange(0);
                            //startWebServiceExchange();
                       });

            AsyncCallback ascb = new AsyncCallback(o =>
            {
                StaticConstants.UpdateMainInterface(StaticConstants.MainWindow, a => ((MDIParentMain)a).buttonItem30.Enabled = true);
                ((Action)((AsyncResult)o).AsyncDelegate).EndInvoke(o);
            });

            
            if (StaticConstants.IsSyncProcess == true)
            {
                WpfCustomMessageBox.Show("����������� ��� ����������� � ������ ������!!!","��������!!!");
            }
            else
            {
                act.BeginInvoke(ascb, null);    
            }
        }

        private void buttonItem6_Click(object sender, EventArgs e)
        {

            //if (m_working_zreport)
            //{
            //    WpfCustomMessageBox.Show("� ������ ������ ���� ����� z-�������, �������� ���������� ����� ��������� � �������!!!","��������!!!");
            //}

            Log("������� ����� ������");
            log.Info("������� ����� ������");
            // ��������� ��� �������� ��������� ����� �������
            Form[] _forms = this.MdiChildren;
            try
            {
                foreach (Form _f in _forms)
                {
                    if (_f.Name != "FormDoc")
                    {
                        _f.Close();
                    }
                    if (_f is FormHost)
                    {
                        _f.Close();
                    }
                }

                FormExchange _form = new FormExchange();
                //_form.m_teremok_id = m_teremok_id;
                _form.ShowDialog();

                if (_form.m_update)
                {
                    this.Close();
                    return;
                }

                // �������� �������
                //Form[] _forms = this.MdiChildren;
                foreach (Form _f in _forms)
                {
                    if (_f.Name == "FormDoc")
                    {
                        FormDoc _fd = (FormDoc)_f;
                        _fd.IninData();
                        break;
                    }
                }


                AsyncWorker_Inbox.RunAnyWay();
                //if (backgroundWorker_Inbox.IsBusy)
                //{
                //    backgroundWorker_Inbox.CancelAsync();
                //    //backgroundWorker_Inbox_DoWork(this, null);
                //    backgroundWorker_Inbox.RunWorkerAsync();
                //}
                //else
                //{
                //    backgroundWorker_Inbox.RunWorkerAsync();
                //}
                //timer_CheckInbox_Tick(null, null);

                if (!is_timers_working())
                {
                    timers_resume();
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                ShowMessage("MD0009", exp.Message, true);
            }
        }

        private void toolStripButton_water_Click(object sender, EventArgs e)
        {
            OpenOrder(23, 0);
        }

        private void toolStripButton_electricity_Click(object sender, EventArgs e)
        {
            OpenOrder(24, 0);
        }

        private List<t_InfoMessage> ReturnNotifications(string query)
        {
            List<t_InfoMessage> tmplist = new List<t_InfoMessage>();
            tmplist = GlobalNotifications.Instance.FindNotificationFilteredByCity(query);
            return tmplist;
        }

        private List<t_InfoMessage> ZMissingKKmEveryDayNotifications(DateTime dt)
        {
            List<t_InfoMessage> tmplist = null;
            List<t_InfoMessage> Resultlist = null;
            
            tmplist = GetMessageForEveryDayNotification(3,5);

            if (tmplist.NotNullOrEmpty())
            {
                t_WorkTeremok t_work = ReturnTeremokWorkTime(dt);

                if (t_work != null)
                {
                    if (IfMessageTimeComes(dt, t_work, "afterdaynotifications_timeout",15))
                    {
                        Resultlist = new List<t_InfoMessage>();

                        List<t_Kkm> kkms = GetTodayWorkedKkms(dt);

                        //���� �� ��������� ������� ������ ���� ����������?
                        foreach (var kkm in kkms)
                        {
                            List<t_ZReport> zrep = GetZreportsByDateAndKkm(dt, kkm);

                            if (!(zrep.NotNullOrEmpty()))
                            {
                                //������� ���������� �� ���������� z-������
                                MakeZMissingMessage(tmplist, Resultlist, kkm);
                            }
                        }
                    }
                }
            }

            return Resultlist;
        }

        private static List<t_ZReport> GetZreportsByDateAndKkm(DateTime dt, t_Kkm kkm)
        {
            List<t_ZReport> zrep = new t_ZReport().Select<t_ZReport>("z_kkm_id='" + kkm.kkm_name +
                "' AND DateValue([z_date])=" + SqlWorker.ReturnDate(dt));
            return zrep;
        }

        private static List<t_ZReport> GetZreportsByDate(DateTime dt)
        {
            List<t_ZReport> zrep = new t_ZReport().Select<t_ZReport>("DateValue([z_date])=" + SqlWorker.ReturnDate(dt));
            return zrep;
        }

        private static List<t_Kkm> GetTodayWorkedKkms(DateTime dt)
        {
            List<t_Kkm> kkms = new t_Kkm().Select<t_Kkm>
                ("DateValue([lasttime_online])=" + SqlWorker.ReturnDate(dt));
            return kkms;
        }

        private static bool IfMessageTimeComes(DateTime dt, t_WorkTeremok t_work,string rb_inner_param_name,int default_timeout)
        {
            if (dt.Hour > t_work.teremok_lastTime.Hour)
            {
                return true;
            }
            else
            {
                if(dt.Hour == t_work.teremok_lastTime.Hour &&
                                    dt.Minute >= t_work.teremok_lastTime.Minute + StaticConstants.RBINNER_CONFIG.GetProperty<int>(rb_inner_param_name, default_timeout))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IfMessageTimeComes(DateTime dt, t_WorkTeremok t_work)
        {
            return dt.Hour >= t_work.teremok_lastTime.Hour &&
                                    dt.Minute >= t_work.teremok_lastTime.Minute;
        }

        private static t_WorkTeremok ReturnTeremokWorkTime(DateTime dt)
        {
            t_WorkTeremok t_work = new t_WorkTeremok().SelectFirst<t_WorkTeremok>
                (String.Format("teremok_id='{0}' AND teremok_day='{1}' AND " +
            "teremok_month='{2}' AND teremok_year='{3}'", StaticConstants.Teremok_ID, dt.Day, dt.Month, dt.Year));
            return t_work;
        }

        private static List<t_InfoMessage> GetMessageForEveryDayNotification(int notif_type, int doc_type_id)
        {
            return GlobalNotifications.Instance.FindNotificationFilteredByCity
                ("type=" + notif_type + " AND dayofweek=0 AND dayofmounth=0 AND lastmonthday=0 AND doc_type_id=" + doc_type_id);
        }

        private static void MakeZMissingMessage(List<t_InfoMessage> tmplist, List<t_InfoMessage> Resultlist, t_Kkm kkm)
        {
            t_InfoMessage info_message =
                (t_InfoMessage)StaticHelperClass.ExecuteMethodByName(tmplist.First(), "MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic, null);
            info_message.message = String.Format(info_message.message, kkm.kkm_name);
            Resultlist.Add(info_message);
        }

        private static List<t_InfoMessage> ReturnKKmEveryDayNotifications(DateTime dt)
        {
            List<t_InfoMessage> tmplist = null;
            List<t_InfoMessage> Resultlist = null;
            tmplist = GetMessageForEveryDayNotification(4, 5);


            if (tmplist.NotNullOrEmpty())
            {
                List<t_ZReport> zrep = GetZreportsByDate(dt).Where(a => a.z_total_return > 0).ToList();
                if (zrep.NotNullOrEmpty())
                {
                    Resultlist = new List<t_InfoMessage>();
                    t_InfoMessage info_message = StaticHelperClass.CloneObject(tmplist.First());

                    string message = info_message.message;
                    info_message.message = "";

                    foreach (var zr in zrep)
                    {
                        info_message.message += String.Format(message + "\r\n", zr.z_kkm);
                    }

                    Resultlist.Add(info_message);
                }
            }

            return Resultlist;
        }

        private void monthCalendar()
        {
            CBData _b = new CBData();
            int _doc_id;
            int dt;
            int dow;
            int dth;
            int dtm;
            DateTime now = DateTime.Now;

            dt = Convert.ToInt32(DateTime.Today.Day);
            int dtc;
            dtc = Convert.ToInt32(DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));

            dth = now.Hour;
            dtm = now.Minute;
            dow = (int)now.DayOfWeek;

            try
            {
                GlobalNotifications.Instance.HideLabel();

                List<t_InfoMessage> notifications=new List<t_InfoMessage>();
                List<t_InfoMessage> tmplist;
                //������������� �� ���������� ��� ������
                if (dt == dtc)
                {
                    notifications.AddRangeNotNull(ReturnNotifications("type=0 AND lastmonthday=-1"));
                }

                //������������� �� ��� ������
                notifications.AddRangeNotNull(ReturnNotifications("type=0 AND lastmonthday=0 AND dayofmounth=" + dt));

                //������������� �� ��� ������
                notifications.AddRangeNotNull(ReturnNotifications("type=0 AND lastmonthday=0 AND dayofmounth=0 AND dayofweek=" + dth));

                //������������� �� ����

                notifications.AddRangeNotNull(ReturnNotifications("type=0 AND lastmonthday=0 AND dayofmounth=0 AND dayofweek=0 AND Hour([timefrom])<=" + dth));

                //������������� �� ����������� ������� �������� z-�������
                notifications.AddRangeNotNull(ZMissingKKmEveryDayNotifications(now));

                //�������� ����������� �� �������� � z-������
                notifications.AddRangeNotNull(ReturnKKmEveryDayNotifications(now));


                if (notifications.Count > 0)
                {
                    notifications.Sort((a, b) => b.priority.CompareTo(a.priority));
                    GlobalNotifications.Instance.ShowInLabel(notifications.First().message);
                }
                return;

                #region msk

                if (CParam.AppCity == 2)
                {
                    _doc_id = _b.OrderCheck(m_teremok_id, 1);
                    if (_doc_id == 0)
                    {
                        if (dth >= 19)
                        {
                            labelinfo.Visible = true;
                            labelinfo.Text = "��������� �������  �����! \n �� �������� ��������� ������� �� 20:00 \n  ���������� �����!";
                        }
                    }
                    else
                    {
                        labelinfo.Visible = false;
                    }

                    _doc_id = _b.OrderCheck(m_teremok_id, 2);
                    if (_doc_id == 0)
                    {
                        if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                        {
                            labelinfo.Visible = true;
                            labelinfo.Text = "��������� �������  �����! \n �� �������� ��������� ������� �� 13:00 \n  ����� �� ������ ��������!";
                        }
                    }
                    else
                    {
                        labelinfo.Visible = false;
                    }

                    _doc_id = _b.OrderCheck(m_teremok_id, 10);
                    if (_doc_id == 0)
                    {
                        if (dt == dtc)
                        {
                            if (dth >= 20)
                            {
                                labelinfo.Visible = true;
                                labelinfo.Text = "��������� �������  �����! \n �� �������� ��������� ������� \n ����������� �������!";
                            }
                        }
                    }
                    else
                    {
                        labelinfo.Visible = false;
                    }

                    _doc_id = _b.OrderCheck(m_teremok_id, 10);
                    if (_doc_id == 0)
                    {
                        if (dt == 15)
                        {
                            if (dth >= 20)
                            {
                                labelinfo.Visible = true;
                                labelinfo.Text = "��������� �������  �����! \n �� �������� ��������� ������� \n ����������� �������!";
                            }
                        }
                    }
                    else
                    {
                        labelinfo.Visible = false;
                    }

                    _doc_id = _b.OrderCheck(m_teremok_id, 5);
                    if (_doc_id == 0)
                    {
                        if (dth >= 22)
                        {
                            labelinfo.Visible = true;
                            labelinfo.Text = "��������� �������  �����! \n ��������� �������� Z-�������!";
                        }
                    }
                    else
                    {
                        labelinfo.Visible = false;
                    }
                }
                #endregion


                #region piter 

                if (CParam.AppCity == 1)
                {
                    _doc_id = _b.OrderCheck(m_teremok_id, 1);
                    if (_doc_id == 0)
                    {
                        if (dth >= 19)
                        {
                            labelinfo.Visible = true;
                            labelinfo.Text = "��������� �������  �����! \n ������ �� ����� � ������������ \n ���������� ��������� �� 21:00";
                        }
                    }
                    _doc_id = _b.OrderCheck(m_teremok_id, 2);
                    if (_doc_id == 0)
                    {
                        if (dth >= 19)
                        {
                            labelinfo.Visible = true;
                            labelinfo.Text = "��������� �������  �����! \n ������ �� ����� � ������������ \n ���������� ��������� �� 21:00";
                        }
                    }
                    else
                    {
                        labelinfo.Visible = false;
                    }

                    _doc_id = _b.OrderCheck(m_teremok_id, 17);
                    if (_doc_id == 0)
                    {
                        if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                        {
                            if (dth >= 10)
                            {
                                labelinfo.Visible = true;
                                labelinfo.Text = "��������� �������  �����! \n �� �������� ��������� ������� �� 12:00 \n  ����� �� ������ ��������!";
                            }
                        }
                    }
                    else
                    {
                        labelinfo.Visible = false;
                    }

                    _doc_id = _b.OrderCheck(m_teremok_id, 5);
                    if (_doc_id == 0)
                    {
                        if (dth >= 22)
                        {
                            labelinfo.Visible = true;
                            labelinfo.Text = "��������� �������  �����! \n ��������� �������� Z-�������!";
                        }
                    }
                    else
                    {
                        labelinfo.Visible = false;
                    }
                }
                #endregion
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Log(exp.ToString());
            }
        }

        private void buttonItem2_Click(object sender, EventArgs e)
        {

            if (ribbonControl1.Expanded == false)
            {

                ribbonControl1.Expanded = true;
                buttonExpanded.Text = "  �������� ������";
                buttonExpanded.Image = Image.FromFile(CParam.AppFolder + "\\Img\\3uparrow.png");

                Form[] _forms = this.MdiChildren;

                foreach (Form _f in _forms)
                {
                    if (_f.Name == "FormDoc")
                    {
                        FormDoc _fd = (FormDoc)_f;
                        _fd.IninData();
                        break;
                    }
                }
            }
            else
            {
                ribbonControl1.Expanded = false;
                buttonExpanded.Text = "  ���������� ������";
                buttonExpanded.Image = Image.FromFile(CParam.AppFolder + "\\Img\\3dowarrow.png");

                Form[] _forms = this.MdiChildren;

                foreach (Form _f in _forms)
                {
                    if (_f.Name == "FormDoc")
                    {
                        FormDoc _fd = (FormDoc)_f;
                        _fd.IninData();
                        break;
                    }
                }
            }
        }

        private void toolStripButton_Invent_morning_Click(object sender, EventArgs e)
        {
            OpenOrder(27, 0);
        }

        private void toolStripButton_water2_Click(object sender, EventArgs e)
        {
            OpenOrder(26, 0);
        }

        private void DoItTest(string kkm_path, string m_kkm)
        {
            string minute = DateTime.Now.Minute.ToString();
            string round_minute = "";
            if (minute.Length == 2)
            {
                round_minute = minute;
            }
            else
            {
                round_minute = "0" + minute;
            }

            Log_kass("����� � " + m_kkm);
            Log_kass("����� ��������: " + DateTime.Now.Hour.ToString() + ":" + round_minute.ToString());
            Log_kass(" ");
            Log_kass("(+) - ���� �����");
            Log_kass("(-) - ���� �� ������");
            Log_kass("(!) - ���� �������");
            Log_kass("(*) - ���� �� ������");
            Log_kass("(?) - ������");
            Log_kass(" ");

            CBData _db = new CBData();
            DataTable _dt_item;
            DirectoryInfo _dir_folder = new DirectoryInfo(kkm_path + "/ccrs");

            WalkDirectoryTree(_dir_folder);
            _dt_item = _db.QueryTable();
            WriteLog(_dt_item, kkm_path);
        }

        public void CheckPath()
        {
            try
            {
                if (m_kkm1_online)
                {
                    SearchRoomCash(CParam.Kkm1Out);
                    //WalkCash(CParam.Kkm1Out);     // ��������� �������� ����
                }
                if (m_kkm2_online)
                {
                    SearchRoomCash(CParam.Kkm2Out);
                    //WalkCash(CParam.Kkm2Out);
                }
                if (m_kkm3_online)
                {
                    SearchRoomCash(CParam.Kkm3Out);
                    //WalkCash(CParam.Kkm3Out);
                }
                if (m_kkm4_online)
                {
                    SearchRoomCash(CParam.Kkm4Out);
                    //WalkCash(CParam.Kkm4Out);
                }
                if (m_kkm5_online)
                {
                    SearchRoomCash(CParam.Kkm5Out);
                    //WalkCash(CParam.Kkm5Out);
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        public void WalkCash(string kkm_path)
        {
            try
            {
                {
                    CTransfer _transfer = new CTransfer();

                    if (m_kkm1_online)
                    {
                        DoItTest(kkm_path.Substring(0, 13), m_kkm);
                    }
                    if (m_kkm2_online)
                    {
                        DoItTest(kkm_path.Substring(0, 13), m_kkm);
                    }
                    if (m_kkm3_online)
                    {
                        DoItTest(kkm_path.Substring(0, 13), m_kkm);
                    }
                    if (m_kkm4_online)
                    {
                        DoItTest(kkm_path.Substring(0, 13), m_kkm);
                    }
                    if (m_kkm5_online)
                    {
                        DoItTest(kkm_path.Substring(0, 13), m_kkm);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        public void WalkDirectoryTree(DirectoryInfo root)
        {
            string _name;
            string _hash_code;
            DateTime _date_time;

            try
            {
                foreach (var dirInfo in root.GetDirectories()) WalkDirectoryTree(dirInfo);
                {
                    DirectoryInfo _dir_sf = new DirectoryInfo(root.FullName);
                    foreach (FileInfo _file in _dir_sf.GetFiles())
                    {
                        if (!_dir_sf.Name.EndsWith("Log") || !_dir_sf.Name.EndsWith("Exchange"))
                        {
                            CalculateHash(_file.FullName);
                            _name = _file.Name;
                            _hash_code = c_hash;
                            _date_time = _file.CreationTime;
                            CBData _db = new CBData();

                            if (_db.FileSearch(_name))
                            {
                                if (_db.�ompareStandart(_name) == _hash_code)
                                {
                                    //��� ���������.
                                }
                                else
                                {
                                    Log_kass("(!) - " + _file.FullName);
                                }
                            }
                            else
                            {
                                Log_kass("(+) - " + _file.FullName);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Log_kass("(*) - " + exp.Message);
            }
        }

        private void CalculateHash(string _file)
        {
            try
            {
                FileStream fs = System.IO.File.Open(_file, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] hash = new byte[16];
                System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                hash = md5.ComputeHash(fs);
                c_hash = HashToString(hash);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Log_kass("(?) - " + exp.Message);
            }
            finally
            {

            }
        }

        private string HashToString(byte[] hash)
        {
            string ret = "";
            for (int i = 0; i < hash.Length; i++)
            {
                ret += String.Format("{0:X2}", hash[i]);
            }
            return ret;
        }

        protected void WriteLog(DataTable dt, string kkm_path)
        {
            try
            {
                // ��������� ������
                foreach (DataRow _row in dt.Rows)
                {
                    if (File.Exists(kkm_path + _row[0].ToString()))
                    {

                    }
                    else
                    {
                        Log_kass("(-) - " + _row[0].ToString());
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        public void Log_kass(string message)
        {
            TextWriter m_file = null;
            try
            {
                m_file = new StreamWriter(log_file + "_" + m_kkm + "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + ".log", true, System.Text.Encoding.GetEncoding(1251));
                m_file.WriteLine(message);
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
            }
            finally
            {
                if (m_file != null)
                    m_file.Close();
            }
        }

        private void backgroundWorker_WalkKkm_DoWork(object sender, DoWorkEventArgs e)
        {
            log.Debug("backgroundWorker_WalkKkm_DoWork started");
            m_thread_work = true;
            log.Debug("CheckKKM");
            CheckKKM();
            StaticHelperClass.ExecuteInvokeRequiredDelegate_WinForm<MDIParentMain>
                (this, (oo) => { ((MDIParentMain)oo).ShowKKMStatus(); });
            
            log.Debug("CheckPath");
#if(!DEB)
            //StaticHelperClass.ExecuteInvokeRequiredDelegate_WinForm<MDIParentMain>(this, (oo) => { ((MDIParentMain)oo).CheckPath(); });
            //CheckPath();
#endif
            m_cash_log = true;
            m_thread_work = false;
            m_cash_send_log = true;
            log.Debug("backgroundWorker_WalkKkm_DoWork finished");
        }

        private void timer_WalkKkm_Tick(object sender, EventArgs e)
        {
            if (IsWorkerEnabled(backgroundWorker_CheckKKM))
            {
                backgroundWorker_CheckKKM.RunWorkerAsync();
            }
        }

        public void ZReportParse(FileInfo file)
        {
            StreamReader _sr = null;
            try
            {
                string _line_or;
                string _line;

                char[] _separator_space = " ".ToCharArray();
                string _param;


                DateTime _check_datetime = DateTime.Now;

                _sr = new StreamReader(file.FullName, System.Text.Encoding.GetEncoding(1251));
                while ((_line_or = _sr.ReadLine()) != null)
                {
                    _line = MyTrim(_line_or);
                    string[] _s = _line.Split(_separator_space);
                    _param = _s[0];

                    switch (_param)
                    {
                        case "�����":
                            m_kkm = _s[_s.Length - 1];
                            break;
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                throw exp;
            }
            finally
            {
                if (_sr != null)
                    _sr.Close();
            }
        }

        public void SearchRoomCash(string kkm_folder_in)
        {
            try
            {
                CBData _data = new CBData();
                DirectoryInfo _dir = new DirectoryInfo(kkm_folder_in);
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    if (_file.Name.Substring(0, 1).ToLower() == "x")
                    {
                        ZReportParse(_file);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Log("������ " + exp.Message);
                throw exp;
            }
        }

        private void SendLogKKM(FtpSession m_client, string _teremok_folder)
        {
            try
            {
                DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder);
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    if (_file.Name.StartsWith("kkm"))
                    {
                        SendFileResuming(m_client, _file);

                        if (!Directory.Exists(CParam.AppFolder + "\\KKM_log\\"))
                            Directory.CreateDirectory(CParam.AppFolder + "\\KKM_log\\");
                        File.Move(_file.FullName, CParam.AppFolder + "\\KKM_log\\" + _file.Name);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        private string MyTrim(string str)
        {
            string res = "";
            bool _space = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].ToString() != " ")
                {
                    res = res + str[i];
                    _space = false;
                }
                else
                {
                    if (!_space)
                    {
                        res = res + str[i];
                        _space = true;
                    }
                }
            }
            return res.Trim();
        }

        private void buttonItemDetector_Click(object sender, EventArgs e)
        {
            FormNewDoc _form = new FormNewDoc();
            _form.MdiParent = this;
            _form.Show();
        }

        private void button_Mark_Click(object sender, EventArgs e)
        {
            WindowStarter.�������_������������_����_����(DateTime.Now);
        }

        private void button_Mark_ClickNM(object sender, EventArgs e)
        {
            WindowStarter.�������_������������_����_����(DateTime.Now.AddMonths(1));
        }


        internal void Tabel_open(DateTime tabel_date)
        {           
            #region header
            log.Trace("��������� ������");

            ARMWeb systemService = StaticConstants.WebService;
            log.Trace("���������������� ���������");
            Guid guid;string _guid;int _docID;
            CBData db = new CBData();
            BlokResult _bkr;BlokDoc _bk;
            
            int rec = 0;int countGetDocument = 0;
            #endregion

            #region ���� ������ ���������

            _guid = db.MarkCheck(m_teremok_id, 28, tabel_date);
            _docID = db.MarkDocID(m_teremok_id, 28, tabel_date);
            log.Trace("���� �������� ������ � ����");
            #endregion

            #region checkOpened Docs

            
            List<FormMark2> formm2_list = this.MdiChildren.OfType<FormMark2>().ToList();
            if (formm2_list.Count != 0)
            {
                List<FormMark2> formm2_opend_list = formm2_list.Where<FormMark2>(a => { return a._doc_id == _docID; }).ToList<FormMark2>();
                if (formm2_opend_list.Count != 0)
                {
                    formm2_opend_list[0].Activate();
                    return;
                }
            }
            log.Trace("��������� ������� �� ���� ������ �����");
            #endregion

            StaticConstants.IsTabelOpened = true;

            ProgressWorker pw = new ProgressWorker(this, "11", "���������, ���� �������� ������...");
            pw.Start();
            log.Trace("��������� progressworker");
            pw.ReportProgress(10);

            WorkEmployee[] we = null;
            #region get data from webserv
            try
            {
                try
                {         
                    {
                        log.Trace("��������� ���������� � ����");
                        OleDbConnection _conn = null;
                        _conn = new OleDbConnection(CParam.ConnString);
                        _conn.Open();
                        rec = 0;
                        log.Trace("������ �������� ��������");
                        
                        t_PropValue updationFlag = new t_PropValue().
                            SelectFirst<t_PropValue>("Prop_name='GetDocument' AND Prop_type='" + CParam.TeremokId + "'");

                        if (updationFlag != null)
                        {
                            int.TryParse(updationFlag.prop_value, out rec);
                        }
                        else
                        {
                            updationFlag = new t_PropValue() { prop_name = "GetDocument", prop_value = "0", prop_type = CParam.TeremokId };
                            updationFlag.Create();
                        }
                        log.Trace("���������� ������ �� ���������");
                        
                        PlanDocument[] plan = systemService.GetDocument(Convert.ToInt32(CParam.TeremokId), ref rec);
                        

                        log.Trace("����� �������");
                        
                        //�������� ������ ������� ��������� � 1�
                        try
                        {
                            updationFlag.prop_value = rec.ToString();
                            updationFlag.Update();

                        }catch(Exception ex)
                        {
                            Log(ex, "�� ������� �������� ���� ����������" + CParam.TeremokId);
                        }

                        log.Trace("�������� "+plan.Count()+" ����������");
                        foreach (var i in plan)
                        {
                            pw.ReportProgress(20);
                            //������� �������� ��� �� ��������
                            DateTime dt = i.Date;
                            log.Trace("������������ �������� �� ����� "+dt.ToShortDateString());
                            if (!(StaticConstants.Tabel_Opened_Date.Year == dt.Year && StaticConstants.Tabel_Opened_Date.Month == dt.Month))
                            {
                                log.Trace("��� �������� ��������� ���� �� ��������� ��������");
                                continue;
                            }

                            string hash = FormMark2.GetTabelHash(i);
                            //������� ID ��������� � ������� ��� ������������.
                            log.Trace("��������� �������� � ���� ������");

                            t_Doc doc = new t_Doc().SelectFirst<t_Doc>("doc_guid='" + i.ID.ToString() + "'");
                            
                            if(doc!=null)
                            {
                                if (doc.doc_hash != hash)
                                {
                                    int docID = doc.doc_id;
                                    db.DeleteMark(docID);
                                    we = i.ArrayWorkEmployee;
                                    foreach (var a in we)
                                    {
                                        //����� ����������
                                        db.ImportMarkEmp(a.Name.ToString(), a.Responsibility_Name.ToString(), a.Responsibility.ToString(), countGetDocument, docID);
                                        countGetDocument++;
                                        DayWork[] dw = a.ArrayDayWork;
                                        foreach (var c in dw)
                                        {
                                            if (c.Value == "" && c.Name == "") continue;
                                            db.ImportMarkEmpDetails(c.Number.ToString(), c.SmenaType.ToString(), c.Value.ToString(), c.Name.ToString(), c.FirstTime.Value.ToShortTimeString(), c.LastTime.Value.ToShortTimeString(), a.Name.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        log.Trace("���������� ��������");
                        pw.ReportProgress(30);
                    }
                }
                catch (Exception exp)
                {
                    Log("������ � ������� GetDocument");
                    log.Info("������ � ������� GetDocument");
                    log.Error("Exception: " + exp.Message, exp);
                }
            #endregion

                log.Trace("�������������� ���� ������");

                if (_guid == "")
                {
                    _bk = new BlokDoc { Date = tabel_date, ID = "0" };
                    _bkr = systemService.IsBlocked(Convert.ToInt32(CParam.TeremokId), _bk, true);   //���������� ��������� �� ������� ����������
                }
                else
                {
                    _bk = new BlokDoc { Date = tabel_date, ID = _guid };
                    _bkr = systemService.IsBlocked(Convert.ToInt32(CParam.TeremokId), _bk, true);   //���������� ��������� �� ������� ����������
                }


                if (_guid == "")
                {
                    guid = Guid.NewGuid();
                    db.OrderAddMark(28, m_teremok_id, Convert.ToString(guid), tabel_date);          //��������� �������� � ���� ������
                    _docID = db.MarkDocIDGuid(m_teremok_id, Convert.ToString(guid));
                }
                pw.ReportProgress(60);
                if (_bkr.Result == true)
                {
                    log.Trace("������� ���� ������");
                    FormMark2 _form = new FormMark2();
                    _form.MdiParent = this;
                    _form._doc_id = _docID;
                    _form.readDoc = false;
                    log.Trace("�������� ���������");
                    _form.Show();
                    log.Trace("�������");
                }
                if (_bkr.Result == false)
                {
                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show("�������� ������������ � 1�: " + _bkr.Name.ToString() + " \n������������� ���������, ������� ��� ������?\n�������������� �������� ��� �������� ��������� � 1�, ���� ��� ����� ��� �������������, ��������� ������ ������������.");
                    }
                    log.Trace("������� ���� ������");
                    StaticConstants.IsDocumentTabelBlocked = true;
                    FormMark2 _form = new FormMark2();
                    _form.MdiParent = this;
                    _form._doc_id = _docID;
                    _form.readDoc = true;
                    _form.Show();
                    log.Trace("������� ����");

                }
            }
            catch (Exception exp)
            {      
                log.Error("Exception: " + exp.Message, exp);
                Log(exp.ToString());
            }
            finally
            {
                pw.Stop();
            }
        }

        private void walkToLogs()
        {
            try
            {
                if (m_kkm1_online)
                {
                    string _pathToKKM = CParam.Kkm1In;
                    string[] _path = _pathToKKM.Split('\\');
                    SearchRoomCash(CParam.Kkm1Out);
                    copyLogOpos("\\\\" + _path[2] + "\\" + CParam.OPOSLog, m_kkm);
                    copyLogPos("\\\\" + _path[2] + "\\" + CParam.POSLog, m_kkm);
                    copyLogInpas("\\\\" + _path[2] + "\\" + CParam.InpasLog, m_kkm);
                    copyLogSmart("\\\\" + _path[2] + "\\" + CParam.SmartLog, m_kkm);
                    addToZip(m_kkm);
                }
                if (m_kkm2_online)
                {
                    string _pathToKKM = CParam.Kkm2In;
                    string[] _path = _pathToKKM.Split('\\');
                    SearchRoomCash(CParam.Kkm1Out);
                    copyLogOpos("\\\\" + _path[2] + "\\" + CParam.OPOSLog, m_kkm);
                    copyLogPos("\\\\" + _path[2] + "\\" + CParam.POSLog, m_kkm);
                    copyLogInpas("\\\\" + _path[2] + "\\" + CParam.InpasLog, m_kkm);
                    copyLogSmart("\\\\" + _path[2] + "\\" + CParam.SmartLog, m_kkm);
                    addToZip(m_kkm);
                }
                if (m_kkm3_online)
                {
                    string _pathToKKM = CParam.Kkm3In;
                    string[] _path = _pathToKKM.Split('\\');
                    SearchRoomCash(CParam.Kkm1Out);
                    copyLogOpos("\\\\" + _path[2] + "\\" + CParam.OPOSLog, m_kkm);
                    copyLogPos("\\\\" + _path[2] + "\\" + CParam.POSLog, m_kkm);
                    copyLogInpas("\\\\" + _path[2] + "\\" + CParam.InpasLog, m_kkm);
                    copyLogSmart("\\\\" + _path[2] + "\\" + CParam.SmartLog, m_kkm);
                    addToZip(m_kkm);
                }
                if (m_kkm4_online)
                {
                    string _pathToKKM = CParam.Kkm4In;
                    string[] _path = _pathToKKM.Split('\\');
                    SearchRoomCash(CParam.Kkm1Out);
                    copyLogOpos("\\\\" + _path[2] + "\\" + CParam.OPOSLog, m_kkm);
                    copyLogPos("\\\\" + _path[2] + "\\" + CParam.POSLog, m_kkm);
                    copyLogInpas("\\\\" + _path[2] + "\\" + CParam.InpasLog, m_kkm);
                    copyLogSmart("\\\\" + _path[2] + "\\" + CParam.SmartLog, m_kkm);
                    addToZip(m_kkm);
                }
                if (m_kkm5_online)
                {
                    string _pathToKKM = CParam.Kkm5In;
                    string[] _path = _pathToKKM.Split('\\');
                    SearchRoomCash(CParam.Kkm1Out);
                    copyLogOpos("\\\\" + _path[2] + "\\" + CParam.OPOSLog, m_kkm);
                    copyLogPos("\\\\" + _path[2] + "\\" + CParam.POSLog, m_kkm);
                    copyLogInpas("\\\\" + _path[2] + "\\" + CParam.InpasLog, m_kkm);
                    copyLogSmart("\\\\" + _path[2] + "\\" + CParam.SmartLog, m_kkm);
                    addToZip(m_kkm);
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                Exp(exp.Message);
            }
        }

        private void copyLogOpos(string _pathKKM, string _kkm)
        {
            try
            {
                DirectoryInfo _dir = new DirectoryInfo(_pathKKM);
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    if (_file.Name.EndsWith(".log"))
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\Temp\\" + _kkm + "\\FR"))
                            Directory.CreateDirectory(CParam.AppFolder + "\\Temp\\" + _kkm + "\\FR");
                        File.Copy(_file.FullName, CParam.AppFolder + "\\Temp\\" + _kkm + "\\FR\\" + _file.Name, true);
                        Log("���������� ���: " + _file.Name);
                        log.Info("���������� ���: " + _file.Name);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        private void copyLogPos(string _pathKKM, string _kkm)
        {
            try
            {
                DirectoryInfo _dir = new DirectoryInfo(_pathKKM);
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    if (_file.Name.EndsWith(".log"))
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\Temp\\" + _kkm + "\\ARM"))
                            Directory.CreateDirectory(CParam.AppFolder + "\\Temp\\" + _kkm + "\\ARM");
                        File.Copy(_file.FullName, CParam.AppFolder + "\\Temp\\" + _kkm + "\\ARM\\" + _file.Name, true);
                        Log("���������� ���: " + _file.Name);
                        log.Info("���������� ���: " + _file.Name);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        private void copyLogInpas(string _pathKKM, string _kkm)
        {
            try
            {
                DirectoryInfo _dir = new DirectoryInfo(_pathKKM);
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    if (_file.Name.EndsWith(".log"))
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\Temp\\" + _kkm + "\\Inpas"))
                            Directory.CreateDirectory(CParam.AppFolder + "\\Temp\\" + _kkm + "\\Inpas");
                        File.Copy(_file.FullName, CParam.AppFolder + "\\Temp\\" + _kkm + "\\Inpas\\" + _file.Name, true);
                        Log("���������� ���: " + _file.Name);
                        log.Info("���������� ���: " + _file.Name);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        private void copyLogSmart(string _pathKKM, string _kkm)
        {
            try
            {
                DirectoryInfo _dir = new DirectoryInfo(_pathKKM);
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    if (_file.Name.EndsWith(".log"))
                    {
                        if (!Directory.Exists(CParam.AppFolder + "\\Temp\\" + _kkm + "\\SC"))
                            Directory.CreateDirectory(CParam.AppFolder + "\\Temp\\" + _kkm + "\\SC");
                        File.Copy(_file.FullName, CParam.AppFolder + "\\Temp\\" + _kkm + "\\SC\\" + _file.Name, true);
                        Log("���������� ���: " + _file.Name);
                        log.Info("���������� ���: " + _file.Name);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
            }
        }

        private void addToZip(string _kkm)
        {
            try
            {
                using (ZipFile _zip = new ZipFile())
                {
                    _zip.AddDirectory(CParam.AppFolder + "\\Temp\\" + _kkm);
                    if (File.Exists(CParam.AppFolder + "\\LogKKM\\" + _kkm + "_" + DateTime.Today.ToShortDateString() + ".zip"))
                        File.Delete(CParam.AppFolder + "\\LogKKM\\" + _kkm + "_" + DateTime.Today.ToShortDateString() + ".zip");
                    _zip.Save(CParam.AppFolder + "\\LogKKM\\" + _kkm + "_" + DateTime.Today.ToShortDateString() + ".zip");
                    DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder + "\\Temp\\" + _kkm);
                    _dir.Delete(true);
                }
            }
            catch (Exception exp)
            {
                log.Error("Exception: " + exp.Message, exp);
                throw exp;
            }
        }

        private void ribbonControl1_Click(object sender, EventArgs e)
        {
        }

        private void MDIParentMain_Resize(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is FormHost)
                {
                    f.Dock = DockStyle.Fill;
                    //this.LayoutMdi(MdiLayout.TileVertical);
                    //Rectangle mdiClientArea = Rectangle.Empty;
                    //mdiClientArea = this.ClientRectangle;
                    //f.Bounds = mdiClientArea;

                   // f.WindowState = FormWindowState.Maximized;
                    //f.Width = this.ClientRectangle.Width - 20;
                    //f.Height = this.ClientRectangle.Height - 40;
                    continue;
                }
                if (f is FormMarochOtch)
                {
                    f.Dock = DockStyle.Fill;
                    //f.Width = this.ClientRectangle.Width-20;
                    //f.Height = this.ClientRectangle.Height-40;
                    continue;
                }

                if (f is HostForm)
                {
                    f.Width = this.ClientRectangle.Width;
                    f.Height = this.ClientRectangle.Height;
                    continue;
                }
                if (f is FormDoc)
                {
                    f.Dock = DockStyle.Fill;                
                    continue;
                }
                if(!(f is FormDoc)){
                f.Dock = DockStyle.Fill;                
                    //f.Width = this.ClientRectangle.Width;
                    //f.Height = this.ClientRectangle.Height;
                    continue;
                }
            }
        }


        private void MDIParentMain_Activated(object sender, EventArgs e)
        {
            
        }
    }
}
                        
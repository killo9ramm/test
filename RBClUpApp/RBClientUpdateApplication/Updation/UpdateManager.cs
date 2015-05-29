using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Config_classes;
using System.IO;

namespace RBClientUpdateApplication.Updation
{
    public enum ClientStates {Error=-1, CopingRBU = 1, CopiedRBU = 10, RBUEated = 100, CopingRBC = 1000, CopiedRBC = 10000, Finished = 100000 };

    public class UpdateManager
    {
     //   public string 

        public static MainWindow mainWindow;

        public static ArhivStates ArchState
        {
            get
            {
                return ArhiveManager.ArchiveState;
            }
        }
        public static ClientStates InitialState;

        internal static string PathToFolder;
        internal static string FolderName;
        internal static string RbUpdaterName;

        internal static int RBUCheckTimeout=5000;

        internal static string RbName;
        internal static byte[] RBUpdater_arr;
        
        private static byte[] _RBClient_arr=null;
        internal static byte[] RBClient_arr
        {
            get
            {
                if (_RBClient_arr == null)
                {
                    _RBClient_arr = File.ReadAllBytes(ArhiveManager.Archive.FullName);
                }
                return _RBClient_arr;
            }
        }

        public bool Stop = false;

        public static void StartUploadFile(MainWindow _mainWindow)
        {
            try
            {
                //выбор начальной стадии
                mainWindow = _mainWindow;

                Root.Log("Начали обновление армов");

                PathToFolder = ConfigClass.GetProperty("FtpArmFolder").ToString();
                FolderName = ConfigClass.GetProperty("OutputFolder").ToString();
                RbName = ConfigClass.GetProperty("RBClientArchiveName").ToString();
                int.TryParse(ConfigClass.GetProperty("RBUCheckTimeout").ToString(), out RBUCheckTimeout);

                
                InitialState = ClientStates.RBUEated;
                
                //if () //проверить есть ли файл если есть то копировать его 

                //создать потоки на копирование

                List<UpdateItem> list = Root.ItemsList.Where(a => a.RestControl.IsChecked).ToList();

                //создать классы updateClient
                List<UpdateClient> updcl_list = new List<UpdateClient>();
                list.ForEach(a =>
                {
                    UpdateClient updcl = new UpdateClient(a);
                    updcl_list.Add(updcl);
                });

                
                //добавить их в тредпул
                //updcl_list[0].Start(InitialState);

                mainWindow.progress1.Maximum = InitialState == ClientStates.RBUEated ? updcl_list.Count : updcl_list.Count * 2;

                updcl_list.ForEach(a =>
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(a.Start), InitialState);
                });
            }
            catch (Exception ex)
            {
                Root.Log(ex, "Не удалось запустить обновление");
            }
        }

        public static bool Started = false;

        public static void Start(MainWindow _mainWindow)
        {
            try
            {
                Started = true;
                //выбор начальной стадии
                mainWindow = _mainWindow;

                Root.Log("Начали обновление армов");

                PathToFolder = ConfigClass.GetProperty("FtpArmFolder").ToString();
                FolderName = ConfigClass.GetProperty("OutputFolder").ToString();
                RbName = ConfigClass.GetProperty("RBClientArchiveName").ToString();
                int.TryParse(ConfigClass.GetProperty("RBUCheckTimeout").ToString(), out RBUCheckTimeout);

                if (Root.RBUCopy)
                {
                    InitialState = ClientStates.CopingRBU;
                    string rbUpdater_path = ConfigClass.GetProperty("RBUpdateFile").ToString();
                    FileInfo fi = new FileInfo(rbUpdater_path);
                    RBUpdater_arr = File.ReadAllBytes(rbUpdater_path);
                    RbUpdaterName = fi.Name;

                }
                else
                {
                    InitialState = ClientStates.RBUEated;
                }

                //создать поток на архивацию
                Root.Log("Запускаем на архивацию");
                ArhiveManager.CreateArhiveThread();

                //создать потоки на копирование

                List<UpdateItem> list = Root.ItemsList.Where(a => a.RestControl.IsChecked).ToList();

                //создать классы updateClient
                List<UpdateClient> updcl_list = new List<UpdateClient>();
                list.ForEach(a =>
                {
                    UpdateClient updcl = new UpdateClient(a);
                    updcl_list.Add(updcl);
                });

                mainWindow.progress1.Maximum = InitialState == ClientStates.RBUEated ? updcl_list.Count : updcl_list.Count * 2;

                updcl_list.ForEach(a =>
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(a.Start), InitialState);
                });
            }catch(Exception ex)
            {
                Root.Log(ex,"Не удалось запустить обновление");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ftp;
using System.Threading;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace RBClient.Classes
{
     public class FtpServer
        {
         private FtpServer() { }
         public static FtpServer CreateServer(string ftpServer,string ftpLogin,string ftpPassword,int ftpPort)
         {
             if (ftpServer == "" || ftpLogin == "" || ftpServer == null || ftpLogin == null || ftpPort==-1)
             {
                 return null;
             }

             FtpServer server = new FtpServer();
             server.ftpServer = ftpServer;
             server.ftpLogin = ftpLogin;
             server.ftpPassword = ftpPassword;
             server.ftpPort = ftpPort;
             return server;
         }
            public string ftpServer{get;set;}
            public int ftpPort{get;set;}
            public string ftpLogin{get;set;}
            public string ftpPassword{get;set;}
        }
    class DownloadHelper
    {
        #region good
        private static Thread DownloadThread = null;
        public enum working_state { NotWorking, Working, Completed, Error }

        public delegate void StateChanged_(working_state state);
        public static StateChanged_ StateChanged;

        private static working_state _threadState = working_state.NotWorking;
        private static working_state threadState
        {
            get { return _threadState; }
            set
            {
                _threadState = value;
                Log("Изменен статус потока " + _threadState);
                if (StateChanged != null)
                {
                    StateChanged(value);
                }
            }
        }

        public delegate void MyDelegate(object message);
        public static MyDelegate Logg;

        public delegate void MyDelegate1(string message);
        public static MyDelegate1 Logg1;

        public delegate void PercentageChanged_(int percent);
        public static PercentageChanged_ PercentChanged;

        private static int _percentage=0;
        public static int Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                _percentage = value;
                if (PercentChanged != null)
                {
                    PercentChanged(_percentage);
                }
            }
        }


        private static string _Window_Header = "";
        private static string _fileName = "";
        private static string _ftpServer = "";
        private static string _ftpLogin = "";
        private static string _ftpPassword = "";
        private static int _ftpPort = 0;
        #endregion

        static WindowProgress win;

        /// <summary>
        /// Начинает асинхронную закачку файла из фтп
        /// </summary>
        public static void AsyncDownloadFromFtp(string Window_Header, string fileName, string ftpServer, int ftpPort, string ftpLogin, string ftpPassword)
        {
            _Window_Header = Window_Header; _fileName = fileName; _ftpLogin = ftpLogin; _ftpPassword = ftpPassword;
            _ftpServer = ftpServer; _ftpPort = ftpPort;

            if (DownloadThread == null || threadState!=working_state.Working)
                
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel
                    .ComponentResourceManager(typeof(MDIParentMain));
                win = new WindowProgress();
                win.Text = "Арм Ур. Идет скачивание файлов...";
                win.TopMost = false;
                win.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                win.ShowInTaskbar = true;
                win.StartPosition = FormStartPosition.CenterScreen;
                win.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                win.Header = Window_Header;
                win.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13);

                PercentChanged += delegate(int percent)
                {
                    try
                    {
                        if (win.InvokeRequired)
                        {
                            win.Invoke(new MethodInvoker(delegate
                            {
                                win.Header = Window_Header + " " + percent + "%";
                                win.Status = percent;
                            }));
                        }
                        else { win.Status = percent; }
                    }catch(Exception ex)
                    {
                        Log("AsyncDownloadFromFtp PercentChanged", ex);
                    }
                };

                StateChanged += delegate(working_state state)
                {
                    try
                    {
                        if (win.InvokeRequired)
                        {
                            win.Invoke(new MethodInvoker(delegate
                            {
                                if (state == working_state.Completed)
                                {
                                    win.Close();
                                }
                                if (state == working_state.Error)
                                {
                                    MessageBox.Show("Произошла ошибка при скачивании файлов! Обратитесь в тех.поддержку!");
                                }
                            }));
                        }
                    }catch(Exception ex)
                    {
                        Log("AsyncDownloadFromFtp StateChanged", ex);
                    }
                };

                Thread win_thread = new Thread(new ThreadStart(delegate
                {
                    try
                    {
                        if (win.InvokeRequired)
                        {
                            win.Invoke(new MethodInvoker(delegate
                            { win.ShowDialog(); }));
                        }
                        else
                        {
                            win.ShowDialog();
                        }
                    }catch(Exception ex){
                        Log("AsyncDownloadFromFtp Thread win_thread = new Thread(new ThreadStart(delegate", ex);
                    }
                }));
                win_thread.Name = "WinProgressThread";
                win_thread.Start();

                //создать новый поток и дать ему задание
                DownloadThread = new Thread(new ThreadStart (delegate{
                    threadState = working_state.Working;
                    StartDownload();
                    StaticConstants.MainWindow.ParseDoc();

                }));
                DownloadThread.Name = "FtpDownloadThread";
                DownloadThread.Start();
            }
        }

        /// <summary>
        /// Начинает асинхронную закачку файла из фтп
        /// </summary>
        public static void AsyncDownloadFromFtp(string Window_Header, string fileName, string teremok_folder, string ftpServer, int ftpPort, string ftpLogin, string ftpPassword)
        {
            _Window_Header = Window_Header; _fileName = fileName; _ftpLogin = ftpLogin; _ftpPassword = ftpPassword;
            _ftpServer = ftpServer; _ftpPort = ftpPort;

            if (DownloadThread == null || threadState != working_state.Working)
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel
                    .ComponentResourceManager(typeof(MDIParentMain));
                win = new WindowProgress();
                win.Text = "Арм Ур. Идет скачивание файлов...";
                win.TopMost = false;
                win.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                win.ShowInTaskbar = true;
                win.StartPosition = FormStartPosition.CenterScreen;
                win.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                win.Header = Window_Header;
                win.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13);

                PercentChanged += delegate(int percent)
                {
                    try
                    {
                        if (win.InvokeRequired)
                        {
                            win.Invoke(new MethodInvoker(delegate
                            {
                                win.Header = Window_Header + " " + percent + "%";
                                win.Status = percent;
                            }));
                        }
                        else { win.Status = percent; }
                    }
                    catch (Exception ex)
                    {
                        Log("AsyncDownloadFromFtp PercentChanged", ex);
                    }
                };

                StateChanged += delegate(working_state state)
                {
                    try
                    {
                        if (win.InvokeRequired)
                        {
                            win.Invoke(new MethodInvoker(delegate
                            {
                                if (state == working_state.Completed)
                                {
                                    win.Close();
                                }
                                if (state == working_state.Error)
                                {
                                    MessageBox.Show("Произошла ошибка при скачивании файлов! Обратитесь в тех.поддержку!");
                                }
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("AsyncDownloadFromFtp StateChanged", ex);
                    }
                };

                Thread win_thread = new Thread(new ThreadStart(delegate
                {
                    try
                    {
                        if (win.InvokeRequired)
                        {
                            win.Invoke(new MethodInvoker(delegate
                            { win.ShowDialog(); }));
                        }
                        else
                        {
                            win.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("AsyncDownloadFromFtp Thread win_thread = new Thread(new ThreadStart(delegate", ex);
                    }
                }));
                win_thread.Name = "WinProgressThread";
                win_thread.Start();

                //создать новый поток и дать ему задание
                DownloadThread = new Thread(new ThreadStart(delegate
                {
                    threadState = working_state.Working;
                    StartDownload(teremok_folder);
                    StaticConstants.MainWindow.ParseDoc();

                }));
                DownloadThread.Name = "FtpDownloadThread";
                DownloadThread.Start();
            }
        }
        

        #region logging
        public static void Log(string message)
        {
            if (null != Logg1)
            {
                Logg1(message);
            }
        }
        public static void Log(string message, Exception exp)
        {
            if (null != Logg1)
            {
                Logg1(message + " error: " + exp.Message);
            }
        }
        #endregion


        private static void StartDownload()
        {
            FtpSession m_client = new FtpSession();
            try
            {
                try
                {
                    m_client.Server = _ftpServer;
                    m_client.Port = _ftpPort;
                    m_client.Connect(_ftpLogin, _ftpPassword);
                    ExchangeFTP(m_client, _fileName);
                }
                catch (FtpException exp)
                {
                    Log("Не удалось подключиться к ftp серверу ", exp);
                }
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
                }
                catch (Exception exp)
                {
                    Log("Не удалось закрыть соединение с ftp сервером ", exp);
                }
            }
        }

        private static void StartDownload(string teremok_folder)
        {
            FtpSession m_client = new FtpSession();
            try
            {
                try
                {
                    m_client.Server = _ftpServer;
                    m_client.Port = _ftpPort;
                    m_client.Connect(_ftpLogin, _ftpPassword);
                    ExchangeFTP(m_client, _fileName, teremok_folder);
                }
                catch (FtpException exp)
                {
                    Log("Не удалось подключиться к ftp серверу ", exp);
                }
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
                }
                catch (Exception exp)
                {
                    Log("Не удалось закрыть соединение с ftp сервером ", exp);
                }
            }
        }

       

        private static bool GetFileFromFtpServer(FtpServer server,
            string teremok_folder, string file_Name, DirectoryInfo destinationFolder, bool deleteOnFtp)
        {
            FtpSession m_client = new FtpSession();
            bool file_downloaded = false;
            try
            {
                try
                {
                    m_client.Server = server.ftpServer;
                    m_client.Port = server.ftpPort;
                    m_client.Connect(server.ftpLogin, server.ftpPassword);
                    file_downloaded = ExchangeFTP(m_client, file_Name, teremok_folder, destinationFolder,deleteOnFtp);
                }
                catch (FtpException exp)
                {
                    Log("Не удалось подключиться к ftp серверу ", exp);
                }
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
                }
                catch (Exception exp)
                {
                    Log("Не удалось закрыть соединение с ftp сервером ", exp);
                }
            }
            return file_downloaded;
        }

        public static void StartDownload(string teremok_folder, string file_Name, DirectoryInfo destinationFolder, bool deleteOnFtp)
        {
            List<FtpServer> servers = GetFtpServers();
            Log("Начинаем загрузку " + file_Name);
            foreach (var a in servers)
            {
                if (GetFileFromFtpServer(a, teremok_folder, file_Name, destinationFolder,deleteOnFtp))
                    break;
            }
        }

        public static List<FtpServer> GetFtpServers()
        {
            List<FtpServer> list = new List<FtpServer>();
            for (int i = 1; i <= 4; i++)
            {
                string fs = StaticConstants.ReturnConfValue<string>("ftp_server"+i.ToString(), "");
                int fp = StaticConstants.ReturnConfValue<int>("ftp_port" + i.ToString(), -1);
                string fl = StaticConstants.ReturnConfValue<string>("ftp_login" + i.ToString(), "");
                string fpa = StaticConstants.ReturnConfValue<string>("ftp_pass" + i.ToString(), "");
                FtpServer fss=FtpServer.CreateServer(fs, fl, fpa, fp);
                if (fss != null)
                    list.Add(fss);

            }

            if (list.Count == 0)
                return null;
            return list;
        }

        private static void ExchangeFTP(FtpSession m_client, string filename)
        {
            Log("Thread ExchangeFTP started");
            string _teremok_folder = "";
            string _teremok_id;

            try
            {
                CBData _data = new CBData();

                DataTable _table_folders;
                DataTable _table_doc;

                _table_folders = _data.GetTeremokFolders();
                _teremok_folder = _table_folders.Rows[0][0].ToString();

                if (_teremok_folder == "") { Log("Папка теремка не выгрузилась из базы"); return; }

                Ftp.FtpDirectory rest_dir = m_client.CurrentDirectory.FindSubdirectory(_teremok_folder.ToLower());
                if (null != rest_dir)
                {
                    m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(_teremok_folder.ToLower());
                }
                else
                {
                    Log("Директория теремка не найдена на фтп folder: " + _teremok_folder.ToLower());
                    return;
                }


                Ftp.FtpDirectory in_dir = m_client.CurrentDirectory.FindSubdirectory("out");
                if (null != rest_dir)
                {
                    m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory("out");
                }
                else
                {
                    Log("Директория out не найдена на фтп в папке теремка folder: " + _teremok_folder.ToLower());
                    return;
                }


                List<String> names = new List<string>();
                VbEnumableCollection items = m_client.CurrentDirectory.Files;

                foreach (FtpFile item in items)
                {
                    if (item.Name == filename)
                    {
                        Log("Начинаем загрузку файла " + item.Name);
                        GetFileResuming(m_client, CParam.AppFolder + "\\inbox\\" + item.Name, item.Name);
                        names.Add(item.Name);
                        Log("Загружен файл " + item.Name);
                    }
                }


                foreach (String name in names)
                {
                        Log("Удаляем на FTP файл " + name);
                        m_client.CurrentDirectory.RemoveFile(name);
                        Log("Удален на FTP файл " + name);
                }
                threadState = working_state.Completed;
            }
            catch(IOException exp)
            {
                Log("Exception: " + exp, exp);
            }
            catch (Exception exp)
            {
                Log("Exception: " + exp, exp);
                threadState = working_state.Error;
            }
        }

        private static void ExchangeFTP(FtpSession m_client, string filename,string teremok_folder)
        {
            Log("Thread ExchangeFTP started");
            string _teremok_folder = "";
            string _teremok_id;

            try
            {
                CBData _data = new CBData();

                DataTable _table_folders;
                DataTable _table_doc;

                
                _teremok_folder = teremok_folder;

                if (_teremok_folder == "") { Log("Папка теремка не выгрузилась из базы"); return; }

                Ftp.FtpDirectory rest_dir = m_client.CurrentDirectory.FindSubdirectory(_teremok_folder.ToLower());
                if (null != rest_dir)
                {
                    m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(_teremok_folder.ToLower());
                }
                else
                {
                    Log("Директория теремка не найдена на фтп folder: " + _teremok_folder.ToLower());
                    return;
                }


                Ftp.FtpDirectory in_dir = m_client.CurrentDirectory.FindSubdirectory("out");
                if (null != rest_dir)
                {
                    m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory("out");
                }
                else
                {
                    Log("Директория out не найдена на фтп в папке теремка folder: " + _teremok_folder.ToLower());
                    return;
                }


                List<String> names = new List<string>();
                VbEnumableCollection items = m_client.CurrentDirectory.Files;

                foreach (FtpFile item in items)
                {
                    if (item.Name == filename)
                    {
                        Log("Начинаем загрузку файла " + item.Name);
                        GetFileResuming(m_client, CParam.AppFolder + "\\inbox\\" + item.Name, item.Name);
                        names.Add(item.Name);
                        Log("Загружен файл " + item.Name);
                    }
                }


                foreach (String name in names)
                {
                    Log("Удаляем на FTP файл " + name);
                    m_client.CurrentDirectory.RemoveFile(name);
                    Log("Удален на FTP файл " + name);
                }
                threadState = working_state.Completed;
            }
            catch (IOException exp)
            {
                Log("Exception: " + exp, exp);
            }
            catch (Exception exp)
            {
                Log("Exception: " + exp, exp);
                threadState = working_state.Error;
            }
        }

        private static bool ExchangeFTP(FtpSession m_client, string filename, string teremok_folder,
            DirectoryInfo destinationFolder, bool deleteOnFtp)
        {
            Log("Thread ExchangeFTP started "+Serializer.JsonSerialize(new object[5]{m_client.ControlChannel.Server,m_client.Port,filename,teremok_folder,destinationFolder.FullName}));
            string _teremok_folder = "";
            string _teremok_id;
            bool file_downloaded=false;

            try
            {
                CBData _data = new CBData();

                DataTable _table_folders;
                DataTable _table_doc;


                _teremok_folder = teremok_folder;

                if (_teremok_folder == "") { Log("Папка теремка не выгрузилась из базы"); return file_downloaded; }

                List<string> remote_folders = _teremok_folder.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                remote_folders.ForEach(a =>
                {
                    Ftp.FtpDirectory ftp_dir = m_client.CurrentDirectory.FindSubdirectory(a);
                    if (null != ftp_dir)
                    {
                        m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(a);
                    }
                    else
                    {
                        string message="Директория "+a+" не найдена на фтп в папке теремка folder: " + _teremok_folder.ToLower();
                        Log(message);
                        throw new Exception(message);
                    }
                });

                List<String> names = new List<string>();
                VbEnumableCollection items = m_client.CurrentDirectory.Files;

                foreach (FtpFile item in items)
                {
                    if (item.Name == filename)
                    {
                        Log("Начинаем загрузку файла " + item.Name);
                        GetFileResuming(m_client,Path.Combine(destinationFolder.FullName,item.Name), item.Name);
                        names.Add(item.Name);
                        Log("Загружен файл " + item.Name);
                        file_downloaded=true;
                    }
                }

                if (deleteOnFtp)
                {
                    foreach (String name in names)
                    {
                        Log("Удаляем на FTP файл " + name);
                        m_client.CurrentDirectory.RemoveFile(name);
                        Log("Удален на FTP файл " + name);
                    }
                }
                threadState = working_state.Completed;
            }
            catch (IOException exp)
            {
                Log("Exception: " + exp, exp);
            }
            catch (Exception exp)
            {
                Log("Exception: " + exp, exp);
                threadState = working_state.Error;
            }
            return file_downloaded;
        }


        private static void m_client_FileTransferProgress(object sender, IFtpFileTransferArgs args)
        {
            Percentage=args.TransferedPercentage;
        }

        private static void GetFileResuming(FtpSession m_client, String localPath, String name)
        {
            try
            {
                m_client.FileTransferProgress += new FtpFileEventHandler(m_client_FileTransferProgress); //changed file progress

                FtpFile ftpFile = m_client.CurrentDirectory.FindFile(name);
                if (ftpFile != null)
                {
                    long offset = 0;
                    string tmpName = localPath + ".tmp";
                    Log("tmpName='" + tmpName + "'");

                    Boolean needToTransfer = true;
                    // проверяем есть ли старый файл
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

                    if (needToTransfer)
                    {
                        // проверяем есть ли временный файл
                        FileInfo fiTmp = new FileInfo(tmpName);
                        if (fiTmp.Exists)
                        {
                            offset = fiTmp.Length;
                        }
                        if (offset < ftpFile.Size)
                        {
                            m_client.CurrentDirectory.GetFile(tmpName, name, offset);
                        }
                        // находим временный файл
                        fiTmp = new FileInfo(tmpName);
                        if (fiTmp.Exists)
                        {
                            // переименовываем в нормальное имя
                            Log("Переименовываем тмп файл " + fiTmp.Name);
                            fiTmp.MoveTo(localPath);
                            Log("Закончили! Файл " + localPath);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Log("Exception: " + exp, exp);
                throw exp;
            }
            finally
            {
                m_client.FileTransferProgress -= new FtpFileEventHandler(m_client_FileTransferProgress); //unbind changed file progress
            }
        }
    }
}

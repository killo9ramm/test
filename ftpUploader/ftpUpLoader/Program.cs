using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomLogger;
using Config_classes;
using System.Threading;
using System.IO;
using ZreportWork;
using RBClient.Classes;
using RBClient.Classes.CustomClasses;
using System.Text.RegularExpressions;
using Classes;
using Models;

namespace ftpUpLoader
{
    class Program
    {
        static Logger logger;
        static ConfigClass config;

        static void Main(string[] args)
        {
            logger = new Logger();
#if(DEBUG)

            //ConfigClass.LoadConfigFile("Config.xml");
            //outbox_action();

            //FtpUploader fu = new FtpUploader("a1.teremok.ru","333","ftpteremokspb","NthtvjrCG,2");
            //fu.LogEvent = Log;
            //fu.Connect();
            //string filename="1ee6102b-5b52-11e4-a98e-3085a9967c88.avi";
            //fu.GetFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename), filename, false);
            //return;
#endif

            
            ConfigClass.LoadConfigFile("Config.xml");
            Log("Старт");

            while (true)
            {
                outbox_action();
            }
        }



        private static void outbox_action()
        {
           
                DateTime now = DateTime.Now;
                //проверить не настало ли время проверки
                short inter;
                string str1 = ConfigClass.GetProperty("time_interval_work_min").ToString();

                inter = Int16.Parse(str1);
                bool exchange=true;


#if(DEBUG)
                //  PerformInboxAction();
                PerformExchange();
                //ОбработкаТотчета();
                //ОпроситьКассу();
              //  return;
#endif


                var times=ПолучитьВремяОбмена();

                foreach (TimeSpan a in times)
                {
                   
                    TimeSpan upltime = a;
                    if (now.Hour == upltime.Hours && now.Minute >= upltime.Minutes && now.Minute <= upltime.Minutes + inter)
                    {
                        exchange = true;
                        break;
                    }
                    else
                    {
                        exchange = false;
                    }
                }



                if (exchange)
                {
                    ОбработкаТотчета();
                    ОпроситьКассу();
                   
                    //проверить есть ли файлы
                    PerformInboxAction();
                    PerformExchange();
                    Thread.Sleep(ConfigClass.GetProperty<int>("uploader_rest_timeout", 300000));
                    //залить на фтп

                    //отправить в темп
                }
                else
                {
                    Thread.Sleep(ConfigClass.GetProperty<int>("uploader_rest_timeout", 300000));
                }
            
        }
        #region web serv kkm обработка
        private static void ОпроситьКассу()
        {
            try
            {
                if (ПораОтправитьОтчет("kkm_online_check_interval"))
                {
                    ОтправитьИнформациюОКассеНаВебсервис();
                }
            }catch(Exception ex)
            {
                Log(ex, "Ошибка в ОпроситьКассу");
            }
        }

        private static void ОбработкаТотчета()
        {
            try
            {
                if (ПораОтправитьОтчет("treport_check_interval"))
                {
                    ОтправтьТОтчет();
                }
            }catch(Exception ex)
            {
                Log(ex, "Ошибка в ОбработкаТотчета");
            }
        }
        
        public static string Kkm_num = "";

        public static t_Kkm KKM = new t_Kkm();

        private static void ОтправтьТОтчет()
        {
            string _in = ConfigClass.GetProperty<string>("kkm_in_folder",@"c:\in");
            string _out = Path.Combine(ConfigClass.GetProperty<string>("kkm_folder", @"c:\out"),
                ConfigClass.GetProperty<string>("work_folder", @"otchet"));

            FileInfo tFile=ПолучитьТОтчет(_in,_out);

            ОтправитьИнформациюОКассеНаВебсервис(tFile);
        }

        //отправить z-отчет
        private static void СообщитьОбОтправленномZОтчете(FileInfo file)
        {
            WebServiceExchanger wse = new WebServiceExchanger() { LogEvent = Log };
            wse.kkm_ConfirmZReportSended(file,DateTime.Now);
        }

        //отправить т-отчет
        private static void ОтправитьИнформациюОКассеНаВебсервис(FileInfo tFile)
        {
            WebServiceExchanger wse = new WebServiceExchanger() { LogEvent = Log };
            wse.SendTreportInfo(tFile);
        }
        //касса онлайн
        private static void ОтправитьИнформациюОКассеНаВебсервис()
        {
            WebServiceExchanger wse = new WebServiceExchanger() { LogEvent = Log };

            string _in = ConfigClass.GetProperty<string>("kkm_in_folder", @"c:\in");
            string _out = Path.Combine(ConfigClass.GetProperty<string>("kkm_folder", @"c:\out"),
                ConfigClass.GetProperty<string>("work_folder", @"otchet"));
            wse.kkm_UpdateOnlineState(_in,_out,true);
        }

        private static string ПолучитьНомерКассы(FileInfo tFile)
        {
            ReportHelper rph = new ReportHelper() { LogEvent = Log };
            string text = File.ReadAllText(tFile.FullName);
            return rph.GetKkmName(text);
        }

        private static FileInfo ПолучитьТОтчет(string _in,string _out)
        {
            ReportHelper rph = new ReportHelper() { LogEvent=Log};
            
#if(DEBUG)
            var treps=rph.GetTReports(new List<string> { _in }, new List<string> { _out }, false);
#else
            var treps=rph.GetTReports(new List<string> { _in }, new List<string> { _out }, true);
#endif

            treps.Sort((a, b) => b.CreationTime.CompareTo(a.CreationTime));
            if (treps.NotNullOrEmpty())
            {
                var trep=treps.First();
                DateTime dt=trep.CreationTime;
                treps.Remove(trep);

                var delt=treps.Where(a => (dt - a.CreationTime).TotalDays > 7).ToList();
                foreach(var del in delt)
                {
                    del.Delete();
                }

                KKM.last_treport = trep.Name;
                return trep;
            }
            return null;
        }

        public static DateTime LastDateTimeTrepChecked;
        private static bool ПораОтправитьОтчет(string param_name)
        {
            DateTime dt = DateTime.Now;
            if ((dt - LastDateTimeTrepChecked).TotalSeconds > ConfigClass.GetProperty<int>(param_name,1800000))
            {
                LastDateTimeTrepChecked = dt;
                return true;
            }
            return false;
        }

        #endregion

        private static List<TimeSpan> ПолучитьВремяОбмена()
        {
            List<TimeSpan> times = new List<TimeSpan>();

            List<string> str = (List<string>)ConfigClass.GetProperty("time_start_work");
            times = str.Select(a => TimeSpan.Parse(a)).ToList();

            times.Sort((a, b) => b.CompareTo(a));

            return times;
        }

        private static DirectoryInfo Temp_Dir = null;
        private static Regex Temp_Dir_reg = new Regex(@"\d{18}");

        static TaskManager tm = null;

        private static void PerformInboxAction()
        {
            FtpUploader uploader=null;
            DirectoryInfo currDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            DirectoryInfo menu_dir = new DirectoryInfo(Path.Combine(currDir.FullName, ConfigClass.GetProperty("menu_folder","menu").ToString()));
            DirectoryInfo files_menu_dir=null;
            DirectoryInfo dir_kkm_in = new DirectoryInfo(ConfigClass.GetProperty("kkm_in_folder",@"c:\in").ToString());

            try
            {
                Log("Получаем список параметров имен файлов");

                object file_mask = ConfigClass.GetProperty("file_mask1",@"(?i)(\d+)_menu_tu_(.*?)_(\d+).zip");

                if (file_mask != null)
                {
                    Regex reg = new Regex(file_mask.ToString());

                    Log("Подключаемся к фтп");

                    string server = ConfigClass.GetProperty("ftp_url").ToString();
                    string port = ConfigClass.GetProperty("ftp_port").ToString();
                    string username = ConfigClass.GetProperty("ftp_user").ToString();
                    string password = ConfigClass.GetProperty("ftp_password", false).ToString();

                    string remote_folder = ConfigClass.GetProperty("terem_name").ToString() + ConfigClass.GetProperty("ftp_download_folder").ToString();
                    Log("удаленная папка " + remote_folder);
                    List<string> remote_folders = remote_folder.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).ToList();


                    uploader = new FtpUploader(server, port, username, password);
                    uploader.LogEvent = Log;
                    uploader.Connect();

                    Log("Находим нужную папку");
                    Log("ищем файлы");
                    List<string> files = uploader.GetFileList(remote_folders);

                    files = files.Where(a => reg.IsMatch(a)).ToList();

                    if (files.Count > 0)
                    {
                        Log("Качаем файл");

                        if (tm != null && tm.IsWorking == true)
                        {
                            tm.Abort();
                        }

                        menu_dir.CreateOrReturn();
                        files_menu_dir = new DirectoryInfo(Path.Combine(menu_dir.FullName, reg.Match(files[0]).Groups[3].Value)).CreateOrReturn();
                        files_menu_dir.DeleteOldFilesInDir(0);

                        Log("Начинаем извлечение файлов");
                        FileInfo fi = uploader.GetFile(Path.Combine(files_menu_dir.FullName, files[0]), files[0],true);
                        ZippArhiveHelper.ArhiveManager.UnArchiveFile(fi, files_menu_dir);
                        Log("Файлы извлечены");
                        
                        fi.Delete();
                        Log("Удаляем архив из корневой директории");

                        if (tm == null)
                        {
                            Log("Запускаем обновление меню");
                            tm = new TaskManager(files_menu_dir, dir_kkm_in);
                            tm.LogEvent = Log;
                            tm.Go();
                        }
                        else
                        {
                            if (tm.IsWorking == true)
                            {
                                Log("Обновление меню уже запущено. Повторно не запускаем.");
                            }
                            else
                            {
                                Log("Запускаем обновление меню");
                                tm = new TaskManager(files_menu_dir, dir_kkm_in);
                                tm.LogEvent = Log;
                                tm.Go();
                            }
                        }
                    }

                    uploader.Close();
                }
            }catch(Exception ex)
            {
                Log(ex,"Произошла ошибка при скачивании файла");
                if(uploader!=null)
                    uploader.Close();
            }
        }
        private static void PerformExchange()
        {
            string error_step="";
            try
            {
                DirectoryInfo kkmdir = new DirectoryInfo(ConfigClass.GetProperty("kkm_folder").ToString());

                DirectoryInfo dir = new DirectoryInfo(Path.Combine(kkmdir.FullName, ConfigClass.GetProperty("work_folder").ToString()));

                List<FileInfo> files = dir.GetFiles().ToList();

                ОбработатьТемповыеСтарыеПапки();

                if (files.Count == 0)
                {
                    if (Temp_Dir != null && Temp_Dir.Exists && Temp_Dir.GetFiles().Length > 0)
                    {
                        bool clear_temp_dir1 = true;

                        //отправить по фтп
                        clear_temp_dir1 = ОтправитьСтарыеФайлыИзТемпа(clear_temp_dir1);
                    }
                    else
                    {
                        ОбработатьТемповыеСтарыеПапки();
                    }

                    return;
                }
                else
                {

                    DirectoryInfo back_dir = new DirectoryInfo(Path.Combine(kkmdir.FullName, ConfigClass.GetProperty("temp_folder").ToString()));
                    back_dir.CreateOrReturn();

                    Log("сделали папку back" + back_dir.FullName);

                    DirectoryInfo temp_dir =
#if(DEBUG)
 new DirectoryInfo(Path.Combine(@"\\10.8.1.31\Out\Z_ftpUploader", DateTime.Now.Ticks.ToString()));
#else
                        HomeDirectory.CreateSubdirectory(DateTime.Now.Ticks.ToString());
#endif
                        //new DirectoryInfo(DateTime.Now.Ticks.ToString());
                    temp_dir.CreateOrReturn();
                    Temp_Dir = temp_dir;
                    Log("сделали папку temp" + temp_dir.FullName);

                    files.Sort((a, b) => b.CreationTime.CompareTo(a.CreationTime));

                    Log("начинаем делать z");
                    ZreportMaker zmaker = new ZreportMaker();
                    zmaker.LogEvent = logger.Log;


                    object useShiftsNumbers = ConfigClass.GetProperty("use_shifts_numbers");
                    bool flag = false;
                    List<FileInfo> z_list = new List<FileInfo>();
                    if (useShiftsNumbers != null && bool.TryParse(useShiftsNumbers.ToString(), out flag))
                    {
                        z_list = zmaker.makeZreports(files, ConfigClass.GetProperty("terem_name").ToString() +
                            ConfigClass.GetProperty("z_perort_name_prefix").ToString(), temp_dir, back_dir, flag);
                    }
                    else
                    {
                        z_list = zmaker.makeZreports(files, ConfigClass.GetProperty("terem_name").ToString() +
                            ConfigClass.GetProperty("z_perort_name_prefix").ToString(), temp_dir, back_dir);
                    }


                    bool clear_temp_dir = true;

                    //отправить по фтп

                    clear_temp_dir = SendOnFtp(z_list, clear_temp_dir);

                    //удалить темповую директорию
                    if (clear_temp_dir)
                    {
                        temp_dir.Delete(true);
                    }
                }
            }catch(Exception ex)
            {
                Log("Ошибка формирования отчетов "+ex.Message);
            }
        }

        public static DirectoryInfo HomeDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        private static void ОбработатьТемповыеСтарыеПапки()
        {
            DirectoryInfo currDir =
#if(DEBUG)
 new DirectoryInfo(@"\\10.8.1.31\Out\Z_ftpUploader");
#else
                HomeDirectory;
#endif
                //new DirectoryInfo(Directory.GetCurrentDirectory());
            List<DirectoryInfo> tempsDir = currDir.GetDirectories().ToList().Where(a => Temp_Dir_reg.IsMatch(a.Name)).ToList();
            if (tempsDir.Count > 0)
            {
                tempsDir.ForEach(a =>
                {
                    bool clear_temp_dir2 = true;

                    //отправить по фтп
                    List<FileInfo> z_list2 = a.GetFiles().ToList();
                    
                    clear_temp_dir2 = SendOnFtp(z_list2, clear_temp_dir2);

                    //удалить темповую директорию
                    if (clear_temp_dir2)
                    {
                        a.Delete(true);
                    }
                });
            }
        }

        private static bool ОтправитьСтарыеФайлыИзТемпа(bool clear_temp_dir1)
        {
            List<FileInfo> z_list1 = Temp_Dir.GetFiles().ToList();
            clear_temp_dir1 = SendOnFtp(z_list1, clear_temp_dir1);

            //удалить темповую директорию
            if (clear_temp_dir1)
            {
                Temp_Dir.Delete(true);
                Temp_Dir = null;
            }
            return clear_temp_dir1;
        }

        private static bool SendOnFtp(List<FileInfo> z_list, bool clear_temp_dir)
        {
            try
            {
                if (!_SendOnFtp(z_list, clear_temp_dir))
                {
                    Log("_SendOnFtp error1");
                    bool flag = true;
                    foreach (var file in z_list)
                    {
                        flag = flag&&___SendOnFtp(file);
                    }
                    return flag;
                }
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        private static bool ___SendOnFtp(FileInfo file)
        {
            try
            {
                string server = ConfigClass.GetProperty("ftp_url").ToString();
                string port = ConfigClass.GetProperty("ftp_port").ToString();
                string username = ConfigClass.GetProperty("ftp_user").ToString();
                string password = ConfigClass.GetProperty("ftp_password", false).ToString();
                string remote_folder = "/" + ConfigClass.GetProperty("terem_name").ToString()+
                    ConfigClass.GetProperty("ftp_folder").ToString().Replace(@"\", "/");
                FtpHelperClass.SendFileOnServer(server, port, file, username, password, remote_folder);
                СообщитьОбОтправленномZОтчете(file);
                Log("file sended type 2 "+file.Name);
                return true;
            }
            catch (Exception ex1)
            {
                Log(ex1, "_SendOnFtp error2 " + file.FullName);
                return false;
            }
        }

        

        private static bool _SendOnFtp(List<FileInfo> z_list, bool clear_temp_dir)
        {
            Log("Пробуем отправить по ftp");
            if (z_list != null && z_list.Count > 0)
            {
                string server = ConfigClass.GetProperty("ftp_url").ToString();
                string port = ConfigClass.GetProperty("ftp_port").ToString();
                string username = ConfigClass.GetProperty("ftp_user").ToString();
                string password = ConfigClass.GetProperty("ftp_password",false).ToString();

                string remote_folder = ConfigClass.GetProperty("terem_name").ToString() + ConfigClass.GetProperty("ftp_folder").ToString();
                Log("удаленная папка" + remote_folder);

                List<string> remote_folders = remote_folder.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                FtpUploader uploader = new FtpUploader(server, port, username, password);
                uploader.LogEvent = Log;
                uploader.Connect();

                z_list.ForEach(a =>
                {
                    CustomAction ca = new CustomAction((o) =>
                    {
                        if (!uploader.SendFile(a, remote_folders, false))
                        {
                            throw new Exception("Не удалось отправить файл!");
                        }
                        else
                        {
                            СообщитьОбОтправленномZОтчете(a);
                        }
                    }, null);

                    ca.Start();

                    //проверка статуса
                    if (ca.State == StateEnum.Error || ca.State == StateEnum.ErrorTriesEnded)
                    {
                        Log("Отчет " + a.Name + " не отправлен!!");
                        clear_temp_dir = false;
                    }
                    else
                    {
                        Log("Отчет " + a.Name + " отправлен!! Статус: "+ ca.State.ToString());
                        a.Delete();
                    }
                });
                uploader.Close();
            }
            else
            {
                Log("Нет файлов в темповой директории");
            }
            return clear_temp_dir;
        }

        static void Log(string message)
        {
            lock (logger)
            {
                logger.Log(message);
            }
        }
        static void Log(Exception ex,string message)
        {
            lock (logger)
            {
                logger.Log(ex, message);
            }
        }
    }
}

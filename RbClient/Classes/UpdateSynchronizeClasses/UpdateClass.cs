using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CustomLogger;
using Debug_classes;
using RBClient.Classes.InternalClasses.Models;
using Config_classes;
using RBServer.Debug_classes;
using System.Threading;
using RBClient.ru.teremok.msk;
using System.Text.RegularExpressions;
using RBClient.Classes.DocumentClasses;

namespace RBClient.Classes.CustomClasses
{
    partial class UpdateClass : LoggerBase, IDisposable
    {
        /// <summary>
        /// Создает папку в руте
        /// </summary>
        /// <param name="dirName">имя целевой папки</param>
        public DirectoryInfo prepareDirectoryInRoot(string dirName,bool clear)
        {
            DirectoryInfo _folder = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), dirName));
            _folder.CreateOrReturn();

            if (clear && !_folder.IsEmpty())
            {
                _folder.DeleteOldFilesInDir(0);
                _folder.GetDirectories().ToList().ForEach(a => a.Delete(true));
            }
            return _folder;
        }

        string config_folder =StaticConstants.CONFIGS_FOLDER;
        t_Doc currentDocument = null;

        
        public void PosDisplayUnArchive(FileInfo arch_file)
        {
            try
            {
                string _folder_str = StaticConstants.POSDISPLAY_FOLDER;
                string pos_rb_config = StaticConstants.POSDISPLAY_CONFIG;
                

                DirectoryInfo folder=prepareDirectoryInRoot(_folder_str,true);

                //разархивировать архив
                //положить в папку в рбклиент
                ZipHelper.UnZipFile(arch_file.FullName, folder.FullName);

                //скопировать конфиги в папку Configs
                FileInfo config = folder.FindFile(pos_rb_config, false);

                if (config != null)
                {
                    DirectoryInfo con_folder = prepareDirectoryInRoot(config_folder, false);
                    config.MoveWithReplase(con_folder.FullName);
                }

                CTransfer ct = new CTransfer();
                currentDocument = ct.CreateDocumentRecieved(38, "Обновление для программы PosDisplay");
                //arch_file.Delete();
                StaticConstants.MainGridUpdate();

            }catch(Exception ex)
            {
                Log(ex, "Не удалось в PosDisplayUnArchive");
                throw (ex);
            }
        }

        public void PosDisplayLoadOnKKm()
        {
            #region документ
            t_Doc doc = new t_Doc().SelectLast<t_Doc>("doc_type_id=38");
            doc.doc_desc = "Синхронизация....";
            doc.Update();
            StaticConstants.MainGridUpdate();
            #endregion

            Log("Начали обновление посдисплеев на кассах");
            List<string> emark_folder_pathes = RbClientGlobalStaticMethods.ReturnPosRootFolders();

            Log("emark directories is "+Serializer.JsonSerialize(emark_folder_pathes));

            ////искать конфиг
            DirectoryInfo folder = prepareDirectoryInRoot(StaticConstants.POSDISPLAY_FOLDER, false);
            Log("local directory of pos is " + folder.FullName);

            if (folder.GetFiles().Length < 2)
            {
                string message="Файлов в папке posdisplay меньше 2х - обновление не производим";
                Log(message);
                throw new Exception(message);
            }

            //раскидать файлы на кассы
            if (emark_folder_pathes != null)
            {
                emark_folder_pathes.ForEach(a =>
                {
                    new CustomAction((o) =>
                    {
                        Log("Работаем с кассой "+a);
                        //остановить емарк на кассе
                        if (!stopPosOnKKm(a))
                        {
                            Log("Не могу остановить процесс на кассе " + a);
                            throw new Exception("Не могу остановить процесс на кассе "+a);
                        }

                        //скопировать файлы на кассу
                        //удалить папку на емарке
                        new CustomAction((oo) =>
                        {
                            DirectoryInfo emark_dir = new DirectoryInfo(a);
                            Log("Try to clear dir " + emark_dir.FullName);
                            emark_dir.ClearDirectory();
                        }, null).Start();

                        //залить файлы
                        folder.GetFiles("*.*", SearchOption.AllDirectories).ToList().ForEach(b =>
                        {
                            new CustomAction((oo) =>
                            {
                                string rel_path = 
                                    b.FullName.Substring(b.FullName.IndexOf(StaticConstants.POSDISPLAY_FOLDER) + StaticConstants.POSDISPLAY_FOLDER.Length);
                                string folders = "";
                                DebugPanel.getFileName(rel_path, ref folders, a);
                                string file_path = a + rel_path;
                                b.CopyTo(file_path, true);
                            }, null).Start();
                        });

                        //записать флаг запуска на кассу
                        new CustomAction((oo) =>
                        {
                            startPosOnKKm(a);
                        }, null) {Timeout=15000}.Start();
                        
 
                        //обновить в документе
                        
                        doc.doc_desc += " "+a+" обновлена!!";
                        doc.Update();
                        StaticConstants.MainGridUpdate();
                    }, null) {Timeout = 30000,MaxTries=3}.Start();

                });
                doc.doc_desc=doc.doc_desc.Replace("Синхронизация....", "Синхронизация завершена!!");
                doc.Update();
                StaticConstants.MainGridUpdate();
            }
        }

        public void copyPrKillerOnKKm(string kkm_path)
        {
            try
            {
                DirectoryInfo kkm_dir = new DirectoryInfo(kkm_path);
                Log("kkm_dir="+kkm_dir.FullName);
                kkm_dir = kkm_dir.Root;
                FileInfo killer_file = new FileInfo(Path.Combine(kkm_dir.FullName, StaticConstants.KILLER_PATH));
                Log("killer_file_exist=" + killer_file.Exists);
                if (!killer_file.Exists)
                {
                    Log("killer file is "+killer_file.FullName);
                    new CustomAction((o) =>
                    {

                        DirectoryInfo local_dir = RbClientGlobalStaticMethods.GetDirectory(StaticConstants.LOCAL_KILLER_DIRECTORY);//new DirectoryInfo(Path.Combine(kkm_dir.FullName, StaticConstants.LOCAL_KILLER_DIRECTORY)).CreateOrReturn();
                        //g("local_dir is="+local_dir.FullName);
                        //    RbClientGlobalStaticMethods.GetDirectory(StaticConstants.LOCAL_KILLER_DIRECTORY);
                        local_dir.CopyInternalFilesToDir(killer_file.Directory.FullName, true);

                        //скопировать ярлык в автозагрузку
                        FileInfo ink_file = new FileInfo(Path.Combine(local_dir.FullName, StaticConstants.KILLER_INC_NAME));
                        ink_file.CopyTo(Path.Combine(Path.Combine(killer_file.Directory.Root.FullName,
                            StaticConstants.AUTO_LOAD_PATH_XP), ink_file.Name), true);

                    }, null) { LogEvent=new NLogDelegate((o)=>Log(o.ToString()))}.Start();
                }
            }catch(Exception ex){
                Log(ex,"Не удалось скопировать киллер на кассы");
            }
        }

        private bool stopPosOnKKm(string kkm_path)
        {
            //проверить программу доступность программы киллера на кассе
            DirectoryInfo kkm_dir = new DirectoryInfo(kkm_path);
            kkm_dir = kkm_dir.Root;
            FileInfo killer_file=new FileInfo(Path.Combine(kkm_dir.FullName,StaticConstants.KILLER_PATH));

            if (!killer_file.Exists)
            {
                //если её нет - скопировать
                copyPrKillerOnKKm(kkm_path);

                throw new Exception("Файлы на кассу скопированы, нужно ждать до её перезагрузки "+ kkm_path);
            }
            else
            {
                
                //остановка процесса posmonitor

                CustomAction act1=new CustomAction((o) =>
                {
                    //если прога есть то записать флаг остановки в конфиг
                    DirectoryInfo remote_killer_dir = killer_file.Directory;

                    ConfigClass killerconfig = new ConfigClass(
                        remote_killer_dir.PFileIn(StaticConstants.KILLER_CONFIG_NAME));
                    killerconfig.SetProperty(StaticConstants.KILLER_STOP_FLAG_NAME,"1");

                    StaticConstants.KILLER_OLD_TIMEOUT_PERIOD=killerconfig.GetProperty(StaticConstants.KILLER_TIMEOUT_MS_NAME).ToString();

                    killerconfig.SetProperty(StaticConstants.KILLER_TIMEOUT_MS_NAME,
                        StaticConstants.KILLER_TIMEOUT_MS_VALUE);

                    Thread.Sleep(int.Parse(StaticConstants.KILLER_OLD_TIMEOUT_PERIOD));
                    
                    // проверить отработал ли флаг

                    CustomAction act2 = new CustomAction((oo) =>
                    {
                        string stop_prop_val=killerconfig.GetProperty(StaticConstants.KILLER_STOP_FLAG_NAME).ToString();
                        if (stop_prop_val != "0") throw new Exception("Процесс еще не убит");
                    }, null) {Timeout=int.Parse(StaticConstants.KILLER_TIMEOUT_MS_VALUE)};act2.Start();

                    if (!act2.IsSuccess())
                    {
                        Log("Процесс не удалось убить на кассе " + kkm_dir.FullName);
                        throw new Exception("Процесс не удалось убить на кассе " + kkm_dir.FullName);
                    }

                }, null);act1.Start();

                if (!act1.IsSuccess()) return false;
            }
            return true;
        }

        private void startPosOnKKm(string kkm_path)
        {
            DirectoryInfo kkm_dir = new DirectoryInfo(kkm_path);
            kkm_dir = kkm_dir.Root;
            FileInfo killer_file = new FileInfo(Path.Combine(kkm_dir.FullName, StaticConstants.KILLER_PATH));
            CustomAction act1 = new CustomAction((o) =>
            {
                //если прога есть то записать флаг остановки в конфиг
                DirectoryInfo remote_killer_dir = killer_file.Directory;

                ConfigClass killerconfig = new ConfigClass(
                    remote_killer_dir.PFileIn(StaticConstants.KILLER_CONFIG_NAME));

                killerconfig.SetProperty(StaticConstants.KILLER_START_FLAG_NAME, "1");
                killerconfig.SetProperty(StaticConstants.KILLER_TIMEOUT_MS_NAME, StaticConstants.KILLER_OLD_TIMEOUT_PERIOD);

                Log("Процесс PosMonitor запущен на кассе "+kkm_path);

            }, null); act1.Start();


            if (!act1.IsSuccess()) Log("Процесс PosMonitor не удалось запустить на кассе " + kkm_path);
        }

        public static List<string> return_folders_in_kkm_pathes(string folder_name)
        {
            MDIParentMain.Log(String.Format("Определяем список касс"));
            if (CParam.Kkm1In == "") { MDIParentMain.Log(String.Format("Инициализируем параметры")); CParam.Init(); }

            List<string> kkmInList = RbClientGlobalStaticMethods.ReturnKKmInPathes();
            List<string> kkm_list = new List<string>();

            kkmInList.ForEach(a => kkm_list.Add(ReturnFolderInKKm(a, folder_name)));

            MDIParentMain.Log(String.Format("Определили список касс ={0}", kkm_list.Count));
            return kkm_list;
        }

        private static string ReturnFolderInKKm(string kkm_path,string folder_name)
        {
            if (kkm_path == "") return "";
            DirectoryInfo dir = new DirectoryInfo(kkm_path);
            dir = dir.Root;
            dir=dir.FindDirectory(folder_name, true);

            if (dir == null) return "";
            else
                return dir.FullName;
        }



        #region kkm updation
        /// <summary>
        /// Обновить статус кассы. Приоритет 30.
        /// </summary>
        /// <param name="kkmin"></param>
        /// <param name="kkmout"></param>
        /// <param name="online"></param>
        public static void kkm_UpdateOnlineState(string kkmin, string kkmout,bool online)
        {
            try
            {
                t_Kkm kkm = CheckKKmExist(kkmout);
                ModelItemClass mc = new ModelItemClass(kkm);
                if (online)
                    kkm.lasttime_online = DateTime.Now;

                if (kkm.in_folder != kkmin)
                    kkm.in_folder = kkmin;

                kkm.online = online;

                CheckKKmWorkedToDay(DateTime.Now, kkm);

                mc.DBUpdate(() =>
                {
                    StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "SendKKmZInfo",
                                                    new object[] { StaticConstants.Main_Teremok_1cName,
                        kkm.kkm_name, DateTime.Now, kkm.workedToDay, kkm.lasttime_online},
                                                    30);
                });

            }catch(Exception ex)
            {
                MDIParentMain.Log(ex, "Ошибка в kkm_UpdateOnlineState не удалось обновить kkm_name");
            }
        }

        [Obsolete]
        public static void kkm_UpdateOnlineState_(string kkmin, string kkmout, bool online)
        {
            try
            {
                t_Kkm kkm = CheckKKmExist(kkmout);
                ModelItemClass mc = new ModelItemClass(kkm);
                if (online)
                    kkm.lasttime_online = DateTime.Now;

                if (kkm.in_folder != kkmin)
                    kkm.in_folder = kkmin;

                kkm.online = online;

                CheckKKmWorkedToDay(DateTime.Now, kkm);

                mc.DBUpdate(() =>
                {
                    new CustomAction(o =>
                    {
                        string st = StaticConstants.WebServiceSystem.SendKKmZInfo(StaticConstants.Main_Teremok_1cName,
                            kkm.kkm_name, DateTime.Now, kkm.workedToDay, kkm.lasttime_online);
                        if (st.IndexOf("1") == -1) throw new Exception("kkm_UpdateOnlineState package not delivered");
                    }, null)
                    {
                        Timeout = 1000,
                        LogEvent = (o) => { MDIParentMain.Log(o.ToString()); }
                    }.Start();
                });

            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "Ошибка в kkm_UpdateOnlineState не удалось обновить kkm_name");
            }
        }

        public static t_Kkm CheckKKmExist(string kkmout)
        {
            t_Kkm kkm = new t_Kkm().SelectFirst<t_Kkm>(
            "out_folder='" + kkmout + "'");

            if (kkm == null)
            {
                kkm = new t_Kkm() {out_folder = kkmout };
                kkm.Create();
            }

            return kkm;
        }

        /// <summary>
        /// Обновить кассу по приходу т-репорта. Приоритет 40.
        /// </summary>
        /// <param name="kkmout"></param>
        /// <param name="trep_file"></param>
        public static void kkm_UpdateTReport(string kkmout, FileInfo trep_file)
        {
            try
            {
                t_Kkm kkm = CheckKKmExist(kkmout);
                ModelItemClass mc = new ModelItemClass(kkm);

                string text = System.IO.File.ReadAllText(trep_file.FullName, Encoding.GetEncoding(1251));
                kkm.kkm_name = GetKkmName(text);
                string shift_num=GetShiftNum(text);
                int shift_num_int=0;

                GetLastCheckInfo(text, ref kkm.last_check_datetime, ref kkm.last_check_num);
                CheckKKmWorkedToDay(DateTime.Now, kkm);

                kkm.last_treport = trep_file.Name;

                mc.DBUpdate(() =>
                {
                    MDIParentMain.Log(String.Format("Tkkm_UpdateTReport kkm:'{0}' shift:'{1}'", kkm.kkm_name, shift_num));

                    if (int.TryParse(shift_num, out shift_num_int))
                    {
                        StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "SendKKmZInfoSM",
                                new object[] { StaticConstants.Main_Teremok_1cName,
                        kkm.kkm_name, DateTime.Now, kkm.workedToDay, kkm.lasttime_online, shift_num_int},
                                40);
                    }
                    else
                    {
                        MDIParentMain.Log(String.Format("Ошибка парсинга kkm_UpdateTReport shift_num:'{0}'",shift_num));
                        StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "SendKKmZInfo",
                                new object[] { StaticConstants.Main_Teremok_1cName,
                        kkm.kkm_name, DateTime.Now, kkm.workedToDay, kkm.lasttime_online},
                                40);
                    }
                });

            }catch(Exception ex)
            {
                MDIParentMain.Log(ex, "Ошибка в kkm_UpdateTReport не удалось обновить kkm_name");
            }
        }

        [Obsolete]
        public static void kkm_UpdateTReport_(string kkmout, FileInfo trep_file)
        {
            try
            {
                t_Kkm kkm = CheckKKmExist(kkmout);
                ModelItemClass mc = new ModelItemClass(kkm);

                string text = System.IO.File.ReadAllText(trep_file.FullName, Encoding.GetEncoding(1251));
                kkm.kkm_name = GetKkmName(text);
                string shift_num = GetShiftNum(text);
                int shift_num_int = 0;

                GetLastCheckInfo(text, ref kkm.last_check_datetime, ref kkm.last_check_num);
                CheckKKmWorkedToDay(DateTime.Now, kkm);

                kkm.last_treport = trep_file.Name;

                mc.DBUpdate(() =>
                {
                    new CustomAction(o =>
                    {
                        string st = "";
                        if (int.TryParse(shift_num, out shift_num_int))
                        {
                            st = StaticConstants.WebServiceSystem.SendKKmZInfoSM(StaticConstants.Main_Teremok_1cName,
                            kkm.kkm_name, DateTime.Now, kkm.workedToDay, kkm.lasttime_online, shift_num_int);
                        }
                        else
                        {
                            st = StaticConstants.WebServiceSystem.SendKKmZInfo(StaticConstants.Main_Teremok_1cName,
                            kkm.kkm_name, DateTime.Now, kkm.workedToDay, kkm.lasttime_online);
                        }

                        MDIParentMain.Log("kkm_UpdateTReport " + st);
                        if (st.IndexOf("1") == -1) throw new Exception("kkm_UpdateTReport package not delivered");
                    }, null)
                    {
                        Timeout = 1000,
                        LogEvent = (o) => { MDIParentMain.Log(o.ToString()); }
                    }.Start();
                });

            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "Ошибка в kkm_UpdateTReport не удалось обновить kkm_name");
            }
        }

        /// <summary>
        /// Обновить кассу по приходу z-репорта. Приоритет 70
        /// </summary>
        /// <param name="kkmout"></param>
        /// <param name="trep_file"></param>
        internal static void kkm_UpdateZReport(string kkmout, FileInfo zrep_file)
        {
            try
            {
                t_Kkm kkm = CheckKKmExist(kkmout);
                ModelItemClass mc = new ModelItemClass(kkm);

                string text = System.IO.File.ReadAllText(zrep_file.FullName, Encoding.GetEncoding(1251));
                kkm.kkm_name = GetKkmName(text);
                
                string shift_num = GetShiftNum(text);
                int shift_num_int = 0;

                GetLastCheckInfo(text, ref kkm.last_check_datetime, ref kkm.last_check_num);
                CheckKKmWorkedToDay(DateTime.Now, kkm);
                kkm.last_zreport = zrep_file.Name;

                if (kkm.workedToDay)
                {
                    mc.DBUpdate(() =>
                    {
                        if (int.TryParse(shift_num, out shift_num_int))
                        {
                            StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "SendKKmZInfoZSM",
                                new object[] { StaticConstants.Main_Teremok_1cName, 
                                                kkm.kkm_name, DateTime.Now, kkm.last_zreport, kkm.last_check_datetime, shift_num_int},
                                70);
                        }
                        else
                        {
                            StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "SendKKmZInfoZ",
                                new object[] { StaticConstants.Main_Teremok_1cName,
                                                kkm.kkm_name, DateTime.Now, kkm.last_zreport, kkm.last_check_datetime},
                                70);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "Ошибка в kkm_UpdateZReport не удалось обновить kkm_name");
            }
        }

        [Obsolete]
        internal static void kkm_UpdateZReport_(string kkmout, FileInfo zrep_file)
        {
            try
            {
                t_Kkm kkm = CheckKKmExist(kkmout);
                ModelItemClass mc = new ModelItemClass(kkm);

                string text = System.IO.File.ReadAllText(zrep_file.FullName, Encoding.GetEncoding(1251));
                kkm.kkm_name = GetKkmName(text);

                string shift_num = GetShiftNum(text);
                int shift_num_int = 0;

                GetLastCheckInfo(text, ref kkm.last_check_datetime, ref kkm.last_check_num);
                CheckKKmWorkedToDay(DateTime.Now, kkm);
                kkm.last_zreport = zrep_file.Name;

                if (kkm.workedToDay)
                {
                    mc.DBUpdate(() =>
                    {
                        new CustomAction(o =>
                        {
                            string result_str = "";
                            if (int.TryParse(shift_num, out shift_num_int))
                            {
                                result_str = StaticConstants.WebServiceSystem.SendKKmZInfoZSM(StaticConstants.Main_Teremok_1cName,
                                kkm.kkm_name, DateTime.Now, kkm.last_zreport, kkm.last_check_datetime, shift_num_int);
                            }
                            else
                            {
                                result_str = StaticConstants.WebServiceSystem.SendKKmZInfoZ(StaticConstants.Main_Teremok_1cName,
                                kkm.kkm_name, DateTime.Now, kkm.last_zreport, kkm.last_check_datetime);

                            }
                            if (result_str.IndexOf("1") == -1) throw new Exception("kkm_UpdateZReport package not delivered");
                        }, null)
                        {
                            Timeout = 1000,
                            LogEvent = (o) => { MDIParentMain.Log(o.ToString()); }
                        }.Start();

                    });
                }
            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "Ошибка в kkm_UpdateZReport не удалось обновить kkm_name");
            }
        }

        /// <summary>
        /// Подтверждение что z-отчет отправлен. 50 приоритет
        /// </summary>
        /// <param name="zname">имя z-отчета</param>
        /// <param name="ToDay">текущая дата</param>
        internal static void kkm_ConfirmZReportSended(FileInfo zrep_file, DateTime ToDay)
        {
            try
            {
                string[] z_file = GetZInfoFromZname(zrep_file.Name);
                if (z_file == null) return;


                StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "ConfirmZreportSended",
                    new object[] { StaticConstants.Main_Teremok_1cName, z_file[3], ToDay, z_file[1], true},
                    50);

                //отправить отчет на вебсервис
                SendZreportOnWebService(zrep_file, ToDay, z_file);

            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "Ошибка в kkm_ConfirmZReportSended");
            }
        }

        [Obsolete]
        internal static void kkm_ConfirmZReportSended_(FileInfo zrep_file, DateTime ToDay)
        {
            try
            {
                string[] z_file = GetZInfoFromZname(zrep_file.Name);
                if (z_file == null) return;

                //отправить подтверждение
                new CustomAction(o =>
                {
                    var result_str = StaticConstants.WebServiceSystem.ConfirmZreportSended
                        (StaticConstants.Main_Teremok_1cName, z_file[3], ToDay, z_file[1], true);
                    if (result_str.IndexOf("1") == -1) throw new Exception("kkm_ConfirmZReportSended package not delivered");

                }, null)
                {
                    Timeout = 1000,
                    LogEvent = (o) => { MDIParentMain.Log(o.ToString()); }
                }.Start();

                //отправить отчет на вебсервис
                SendZreportOnWebService(zrep_file, ToDay, z_file);

            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "Ошибка в kkm_ConfirmZReportSended");
            }
        }

        /// <summary>
        /// Отправить z-отчет на вебсервис. 60 приоритет
        /// </summary>
        /// <param name="zrep_file"></param>
        /// <param name="ToDay"></param>
        /// <param name="z_file"></param>
        private static void SendZreportOnWebService(FileInfo zrep_file, DateTime ToDay, string[] z_file)
        {
            try
            {
                int kkm_num = int.Parse(z_file[3]);
                string shift = GetZInfoFromZname(zrep_file.Name)[4];
                int shift_num = int.Parse(shift);

                StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "RecieveZReport",
                    new object[] { StaticConstants.Main_Teremok_1cName, kkm_num, ToDay, z_file[1], shift_num, System.IO.File.ReadAllBytes(zrep_file.FullName) },
                    60);

            }catch(Exception ex){
                 MDIParentMain.Log(ex, "Ошибка в SendZreportOnWebService");
            }
        }

        [Obsolete]
        private static void SendZreportOnWebService_(FileInfo zrep_file, DateTime ToDay, string[] z_file)
        {
            try
            {
                int kkm_num = int.Parse(z_file[3]);
                string shift = GetZInfoFromZname(zrep_file.Name)[4];
                int shift_num = int.Parse(shift);
                new CustomAction(o =>
                {
                    var result_str = StaticConstants.WebServiceSystem.RecieveZReport
                        (StaticConstants.Main_Teremok_1cName, kkm_num, ToDay, z_file[1], shift_num, System.IO.File.ReadAllBytes(zrep_file.FullName));

                    if (result_str.IndexOf("1") == -1) throw new Exception("SendZreportOnWebService package not delivered");

                }, null)
                {
                    Timeout = 1000,
                    LogEvent = (o) => { MDIParentMain.Log(o.ToString()); }
                }.Start();
            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "Ошибка в SendZreportOnWebService");
            }
        }

        /// <summary>
        /// Возвращает информацию по z-отчету
        ///0: ATRIU
        ///1: X140911999.071
        ///2: 140911
        ///3: 999
        ///4: 071
        /// </summary>
        /// <param name="zrep_file"></param>
        /// <returns></returns>
        private static string[] GetZInfoFromZname(string zrep_file)
        {
            string regg=StaticConstants.RBINNER_CONFIG.GetProperty<string>("z_report_name_reg"
                , @"(\S+?)_(X(\d{6})(\S+?)[.](\S+?))[.]zip");
            Regex reg=new Regex(regg);
            if (reg.IsMatch(zrep_file))
            {
                string[] st = new string[5];
                Match m=reg.Match(zrep_file);
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

        private static void CheckKKmWorkedToDay(DateTime ToDay, t_Kkm kkm)
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

        public static string GetKkmName(string z_rep_text)
        {
            string result = "";

            Regex reg = new Regex(
                   StaticConstants.RBINNER_CONFIG.GetProperty<string>("kkm_in_treport_reg", @"Касса (\d+)"));
            if (reg.IsMatch(z_rep_text))
            {
                result = reg.Match(z_rep_text).Groups[1].Value.Trim();
            }
            return result;
        }

        public static int GetKkmName(FileInfo z_rep)
        {
            string result = "";
            string z_rep_text = System.IO.File.ReadAllText(z_rep.FullName, Encoding.GetEncoding(1251));

            Regex reg = new Regex(
                   StaticConstants.RBINNER_CONFIG.GetProperty<string>("kkm_in_treport_reg", @"Касса (\d+)"));
            if (reg.IsMatch(z_rep_text))
            {
                result = reg.Match(z_rep_text).Groups[1].Value.Trim();
            }

            int kkm = -1;
            if (int.TryParse(result, out kkm))
                return kkm;
            else
            {
                MDIParentMain.Log("Ошибка в GetKkmName, не парсится номер кассы <" + result + ">");
                return -1;
            }
        }

        private static string GetShiftNum(string z_rep_text)
        {
            string result = "";

            Regex reg = new Regex(
                   StaticConstants.RBINNER_CONFIG.GetProperty<string>("shiftnum_in_treport_reg", @"Смена (\d+)"));
            if (reg.IsMatch(z_rep_text))
            {
                result = reg.Match(z_rep_text).Groups[1].Value.Trim();
            }
            return result;
        }

        private static void GetLastCheckInfo(string z_rep_text, ref DateTime last_check_date, ref string last_check_num)
        {
            try
            {
                Regex reg = new Regex(
                          StaticConstants.RBINNER_CONFIG.GetProperty<string>("check_reg", @"(?s)Чек\s*(\S+)\s+(\S+ \S+).*?[+]"));
                if (reg.IsMatch(z_rep_text))
                {
                    Match m = reg.Matches(z_rep_text).OfType<Match>().Last();
                    last_check_num = m.Groups[1].Value;
                    last_check_date = DateTime.Parse(m.Groups[2].Value);
                }
            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex, "ошибка в GetKkmName");
            }
        }

        /// <summary>
        /// Отправка файлов с кассы перед запуском сбора z-отчета. Приоритет 20
        /// </summary>
        /// <param name="terem_1c"></param>
        /// <param name="kkm_num"></param>
        /// <param name="type"></param>
        /// <param name="outPath"></param>
        internal static void kkm_LogOutFilesIn(string terem_1c, int kkm_num, string type, string outPath)
        {
            kkm_LogOutFiles(terem_1c, kkm_num, type, outPath, 20);
        }
        /// <summary>
        /// Отправка файлов с кассы после запуска сбора z-отчета. Приоритет 10
        /// </summary>
        /// <param name="terem_1c"></param>
        /// <param name="kkm_num"></param>
        /// <param name="type"></param>
        /// <param name="outPath"></param>
        internal static void kkm_LogOutFilesOut(string terem_1c, int kkm_num, string type, string outPath)
        {
            kkm_LogOutFiles(terem_1c, kkm_num, type, outPath, 10);
        }

        internal static void kkm_LogOutFiles(string terem_1c, int kkm_num, string type, string outPath, int priority)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(outPath);
                DateTime dt = DateTime.Now.AddDays(-1);

#if(DEB)
            var filess = dir.GetFiles();
#else
                var filess = dir.GetFiles().Where(a => a.CreationTime > dt);
#endif
                var files = filess.Select(a => new ArmServices.Fileinfo() { filename = a.Name, length = a.Length }).ToArray();


                StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "RecieveLogFilesList",
                    new object[] { terem_1c, kkm_num, type, outPath, DateTime.Now, files },
                    priority);

            }catch(Exception ex)
            {
                MDIParentMain.Log(ex, "Ошибка в kkm_LogOutFiles");
            }
        }

         [Obsolete]
        internal static void kkm_LogOutFiles_(string terem_1c,int kkm_num, string type,string outPath)
        {
            new CustomAction(oo =>
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(outPath);
                    DateTime dt = DateTime.Now.AddDays(-1);

#if(DEB)
                    var filess = dir.GetFiles();
#else
                    var filess = dir.GetFiles().Where(a=>a.CreationTime>dt);
#endif
                    var files = filess.Select(a => new ArmServices.Fileinfo() { filename = a.Name, length = a.Length/*, fileInfo = new byte[0]Serializer.BinarySerialize(a)*/ }).ToArray();

                    //Serializer.binary_write(new System.Collections.ArrayList() { files }, @"C:\Out\2.txt");
                    //отправить подтверждение

                    new CustomAction(o =>
                    {
                        var result_str = StaticConstants.WebServiceSystem.RecieveLogFilesList(terem_1c, kkm_num, type, outPath, DateTime.Now, files);
                        if (result_str.IndexOf("1") == -1) throw new Exception("RecieveLogFilesList package not delivered");

                    }, null)
                    {
                        Timeout = 1000,
                        LogEvent = (o) =>
                        {
                            MDIParentMain.Log(o.ToString());
                        }
                    }.Start();
                }
                catch (Exception ex)
                {
                    MDIParentMain.Log(ex, "Ошибка в kkm_LogOutFiles");
                    throw ex;
                }
            }, null)
            {
                Timeout = 1000,
                LogEvent = (o) =>
                {
                    MDIParentMain.Log(o.ToString());
                }
            }.Start();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RBClient.Classes.CustomClasses;
using System.Data;
using Config_classes;
using RBClient.WinForms.ViewModels;
using RBClient.Classes.InternalClasses.Models;

namespace RBClient.Classes
{
    partial class RbClientGlobalStaticMethods
    {
        #region static log
        public static Action<string> LogEvent;

        public static void Log(string message)
        {
            if (null != LogEvent)
            {
                LogEvent(message);
            }
        }

        public static void Log(Exception ex)
        {
            if (null != LogEvent)
            {
                LogEvent(ex.Message);
            }
        }

        public static void Log(Exception ex, string message)
        {
            if (null != LogEvent)
            {
                LogEvent("Error " + ex.Message + " Message:" + message);
            }
        }
#endregion

        /// <summary>
        /// Получаем входящие папки касс
        /// </summary>
        /// <returns></returns>
        public static List<string> ReturnKKmInPathes()
        {
            bool timer_kkm_walked = StaticConstants.MainWindow.m_kkm1_online || StaticConstants.MainWindow.m_kkm2_online || StaticConstants.MainWindow.m_kkm3_online ||
                StaticConstants.MainWindow.m_kkm4_online || StaticConstants.MainWindow.m_kkm5_online;

            List<string> kkm_params_names = new List<string>() { "Kkm1In", "Kkm2In", "Kkm3In", "Kkm4In", "Kkm5In"};
            List<string> kkm_list = new List<string>();

            if (!timer_kkm_walked)
            {
                kkm_params_names.ForEach(a =>
                {
                    new TimeOutAction((o) =>
                    {
                        string kkm_path=StaticHelperClass.ReturnStaticClassFieldValue(typeof(CParam), a).ToString();
                        if (Directory.Exists(kkm_path)) 
                            kkm_list.Add(kkm_path);        

                    }, null) { Timeout = 10000 }.Start();
                });
            }
            else
            {
                if (StaticConstants.MainWindow.m_kkm1_online) kkm_list.Add(CParam.Kkm1In);
                if (StaticConstants.MainWindow.m_kkm2_online) kkm_list.Add(CParam.Kkm2In);
                if (StaticConstants.MainWindow.m_kkm3_online) kkm_list.Add(CParam.Kkm3In);
                if (StaticConstants.MainWindow.m_kkm4_online) kkm_list.Add(CParam.Kkm4In);
                if (StaticConstants.MainWindow.m_kkm5_online) kkm_list.Add(CParam.Kkm5In);
            }

            MDIParentMain.Log("kkm inPathes list="+Serializer.JsonSerialize(kkm_list));

            if(kkm_list.Count==0) return null;
            else return kkm_list;
        }

        /// <summary>
        /// Получаем исходящие папки касс
        /// </summary>
        /// <returns></returns>
        public static List<string> ReturnKKmOutPathes()
        {
            bool timer_kkm_walked = StaticConstants.MainWindow.m_kkm1_online || StaticConstants.MainWindow.m_kkm2_online || StaticConstants.MainWindow.m_kkm3_online ||
                StaticConstants.MainWindow.m_kkm4_online || StaticConstants.MainWindow.m_kkm5_online;

            List<string> kkm_list = new List<string>();
            List<string> kkm_params_names = new List<string>() { "Kkm1Out", "Kkm2Out", "Kkm3Out", "Kkm4Out", "Kkm5Out" };

            if (!timer_kkm_walked)
            {
                kkm_params_names.ForEach(a =>
                {
                    new TimeOutAction((o) =>
                    {
                        string kkm_path = StaticHelperClass.ReturnStaticClassFieldValue(typeof(CParam), a).ToString();
                        if (Directory.Exists(kkm_path))
                            kkm_list.Add(kkm_path);

                    }, null) { Timeout = 10000 }.Start();
                });
            }
            else
            {
                if (StaticConstants.MainWindow.m_kkm1_online) kkm_list.Add(CParam.Kkm1Out);
                if (StaticConstants.MainWindow.m_kkm2_online) kkm_list.Add(CParam.Kkm2Out);
                if (StaticConstants.MainWindow.m_kkm3_online) kkm_list.Add(CParam.Kkm3Out);
                if (StaticConstants.MainWindow.m_kkm4_online) kkm_list.Add(CParam.Kkm4Out);
                if (StaticConstants.MainWindow.m_kkm5_online) kkm_list.Add(CParam.Kkm5Out);
            }

            MDIParentMain.Log("kkm outPathes list=" + Serializer.JsonSerialize(kkm_list));

            if (kkm_list.Count == 0) return null;
            else return kkm_list;
        }

        /// <summary>
        /// Получаем исходящие папки касс
        /// </summary>
        /// <returns></returns>
        public static List<string> ReturnKKmOutPathes(bool timer_kkm_walked)
        {
            List<string> kkm_list = new List<string>();
            List<string> kkm_params_names = new List<string>() { "Kkm1Out", "Kkm2Out", "Kkm3Out", "Kkm4Out", "Kkm5Out" };

            if (!timer_kkm_walked)
            {
                kkm_params_names.ForEach(a =>
                {
                    new TimeOutAction((o) =>
                    {
                        string kkm_path = StaticHelperClass.ReturnStaticClassFieldValue(typeof(CParam), a).ToString();
                        if (Directory.Exists(kkm_path))
                        {
                            kkm_list.Add(kkm_path);
                        }

                    }, null) { Timeout = 10000 }.Start();
                });
            }
            else
            {
                if (StaticConstants.MainWindow.m_kkm1_online) kkm_list.Add(CParam.Kkm1Out);
                if (StaticConstants.MainWindow.m_kkm2_online) kkm_list.Add(CParam.Kkm2Out);
                if (StaticConstants.MainWindow.m_kkm3_online) kkm_list.Add(CParam.Kkm3Out);
                if (StaticConstants.MainWindow.m_kkm4_online) kkm_list.Add(CParam.Kkm4Out);
                if (StaticConstants.MainWindow.m_kkm5_online) kkm_list.Add(CParam.Kkm5Out);
            }

            MDIParentMain.Log("kkm outPathes list=" + Serializer.JsonSerialize(kkm_list));

            if (kkm_list.Count == 0) return null;
            else return kkm_list;
        }

        public static List<string> ReturnPosRootFolders()
        {
            List<string> kkm_pahes = new List<string>();
            List<string> kkm_in_pahes = new List<string>();
            List<string> emark_folder_pathes = new List<string>();

            kkm_in_pahes = ReturnKKmInPathes();
            kkm_in_pahes.ForEach(a =>
            {         
                kkm_pahes.Add(new DirectoryInfo(a).GetGlobalRoot()); 
            });

            string pos_rb_config = StaticConstants.POSDISPLAY_CONFIG;
            //искать конфиг
            FileInfo config = RbClientGlobalStaticMethods.GetDirectory(StaticConstants.CONFIGS_FOLDER).FindFile(pos_rb_config, false);
            Log("prepare config file");
            if (config != null)
            {

                //взять пути из конфига
                using (ConfigClass pos_config = new ConfigClass(config.FullName))
                {
                    Dictionary<string, object> prop_dict = pos_config.GetAllProperties(StaticConstants.POS_CONFIG_KEY);

                    kkm_pahes.ForEach(a =>
                    {
                        prop_dict.Values.OfType<string>().ToList().ForEach(b =>
                        {
                            emark_folder_pathes.Add(Path.Combine(a, b));
                        });
                    });

                }
            }
            else
            {
                //получить стандартные пути
                kkm_in_pahes.ForEach(a =>
                    {
                        emark_folder_pathes.Add(new DirectoryInfo(a).Root.FullName);  
                    });
                    //emark_folder_pathes = return_folders_in_kkm_pathes(StaticConstants.EMARK_FOLDER_NAME);
               // Serializer.JsonSerialize("emark_folders="+emark_folder_pathes);
            }
            if (emark_folder_pathes.Count == 0) return null;
            return emark_folder_pathes;
        }



        /// <summary>
        /// Находим искомые папки в кассах
        /// </summary>
        /// <param name="folder_name">имя искомой папки</param>
        /// <returns></returns>
        public static List<string> return_folders_in_kkm_pathes(string folder_name)
        {
            MDIParentMain.Log(String.Format("Определяем список касс"));
            if (CParam.Kkm1In == "") { MDIParentMain.Log(String.Format("Инициализируем параметры")); CParam.Init(); }

            List<string> kkmInList = RbClientGlobalStaticMethods.ReturnKKmInPathes();
            List<string> kkm_list = new List<string>();

            kkmInList.ForEach(a => {
                
                //string path="";

                new CustomAction((o) =>
                {
                    DirectoryInfo dir = new DirectoryInfo(a);
                    Log("globalroot=" + dir.Root.FullName);
                   string path = dir.Root.FindDirectory(folder_name, true).Name;
                        //ReturnFolderInKKm(dir.Root.FullName, folder_name);
                    Log("globalrootdirs=" + Serializer.JsonSerialize(dir.Root.GetDirectories().Select(b=>b.Name).ToList()));
                    Log("path="+path);
                    if (path == "") throw new Exception("Не могу найти на кассе" + dir.Root.FullName + " папку " + folder_name);
                    kkm_list.Add(path);
                }, null) { LogEvent=new NLogDelegate((oo)=>Log(oo.ToString()))}.Start();
            });

            MDIParentMain.Log(String.Format("Определили список касс ={0}", kkm_list.Count));
            return kkm_list;
        }

        /// <summary>
        /// Находим нужную папку входя в рут через kkm_path
        /// </summary>
        /// <param name="kkm_path">точка входа в рут</param>
        /// <param name="folder_name">искомая папка относительно рута</param>
        /// <returns></returns>
        public static string ReturnFolderInKKm(string kkm_path, string folder_name)
        {
            try
            {
                if (kkm_path == "") return "";
                DirectoryInfo dir = new DirectoryInfo(kkm_path);
                dir = dir.Root;
                dir = dir.FindDirectory(folder_name, true);

                if (dir == null) return "";
                else
                    return dir.FullName;
            }catch(Exception ex)
            {
                Log("Ошибка поиска "+ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Получить текущую директорию приложения
        /// </summary>
        private static DirectoryInfo _AppDirectory = null;
        public static DirectoryInfo AppDirectory
        {
            get
            {
                if (_AppDirectory == null)
                    _AppDirectory = new DirectoryInfo(CParam.AppFolder);
                return _AppDirectory;
            }
        }

        /// <summary>
        /// Возвращает или создает нужную директорию в текущей приложения
        /// </summary>
        /// <param name="dir_name"></param>
        /// <returns></returns>
        public static DirectoryInfo GetDirectory(string dir_name)
        {
            DirectoryInfo s_treport_dir = new DirectoryInfo(Path.Combine(AppDirectory.FullName, dir_name)).CreateOrReturn();
            return s_treport_dir;
        }


        /// <summary>
        /// Метод выполняется после каждого обмена
        /// </summary>
        public static void AfterExchangeRoutine()
        {
        }

        /// <summary>
        /// получает фильтрованный список номенклатур
        /// </summary>
        public static DataTable GetFullNomeList(int doc_id)
        {
            if (StaticConstants._FullNomenclatureList == null)
            {
                CBData _data = new CBData();

                DataTable dt = _data.NomenclatureFullList(doc_id);

                StaticConstants._FullNomenclatureList = dt;
            }
            return StaticConstants._FullNomenclatureList;
        }


        /// <summary>
        /// Получаем T репорты со всех касс
        /// </summary>
        /// <param name="OutPathes"></param>
        /// <returns></returns>
        public static List<FileInfo> GetTReports(List<string> InPathes,List<string> OutPathes,bool deleteOldTReps)
        {
            //получить пути для касс
            List<string> kkmInList = InPathes;
            List<string> kkmOutList = OutPathes;

            List<FileInfo> t_report_list=new List<FileInfo>();

            if (kkmInList == null || kkmOutList == null)
            {
                Log("Нет касс в сети!!!");
                return null;
            }

            //Очистить файлы t-report на кассах
            if (deleteOldTReps)
            {
                kkmOutList.ForEach(a =>
                {
                    DirectoryInfo dir = new DirectoryInfo(a);
                    List<FileInfo> file_pathes = dir.GetFiles("T*", SearchOption.TopDirectoryOnly).ToList();
                    file_pathes.ForEach(b => new CustomAction((o) =>
                    {
                        b.Delete();
                    }
                    , null).Start());
                });

            }

            //закинуть туда 0.dat

            kkmInList.ForEach(a => new CustomAction((o) =>
            {
                Log(String.Format("0.dat кладем в {0}",a));
                string filename = o.ToString(); File.Create(System.IO.Path.Combine(a, filename)); }
            , "0.dat") { LogEvent = (oo) => { MDIParentMain.Log(oo.ToString()); } }.Start());

            //дождаться ответа касс
            kkmOutList.ForEach(a => new CustomAction((o) =>
            {
                DirectoryInfo kkm_dir = new DirectoryInfo(a);
                List<FileInfo> file_pathes = kkm_dir.GetFiles("T*", SearchOption.TopDirectoryOnly).ToList();
                if (file_pathes == null || file_pathes.Count == 0) throw new Exception("Еще пока нет Трепорта");
                file_pathes.Sort((c, b) => b.CreationTime.CompareTo(c.CreationTime));

                if (file_pathes.NotNullOrEmpty())
                    UpdateClass.kkm_UpdateTReport(a, file_pathes.First());

                t_report_list.AddRange(file_pathes);

            }, null) { Timeout = 10000, LogEvent = (oo) => { MDIParentMain.Log(oo.ToString());} }.Start());

            if (t_report_list.Count > 0)
                return t_report_list;
            else return null;
        }

        /// <summary>
        /// ищет конфигурационный файл
        /// </summary>
        /// <param name="config_file">имя файла</param>
        /// <param name="findRecursive">искать рекурсивно</param>
        /// <returns></returns>
        public static FileInfo ReturnConfig(string config_file,bool findRecursive)
        {
            string _config = StaticConstants.INNER_CONFIG;
            FileInfo config = RbClientGlobalStaticMethods.GetDirectory(StaticConstants.CONFIGS_FOLDER)
                .FindFile(_config, false);
            return config;
        }



        /// <summary>
        /// возвращает время начала работы теремка
        /// </summary>
        /// <param name="t_w">запись из таблицы о времени работы теремка</param>
        /// <returns></returns>
        internal static string getDefaultTimeFrom(InternalClasses.Models.t_WorkTeremok t_w)
        {
            try
            {
                if (t_w != null)
                {
                    return t_w.teremok_firstTime.ToString("HH:mm");
                }
                return MarkViewModelItem.DefaultStartTime;
            }catch(Exception ex)
            {
                Log(ex, "Не получено начальное время работы теремка");
                return MarkViewModelItem.DefaultStartTime;
            }
        }

        /// <summary>
        /// возвращает время окончания работы теремка
        /// </summary>
        /// <param name="t_w">запись из таблицы о времени работы теремка</param>
        /// <returns></returns>
        internal static string getDefaultTimeTo(InternalClasses.Models.t_WorkTeremok t_w)
        {
            try
            {
                if (t_w != null)
                {
                    return t_w.teremok_lastTime.ToString("HH:mm");
                }
                return MarkViewModelItem.DefaultEndTime;
            }
            catch (Exception ex)
            {
                Log(ex, "Не получено конечное время работы теремка");
                return MarkViewModelItem.DefaultEndTime;
            }
        }


        /// <summary>
        /// отбрасываем нули после запятой
        /// </summary>
        /// <param name="doubl"></param>
        /// <returns></returns>
        internal static object ReturnIntOrDoubleFromDouble(object doubl,string format)
        {
            if (doubl is double)
            {
                double o=(double)doubl;
                if (o % 1 == 0)
                {
                    return (int)o;
                }
                else
                {
                    return o.ToString(format);
                }
            }
            if (doubl is decimal)
            {
                decimal o = (decimal)doubl;
                if (o % 1 == 0)
                {
                    return (int)o;
                }
                else
                {
                    return o.ToString(format);
                }
            }
            return doubl;
        }
    }
}

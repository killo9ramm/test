using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Debug_classes;
using System.Threading;
using killolib;
using Config_classes;

namespace RBClient.Classes.CustomClasses
{
    class VideoUpdateClass
    {
        public static int doc_type = 30;
        public const string Delete_flag="_D_";
        public const string Add_flag = "_A_";
        public static List<string> flags = new List<string>() {Delete_flag,Add_flag};
        public static string video_folder = "ADV_VIDEO";
        public static string emark_video_folder = Path.Combine("Emark","video");
        public static DirectoryInfo working_dir;
        

        public static Thread coping_thread;

        static VideoUpdateClass()
        {
            //создать папку ADV_VIDEO   
            working_dir = Path.Combine(Directory.GetCurrentDirectory(), video_folder).CreateOrReturnDirectory();
        }

        public static void OperateEducArchiveDoc(FileInfo _filename)
        {
            try
            {
                OperateEducArchive(_filename);
                
                int m_teremok_id = Convert.ToInt32(CParam.TeremokId);
                
                CTransfer _transfer = new CTransfer();
                _transfer.LoadVideoArchive(_filename.FullName, 36, m_teremok_id);

                StaticConstants.MainGridUpdate();
            }catch(Exception exp)
            {
                MDIParentMain.Log("Не получилось скопировать обучающее видео");
            }

        }

        public static void OperateEducArchive(FileInfo _filename)
        {
            try
            {
                string educ_folder_str = StaticConstants.EDUC_VIDEO_FOLDER;
                DirectoryInfo educ_folder = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), educ_folder_str));
                educ_folder.CreateOrReturn();
                synchronizeWorkingDir(_filename, educ_folder);
            }
            catch (Exception exp)
            {
                MDIParentMain.Log("Не получилось скопировать обучающее видео");
            }

        }

        public static void OperateEducArchive()
        {
            string video_mask = "EDUC_VIDEO";
            DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder + "\\inbox\\");
            _dir.GetFiles().ToList<FileInfo>().Where(a => a.Name.IndexOf("video_mask") != -1).ToList().ForEach(b =>
            {
                OperateEducArchive(b);
                b.Delete();
            });
        }

        public static void OperateAdvArchive(FileInfo _filename)
        {
            video_folder = "ADV_VIDEO";
            emark_video_folder = Path.Combine("Emark","video");
            working_dir = Path.Combine(Directory.GetCurrentDirectory(), video_folder).CreateOrReturnDirectory();

            Thread thread = new Thread((delegate(object a)
            {
                FileInfo filename = (FileInfo)a;
                //прочитать флаги и синхронизировать рабочую папку с видео
                
                 synchronizeWorkingDir(filename);

                 //добаввить t_doc
                 CBData _b = new CBData();
                 CParam.Init();
                 int m_teremok_id = Convert.ToInt32(CParam.TeremokId);
                 //   int _doc_id = _b.AdvAdd(doc_type, m_teremok_id, 4,"Получены новые ролики!");
                 CTransfer _transfer = new CTransfer();
                 _transfer.LoadVideoArchive(filename.FullName, 30, m_teremok_id);

                 StaticConstants.MainGridUpdate();

                //раскидать на кассы
                 try
                 {
                     sendVideoFilesToKkm();
                 }
                 catch (Exception ex)
                 {
                     MDIParentMain.Log(ex,"Не удалась рассылка роликов на кассы exp: "+ex.Message);
                 }
                
                                        
            }));
            thread.Name = "KKmWorkingThread";
            thread.Start(_filename);
        }

        public static void sendVideoFilesToKkmAsync()
        {
            if (coping_thread == null || coping_thread.ThreadState != ThreadState.Running)
            {
                coping_thread = new Thread(sendVideoFilesToKkm);
                coping_thread.Start();
            }
        }

        public static void sendVideoFilesToKkm()
        {
            MDIParentMain.Log("Рассылаем ролики на кассы");
            //собрать кассы
            List<string> kkm_pathes = return_kkm_pathes(); MDIParentMain.Log("Количество роликов касс "+kkm_pathes.Count);
            
            MDIParentMain.Log("Статус потока копирования "+ThreadCopyClass.StaticState);
            if (ThreadCopyClass.StaticState == CopyThreadState.StaticComplete) 
            {
                ThreadCopyClass.Reset();
                List<FileInfo> files = working_dir.GetFiles().ToList().Where(b=>b.Name.IndexOf("Thumbs.db")==-1).ToList();  MDIParentMain.Log("Количество роликов "+files.Count);
                
                
                kkm_pathes.ForEach(a =>
                {
                   try
                    {
                        DirectoryInfo dir = new DirectoryInfo(a);   MDIParentMain.Log("Наыинаем копирование на кассу "+a);
                //        сделать сравнение файлового списка с тем что на кассе и если одинаковые то не копировать
                        if (!dir.FolderHasSameFiles(working_dir))
                        {MDIParentMain.Log("Директории не равны");
                  //          копируем файлы
                            files.ForEach(c => 
                                {
                    //            сделать проверку файлов на соответсвие
                                    try
                                    {
                                        MDIParentMain.Log("начинаем синхронное копирование ролика "+c+" в "+a);
                                        ThreadCopyClass.ThreadCopySync(c, a);
                                        
                                    }catch(Exception ex)
                                    {
                                        MDIParentMain.Log(ex,"Не удалось скопировать файл на кассу ");
                                    }
                                });
                            

                            string temp_folder = Path.Combine(a, "TEMP"); MDIParentMain.Log(String.Format("Создаем темповую директорию"));
                            DirectoryInfo dir_temp = new DirectoryInfo(temp_folder);
                            dir_temp.CreateOrReturn();

                            //List<FileInfo> list1 = dir.GetFiles().ToList<FileInfo>();
                            //List<FileInfo> list2 = list1.Where(b => !files.Select(a1=>a1.Name).Contains(b.Name)).ToList();


                            List<FileInfo> files_to_temp = dir.GetFiles().ToList<FileInfo>().Where(b => !files.Select(a1 => a1.Name).Contains(b.Name)).ToList(); MDIParentMain.Log(String.Format("Файлов {0} для темпа", files_to_temp.Count));

                            //List<FileInfo> files_to_temp1 = dir.GetFiles().ToList<FileInfo>().Where(b => !files.Contains(b)).ToList();
                            //List<FileInfo> files_to_temp2 = dir.GetFiles().ToList<FileInfo>().Where(b => files.Contains(b)).ToList();
                            //List<FileInfo> files_to_temp3 = dir.GetFiles().ToList<FileInfo>().Where(b => files.Contains(b)==false).ToList();

                            files_to_temp.ForEach(c =>
                            {
                                try
                                {
                                    MDIParentMain.Log(String.Format("Начинаем копирование файла {0} в темп",c));
                                    ThreadCopyClass.ThreadMoveSync(c, temp_folder);
                                }
                                catch (Exception ex)
                                {
                                    MDIParentMain.Log(ex,"Файл занят кассой "+c);
                                }
                            });

                            MDIParentMain.Log(String.Format("Очищаем директорию темп если она больше 100мб"));
                            
                            dir_temp.DeleteOldFilesInDir(100000000);
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        MDIParentMain.Log(ex, "Касса не найдена "+a);
                    }
                });
            }
        }

        public static List<string> return_kkm_pathes()
        {
            MDIParentMain.Log(String.Format("Определяем список касс"));
            if (CParam.Kkm1In == "") { MDIParentMain.Log(String.Format("Инициализируем параметры")); CParam.Init(); }

            List<string> kkm_list = new List<string>();
            
             UpdateClass updcls = new UpdateClass() { LogEvent = MDIParentMain.Log };
            FileInfo config = updcls.prepareDirectoryInRoot(StaticConstants.CONFIGS_FOLDER, false)
                .FindFile(StaticConstants.ADVVIDEO_CONFIG, false);

            List<string> kkm_pahes = new List<string>();
            RbClientGlobalStaticMethods.ReturnKKmInPathes().ForEach(a =>
            {
                kkm_pahes.Add(new DirectoryInfo(a).GetGlobalRoot());
            });

            if (config != null)
            {
                //взять пути из конфига
                using (ConfigClass pos_config = new ConfigClass(config.FullName))
                {
                    Dictionary<string, object> prop_dict = 
                        pos_config.GetAllProperties(StaticConstants.ADV_VIDEO_CONFIG_KEY);
                    //kkm_list = prop_dict.Values.OfType<string>().ToList();
                    kkm_pahes.ForEach(a =>
                    {
                        prop_dict.Values.OfType<string>().ToList().ForEach(b =>
                        {
                            kkm_list.Add(Path.Combine(a, b));
                        });
                    });
                }
            }
            else
            {

                kkm_list.Add(ReturnKKmEmarkVideoFolder(CParam.Kkm1In));
                kkm_list.Add(ReturnKKmEmarkVideoFolder(CParam.Kkm2In));
                kkm_list.Add(ReturnKKmEmarkVideoFolder(CParam.Kkm3In));
                kkm_list.Add(ReturnKKmEmarkVideoFolder(CParam.Kkm4In));
                kkm_list.Add(ReturnKKmEmarkVideoFolder(CParam.Kkm5In));

                kkm_list = kkm_list.Where(a => a != "").ToList();
            }
            
            MDIParentMain.Log(String.Format("Определили список касс ={0}",kkm_list.Count));
            return kkm_list;
        }

        private static string ReturnKKmEmarkVideoFolder(string kkm_path)
        {
            if (kkm_path == "") return "";
            DirectoryInfo dir = new DirectoryInfo(kkm_path);
            dir=dir.FindParentDir("c");
            if (dir == null) return "";
            else
                return Path.Combine(dir.FullName, emark_video_folder);
        }

        public static void synchronizeWorkingDir(FileInfo filename, DirectoryInfo dir)
        {
            string flag = "";
            flags.ForEach(a =>
            {
                if (filename.Name.IndexOf(a) != -1)
                {
                    flag = a;
                    return;
                }
            });

            if (flag == "")
            {
                string temp_folder = Path.Combine(dir.FullName, "TEMP");
                DirectoryInfo dir_temp = new DirectoryInfo(temp_folder);
                dir_temp.CreateOrReturn();
                //освободить директорию

                List<FileInfo> files_to_temp = dir.GetFiles().Where(a => a.Name.IndexOf("Thumbs.db") == -1).ToList();
                files_to_temp.ForEach(a =>
                {
                    a.CopyTo(Path.Combine(temp_folder, a.Name), true);
                    a.Delete();
                });

                //сделать проверку количества видео файлов в папке temp

                dir_temp.DeleteOldFilesInDir(100000000);

                //разархивировать видео
                ZipIonicHelper.Exctract(filename, dir);
                return;
            }

            if (flag == Delete_flag)
            {
                List<string> files_to_delete = ZipIonicHelper.GetZipEntryFileNames(filename.FullName);
                List<FileInfo> files_in_dir = dir.GetFiles().ToList();

                if (files_in_dir == null || files_in_dir.Count == 0) return;

                files_to_delete.ForEach(a =>
                {
                    files_in_dir.Where(b => b.BaseFileName() == a.BaseFileName()).ToList().ForEach(c => c.Delete());
                });

                return;
            }

            if (flag == Add_flag)
            {
                //добавить видео из архива
                ZipIonicHelper.Exctract(filename, dir);
                return;
            }
        }

        public static void synchronizeWorkingDir(FileInfo filename)
        {
            string flag = "";
            flags.ForEach(a =>
            {
                if(filename.Name.IndexOf(a)!=-1){
                    flag = a;
                    return;
                }
            });
            
            if (flag=="")
            {
                string temp_folder = Path.Combine(working_dir.FullName, "TEMP");
                DirectoryInfo dir_temp = new DirectoryInfo(temp_folder);
                dir_temp.CreateOrReturn();
                //освободить директорию

                List<FileInfo> files_to_temp=working_dir.GetFiles().Where(a => a.Name.IndexOf("Thumbs.db") == -1).ToList();
                files_to_temp.ForEach(a =>
                {
                    a.CopyTo(Path.Combine(temp_folder,a.Name),true);
                    a.Delete();
                });

                //сделать проверку количества видео файлов в папке temp
                
                dir_temp.DeleteOldFilesInDir(100000000);

                //разархивировать видео
                ZipIonicHelper.Exctract(filename, working_dir);

                //скопировать файл конфига в папку конфигов
                FileInfo config = working_dir.FindFile(StaticConstants.ADVVIDEO_CONFIG, false);

                if (config != null)
                {
                    DirectoryInfo con_folder = new DirectoryInfo(StaticConstants.CONFIGS_FOLDER);
                    config.MoveWithReplase(con_folder.FullName);
                }
                return;
            }

            if (flag == Delete_flag)
            {
                List<string> files_to_delete = ZipIonicHelper.GetZipEntryFileNames(filename.FullName);
                List<FileInfo> files_in_working_dir = working_dir.GetFiles().ToList();

                if (files_in_working_dir == null || files_in_working_dir.Count == 0) return;

                files_to_delete.ForEach(a =>
                {
                    files_in_working_dir.Where(b => b.BaseFileName()==a.BaseFileName()).ToList().ForEach(c=>c.Delete());
                });

                return;
            }

            if (flag == Add_flag)
            {
                //добавить видео из архива
                ZipIonicHelper.Exctract(filename, working_dir);

                //скопировать файл конфига в папку конфигов                                
                FileInfo config = working_dir.FindFile(StaticConstants.ADVVIDEO_CONFIG, false);

                if (config != null)
                {
                    DirectoryInfo con_folder = new DirectoryInfo(StaticConstants.CONFIGS_FOLDER);
                    config.MoveWithReplase(con_folder.FullName);
                }
                return;
            }
        }
    }
}

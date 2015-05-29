using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomLogger;
using System.IO;
using Debug_classes;
using Config_classes;
using System.Diagnostics;
using System.Threading;
using System.Windows.Documents;
using System.Security;
using System.Collections;
using RBServer.Debug_classes;

namespace RBClient.Classes.CustomClasses
{
    partial class UpdateClass : LoggerBase, IDisposable
    {

        public void ОбработатьВходящийУстановочныйПакетДляКасс(FileInfo _file)
        {
            try
            {
                OperatingObject = CreatePackageName(_file);
                DirectoryInfo pe = prepareDirectoryInRoot(StaticConstants.INSTALL_PACKAGE_FOLDER, false);
                DirectoryInfo folder = pe.CreateSubdirectory(CreatePackageName(_file));
                folder.DeleteOldFilesInDir(0);

                Log("Распаковываем архив с файлом " + _file.Name);
                ZipHelper.UnZipFile(_file.FullName, folder.FullName);

                Log("Начали копирование пакета на кассы");
                List<string> kkmin_folder_pathes = RbClientGlobalStaticMethods.ReturnKKmInPathes();
                Log("kkmin_folder_pathes directories is " + Serializer.JsonSerialize(kkmin_folder_pathes));

                //синхронизация с кассами и запуск процесса
                FileInfo config = folder.FindFile(StaticConstants.INSTALL_PACKAGE_CONFIG, false);
                if (config != null)
                {
                    Config = new ConfigClass(config.FullName);
                    if (Config!=null)
                    {
                        Log("Нашли в пакете файл конфига " + config.Name);
                    }
                }

                Hashtable _results = new Hashtable(); 
                bool results=true;
                string messages = "";
                foreach (string kkm_path in kkmin_folder_pathes)
                {
                    
                    string message = "";
                    bool result = InstallPackageOnKKm(kkm_path, folder,ref message);    
                    results=results&&result;
                        _results.Add(kkm_path, new object[2]{ message, result });
                        messages += message + "\r\n";
                }
                
                if (CheckValidKKmResults(_results))
                {
                    if (results)
                    {
                        Log("Входящий пакет для касс обработан полностью!" + _file.Name);
                        messages = "Теремок :"+StaticConstants.Teremok_Name+" Пакет:"+_file.Name+
                            "\r\nВходящий пакет для касс обработан полностью!\r\n"
                            +messages;
                        ErrorNotifications.NotificateKKSInstall(messages);
                        _file.Delete();
                    }
                    else
                    {
                        Log("Входящий пакет для касс обработан частично!!! Нужен разбор полетов!!!" + _file.Name);

                        messages = "Теремок :" + StaticConstants.Teremok_Name + " Пакет:" + _file.Name +
                            "\r\nВходящий пакет для касс обработан частично!!! Нужен разбор полетов!!!\r\n"+
                            "Пакет сохранен в " + StaticConstants.TRASH_FOLDER +"\r\n"
                            + messages;
                        ErrorNotifications.NotificateKKSInstall(messages);

                        Move_File_To_Temp(StaticConstants.TRASH_FOLDER, _file);
                    }
                }
                else
                {
                    Log("Не удалось в обработать входящий пакет для касс" + _file.Name);
                    messages = "Теремок :" + StaticConstants.Teremok_Name + " Пакет:" + _file.Name +
                            "\r\nНе удалось в обработать входящий пакет для касс\r\n"+
                            "Пакет сохранен в " + StaticConstants.TRASH_FOLDER + "\r\n"
                            + messages;
                    ErrorNotifications.NotificateKKSInstall(messages);
                    Move_File_To_Temp(StaticConstants.TRASH_FOLDER, _file);
                }
                
                Dispose();
            }
            catch (Exception ex)
            {
                Log(ex, "Не удалось в обработать входящий пакет " + _file.Name);
                Dispose();
                Move_File_To_Temp(StaticConstants.TRASH_FOLDER, _file);
            }
        }

        private bool CheckValidKKmResults(Hashtable execution_results)
        {
            bool result = false;
            foreach (string key in execution_results.Keys)
            {
                object[] objects = (object[])execution_results[key];
                result = result || (bool)objects[1];
            }

            return result;
        }

        private bool InstallPackageOnKKm(string kkm_path,DirectoryInfo packageFolder,ref string message)
        {
            try
            {
                //получаем папки
                DirectoryInfo killer_directory = GetKillerDirectory(kkm_path);
                DirectoryInfo kkm_package_directory =
                    killer_directory.CreateOrReturnSubDirectory((string)OperatingObject);
                kkm_package_directory.DeleteOldFilesInDir(0);

                //копируем файлы
                CopyFilesTokkm(packageFolder, kkm_package_directory);

                //запускаем процесс
                ConfigClass killerconfig = new ConfigClass(GetKillerConfigFile(kkm_path));

                string old_process_name = killerconfig.GetProperty(StaticConstants.KILLER_PROCESS_NAME).ToString();

                string start_path = kkm_package_directory.FindFile(Config.GetProperty("file_name")
                    .ToString(), true).FullName;

                if (Config.GetProperty<bool>("replace_webpath_on_root_in_prconfig", true))
                {
                    start_path = kkm_package_directory.FindFile(Config.GetProperty("file_name").ToString(), true)
                    .ReplaceRootDirectoryOnDisk(
                    Config.GetProperty<string>("replace_webpath_on_root_in_prconfig_value", "c:"));
                }
                
                killerconfig.SetProperty(StaticConstants.KILLER_START_FILE_PATH,
                    start_path);
                killerconfig.SetProperty(StaticConstants.KILLER_START_FLAG_NAME, "1");

                //проверяем отработал ли киллер
                bool exitFlag = false;
                if (CheckPrKillerProcessStarted(killerconfig, 10))
                {
                    message = "Пакет успешно запущен " + packageFolder.Name + " на кассу" + kkm_path;
                    exitFlag=true;
                }
                else
                {
                    message = "Не удалось установить пакет " + packageFolder.Name + " на кассу" + kkm_path +
                        " \r\n Кончился таймаут киллера!";
                    exitFlag=false;
                }

                //восстановить старое значение в параметре prkiller
                killerconfig.SetProperty(StaticConstants.KILLER_START_FILE_PATH,old_process_name);
                return exitFlag;
            }
            catch (Exception ex)
            {
                message = "Не удалось установить пакет " + packageFolder.Name + " на кассу" + kkm_path + " \r\n Error:" +
                    ex.Message;
                Log(ex, "Не удалось установить пакет " + packageFolder.Name + " на кассу" + kkm_path);
                return false;
            }
        }

        private static bool CheckPrKillerProcessStarted(ConfigClass killerconfig,int tries)
        {
            CustomAction ca = new CustomAction((o) =>
            {
                if (killerconfig.GetProperty(StaticConstants.KILLER_START_FLAG_NAME).ToString() != "0")
                {
                    throw new Exception();
                }
            }, null)
            {
                Timeout = int.Parse(killerconfig.GetProperty(StaticConstants.KILLER_TIMEOUT_MS_NAME).ToString()),
                MaxTries = tries
            };
            ca.Start();
            return ca.IsSuccess();
        }

        private static void CopyFilesTokkm(DirectoryInfo packageFolder, DirectoryInfo kkm_package_directory)
        {
            packageFolder.GetFiles("*.*", SearchOption.AllDirectories).ToList().ForEach(b =>
            {
                new CustomAction((oo) =>
                {
                    b.RemoteCopyFileWithDirCreation(packageFolder, kkm_package_directory);
                }, null).Start();
            });
        }

        private string GetKillerConfigFile(string kkm_path)
        {
            DirectoryInfo kkm_dir = new DirectoryInfo(kkm_path);
            kkm_dir = kkm_dir.Root;
            FileInfo killer_file = new FileInfo(Path.Combine(kkm_dir.FullName, StaticConstants.KILLER_PATH));
            return killer_file.Directory.PFileIn(StaticConstants.KILLER_CONFIG_NAME);
        }

        private DirectoryInfo GetKillerDirectory(string kkm_path)
        {
            DirectoryInfo kkm_dir = new DirectoryInfo(kkm_path);
            kkm_dir = kkm_dir.Root;
            FileInfo killer_file = new FileInfo(Path.Combine(kkm_dir.FullName, StaticConstants.KILLER_PATH));
            return killer_file.Directory;
        }


        public void ОбработатьВходящийУстановочныйПакет(FileInfo _file)
        {
            try
            {
                OperatingObject = _file.FullName;
                DirectoryInfo pe = prepareDirectoryInRoot(StaticConstants.INSTALL_PACKAGE_FOLDER, false);
                DirectoryInfo folder = pe.CreateSubdirectory(CreatePackageName(_file));
                folder.DeleteOldFilesInDir(0);

                Log("Распаковываем архив с файлом "+_file.Name);
                ZipHelper.UnZipFile(_file.FullName, folder.FullName);

                int? return_code = null;
                FileInfo config = folder.FindFile(StaticConstants.INSTALL_PACKAGE_CONFIG, false);
                if (config != null)
                {
                    Log("Нашли в пакете файл конфига " + config.Name);

                    return_code=InstallPackage(config, folder);
                }
                else
                {
                    Log("В пакете нет файла конфига!!!");

                    return_code = InstallPackageFile(folder.GetFiles("*.exe").FirstOrDefault());
                }

                if (return_code != 0)
                {
                    Log("Пакет не отработал!!! Код выхода " + return_code);
                }

                //проверка кода возврата и удаление архива
                CheckIfInstallationComplete((int)return_code,_file);
               
                Dispose();
            }catch(Exception ex){
                Log(ex, "Не удалось в обработать входящий пакет "+_file.Name);
                Dispose();
                Move_File_To_Temp(StaticConstants.TRASH_FOLDER, _file);
            }
        }

        private void CheckIfInstallationComplete(int return_code, FileInfo _file)
        {
            List<string> exit_codes = GetExitCodes();

            string message="";

            if (return_code == 0)
            {
                if(exit_codes.NotNullOrEmpty())
                {
                    if (exit_codes.Contains(ExitCode.ToString()))
                    {
                        Log("Пакет установлен успешно!!! Удаляем файл" + _file.FullName);

                        message = "Теремок :" + StaticConstants.Teremok_Name + " Пакет:" + _file.Name +
                            "\r\nПакет установлен успешно!!!\r\n";
                        ErrorNotifications.NotificateAdminPkInstall(message);

                        CustomActionsOnSuccessInstallation(InstallName);
                        _file.Delete();
                    }
                    else
                    {
                        Log("Пакет запустился но не отработал!!! Код выхода не совпадает с переданными успешными кодами. "
                           +"Нужен разбор полетов. "+ _file.FullName);

                        message = "Теремок :" + StaticConstants.Teremok_Name + " Пакет:" + _file.Name +
                            "\r\nПакет запустился но не отработал!!!"+
                            " Код выхода не совпадает с переданными успешными кодами. \r\n"+
                            "Пакет находится в папке " + StaticConstants.TRASH_FOLDER;

                        ErrorNotifications.NotificateAdminPkInstall(message);

                        Move_File_To_Temp(StaticConstants.TRASH_FOLDER, _file);
                    }
                }
                else
                {
                    Log("Пакет отработал успешно!!! Но коды выхода переданы не были! Удаляем файл" + _file.FullName);

                    message = "Теремок :" + StaticConstants.Teremok_Name + " Пакет:" + _file.Name +
                            "\r\nПакет отработал успешно!!! Но коды выхода переданы не были!";
                    ErrorNotifications.NotificateAdminPkInstall(message);

                    _file.Delete();
                }
                
            }
            if (return_code == -1)
            {
                Log("Пакет не запустился!!! "
                           + "Нужен разбор полетов. " + _file.FullName);
                Move_File_To_Temp(StaticConstants.TRASH_FOLDER, _file);
            }
            if (return_code == -2)
            {
                Log("Пакет запущен но проработал более чем " +
                       StaticConstants.RBINNER_CONFIG.GetProperty<int>("file_execution_timeout", 1800000) / 60000
                       + " минут!!! Нужен разбор полетов!" + _file.FullName);
                Move_File_To_Temp(StaticConstants.TRASH_FOLDER, _file);
            }

        }

        private void CustomActionsOnSuccessInstallation(string packName)
        {
            if (packName == "dotNetFx40_Full_x86_x64.exe")
            {
                if (StaticConstants.UpdateHelper.IsNewerVersion("5.6.4.7"))
                {
                    StaticConstants.UpdateHelper.SetVersion("5.6.4.7");
                }
            }
        }

        private void Move_File_To_Temp(string temp_folder_name,FileInfo _file)
        {
            try
            {
                DirectoryInfo dir = prepareDirectoryInRoot(temp_folder_name, false);
                Log("Перемещаем файл " + _file.FullName+" в папку "+dir.FullName);
                _file.MoveWithReplase(dir.FullName);
            }catch(Exception ex)
            {
                Log(ex, "Не удалось переместить пакет " + _file.Name+" в "+temp_folder_name);
            }
        }

        
        public ConfigClass Config;
        private int ExitCode = -1;
        public Type UpdationType;
        public string UpdationTypeStr;



        

       

        public void Dispose()
        {
            try
            {
                OperatingObject = null;
                
                InstallName = null;
                ExitCode = -1;
                if (UpdationList.Contains(this))
                {
                    UpdationList.Remove(this);
                }
                if (Config != null) Config.Close();
                Config = null;
            }catch(Exception ex)
            {
                Log("UpdateClassDisposeError "+ex.Message);
            }
        }   

        public int InstallPackage(FileInfo config,DirectoryInfo dir)
        {
            if (config != null)
            {
                //взять пути из конфига
                string file_name;

                Config = new ConfigClass(config.FullName);
                InstallName = (string)Config.GetProperty("install_name");
                file_name=(string)Config.GetProperty("file_name");

                return StartProcess(Path.Combine(dir.FullName, file_name));
            }
            return -1;
        }

        private int StartProcess(string file_name)
        {
            if (InstallName != null)
            {
                if (PackageIsInstalled(InstallName))
                {
                    Log("Пакет был установлен ранее " + InstallName);
                    return 0;
                }
            }

            return StartProcesses(file_name);
        }

        private void SetArguments(ref Process process)
        {
            if (Config != null)
            {
                object o =Config.GetProperty("start_params");
                if (o != null)
                {
                    process.StartInfo.Arguments = o.ToString();
                    Log("Устанавливаем аргументы файлу " + process.StartInfo.Arguments);
                }
            }
        }

        private int StartProcesses(string file_name)
        {
            List<string[]> user_creds = GetUserCreds();
            List<string> exit_codes = GetExitCodes();

            if (user_creds.NotNullOrEmpty())
            {
                user_creds.Add(new string[2]);
                foreach(string[] user_cred in user_creds)
                {
                    if (StartProcessOnPc(file_name, user_cred, out ExitCode))
                    {
                        if(exit_codes.NotNullOrEmpty())
                        {
                            if (exit_codes.Contains(ExitCode.ToString()))
                            {
                                return 0;
                            }
                        }else
                            return 0;
                    }
                }
                return -1;
            }
            else
            {
                if (StartProcessOnPc(file_name,null, out ExitCode))
                    return 0;
                else
                    return -1;
            }
        }

        private bool StartProcessOnPc(string file_name,string[] user_cred,out int ExCode)
        {
            try
            {
                Log("Запускаем процесс " + file_name);

                Process process = new Process();
                process.StartInfo.FileName = file_name;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.EnableRaisingEvents = true;
                process.StartInfo.WorkingDirectory = new FileInfo(file_name).Directory.FullName;

                SetArguments(ref process);
                SetCredentials(user_cred,ref process);

                process.Start();
                bool process_ended = process.WaitForExit(StaticConstants.RBINNER_CONFIG.GetProperty<int>("file_execution_timeout", 1800000));

                if (process_ended)
                {
                    if (process != null)
                    {
                        ExCode = process.ExitCode;    
                    }else
                        ExCode = 0;
                    
                    return true;
                }
                else
                {
                    ExCode = -2;
                    return false;
                }

                
            }catch(Exception ex)
            {
                Log(ex,"Ошибка при запуске файла " + InstallName);
                ExCode = -1;
                return false;
            }
        }

        private void SetCredentials(string[] user_cred,ref Process process)
        {
            if (user_cred.NotNullOrEmpty())
            {
                if (user_cred[0] != null && user_cred[1] != null)
                {
                    process.StartInfo.UserName = user_cred[0];
                    process.StartInfo.Password = returnPassword(user_cred[1]);
                    Log("Устанавливаем имя пользователя " + process.StartInfo.UserName);
                }
                else
                {
                    Log("Пробуем с пустыми реквизитами");
                }
            }
        }

        private SecureString returnPassword(string pass)
        {
            SecureString str = null;
            unsafe
            {
                char[] chArray =PasswordDecoder.decode_string(pass).ToCharArray();
                fixed (char* chRef = chArray)
                {
                    str = new SecureString(chRef, chArray.Length);
                }
            }
            return str;
        }

        private List<string> GetExitCodes()
        {
            if (Config != null)
            {
                return Config.GetProperties("exit_code").OfType<string>().ToList();
            }
            return null;
        }

        private List<string[]> GetUserCreds()
        {
            List<string[]> list = new List<string[]>();
            if (Config != null)
            {
                List<object> lo=Config.GetProperties("user_cred");
                foreach (object o in lo)
                {
                    list.Add(((string)o).Split(new string[]{"---"},StringSplitOptions.None));
                }
            }
            return list;
        }

        public bool PackageIsInstalled(string name)
        {
            return false;
        }

        public int InstallPackageFile(FileInfo file)
        {
            return StartProcess(file.FullName);            
        }

        public string CreatePackageName(FileInfo _file)
        {
            string name = "";
            DateTime dt=DateTime.Now;
            name = dt.Year + "_" + dt.Month + "_" + dt.Day +" "+_file.Name;
            
            //name += dt.ToOADate().ToString();
                //dt.Year + "_" + dt.Month + "_" + dt.Day + "_" + dt.Hour + "-" + dt.Minute + "-" + dt.Second + "_" + _file.Name;
            return name;
        }
    }
}

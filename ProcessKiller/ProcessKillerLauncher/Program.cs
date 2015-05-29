using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config_classes;
using System.Diagnostics;
using System.Security;
using CustomLogger;
using System.IO;
using System.Management;
using System.Threading;
using ProcessServiceWorker;

namespace ProcessKillerLauncher
{
    class Program
    {
        public static string successPath = "";

        internal static Logger logger;
        static void Main(string[] args)
        {
            logger = new Logger();
            logger.LoggerName = "ProcessKillerLauncher";
            logger.Log("Вошли");
            ConfigClass config = new ConfigClass("Config.xml");

            CheckOnly1Instance();

            while(true)
            try
            {

                object timeout = config.GetProperty("timeout_ms");
                object process_name = config.GetProperty("process_name");
                object killer_flag = config.GetProperty("killer_flag");

                object starter_flag = config.GetProperty("starter_flag");
                object starter_name = config.GetProperty("starter_name");

                //убиваем процесс
                if (killer_flag.ToString() == "1")
                {
                    ManagementScope scope = new ManagementScope(@"\\root\CIMV2");
                   // /scope.Connect();

                    ObjectQuery query = new ObjectQuery(
                        "SELECT * FROM Win32_Process");


                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                        //new ManagementObjectSearcher(scope, query);

                    ManagementObjectCollection theCollection = searcher.Get();

                    foreach (ManagementObject queryObj in theCollection)
                    {
                        //Console.WriteLine(queryObj["Name"]);
                        if (queryObj["Name"].ToString() == process_name.ToString())
                        {
                            queryObj.InvokeMethod("Terminate", null);
                            config.SetProperty("killer_flag","0");
                            break;
                        }
                    }
                }

                //запускаем процесс
                if (starter_flag.ToString() == "1")
                {
                 ProcessWorker.StartProcess(starter_name.ToString());                    
                 config.SetProperty("starter_flag", "0");
                }

                Thread.Sleep(int.Parse(timeout.ToString()));
            }
            catch (Exception ex)
            {
                logger.Log(ex.ToString());
            }
        }


        

        private static void CheckOnly1Instance()
        {
            // разрешен только один экземпляр программы
            System.Diagnostics.Process[] prc = 
                System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName);

            if (prc.Length > 1)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                logger.Log("Попытка запустить еще 1 экземпляр программы prkillerlauncher");
                return;
            }
        }

        public static void useWMI()
        {
            ConnectionOptions connection = new ConnectionOptions();
            connection.Username = "Администратор";
            connection.Password = " ";
            //connection.Password = "Qwerty11";
            connection.Authority = "ntlmdomain:MSK";

            ManagementScope scope = new ManagementScope(
                "\\\\10.11.0.236\\root\\CIMV2", connection); //scope.Connect(); //10 11 0 235  zvezdochka
            //"\\\\zvezdochka\\root\\CIMV2", connection);
            try { scope.Connect(); }catch (Exception ex)
            {
            }

            //scope = new ManagementScope("\\\\10.11.0.235\\root\\CIMV2", connection); scope.Connect(); //10 11 0 235  zvezdochka
           // scope = new ManagementScope("\\\\10.8.1.18\\root\\CIMV2", connection); scope.Connect(); //10 11 0 235  zvezdochka

            ObjectQuery query = new ObjectQuery(
                "SELECT * FROM Win32_Service");

            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject queryObj in searcher.Get())
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Win32_Service instance");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Caption: {0}", queryObj["Caption"]);
                Console.WriteLine("Description: {0}", queryObj["Description"]);
                Console.WriteLine("Name: {0}", queryObj["Name"]);
                Console.WriteLine("PathName: {0}", queryObj["PathName"]);
                Console.WriteLine("State: {0}", queryObj["State"]);
                Console.WriteLine("Status: {0}", queryObj["Status"]);
            }
        }
        static void Main1(string[] args)
        {


            useWMI();

                Logger logger = new Logger();
                logger.LoggerName = "ProcessKillerLauncher";

            try
            {
                logger.Log("Вошли");
                FileInfo fi = new FileInfo(logger.LogFileName);
                successPath = Path.Combine(fi.Directory.FullName, "success.ok");
                if (File.Exists(successPath)) File.Delete(successPath);

                ConfigClass config = new ConfigClass("Config.xml");

                object u_name_obj = config.GetProperty("username");
                object u_pass_obj = config.GetProperty("password");

                List<string> username = new List<string>();
                if (u_name_obj is string)
                {
                    username.Add((string)u_name_obj);
                }
                else
                {
                    username.AddRange((List<string>)u_name_obj);
                }

                List<string> password = new List<string>();
                if (u_pass_obj is string)
                {
                    password.Add((string)u_pass_obj);
                }
                else
                {
                    password.AddRange((List<string>)u_pass_obj);
                }

                string process_name = config.GetProperty("process_name").ToString();
                string killer_name = config.GetProperty("killer_name").ToString();
                logger.Log("Залили параметры");

                var pairs = from un in username
                            from pw in password
                            select new { uname = un, pass = pw };

                foreach (var pair in pairs)
                {
                    try
                    {
                        if (File.Exists(successPath))
                        {
                            break;
                        }
                        Process process = new Process();
                        process.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), killer_name);
                        //process.StartInfo.FileName = killer_name;

                        // process.StartInfo.UserName = pair.uname;
                        process.StartInfo.UserName = "Администратор";

                        SecureString theSecureString = new SecureString();

                        //pair.pass.ToCharArray().ToList().ForEach(a => theSecureString.AppendChar(a));
                        " ".ToCharArray().ToList().ForEach(a => theSecureString.AppendChar(a));

                        process.StartInfo.Password = theSecureString;
                        process.StartInfo.Arguments = process_name;
                        process.StartInfo.CreateNoWindow = false;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        process.StartInfo.UseShellExecute = false;
                        logger.Log("Запускаем приложение " + process_name);
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex, "Не удалось запустить приложение " + process_name);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex,"Ошибка");
            }
        }
        
    }
}

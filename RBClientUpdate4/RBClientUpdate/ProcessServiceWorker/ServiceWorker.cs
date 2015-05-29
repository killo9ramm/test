using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using Debug_classes;

namespace ProcessServiceWorker
{
    public class ServiceWorker
    {
        public static event Debug_classes.DebugPanel.MessageEventHandler LogEvent;

        public static void test()
        {
            ServiceController[] scs = ServiceController.GetServices();

            List<ServiceController> scs1 = scs.ToList().Where(a => a.ServiceName.IndexOf("Service1") != -1).ToList();
        }

        public static void StopService(string service)
        {
            ServiceController sc = null;
            try
            {
                sc = new System.ServiceProcess.ServiceController(service);
                sc.Stop();
                sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);
                Log("Service Stopped "+service);
            }
            catch(Exception ex)
            {
                Log("Service Stopped Error: " + service + " Error: "+ex.Message);
                if (ex.InnerException != null)
                {
                   Log("Service Stopped Inner Error: " + service + " Error: " + ex.InnerException.Message);
                }
                throw new CException(ex.Message);
            }
            finally
            {
                if(sc!=null)sc.Dispose();
            }
        }

        public static void StopService(List<string> service)
        {
            service.ForEach(a => StopService(a));
        }

        public static void StartService(string service)
        {

            ServiceController sc = null;
            try
            {
                sc = new System.ServiceProcess.ServiceController(service);
                sc.Start();
                sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);
                Log("Service Started " + service);
            }
            catch (Exception ex)
            {
                Log("Service Started Error: " + service + " Error: " + ex.Message);
                throw new CException(ex.Message);
            }
            finally
            {
                if (sc != null) sc.Dispose();
            }
        }

        public static void StartService(List<string> service)
        {
            service.ForEach(a => StartService(a));
        }

        private static void Log(string message)
        {
            if (null != LogEvent)
            {
                MessageEventArgs mess = new MessageEventArgs(message);
                LogEvent(null,mess);
            }
        }
    }

}


//static void Main(string[] args)
//        {
//            // Create an Instance of ServiceController
//            ServiceController myService = new ServiceController();
//            // Define the name of your service here. 
//            // I am using the 'ImapiService' for this example
//            // because everyone should have this service
//            // After this point, myService is now refering to "ImapiService"
//            myService.ServiceName = "ImapiService";
//            // Get the status of myService
//            // Possible Status Returns: { StartPending, Running, PausePending, Paused, 
//            //          StopPending, Stopped, ContinuePending }
//            // For more info on Service Status,
//            // go to: http://msdn.microsoft.com/en-us/library/
//            //             system.serviceprocess.servicecontrollerstatus.aspx
//            string svcStatus = myService.Status.ToString();
//            if (svcStatus == "Running")
//            {
//                Console.WriteLine(myService.ServiceName + " is in a " + 
//                                  svcStatus + "State");
//                Console.WriteLine("Attempting to Stop!");
//                myService.Stop();   // STOP the service if it is already Running
//                // This next block is for example only to show you the states 
//                // that the service is going through when it is stopping
//                string svcStatusWas = "";   // This is used for this example only
//                while (svcStatus != "Stopped")
//                {
//                    svcStatusWas = svcStatus;
//                    myService.Refresh();
//                    // REMEMBER: svcStatus was SET TO myService.Status.ToString above. 
//                    // Use the Refresh() Method to refresh the value of myService.Status and 
//                    // reassign it to svcStatus

//                    svcStatus = myService.Status.ToString();
//                }
//                Console.WriteLine("Service Stopped!!");
//            }
//            else if (svcStatus == "Stopped")
//            {
//                Console.WriteLine(myService.ServiceName + 
//                        " is in a " + svcStatus + "State");
//                Console.WriteLine("Attempting to Start!");
//                // START the service if it is already Stopped
//                myService.Start();
//                // This is used for this example only
//                string svcStatusWas = "";
//                while (svcStatus != "Running")
//                {
//                    if (svcStatus != svcStatusWas)
//                    // Check to see if the Staus is the same as it was before
//                    {
//                        Console.WriteLine("Status: " + svcStatus);
//                    }
//                    svcStatusWas = svcStatus;
//                    myService.Refresh();
//                    svcStatus = myService.Status.ToString();
//                }
//                Console.WriteLine("Service Started!!");
//            }
//            else
//            {
//                // STOP the service if it is in any other state
//                myService.Stop();
//                Console.WriteLine("Status: " + svcStatus);
//                while (svcStatus != "Stopped")
//                {
//                    myService.Refresh();
//                    svcStatus = myService.Status.ToString();
//                }
//                Console.WriteLine("Service Stopped!!");
//            }
//            // Notification that the program is going into a sleep state
//            Console.WriteLine("----Sleeping----");
//            // This is a way to pause your program. This is set to 30 seconds.
//            System.Threading.Thread.Sleep(30000);
//        }
//    }
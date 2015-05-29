using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomLogger;
using System.Threading;

namespace RBClient.Classes.CustomClasses
{
    class AsyncWorker:LoggerBase
    {
        Action act;
        public AsyncWorker(Action action)
        {
            act = action;
        }

        public bool IsBusy=false;

        private string _name="";
        public string Name
        {
            get { return _name;}
            set { _name=value+"_"+DateTime.Now.Ticks;}
        }

        public bool Run()
        {
            if (!IsBusy)
            {
                Log("запустил процесс");
                go();
                return true;
            }
            else
            {
                Log("занят");
            }
            return false;
        }

        public void RunAnyWay()
        {
            Log("запускаю метод принудительно");
            if (IsBusy)
            {
                Log("занят");
                Cancel();
                Run();
            }
            else
            {
                Log("свободен");
                Run();
            }
        }

        private void Log(Exception ex, string message)
        {
            message = "AsyncWorker " + Name + " " + message;
            base.Log(ex,message);
        }
        private void Log(string message)
        {
            message = "AsyncWorker " + Name + " " + message;
            base.Log(message);
        }

        public void Cancel()
        {
            Log("отменяем текущую задачу");
            if (IsBusy)
            {
                if (thread.IsAlive)
                {
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        Log("прекращаем поток");
                        ((Thread)o).Abort();
                    }, thread);
                }
                IsBusy = false;
            }
        }

        System.Threading.Thread thread;
        private void go()
        {
            IsBusy = true;
            thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                try
                {
                    act();
                    IsBusy = false;
                }catch(Exception ex)
                {
                    Log(ex);
                }

            })) { IsBackground = true };
            thread.Start();
        }
    }
}

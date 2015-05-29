using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomLogger;
using RBClient.Classes.InternalClasses.Models;
using System.Threading;


using System.Diagnostics;
using RBClient.Classes.CustomClasses;

namespace RBClient.Classes
{
    public enum TaskManagerState {Stopped=1,Timing=2,ExecutingTask=3};

    abstract class TaskManager : LoggerBase
    {
        private object loko=new object();
        public int Timeout = 300000;
        public string Name = "TaskManager";
        public bool CancelEnquire = false;

        protected TaskManagerState _ServiceState = TaskManagerState.Stopped;
        public TaskManagerState ServiceState { get{return _ServiceState;}}
        System.Threading.Timer timer;

        public TaskManager()
        {
            _ServiceState = TaskManagerState.Stopped;
            Name += DateTime.Now.Ticks.ToString();
            LLog("TaskManager {0} created",Name);
        }

        protected abstract bool ExecuteTask(object task);
        protected abstract object GetNextTask();
        protected abstract bool HasTasks();
        protected abstract void AddTask(object task);
        protected abstract void RemoveCurrentTask(object task);

        protected void PauseAndRestartService()
        {
            LLog("TaskManager {0} paused on {1}ms", Name, Timeout);
            if (timer != null)
                timer.Dispose();
            timer = new Timer(o => Start(), null, Timeout, System.Threading.Timeout.Infinite);
        }
        protected void StopService()
        {
            _ServiceState = TaskManagerState.Stopped;
            CancelEnquire = false;
            LLog("TaskManager {0} stopped", Name);
        }
        protected bool ServiceIsNotStarted()
        {
            if (ServiceState == TaskManagerState.Stopped)
            {
                return true;
            }
            LLog("TaskManager {0} is working now. Dont start again", Name);
            return false;
        }
        protected void Start()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    _ServiceState = TaskManagerState.ExecutingTask;
                    LLog("TaskManager {0} Start", Name);
                    if (CancelEnquire)
                    {
                        StopService();
                    }
                    else
                    {
                        LLog("TaskManager {0} check hastasks", Name);
                        if (HasTasks())
                        {
                            object task = null;
                            lock (loko)
                            {
                                LLog("TaskManager {0} GetNextTask", Name);

                                task = GetNextTask();
                            }
                            PerformTask(task);
                        }
                        else
                        {
                            StopService();
                        }
                    }
                }catch(Exception ex){
                    Log(ex, "TaskManager Start error");
                }
            },null);
        }
        protected void PerformTask(object task)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    LLog("TaskManager {0} PerformTask {1}", Name, task);
                    if (CancelEnquire)
                    {
                        StopService();
                    }
                    else
                    {
                        LLog("TaskManager {0} ExecuteTask {1}", Name, task);
                        if (!ExecuteTask(task))
                        {
                            _ServiceState = TaskManagerState.Timing;
                            PauseAndRestartService();
                        }
                        else
                        {
                            RemoveCurrentTask(task);
                            _ServiceState = TaskManagerState.Stopped;
                            Start();
                        }
                    }
                }catch(Exception ex){
                    Log(ex, "TaskManager PerformTask error");
                }
            }, null);
        }

        public void StartService()
        {
            if (ServiceIsNotStarted())
            {
                Start();
            }
        }
        public void EnqeueTask(object task)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    if (!IsDublicate(task))
                    {
                        lock (loko)
                        {
                            LLog("TaskManager {0} AddTask {1}", Name, task);
                            AddTask(task);
                        }
                        StartService();
                    }
                }catch(Exception ex)
                {
                    Log(ex, "TaskManager EnqeueTask error");
                }
            }, null);
        }
        
        protected void LLog(string message, params object[] para)
        {
            Log(String.Format(message, para));
        }

        public virtual bool IsDublicate(object task)
        {
            LLog("TaskManager {0} check for dublicates tasks {1}", Name,task);
            return false;
        }

    }


    class TestManager : TaskManager
    {
        List<int> queue = new List<int>();
        protected override object GetNextTask()
        {
            return queue.First();
        }

        protected override bool ExecuteTask(object task)
        {
            if (task.ToString() == "3" || task.ToString() == "6")
            {
                Debug.WriteLine("\r\nExecuting error: " + task);
                return false;
            }
            Debug.WriteLine("\r\nExecuting: "+task);
            Thread.Sleep(500);
            return true;
        }

        protected override bool HasTasks()
        {
            return queue.Count() > 0;
        }
        protected override void AddTask(object task)
        {
            queue.Add(int.Parse(task.ToString()));
        }
        protected override void RemoveCurrentTask(object task)
        {
            queue.Remove((int)task);
        }
    }




    class WebTaskManager : TaskManager
    {

        public int MaxDaysTries = StaticConstants.RBINNER_CONFIG.GetProperty<int>("task_valid_through_all_others_days", 5);

        private t_WebServiceTask CheckTask(object task)
        {
            var temp = task as t_WebServiceTask;
            if (temp == null)
            {
                throw new ArgumentException("Task is not valid class", "Task");
            }
            return temp;
        }


        public void TaskFail(t_WebServiceTask tas, string error)
        {
            MDIParentMain.ExecuteTryAction(() =>
            {
                if (tas != null)
                {
                    tas.failed = true;
                    tas.triescount++;
                    tas.taskupdatedate = DateTime.Now;
                    tas.description += String.Format("---- {0} error {1}", tas.taskupdatedate, error);
                    tas.UpdateOle();
                }
                else
                {
                    Log("task is null");
                }
            });
        }

        public void TaskSucceed(t_WebServiceTask tas)
        {
            tas.succed = true;
            tas.failed = false;
            tas.taskupdatedate = DateTime.Now;
            tas.UpdateOle();
        }


        protected override bool ExecuteTask(object task)
        {
            t_WebServiceTask tas = null;
            try
            {
                tas = CheckTask(task);
                var o=Serializer.BinaryDeSerialize(tas.taskparams) as object[];
                if (o == null) {
                    Log(String.Format("Error in task params {0} taskparams is null", tas.id));
                    return false; }

                if (IsTaskValid((t_WebServiceTask)task))
                {
#if(!DEBUG)
                    var result = RbClientGlobalStaticMethods.CallWebFunction(StaticConstants.WebServiceSystem, tas.funcname, o);

                    if (result.ToString().IndexOf("1") == -1)
                    {
                        TaskFail(tas, result.ToString());
                        return false;
                    }
                    else
                    {
                        TaskSucceed(tas);
                    }
                    
#else
                    TaskSucceed(tas);
#endif
                }
                return true;
            }
            catch (Exception ex)
            {
                TaskFail(tas, ex.Message);
                return false;
            }
        }

        protected override object GetNextTask()
        {
            try
            {
                var tasks = GetCurrTasks();
                var taskss = tasks.OrderByDescending(a => a.priority).ThenByDescending(a => a.taskdate);

                if (tasks.NotNullOrEmpty())
                {
                    var task = taskss.First();
                    return task;
                }
                else
                    return null;
            }catch(Exception ex)
            {
                Log(ex, "GetNextTask() error");
                return null;
            }
        }

        private bool IsTaskValid(t_WebServiceTask task)
        {
            int minDelay = 0;

            minDelay = GetFuncMinuteDelay(task.funcname);
            if (minDelay != 0)
            {
                var tasks = GetAllUnSuccessTasks(task.priority, minDelay);
                if (tasks.NotNullOrEmpty())
                {
                    tasks.ForEach(a => SwitchOffTask(a));
                    return false;
                }
            }
            return true;
        }

        private void SwitchOffTask(t_WebServiceTask task)
        {
            try
            {
                task.succed = true;
                task.failed = true;
                task.taskupdatedate = DateTime.Now;
                task.description = "not recent task";
                task.UpdateOle();
            }catch(Exception ex)
            {
                Log(ex, "SwitchOffTask error task:"+Serializer.JsonSerialize(task));
            }
        }

        private List<t_WebServiceTask> GetAllUnSuccessTasks(int priority, int minDelay)
        {
            var tasks = new t_WebServiceTask().Select<t_WebServiceTask>("succed=0 AND priority="+priority+" AND taskdate<"
                + SqlWorker.ReturnDate(DateTime.Now.AddMinutes(-minDelay)));
            return tasks;
        }

        private int GetFuncMinuteDelay(string funcname)
        {
            return StaticConstants.RBINNER_CONFIG.GetProperty<int>(String.Format("task_valid_through_{0}", funcname), 0);
        }

        private List<t_WebServiceTask> GetCurrTasks()
        {
            var tasks =
                new t_WebServiceTask().Select<t_WebServiceTask>("servicename='WebServiceSystem' AND succed=0 AND taskdate>"
                + SqlWorker.ReturnDate(DateTime.Now.AddDays(-MaxDaysTries)));

            var taskss =
                new t_WebServiceTask().Select<t_WebServiceTask>("servicename='WebServiceSystem' AND succed=0 AND taskdate<"
                + SqlWorker.ReturnDate(DateTime.Now.AddDays(-MaxDaysTries)));
            if (taskss.NotNullOrEmpty())
            {
                taskss.ForEach(a => SwitchOffTask(a));
            }

            return tasks;
        }

        public List<t_WebServiceTask> GetCurrTasks(string servicename)
        {
            var tasks =
                new t_WebServiceTask().Select<t_WebServiceTask>("servicename='" + servicename + "' AND succed=0 AND taskdate>"
                + SqlWorker.ReturnDate(DateTime.Now.AddDays(-MaxDaysTries)));

            var taskss =
                new t_WebServiceTask().Select<t_WebServiceTask>("servicename='" + servicename + "' AND succed=0 AND taskdate<"
                + SqlWorker.ReturnDate(DateTime.Now.AddDays(-MaxDaysTries)));
            if (taskss.NotNullOrEmpty())
            {
                taskss.ForEach(a => SwitchOffTask(a));
            }

            return tasks;
        }

        private List<t_WebServiceTask> GetAllUnSuccessTasks(string query)
        {
            List<t_WebServiceTask> tasks = new List<t_WebServiceTask>();
            if (query != "")
            {
                tasks = new t_WebServiceTask().Select<t_WebServiceTask>("succed=0 AND " + query);
            }
            else
            {
                tasks = new t_WebServiceTask().Select<t_WebServiceTask>("succed=0");
            }
            return tasks;
        }

        protected override bool HasTasks()
        {
            try
            {
                var tasks = GetCurrTasks();
                if (tasks.NotNullOrEmpty())
                {
                    return true;
                }
                return false;
            }catch(Exception ex)
            {
                Log(ex, "HasTasks() error");
                return false;
            }
        }

        protected override void RemoveCurrentTask(object task)
        {
           
        }


        protected override void AddTask(object task)
        {
            try
            {
                var tas = CheckTask(task);
                AddWsTask(tas);
            }
            catch(Exception ex)
            {
                Log(ex);
            }
        }
        public void AddWsTask(t_WebServiceTask task)
        {
            task.taskdate = DateTime.Now;
            task.taskupdatedate = task.taskdate;
            task.CreateOle();
        }

        public override bool IsDublicate(object task)
        {
            try
            {
                t_WebServiceTask temp = CheckTask(task);
                if (DobleAction(temp))
                {
                    return true;
                }
                return false;
            }catch(Exception ex)
            {
                Log(ex);
                return true;
            }
        }

        private bool DobleAction(t_WebServiceTask task)
        {
            if (task.funcname == "SetTeremokVersion")
            {
                var list=GetAllUnSuccessTasks("funcname='SetTeremokVersion'");
                if (list.NotNullOrEmpty())
                {
                    list.ForEach(a => SwitchOffTask(a));
                }                
            }
            return false;
        }

        public void EnqeueTask(string webservice_name, int webservice_priority, string funcname, object[] taskparams, int priority)
        {
            t_WebServiceTask task = CreateTaskTab(webservice_name, webservice_priority, funcname, taskparams, priority);
            base.EnqeueTask(task);
        }

        public t_WebServiceTask CreateTaskTab(string webservice_name, int webservice_priority, string funcname, object[] taskparams, int priority)
        {
            t_WebServiceTask task = new t_WebServiceTask();
            task.servicename = webservice_name;
            task.servicepriority = webservice_priority;
            task.funcname = funcname;
            task.taskparams = Serializer.BinarySerialize(taskparams);
            task.priority = priority;

            return task;
        }

    }
}
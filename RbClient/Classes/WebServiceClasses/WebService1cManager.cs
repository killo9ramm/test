using RBClient.Classes.DocumentClasses;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes.OrderClasses;
using RBClient.ru.teremok.msk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient.Classes.WebServiceClasses
{
    class WebService1cManager : LoggerBaseMdi
    {
        internal t_WebServiceTask EnqeueTask(t_SerializationHip docstate)
        {
            t_WebServiceTask twstask = CreateTaskTab(docstate);
            t_WebServiceTask existingTask = null;

            if (CheckIfTaskAlreadyExist(twstask, ref existingTask))
            {
                UpdateWebTask(ref twstask, existingTask);
            }
            else
            {
                CreateWebTask(ref twstask);
            }
            return twstask;
        }

        private bool CheckIfTaskAlreadyExist(t_WebServiceTask twstask,ref t_WebServiceTask existingTask)
        {
            existingTask = new t_WebServiceTask().SelectFirst<t_WebServiceTask>(String.Format("succed=0 AND funcname='{0}' AND servicename='{1}' AND related_doc_id={2}",
                twstask.funcname, twstask.servicename, twstask.related_doc_id));

            if (existingTask == null)
                return false;
            return true;
        }

        private void UpdateWebTask(ref t_WebServiceTask twstask, t_WebServiceTask existingTask)
        {
            if (NeedTaskUpdation(twstask,existingTask))
            {
                UpdateWebTaskInDb(ref twstask, existingTask);
            }
        }

        private bool NeedTaskUpdation(t_WebServiceTask twstask, t_WebServiceTask existingTask)
        {
            return twstask.taskparams == existingTask.taskparams;
        }

        private void UpdateWebTaskInDb(ref t_WebServiceTask twstask,t_WebServiceTask existingTask)
        {
            twstask.taskparams = existingTask.taskparams;
        }

        private void CreateWebTask(ref t_WebServiceTask twstask)
        {
            twstask.CreateOle();
        }

        public t_WebServiceTask CreateTaskTab(t_SerializationHip docstate)//string webservice_name, int webservice_priority, string funcname, object[] taskparams, int priority)
        {
            t_WebServiceTask task = new t_WebServiceTask();
            task.servicename = WebService1CStateClass.SERVICETYPE;
            task.funcname = docstate.object_name;
            task.taskparams = docstate.object_data;
            task.related_doc_id = docstate.related_doc_id;
            return task;
        }

        internal bool Execute1Task(t_WebServiceTask wtask)
        {
            string error = "";
            if (ExecuteWebServiceMethod(wtask,ref error))
            {
                StaticConstants.WebTaskManager.TaskSucceed(wtask);
                UpdateDocument(wtask);
                return true;
            }
            else
            {
                StaticConstants.WebTaskManager.TaskFail(wtask, error);
                return false;
            }
        }

        private bool ExecuteWebServiceMethod(t_WebServiceTask wtask,ref string error)
        {
            Exception ex=null;
            StaticHelperClass.CallMethodEx<ARMWeb>(StaticConstants.WebService, wtask.funcname, (object[])Serializer.BinaryDeSerialize(wtask.taskparams),out ex);
            if (ex == null)
            {
                return true;
            }
            else
            {
                error = ex.Message;
                return false;
            }
        }

        private void UpdateDocument(t_WebServiceTask wtask)
        {
            OrderClass order = OrderClass.CreateOrderClass(wtask.related_doc_id);
            order.Document_SetAsSended();
        }

        internal List<t_WebServiceTask> GetCurrentTasks()
        {
            List<t_WebServiceTask> _list = new t_WebServiceTask().Select<t_WebServiceTask>(String.Format("servicename='{0}' AND succed=0", WebService1CStateClass.SERVICETYPE));
            return _list;
        }
    }
}

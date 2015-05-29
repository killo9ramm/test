using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes.InternalClasses.Models;
using System.Reflection;

namespace RBClient.Classes
{
    public interface IWebServiceExchanger
    {
         void MakeTask(string teremok_id,int doc_id,object content);
         void SendDocuments(int teremok_id);
         void RemoveTask(string teremok_id, int doc_id);
    }
    public class WebServiceExchanger : IWebServiceExchanger
    {
        private List<t_TaskExchangeWeb> _inner_tasks;
        private List<t_TaskExchangeWeb> inner_tasks
        {
            get
            {
                if (_inner_tasks == null)
                {
                    _inner_tasks=new List<t_TaskExchangeWeb>();
                }
                return _inner_tasks;
            }
        }

        public bool Working = false;

        private static WebServiceExchanger _Instance;
        public static WebServiceExchanger Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new WebServiceExchanger();
                }
                return _Instance;
            }
        }

        public void MakeTask(string teremok_id, int doc_id, object content)
        {
            byte[] cont = Serializer.BinarySerialize(content);

            t_TaskExchangeWeb web_task = new t_TaskExchangeWeb().SelectFirst<t_TaskExchangeWeb>("doc_id="+doc_id+" AND teremok_id="+teremok_id);
            if (web_task != null) web_task.Delete();

            web_task = new t_TaskExchangeWeb();
            web_task.teremok_id = int.Parse(teremok_id);
            web_task.doc_id = doc_id;
            web_task.doc_content = cont;
            web_task.CreateOle();

            MDIParentMain.Log("Создана задача по отправке документа по вебсервису. doc_id="+doc_id+" teremok_id="+teremok_id);
        }

        public void SendDocuments(int teremok_id)
        {
            if (!Working)
            {
                Working = true;
                List<t_TaskExchangeWeb> web_tasks = new t_TaskExchangeWeb().Select<t_TaskExchangeWeb>("teremok_id=" + StaticConstants.Teremok_ID);
                if (web_tasks != null && web_tasks.Count > 0)
                {
                    _inner_tasks = web_tasks;
                    web_tasks.ForEach(a =>
                    {
                        try
                        {
                            MainProgressReport.Instance.ReportProgress("Отправляем документ №"+a.doc_id+" по WebService");
                            SendDocument(int.Parse(StaticConstants.Teremok_ID), a.doc_id, a.doc_content, "1");
                            MainProgressReport.Instance.ReportProgress("Документ №" + a.doc_id + " отправлен!");
                        }
                        catch (Exception ex)
                        {
                            MainProgressReport.Instance.ReportProgress("Документ №" + a.doc_id + " не удалось отправить!");
                            MDIParentMain.Log(ex, "Не удалось отправить докумет по вебсервису t_taskExchangeWebId=" + a.id);

                        }
                    });
                }
                Working = false;
            }
            else
            {
                MDIParentMain.Log("Отправку по вебсервису не производим т.к. она уже идет SendDocuments");
            }
        }

        public void SendDocument(int teremok_ID,int doc_id,byte[] doc_content,object right_result)
        {
            //десериализовать контент 
            object content = Serializer.BinaryDeSerialize(doc_content);
            //определить тип контента
            Type content_type=content.GetType();
            MethodInfo web_method= StaticHelperClass.FindMethodBySignature(StaticConstants.WebService,"",new Type[]{typeof(int),content_type});

            object result=web_method.Invoke(StaticConstants.WebService,new object[]{teremok_ID,content});
            
            //обновить статус документов

            StaticConstants.CBData.DocUpdateDocStateNew(doc_id, 3, "Отправлен ");

            //if(result.Equals(right_result)) 
            RemoveTask(teremok_ID.ToString(), doc_id);
        }

        public void RemoveTask(string teremok_id, int doc_id)
        {
            //удалить из таблиц//удалить запись из таблиц t_taskexchange t_taskExchangeWeb
            t_TaskExchangeWeb web_task = new t_TaskExchangeWeb().SelectFirst<t_TaskExchangeWeb>("teremok_id="+teremok_id+
                " AND doc_id="+doc_id);
            if (web_task != null) web_task.Delete();

            t_TaskExchange task = new t_TaskExchange().SelectFirst<t_TaskExchange>("task_doc_id="+doc_id);
            if (task != null) task.Delete();
        }
    }
}

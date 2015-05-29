using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBServer.Debug_classes;
using NLog;
using Config_classes;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace RBClientUpdateApplication
{
    public class Root
    {
         static Root()
        {
            ConfigClass.LoadConfigFile();
        }

        public const string TEMP_FOLDER_NAME = "TEMP";

        //подключить статическое логирование

        public delegate void MessageEventHandler(object o, MessageEventArgs e);
        public static event MessageEventHandler LogEvent;
        public static event MessageEventHandler TraceEvent;

        public static void Log(string message)
        {
            Log(null, message);
        }

        public static void Log(Exception ex,string message)
        {

            string messagee = message;
            if (ex != null)
            {
                messagee += " exception: " + ex.Message;

                if (null != LogEvent)
                {
                    MessageEventArgs mess = new MessageEventArgs(messagee);
                    LogEvent(null, mess);
                }
            }
            else
            {
                if (null != TraceEvent)
                {
                    MessageEventArgs mess = new MessageEventArgs(messagee);
                    TraceEvent(null, mess);
                }
                else
                {
                    if (null != LogEvent)
                    {
                        MessageEventArgs mess = new MessageEventArgs(messagee);
                        LogEvent(null, mess);
                    }
                }
            }
        }

        public static void Trace(string message)
        {
            if (null != TraceEvent)
            {
                MessageEventArgs mess = new MessageEventArgs(message);
                TraceEvent(null, mess);
            }
        }

        private static RBMEntities _rbm = null;
        public static RBMEntities RBM
        {
            get {
                if (_rbm == null)
                {
                    string connectionString = ConfigClass.GetProperty("DataBaseConnectionString").ToString();
                    try
                    {
                        _rbm = new RBMEntities(connectionString);
                    }
                    catch(Exception exp)
                    {
                        Log(exp, "Не получается открыть соединение по строке из конфигурации");
                        _rbm = new RBMEntities();
                    }
                }
                return _rbm;
            }
        }

        public static List<UpdateItem> _itemsList = null;
        public static List<UpdateItem> ItemsList
        {
            get
            {
                try
                {
                    if (_itemsList == null)
                    {
                        //List<t_Teremok> teremok_list = Root.RBM.t_Teremok.Where<t_Teremok>(a => a.teremok_use_ARM).OrderBy(a => a.teremok_name).ToList();
                        string t_city = ConfigClass.GetProperty("City").ToString();

                        List<t_Teremok> teremok_list = Root.RBM.t_Teremok.Where<t_Teremok>(a => a.teremok_use_ARM && ((bool)a.deleted != true || a.deleted==null) && (a.teremok_address != "" || a.teremok_address != null)
                            && a.teremok_city == t_city)
                  .OrderBy(a => a.teremok_name).ToList();
                    //    List<t_Teremok> teremok_list = Root.RBM.t_Teremok.Where<t_Teremok>(a => a.teremok_use_ARM && !(bool)a.deleted && (a.teremok_address != "" || a.teremok_address != null))
                    //.OrderBy(a => a.teremok_name).ToList();

                        Root.CreateMainItemsList(teremok_list);
                    }
                    return _itemsList;
                }
                catch (Exception exp)
                {
                    Log(exp, "Не могу получить список ресторанов!");
                    if (exp.InnerException != null)
                    {
                        Log(exp.InnerException, "Не могу получить список ресторанов!");
                    }
                    throw exp;
                }
            }
        }

        internal static void CreateMainItemsList(List<t_Teremok> teremok_list)
        {
            _itemsList = new List<UpdateItem>();
            try
            {
                teremok_list.ForEach(a => {
                    _itemsList.Add(new UpdateItem(a));
                });

                //применить свойства
                List<string> l_from_prop=Properties.Settings.Default.SelectedItems.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

                _itemsList.ForEach(a =>
                    {
                        if (l_from_prop.Contains(a.Teremok_1c_name)) a.RestControl.IsChecked = true;
                    });
            }
            catch (Exception exp)
            {
                Log(exp,"Не могу получить список ресторанов!");
                throw new CException("Не удалось сформировать лист со списком Items", exp);
                
            }
        }

        internal static bool RBUCopy = false;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomLogger;

namespace RBClient.Classes.CustomClasses
{
    partial class UpdateClass : LoggerBase, IDisposable
    {

        public string InstallName;
        public static object OperatingObject;
        public static List<UpdateClass> UpdationList = new List<UpdateClass>();


        public static UpdateClass CreateUpdateTask(object o)
        {
            if (UpdateClass.UpdationList.Where(a => a.UpdationType.Equals(o.GetType())).Count() == 0)
            {
                UpdateClass updcls = new UpdateClass() { LogEvent = MDIParentMain.Log, UpdationType = o.GetType() };
                MDIParentMain.Log("Создаем класс обновления для типа " + o.GetType().Name);
                UpdateClass.UpdationList.Add(updcls);

                return updcls;
            }
            else
            {
                MDIParentMain.Log("Обновление для типа " + o.GetType().Name + " не запускаем! Оно уже идет!");
                return null;
            }
        }

        public static UpdateClass CreateUpdateTaskStr(object o)
        {
            if (UpdateClass.UpdationList.Where(a => a.UpdationTypeStr.Equals(o.ToString())).Count() == 0)
            {
                UpdateClass updcls = new UpdateClass() { LogEvent = MDIParentMain.Log, UpdationTypeStr = o.ToString()};
                MDIParentMain.Log("Создаем класс обновления для типа " + o.GetType().Name);
                UpdateClass.UpdationList.Add(updcls);

                return updcls;
            }
            else
            {
                MDIParentMain.Log("Обновление для типа " + o.ToString() + " не запускаем! Оно уже идет!");
                return null;
            }
        }
    }
}

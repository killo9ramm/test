using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.ComponentModel;

namespace RBClient.Classes.InternalClasses
{
    class ClassFactory
    {
        public static object SpecialParse(Type type, string val)
        {
            var converter = TypeDescriptor.GetConverter(type);
            object result = converter.ConvertFrom(val);
            return result;
        }

        /// <summary>
        /// Устанавливаем свойство экземпляру класса
        /// </summary>
        /// <param name="obj">Экземпляр класса</param>
        /// <param name="value">Значение свойства</param>
        /// <param name="property_index">индекс свойства</param>
        public static void SetValueToClassProperty(object obj,object value,int property_index)
        {
            try{

                Type type = obj.GetType();
                List<FieldInfo> pinfos =  type.GetFields().OrderBy(field => field.MetadataToken).ToList();

                pinfos[property_index].SetValue(obj,value);
            
            }
            catch (Exception ex)
            {
                Log("Не удалось присвоить свойство типу " + obj.GetType().ToString(), ex);
            }
        }


        /// <summary>
        /// получает значение поля по индексу и имя этог поля
        /// </summary>
        /// <param name="obj">обрабатываемый класс</param>
        /// <param name="property_index">индекс поля</param>
        /// <param name="prop_name">имя поля</param>
        /// <returns></returns>
        public static object GetValueFromClassProperty(object obj, int property_index, out string prop_name)
        {
            try
            {
                Type type = obj.GetType();
                List<FieldInfo> pinfos = type.GetFields().OrderBy(field => field.MetadataToken).ToList();

                prop_name=pinfos[property_index].Name;

                return pinfos[property_index].GetValue(obj);

            }
            catch (Exception ex)
            {
                Log("Не удалось присвоить свойство типу " + obj.GetType().ToString(), ex);
                prop_name = null;
                return null;
            }
        }

        public static T CreateClass<T>(DataRow dr) where T:new()
        {
            try
            {
                T _class = new T();
                Type type = _class.GetType();

                List<FieldInfo> pinfos =  type.GetFields().OrderBy(field => field.MetadataToken).ToList();
                foreach (FieldInfo pi in pinfos)
                {
                    //по полю найти значение ячейки
                    object value = CellHelper.FindCell(dr, pi.Name);

                    //если оно не нуль
                    if (!(value is System.DBNull))
                    {
                        pi.SetValue(_class, value);
                    }
                }
                return _class;
            }
            catch(Exception ex)
            {
                Log("Не удалось создать класс "+typeof(T).ToString(),ex);
                return default(T);
            }
        }

        public static object CreateClass(DataRow dr,Type type)
        {
            try
            {
                object _class = Activator.CreateInstance(type);
                List<FieldInfo> pinfos =  type.GetFields().OrderBy(field => field.MetadataToken).ToList();
                foreach (FieldInfo pi in pinfos)
                {
                    //по полю найти значение ячейки
                    object value = CellHelper.FindCell(dr, pi.Name);

                    //если оно не нуль
                    if (!(value is System.DBNull))
                    {
                        pi.SetValue(_class, value);
                    }
                }
                return _class;
            }
            catch (Exception ex)
            {
                Log("Не удалось создать класс " + type.ToString(), ex);
                return null;
            }
        }


        public static List<T> CreateClasses<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T _class = CreateClass<T>(dr);
                list.Add(_class);
            }
            return list;
        }

        public static List<object> CreateClasses(DataTable dt,Type type)
        {
            List<object> list = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                object _class = CreateClass(dr,type);
                list.Add(_class);
            }
            return list;
        }

        #region logging
        /// <summary>
        /// Делегат лога в него можно добавлять события логов
        /// </summary>
        public static NLogDelegate LogEvent; 
        private static NLogDelegate _LogEventSingle;
        /// <summary>
        /// Одинарное событие лога, при определении очищает все остальные
        /// </summary>
        public static NLogDelegate LogEventSingle
        {
            set
            {
                LogEvent = null;
                _LogEventSingle = null;
                _LogEventSingle = value;
            }
            get
            {
                return _LogEventSingle;
            }
        }

        private static void WriteLog(string message)
        {
            if (LogEventSingle!=null)
            {
                LogEventSingle(message);
            }
            if (LogEvent!=null)
            {
                LogEvent(message);
            }
        }
        /// <summary>
        /// выводим лог
        /// </summary>
        /// <param name="message"></param>
        private static void Log(string message)
        {
            WriteLog(message);
        }

        /// <summary>
        /// выводим лог
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exp"></param>
        private static void Log(string message, Exception exp)
        {
             WriteLog(message + " exception: " + exp.Message);
        }

        /// <summary>
        /// выводим лог
        /// </summary>
        /// <param name="exp"></param>
        private static void Log(Exception exp)
        {
            WriteLog(exp.Message);
        }

        #endregion
    }
}

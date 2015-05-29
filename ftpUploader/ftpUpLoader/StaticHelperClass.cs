using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;




namespace RBClient.Classes
{
    public class StaticHelperClass
    { 
       

        /// <summary>
        /// Возвращает из класса указанное свойство
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nclass"></param>
        /// <param name="prop_name"></param>
        /// <returns></returns>
        public static object ReturnClassItemValue<T>(T nclass, string prop_name) where T:new()
        {
            try
            {
                Type type = nclass.GetType();
                FieldInfo pi = type.GetField(prop_name);

                object o = pi.GetValue(nclass);
                return o;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Возвращает из класса указанное статическое свойство
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nclass"></param>
        /// <param name="prop_name"></param>
        /// <returns></returns>
        public static object ReturnStaticClassFieldValue(Type static_class_type, string prop_name)
        {
            try
            {
               // Type type = static_class_type.GetType();
                FieldInfo pi = static_class_type.GetField(prop_name);

                object o = pi.GetValue(null);
                return o;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Устанавливает указанное свойство
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nclass"></param>
        /// <param name="prop_name"></param>
        /// <param name="prop_value"></param>
        public static void SetClassItemValue<T>(T nclass, string prop_name, object prop_value) where T : new()
        {
            try
            {
                Type type = nclass.GetType();
                FieldInfo pi = type.GetField(prop_name);
                if (pi != null)
                {
                    pi.SetValue(nclass, prop_value);
                    return;
                }

                PropertyInfo _pi = type.GetProperty(prop_name);
                if (_pi != null)
                {
                    _pi.SetValue(nclass, prop_value, null);
                    return;
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Устанавливает свойство классу
        /// </summary>
        /// <param name="T"></param>
        /// <param name="nclass"></param>
        /// <param name="prop_name"></param>
        /// <param name="prop_value"></param>
        public static void SetClassItemValue(Type T,object nclass, string prop_name, object prop_value)
        {
            try
            {
                Type type = T;
                
                FieldInfo pi = type.GetField(prop_name);
                if (pi != null)
                {
                    pi.SetValue(nclass, prop_value);
                    return;
                }

                PropertyInfo _pi = type.GetProperty(prop_name);
                if (_pi != null)
                {
                    _pi.SetValue(nclass, prop_value, null);
                    return;
                }

            }
            catch (Exception ex)
            {
            }
        }


       
    }
    /// <summary>
    /// Энумератор состояний
    /// </summary>
    public enum StateEnum { Error = -3, Created = -2, NotSatrted = -1, Started = 0, Working = 1, RetryAction = 2, ErrorTriesEnded = -4, SuccessfulComplete = 5 };
}

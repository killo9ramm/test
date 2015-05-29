using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Windows.Forms;
using System.Collections;



namespace RBClient.Classes
{
    public class StaticHelperClass
    { 
       
        /// <summary>
        /// возвращает цвет из строки
        /// </summary>
        /// <param name="color_str"></param>
        /// <returns></returns>
        public static System.Drawing.Color ReturnColorWF(string color_str)
        {
            System.Drawing.Color color = System.Drawing.Color.FromArgb(0, 0, 0);

            try
            {
                byte b = Convert.ToByte(color_str.Substring(0, 2), 16);
                byte g = Convert.ToByte(color_str.Substring(2, 2), 16);
                byte r = Convert.ToByte(color_str.Substring(4, 2), 16);
                color = System.Drawing.Color.FromArgb(r, g, b);
            }
            catch (Exception ex)
            {

            }
            return color;
        }

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
        /// Возвращает числовой хэш перебирая свойства
        /// </summary>
        /// <param name="o">объект</param>
        /// <returns></returns>
        public static long GetHashCodeByProperties(object o)
        {
            try
            {
                if (o == null) return - 1;
                Type type = o.GetType();
                string hash = "";
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (pi.GetValue(o, null)!=null)
                    hash += pi.GetValue(o, null).ToString();
                }

                long hashl = 0;

                foreach (char c in hash)
                {
                    //сделать возврат пустого значения
                    hashl += (int)c;
                }
                return hashl;

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// Возвращает числовой хэш перебирая свойства
        /// </summary>
        /// <param name="o">объект</param>
        /// <returns></returns>
        public static long GetHashCodeByFields(object o)
        {
            try
            {
                if (o == null) return -1;
                Type type = o.GetType();
                string hash = "";
                foreach (FieldInfo pi in type.GetFields())
                {
                    if (pi.GetValue(o) != null)
                        hash += pi.GetValue(o).ToString();
                }

                long hashl = 0;

                foreach (char c in hash)
                {
                    //сделать возврат пустого значения
                    hashl += (int)c;
                }
                return hashl;

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// Возвращает из класса указанное свойство
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nclass"></param>
        /// <param name="prop_name"></param>
        /// <returns></returns>
        public static object ReturnClassItemValue(object nclass, int prop_index)
        {
            try
            {
                Type type = nclass.GetType();


                FieldInfo pi = type.GetFields().OrderBy(field => field.MetadataToken).ToList()[prop_index];
                if (pi != null)
                {
                    object o = pi.GetValue(nclass);
                    return o;    
                }

                PropertyInfo _pi = type.GetProperties()[prop_index];
                if (_pi != null)
                {
                    object o = pi.GetValue(nclass);
                    return o;
                    
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
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

        /// <summary>
        /// Устанавливает свойство классу
        /// </summary>
        /// <param name="T"></param>
        /// <param name="nclass"></param>
        /// <param name="prop_name"></param>
        /// <param name="prop_value"></param>
        public static void SetClassItemValue(Type T, object nclass, int prop_index, object prop_value)
        {
            try
            {
                Type type = T;

                FieldInfo pi = type.GetFields()[prop_index];//(prop_name);
                if (pi != null)
                {
                    pi.SetValue(nclass, prop_value);
                    return;
                }

                PropertyInfo _pi = type.GetProperties()[prop_index];
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

        public static object CallMethod<T>(T nclass, string method_name, object[] param_s) where T : new()
        {
            try
            {
                object o = null;
                Type type = nclass.GetType();
                MethodInfo pi = type.GetMethod(method_name);

                List<string> ls=
                    type.GetMethods().Select(a => a.Name).Where(a => a.IndexOf("Click") != -1).ToList();

                if (pi != null)
                {
                    o=pi.Invoke(nclass, param_s);
                }
                return o;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Находит метод по сигратуре
        /// </summary>
        /// <param name="o">объект искомого типа</param>
        /// <param name="method_name">часть имени метода</param>
        /// <param name="param_s">список параметров</param>
        /// <returns></returns>
        public static MethodInfo FindMethodBySignature(object o, string method_name, Type[] param_s)
        {
            try
            {
                Type type = o.GetType();
                List<MethodInfo> mi_list = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).ToList();
                MethodInfo method=null;

                foreach (MethodInfo mi in mi_list)
                {
                    if (method_name != "") 
                    {
                        if (mi.Name.IndexOf(method_name) == -1) continue;
                    }
                    List<Type> params_types_list = mi.GetParameters().Select(a=>a.ParameterType).ToList();
                    if (params_types_list.Count != param_s.Length) continue;

                    bool flag=true;

                    param_s.ToList().ForEach(a =>
                    {
                        if (!params_types_list.Contains(a)) flag = false;
                    });

                    if (flag) { method = mi; break; }
                }
                return method;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static object ExecuteMethodByName(object o, string method_name,BindingFlags flags, object[] param_s)
        {
            try
            {
                Type type = o.GetType();
                List<MethodInfo> mi_list = type.GetMethods(flags).ToList();
                MethodInfo method = null;

                foreach (MethodInfo mi in mi_list)
                {
                    if (mi.Name == method_name)
                    {
                        method = mi; break;
                    }
                }
                if (method != null)
                {
                    return method.Invoke(o, param_s);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Клонируем объект
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T CloneObject<T>(T obj)
        {
            T objectt =(T)StaticHelperClass.ExecuteMethodByName(obj, "MemberwiseClone",
                            BindingFlags.Instance | BindingFlags.NonPublic, null);
            return objectt;
        }

        public static object ExecuteMethodByName(object o, string method_name, object[] param_s)
        {
            try
            {
                Type type = o.GetType();
                List<MethodInfo> mi_list = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).ToList();
                MethodInfo method=null;

                foreach (MethodInfo mi in mi_list)
                {
                    if (mi.Name == method_name && mi.GetParameters().Count() == param_s.Count())
                    {
                        method = mi; break;
                    }
                }                
                if (method != null)
                {
                    return method.Invoke(o, param_s);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Выполняет делегат который требует ивоука
        /// </summary>
        /// <typeparam name="T">Класс контрола</typeparam>
        /// <param name="control">Контрол</param>
        /// <param name="act">Действие над ним</param>
        public static void ExecuteInvokeRequiredDelegate_WinForm<T>(T control, Action<T> act) where T : Control
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(delegate
                {
                    act(control);
                    
                }));
            }
            else { act(control); } 
        }


       
    }
    /// <summary>
    /// Энумератор состояний
    /// </summary>
    public enum StateEnum { Error = -3, Created = -2, NotSatrted = -1, Started = 0, Working = 1, RetryAction = 2, ErrorTriesEnded = -4, SuccessfulComplete = 5 };
}

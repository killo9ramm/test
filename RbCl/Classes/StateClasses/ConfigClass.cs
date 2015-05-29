using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using NLog;

using System.IO;
using System.Text.RegularExpressions;
using RBClient.Classes;
using CustomLogger;

namespace Config_classes
{
        public class ConfigClass : LoggerBase,IDisposable
        {
            public static List<ConfigClass> Configs=new List<ConfigClass>();

            private Dictionary<string, object> Properties;
            private string Filename;
            public XDocument Document;
            private string configName;



            //приватный конструктор
            public ConfigClass(string configPath)
            {
                configName = configPath;
                LoadConfigFile();
                Configs.Add(this);
            }

            public void Close()
            {
                Configs.Remove(this);
            }

            public void Dispose()
            {
                Close();
            }

            static ConfigClass()
            {

            }

            //запись свойства
            public bool SetProperty(string propName, string value)
            {
                //заполнить поля
                if (!LoadXml()) { return false; }
                return SetPropertyInXml(propName, value);

                //сделать разбиение по точке
            }
            private bool SetPropertyInXml(string propName, string value)
            {
                try
                {
                    object result = null;
                    //ищем рут
                    XElement root = (from a in Document.Descendants("props")
                                     select a).FirstOrDefault<XElement>(); if (root == null) return false;

                    XElement xx = (from a in Document.Descendants()
                                   where a.Parent == root && a.Attribute("name").Value == propName
                                   select a).FirstOrDefault<XElement>(); if (xx == null) return false;

                    xx.Attribute("value").Value = value;

                    Document.Save(Filename);

                    LoadConfigFile();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public Dictionary<string, object> GetAllProperties()
            {
                if (Properties!=null && Properties.Count>0) return Properties;

                Dictionary<string, object> dict = new Dictionary<string, object>();
                if (Document == null) return null;

                var c = (from k in Document.Descendants("prop")
                         where k.Parent.Name == "props"
                         select k).ToList<XElement>();

                if(null!=c)
                {
                    c.ForEach(a=>dict.Add(a.Attribute("name").Value,a.Attribute("value").Value));
                    return dict;
                }else
                    return null;
            }

            public Dictionary<string, object> GetAllProperties(string startsWith)
            {
                if (Properties != null && Properties.Count > 0) return Properties;

                Dictionary<string, object> dict = new Dictionary<string, object>();
                if (Document == null) return null;

                var c = (from k in Document.Descendants("prop")
                         where k.Parent.Name == "props" && k.Attribute("name").Value.StartsWith(startsWith)
                         select k).ToList<XElement>();

                if (null != c)
                {
                    c.ForEach(a => dict.Add(a.Attribute("name").Value, a.Attribute("value").Value));
                    return dict;
                }
                else
                    return null;
            }

            //загрузка по умолчанию
            public void SaveConfigFile(string text)
            {
                File.WriteAllText(configName,text);
                LoadConfigFile();
            }
            //загрузка по умолчанию
            public bool LoadConfigFile()
            {
                return LoadConfigFile(configName);
            }

            //загрузка файла конфига
            public bool LoadConfigFile(string filename)
            {
                if (!File.Exists(filename))
                {
                    return false;
                }
                Filename = filename;
                return LoadXml();
            }
            //загрузка хмл
            private bool LoadXml()
            {
                try
                {
                    Document = XDocument.Load(Filename);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            //поиск свойства
            public object GetProperty(string propName)
            {
                //заполнить поля
                if (!LoadXml()) { return null; }
                return FindPropertyInXml(propName);

                //сделать разбиение по точке
            }

            public object GetProperty(string propName,object default_value)
            {
                //заполнить поля
                object o = GetProperty(propName);
                if (o != null)
                    return o;
                return default_value;
            }

            public List<object> GetProperties(string propName)
            {
                if (!LoadXml()) { return null; }
                return FindPropertiesInXml(propName);
            }

            public T GetProperty<T>(string propName, T default_value) //where T:struct
            {
                //заполнить поля
                T val = default_value;
                object o = GetProperty(propName);

                if (o != null)
                {
                    try
                    {
                        val = (T)Convert.ChangeType(o, typeof(T));
                        return val;
                    }
                    catch (Exception e)
                    { return default_value; }
                }
                return default_value;
            }

            private object FindPropertyInXml(string propName)
            {
                object result = null;
                //ищем рут
                XElement root = (from a in Document.Descendants("props")
                                 select a).FirstOrDefault<XElement>();if (root == null) return null;


                //разбиваем свойство по точкам
                List<string> props_list=propName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                if (props_list == null || props_list.Count == 0 || props_list.Count == 1)
                {
                    //одноуровневая модель
                    //ищем 1й уровень
                     var b= from a in Document.Descendants()
                                   where a.Parent == root && a.Attribute("name").Value == propName
                                   select a;
                     XElement xx = null;
                     try
                     {
                         xx = b.FirstOrDefault<XElement>();
                     }
                     catch (Exception ex)
                     {
                         Log("Ошибка поиска элемента "+propName+" Error: "+ex.Message);
                     }
                    if (xx == null) return null;

                    return PrepareResult(xx.Attribute("value").Value);
                }
                else
                {
                    //многоуровневая модель
                    XElement xx= (from a in Document.Descendants()
                                   where a.Parent == root && a.Attribute("name").Value == props_list[0]
                                   select a).FirstOrDefault<XElement>(); if (xx == null) return null;
                    throw new Exception();

                }

                //ищем последующие уровни
                return null;
            }

            private List<object> FindPropertiesInXml(string propName)
            {
                object result = null;
                //ищем рут
                XElement root = (from a in Document.Descendants("props")
                                 select a).FirstOrDefault<XElement>(); if (root == null) return null;

                //разбиваем свойство по точкам
                List<string> props_list = propName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                if (props_list == null || props_list.Count == 0 || props_list.Count == 1)
                {
                    //одноуровневая модель
                    //ищем 1й уровень
                    var b = from a in Document.Descendants()
                            where a.Parent == root && a.Attribute("name").Value.IndexOf(propName)==0
                            select a;
                    IEnumerable<XElement> xx = null;
                    try
                    {
                        xx = b;
                    }
                    catch (Exception ex)
                    {
                        Log("Ошибка поиска элементов " + propName + " Error: " + ex.Message);
                    }
                    if (xx == null) return null;

                    return xx.Select(a=>(object)a.Attribute("value").Value).ToList();
                }
                else
                {
                    //многоуровневая модель
                    XElement xx = (from a in Document.Descendants()
                                   where a.Parent == root && a.Attribute("name").Value == props_list[0]
                                   select a).FirstOrDefault<XElement>(); if (xx == null) return null;
                    throw new Exception();

                }

                //ищем последующие уровни
                return null;
            }


            private static object PrepareResult(string result)
            {
                List<string> list = result.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                if (list == null || list.Count == 0 || list.Count == 1) return result;
                else return list;
            }


            public List<Regex> GetRegexList(string param)
            {
                return GetPropertiesList<Regex>(param);
            }

            public List<T> GetPropertiesList<T>(string param)
                where T:class
            {
                try
                {
                    List<T> reg_list = new List<T>();
                    object o = GetProperties(param);

                    if (o != null && o is List<object>)
                    {
                        foreach (var s in (List<object>)o)
                        {
                            reg_list.Add((T)CreateTypeInstance(typeof(T),new object[]{s}));
                        }
                    }
                    if (reg_list.NotNullOrEmpty())
                        return reg_list;
                    else
                        return null;
                }catch(Exception ex)
                {
                    Log(ex, String.Format("GetPropertiesList create error of type {0} from {1}", typeof(T).ToString(), Serializer.JsonSerialize(param)));
                    return null;
                }
            }

            public object CreateTypeInstance(Type T,params object[] pars)
            {
                try
                {
                    var t = Activator.CreateInstance(T, pars);
                    return t;
                }catch(Exception ex)
                {
                    Log(ex, String.Format("CreateTypeInstance cannot create instance of {0} from {1}", T.ToString(), Serializer.JsonSerialize(pars)));
                    return null;
                }
            }

        }

    }


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.IO;


namespace Config_classes
{
        public class ConfigClass
        {
            private static Dictionary<string, object> Properties;
            private static string Filename;
            private static XDocument Document;



            //public static event Debug_classes.DebugPanel.MessageEventHandler LogEvent;
            private static void Log(string message)
            {
                //if (null != LogEvent)
                //{
                //    MessageEventArgs mess = new MessageEventArgs(message);
                //    LogEvent(null, mess);
                //}
            }

            //приватный конструктор
            private ConfigClass()
            {
            }

            static ConfigClass()
            {
                //logger.Debug("Config_Started");
            }

            //загрузка по умолчанию
            public static bool LoadConfigFile()
            {
                System.IO.File.WriteAllText("12112.txt",System.IO.Directory.GetCurrentDirectory());
                return LoadConfigFile("RBUpdaterConfig.xml");
            }
            //загрузка файла конфига
            public static bool LoadConfigFile(string filename)
            {
                if (!File.Exists(filename))
                {
                    return false;
                }
                Filename = filename;
                return LoadXml();
            }
            //загрузка хмл
            private static bool LoadXml()
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
            public static object GetProperty(string propName)
            {
                //заполнить поля
                if (!LoadXml()) { return null; }
                return FindPropertyInXml(propName);

                //сделать разбиение по точке
            }

            public static T GetProperty<T>(string propName, T default_value)
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


            public static string GetProperty(string propName, string default_value)
            {
                //заполнить поля
                string val = default_value;
                object o = GetProperty(propName);

                if (o != null)
                {
                    try
                    {
                        val = (string)Convert.ChangeType(o, typeof(string));
                        return val;
                    }
                    catch (Exception e)
                    { return default_value; }
                }
                return default_value;
            }

            public static object GetProperty(string propName,bool split)
            {
                //заполнить поля
                if (!LoadXml()) { return null; }
                return FindPropertyInXml(propName,split);

                //сделать разбиение по точке
            }

            public static Dictionary<string,object> FindPropertiesInXmlByBegin(string propName, bool split)
            {
                if (!LoadXml()) { return null; }
                XElement root = (from a in Document.Descendants("props")
                                 select a).FirstOrDefault<XElement>(); if (root == null) return null;

                var b = from a in Document.Descendants()
                        where a.Parent == root && a.Attribute("name").Value.IndexOf(propName) == 0
                        select a;
                if (b != null && b.Count() > 0)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (var _n in b)
                    {
                        if(split){
                            dict.Add(_n.Attribute("name").Value, FindPropertyInXml(_n.Attribute("name").Value, split));
                        }else
                        {
                            dict.Add(_n.Attribute("name").Value, FindPropertyInXml(_n.Attribute("name").Value));
                        }
                    }
                    return dict;
                }
                return null;
            }

            private static object FindPropertyInXml(string propName,bool split)
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
                            where a.Parent == root && a.Attribute("name").Value == propName
                            select a;
                    XElement xx = null;
                    try
                    {
                        xx = b.FirstOrDefault<XElement>();
                    }
                    catch (Exception ex)
                    {
                        Log("Ошибка поиска элемента " + propName + " Error: " + ex.Message);
                    }
                    if (xx == null) return null;

                    if (split)
                    {
                        return PrepareResult(xx.Attribute("value").Value);
                    }
                    return xx.Attribute("value").Value;
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

            private static object FindPropertyInXml(string propName)
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

            private static object PrepareResult(string result)
            {
                List<string> list = result.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                if (list == null || list.Count == 0 || list.Count == 1) return result;
                else return list;
            }

            private List<XElement> FindElementUnderRoot(string rootElement,string elementName)
            {

                return null;
            }

            private List<XElement> FindElementUnderRoot(XElement rootElement,string elementName)
            {

                return null;
            }
        }

    }


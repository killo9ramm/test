using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debug_classes;
using System.Xml.Linq;
using NLog;
using RBServer.Debug_classes;
using System.IO;

namespace Config_classes
{
        public class ConfigClass
        {
            private static Dictionary<string, object> Properties;
            private static string Filename="";
            public static XDocument Document;
            private static string configName = "RBUpdaterConfig.xml";

            public static event MessageEventHandler LogEvent;
            private static void Log(string message)
            {
                if (null != LogEvent)
                {
                    MessageEventArgs mess = new MessageEventArgs(message);
                    LogEvent(null, mess);
                }
            }

            //приватный конструктор
            private ConfigClass()
            {

            }

            static ConfigClass()
            {

            }

            public Dictionary<string, object> GetAllProperties()
            {
                if (Properties!=null && Properties.Count>0) return Properties;

                Dictionary<string, string> dict = new Dictionary<string, string>();
                if (Document == null) return null;

                var c = (from k in Document.Descendants("props")
                         select k).ToList<XElement>();
                   

                return null;
            }

            //загрузка по умолчанию
            public static void SaveConfigFile(string text)
            {
                File.WriteAllText(configName,text);
                LoadConfigFile();
            }
            //загрузка по умолчанию
            public static bool LoadConfigFile()
            {
                return LoadConfigFile(configName);
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
                    Document = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,Filename));
                }
                catch(Exception ex)
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
                    throw new CException();

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

        }

    }


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace RBClient
{
    class CConfig
    {
        public string m_app_folder;
        public string m_arm_kassir;
        public string m_FTP_server;
        public string m_FTP_server2;
        public string m_FTP_port;
        public string m_FTP_login;
        public string m_FTP_password;
        //public string m_ver;
        public string m_teremok_id;
        //public string m_teremok_code1C;
        public string m_folder_kkm1_in;
        public string m_folder_kkm1_out;
        public string m_folder_kkm2_in;
        public string m_folder_kkm2_out;
        public string m_folder_kkm3_in;
        public string m_folder_kkm3_out;
        public string m_folder_kkm4_in;
        public string m_folder_kkm4_out;

        public string m_dep;
        public string m_connstring;
        public int m_timer;
        public int m_timer_inbox;
        public int m_timer_report;

        public CConfig()
        {
            XmlReader _xmlreader = null;
            XDocument _doc; 
            string _app_folder = Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "").Replace("\\bin\\Release", "");

            m_arm_kassir = "1"; // по умолчанию первая версия
            m_timer_report = 600000; // по умолчанию
            m_app_folder = _app_folder;

            try
            {
                _xmlreader = XmlReader.Create(_app_folder + "\\RBConfig.xml");
                 _doc = XDocument.Load(_xmlreader);

                foreach (XElement el in _doc.Root.Elements())
                {
                    switch (el.Attribute("name").Value)
                    {
                        case "arm_kassir":
                            m_arm_kassir = el.Value;
                            break;
                        case "ftpserver":
                            m_FTP_server = el.Value;
                            break;
                        case "ftpserver2":
                            m_FTP_server2 = el.Value;
                            break;
                        case "ftpport":
                            m_FTP_port = el.Value;
                            break;
                        case "ftplogin":
                            m_FTP_login = el.Value;
                            break;
                        case "ftppassword":
                            m_FTP_password = el.Value;
                            break;
                        case "teremok_id":
                            m_teremok_id = el.Value;
                            break;                      
                        //case "app_folder":
                        //    m_app_folder = el.Value;
                        //    break;
                        case "folder_kkm1_in":
                            m_folder_kkm1_in = el.Value;
                            break;
                        case "folder_kkm1_out":
                            m_folder_kkm1_out = el.Value;
                            break;
                        case "folder_kkm2_in":
                            m_folder_kkm2_in = el.Value;
                            break;
                        case "folder_kkm2_out":
                            m_folder_kkm2_out = el.Value;
                            break;
                        case "folder_kkm3_in":
                            m_folder_kkm3_in = el.Value;
                            break;
                        case "folder_kkm3_out":
                            m_folder_kkm3_out = el.Value;
                            break;
                        case "folder_kkm4_in":
                            m_folder_kkm4_in = el.Value;
                            break;
                        case "folder_kkm4_out":
                            m_folder_kkm4_out = el.Value;
                            break;
                        case "connstring":
                            m_connstring = el.Value;
                            break;
                        case "dep":
                            m_dep = el.Value;
                            break;
                        case "timer":
                            m_timer = Convert.ToInt32(el.Value.ToString());
                            break;
                        case "timer_inbox":
                            m_timer_inbox = Convert.ToInt32(el.Value.ToString());
                            break;
                        case "timer_report":
                            m_timer_report = Convert.ToInt32(el.Value.ToString());
                            break;
                    }
                }
                m_connstring = m_connstring + m_app_folder + "\\Data\\RBA.accdb";
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_xmlreader != null)
                    _xmlreader.Close();
            }
        }

        public void UpdateParametr(string param, string value)
        {
            
            string _app_folder = Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "").Replace("\\bin\\Release", "");
            XmlWriter writer = null;

            
            XmlTextWriter _xmlwriter = new XmlTextWriter(_app_folder + "\\RBConfig.xml", Encoding.Unicode);

            try            
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                writer = XmlWriter.Create (_app_folder + "\\RBConfig.xml", settings);
                writer.WriteAttributeString(param, value);
                writer.Flush();
                writer.Close();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }
    }
}

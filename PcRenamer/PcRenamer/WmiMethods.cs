using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using CustomLogger;
using RBClient;
using RBClient.Classes;

namespace PcRenamer
{
    public class WmiMethods : LoggerBase
    {
        public class Credentials
        {
            public string UserName;
            public string Password;
        }

        public Credentials CheckConnection(ManagementScope scope,List<Credentials> credList)
        {
            foreach (var a in credList)
            {
                ConnectionOptions connectoptions=CreateOptionsAdm(a.UserName,a.Password);
                if(CheckConnection(connectoptions,scope)) return a;
            }
            return null;
        }

        public bool CheckConnection(ConnectionOptions connOptions, ManagementScope scope)
        {
            try
            {
                scope.Options = connOptions;
                scope.Connect();
                return true;
            }catch(Exception ex)
            {
                Log(ex,"wmi CheckConnection error");
                return false;
            }
        }
        public bool CheckConnection(ManagementScope scope)
        {
            try
            {
                scope.Connect();
                return true;
            }
            catch (Exception ex)
            {
                Log(ex, "wmi CheckConnection error");
                return false;
            }
        }

        public ConnectionOptions CreateOptionsAdm(string username,string password)
        {
            ConnectionOptions connectoptions = new ConnectionOptions();
            connectoptions.Username = username;
            connectoptions.Password = password;
            connectoptions.EnablePrivileges = true;
            connectoptions.Impersonation = System.Management.ImpersonationLevel.Impersonate;
            return connectoptions;
        }

        public ManagementScope CreateScope(string pc_name)
        {
            ManagementScope scope = new ManagementScope(@"\\" + pc_name + @"\root\cimv2");
            return scope;
        }

        public ManagementScope CreateScope()
        {
            ManagementScope scope = new ManagementScope(@"\root\cimv2");
            return scope;
        }

        public ManagementBaseObject RunProcess(ManagementScope scope, string process_path)
        {
            try
            {
                ObjectGetOptions objectGetOptions = new ObjectGetOptions();
                ManagementPath managementPath = new ManagementPath("Win32_Process");
                ManagementClass processClass = new ManagementClass
                    (scope, managementPath, objectGetOptions);
                ManagementBaseObject inParams = processClass.GetMethodParameters("Create");
                inParams["CommandLine"] = process_path;
                ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, null);
                return outParams;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<string> Get_Installed_Process_List(ManagementScope scope)
        {
            List<string> prc_list = new List<string>();
            SelectQuery query = new SelectQuery("SELECT Name FROM Win32_Product");
            ManagementObjectSearcher mos = new ManagementObjectSearcher(scope, query);// where Name="+process_name);

            foreach (ManagementObject mo in mos.Get())
            {
                try
                {
                    prc_list.Add((string)mo.GetPropertyValue("Name"));
                }catch(Exception ex)
                {
                    Log(ex,"cannot add process "+mo.GetText(TextFormat.Mof));
                }
            }
            if(prc_list.NotNullOrEmpty()){
                return prc_list;
            }
            return null;
        }

        public ManagementObject Get_Installed_Process(ManagementScope scope, string process_name)
        {
            SelectQuery query = new SelectQuery("SELECT * FROM Win32_Product");
            ManagementObjectSearcher mos = new ManagementObjectSearcher(scope, query);// where Name="+process_name);
            
            foreach (ManagementObject mo in mos.Get())
            {
                if (mo["process_name"] != null)
                {
                    return mo;
                }
            }
            return null;
        }

        public string GetMachineName(ManagementScope scope)
        {
            List<string> m_names=GetMachinename(scope);
            string m_suffix = Program.GetDnsSuffix();

            string m_name = "";

            if (m_names.NotNullOrEmpty())
            {
                m_name += m_names[0];
            }
            if (!String.IsNullOrEmpty(m_suffix))
            {
                m_name += "." + m_suffix;
            }
            return m_name;
        }

        public List<string> GetMachinename(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery(
                "SELECT * FROM Win32_ComputerSystem");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            ManagementObjectCollection theCollection = searcher.Get();

            var ls = new List<string>();

            foreach (ManagementObject queryObj in theCollection)
            {
                if (queryObj["Name"] != null)
                {
                    string st = queryObj["Name"].ToString();
                    if(st!="")
                        ls.Add(st);
                }
            }
            return ls;
        }

        public List<string> GetDnsSuffixSearchOrder(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery(
                "SELECT * FROM Win32_NetworkAdapterConfiguration");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            ManagementObjectCollection theCollection = searcher.Get();

            var ls = new List<string>();

            foreach (ManagementObject queryObj in theCollection)
            {
                object o = queryObj["DNSDomainSuffixSearchOrder"];
                Log("dns suff");
                if (o != null && o is string[])
                {
                    string[] s = (string[])o;
                    Log("dns suff is " + Serializer.JsonSerialize(s));
                    if (s.NotNullOrEmpty())
                    {
                        foreach (var oo in s)
                        {
                            if(oo!="")
                                ls.Add(oo);
                        }
                        
                    }
                }
            }
            return ls;
        }

        void _1()
        {
            ManagementScope scope = CreateScope("asdasd");
            List<Credentials> clist = null;

            if (CheckConnection(scope, clist) != null)
            {
                //execute app 
                //start pr
            }
            else
            {
                //error
            }

            // execute scope
        }

        static void main()
        {
            try
            {
                WmiMethods wmi = new WmiMethods(); wmi.LogEvent = MDIParentMain.Log;
                ManagementScope sc=wmi.CreateScope("10.15.127.3");
                ConnectionOptions co=wmi.CreateOptionsAdm("Администратор", " ");
                
                wmi.CheckConnection(co, sc);

                



                ConnectionOptions connectoptions = new ConnectionOptions();
                //  connectoptions.Username = string.Format(@"{0}", @"msk\krigel.s");
                connectoptions.Username = "Администратор";//string.Format(@"{0}", @"Admin");
                connectoptions.Password = " ";

                //?connectoptions.Impersonation = ImpersonationLevel.Impersonate;
                

                ManagementScope scope = new ManagementScope(@"\\" + "10.15.127.3" + @"\root\cimv2");
                //ManagementScope scope = new ManagementScope(@"\\" + "10.11.0.177" + @"\root\cimv2");
                scope.Options = connectoptions;
                scope.Options.EnablePrivileges = true;
                scope.Options.Impersonation = System.Management.ImpersonationLevel.Impersonate;

                scope.Connect();

                SelectQuery query = new SelectQuery("select * from Win32_Process WHERE Name = 'ProcessKillerLauncher.exe'");

                ManagementObjectSearcher theSearcher = new ManagementObjectSearcher(scope, query);

                ManagementObjectCollection theCollection = theSearcher.Get();

                foreach (ManagementObject theCurObject in theCollection)
                {

                    theCurObject.InvokeMethod("Terminate", null);

                }

            }
            catch (Exception ex)
            {
            }
        }
    }
}

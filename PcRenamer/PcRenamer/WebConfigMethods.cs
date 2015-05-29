using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using RBClient.Classes.CustomClasses;
using RBClient.Classes;

namespace PcRenamer
{
    class WebConfigMethods
    {
        /// <summary>
        /// Set's a new Gateway address of the local machine
        /// </summary>
        /// <param name="gateway">The Gateway IP Address</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public static void setGateway(string[] gateways)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    try
                    {
                        ManagementBaseObject setGateway;
                        ManagementBaseObject newGateway =
                            objMO.GetMethodParameters("SetGateways");

                        newGateway["DefaultIPGateway"] = gateways;
                        newGateway["GatewayCostMetric"] = new int[] { 1 };

                        setGateway = objMO.InvokeMethod("SetGateways", newGateway, null);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        /// <summary>
        /// Set's the DNS Server of the local machine
        /// </summary>
        /// <param name="NIC">NIC address</param>
        /// <param name="DNS">DNS server address</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public static void setDNS(string[] DNS)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    // if you are using the System.Net.NetworkInformation.NetworkInterface you'll need to change this line to if (objMO["Caption"].ToString().Contains(NIC)) and pass in the Description property instead of the name 
                    //if (objMO["Caption"].Equals(NIC))
                    //{
                        try
                        {
                            ManagementBaseObject newDNS =
                                objMO.GetMethodParameters("SetDNSServerSearchOrder");
                            newDNS["DNSServerSearchOrder"] = DNS;
                            ManagementBaseObject setDNS =
                                objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    //}
                }
            }
        }


        public static List<string> GetArrParamFromNetwAd(ManagementObjectCollection objMOC, string p)
        {
            List<string> _list = new List<string>();
            foreach (ManagementObject objMO in objMOC)
            {
                if (!(bool)objMO["IPEnabled"])
                    continue;
                string[] _s = (string[])objMO[p];

                if (_s != null)
                {
                    _list.AddRange(_s);
                }
            }
            if (_list.NotNullOrEmpty())
            {
                return _list;
            }
            else
            {
                return null;
            }
        }

    }
}

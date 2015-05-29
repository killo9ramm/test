using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config_classes;
using PcRenamer.ArmService;
using System.Net;
using RBClient.Classes.CustomClasses;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace PcRenamer
{
    public static partial class Program
    {

            public static bool CustomCertificateValidatior(object sender,
        X509Certificate certificate, X509Chain chain,
        SslPolicyErrors policyErrors)
        {
            // anything goes!
            return true;

            // PS: you could put your own validation logic here, 
            // through accessing the certificate properties:
            // var publicKey = certificate.GetPublicKey();

        }

        private static Service1 _WebService = null;
        public static Service1 WebService
        {
            get
            {
                if (_WebService == null)
                {

                    string web_service_url =
#if(DEBUG)
 "https://dev01/armservice/Service1.asmx";
#else
                        CNFG.GetProperty<string>("web_service_url","");
#endif



 _WebService = new Service1();

                    if (web_service_url != "")
                    {
                        _WebService.Url = web_service_url;
                    }

                    string user_name=CNFG.GetProperty<string>("user_name","");
                    string password=CNFG.GetProperty<string>("password","");
                    string domain=CNFG.GetProperty<string>("user_domain","msk");

                    if(user_name!="")
                    {
                        _WebService.Credentials = new NetworkCredential(user_name, PasswordDecoder.decode_string(password), domain);
                    //    _WebService.Credentials = new NetworkCredential(user_name, PasswordDecoder.decode_string(password));
                    }

                    string val_by_cert=CNFG.GetProperty<string>("validation_by_cert","0");

                    if (val_by_cert == "0")
                    {
                        ServicePointManager.ServerCertificateValidationCallback = null;
                        ServicePointManager.ServerCertificateValidationCallback +=
                        new RemoteCertificateValidationCallback(CustomCertificateValidatior);
                    }

#if(DEBUG)

                    _WebService.Proxy = new WebProxy("mskisa01.msk.teremok.biz", 8080);
                    var cr = new NetworkCredential("arm_services", "mg5Qukf7DG8RrvwZ", "msk");
                    _WebService.Proxy.Credentials = cr;

#endif
                }
                return _WebService;
            }
            set
            {
                _WebService = value;
            }
        }
        
    }
}

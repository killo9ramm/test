using RBClient.Classes.DocumentClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient.Classes.OrderClasses
{
    public class OrderStateClass
    {
        public OrderClass order { get; set; }
        public DateTime CreationDate{ get; private set; }

        public OrderStateClass()
        {
            CreationDate = DateTime.Now;
        }
    }

    public class WebServiceStateClass : OrderStateClass
    {
        public string FunctionName { get; private set; }
        public object[] Params { get; private set; }
        public string ServiceType { get; private set; }

        protected WebServiceStateClass()
        {
        }

        public WebServiceStateClass(string _FunctionName, object[] _Params) : base()
        {
            FunctionName = _FunctionName;
            Params = _Params;
        }
    }
    public class WebService1CStateClass : WebServiceStateClass
    {
        public const string SERVICETYPE="Web Service 1c function";

        public string ServiceType { get; private set; }

        protected WebService1CStateClass()
        {
        }

        public WebService1CStateClass(string _FunctionName, object[] _Params) : base(_FunctionName,_Params)
        {
            ServiceType = SERVICETYPE;
        }
    }
}

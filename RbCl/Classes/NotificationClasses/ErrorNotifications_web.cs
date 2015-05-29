using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient.Classes
{
    partial class ErrorNotifications
    {
        public enum WebInfoMessageState {Success=0,Error=99};

        public static string Web_notificate_success(string _header,string _message)
        {
            StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "SendNotification",
                                new object[] { _header, _message, (int)WebInfoMessageState.Success},
                                55);
            return "1";             
        }

        public static string Web_notificate_success(string _header, string _message,int priority)
        {
            StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "SendNotification",
                                new object[] { _header, _message, (int)WebInfoMessageState.Success },
                                priority);
            return "1";
        }

        public static string Web_notificate_error(string _header, string _message)
        {
            StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "SendNotification",
                                new object[] { _header, _message, (int)WebInfoMessageState.Error },
                                55);

            return "1";
        }
    }
}

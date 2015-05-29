using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBServer.Debug_classes
{
    class CException : System.Exception
    {
        public string Message { get; set; }
        public CException() : base() 
        {
            Message =base.Message+ "\r\n" + DebugPanel.InMethod(0, 10);
            DebugPanel.OnErrorOccured(Message);
        }

        public CException(string message) : base(message) 
        {
            Message = base.Message + "\r\n" + DebugPanel.InMethod(0, 10);
            DebugPanel.OnErrorOccured(Message);
        }

        public CException(string message, Exception exception)
            : base(message, exception)
        {
            Message = base.Message + "\r\n" + DebugPanel.InMethod(0, 10);
            DebugPanel.OnErrorOccured(Message);
        }
    }
}

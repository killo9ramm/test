using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes;

namespace Global.Custom.Actions
{
    public class SafeAction : BaseAction
    {
        Action<object> _act;
        object _o;
        

        public SafeAction(Action<object> act,object o)
        {
            _act = act;
        }

        public bool Start()
        {
            try
            {
                _act(_o);
                return true;
            }catch(Exception ex)
            {
                Log(ex);
                return false;
            }
        }
    }

    public abstract class BaseAction
    {
        public NLogDelegate LogEvent;
        public void Log(string message)
        {
            if (LogEvent != null)
                LogEvent(message);
        }
        public void Log(string message, Exception exp)
        {
            if (LogEvent != null)
                LogEvent(message + " exception: " + exp.Message);
        }
        public void Log(Exception exp)
        {
            if (LogEvent != null)
                LogEvent("Exception: " + exp.Message);
        }
    }
}

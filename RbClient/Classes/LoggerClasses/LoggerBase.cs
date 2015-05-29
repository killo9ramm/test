using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomLogger
{
    public class LoggerBase
    {
        public Action<string> LogEvent;

        public virtual void Log(string message)
        {
            if (null != LogEvent)
            {
                LogEvent(message);
            }
        }

        public virtual void Log(Exception ex)
        {
            if (null != LogEvent)
            {
                LogEvent(ex.Message);
            }
        }

        public virtual void Log(Exception ex,string message)
        {
            if (null != LogEvent)
            {
                LogEvent("Error "+ex.Message+" Message:"+message);
            }
        }
    }
}

using System;

namespace CustomLogger
{
	public class LoggerBase
	{
		public Action<string> LogEvent;

		public LoggerBase()
		{
		}

		public virtual void Log(string message)
		{
			if (this.LogEvent != null)
			{
				this.LogEvent(message);
			}
		}

		public virtual void Log(Exception ex, string message)
		{
			if (this.LogEvent != null)
			{
				this.LogEvent(string.Concat("Error ", ex.Message, " Message:", message));
			}
		}
	}
}
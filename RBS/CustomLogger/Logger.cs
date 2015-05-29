using System;
using System.IO;

namespace CustomLogger
{
	internal class Logger
	{
		public string LoggerName = "Trace";

		public string LogFileName = "Logger.txt";

		private int counter;

		private FileInfo LogFile;

		public Logger()
		{
		}

		public void Log(string message)
		{
			string logFileName = this.LogFileName;
			string[] loggerName = new string[] { this.LoggerName, "----", null, null, null, null };
			loggerName[2] = DateTime.Now.ToString();
			loggerName[3] = "-----";
			loggerName[4] = message;
			loggerName[5] = "\r\n";
			File.AppendAllText(logFileName, string.Concat(loggerName));
		}

		public void Log(Exception ex, string message)
		{
			if (ex != null)
			{
				this.Log(string.Concat("error: ", ex.Message, " message: ", message));
			}
		}
	}
}
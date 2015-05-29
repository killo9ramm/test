using System;
using System.Runtime.CompilerServices;

namespace RBServer.Debug_classes
{
	internal class CException : Exception
	{
		public new string Message
		{
			get;
			set;
		}

		public CException()
		{
			this.Message = string.Concat(base.Message, "\r\n", DebugPanel.InMethod(0, 10));
			DebugPanel.OnErrorOccured(this.Message);
		}

		public CException(string message) : base(message)
		{
			this.Message = string.Concat(base.Message, "\r\n", DebugPanel.InMethod(0, 10));
			DebugPanel.OnErrorOccured(this.Message);
		}

		public CException(string message, Exception exception) : base(message, exception)
		{
			this.Message = string.Concat(base.Message, "\r\n", DebugPanel.InMethod(0, 10));
			DebugPanel.OnErrorOccured(this.Message);
		}
	}
}
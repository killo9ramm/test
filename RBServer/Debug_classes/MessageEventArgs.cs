using System;

namespace RBServer.Debug_classes
{
	public class MessageEventArgs : EventArgs
	{
		public string Message = "";

		public object Object;

		public MessageEventArgs(string message)
		{
			this.Message = message;
		}

		public MessageEventArgs()
		{
		}

		public override string ToString()
		{
			string str = "";
			if (this.Message != "")
			{
				str = string.Concat(str, " Message:", this.Message, "\r\n");
			}
			if (this.Object != null)
			{
				str = string.Concat(str, " Object:", this.Object.ToString(), "\r\n");
			}
			return str;
		}
	}
}
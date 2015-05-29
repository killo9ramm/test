using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace RBServer.Debug_classes
{
	internal class FileInfo
	{
		public System.IO.FileInfo file;

		internal DateTime CreationTime
		{
			get
			{
				return this.file.CreationTime;
			}
		}

		internal bool Exists
		{
			get
			{
				return this.file.Exists;
			}
		}

		internal string Extension
		{
			get
			{
				if (this.file == null)
				{
					return "";
				}
				return this.file.Extension;
			}
		}

		internal string FullName
		{
			get
			{
				return this.file.FullName;
			}
		}

		internal long Length
		{
			get
			{
				return this.file.Length;
			}
		}

		internal string Name
		{
			get
			{
				return this.file.Name;
			}
		}

		private FileInfo()
		{
		}

		internal FileInfo(System.IO.FileInfo fi)
		{
			this.file = fi;
		}

		internal FileInfo(string path)
		{
			this.file = new System.IO.FileInfo(path);
		}

		internal void Delete()
		{
			try
			{
				if (RBServer.Debug_classes.FileInfo.DeleteEvent != null)
				{
					DebugPanel.MessageEventHandler messageEventHandler = RBServer.Debug_classes.FileInfo.DeleteEvent;
					ArrayList arrayLists = new ArrayList();
					arrayLists.Add(this.file);
					messageEventHandler(arrayLists, null);
				}
				this.file.Delete();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = string.Concat("Не удалось удалить файл ", this.file.FullName, " ошибка ", exception.Message);
				DebugPanel.OnFatalErrorOccured(exception, str);
			}
		}

		internal void MoveTo(string destination)
		{
			try
			{
				if (RBServer.Debug_classes.FileInfo.MoveEvent != null)
				{
					DebugPanel.MessageEventHandler messageEventHandler = RBServer.Debug_classes.FileInfo.MoveEvent;
					ArrayList arrayLists = new ArrayList();
					arrayLists.Add(this.file);
					arrayLists.Add(destination);
					messageEventHandler(arrayLists, null);
				}
				this.file.MoveTo(destination);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = DebugPanel.InMethod(4, 7);
				string[] fullName = new string[] { "Не удалось переместить файл ", this.file.FullName, " в ", destination, " ошибка ", exception.Message, " inmeth:", str };
				DebugPanel.OnFatalErrorOccured(exception, string.Concat(fullName));
			}
		}

		internal static event DebugPanel.MessageEventHandler DeleteEvent;

		internal static event DebugPanel.MessageEventHandler MoveEvent;
	}
}
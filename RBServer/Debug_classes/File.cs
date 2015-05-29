using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;

namespace RBServer.Debug_classes
{
	internal class File
	{
		public File()
		{
		}

		internal static void Copy(string source, string destination, bool overwrite)
		{
			try
			{
				System.IO.File.Copy(source, destination, overwrite);
				if (RBServer.Debug_classes.File.CopyEvent != null)
				{
					DebugPanel.MessageEventHandler messageEventHandler = RBServer.Debug_classes.File.CopyEvent;
					ArrayList arrayLists = new ArrayList();
					arrayLists.Add(source);
					arrayLists.Add(destination);
					arrayLists.Add(overwrite);
					messageEventHandler(arrayLists, null);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string[] strArrays = new string[] { "Не удалось копировать файл ", source, " в ", destination, " ошибка ", exception.Message };
				DebugPanel.OnFatalErrorOccured(exception, string.Concat(strArrays));
			}
		}

		internal static void Copy(string source, string destination)
		{
			try
			{
				System.IO.File.Copy(source, destination);
				if (RBServer.Debug_classes.File.CopyEvent != null)
				{
					DebugPanel.MessageEventHandler messageEventHandler = RBServer.Debug_classes.File.CopyEvent;
					ArrayList arrayLists = new ArrayList();
					arrayLists.Add(source);
					messageEventHandler(arrayLists, null);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string[] strArrays = new string[] { "Не удалось копировать файл ", source, " в ", destination, " ошибка ", exception.Message };
				DebugPanel.OnFatalErrorOccured(exception, string.Concat(strArrays));
			}
		}

		internal static void Delete(string source)
		{
			try
			{
				if (RBServer.Debug_classes.File.DeleteEvent != null)
				{
					DebugPanel.MessageEventHandler messageEventHandler = RBServer.Debug_classes.File.DeleteEvent;
					ArrayList arrayLists = new ArrayList();
					arrayLists.Add(source);
					messageEventHandler(arrayLists, null);
				}
				System.IO.File.Delete(source);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = string.Concat("Не удалось удалить файл ", source, " ошибка ", exception.Message);
				DebugPanel.OnFatalErrorOccured(exception, str);
			}
		}

		internal static bool Exists(string source)
		{
			return System.IO.File.Exists(source);
		}

		internal static bool Move(string source, string destination)
		{
			bool flag;
			try
			{
				if (RBServer.Debug_classes.File.MoveEvent != null)
				{
					DebugPanel.MessageEventHandler messageEventHandler = RBServer.Debug_classes.File.MoveEvent;
					ArrayList arrayLists = new ArrayList();
					arrayLists.Add(source);
					arrayLists.Add(destination);
					messageEventHandler(arrayLists, null);
				}
				if (!System.IO.File.Exists(destination))
				{
					System.IO.File.Move(source, destination);
				}
				else
				{
					System.IO.File.Delete(destination);
					System.IO.File.Copy(source, destination);
				}
				flag = true;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = DebugPanel.InMethod(4, 7);
				string[] strArrays = new string[] { "Не удалось переместить файл ", source, " в ", destination, " ошибка ", exception.Message, " inmeth:", str };
				DebugPanel.OnFatalErrorOccured(exception, string.Concat(strArrays));
				flag = false;
			}
			return flag;
		}

		internal static string ReadAllText(string source, Encoding enc)
		{
			return System.IO.File.ReadAllText(source, enc);
		}

		internal static event DebugPanel.MessageEventHandler CopyEvent;

		internal static event DebugPanel.MessageEventHandler DeleteEvent;

		internal static event DebugPanel.MessageEventHandler MoveEvent;
	}
}
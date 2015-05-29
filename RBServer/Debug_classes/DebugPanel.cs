using RBClient.Classes;
using RBServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace RBServer.Debug_classes
{
	internal class DebugPanel
	{
		private static string _back_folder_path;

		public static string admin_email;

		private static string log_path;

		private static bool _on;

		private static bool _smon;

		private static RBServer.Debug_classes.FileInfo logfile;

		private static Regex method_Name;

		private static Regex fname_regx;

		private static List<string> Remove_symb;

		private static List<string> Replace_symb;

		private static string Replace_value;

		public static string back_folder_path
		{
			get
			{
				return DebugPanel._back_folder_path;
			}
			set
			{
				DebugPanel._back_folder_path = value;
			}
		}

		internal static bool IsON
		{
			get
			{
				return DebugPanel._on;
			}
			set
			{
				if (DebugPanel._on != value)
				{
					if (!value)
					{
						RBServer.Debug_classes.File.MoveEvent -= new DebugPanel.MessageEventHandler(DebugPanel.CreateDelMoveFolderTree);
						RBServer.Debug_classes.FileInfo.MoveEvent -= new DebugPanel.MessageEventHandler(DebugPanel.CreateDelMoveFolderTree);
						RBServer.Debug_classes.File.DeleteEvent -= new DebugPanel.MessageEventHandler(DebugPanel.CreateDelMoveFolderTree);
						RBServer.Debug_classes.FileInfo.DeleteEvent -= new DebugPanel.MessageEventHandler(DebugPanel.CreateDelMoveFolderTree);
						DebugPanel.Log("Дебаг выключен!");
					}
					else
					{
						DebugPanel.back_folder_path = DebugPanel.checkFolderExist(DebugPanel.back_folder_path).FullName;
						RBServer.Debug_classes.File.MoveEvent += new DebugPanel.MessageEventHandler(DebugPanel.CreateDelMoveFolderTree);
						RBServer.Debug_classes.FileInfo.MoveEvent += new DebugPanel.MessageEventHandler(DebugPanel.CreateDelMoveFolderTree);
						RBServer.Debug_classes.File.DeleteEvent += new DebugPanel.MessageEventHandler(DebugPanel.CreateDelMoveFolderTree);
						RBServer.Debug_classes.FileInfo.DeleteEvent += new DebugPanel.MessageEventHandler(DebugPanel.CreateDelMoveFolderTree);
						DebugPanel.Log("Дебаг включен!");
					}
					DebugPanel._on = value;
				}
			}
		}

		internal static bool SendAdminMessageIsON
		{
			set
			{
				if (DebugPanel._smon != value)
				{
					if (!value)
					{
						DebugPanel.ErrorOccured -= new DebugPanel.MessageEventHandler(DebugPanel.SendErrorMessageToAdmin);
						DebugPanel.FatalErrorOccured -= new DebugPanel.MessageEventHandler(DebugPanel.SendErrorMessageToAdmin);
						DebugPanel.Log("Отсылка сервисных сообщений выключена!");
					}
					else
					{
						DebugPanel.ErrorOccured += new DebugPanel.MessageEventHandler(DebugPanel.SendErrorMessageToAdmin);
						DebugPanel.FatalErrorOccured += new DebugPanel.MessageEventHandler(DebugPanel.SendErrorMessageToAdmin);
						DebugPanel.Log("Отсылка сервисных сообщений включена!");
					}
					DebugPanel._smon = value;
				}
			}
		}

		static DebugPanel()
		{
			DebugPanel._back_folder_path = "C:\\RBSERV_PRODUCTION";
			DebugPanel.admin_email = "krigel.s@teremok.ru";
			DebugPanel._on = false;
			DebugPanel._smon = false;
			DebugPanel.method_Name = new Regex("^[a-z_A-Z0-9]+");
			DebugPanel.fname_regx = new Regex("[a-z_A-Z10-9а-яА-я.-]*[.][a-zA-Z0-9]+");
			DebugPanel.Remove_symb = new List<string>()
			{
				":"
			};
			DebugPanel.Replace_symb = new List<string>()
			{
				"\\"
			};
			DebugPanel.Replace_value = "\\";
			DebugPanel.log_path = "rbs_log_temp.txt";
		}

		public DebugPanel()
		{
		}

		private static System.IO.DirectoryInfo checkFolderExist(string ch_path)
		{
			System.IO.DirectoryInfo directoryInfo = null;
			try
			{
				directoryInfo = (!System.IO.Directory.Exists(ch_path) ? System.IO.Directory.CreateDirectory(ch_path) : new System.IO.DirectoryInfo(ch_path));
			}
			catch (Exception exception)
			{
				Console.WriteLine(string.Concat("Не могу получить доступ к директории ", ch_path));
				throw new Exception();
			}
			return directoryInfo;
		}

		public static SmtpClient ConnectToMailClient(string smtpServer, string smtpLogin, string smtpPassword, string domain)
		{
			SmtpClient smtpClient = new SmtpClient(smtpServer)
			{
				Credentials = new NetworkCredential(smtpLogin, smtpPassword, domain)
			};
			return smtpClient;
		}

		internal static void CreateDelMoveFolderTree(object sender, MessageEventArgs e)
		{
			ArrayList arrayLists = sender as ArrayList;
			if (sender == null)
			{
				DebugPanel.Log(string.Concat("не удалось определить тип sender ", sender.GetType().Name));
				throw new Exception(string.Concat("не удалось определить тип sender ", sender.GetType().Name));
			}
			DebugPanel.CreateFileCopy(arrayLists[0]);
		}

		private static void CreateFileCopy(object obj)
		{
			if (obj is string)
			{
				DebugPanel.CreateFileCopy(obj as string);
			}
			if (obj is System.IO.FileInfo)
			{
				DebugPanel.CreateFileCopy(obj as System.IO.FileInfo);
			}
		}

		private static void CreateFileCopy(System.IO.FileInfo path)
		{
			DebugPanel.CreateFileCopy(path.FullName);
		}

		private static void CreateFileCopy(string path)
		{
			string str = "";
			string fileName = DebugPanel.getFileName(path, ref str);
			System.IO.File.Copy(path, Path.Combine(str, fileName), true);
			DebugPanel.Log(string.Concat("Сделана копия ", fileName, " путь ", str));
		}

		private static string CreateFolderStructure(List<string> folder_list, System.IO.DirectoryInfo parent_dir)
		{
			if (folder_list.Count == 0)
			{
				return parent_dir.FullName;
			}
			string str = folder_list.First<string>();
			string str1 = Path.Combine(parent_dir.FullName, str);
			System.IO.DirectoryInfo directoryInfo = DebugPanel.checkFolderExist(str1);
			folder_list.Remove(str);
			return DebugPanel.CreateFolderStructure(folder_list, directoryInfo);
		}

		public static string getFileName(string fullpath, ref string folders)
		{
			DebugPanel.Remove_symb.ForEach((string s) => fullpath = fullpath.Replace(s, ""));
			DebugPanel.Replace_symb.ForEach((string s) => fullpath = fullpath.Replace(s, DebugPanel.Replace_value));
			string str = fullpath;
			string[] replaceValue = new string[] { DebugPanel.Replace_value };
			List<string> list = str.Split(replaceValue, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
			string value = DebugPanel.fname_regx.Match(fullpath).Value;
			list.Remove(value);
			System.IO.DirectoryInfo directoryInfo = DebugPanel.checkFolderExist(DebugPanel.back_folder_path);
			folders = DebugPanel.CreateFolderStructure(list, directoryInfo);
			return value;
		}

		internal static void GetLocalVars(StackFrame stf)
		{
			string name = stf.GetMethod().Name;
		}

		internal static string InMethod(int from_frame, int length)
		{
			string str = "---------------(техническая информация)\r\n";
			List<StackFrame> list = (new StackTrace()).GetFrames().ToList<StackFrame>();
			int count = list.Count;
			if (from_frame + length < list.Count)
			{
				count = from_frame + length;
			}
			for (int i = from_frame; i < count; i++)
			{
				str = string.Concat(str, DebugPanel.method_Name.Match(list[i].ToString()), "  ");
			}
			return str;
		}

		internal static void Log(string message)
		{
			try
			{
				DebugPanel.ReleaseFileFromGrow(DebugPanel.log_path, 30000000);
				DateTime now = DateTime.Now;
				string str = "";
				string[] strArrays = new string[] { now.ToString(), " --- ", message, " ---  in ", str, "\r\n" };
				string str1 = string.Concat(strArrays);
				System.IO.File.AppendAllText(DebugPanel.log_path, str1);
			}
			catch (Exception exception)
			{
				Console.WriteLine(string.Concat("Debug_Panel.log error ", exception.Message));
			}
		}

		internal static void Log(Exception ex, string message)
		{
			try
			{
				DebugPanel.ReleaseFileFromGrow(DebugPanel.log_path, 30000000);
				DateTime now = DateTime.Now;
				string str = DebugPanel.InMethod(4, 7);
				string[] strArrays = new string[] { now.ToString(), " error: ", ex.Message, " --- ", message, " ---  in ", str, "\r\n" };
				string str1 = string.Concat(strArrays);
				System.IO.File.AppendAllText(DebugPanel.log_path, str1);
			}
			catch (Exception exception)
			{
				Console.WriteLine(string.Concat("Debug_Panel.log error ", exception.Message));
			}
		}

		internal static void OnErrorOccured(object obj, string message)
		{
			DebugPanel.Log(string.Concat("Error: ", message, "object ", obj.ToString()));
			MessageEventArgs messageEventArg = new MessageEventArgs(string.Concat(message, " inmethod: ", DebugPanel.InMethod(4, 7)));
			if (DebugPanel.ErrorOccured != null)
			{
				DebugPanel.ErrorOccured(obj, messageEventArg);
			}
		}

		internal static void OnErrorOccured(string message)
		{
			DebugPanel.Log(string.Concat("Error: ", message));
			MessageEventArgs messageEventArg = new MessageEventArgs(string.Concat(message, " inmethod: ", DebugPanel.InMethod(4, 7)));
			if (DebugPanel.ErrorOccured != null)
			{
				DebugPanel.ErrorOccured(null, messageEventArg);
			}
		}

		internal static void OnErrorOccured(object obj)
		{
			DebugPanel.Log(string.Concat("Error: object ", obj.ToString()));
			string str = string.Concat("Error: object ", obj.ToString(), " inmethod: ", DebugPanel.InMethod(4, 7));
			MessageEventArgs messageEventArg = new MessageEventArgs(str);
			if (DebugPanel.ErrorOccured != null)
			{
				DebugPanel.ErrorOccured(obj, messageEventArg);
			}
		}

		internal static void OnFatalErrorOccured(object obj, string message)
		{
			DebugPanel.Log(string.Concat("FatalError: ", message, "object ", obj.ToString()));
			MessageEventArgs messageEventArg = new MessageEventArgs(string.Concat(message, " inmethod: ", DebugPanel.InMethod(4, 7)));
			if (DebugPanel.FatalErrorOccured != null)
			{
				DebugPanel.FatalErrorOccured(obj, messageEventArg);
			}
		}

		internal static void OnFatalErrorOccured(string message)
		{
			DebugPanel.Log(string.Concat("FatalError: ", message));
			MessageEventArgs messageEventArg = new MessageEventArgs(string.Concat(message, " inmethod: ", DebugPanel.InMethod(4, 7)));
			if (DebugPanel.FatalErrorOccured != null)
			{
				DebugPanel.FatalErrorOccured(null, messageEventArg);
			}
		}

		internal static void OnFatalErrorOccured(object obj)
		{
			DebugPanel.Log(string.Concat("FatalError: object ", obj.ToString()));
			string str = string.Concat("Error: object ", obj.ToString(), " inmethod: ", DebugPanel.InMethod(4, 7));
			MessageEventArgs messageEventArg = new MessageEventArgs(str);
			if (DebugPanel.FatalErrorOccured != null)
			{
				DebugPanel.FatalErrorOccured(obj, messageEventArg);
			}
		}

		public static MailMessage PrepareMessageToSend(string From, string To, string Subject, string Message, bool isHTML)
		{
			MailMessage mailMessage = new MailMessage(From, To, Subject, Message)
			{
				IsBodyHtml = isHTML
			};
			return mailMessage;
		}

		internal static void ReleaseFileFromGrow(string filename, double size)
		{
			System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);
			if ((double)fileInfo.Length > size)
			{
				DateTime date = DateTime.Now.Date;
				string str = string.Concat("log\\", date.ToShortDateString(), fileInfo.Name);
				if (!RBServer.Debug_classes.File.Exists(str))
				{
					System.IO.File.Copy(filename, str, true);
					System.IO.File.Delete(filename);
				}
			}
		}

		public static void SaveEmailToDisk(MailMessage mm)
		{
			try
			{
				System.IO.DirectoryInfo directoryInfo = DebugPanel.checkFolderExist(Path.Combine(DebugPanel.back_folder_path, "SendedMails"));
				SmtpClient smtpClient = new SmtpClient("mysmtphost")
				{
					DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
					PickupDirectoryLocation = directoryInfo.FullName
				};
				smtpClient.Send(mm);
			}
			catch (Exception exception)
			{
				DebugPanel.Log(string.Concat("Не получилось сохранить сообщение на диск ", exception.Message));
			}
		}

		private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
		{
			string userState = (string)e.UserState;
			if (e.Cancelled)
			{
				DebugPanel.Log(string.Concat("[{0}] Send canceled.", userState));
			}
			if (e.Error == null)
			{
				DebugPanel.Log("Message sent.");
				return;
			}
			DebugPanel.Log(string.Concat(userState, " ", e.Error.ToString()));
		}

		internal static void SendErrorMessageToAdmin(object sender, MessageEventArgs ex)
		{
			DebugPanel.Log(string.Concat("SendErrorMessageToAdmin:", ex.Message));
			DebugPanel.SendErrorMessageToAdmin(ex);
		}

		internal static void SendErrorMessageToAdmin(MessageEventArgs ex)
		{
			CConfig cConfig = new CConfig();
			string str = DebugPanel.InMethod(4, 7);
			string[] message = new string[] { "message: ", ex.Message, " object ", ex.ToString(), " inmeth:", str };
			string str1 = string.Concat(message);
			string str2 = str1.Substring(0, str1.Trim().IndexOf(' ') + 1);
			DebugPanel.SendMail(cConfig, DebugPanel.admin_email, str2, str1, false, "msk");
		}

		public static void SendMail(string smtp_server, string smtp_Login, string smtp_pass, string smtp_domain, string From, string To, string Subject, string Message, bool isHTML)
		{
			try
			{
				SmtpClient mailClient = DebugPanel.ConnectToMailClient(smtp_server, smtp_Login, smtp_pass, smtp_domain);
				MailMessage send = DebugPanel.PrepareMessageToSend(From, To, Subject, Message, isHTML);
				DebugPanel.SendMessage(mailClient, send, new SendCompletedEventHandler(DebugPanel.SendCompletedCallback));
			}
			catch (Exception exception)
			{
				DebugPanel.Log(string.Concat("Ошибка создания Email ", exception.Message));
			}
		}

		internal static void SendMail(CConfig _conf, string To, string Subject, string Message, bool isHTML, string domain)
		{
			try
			{
				string mSendFrom = _conf.m_send_from;
				string mSmtpLogin = _conf.m_smtp_login;
				if (string.IsNullOrEmpty(mSendFrom))
				{
					mSendFrom = mSmtpLogin;
				}
				SmtpClient mailClient = DebugPanel.ConnectToMailClient(_conf.m_smtp_server, mSmtpLogin, _conf.m_smtp_pass, domain);
				MailMessage send = DebugPanel.PrepareMessageToSend(mSendFrom, To, Subject, Message, isHTML);
				DebugPanel.SendMessage(mailClient, send, new SendCompletedEventHandler(DebugPanel.SendCompletedCallback));
			}
			catch (Exception exception)
			{
				DebugPanel.Log(string.Concat("Ошибка создания Email ", exception.Message));
			}
		}

		public static bool SendMessage(SmtpClient client, MailMessage mm, SendCompletedEventHandler SendCompletedCallback)
		{
			bool flag = false;
			try
			{
				client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback.Invoke);
				client.SendAsync(mm, null);
				flag = true;
			}
			catch (Exception exception)
			{
				DebugPanel.Log(string.Concat("Ошибка отправки Email ", exception.Message));
				flag = false;
			}
			return flag;
		}

		internal static event DebugPanel.MessageEventHandler ErrorOccured;

		internal static event DebugPanel.MessageEventHandler FatalErrorOccured;

		internal static event DebugPanel.MessageEventHandler FileCopied;

		internal static event DebugPanel.MessageEventHandler FileDeleted;

		internal static event DebugPanel.MessageEventHandler FileMoved;

		internal class IgnoredFile
		{
			internal static string extension;

			private static Regex reg_try_number;

			static IgnoredFile()
			{
				DebugPanel.IgnoredFile.extension = ".ignored";
				DebugPanel.IgnoredFile.reg_try_number = new Regex("[.](\\d)[.]");
			}

			public IgnoredFile()
			{
			}

			internal static bool IsIgnored(RBServer.Debug_classes.FileInfo _file)
			{
				if ((new System.IO.FileInfo(_file.FullName)).Extension == DebugPanel.IgnoredFile.extension)
				{
					return true;
				}
				return false;
			}

			internal static bool IsIgnored(string _file)
			{
				if ((new System.IO.FileInfo(_file)).Extension == DebugPanel.IgnoredFile.extension)
				{
					return true;
				}
				return false;
			}

			internal static void MakeFileIgnored(RBServer.Debug_classes.FileInfo _file)
			{
				_file.MoveTo(string.Concat(_file.FullName, DebugPanel.IgnoredFile.extension));
				DebugPanel.OnErrorOccured(string.Concat("Неверный файл в структуре ", _file.FullName, " теперь игнорируется"));
			}

			internal static void MakeFileIgnored(string _file)
			{
				RBServer.Debug_classes.FileInfo fileInfo = new RBServer.Debug_classes.FileInfo(_file);
				fileInfo.MoveTo(string.Concat(_file, DebugPanel.IgnoredFile.extension));
				DebugPanel.OnErrorOccured(string.Concat("Неверный файл в структуре ", fileInfo.FullName, " теперь игнорируется"));
			}

			internal static void TryToMakeUnIgnored(RBServer.Debug_classes.FileInfo file_info, int count_max)
			{
				try
				{
					string str = "";
					str = file_info.Name.Replace(file_info.Extension, "");
					System.IO.DirectoryInfo directory = file_info.file.Directory;
					List<System.IO.FileInfo> list = directory.GetFiles(string.Concat(str, "*")).ToList<System.IO.FileInfo>();
					if (!DebugPanel.IgnoredFile.reg_try_number.IsMatch(file_info.Name))
					{
						if (list.Count == 1)
						{
							byte[] numArray = System.IO.File.ReadAllBytes(file_info.FullName);
							file_info.file.CopyTo(Path.Combine(directory.FullName, str), true);
							file_info.file.Delete();
							System.IO.File.WriteAllBytes(Path.Combine(directory.FullName, string.Concat(str, ".0", DebugPanel.IgnoredFile.extension)), numArray);
						}
						if (list.Count == 2)
						{
							System.IO.FileInfo fileInfo = (
								from a in list
								where DebugPanel.IgnoredFile.reg_try_number.IsMatch(a.Name)
								select a).First<System.IO.FileInfo>();
							int num = int.Parse(DebugPanel.IgnoredFile.reg_try_number.Match(fileInfo.Name).Groups[1].Value);
							if (num < count_max)
							{
								num++;
								byte[] numArray1 = System.IO.File.ReadAllBytes(file_info.FullName);
								file_info.file.CopyTo(Path.Combine(directory.FullName, str), true);
								file_info.file.Delete();
								string fullName = directory.FullName;
								object[] objArray = new object[] { str, ".", num, DebugPanel.IgnoredFile.extension };
								System.IO.File.WriteAllBytes(Path.Combine(fullName, string.Concat(objArray)), numArray1);
								fileInfo.Delete();
							}
							else
							{
								System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(Path.Combine(DebugPanel.back_folder_path, "Ignored"));
								directoryInfo.CreateOrReturn();
								fileInfo.CopyTo(Path.Combine(directoryInfo.FullName, fileInfo.Name), true);
								list.ForEach((System.IO.FileInfo a) => a.Delete());
							}
						}
						if (list.Count > 2)
						{
							DebugPanel.SendErrorMessageToAdmin(new MessageEventArgs(string.Concat("Проверить ignored файлы ", file_info.FullName)));
						}
					}
					else
					{
						str = DebugPanel.IgnoredFile.reg_try_number.Replace(string.Concat(str, "."), "");
						list = directory.GetFiles(string.Concat(str, "*")).ToList<System.IO.FileInfo>();
						if (list.Count == 1)
						{
							System.IO.FileInfo fileInfo1 = (
								from a in list
								where DebugPanel.IgnoredFile.reg_try_number.IsMatch(a.Name)
								select a).First<System.IO.FileInfo>();
							int num1 = int.Parse(DebugPanel.IgnoredFile.reg_try_number.Match(fileInfo1.Name).Groups[1].Value);
							if (num1 < count_max)
							{
								num1++;
								byte[] numArray2 = System.IO.File.ReadAllBytes(file_info.FullName);
								file_info.file.CopyTo(Path.Combine(directory.FullName, str), true);
								file_info.file.Delete();
								string fullName1 = directory.FullName;
								object[] objArray1 = new object[] { str, ".", num1, DebugPanel.IgnoredFile.extension };
								System.IO.File.WriteAllBytes(Path.Combine(fullName1, string.Concat(objArray1)), numArray2);
								fileInfo1.Delete();
							}
							else
							{
								System.IO.DirectoryInfo directoryInfo1 = new System.IO.DirectoryInfo(Path.Combine(DebugPanel.back_folder_path, "Ignored"));
								directoryInfo1.CreateOrReturn();
								fileInfo1.CopyTo(Path.Combine(directoryInfo1.FullName, fileInfo1.Name), true);
								list.ForEach((System.IO.FileInfo a) => a.Delete());
							}
						}
					}
				}
				catch (Exception exception)
				{
					DebugPanel.Log(exception, "Ошибка разыгнорирования");
				}
			}
		}

		public delegate void MessageEventHandler(object o, MessageEventArgs e);
	}
}
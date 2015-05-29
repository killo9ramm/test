using Debug_classes;
using InternalClasses;
using RBServer.Debug_classes;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;


namespace RBServer
{
	public class Program
	{
		public static CTransfer _transfer;

		public static CConfig _config;

		private static uint TH32CS_SNAPPROCESS;

		static Program()
		{
			Program.TH32CS_SNAPPROCESS = 2;
		}

		public Program()
		{
		}

		private static void Check_updation(string folder)
		{
			try
			{
				string str = Path.Combine(System.IO.Directory.GetCurrentDirectory(), folder);
				System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(str);
				if (directoryInfo.Exists && directoryInfo.GetFiles() != null && (int)directoryInfo.GetFiles().Length != 0)
				{
					Program.ProcessStart(Program._config.update_process);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Program._transfer.Log(string.Concat("Не удалось обновиться... Error: ", exception.Message), 0);
			}
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

		private static Process GetParentProcess()
		{
			int num = 0;
			int id = Process.GetCurrentProcess().Id;
			IntPtr intPtr = Program.CreateToolhelp32Snapshot(Program.TH32CS_SNAPPROCESS, 0);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			Program.PROCESSENTRY32 pROCESSENTRY32 = new Program.PROCESSENTRY32()
			{
				dwSize = (uint)Marshal.SizeOf(typeof(Program.PROCESSENTRY32))
			};
			if (!Program.Process32First(intPtr, ref pROCESSENTRY32))
			{
				return null;
			}
			do
			{
				if ((long)id != (long)pROCESSENTRY32.th32ProcessID)
				{
					continue;
				}
				num = (int)pROCESSENTRY32.th32ParentProcessID;
			}
			while (num == 0 && Program.Process32Next(intPtr, ref pROCESSENTRY32));
			if (num <= 0)
			{
				return null;
			}
			return Process.GetProcessById(num);
		}

		private static string getProcessInfo()
		{
			string str = "";
			Program.TryAction(() => {
				Process parentProcess = Program.GetParentProcess();
				if (parentProcess != null)
				{
					str = string.Concat(str, "\r\n Parent Process\r\n ProcessName: ", parentProcess.ProcessName);
					str = string.Concat(str, "\r\n MachineName: ", parentProcess.MachineName);
					if (parentProcess.MainModule != null)
					{
						str = string.Concat(str, "\r\n MainModule: ", parentProcess.MainModule.ModuleName);
						str = string.Concat(str, "\r\n MainModulePath: ", parentProcess.MainModule.FileName);
					}
				}
			}, new Action<string>(DebugPanel.Log));
			return str;
		}

		private static string getUserInfo()
		{
			string str = "";
			Program.TryAction(() => {
				(new SecurityPermission(SecurityPermissionFlag.ControlPrincipal)).Assert();
				WindowsIdentity current = WindowsIdentity.GetCurrent();
				if (current != null)
				{
					str = string.Concat(new string[] { str, "\r\n User: ", current.Name, " UserId: ", current.User.Value });
					str = string.Concat(str, "\r\n OwnerId: ", current.Owner.Value);
				}
			}, new Action<string>(DebugPanel.Log));
			return str;
		}

		private static void Main(string[] args)
		{
            
			DebugPanel.SendAdminMessageIsON = true;
			Program._transfer = new CTransfer();
            Program._transfer.Log("RBService started...0", 0);
			Program._config = new CConfig();
			Program._config.SetParam("parm", "name", "reboot", "2");

            Program._transfer.Log("RBService started...1", 0);
#if(DEBUG)
            //try
            //{
            //    System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(@"H:\RBSERV_PRODUCTION");
            //    long num = long.Parse(Program._config.rbs_intern_degugger_folder_size);
            //    int files_count = int.Parse(Program._config.remove_backup_files_count);
            //    directoryInfo.DeleteOldFilesInDir(num, files_count);
            //}
            //catch (Exception exception)
            //{
            //    Program._transfer.Log(exception.Message, 2);
            //}
#endif
            

			Process[] processesByName = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
			if ((int)processesByName.Length <= 2)
			{
                Program._transfer.Log("RBService started...2", 0);
				DebugPanel.back_folder_path = Program._config.rbs_intern_degugger_folder;
				DateTime now = DateTime.Now;
				string str = string.Concat("Сервер запущен : ", now.ToString());
				string userInfo = Program.getUserInfo();
				string processInfo = Program.getProcessInfo();
				Program._transfer.SendMail(Program._config.email_rbs_err, "Error RBSERVER--", string.Concat(str, userInfo, processInfo), false);
				DebugPanel.Log(string.Concat(str, userInfo, processInfo));
				int mTimer = Program._config.m_timer;
				while (true)
				{
                    Program._transfer.Log("RBService started...3", 0);
					if (!Program._config.rbs_paused)
					{
						try
						{
							if (!System.IO.Directory.Exists("log"))
							{
								System.IO.Directory.CreateDirectory("log");
							}
							Program._transfer.Log("RBService started...", 0);
							while (true)
							{
								DebugPanel.back_folder_path = Program._config.rbs_intern_degugger_folder;
								DebugPanel.IsON = Program._config.rbs_intern_degugger_state;

                                Program._transfer.Job();
                                Thread.Sleep(mTimer);
                                LogProcessing.log_processing_routine();

                                if (DebugPanel.IsON && (DateTime.Now.Hour >= 23 || DateTime.Now.Hour <= 9))
                                {
									try
									{
										System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(DebugPanel.back_folder_path);
										long num = long.Parse(Program._config.rbs_intern_degugger_folder_size);
                                        int files_count=int.Parse(Program._config.remove_backup_files_count);
										directoryInfo.DeleteOldFilesInDir(num, files_count);
									}
									catch (Exception exception)
									{
										Program._transfer.Log(exception.Message, 2);
									}
								}
							}
						}
						catch (Exception exception1)
						{
							Program._transfer.Log(exception1.Message, 0);
						}
					}
					else
					{
						Thread.Sleep(mTimer);
					}
				}
			}
			string str1 = string.Concat("Не удалось запустить рбсервер т.к. уже работает 1 инстанс ", processesByName[1].ProcessName);
			DebugPanel.Log(str1);
			Program._transfer.Log(str1, 0);
			Program._transfer.Log(str1, 2);
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool Process32First(IntPtr hSnapshot, ref Program.PROCESSENTRY32 lppe);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool Process32Next(IntPtr hSnapshot, ref Program.PROCESSENTRY32 lppe);

		private static void ProcessStart(string fullName)
		{
			Process process = null;
			string str = fullName;
			try
			{
				process = new Process();
				process.StartInfo.FileName = str;
				process.StartInfo.CreateNoWindow = false;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
				process.StartInfo.UseShellExecute = false;
				process.Start();
				Program._transfer.Log(string.Concat("Process Started: ", str), 0);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Program._transfer.Log(string.Concat("Process Started Error: ", str, " Error: ", exception.Message), 0);
				throw exception;
			}
		}

		private static void test_method()
		{
		}

		private static void TryAction(Action act, Action<string> Log)
		{
			try
			{
				act();
			}
			catch (Exception exception)
			{
				Log(string.Concat("Exception ", exception.Message));
			}
		}

		public struct PROCESSENTRY32
		{
			public uint dwSize;

			public uint cntUsage;

			public uint th32ProcessID;

			public IntPtr th32DefaultHeapID;

			public uint th32ModuleID;

			public uint cntThreads;

			public uint th32ParentProcessID;

			public int pcPriClassBase;

			public uint dwFlags;

			public string szExeFile;
		}
	}
}
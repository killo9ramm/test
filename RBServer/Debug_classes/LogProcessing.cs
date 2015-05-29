using Ionic.Zip;
using RBServer.Debug_classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Debug_classes
{
	internal class LogProcessing
	{
		public LogProcessing()
		{
		}

		private static void kkm_log_processing()
		{
			Action<System.IO.FileInfo> action = null;
			Regex regex = new Regex("(?i:kkm)_(\\d+)_(\\d+)_(\\d+)_(\\d+)_(\\d+)[.]log");
			Regex regex1 = new Regex("((\\d){4}).*?(\\d{1,2}).*?(\\d{1,2})");
			string str1 = "kkm_log";
			List<System.IO.FileInfo> list = (new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory())).GetFiles("kkm*.log").ToList<System.IO.FileInfo>();
			list.ForEach((System.IO.FileInfo a) => RBServer.Debug_classes.File.Move(a.FullName, Path.Combine(str1, a.Name)));
			list = (new System.IO.DirectoryInfo(str1)).GetFiles("kkm*.log").ToList<System.IO.FileInfo>();
			list.Sort((System.IO.FileInfo a, System.IO.FileInfo b) => {
				DateTime? nullable = LogProcessing.parseDate(regex1, a.Name);
				DateTime? nullable1 = LogProcessing.parseDate(regex1, b.Name);
				if (nullable.HasValue && nullable1.HasValue)
				{
					return nullable.Value.CompareTo(nullable1.Value);
				}
				return a.CreationTime.CompareTo(b.CreationTime);
			});
			DateTime dateTime2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
			list = (
				from a in list
				where LogProcessing.parseDate(a) < dateTime2
				select a).ToList<System.IO.FileInfo>();
			List<System.IO.FileInfo> fileInfos = new List<System.IO.FileInfo>();
			int month = -1;
			list.ForEach((System.IO.FileInfo a) => {
				DateTime dateTime = LogProcessing.parseDate(a);
				if (month == -1)
				{
					fileInfos.Add(a);
					month = dateTime.Month;
					return;
				}
				if (month != dateTime.Month)
				{
					using (ZipFile zipFiles = new ZipFile())
					{
						DateTime dateTime1 = LogProcessing.parseDate(fileInfos[0]);
						string str = Path.Combine(str1, string.Concat(new object[] { "kkm_", dateTime1.Month, ".", dateTime1.Year, "_archive.zip" }));
						zipFiles.AddFiles(
							from b in fileInfos
							select Path.Combine(str1, b.Name));
						zipFiles.Save(str);
						List<System.IO.FileInfo> filesToArchive = fileInfos;
						if (action == null)
						{
							action = (System.IO.FileInfo c) => c.Delete();
						}
						filesToArchive.ForEach(action);
					}
					fileInfos.Clear();
					month = dateTime.Month;
				}
				fileInfos.Add(a);
			});
		}

		public static void log_processing_routine()
		{
			try
			{
				LogProcessing.rbservice2_log_processing(5);
				LogProcessing.rbservice0_log_processing(10);
				LogProcessing.rbs_log_temp_processing(5);
			}
			catch (Exception exception)
			{
				DebugPanel.Log(string.Concat("log_processing_routine error ", exception.Message));
			}
		}

		private void nlog_processing()
		{
			string str = "Log";
			List<System.IO.FileInfo> list = (
				from a in (new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory())).GetFiles("RBClient.nlog*").ToList<System.IO.FileInfo>()
				where a.Name != "RBClient.nlog"
				select a).ToList<System.IO.FileInfo>();
			list.Sort((System.IO.FileInfo a, System.IO.FileInfo b) => a.Name.CompareTo(b.Name));
			if (list.Count == 0)
			{
				return;
			}
			using (ZipFile zipFiles = new ZipFile())
			{
				string[] shortDateString = new string[] { "Rbclient_", null, null, null, null };
				DateTime creationTime = list.First<System.IO.FileInfo>().CreationTime;
				shortDateString[1] = creationTime.ToShortDateString();
				shortDateString[2] = "-";
				DateTime lastWriteTime = list.Last<System.IO.FileInfo>().LastWriteTime;
				shortDateString[3] = lastWriteTime.ToShortDateString();
				shortDateString[4] = "nlog_archive.zip";
				string str1 = Path.Combine(str, string.Concat(shortDateString));
				zipFiles.AddFiles(
					from b in list
					select b.Name);
				zipFiles.Save(str1);
				list.ForEach((System.IO.FileInfo c) => c.Delete());
			}
		}

		private static DateTime? parseDate(Regex reg, string file_name)
		{
			DateTime? nullable = null;
			try
			{
				string[] value = new string[] { reg.Match(file_name).Groups[1].Value, reg.Match(file_name).Groups[3].Value, reg.Match(file_name).Groups[4].Value };
				string[] strArrays = value;
				nullable = new DateTime?(DateTime.Parse(string.Format("{0}-{1}-{2}", strArrays[0], strArrays[1], strArrays[2])));
			}
			catch (Exception exception)
			{
				DebugPanel.Log(string.Concat("Ошибка парсинга файла логов ", file_name));
			}
			return nullable;
		}

		private static DateTime parseDate(System.IO.FileInfo file_name)
		{
			Regex regex = new Regex("((\\d){4}).*?(\\d{1,2}).*?(\\d{1,2})");
			DateTime? nullable = LogProcessing.parseDate(regex, file_name.Name);
			if (!nullable.HasValue)
			{
				return file_name.CreationTime;
			}
			return nullable.Value;
		}

		private void RBClient_log_processing()
		{
			string str = "Log";
			DateTime dateTime = DateTime.Now.AddDays(-10);
			List<System.IO.FileInfo> list = (new System.IO.DirectoryInfo(Path.Combine(System.IO.Directory.GetCurrentDirectory(), str))).GetFiles("*RBClient.log").ToList<System.IO.FileInfo>();
			list = (
				from a in list
				where LogProcessing.parseDate(a) < dateTime
				select a).ToList<System.IO.FileInfo>();
			list.Sort((System.IO.FileInfo a, System.IO.FileInfo b) => LogProcessing.parseDate(a).CompareTo(LogProcessing.parseDate(b)));
			if (list.Count == 0)
			{
				return;
			}
			using (ZipFile zipFiles = new ZipFile())
			{
				LogProcessing.parseDate(list[0]);
				string str1 = str;
				string[] shortDateString = new string[] { "Rbclient_", null, null, null, null };
				DateTime dateTime1 = LogProcessing.parseDate(list.First<System.IO.FileInfo>());
				shortDateString[1] = dateTime1.ToShortDateString();
				shortDateString[2] = "-";
				DateTime dateTime2 = LogProcessing.parseDate(list.Last<System.IO.FileInfo>());
				shortDateString[3] = dateTime2.ToShortDateString();
				shortDateString[4] = "_archive.zip";
				string str2 = Path.Combine(str1, string.Concat(shortDateString));
				zipFiles.AddFiles(
					from b in list
					select Path.Combine(str, b.Name));
				zipFiles.Save(str2);
				list.ForEach((System.IO.FileInfo c) => c.Delete());
			}
		}

		private static void rbs_log_temp_processing(int logFilescount)
		{
			string str = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "log");
			List<System.IO.FileInfo> list = (new System.IO.DirectoryInfo(str)).GetFiles("*rbs_log_temp*").ToList<System.IO.FileInfo>();
			list.Sort((System.IO.FileInfo a, System.IO.FileInfo b) => b.CreationTime.CompareTo(a.CreationTime));
			List<System.IO.FileInfo> fileInfos = list.TakeWhile<System.IO.FileInfo>((System.IO.FileInfo a, int i) => i < logFilescount).ToList<System.IO.FileInfo>();
			list.ForEach((System.IO.FileInfo a) => {
				if (!fileInfos.Contains(a))
				{
					a.Delete();
				}
			});
		}

		private static void rbservice0_log_processing(int daysCount)
		{
			string str = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "log");
			DateTime dateTime = DateTime.Now.AddDays((double)(-daysCount));
			List<System.IO.FileInfo> list = (
				from a in (new System.IO.DirectoryInfo(str)).GetFiles("*_RBService_0.log").ToList<System.IO.FileInfo>()
				where LogProcessing.parseDate(a) < dateTime
				select a).ToList<System.IO.FileInfo>();
			if (list.Count == 0)
			{
				return;
			}
			using (ZipFile zipFiles = new ZipFile())
			{
				string str1 = str;
				string[] shortDateString = new string[] { "RBService_0", null, null, null, null };
				DateTime dateTime1 = LogProcessing.parseDate(list.First<System.IO.FileInfo>());
				shortDateString[1] = dateTime1.ToShortDateString();
				shortDateString[2] = "-";
				DateTime dateTime2 = LogProcessing.parseDate(list.Last<System.IO.FileInfo>());
				shortDateString[3] = dateTime2.ToShortDateString();
				shortDateString[4] = "_archive.zip";
				string str2 = Path.Combine(str1, string.Concat(shortDateString));
				zipFiles.AddFiles(
					from b in list
					select Path.Combine(str, b.Name));
				zipFiles.Save(str2);
				list.ForEach((System.IO.FileInfo c) => c.Delete());
			}
		}

		private static void rbservice2_log_processing(int daysCount)
		{
			string str = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "log");
			DateTime dateTime = DateTime.Now.AddDays((double)(-daysCount));
			List<System.IO.FileInfo> list = (
				from a in (new System.IO.DirectoryInfo(str)).GetFiles("*_RBService_2.log").ToList<System.IO.FileInfo>()
				where LogProcessing.parseDate(a) < dateTime
				select a).ToList<System.IO.FileInfo>();
			list.ForEach((System.IO.FileInfo a) => a.Delete());
		}
	}
}
using Ftp;
using Ionic.Zip;
using RBClient.Classes;
using RBServer.Debug_classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using WebService1;
using System.Reflection;
using RBMModel;
using ReportClasses;
using RBClient;

namespace RBServer
{
	public class CTransfer
	{
		private string log_file_0 = "log\\RBService_0.log";

		private string log_file_1 = "log\\RBService_1.log";

		private string log_file_2 = "log\\RBService_1.log";

		private string _errfile = "";

		public DateTime date_create;

		public DateTime date_send;

		public bool loadS;

		public bool writeBD;

		internal CConfig _config = new CConfig();

		internal static string ErrorFile;

		private Regex param_reg = new Regex("(?-s)(.+=.+)");

		private Regex date_reg = new Regex("\\d{2}[.]\\d{2}[.]\\d{4}\\s\\d{2}[:]\\d{2}[:]\\d{2}");

		public static CTransfer.YZreport zreport_cl;

		public static CTransfer.YZreport yreport_cl;

		static CTransfer()
		{
			CTransfer.ErrorFile = "";
		}

		public CTransfer()
		{
		}

		private string AttachZeroToDate(int c)
		{
			if (c >= 10)
			{
				return c.ToString();
			}
			return string.Concat("0", c.ToString());
		}

		private void BanSendMail(SqlConnection conn, string _code_teremok, int _doc_id, string doc_type, string subj)
		{
            if (!_config.enable_email_send_flag)
            {
                return;
            }

			if (subj == "")
			{
				subj = string.Concat("ALARM Нарушен срок отправки ", this.DocTypeID(conn, doc_type), " ", this.GetTeremokName(_code_teremok, conn));
			}
			string str = "<html><body><table border=1>";
			if (_doc_id != 0)
			{
				str = string.Concat(str, "<tr>");
				str = string.Concat(str, "<td>Ресторан </td>");
				str = string.Concat(str, "<td align=center>", this.GetTeremokName(_code_teremok, conn), "</td>");
				str = string.Concat(str, "<tr>");
				str = string.Concat(str, "<td>Дата создания </td>");
				object obj = str;
				object[] dateCreate = new object[] { obj, "<td align=center>", this.date_create, "</td>" };
				str = string.Concat(dateCreate);
				str = string.Concat(str, "<tr>");
				str = string.Concat(str, "<td align=center>Дата подготовки к отправке </td>");
				object obj1 = str;
				object[] dateSend = new object[] { obj1, "<td align=center>", this.date_send, "</td>" };
				str = string.Concat(dateSend);
				str = string.Concat(str, "</table></body></html>");
				str = string.Concat(str, "<html><body><table border=0>");
				str = string.Concat(str, "</table></body></html>");
				str = string.Concat(str, this.OrderDetailForMessage(_doc_id, conn));
				this.SendMail(this._config.email_rbs_alarm_restoran, subj, str, true);
			}
		}

		private void CatchError(DataTable dt)
		{
			string str = "";
			string str1 = "";
			string str2 = "";
			DataRow dataRow = null;
			try
			{
				foreach (DataRow row in dt.Rows)
				{
					dataRow = row;
					Convert.ToDateTime(row[1].ToString());
					Convert.ToInt32(row[3].ToString());
					Convert.ToInt32(row[4].ToString());
					str = row[6].ToString();
					str1 = row[1].ToString();
					str2 = row[5].ToString();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (dataRow == null)
				{
					string emailRbsErrSvyaznoyCsv = this._config.email_rbs_err_svyaznoy_csv;
					string[] strArrays = new string[] { "Теремок: ", str.ToString(), " Дата: ", str1.ToString(), " Чек: ", str2.ToString(), "error: ", exception.Message };
					this.SendMail(emailRbsErrSvyaznoyCsv, "Сбой выгрузки Связного", string.Concat(strArrays), false);
				}
				else
				{
					string emailRbsErrSvyaznoyCsv1 = this._config.email_rbs_err_svyaznoy_csv;
					string[] strArrays1 = new string[] { "Теремок: ", str.ToString(), " Дата: ", str1.ToString(), " Чек: ", str2.ToString(), "error: ", exception.Message, null };
					object[] objArray = new object[] { dataRow[1].ToString(), dataRow[2].ToString(), dataRow[3].ToString(), dataRow[4].ToString(), dataRow[5].ToString(), dataRow[6].ToString() };
					strArrays1[8] = string.Format("1 {0} 2{1} 3{2} 4{3} 5{4} 6{5}", objArray);
					this.SendMail(emailRbsErrSvyaznoyCsv1, "Сбой выгрузки Связного", string.Concat(strArrays1), false);
				}
			}
		}

		public void CheckItemParse(int z_id, string line, DateTime datetime, SqlConnection conn)
		{
			CConfig cConfig = new CConfig();
			int num = 0;
			try
			{
				if (line != "-")
				{
					if (line.Substring(0, 13) != "Возврат по че")
					{
						double num1 = Convert.ToDouble(this.ParceAmount(line.Substring(13, 12)));
						double num2 = Convert.ToDouble(this.ParceAmount(line.Substring(25, 11)));
						double num3 = Convert.ToDouble(this.ParceAmount(line.Substring(36, 11)));
						SqlCommand sqlCommand = new SqlCommand()
						{
							Connection = conn
						};
						string str = "INSERT INTO t_CheckItem (ch_mnome_id, ch_zreport_id, ch_amount1, ch_amount2, ch_count, ch_datetime) VALUES(@ch_mnome_id, @ch_zreport_id, @ch_amount1, @ch_amount2, @ch_count, @ch_datetime)";
						sqlCommand.Parameters.AddWithValue("@ch_mnome_id", num);
						sqlCommand.Parameters.AddWithValue("@ch_zreport_id", z_id);
						sqlCommand.Parameters.AddWithValue("@ch_amount1", num2);
						sqlCommand.Parameters.AddWithValue("@ch_amount2", num3);
						sqlCommand.Parameters.AddWithValue("@ch_count", num1);
						sqlCommand.Parameters.AddWithValue("@ch_datetime", datetime);
						sqlCommand.CommandText = str;
						sqlCommand.ExecuteNonQuery();
					}
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private bool CheckMiniPOSFile(string file_name, string KKM, SqlConnection conn)
		{
			bool flag;
			try
			{
				string[] fileName = new string[] { "SELECT count(*) FROM t_MiniPOSFile WHERE mf_file = '", file_name, "' AND mf_kkm = '", KKM, "'" };
				flag = (Convert.ToInt16((new SqlCommand(string.Concat(fileName), conn)).ExecuteScalar()) != 0 ? true : false);
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return flag;
		}

		private void CompleteCheckMiniPOS(string file_name, string KKM, SqlConnection conn)
		{
			CConfig cConfig = new CConfig();
			try
			{
				if (RBServer.Debug_classes.File.Exists(string.Concat(cConfig.m_app_folder, "\\Kassa2.Rep")))
				{
					RBServer.Debug_classes.File.Delete(string.Concat(cConfig.m_app_folder, "\\Kassa2.Rep"));
				}
				SqlCommand sqlCommand = new SqlCommand("INSERT INTO t_MiniPOSFile(mf_file, mf_kkm) VALUES(@mf_file, @mf_kkm)", conn);
				sqlCommand.Parameters.AddWithValue("@mf_file", file_name);
				sqlCommand.Parameters.AddWithValue("@mf_kkm", KKM);
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private void Copy_log_toOlder_date(string logfile)
		{
			string[] str = new string[] { "log\\", null, null, null, null, null, null, null };
			str[1] = DateTime.Now.Year.ToString();
			str[2] = "_";
			str[3] = DateTime.Now.Month.ToString();
			str[4] = "_";
			str[5] = DateTime.Now.Day.ToString();
			str[6] = "_";
			str[7] = logfile;
			string str1 = string.Concat(str);
			if (!RBServer.Debug_classes.File.Exists(str1))
			{
				logfile = string.Concat("log\\", logfile);
				if (RBServer.Debug_classes.File.Exists(logfile))
				{
					RBServer.Debug_classes.File.Move(logfile, str1);
				}
			}
		}

		private void CopyToCSV(DataTable dt)
		{
			object[] year = new object[] { "teremok_reestr_", null, null, null, null, null, null };
			year[1] = DateTime.Now.Year;
			year[2] = "_";
			year[3] = DateTime.Now.Month;
			year[4] = "_";
			year[5] = DateTime.Now.Day;
			year[6] = ".csv";

			RBServer.Debug_classes.StreamWriter streamWriter = new RBServer.Debug_classes.StreamWriter(string.Concat(year), false);
			try
			{
				try
				{
					streamWriter.WriteLine("EAN13;DATE;TIME;ART;QUANTITY;PRICE;SUM;OP_ID;TO_ID");
					foreach (DataRow row in dt.Rows)
					{
						try
						{
							DateTime dateTime = Convert.ToDateTime(row[1].ToString());
							object[] str = new object[] { row[0].ToString(), ";", this.AttachZeroToDate(dateTime.Day), ".", this.AttachZeroToDate(dateTime.Month), ".", this.AttachZeroToDate(dateTime.Year), ";", this.AttachZeroToDate(dateTime.Hour), ":", this.AttachZeroToDate(dateTime.Minute), ":", this.AttachZeroToDate(dateTime.Second), ";", row[2].ToString(), ";", Convert.ToInt32(row[3].ToString()), ";", Convert.ToInt32(row[4].ToString()) / Convert.ToInt32(row[3].ToString()) * 100, ";", Convert.ToInt32(row[4].ToString()) * 100, ";", row[5].ToString(), ";", row[6].ToString() };
							streamWriter.WriteLine(string.Concat(str));
						}
						catch (FormatException formatException1)
						{
							FormatException formatException = formatException1;
							object[] message = new object[] { formatException.Message, " в файле ", streamWriter, " в строке выборки ", null };
							int num = dt.Rows.IndexOf(row);
							message[4] = num.ToString();
							this.Log(string.Concat(message), 0);
							Console.WriteLine(string.Concat("Оппа, не верный формат входных данных!\n", formatException.ToString()));
						}
					}
				}
				catch (InvalidCastException invalidCastException)
				{
					Console.WriteLine(string.Concat("Оппа, проблемы с преобразованим к типу!\n", invalidCastException.ToString()));
				}
				catch (Exception exception)
				{
					Console.WriteLine(string.Concat("Оппа, что-то произошло, но я не знаю что. :(\n", exception.ToString()));
				}
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
				}
			}
		}

		private bool date_incremention(string line, int days_increment)
		{
			bool flag = false;
			string value = "";
			Regex regex = new Regex(".*?(20\\d{2}[_]?\\d{2}[_]?\\d{2})");
			if (regex.IsMatch(line))
			{
				try
				{
					value = regex.Match(line).Groups[1].Value;
					if (value != "")
					{
						value = value.Replace("_", "");
						DateTime dateTime = DateTime.Now.AddDays((double)days_increment);
						DateTime dateTime1 = DateTime.Now.AddDays((double)(-days_increment));
						DateTime dateTime2 = new DateTime(int.Parse(value.Substring(0, 4)), int.Parse(value.Substring(4, 2)), int.Parse(value.Substring(6, 2)));
						if (dateTime2 > dateTime || dateTime2 < dateTime1)
						{
							flag = true;
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					string str = string.Concat("Не парсится дата документа filename:", line, " error:", exception.Message);
					DebugPanel.Log(str);
				}
			}
			return flag;
		}

		private string DocTypeID(SqlConnection conn, string doc_id)
		{
			string str;
			string str1 = "";
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				str1 = string.Concat("SELECT doctype_name FROM t_DocTypeRef WHERE doctype_id='", doc_id, "'");
				sqlCommand.CommandText = str1;
				str = sqlCommand.ExecuteScalar().ToString();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				CTransfer.ErrorFile = string.Concat("error: ", conn.ToString(), " command: ", str1);
				throw exception;
			}
			return str;
		}

		private bool DoExtract(string file_path, string path_dest)
		{
			bool flag;
			try
			{
				if (!RBServer.Debug_classes.Directory.Exists(path_dest))
				{
					RBServer.Debug_classes.Directory.CreateDirectory(path_dest);
					this.Log(string.Concat("Создана директория ", path_dest), 0);
				}
				using (ZipFile zipFiles = ZipFile.Read(file_path))
				{
					foreach (ZipEntry zipEntry in zipFiles)
					{
						RBServer.Debug_classes.FileInfo fileInfo = new RBServer.Debug_classes.FileInfo(zipEntry.FileName);
						using (RBServer.Debug_classes.FileStream fileStream = new RBServer.Debug_classes.FileStream(string.Concat(path_dest, "\\", fileInfo.Name), FileMode.Create))
						{
							zipEntry.Extract(fileStream);
						}
					}
				}
				this.Log(string.Concat("Распакованы файл ", file_path), 0);
				flag = true;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string[] filePath = new string[] { "Ошибка распаковки zip архива. паф: ", file_path, "  дест: ", path_dest, "  error: ", exception.Message };
				this.Log(string.Concat(filePath), 0);
				flag = false;
			}
			return flag;
		}

		private void ExchangeFTP(FtpSession m_client)
		{
			try
			{
				CConfig cConfig = new CConfig();
				string str = cConfig.m_ftp_folder_name.ToString();
				if (str != "")
				{
					m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(str.ToLower());
					m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory("in");
					List<string> strs = new List<string>();
					foreach (FtpFile file in m_client.CurrentDirectory.Files)
					{
						if (file.IsFile)
						{
							char[] charArray = "_".ToCharArray();
							string[] strArrays = file.Name.Substring(0, file.Name.Length - 4).Split(charArray);
							if ((int)strArrays.Length != 2)
							{
								string str1 = strArrays[0];
								string str2 = strArrays[1];
								Convert.ToInt32(strArrays[5]);
								if (str2 == "1" || str2 == "2" || str2 == "15" || str2 == "16")
								{
									this.GetFileResuming(m_client, string.Concat(cConfig.m_folder_1c_out, "\\", file.Name), file.Name);
									strs.Add(file.Name);
								}
								else
								{
									this.GetFileResuming(m_client, string.Concat(cConfig.m_folder_load, "\\", file.Name), file.Name);
									strs.Add(file.Name);
								}
							}
						}
						foreach (string str3 in strs)
						{
							m_client.CurrentDirectory.RemoveFile(str3);
						}
					}
					m_client.CurrentDirectory = m_client.CurrentDirectory.Parent;
					m_client.CurrentDirectory = m_client.CurrentDirectory.Parent;
					m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory(str.ToLower());
					m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory("out");
					List<string> strs1 = new List<string>();
					foreach (FtpFile ftpFile in m_client.CurrentDirectory.Files)
					{
						if (!ftpFile.IsFile)
						{
							continue;
						}
						this.GetFileResuming(m_client, string.Concat(cConfig.m_folder_load, "\\", ftpFile.Name), ftpFile.Name);
						strs1.Add(ftpFile.Name);
					}
					foreach (string str4 in strs1)
					{
						m_client.CurrentDirectory.RemoveFile(str4);
					}
					m_client.CurrentDirectory = m_client.CurrentDirectory.Parent;
					m_client.CurrentDirectory = m_client.CurrentDirectory.Parent;
				}
			}
			catch (Exception exception)
			{
				this.Log(string.Concat("Ошибка E0003", exception), 0);
			}
		}

		private void ExchangeFTPSB(FtpSession m_client)
		{
			try
			{
				CConfig cConfig = new CConfig();
				m_client.CurrentDirectory = m_client.CurrentDirectory.FindSubdirectory("reestrs".ToLower());
				this.SendFileResuming(m_client, new RBServer.Debug_classes.FileInfo("C:\\Project\\Project\\RBServer\\RBServer\\bin\\Release\\teremok_reestr_2013_2_21.csv"));
			}
			catch (Exception exception)
			{
				this.Log(string.Concat("Ошибка E0003", exception), 0);
			}
		}

		private void ExecuteInboxTask(RBServer.Debug_classes.FileInfo file_info, SqlConnection conn)
		{
			try
			{
				if (!DebugPanel.IgnoredFile.IsIgnored(file_info))
				{
					CConfig cConfig = new CConfig();
					char[] charArray = "_".ToCharArray();
					string[] strArrays = file_info.Name.Substring(0, file_info.Name.Length - 4).Split(charArray);
					if ((int)strArrays.Length == 2)
					{
						Root.Teremok_name = SqlWorker.SelectTeremokNameFromDBSafe(conn, strArrays[0]);
						string str = strArrays[1].Substring(7, strArrays[1].Length - 11).Replace(".", "");
						string[] mFolderZ = new string[] { cConfig.m_folder_Z, "\\", strArrays[0], "\\", str };
						string str1 = string.Concat(mFolderZ);
						if (System.IO.Directory.Exists(str1))
						{
							(new RBServer.Debug_classes.DirectoryInfo(str1)).GetFiles().ToList<RBServer.Debug_classes.FileInfo>().ForEach((RBServer.Debug_classes.FileInfo a) => a.Delete());
							if (cConfig.m_dep == "msk")
							{
								try
								{
									string[] mFolderZ1 = new string[] { cConfig.m_folder_Z, "\\", strArrays[0], "\\", str };
									string str2 = string.Concat(mFolderZ1);
									if (System.IO.Directory.Exists(str1))
									{
										(new RBServer.Debug_classes.DirectoryInfo(str1)).GetFiles().ToList<RBServer.Debug_classes.FileInfo>().ForEach((RBServer.Debug_classes.FileInfo a) => a.Delete());
									}
									else
									{
										RBServer.Debug_classes.Directory.CreateDirectory(str2);
									}
									if (this.DoExtract(file_info.FullName, str2))
									{
										this.ParseZReportDX(strArrays[0], str, conn);
										this.ParseZReport(strArrays[0], str, conn);
										if (!RBServer.Debug_classes.Directory.Exists(string.Concat(cConfig.m_folder_Z, "\\arch\\")))
										{
											RBServer.Debug_classes.Directory.CreateDirectory(string.Concat(cConfig.m_folder_Z, "\\arch\\"));
										}
										if (RBServer.Debug_classes.File.Exists(string.Concat(cConfig.m_folder_Z, "\\arch\\", file_info.Name)))
										{
											RBServer.Debug_classes.File.Delete(string.Concat(cConfig.m_folder_Z, "\\arch\\", file_info.Name));
										}
										RBServer.Debug_classes.File.Move(file_info.FullName, string.Concat(cConfig.m_folder_Z, "\\arch\\", file_info.Name));
									}
									else
									{
										return;
									}
								}
								catch (Exception exception)
								{
									throw new CException("Ошибка в InboxTask", exception);
								}
							}
							if (cConfig.m_dep == "spb")
							{
								string fullName = file_info.FullName;
								string[] strArrays1 = new string[] { cConfig.m_folder_Z, "\\", strArrays[0], "\\", str };
								if (this.DoExtract(fullName, string.Concat(strArrays1)))
								{
									this.ParseZReportSpb(strArrays[0], str, conn);
									if (!RBServer.Debug_classes.Directory.Exists(string.Concat(cConfig.m_folder_Z, "\\_arch\\", strArrays[0])))
									{
										RBServer.Debug_classes.Directory.CreateDirectory(string.Concat(cConfig.m_folder_Z, "\\_arch\\", strArrays[0]));
									}
									string[] mFolderZ2 = new string[] { cConfig.m_folder_Z, "\\_arch\\", strArrays[0], "\\", file_info.Name };
									if (RBServer.Debug_classes.File.Exists(string.Concat(mFolderZ2)))
									{
										string[] strArrays2 = new string[] { cConfig.m_folder_Z, "\\_arch\\", strArrays[0], "\\", file_info.Name };
										RBServer.Debug_classes.File.Delete(string.Concat(strArrays2));
									}
									string fullName1 = file_info.FullName;
									string[] mFolderZ3 = new string[] { cConfig.m_folder_Z, "\\_arch\\", strArrays[0], "\\", file_info.Name };
									RBServer.Debug_classes.File.Move(fullName1, string.Concat(mFolderZ3));
									if (RBServer.Debug_classes.File.Exists(file_info.FullName))
									{
										RBServer.Debug_classes.File.Delete(file_info.FullName);
									}
									this.Log(string.Concat("Удаляю файл ", file_info.FullName), 0);
								}
								else
								{
									return;
								}
							}
						}
						else
						{
							RBServer.Debug_classes.Directory.CreateDirectory(str1);
							return;
						}
					}
					else
					{
						string str3 = strArrays[0];
						Root.Teremok_name = SqlWorker.SelectTeremokNameFromDBSafe(conn, str3);
						string str4 = strArrays[1];
						int num = Convert.ToInt32(strArrays[5]);
						DateTime dateTime = new DateTime(Convert.ToInt32(strArrays[2]), Convert.ToInt32(strArrays[3]), Convert.ToInt32(strArrays[4]));
						if (str4 == "1" || str4 == "2" || str4 == "15" || str4 == "16" || str4 == "29")
						{
							if (RBServer.Debug_classes.File.Exists(string.Concat(cConfig.m_folder_1c_order, "\\\\", file_info.Name)))
							{
								RBServer.Debug_classes.File.Delete(string.Concat(cConfig.m_folder_1c_order, "\\", file_info.Name));
							}
							RBServer.Debug_classes.File.Copy(file_info.FullName, string.Concat(cConfig.m_folder_ftp, "\\\\arh\\", file_info.Name), true);
							this.Log(string.Concat("Попытка отправить файл в цех: ", file_info.Name), 2);
							if (RBServer.Debug_classes.File.Move(file_info.FullName, string.Concat(cConfig.m_folder_1c_order, "\\", file_info.Name)))
							{
								string str5 = "";
								int num1 = 0;
								int num2 = 0;
								RBServer.Debug_classes.FileInfo fileInfo = new RBServer.Debug_classes.FileInfo(string.Concat(cConfig.m_folder_1c_order, "\\", file_info.Name));
								string str6 = strArrays[1];
								string str7 = str6;
								if (str6 != null)
								{
									if (str7 == "1")
									{
										num1 = this.ImportOrder(num, fileInfo, str3, dateTime, 1, conn);
										num2 = this.return_doc_status(num1, conn);
										if (num2 == 2)
										{
											str5 = string.Concat("Отправлен заказ ", this.GetTeremokName(str3, conn));
											if (this.date_send.Hour >= Convert.ToInt32(cConfig.m_zakaz_time))
											{
												this.BanSendMail(conn, str3, num1, strArrays[1], "");
											}
										}
										if (num2 == 5)
										{
											str5 = string.Concat("Отправлена коррекция заказа ", this.GetTeremokName(str3, conn));
										}
									}
									else if (str7 == "29")
									{
										num1 = this.ImportOrder(num, fileInfo, str3, dateTime, 29, conn);
										num2 = this.return_doc_status(num1, conn);
										if (num2 == 2)
										{
											str5 = string.Concat("Отправлен заказ-кондитерка", this.GetTeremokName(str3, conn));
											if (this.date_send.Hour >= Convert.ToInt32(cConfig.m_zakaz_time))
											{
												this.BanSendMail(conn, str3, num1, strArrays[1], "");
											}
										}
										if (num2 == 5)
										{
											str5 = string.Concat("Отправлена коррекция заказа ", this.GetTeremokName(str3, conn));
										}
									}
									else if (str7 == "2")
									{
										num1 = this.ImportOrder(num, fileInfo, str3, dateTime, 2, conn);
									}
									else if (str7 == "15")
									{
										num1 = this.ImportOrder(num, fileInfo, str3, dateTime, 15, conn);
										num2 = this.return_doc_status(num1, conn);
										if (num2 == 2)
										{
											str5 = string.Concat("Отправлена входящая накаладная ", this.GetTeremokName(str3, conn));
											if (this.date_send.Hour >= Convert.ToInt32(cConfig.m_invoice_time))
											{
												if (this.return_doc_count(conn, strArrays[1], dateTime, this.GetTeremokID(str3, conn)) == 1)
												{
													this.BanSendMail(conn, str3, num1, strArrays[1], "");
												}
												else if (this.date_send.Hour >= Convert.ToInt32(cConfig.m_invoice_time2))
												{
													this.BanSendMail(conn, str3, num1, strArrays[1], string.Concat("ALARM Нарушен срок отправки довоза ", this.GetTeremokName(str3, conn)));
												}
											}
										}
										if (num2 == 5)
										{
											str5 = string.Concat("Отправлена коррекция входящей накаладной ", this.GetTeremokName(str3, conn));
										}
									}
									else if (str7 == "16")
									{
										num1 = this.ImportOrder(num, fileInfo, str3, dateTime, 16, conn);
										num2 = this.return_doc_status(num1, conn);
										if (num2 == 2)
										{
											str5 = string.Concat("Отправлена Возвратная накладная ", this.GetTeremokName(str3, conn));
											if (this.date_send.Hour >= Convert.ToInt32(cConfig.m_return_time))
											{
												this.BanSendMail(conn, str3, num1, strArrays[1], "");
											}
										}
										if (num2 == 5)
										{
											str5 = string.Concat("Отправлена коррекция Возвратной накладной ", this.GetTeremokName(str3, conn));
										}
									}
									else if (str7 != "7")
									{
									}
								}
								if (str5 != "")
								{
									string str8 = "";
									if (num1 != 0)
									{
										object[] dateCreate = new object[] { "Дата создания:", this.date_create, " \n\nДата подготовки к отправке:", this.date_send, "\n\n", this.OrderDetailForMessage(num1, conn) };
										str8 = string.Concat(dateCreate);
									}
                                    if (_config.enable_email_send_flag)
                                    {
                                        this.SendMail(cConfig.email_rbs_mess_inbox, str5, str8, true);
                                    }
								}
							}
							else
							{
								return;
							}
						}
						else
						{
							if (RBServer.Debug_classes.File.Exists(string.Concat(cConfig.m_folder_1c_in, "\\", file_info.Name)))
							{
								RBServer.Debug_classes.File.Delete(string.Concat(cConfig.m_folder_1c_in, "\\", file_info.Name));
							}
							if (this.MoveFile(file_info.FullName, string.Concat(cConfig.m_folder_1c_in, "\\", file_info.Name)))
							{
								RBServer.Debug_classes.FileInfo fileInfo1 = new RBServer.Debug_classes.FileInfo(string.Concat(cConfig.m_folder_1c_in, "\\", file_info.Name));
								string str9 = strArrays[1];
								string str10 = str9;
								if (str9 != null)
								{
									switch (str10)
									{
										case "3":
										{
											this.ImportOrder(num, fileInfo1, str3, dateTime, 3, conn);
											break;
										}
										case "6":
										{
											string.Concat("Отправлено списание ", this.GetTeremokName(str3, conn));
											break;
										}
										case "9":
										{
											this.ImportOrder(num, fileInfo1, str3, dateTime, 9, conn);
											break;
										}
										case "10":
										{
											this.ImportOrder(num, fileInfo1, str3, dateTime, 10, conn);
											break;
										}
										case "13":
										{
											this.ImportOrder(num, fileInfo1, str3, dateTime, 13, conn);
											break;
										}
										case "14":
										{
											this.ImportOrder(num, fileInfo1, str3, dateTime, 14, conn);
											break;
										}
										case "19":
										{
											this.ImportOrder(num, fileInfo1, str3, dateTime, 19, conn);
											break;
										}
										case "21":
										{
											this.ImportOrder(num, fileInfo1, str3, dateTime, 21, conn);
											break;
										}
										case "23":
										{
											this.ImportOrder(num, fileInfo1, str3, dateTime, 23, conn);
											break;
										}
										case "27":
										{
											this.ImportOrder(num, fileInfo1, str3, dateTime, 27, conn);
											break;
										}
									}
								}
							}
							else
							{
								return;
							}
						}
						if (RBServer.Debug_classes.File.Exists(file_info.FullName))
						{
							RBServer.Debug_classes.File.Delete(file_info.FullName);
						}
						this.Log(string.Concat("Удаляю файл ", file_info.FullName), 0);
					}
				}
				else
				{
					DebugPanel.IgnoredFile.TryToMakeUnIgnored(file_info, 5);
				}
			}
			catch (Exception exception2)
			{
				Exception exception1 = exception2;
				CTransfer.ErrorFile = string.Concat("error: ", exception1.Message, " file: ", file_info.FullName);
				object[] errorFile = new object[] { "ExecuteInboxTask error: ", exception1, " ", CTransfer.ErrorFile };
				this.Log(string.Concat(errorFile), 2);
				throw exception1;
			}
		}

		private void ExecuteInboxTaskLOG(RBServer.Debug_classes.FileInfo file_info, SqlConnection conn)
		{
			try
			{
				if (!DebugPanel.IgnoredFile.IsIgnored(file_info))
				{
					this.Log("Вошли в ExecuteInboxTaskLOG", 0);
					CConfig cConfig = new CConfig();
					char[] charArray = "_".ToCharArray();
					string[] strArrays = file_info.Name.Substring(0, file_info.Name.Length - 4).Split(charArray);
					if ((int)strArrays.Length == 2)
					{
						this.Log("Вошли в елсе по з-отчетам", 0);
						string str = strArrays[1].Substring(7, strArrays[1].Length - 11);
						if (cConfig.m_dep == "msk")
						{
							this.Log("деп==мск", 0);
							this.Log("Екстрактим зип", 0);
							string fullName = file_info.FullName;
							string[] mFolderZ = new string[] { cConfig.m_folder_Z, "\\", strArrays[0], "\\", str };
							if (this.DoExtract(fullName, string.Concat(mFolderZ)))
							{
								this.Log("Парсим дх", 0);
								this.ParseZReportDX(strArrays[0], str, conn);
								this.Log("Парсим з", 0);
								this.ParseZReport(strArrays[0], str, conn);
								this.Log("перемещение файла в арсч", 0);
								if (!RBServer.Debug_classes.Directory.Exists(string.Concat(cConfig.m_folder_Z, "\\arch\\")))
								{
									RBServer.Debug_classes.Directory.CreateDirectory(string.Concat(cConfig.m_folder_Z, "\\arch\\"));
								}
								if (RBServer.Debug_classes.File.Exists(string.Concat(cConfig.m_folder_Z, "\\arch\\", file_info.Name)))
								{
									RBServer.Debug_classes.File.Delete(string.Concat(cConfig.m_folder_Z, "\\arch\\", file_info.Name));
								}
								RBServer.Debug_classes.File.Move(file_info.FullName, string.Concat(cConfig.m_folder_Z, "\\arch\\", file_info.Name));
							}
							else
							{
								return;
							}
						}
						if (cConfig.m_dep == "spb")
						{
							if (this.DoExtract(file_info.FullName, string.Concat(cConfig.m_folder_Z, "\\", strArrays[0], "\\")))
							{
								this.ParseZReport(strArrays[0], str, conn);
								if (!RBServer.Debug_classes.Directory.Exists(string.Concat(cConfig.m_folder_Z, "\\_arch\\", strArrays[0])))
								{
									RBServer.Debug_classes.Directory.CreateDirectory(string.Concat(cConfig.m_folder_Z, "\\_arch\\", strArrays[0]));
								}
								string[] mFolderZ1 = new string[] { cConfig.m_folder_Z, "\\_arch\\", strArrays[0], "\\", file_info.Name };
								if (RBServer.Debug_classes.File.Exists(string.Concat(mFolderZ1)))
								{
									string[] strArrays1 = new string[] { cConfig.m_folder_Z, "\\_arch\\", strArrays[0], "\\", file_info.Name };
									RBServer.Debug_classes.File.Delete(string.Concat(strArrays1));
								}
								string fullName1 = file_info.FullName;
								string[] mFolderZ2 = new string[] { cConfig.m_folder_Z, "\\_arch\\", strArrays[0], "\\", file_info.Name };
								RBServer.Debug_classes.File.Move(fullName1, string.Concat(mFolderZ2));
								if (RBServer.Debug_classes.File.Exists(file_info.FullName))
								{
									RBServer.Debug_classes.File.Delete(file_info.FullName);
								}
								this.Log(string.Concat("Удаляю файл ", file_info.FullName), 0);
							}
							else
							{
								return;
							}
						}
					}
					else
					{
						string str1 = strArrays[0];
						string str2 = strArrays[1];
						int num = Convert.ToInt32(strArrays[5]);
						DateTime dateTime = new DateTime(Convert.ToInt32(strArrays[2]), Convert.ToInt32(strArrays[3]), Convert.ToInt32(strArrays[4]));
						if (str2 == "1" || str2 == "2" || str2 == "15" || str2 == "16")
						{
							if (RBServer.Debug_classes.File.Exists(string.Concat(cConfig.m_folder_1c_order, "\\\\", file_info.Name)))
							{
								RBServer.Debug_classes.File.Delete(string.Concat(cConfig.m_folder_1c_order, "\\", file_info.Name));
							}
							RBServer.Debug_classes.File.Copy(file_info.FullName, string.Concat(cConfig.m_folder_ftp, "\\\\arh\\", file_info.Name), true);
							this.Log(string.Concat("Попытка отправить файл в цех: ", file_info.Name), 2);
							if (this.MoveFile(file_info.FullName, string.Concat(cConfig.m_folder_1c_order, "\\", file_info.Name)))
							{
								string str3 = "";
								int num1 = 0;
								int num2 = 0;
								RBServer.Debug_classes.FileInfo fileInfo = new RBServer.Debug_classes.FileInfo(string.Concat(cConfig.m_folder_1c_order, "\\", file_info.Name));
								string str4 = strArrays[1];
								string str5 = str4;
								if (str4 != null)
								{
									if (str5 == "1")
									{
										num1 = this.ImportOrder(num, fileInfo, str1, dateTime, 1, conn);
										num2 = this.return_doc_status(num1, conn);
										if (num2 == 2)
										{
											str3 = string.Concat("Отправлен заказ ", this.GetTeremokName(str1, conn));
											if (this.date_send.Hour >= Convert.ToInt32(cConfig.m_zakaz_time))
											{
												this.BanSendMail(conn, str1, num1, strArrays[1], "");
											}
										}
										if (num2 == 5)
										{
											str3 = string.Concat("Отправлена коррекция заказа ", this.GetTeremokName(str1, conn));
										}
									}
									else if (str5 == "2")
									{
										num1 = this.ImportOrder(num, fileInfo, str1, dateTime, 2, conn);
									}
									else if (str5 == "15")
									{
										num1 = this.ImportOrder(num, fileInfo, str1, dateTime, 15, conn);
										num2 = this.return_doc_status(num1, conn);
										if (num2 == 2)
										{
											str3 = string.Concat("Отправлена входящая накаладная ", this.GetTeremokName(str1, conn));
											if (this.date_send.Hour >= Convert.ToInt32(cConfig.m_invoice_time))
											{
												if (this.return_doc_count(conn, strArrays[1], dateTime, this.GetTeremokID(str1, conn)) == 1)
												{
													this.BanSendMail(conn, str1, num1, strArrays[1], "");
												}
												else if (this.date_send.Hour >= Convert.ToInt32(cConfig.m_invoice_time2))
												{
													this.BanSendMail(conn, str1, num1, strArrays[1], string.Concat("ALARM Нарушен срок отправки довоза ", this.GetTeremokName(str1, conn)));
												}
											}
										}
										if (num2 == 5)
										{
											str3 = string.Concat("Отправлена коррекция входящей накаладной ", this.GetTeremokName(str1, conn));
										}
									}
									else if (str5 == "16")
									{
										num1 = this.ImportOrder(num, fileInfo, str1, dateTime, 16, conn);
										num2 = this.return_doc_status(num1, conn);
										if (num2 == 2)
										{
											str3 = string.Concat("Отправлена Возвратная накладная ", this.GetTeremokName(str1, conn));
											if (this.date_send.Hour >= Convert.ToInt32(cConfig.m_return_time))
											{
												this.BanSendMail(conn, str1, num1, strArrays[1], "");
											}
										}
										if (num2 == 5)
										{
											str3 = string.Concat("Отправлена коррекция Возвратной накладной ", this.GetTeremokName(str1, conn));
										}
									}
									else if (str5 != "7")
									{
									}
								}
								if (str3 != "")
								{
									string str6 = "";
									if (num1 != 0)
									{
										object[] dateCreate = new object[] { "Дата создания:", this.date_create, " \n\nДата подготовки к отправке:", this.date_send, "\n\n", this.OrderDetailForMessage(num1, conn) };
										str6 = string.Concat(dateCreate);
									}
                                    if (_config.enable_email_send_flag)
                                    {
                                        this.SendMail(cConfig.email_rbs_mess_inbox, str3, str6, true);
                                    }
								}
							}
							else
							{
								return;
							}
						}
						else
						{
							if (RBServer.Debug_classes.File.Exists(string.Concat(cConfig.m_folder_1c_in, "\\", file_info.Name)))
							{
								RBServer.Debug_classes.File.Delete(string.Concat(cConfig.m_folder_1c_in, "\\", file_info.Name));
							}
							if (this.MoveFile(file_info.FullName, string.Concat(cConfig.m_folder_1c_in, "\\", file_info.Name)))
							{
								RBServer.Debug_classes.FileInfo fileInfo1 = new RBServer.Debug_classes.FileInfo(string.Concat(cConfig.m_folder_1c_in, "\\", file_info.Name));
								string str7 = strArrays[1];
								string str8 = str7;
								if (str7 != null)
								{
									switch (str8)
									{
										case "3":
										{
											this.ImportOrder(num, fileInfo1, str1, dateTime, 3, conn);
											break;
										}
										case "6":
										{
											string.Concat("Отправлено списание ", this.GetTeremokName(str1, conn));
											break;
										}
										case "9":
										{
											this.ImportOrder(num, fileInfo1, str1, dateTime, 9, conn);
											break;
										}
										case "10":
										{
											this.ImportOrder(num, fileInfo1, str1, dateTime, 10, conn);
											break;
										}
										case "13":
										{
											this.ImportOrder(num, fileInfo1, str1, dateTime, 13, conn);
											break;
										}
										case "14":
										{
											this.ImportOrder(num, fileInfo1, str1, dateTime, 14, conn);
											break;
										}
										case "19":
										{
											this.ImportOrder(num, fileInfo1, str1, dateTime, 19, conn);
											break;
										}
										case "21":
										{
											this.ImportOrder(num, fileInfo1, str1, dateTime, 21, conn);
											break;
										}
										case "23":
										{
											this.ImportOrder(num, fileInfo1, str1, dateTime, 23, conn);
											break;
										}
										case "27":
										{
											this.ImportOrder(num, fileInfo1, str1, dateTime, 27, conn);
											break;
										}
									}
								}
							}
							else
							{
								return;
							}
						}
						if (RBServer.Debug_classes.File.Exists(file_info.FullName))
						{
							RBServer.Debug_classes.File.Delete(file_info.FullName);
						}
						this.Log(string.Concat("Удаляю файл ", file_info.FullName), 0);
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.Log(string.Concat("ExecuteInboxTask error: ", exception), 2);
				CTransfer.ErrorFile = string.Concat("error: ", exception.Message, " file: ", file_info.FullName);
				throw exception;
			}
		}

		private bool ExecuteOutboxTask(RBServer.Debug_classes.FileInfo file_info, SqlConnection conn)
		{
			bool flag;
			try
			{
				CConfig cConfig = new CConfig();
				bool flag1 = false;
				char[] charArray = "_".ToCharArray();
				string[] strArrays = file_info.Name.Split(charArray);
				string str = strArrays[0];
				string str1 = strArrays[1];
				if (RBServer.Debug_classes.Directory.Exists(string.Concat(cConfig.m_folder_ftp, "\\", str)))
				{
					if (file_info.Name.Substring(file_info.Name.Length - 3, 3) == "xml")
					{
						string str2 = strArrays[1];
						string str3 = str2;
						if (str2 != null)
						{
							switch (str3)
							{
								case "5-0":
								case "4-0":
								case "4-1":
								case "4-2":
								case "4-3":
								case "4-6":
								case "4-9":
								case "4-10":
								case "4-13":
								case "4-14":
								case "4-15":
								case "4-16":
								case "4-19":
								case "4-20":
								case "4-21":
								case "4-23":
								case "4-25":
								case "4-27":
								case "4-29":
								case "15":
								case "16":
								case "19":
								{
									this.ImportNome(file_info, conn);
									flag1 = true;
									break;
								}
								case "4-5":
								case "4-11":
								case "5":
								{
									RBServer.Debug_classes.File.Delete(file_info.FullName);
									break;
								}
								case "7":
								case "31":
								case "menu":
								{
									flag1 = true;
									break;
								}
							}
						}
					}
					if (file_info.Name.Substring(file_info.Name.Length - 3, 3) == "zip")
					{
						string str4 = strArrays[1];
						if (str4 != null && str4 == "menu")
						{
							this.ImportMenu(file_info, conn);
							flag1 = true;
						}
					}
					if (flag1)
					{
						string[] mFolderFtp = new string[] { cConfig.m_folder_ftp, "\\", str, "\\out\\", file_info.Name };
						if (RBServer.Debug_classes.File.Exists(string.Concat(mFolderFtp)))
						{
							string[] mFolderFtp1 = new string[] { cConfig.m_folder_ftp, "\\", str, "\\out\\", file_info.Name };
							RBServer.Debug_classes.File.Delete(string.Concat(mFolderFtp1));
						}
						string fullName = file_info.FullName;
						string[] strArrays1 = new string[] { cConfig.m_folder_ftp, "\\", str, "\\out\\", file_info.Name };
						RBServer.Debug_classes.File.Move(fullName, string.Concat(strArrays1));
						if (RBServer.Debug_classes.File.Exists(file_info.FullName))
						{
							RBServer.Debug_classes.File.Delete(file_info.FullName);
						}
					}
					return true;
				}
				else
				{
					flag = false;
				}
			}
			catch (IndexOutOfRangeException indexOutOfRangeException)
			{
				flag = false;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				CTransfer.ErrorFile = string.Concat("error ", exception.Message, " file ", file_info.FullName);
                Log("ExecuteOutboxTask " + CTransfer.ErrorFile);
				throw exception;
			}
			return flag;
		}

		private void Filedel(string teremok_folder, string kks, SqlConnection conn)
		{
			CConfig cConfig = new CConfig();
			CZReport cZReport = new CZReport();
			try
			{
				string[] mFolderZ = new string[] { cConfig.m_folder_Z, "\\", teremok_folder, "\\", kks };
				foreach (RBServer.Debug_classes.FileInfo file in (new RBServer.Debug_classes.DirectoryInfo(string.Concat(mFolderZ))).GetFiles())
				{
					if (file.CreationTime.AddDays(1) > DateTime.Now)
					{
						continue;
					}
					file.Delete();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.SendMail(cConfig.email_rbs_err, "Error RBSERVER Filedel", exception.Message, false);
				this.Log(exception.Message, 0);
			}
		}

		private void FtpArm_Dirs_Operate(SqlConnection _conn)
		{
			foreach (RBServer.Debug_classes.DirectoryInfo directory in (new RBServer.Debug_classes.DirectoryInfo(this._config.m_folder_ftp)).GetDirectories())
			{
				RBServer.Debug_classes.DirectoryInfo directoryInfo = new RBServer.Debug_classes.DirectoryInfo(string.Concat(directory.FullName, "\\in\\"));
				foreach (RBServer.Debug_classes.FileInfo file in directoryInfo.GetFiles())
				{
					this.Log(string.Concat(directory.FullName, "\\in\\", file.Name), 2);
					if (file.Name.Equals("uptime.log"))
					{
						this.LoadUptime(directory.Name, _conn);
					}
					else if (file.Name.Equals("open.log"))
					{
						this.LoadOpen(directory.Name, _conn);
					}
					else if (!file.Name.Equals("close.log"))
					{
						if (file.Name.Equals("error.log") || file.Name.Equals("dataout.xml"))
						{
							continue;
						}
						if (file.Name.StartsWith("kkm"))
						{
							if (file.Name.EndsWith(".tmp"))
							{
								continue;
							}
							if (RBServer.Debug_classes.File.Exists(string.Concat(this._config.m_folder_log_kkm, "\\", file)))
							{
								RBServer.Debug_classes.File.Delete(string.Concat(this._config.m_folder_log_kkm, "\\", file));
							}
							RBServer.Debug_classes.File.Move(file.FullName, string.Concat(this._config.m_folder_log_kkm, "\\", file.Name));
						}
						else if (file.Name.EndsWith(".zipus"))
						{
							string[] mFolderLog = new string[] { this._config.m_folder_log, "\\", directory.Name, "_", file.Name.Substring(0, file.Name.Length - 2) };
							if (RBServer.Debug_classes.File.Exists(string.Concat(mFolderLog)))
							{
								string[] strArrays = new string[] { this._config.m_folder_log, "\\", directory.Name, "_", file.Name.Substring(0, file.Name.Length - 2) };
								RBServer.Debug_classes.File.Delete(string.Concat(strArrays));
							}
							string fullName = file.FullName;
							string[] mFolderLog1 = new string[] { this._config.m_folder_log, "\\", directory.Name, "_", file.Name.Substring(0, file.Name.Length - 2) };
							RBServer.Debug_classes.File.Move(fullName, string.Concat(mFolderLog1));
							string[] name = new string[] { directory.Name, " - получен файл лога - ", directory.Name, "_", file.Name.Substring(0, file.Name.Length - 2) };
							this.Log(string.Concat(name), 0);
						}
						else if (!file.Name.EndsWith(".zips"))
						{
							if (!file.Name.EndsWith(".tmp"))
							{
								try
								{
									if (!DebugPanel.IgnoredFile.IsIgnored(file))
									{
										this.Log(string.Concat("Приступаю к разбору файла: ", directory.FullName, "\\in\\", file.Name), 2);
										this.ExecuteInboxTask(file, _conn);
										this.Log(string.Concat(directory.Name, " ---- ", file.Name), 0);
									}
									else
									{
										this.Log(string.Concat("Файл игнорируется: ", directory.FullName, "\\in\\", file.Name), 2);
										DebugPanel.IgnoredFile.TryToMakeUnIgnored(file, 5);
										continue;
									}
								}
								catch (Exception exception)
								{
									DebugPanel.OnErrorOccured(string.Concat("Ошибка с файлом ", file.FullName));
									DebugPanel.IgnoredFile.MakeFileIgnored(file);
								}
							}
						}
						else
						{
							string[] strArrays1 = new string[] { this._config.m_folder_log, "\\", directory.Name, "_", file.Name.Substring(0, file.Name.Length - 2) };
							if (RBServer.Debug_classes.File.Exists(string.Concat(strArrays1)))
							{
								string[] mFolderLog2 = new string[] { this._config.m_folder_log, "\\", directory.Name, "_", file.Name.Substring(0, file.Name.Length - 2) };
								RBServer.Debug_classes.File.Delete(string.Concat(mFolderLog2));
							}
							string str = file.FullName;
							string[] strArrays2 = new string[] { this._config.m_folder_log, "\\", directory.Name, "_", file.Name.Substring(0, file.Name.Length - 2) };
							RBServer.Debug_classes.File.Move(str, string.Concat(strArrays2));
							string[] name1 = new string[] { directory.Name, " - получен файл лога - ", directory.Name, "_", file.Name.Substring(0, file.Name.Length - 2) };
							this.Log(string.Concat(name1), 0);
						}
					}
					else
					{
						this.LoadClose(directory.Name, _conn);
					}
				}
			}
		}

		private void FTPServer()
		{
			CConfig cConfig = new CConfig();
			FtpSession ftpSession = new FtpSession();
			try
			{
				ftpSession.Server = cConfig.m_ftp_server;
				ftpSession.Port = cConfig.m_ftp_port;
				ftpSession.Connect(cConfig.m_ftp_login.ToString(), cConfig.m_ftp_pass.ToString());
				this.ExchangeFTP(ftpSession);
			}
			catch (FtpException ftpException)
			{
				this.Log(string.Concat("Ошибка E0001", ftpException), 0);
			}
			catch (Exception exception)
			{
				this.Log(string.Concat("Ошибка E0002", exception), 0);
			}
		}

		private void FTPServerSV()
		{
			CConfig cConfig = new CConfig();
			FtpSession ftpSession = new FtpSession();
			try
			{
				ftpSession.Server = cConfig.m_ftp_server_sb;
				ftpSession.Port = cConfig.m_ftp_port_sb;
				ftpSession.Connect(cConfig.m_ftp_login_sb.ToString(), cConfig.m_ftp_pass_sb.ToString());
				this.ExchangeFTPSB(ftpSession);
			}
			catch (FtpException ftpException)
			{
				this.Log(string.Concat("Ошибка E0001", ftpException), 0);
			}
			catch (Exception exception)
			{
				this.Log(string.Concat("Ошибка E0002", exception), 0);
			}
		}

		private void FTPSVB()
		{
			WebClient webClient = null;
			string ftpSvyaznoyAddress = this._config.ftp_svyaznoy_address;
			object[] year = new object[] { "teremok_reestr_", null, null, null, null, null, null };
			year[1] = DateTime.Now.Year;
			year[2] = "_";
			year[3] = DateTime.Now.Month;
			year[4] = "_";
			year[5] = DateTime.Now.Day;
			year[6] = ".csv";
			string str = string.Concat(year);
			string str1 = null;
			if (RBServer.Debug_classes.File.Exists(str))
			{
				try
				{
					try
					{
						webClient = new WebClient()
						{
							Encoding = Encoding.UTF8
						};
						str1 = string.Concat(ftpSvyaznoyAddress, str);
						webClient.Credentials = new NetworkCredential(this._config.ftp_svyaznoy_username, this._config.ftp_svyaznoy_password);
						webClient.UploadFile(str1, str);
						RBServer.Debug_classes.File.Move(str, string.Concat(str, "_sended"));
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						this.SendMail(this._config.email_rbs_err_svyaznoy_ftp, "Сбой выгрузки данных на FTP", exception.Message, false);
					}
				}
				finally
				{
					webClient.Dispose();
				}
			}
		}

		private void GetFileResuming(FtpSession m_client, string localPath, string name)
		{
			try
			{
				FtpFile ftpFile = m_client.CurrentDirectory.FindFile(name);
				if (ftpFile != null)
				{
					long length = (long)0;
					string str = string.Concat(localPath, ".tmp");
					bool flag = true;
					RBServer.Debug_classes.FileInfo fileInfo = new RBServer.Debug_classes.FileInfo(localPath);
					if (fileInfo.Exists)
					{
						if (fileInfo.Length >= ftpFile.Size)
						{
							flag = false;
						}
						else
						{
							fileInfo.Delete();
						}
					}
					if (flag)
					{
						RBServer.Debug_classes.FileInfo fileInfo1 = new RBServer.Debug_classes.FileInfo(str);
						if (fileInfo1.Exists)
						{
							length = fileInfo1.Length;
						}
						if (length < ftpFile.Size)
						{
							m_client.CurrentDirectory.GetFile(str, name, length);
						}
						fileInfo1 = new RBServer.Debug_classes.FileInfo(str);
						if (fileInfo1.Exists)
						{
							fileInfo1.MoveTo(localPath);
						}
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.Log(string.Concat("Ошибка при транспорте файла E0004", exception), 0);
				throw exception;
			}
		}

		private string GetTeremok1CName(string teremok_1C, SqlConnection conn)
		{
			string str;
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn,
					CommandText = string.Concat("SELECT teremok_1C FROM t_Teremok WHERE teremok_1C='", teremok_1C, "'")
				};
				str = sqlCommand.ExecuteScalar().ToString();
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return str;
		}

		private string GetTeremokID(string teremok_1C, SqlConnection conn)
		{
			string str;
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn,
					CommandText = string.Concat("SELECT teremok_id FROM t_Teremok WHERE teremok_1C='", teremok_1C, "'")
				};
				str = sqlCommand.ExecuteScalar().ToString();
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return str;
		}

		private string GetTeremokName(string teremok_1C, SqlConnection conn)
		{
			string str;
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn,
					CommandText = string.Concat("SELECT teremok_name FROM t_Teremok WHERE teremok_1C='", teremok_1C, "'")
				};
				str = sqlCommand.ExecuteScalar().ToString();
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return str;
		}

		private void Import(SqlConnection _conn, string _terminalNum, string _klishe)
		{
			string str = "";
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = _conn
				};
				str = string.Concat("SELECT count(*) FROM t_Klishe WHERE klishe = ", _klishe, " AND pinNumber = ", _terminalNum);
				sqlCommand.CommandText = str;
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0)
				{
					sqlCommand = new SqlCommand()
					{
						Connection = _conn
					};
					str = "INSERT INTO t_Klishe (klishe, pinNumber) VALUES (@klishe, @pinNumber)";
					sqlCommand.Parameters.AddWithValue("@klishe", _klishe);
					sqlCommand.Parameters.AddWithValue("@pinNumber", _terminalNum);
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				CTransfer.ErrorFile = string.Concat("error: ", _conn.ToString(), " command: ", str);
				throw exception;
			}
		}

		private void ImportCheckItemMiniPOS(string[] s, string KKM, SqlConnection conn)
		{
			try
			{
				SqlCommand sqlCommand = new SqlCommand("[sp_CheckItemMiniPOS]", conn)
				{
					CommandType = CommandType.StoredProcedure
				};
				SqlParameter sqlParameter = sqlCommand.Parameters.Add("@cim_tran", SqlDbType.Int);
				SqlParameter shortDateString = sqlCommand.Parameters.Add("@cim_date", SqlDbType.DateTime);
				SqlParameter sqlParameter1 = sqlCommand.Parameters.Add("@cim_datetime", SqlDbType.DateTime);
				SqlParameter sqlParameter2 = sqlCommand.Parameters.Add("@cim_tran_type", SqlDbType.Int);
				SqlParameter kKM = sqlCommand.Parameters.Add("@cim_kkm_num", SqlDbType.NChar, 13);
				SqlParameter sqlParameter3 = sqlCommand.Parameters.Add("@cim_check_num", SqlDbType.Int);
				SqlParameter sqlParameter4 = sqlCommand.Parameters.Add("@cim_employee_code", SqlDbType.Int);
				SqlParameter sqlParameter5 = sqlCommand.Parameters.Add("@cim_column_08", SqlDbType.NChar, 13);
				SqlParameter sqlParameter6 = sqlCommand.Parameters.Add("@cim_column_09", SqlDbType.Int);
				SqlParameter sqlParameter7 = sqlCommand.Parameters.Add("@cim_column_10", SqlDbType.Float);
				SqlParameter sqlParameter8 = sqlCommand.Parameters.Add("@cim_column_11", SqlDbType.Float);
				SqlParameter sqlParameter9 = sqlCommand.Parameters.Add("@cim_column_12", SqlDbType.Float);
				double num = 0;
				sqlParameter.Value = s[0];
				DateTime dateTime = Convert.ToDateTime(s[1]);
				if (dateTime.Year == 2016)
				{
					dateTime = dateTime.AddYears(-6);
				}
				shortDateString.Value = dateTime.ToShortDateString();
				sqlParameter1.Value = string.Concat(dateTime.ToShortDateString(), " ", s[2]);
				sqlParameter2.Value = s[3];
				kKM.Value = KKM;
				sqlParameter3.Value = s[5];
				sqlParameter4.Value = s[6];
				sqlParameter5.Value = s[7];
				sqlParameter6.Value = s[8];
				num = (s[9] != "" ? Convert.ToDouble(this.ParceAmount(s[9])) : 0);
				sqlParameter7.Value = num;
				num = (s[10] != "" ? Convert.ToDouble(this.ParceAmount(s[10])) : 0);
				sqlParameter8.Value = num;
				num = (s[11] != "" ? Convert.ToDouble(this.ParceAmount(s[11])) : 0);
				sqlParameter9.Value = num;
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private int ImportDoc(string code_teremok, int teremok_doc_id, int doc_type, DateTime dt, SqlConnection conn)
		{
			int num;
			try
			{
				SqlCommand sqlCommand = new SqlCommand("sp_Doc", conn)
				{
					CommandType = CommandType.StoredProcedure
				};
				SqlParameter value = sqlCommand.Parameters.Add("@doc_id", SqlDbType.Int);
				value.Direction = ParameterDirection.InputOutput;
				SqlParameter sqlParameter = sqlCommand.Parameters.Add("@doc_teremok_id", SqlDbType.Int);
				SqlParameter docType = sqlCommand.Parameters.Add("@doc_type_id", SqlDbType.Int);
				SqlParameter sqlParameter1 = sqlCommand.Parameters.Add("@doc_datetime", SqlDbType.DateTime);
				SqlParameter sqlParameter2 = sqlCommand.Parameters.Add("@doc_status_id", SqlDbType.Int);
				SqlParameter value1 = sqlCommand.Parameters.Add("@doc_desc", SqlDbType.NVarChar, 250);
				SqlParameter teremokDocId = sqlCommand.Parameters.Add("@doc_id_teremok", SqlDbType.Int);
				value.Value = DBNull.Value;
				sqlParameter.Value = Convert.ToInt32(this.GetTeremokID(code_teremok, conn));
				docType.Value = doc_type;
				sqlParameter1.Value = dt;
				sqlParameter2.Value = 2;
				value1.Value = DBNull.Value;
				teremokDocId.Value = teremok_doc_id;
				sqlCommand.ExecuteNonQuery();
				num = Convert.ToInt32(value.Value);
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return num;
		}

		private void ImportMenu(RBServer.Debug_classes.FileInfo fi, SqlConnection conn)
		{
			RBServer.Debug_classes.StreamReader streamReader = null;
			CConfig cConfig = new CConfig();
			try
			{
				try
				{
					if (!RBServer.Debug_classes.Directory.Exists(string.Concat(cConfig.m_app_folder, "\\Temp")))
					{
						RBServer.Debug_classes.Directory.CreateDirectory(string.Concat(cConfig.m_app_folder, "\\Temp"));
					}
					using (ZipFile zipFiles = ZipFile.Read(fi.FullName))
					{
						foreach (ZipEntry zipEntry in zipFiles)
						{
							using (RBServer.Debug_classes.FileStream fileStream = new RBServer.Debug_classes.FileStream(string.Concat(cConfig.m_app_folder, "\\Temp\\", zipEntry.FileName), FileMode.Create))
							{
								zipEntry.Extract(fileStream);
							}
						}
					}
					RBServer.Debug_classes.DirectoryInfo directoryInfo = new RBServer.Debug_classes.DirectoryInfo(string.Concat(cConfig.m_app_folder, "\\Temp"));
				Label0:
					foreach (RBServer.Debug_classes.FileInfo file in directoryInfo.GetFiles())
					{
						if (file.Name.Substring(0, 1) != "X")
						{
							continue;
						}
						streamReader = new RBServer.Debug_classes.StreamReader(file.FullName, Encoding.GetEncoding(1251));
						while (true)
						{
							string str = streamReader.ReadLine();
							string str1 = str;
							if (str == null)
							{
								goto Label0;
							}
							string str2 = str1.Substring(6, 13);
							this.ImportMenuToDB(str2, str1.Substring(19, 20), conn);
						}
					}
					if (streamReader != null)
					{
						streamReader.Close();
						streamReader.Dispose();
					}
					foreach (RBServer.Debug_classes.FileInfo fileInfo in directoryInfo.GetFiles())
					{
						RBServer.Debug_classes.File.Delete(fileInfo.FullName);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.SendMail(cConfig.email_rbs_err, "Error RBSERVER Import Menu", exception.Message, false);
					this.Log(exception.Message, 0);
				}
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
					streamReader.Dispose();
				}
			}
		}

		public void ImportMenuToDB(string nome_1c, string nome_name, SqlConnection conn)
		{
			string str;
			if (nome_1c == "")
			{
				return;
			}
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn,
					CommandText = string.Concat("SELECT count(*) FROM t_MenuNome WHERE mnome_1C='", nome_1c, "'")
				};
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) != 0)
				{
					str = "UPDATE t_MenuNome SET mnome_name = @nome_name WHERE mnome_1C=@nome_1c";
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@nome_name", nome_name);
					sqlCommand.Parameters.AddWithValue("@nome_1c", nome_1c);
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
				}
				else
				{
					str = "INSERT INTO t_MenuNome (mnome_1C, mnome_name) VALUES(@mnome_1C, @mnome_name)";
					sqlCommand.Parameters.AddWithValue("@mnome_1C", nome_1c);
					sqlCommand.Parameters.AddWithValue("@mnome_name", nome_name);
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private void ImportNome(RBServer.Debug_classes.FileInfo fi, SqlConnection conn)
		{
			XmlReader xmlReader = null;
			string str = "";
			string str1 = "";
			string str2 = "";
			string str3 = "";
			string str4 = "";
			try
			{
				try
				{
					xmlReader = XmlReader.Create(fi.FullName);
					foreach (XElement xElement in XDocument.Load(xmlReader).Root.Elements())
					{
						foreach (XAttribute xAttribute in xElement.Attributes())
						{
							string str5 = xAttribute.Name.ToString();
							string str6 = str5;
							if (str5 == null)
							{
								continue;
							}
							if (str6 == "c")
							{
								str = xAttribute.Value.ToString();
							}
							else if (str6 == "n")
							{
								str1 = xAttribute.Value.ToString();
							}
							else if (str6 == "be")
							{
								str3 = xAttribute.Value.ToString();
							}
							else if (str6 == "k")
							{
								str4 = xAttribute.Value.ToString();
							}
							else if (str6 == "ue")
							{
								str2 = xAttribute.Value.ToString();
							}
						}
						if (str3 != str2)
						{
							this.ImportNomeToDB(str, str1, str2, str3, str4, conn);
						}
						else
						{
							this.ImportNomeToDB(str, str1, str2, str3, "1", conn);
						}
					}
					xmlReader.Close();
				}
				catch (Exception exception)
				{
					throw exception;
				}
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
			}
		}

		public void ImportNomeToDB(string nome_id, string nome_name, string nome_ue, string nome_be, string nome_k, SqlConnection conn)
		{
			string str;
			if (nome_id == "")
			{
				return;
			}
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn,
					CommandText = string.Concat("SELECT count(*) FROM t_nomenclature WHERE nome_id='", nome_id, "'")
				};
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) != 0)
				{
					str = string.Concat("UPDATE t_nomenclature SET nome_name = @nome_name, nome_ed = @nome_ed, nome_bed = @nome_bed, nome_K = @nome_K WHERE nome_id='", nome_id, "'");
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@nome_name", nome_name);
					sqlCommand.Parameters.AddWithValue("@nome_ed", nome_ue);
					sqlCommand.Parameters.AddWithValue("@nome_bed", nome_be);
					sqlCommand.Parameters.AddWithValue("@nome_K", Convert.ToDouble(this.ParceAmount(nome_k)));
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
				}
				else
				{
					str = "INSERT INTO t_nomenclature (nome_id, nome_name, nome_ed, nome_bed, nome_K) VALUES(@nome_id, @nome_name, @nome_ed, @nome_bed, @nome_K)";
					sqlCommand.Parameters.AddWithValue("@nome_id", nome_id);
					sqlCommand.Parameters.AddWithValue("@nome_name", nome_name);
					sqlCommand.Parameters.AddWithValue("@nome_ed", nome_ue);
					sqlCommand.Parameters.AddWithValue("@nome_bed", nome_be);
					sqlCommand.Parameters.AddWithValue("@nome_K", Convert.ToDouble(this.ParceAmount(nome_k)));
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private int ImportOrder(int _teremok_doc_id, RBServer.Debug_classes.FileInfo fi, string code_teremok, DateTime dt, int doc_type, SqlConnection conn)
		{
			int num;
			XmlReader xmlReader = null;
			int num1 = 0;
			string str = "";
			string str1 = "";
			string str2 = "";
			if (num1 == 0)
			{
				num1 = this.ImportDoc(code_teremok, Convert.ToInt32(_teremok_doc_id), doc_type, dt, conn);
			}
			try
			{
				try
				{
					xmlReader = XmlReader.Create(fi.FullName);
					XDocument xDocument = XDocument.Load(xmlReader);
					if (this.date_incremention(fi.Name, 2))
					{
						this.SendMail(this._config.email_rbs_err, "Error RBSERVER xml Date", "Не соответствует дата документа текущей дате календаря", false, fi.FullName);
					}
					foreach (XElement xElement in xDocument.Root.Elements())
					{
						foreach (XAttribute xAttribute in xElement.Attributes())
						{
							string str3 = xAttribute.Name.ToString();
							string str4 = str3;
							if (str3 == null)
							{
								continue;
							}
							if (str4 == "time")
							{
								this.date_send = Convert.ToDateTime(xAttribute.Value.ToString());
							}
							else if (str4 == "timecreate")
							{
								this.date_create = Convert.ToDateTime(xAttribute.Value.ToString());
							}
							else if (str4 == "c")
							{
								str = xAttribute.Value.ToString();
							}
							else if (str4 == "q")
							{
								str1 = xAttribute.Value.ToString();
							}
							else if (str4 == "dv")
							{
								str2 = xAttribute.Value.ToString();
							}
						}
						if (str == "")
						{
							continue;
						}
						this.ImportOrderToDB(num1, str, str1, str2, conn);
					}
					xmlReader.Close();
					num = num1;
				}
				catch (Exception exception)
				{
					throw exception;
				}
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
			}
			return num;
		}

		private void ImportOrderToDB(int doc_id, string code_id, string quality, string dv, SqlConnection conn)
		{
			try
			{
				SqlCommand sqlCommand = new SqlCommand("sp_Order_Add", conn)
				{
					CommandType = CommandType.StoredProcedure
				};
				SqlParameter value = sqlCommand.Parameters.Add("@opd_id", SqlDbType.Int);
				value.Direction = ParameterDirection.InputOutput;
				SqlParameter num = sqlCommand.Parameters.Add("@opd_doc_id", SqlDbType.Int);
				SqlParameter sqlParameter = sqlCommand.Parameters.Add("@opd_order", SqlDbType.Float);
				SqlParameter num1 = sqlCommand.Parameters.Add("@opd_order2", SqlDbType.Float);
				SqlParameter codeId = sqlCommand.Parameters.Add("@opd_nome_id", SqlDbType.NVarChar, 50);
				value.Value = DBNull.Value;
				num.Value = Convert.ToInt32(doc_id);
				sqlParameter.Value = Convert.ToDouble(this.ParceAmount(quality));
				num1.Value = Convert.ToDouble(this.ParceAmount(dv));
				codeId.Value = code_id;
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private void ImportRSBToDB(SqlConnection _conn, string _terminalNum, string _cardType, string _cardNumber, string _typeTransaction, string _cardTotal, string _commission, string _compensation, string _currency, string _timeOperation, string _authorizationCode, string _referenceNumber, string _flare)
		{
			DateTime now = DateTime.Now;
			now = Convert.ToDateTime(_timeOperation);
			string str = "";
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = _conn
				};
				string[] strArrays = new string[] { "SELECT count(*) FROM t_CardRSB WHERE terminalNum = '", _terminalNum, "' AND cardType = '", _cardType, "' AND timeOperation = '", this.AttachZeroToDate(now.Year), ".", this.AttachZeroToDate(now.Month), ".", this.AttachZeroToDate(now.Day), " ", null, null, null, null, null, null, null, null, null, null };
				strArrays[11] = now.Hour.ToString();
				strArrays[12] = ":";
				strArrays[13] = now.Minute.ToString();
				strArrays[14] = ":";
				strArrays[15] = now.Second.ToString();
				strArrays[16] = "' AND authorizationCode = '";
				strArrays[17] = _authorizationCode;
				strArrays[18] = "' AND cardTotal = '";
				strArrays[19] = _cardTotal;
				strArrays[20] = "'";
				str = string.Concat(strArrays);
				sqlCommand.CommandText = str;
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0)
				{
					sqlCommand = new SqlCommand()
					{
						Connection = _conn
					};
					str = "INSERT INTO t_CardRSB (terminalNum, cardType, cardNumber, typeTransaction, cardTotal, commission, compensation, currency, timeOperation, authorizationCode, referenceNumber, flare) VALUES (@terminalNum, @cardType, @cardNumber, @typeTransaction, @cardTotal, @commission, @compensation, @currency, @timeOperation, @authorizationCode, @referenceNumber, @flare)";
					sqlCommand.Parameters.AddWithValue("@terminalNum", _terminalNum);
					sqlCommand.Parameters.AddWithValue("@cardType", _cardType);
					sqlCommand.Parameters.AddWithValue("@cardNumber", _cardNumber);
					sqlCommand.Parameters.AddWithValue("@typeTransaction", _typeTransaction);
					sqlCommand.Parameters.AddWithValue("@cardTotal", _cardTotal);
					sqlCommand.Parameters.AddWithValue("@commission", _commission);
					sqlCommand.Parameters.AddWithValue("@compensation", _compensation);
					sqlCommand.Parameters.AddWithValue("@currency", _currency);
					sqlCommand.Parameters.AddWithValue("@timeOperation", now);
					sqlCommand.Parameters.AddWithValue("@authorizationCode", _authorizationCode);
					sqlCommand.Parameters.AddWithValue("@referenceNumber", _referenceNumber);
					sqlCommand.Parameters.AddWithValue("@flare", _flare);
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				CTransfer.ErrorFile = string.Concat("error: ", _conn.ToString(), " command: ", str);
				throw exception;
			}
		}

		private void ImportZReport(RBServer.Debug_classes.FileInfo file_zreport, string teremok_folder, SqlConnection conn)
		{
			RBServer.Debug_classes.StreamReader streamReader = null;
			char[] charArray = "   ".ToCharArray();
			string str = null;
			string str1 = null;
			double num = 0;
			double num1 = 0;
			bool flag = false;
			int num2 = 0;
			DateTime now = DateTime.Now;
			CConfig cConfig = new CConfig();
			try
			{
				try
				{
					streamReader = new RBServer.Debug_classes.StreamReader(file_zreport.FullName, Encoding.GetEncoding(1251));
					while (true)
					{
						string str2 = streamReader.ReadLine();
						string str3 = str2;
						if (str2 == null)
						{
							break;
						}
						string[] strArrays = str3.Split(charArray);
						string str4 = strArrays[0];
						string str5 = str4;
						if (str4 != null)
						{
							switch (str5)
							{
								case "Касса":
								{
									str1 = strArrays[1];
									continue;
								}
								case "Смена":
								{
									string str6 = strArrays[1];
									continue;
								}
								case "Zномер":
								{
									str = strArrays[16];
									continue;
								}
								case "ИтогПродаж":
								{
									num = Convert.ToDouble(this.ParceAmount(strArrays[12]));
									continue;
								}
								case "Возврат/аннулирование":
								{
									num1 = Convert.ToDouble(this.ParceAmount(strArrays[1]));
									continue;
								}
								case "ВозвратОбеды":
								{
									DateTime dateTime = new DateTime(Convert.ToInt32(string.Concat("20", file_zreport.Name.Substring(1, 2))), Convert.ToInt32(file_zreport.Name.Substring(3, 2)), Convert.ToInt32(file_zreport.Name.Substring(5, 2)));
									num2 = this.ZReportAdd(str, dateTime, str1, file_zreport.Name, num, num1, this.GetTeremokID(teremok_folder, conn), conn);
									continue;
								}
								case "Чек":
								{
									flag = true;
									if (strArrays[2] == "")
									{
										now = Convert.ToDateTime(string.Concat(strArrays[6], " ", strArrays[7]));
										continue;
									}
									else
									{
										now = Convert.ToDateTime(string.Concat(strArrays[2], " ", strArrays[3]));
										continue;
									}
								}
								case "+":
								{
									flag = false;
									continue;
								}
								case "Оплата":
								{
									continue;
								}
							}
						}
						if (flag)
						{
							this.CheckItemParse(num2, str3, now, conn);
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.SendMail(cConfig.email_rbs_err, "Error RBSERVER ImportZReport", exception.Message, false);
					this.Log(exception.Message, 0);
				}
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
				}
			}
		}

		private bool IsZReportSent(string file_name, string teremok_folder, SqlConnection conn)
		{
			bool flag;
			int num = 0;
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				string str = "SELECT z_id FROM t_ZReport WHERE z_file=@z_file AND z_teremok_id=@z_teremok_id";
				sqlCommand.Parameters.AddWithValue("@z_file", file_name);
				sqlCommand.Parameters.AddWithValue("@z_teremok_id", this.GetTeremokID(teremok_folder, conn));
				sqlCommand.CommandText = str;
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
				if (num != 0)
				{
					str = "DELETE FROM t_CheckItem WHERE ch_zreport_id=@ch_zreport_id";
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@ch_zreport_id", num);
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
					str = "DELETE FROM t_ZReport WHERE z_id = @z_id";
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@z_id", num);
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
				}
				flag = false;
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return flag;
		}

        private SqlConnection connection = null;
		public void Job()
		{
           // Program._transfer.Log("--2", 0);
			this._config = new CConfig();
			SqlConnection sqlConnection = null;
			this.log_file_0 = "log\\RBService_0.log";
			this.log_file_1 = "log\\RBService_1.log";
			this.log_file_2 = "log\\RBService_2.log";
          //  Program._transfer.Log("--3", 0);
			DebugPanel.InMethod(0, 10);
			try
			{
				try
				{
					sqlConnection = new SqlConnection(this._config.m_connstring);
					sqlConnection.Open();
                    connection = sqlConnection;
                   
#if(DEBUG)
                   // this.FtpArm_Dirs_Operate(sqlConnection);
                    //MakeSpbReportSmena();
                    MakeMskReportSmena();
               //     MakeSpbReport();
                 //   MakeNoWebserviceSpbReport();
                   // //msk

                    //HelperClass hc = new HelperClass() { LogEvent = Log };
                    //var td = DateTime.Now;
                    //var date = new DateTime(td.Year, td.Month, td.Day - 1, 0, 0, 0);

                    //var items = hc.GetMskReportNew(date, _config.next_day_check_hour_zfile_report, _config.from_day_check_hour_zfile_report, 9);
                    //items.Sort((a, b) => a.teremok.teremok_name.CompareTo(b.teremok.teremok_name));
                    //items.ForEach(a => a.z_reports_web.Count());
                    //string message = PreparereportMessage(items);
                    //SendMailSync(_config.email_rbs_zfile_report, "Отчет получения z-отчетов за " + date.Date.ToShortDateString(), message, true, "");

                   //piter
                    //HelperClass hc = new HelperClass() { LogEvent = Log };
                    //var td = DateTime.Now;
                    //var date = new DateTime(td.Year, td.Month, td.Day - 1, 0, 0, 0);

                    //var items = hc.GetSpbReport(date, _config.next_day_check_hour_zfile_report, _config.from_day_check_hour_zfile_report, _config.kkm_start_work_hour_zfile_report);

                    //var items1 = items.Where(a => a.teremok != null && a.teremok.teremok_address != null
                    //    && a.teremok.teremok_address.CompareTo("5.6.6.1") >= 0).ToList();
                    //var items2 = items.Where(a => a.teremok == null).ToList();

                    //items1.Sort((a, b) => a.teremok.teremok_name.CompareTo(b.teremok.teremok_name));
                    //items2.Sort((a, b) => a.kassa.teremok_1c.CompareTo(b.kassa.teremok_1c));

                    //////Serializer.binary_write(new ArrayList() { items1 }, "1");
                    //////Serializer.binary_write(new ArrayList() { items2 }, "2");

                    ////var items1 = (List<kkm_terem_stat>)Serializer.binary_get("1")[0];
                    ////var items2 = (List<kkm_terem_stat>)Serializer.binary_get("2")[0];

                    //ReportHelper rh = new ReportHelper() { LogEvent = Log };
                    //string message = rh.ПодготовитьТаблицуДляОтчета(items1);
                    //message += "<br/><br/> " + rh.ПодготовитьТаблицуПоОтсутствующимРесторанам(items2);

                    //SendMailSync(_config.email_rbs_spb_zfile_report, "Отчет получения z-отчетов за " + date.Date.ToShortDateString(), message, true, "");


                    //make_Zfile_Report();
                    //operate_svyaznoy(sqlConnection);
                    //make_svyaznoy_reestr(sqlConnection);
#endif
                    make_Zfile_Report();
                  //  Program._transfer.Log("--4", 0);
                    this.Uptime(sqlConnection);
                 //   Program._transfer.Log("--5", 0);
					this.Rsb_Operation_perform(sqlConnection);
                 //   Program._transfer.Log("--6", 0);
					this.FtpArm_Dirs_Operate(sqlConnection);
                  //  Program._transfer.Log("--7", 0);
					this.Send_Files_to_Restaurants(sqlConnection);
                  //  Program._transfer.Log("--8", 0);
					this.KKM_shtrih_Load(sqlConnection);
                  //  Program._transfer.Log("--9", 0);
                    operate_svyaznoy(sqlConnection);
                 //   Program._transfer.Log("--10", 0);
                    
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.SendMail(this._config.email_rbs_err, "Error RBSERVER--", exception.Message, false);
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
                    connection = null;
				}
			}
		}

        private void MakeMskReport()
        {
            HelperClass hc = new HelperClass() { LogEvent = Log };
            var td = DateTime.Now;
            var date = new DateTime(td.Year, td.Month, td.Day, 0, 0, 0).AddDays(-1);

            var items = hc.GetMskReportNew(date, _config.next_day_check_hour_zfile_report, _config.from_day_check_hour_zfile_report, 9);
            items.Sort((a, b) => a.teremok.teremok_name.CompareTo(b.teremok.teremok_name));
            string message = PreparereportMessage(items, date);
            SendMailSync(_config.email_rbs_zfile_report, "Отчет получения z-отчетов за " + date.Date.ToShortDateString(), message, true, "");
        }

        private void MakeMskReportSmena()
        {
            HelperClass hc = new HelperClass() { LogEvent = Log };
            var td = DateTime.Now;
            var date = new DateTime(td.Year, td.Month, td.Day, 0, 0, 0).AddDays(-1);

            var items = hc.GetMskReportNew____Final(date, _config.next_day_check_hour_zfile_report, _config.from_day_check_hour_zfile_report, 9);

            #region serialize
         //   Serializer.binary_write(new ArrayList() { items }, "1.bin");

            // var arl=Serializer.binary_get("1.bin");
        //   List<kkm_terem_stat> items = (List<kkm_terem_stat>)arl[0];
            #endregion

            items.Sort((a, b) => a.teremok.teremok_name.CompareTo(b.teremok.teremok_name));

            string message = PreparereportMessageSmena(items, date);
            SendMailSync(_config.email_rbs_zfile_report, "Отчет получения z-отчетов за " + date.Date.ToShortDateString(), message, true, "");
        }

        private void MakeSpbReportSmena()
        {
            HelperClass hc = new HelperClass() { LogEvent = Log };
            var td = DateTime.Now;
            var date = new DateTime(td.Year, td.Month, td.Day, 0, 0, 0).AddDays(-1);

           var items = hc.GetSpbReportNew____Final(date, _config.next_day_check_hour_zfile_report, _config.from_day_check_hour_zfile_report, 9);

            #region serialize
            //Serializer.binary_write(new ArrayList() { items }, "1.bin");

           //  var arl=Serializer.binary_get("1.bin");
            // List<kkm_terem_stat> items = (List<kkm_terem_stat>)arl[0];
            #endregion

             items = items.Select(a =>
             {
                 if (a.teremok == null)
                 {
                     a.teremok = new t_Teremok() { teremok_1C = "no in bd", teremok_name = "Нет в базе" };
                 }
                 return a;
             }).ToList();
            items.Sort((a, b) => a.teremok.teremok_name.CompareTo(b.teremok.teremok_name));

            string message = PreparereportMessageSmena(items, date);
            SendMailSync(_config.email_rbs_spb_zfile_report, "Отчет получения z-отчетов за " + date.Date.ToShortDateString(), message, true, "");
        }

        private void MakeSpbReport()
        {
            HelperClass hc = new HelperClass() { LogEvent = Log };
            var td = DateTime.Now;
            var date = new DateTime(td.Year, td.Month, td.Day, 0, 0, 0).AddDays(-1);

            var items = hc.GetSpbReport(date, _config.next_day_check_hour_zfile_report, _config.from_day_check_hour_zfile_report, _config.kkm_start_work_hour_zfile_report);

            var items1 = items.Where(a => a.teremok != null && a.teremok.teremok_address != null && a.teremok.teremok_address.CompareTo("5.6.6.1") >= 0).ToList();
            var items2 = items.Where(a => a.teremok == null).ToList();

            items1.Sort((a, b) => a.teremok.teremok_name.CompareTo(b.teremok.teremok_name));
            items2.Sort((a, b) => a.kassa.teremok_1c.CompareTo(b.kassa.teremok_1c));

            ReportHelper rh = new ReportHelper() { LogEvent = Log };
            string message = rh.ПодготовитьТаблицуДляОтчета(items1, date);
            message += "<br/><br/> " + rh.ПодготовитьТаблицуПоОтсутствующимРесторанам(items2);

            SendMailSync(_config.email_rbs_spb_zfile_report, "Отчет получения z-отчетов за " + date.Date.ToShortDateString(), message, true, "");
        }

        private void make_Zfile_Report()
        {
            try
            {
//#if(!DEBUG)
                this.Log("make_Zfile_Report start", 0);
                if (ZfileReportStartCondition(false))
                {
                    this.Log("make_Zfile_Report msk start", 0);
                    //MakeMskReport();
                    MakeMskReportSmena();

                    ZfileReportStartCondition(true);
                }
                string spb_file="ZfileReport_sended_spb";
                if (ZfileReportStartConditionSpb(spb_file))
                {
                    this.Log("make_Zfile_Report spb start", 0);
                    #region piter
                    MakeSpbReportSmena();
                    //MakeSpbReport();
                    ZfileReportStartCondition(true, spb_file);
                    #endregion
                }
//#endif
                string spb_file1 = "No_WebService_spb";
                if (ZfileReportStartConditionSpb(spb_file1))
                {
                    this.Log("No_WebService_spb spb start", 0);
                    #region piter
                    MakeNoWebserviceSpbReport();
                    ZfileReportStartCondition(true, spb_file1);
                    #endregion
                }

                this.Log("make_Zfile_Report end", 0);
            }catch(Exception ex)
            {
                this.Log(string.Concat("Ошибка make_Zfile_Report", ex.Message), 0);
            }
        }

        private void MakeNoWebserviceSpbReport()
        {
            DateTime date = DateTime.Now.AddDays(-1);
            DataTable dt = ProcedureExecute_FillTable("[get_resto_without_msk_webservice_spb]", (sqlCommand) =>
            {
                SqlParameter value = sqlCommand.Parameters.Add("@date_", SqlDbType.VarChar,50);
                value.Direction = ParameterDirection.Input;
                value.Value = date.ToString("yyyyMMdd");
            });

            ReportHelper rh=new ReportHelper();
            List<string> headers = new List<string> {"Название","Дата"};
            string tab = rh.PreparereportStringTh(headers);
            List<List<string>> table = new List<List<string>>();
            foreach(DataRow row in dt.Rows) 
            {
                var a=new List<string>() { CellHelper.FindCell(row, 0).ToString(), date.Date.ToString() };
                table.Add(a);
                tab += rh.PrepareTr(rh.PreparereportString(a),"");
            }

           SendMailSync(_config.email_rbs_spb_zfile_report, "Отчет неподключенных к вебсервису пк питер " + date.Date.ToShortDateString(), rh.PrepareHtml(rh.PrepareTable(tab, ""), ""), true, "");
        }

        

        private string PreparereportMessage(List<ReportItem> items)
        {
            string html = "Все отчеты дошли";
            if (items.NotNullOrEmpty())
            {
                html = "<tr><th>Название теремка</th><th>Имя 1С</th><th>Номер кассы</th>" +
                    "<th>Дата создания</th><th>Дата последнего выхода в сеть</th><th>Дата получения записи</th></tr>";

                foreach (var item in items)
                {
                    html += "<tr>";
                    html += PreparereportString(item);
                    html += "</tr>";
                }
                return String.Format("<table>{0}</table>", html);
            }
            return html;
        }

        private string PreparereportMessage(List<kkm_terem_stat> items,DateTime date)
        {
            ReportHelper rh = new ReportHelper();
            string html = "Все отчеты дошли";
            if (items.NotNullOrEmpty())
            {

                html = "<tr><th>Название теремка</th><th>Имя 1С</th><th>Номер кассы</th>" +
                    "<th>Причина отсутствия отчета</th></tr>";

                

                foreach (var item in items)
                {
                    //убираем утренние отчеты
                    item.statistics.RemoveAll(a => a.zfile != null && a.lasttime_online==null);
                    

                    //нет связи
                    if ((!item.statistics.NotNullOrEmpty()) && (!item.z_reports.NotNullOrEmpty()))
                    {
                        html += "<tr bgcolor=\"#FACC2E\">";
                        List<string> mes_list = new List<string>() {item.teremok.teremok_name,item.teremok.teremok_1C
                            ,item.kassa.kkm_id.ToString(),
                        String.Format("Нет связи с кассой по сети. Последний раз касса была доступна {0}. "+
                            "",
                            rh.LastKkmAvailable(item)
                            //,item.kassa.isOnline7==true?"работает":"не работает")
                            //,7-(DateTime.Now-((DateTime)item.kassa.date_state_updated)).Days)
                            )};
                        html += PreparereportString(mes_list);
                        html += "</tr>";
                    }

                    //нет z-отчета
                    if ((item.statistics.NotNullOrEmpty()) && (!item.z_reports.NotNullOrEmpty()))
                    {
                        if (item.statistics.Count == 1)
                        {
                            html += prepareSingleLine(item, item.statistics[0]);        
                        }
                        if (item.statistics.Count >1)
                        {
                            var ite = item.statistics.Where(a=>((DateTime)a.datetime).Date==date.Date).Last();
                            var ite_last = item.statistics.Last();
                            if (ite == null)
                                html += prepareSingleLine(item, ite_last);
                            else
                                ite.lasttime_online = ite_last.lasttime_online;
                                html += prepareSingleLine(item, ite);
                        }
                    }
                }
                return String.Format("<table border=\"1\">{0}</table>", html);
            }
            return html;
        }

        private string PreparereportMessageSmena(List<kkm_terem_stat> items, DateTime date)
        {
            ReportHelper rh = new ReportHelper();
            string html = "Все отчеты дошли";
            if (items.NotNullOrEmpty())
            {

                html = "<tr><th>Название теремка</th><th>Имя 1С</th><th>Номер кассы</th>" +
                    "<th>Причина отсутствия отчета</th><th>Номер смены</th></tr>";

                foreach (var item in items)
                {
                    if (item.teremok.teremok_1C == "testres")
                    {
                        continue;
                    }

                    //работала ли касса
                    if (item.osmena != null)
                    {
                        //касса работала

                        html += prepareSingleLine(item, item.osmena);
                    }
                    else
                    {
                        //касса была включена но не производила продажи
                        if (item.statistics.NotNullOrEmpty())
                        {
                            //касса была включена но не производила продажи
                            var info = item.statistics.Where(a => a.zfile == null);

                            if (info.NotNullOrEmpty())
                            {
                                //continue;
                                //касса была включена но не производила продажи
                                html += "<tr bgcolor=\"#1E90FF\">";
                                List<string> mes_list = new List<string>() {item.teremok.teremok_name,item.kassa.teremok_1c
                                ,item.kassa.kkm_id.ToString(),
                            String.Format("Касса включена но продажи не производила. Последний раз касса была доступна {0}. "+
                                "",
                                rh.LastKkmAvailable(item)
                                )};
                                html += PreparereportString(mes_list);
                                html += "</tr>";
                              
                            }
                            else
                            {
                                //по кассе просто пришли отчеты 
                                //disable kkms?
                            }
                        }
                        else
                        {
                            //касса не работала
                            if (CheckIfItemIsNotTemporaryError(item))
                            {
                                html += "<tr bgcolor=\"#FACC2E\">";
                                List<string> mes_list = new List<string>() {item.teremok.teremok_name,item.kassa.teremok_1c
                                ,item.kassa.kkm_id.ToString(),
                            String.Format("Нет связи с кассой по сети. Последний раз касса была доступна {0}. "+
                                "",

                                rh.LastKkmNotWorkedAvailable(item)

                                )};
                                html += PreparereportString(mes_list);
                                html += "</tr>";
                            }
                        }
                    }
                }
                return String.Format("<table border=\"1\">{0}</table>", html);
            }
            return html;
        }

        int error_precision = 1;
        private bool CheckIfItemIsNotTemporaryError(kkm_terem_stat item)
        {
            HelperClass hc = new HelperClass() { LogEvent = Log };
            try
            {
                DateTime dt = DateTime.Now.AddDays(-7);

                List<t_Web_kkm_z_info> result = new List<t_Web_kkm_z_info>();

                hc.GetFromBase(() =>
                {
                    string kkmid = item.kassa.kkm_id.ToString();
                    result = hc.Rbmbase.t_Web_kkm_z_info.Where(a =>
                        a.datetime > dt &&
                        a.terem_1c == item.kassa.teremok_1c &&
                        a.num_kkm == kkmid).ToList();
                });
                if (result.NotNullOrEmpty())
                {
                    if (result.Count > error_precision)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log("CheckIfItemIsNotTemporaryError error: " + ex.Message);
                return false;
            }
            finally
            {
                hc.Dispose();
            }
        }


        private string prepareSingleLine(kkm_terem_stat item, t_Web_kkm_z_info stat)
        {
            string html = String.Format("<tr bgcolor=\"{0}\">", stat.worked == true ? "#FE642E" : "#3ADF00");
            List<string> mes_list = new List<string>() {item.teremok.teremok_name,item.kassa.teremok_1c
                ,item.kassa.kkm_id.ToString(),
                            String.Format("Не получен z-отчет. Последний раз касса была доступна {0}. Продажи по кассе {1}.{2}",
                            stat.lasttime_online,
                            stat.worked==true?"производились":"не производились",
                            ((DateTime)stat.lasttime_online).Date==DateTime.Now.Date
                            ?"Отчет кассовой программой не был выгружен либо ошибка арм. Нужно заново выгрузить отчет."
                            :"На точке не запущен арм или касса не включена."
                            )};
            if (item.osmena != null)
            {
                mes_list.Add(String.Format("{0}",item.osmena.shift_num));
            }
            html += PreparereportString(mes_list);
            html += "</tr>";
            return html;
        }


        private string PreparereportString(List<string> items)
        {
            string query = "";
            query=items.Aggregate(query, (a, b) => a + "<td>" + b + "</td>");
            return query;
        }

        private string PreparereportString(ReportItem item)
        {
            string query = "<td>"+item.teremok.teremok_name+"</td>";
            query += "<td>" + item.Zdata.terem_1c+"</td>";
            query+= "<td>" + item.Zdata.num_kkm + "</td>";
            //query+= "<td>" + item.Zdata.worked + "</td>";
            query+= "<td>" + item.Zdata.datetime + "</td>";
            query+= "<td>" + item.Zdata.lasttime_online + "</td>";
            query+= "<td>" + item.Zdata.date_recieved + "</td>";
            return query;
        }

        internal static string ReturnDBValue(object s)
        {
            string value = "";
            if (s == null) return "''";
            if (s is int || s is double || s is decimal)
            {
                value = s.ToString();
                if (s is decimal)
                {
                    value = value.Replace(',', '.');
                }
            }
            else
                if (s is bool)
                {
                    if ((bool)s) value = "-1";
                    else value = "0";
                }
                else
                {
                    value = "'" + s.ToString() + "'";
                }
            return value;
        }

        public static string HomePath = AppDomain.CurrentDomain.BaseDirectory;

        private bool ZfileReportStartConditionSpb(string filename)
        {
            var dt = DateTime.Now;
            if (dt.Hour >= _config.hour_zfile_report_send_spb)
            {
                if (!System.IO.File.Exists(filename))
                {
                    return true;
                }
            }
            else
            {

                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }
            }
            return false;

        }

        private bool ZfileReportStartCondition(bool CreateFile, string zrepSendedFlagFile)
        {
            string filename = Path.Combine(HomePath, zrepSendedFlagFile);
            if (CreateFile)
            {
                System.IO.File.AppendAllText(filename, " ");
                return false;
            }
            else
            {
                var dt = DateTime.Now;
                if (dt.Hour >= _config.hour_zfile_report_send)
                {
                    if (!System.IO.File.Exists(filename))
                    {
                        return true;
                    }
                }
                else
                {

                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }
                }
                return false;
            }
        }

        private bool ZfileReportStartCondition(bool CreateFile)
        {
            return ZfileReportStartCondition(CreateFile, "ZfileReport_sended");

            //string filename =Path.Combine(HomePath, "ZfileReport_sended");
            //if (CreateFile)
            //{
            //    System.IO.File.AppendAllText(filename," ");
            //    return false;
            //}
            //else
            //{
            //    var dt = DateTime.Now;
            //    if (dt.Hour >= _config.hour_zfile_report_send)
            //    {
            //        if (!System.IO.File.Exists(filename))
            //        {
            //            return true;
            //        }
            //    }
            //    else
            //    {

            //        if (System.IO.File.Exists(filename))
            //        {
            //            System.IO.File.Delete(filename);
            //        }
            //    }
            //    return false;
            //}
        } 

        void operate_svyaznoy(SqlConnection sqlConnection)
        {
            if (RBServer.Debug_classes.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NoSendReestr.txt")))
            {
                return;
            }
            this.send_svyaznoy_reestr(sqlConnection, 20, 30);
        }

		private void KKM_shtrih_Load(SqlConnection _conn)
		{
			foreach (RBServer.Debug_classes.DirectoryInfo directory in (new RBServer.Debug_classes.DirectoryInfo(this._config.m_folder_ftp_M)).GetDirectories())
			{
				if (!(directory.Name == "sevastopol") && !(directory.Name == "mayakovka") && !(directory.Name == "leninskiy") && !(directory.Name == "gruzinskiy"))
				{
					continue;
				}
				foreach (RBServer.Debug_classes.DirectoryInfo directoryInfo in directory.GetDirectories())
				{
					RBServer.Debug_classes.DirectoryInfo directoryInfo1 = new RBServer.Debug_classes.DirectoryInfo(directoryInfo.FullName);
					if (directoryInfo.Name != "rep")
					{
						continue;
					}
					foreach (RBServer.Debug_classes.FileInfo file in directoryInfo1.GetFiles())
					{
						if (DebugPanel.IgnoredFile.IsIgnored(file))
						{
							continue;
						}
						try
						{
							if (file.Name.EndsWith(".zip"))
							{
								if (file.CreationTime.AddDays(50) <= DateTime.Now)
								{
									RBServer.Debug_classes.File.Delete(file.FullName);
								}
								else if (this.LoadSalesFromMiniPOS(file, directory.Name, _conn))
								{
									string fullName = file.FullName;
									string[] mFolderZM = new string[] { this._config.m_folder_Z_M, "\\", directory.Name, "\\", file.Name };
									RBServer.Debug_classes.File.Move(fullName, string.Concat(mFolderZM));
									this.Log(string.Concat(directory.Name, " - загружен файл продаж ШтрихМ - ", file.Name), 1);
								}
							}
						}
						catch (Exception exception)
						{
							DebugPanel.OnErrorOccured(string.Concat("Ошибка с файлом ", file));
							DebugPanel.IgnoredFile.MakeFileIgnored(file);
						}
					}
				}
			}
		}

		private void LoadClose(string teremok_folder, SqlConnection conn)
		{
			RBServer.Debug_classes.StreamReader streamReader = null;
			DateTime dateTime = new DateTime();
			CConfig cConfig = new CConfig();
			try
			{
				try
				{
					streamReader = new RBServer.Debug_classes.StreamReader(string.Concat(cConfig.m_folder_ftp, "\\", teremok_folder, "\\in\\close.log"), Encoding.GetEncoding(1251));
					while (true)
					{
						string str = streamReader.ReadLine();
						string str1 = str;
						if (str == null)
						{
							break;
						}
						dateTime = Convert.ToDateTime(str1);
					}
					if (streamReader != null)
					{
						streamReader.Close();
						streamReader.Dispose();
					}
					SqlCommand sqlCommand = new SqlCommand()
					{
						Connection = conn,
						CommandText = "INSERT INTO t_ExchClose(ec_teremok_id, ec_teremok_1C, ec_datetime) VALUES(@ec_teremok_id, @ec_teremok_1C, @ec_datetime)"
					};
					sqlCommand.Parameters.AddWithValue("@ec_teremok_id", this.GetTeremokID(teremok_folder, conn));
					sqlCommand.Parameters.AddWithValue("@ec_teremok_1C", this.GetTeremok1CName(teremok_folder, conn));
					SqlParameterCollection parameters = sqlCommand.Parameters;
					string[] strArrays = new string[] { dateTime.Month.ToString(), "/", dateTime.Year.ToString(), "/", dateTime.Day.ToString(), " ", dateTime.Hour.ToString(), ":", dateTime.Minute.ToString(), ":", dateTime.Second.ToString() };
					parameters.AddWithValue("@ec_datetime", string.Concat(strArrays));
					sqlCommand.ExecuteNonQuery();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.Log(string.Concat("Ошибка обработки файла close", exception.Message), 0);
				}
			}
			finally
			{
				streamReader.Close();
				streamReader.Dispose();
				RBServer.Debug_classes.File.Delete(string.Concat(cConfig.m_folder_ftp, "\\", teremok_folder, "\\in\\close.log"));
				this.Log(string.Concat("Удален close ", teremok_folder), 0);
			}
		}

		public void LoadOpen(string teremok_folder, SqlConnection conn)
		{
			SqlCommand sqlCommand;
			DateTime dateTime = new DateTime();
			CConfig cConfig = new CConfig();
			try
			{
				try
				{
					string str = RBServer.Debug_classes.File.ReadAllText(string.Concat(cConfig.m_folder_ftp, "\\", teremok_folder, "\\in\\open.log"), Encoding.UTF8);
					if (this.date_reg.IsMatch(str))
					{
						dateTime = Convert.ToDateTime(this.date_reg.Match(str).Value);
						sqlCommand = new SqlCommand()
						{
							Connection = conn,
							CommandText = "INSERT INTO t_ExchOpen(eo_teremok_id, eo_teremok_1C, eo_datetime) VALUES(@eo_teremok_id, @eo_teremok_1C, @eo_datetime)"
						};
						sqlCommand.Parameters.AddWithValue("@eo_teremok_id", this.GetTeremokID(teremok_folder, conn));
						sqlCommand.Parameters.AddWithValue("@eo_teremok_1C", this.GetTeremok1CName(teremok_folder, conn));
						SqlParameterCollection parameters = sqlCommand.Parameters;
						string[] strArrays = new string[] { dateTime.Month.ToString(), "/", dateTime.Year.ToString(), "/", dateTime.Day.ToString(), " ", dateTime.Hour.ToString(), ":", dateTime.Minute.ToString(), ":", dateTime.Second.ToString() };
						parameters.AddWithValue("@eo_datetime", string.Concat(strArrays));
						sqlCommand.ExecuteNonQuery();
					}
					if (this.param_reg.IsMatch(str))
					{
						Match[] matchArray = new Match[this.param_reg.Matches(str).Count];
						this.param_reg.Matches(str).CopyTo(matchArray, 0);
						List<Match> list = matchArray.ToList<Match>();
						string str1 = (
							from m in list
							select new { name = m.Value.Split(new char[] { '=' })[0], valu = m.Value.Split(new char[] { '=' })[1] } into t
							where t.name == "version"
							select t).First().valu.Trim();
						string str2 = (
							from m in list
							select new { name = m.Value.Split(new char[] { '=' })[0], valu = m.Value.Split(new char[] { '=' })[1] } into t
							where t.name == "teremok_id"
							select t).First().valu.Trim();
						sqlCommand = new SqlCommand()
						{
							Connection = conn,
							CommandText = "UPDATE t_Teremok SET teremok_address= @xu_teremok_vers WHERE teremok_id= @xu_teremok_id"
						};
						sqlCommand.Parameters.AddWithValue("@xu_teremok_id", str2);
						sqlCommand.Parameters.AddWithValue("@xu_teremok_vers", str1);
						sqlCommand.ExecuteNonQuery();
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.Log(string.Concat("Ошибка обработки файла open", exception.Message), 0);
				}
			}
			finally
			{
				RBServer.Debug_classes.File.Delete(string.Concat(cConfig.m_folder_ftp, "\\", teremok_folder, "\\in\\open.log"));
			}
		}

		private bool LoadSalesFromMiniPOS(RBServer.Debug_classes.FileInfo file_info, string KKM, SqlConnection conn)
		{
			bool flag;
			RBServer.Debug_classes.StreamReader streamReader = null;
			CConfig cConfig = new CConfig();
			try
			{
				try
				{
					if (!this.CheckMiniPOSFile(file_info.Name, KKM, conn))
					{
						CConfig cConfig1 = new CConfig();
						if (RBServer.Debug_classes.File.Exists(string.Concat(cConfig1.m_app_folder, "\\Kassa.Rep")))
						{
							RBServer.Debug_classes.File.Delete(string.Concat(cConfig1.m_app_folder, "\\Kassa.Rep"));
						}
						using (ZipFile zipFiles = ZipFile.Read(file_info.FullName))
						{
							foreach (ZipEntry zipEntry in zipFiles)
							{
								using (RBServer.Debug_classes.FileStream fileStream = new RBServer.Debug_classes.FileStream(string.Concat(cConfig1.m_app_folder, "\\", zipEntry.FileName), FileMode.Create))
								{
									zipEntry.Extract(fileStream);
								}
							}
						}
						char[] charArray = ";".ToCharArray();
						streamReader = new RBServer.Debug_classes.StreamReader(string.Concat(cConfig1.m_app_folder, "\\Kassa.Rep"), Encoding.GetEncoding(1251));
						int num = 0;
						while (streamReader.Peek() >= 0)
						{
							string str = streamReader.ReadLine();
							if (num > 2)
							{
								this.ImportCheckItemMiniPOS(str.Split(charArray), KKM, conn);
							}
							num++;
						}
						if (streamReader != null)
						{
							streamReader.Close();
							streamReader.Dispose();
						}
						this.CompleteCheckMiniPOS(file_info.Name, KKM, conn);
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.Log(string.Concat(exception.Message, " файл", file_info.FullName), 1);
					this.SendMail(this._config.email_rbs_err, "Error RBSERVER loadSalesFromMiniPOS", string.Concat(exception.Message, " файл", file_info.FullName), false);
					flag = false;
				}
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
					streamReader.Dispose();
				}
			}
			return flag;
		}

		public void LoadUptime(string teremok_folder, SqlConnection conn)
		{
			DateTime dateTime = new DateTime();
			CConfig cConfig = new CConfig();
			try
			{
				try
				{
					string str = RBServer.Debug_classes.File.ReadAllText(string.Concat(cConfig.m_folder_ftp, "\\", teremok_folder, "\\in\\uptime.log"), Encoding.UTF8);
					if (this.date_reg.IsMatch(str))
					{
						dateTime = Convert.ToDateTime(this.date_reg.Match(str).Value);
						SqlCommand sqlCommand = new SqlCommand()
						{
							Connection = conn,
							CommandText = "INSERT INTO t_ExchUptime(xu_teremok_id,xu_teremok_1C, xu_datetime) VALUES(@xu_teremok_id, @xu_teremok_1C, @xu_datetime)"
						};
						sqlCommand.Parameters.AddWithValue("@xu_teremok_id", this.GetTeremokID(teremok_folder, conn));
						sqlCommand.Parameters.AddWithValue("@xu_teremok_1C", this.GetTeremok1CName(teremok_folder, conn));
						SqlParameterCollection parameters = sqlCommand.Parameters;
						string[] strArrays = new string[] { dateTime.Month.ToString(), "/", dateTime.Year.ToString(), "/", dateTime.Day.ToString(), " ", dateTime.Hour.ToString(), ":", dateTime.Minute.ToString(), ":", dateTime.Second.ToString() };
						parameters.AddWithValue("@xu_datetime", string.Concat(strArrays));
						sqlCommand.ExecuteNonQuery();
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.Log(string.Concat("Ошибка обработки файла uptime", exception.Message), 0);
				}
			}
			finally
			{
				RBServer.Debug_classes.File.Delete(string.Concat(cConfig.m_folder_ftp, "\\", teremok_folder, "\\in\\uptime.log"));
			}
		}

        public void Log(string message)
        {
            RBServer.Debug_classes.StreamWriter streamWriter = null;
            CTransfer.ErrorFile = message;
            try
            {
                try
                {
                    streamWriter = new RBServer.Debug_classes.StreamWriter(this.log_file_0, true, Encoding.GetEncoding(1251));
                    
                    DateTime now = DateTime.Now;
                    streamWriter.WriteLine(string.Concat(now.ToString(), " > ", message));
                    streamWriter.Close();
                    if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 45)
                    {
                        this.Copy_log_toOlder_date("RBService_0.log");
                        this.Copy_log_toOlder_date("RBService_2.log");
                    }
                }
                catch (Exception exception)
                {
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
            }
        }

		public void Log(string message, int type)
		{
			RBServer.Debug_classes.StreamWriter streamWriter = null;
			CTransfer.ErrorFile = message;
			try
			{
				try
				{
					if (type == 0)
					{
						streamWriter = new RBServer.Debug_classes.StreamWriter(this.log_file_0, true, Encoding.GetEncoding(1251));
					}
					if (type == 1)
					{
						streamWriter = new RBServer.Debug_classes.StreamWriter(this.log_file_1, true, Encoding.GetEncoding(1251));
					}
					if (type == 2)
					{
						streamWriter = new RBServer.Debug_classes.StreamWriter(this.log_file_2, true, Encoding.GetEncoding(1251));
					}
					DateTime now = DateTime.Now;
					streamWriter.WriteLine(string.Concat(now.ToString(), " > ", message));
					streamWriter.Close();
					if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 45)
					{
						this.Copy_log_toOlder_date("RBService_0.log");
						this.Copy_log_toOlder_date("RBService_2.log");
					}
				}
				catch (Exception exception)
				{
				}
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
				}
			}
		}

		internal void make_svyaznoy_reestr(SqlConnection _conn)
		{
            DateTime dt = DateTime.Now;
			DataTable dataTable = this.returnItems(_conn,dt);
			this.CatchError(dataTable);
			this.CopyToCSV(dataTable);
			this.UpdateSBVtable(_conn, this.returnItemsUpdate(_conn,dt));
		}

        private DataTable returnItems(SqlConnection conn, DateTime dt)
        {
            SqlCommand _command = null;
            DataTable _table;
            string _str_command = "";
            int _timeout = 0;

            try
            {
                //                EAN13;DATE;TIME;ART;QUANTITY;PRICE;SUM;OP_ID;TO_ID
                //                6048093054611559;20.07.2012;12:48:37;0000000000304;1;0;0;132638/000002;svetl

                _command = new SqlCommand();
                _command.Connection = conn;
                _command.CommandTimeout = _config.sql_timeout;
                //_str_command = "SELECT dbo.t_CheckItem.ch_dinner_card, dbo.t_Check.check_datetime, dbo.t_CheckItem.ch_mnome_id, dbo.t_CheckItem.ch_count, dbo.t_CheckItem.ch_amount2, dbo.t_Check.check_num, dbo.t_Teremok.teremok_1C "
                //+ " FROM dbo.t_CheckItem LEFT JOIN  dbo.t_Check ON dbo.t_CheckItem.ch_check_id = dbo.t_Check.check_id LEFT JOIN dbo.t_ZReport ON dbo.t_CheckItem.ch_zreport_id = dbo.t_ZReport.z_id LEFT JOIN "
                //+ " dbo.t_Teremok ON dbo.t_ZReport.z_teremok_id = dbo.t_Teremok.teremok_id "
                //+ " WHERE (dbo.t_ZReport.z_1c_load = '1') AND z_svb_load = '0' AND (dbo.t_CheckItem.ch_svb = 1)";

                _str_command = "SELECT dbo.t_CheckItem.ch_dinner_card, dbo.t_Check.check_datetime, dbo.t_CheckItem.ch_mnome_id, dbo.t_CheckItem.ch_count, dbo.t_CheckItem.ch_amount2, dbo.t_Check.check_num, dbo.t_Teremok.teremok_1C "
                + " FROM dbo.t_CheckItem LEFT JOIN  dbo.t_Check ON dbo.t_CheckItem.ch_check_id = dbo.t_Check.check_id LEFT JOIN dbo.t_ZReport ON dbo.t_CheckItem.ch_zreport_id = dbo.t_ZReport.z_id LEFT JOIN "
                + " dbo.t_Teremok ON dbo.t_ZReport.z_teremok_id = dbo.t_Teremok.teremok_id "
                + " WHERE (ch_datetime > CONVERT(DATETIME, '" + dt.AddMonths(-1).ToString("yyyy-MM-dd 00:00:00") + "', 102)) AND  (dbo.t_ZReport.z_1c_load = 1) AND (z_svb_load = 0) AND (dbo.t_CheckItem.ch_svb = 1)";

                _command.CommandText = _str_command;
                SqlDataAdapter _data_adapter = new SqlDataAdapter(_str_command, conn);
                _table = new DataTable("Svayznoi");
                _timeout = _command.CommandTimeout;
                Log("Начинаем выгрузку связного", 0);
                _data_adapter.Fill(_table);
                return _table;
            }
            catch (Exception _exp)
            {
                ErrorFile = " error: " + conn.ToString() + " command: " + _str_command;
                Log(_exp.Message + ErrorFile + " timeout:" + _timeout.ToString(), 0);
                throw _exp;
            }
        }

        private DataTable returnItemsUpdate(SqlConnection conn, DateTime dt)
        {
            SqlCommand _command = null;
            DataTable _table;
            try
            {
                _command = new SqlCommand();
                _command.Connection = conn;
                _command.CommandTimeout = _config.sql_timeout;
                string _str_command = "SELECT DISTINCT dbo.t_ZReport.z_id "
                + " FROM dbo.t_CheckItem INNER JOIN  dbo.t_Check ON dbo.t_CheckItem.ch_check_id = dbo.t_Check.check_id INNER JOIN dbo.t_ZReport ON dbo.t_CheckItem.ch_zreport_id = dbo.t_ZReport.z_id INNER JOIN "
                + " dbo.t_Teremok ON dbo.t_ZReport.z_teremok_id = dbo.t_Teremok.teremok_id "
                + "WHERE (ch_datetime > CONVERT(DATETIME, '" + dt.AddMonths(-1).ToString("yyyy-MM-dd 00:00:00") + "', 102)) AND (z_svb_load = 0) AND (dbo.t_ZReport.z_1c_load = 1) AND (dbo.t_CheckItem.ch_svb = 1) AND (dbo.t_Teremok.teremok_1C NOT LIKE 'testres')";
                _command.CommandText = _str_command;
                SqlDataAdapter _data_adapter = new SqlDataAdapter(_str_command, conn);
                _table = new DataTable("Svayznoi");
                _data_adapter.Fill(_table);
                return _table;
            }
            catch (Exception _exp)
            {
                Log(_exp.Message, 0);
                ErrorFile = "error: " + conn.ToString() + " command: " + _command;
                throw _exp;
            }
        }

		private void Menu(RBServer.Debug_classes.FileInfo fi)
		{
		}

		private bool MoveFile(string file_from, string file_to)
		{
			bool flag;
			try
			{
				RBServer.Debug_classes.File.Move(file_from, file_to);
				this.Log(string.Concat("Перемещаю файл ", file_from), 2);
				flag = true;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.Log(string.Concat("Ошибка отправки: ", file_to, "Строка ошибки: ", exception.ToString()), 2);
				flag = false;
			}
			return flag;
		}

		private void MoveTransferFile(RBServer.Debug_classes.FileInfo fi)
		{
			RBServer.Debug_classes.StreamReader streamReader = null;
			try
			{
				try
				{
					streamReader = new RBServer.Debug_classes.StreamReader(fi.FullName, Encoding.GetEncoding(1251));
					string str = streamReader.ReadLine();
					string str1 = str.Substring(30, str.Length - 32);
					CConfig cConfig = new CConfig();
					if (RBServer.Debug_classes.Directory.Exists(string.Concat(cConfig.m_folder_ftp, "\\", str1)))
					{
						string fullName = fi.FullName;
						string[] mFolderFtp = new string[] { cConfig.m_folder_ftp, "\\", str1, "\\out\\", fi.Name };
						RBServer.Debug_classes.File.Copy(fullName, string.Concat(mFolderFtp));
					}
				}
				catch (Exception exception)
				{
				}
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
				}
			}
		}

		private string OrderDetailForMessage(int doc_id, SqlConnection conn)
		{
			string str;
			if (doc_id == 0)
			{
				return "";
			}
			string str1 = "<html><body><table border=1>";
			try
			{
				string str2 = string.Concat("SELECT nome_name, opd_order, nome_ed, nome_k, nome_bed, opd_order_bed FROM v_OrderDetails WHERE opd_doc_id = ", doc_id.ToString());
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(str2, conn);
				DataTable dataTable = new DataTable("OrderDetails");
				sqlDataAdapter.Fill(dataTable);
				foreach (DataRow row in dataTable.Rows)
				{
					str1 = string.Concat(str1, "<tr>");
					str1 = string.Concat(str1, "<td>", row[0].ToString(), "</td>");
					str1 = string.Concat(str1, "<td align=right>", row[1].ToString(), "</td>");
					str1 = string.Concat(str1, "<td>", row[4].ToString(), "</td>");
					str1 = string.Concat(str1, "</tr>");
				}
				str1 = string.Concat(str1, "</table></body></html>");
				str = str1;
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return str;
		}

		private string ParceAmount(string amount)
		{
			string str;
			try
			{
				str = (amount != "" ? amount.Replace(".", ",") : "0");
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return str;
		}

		private void parseFile(RBServer.Debug_classes.FileInfo fileName, SqlConnection _conn)
		{
			XmlReader xmlReader = null;
			string str = "";
			string str1 = "";
			string str2 = "";
			string str3 = "";
			string str4 = "";
			string str5 = "";
			string str6 = "";
			string str7 = "";
			string str8 = "";
			string str9 = "";
			string str10 = "";
			string str11 = "0";
			try
			{
				try
				{
					xmlReader = XmlReader.Create(fileName.FullName);
					foreach (XElement xElement in XDocument.Load(xmlReader).Root.Elements())
					{
						foreach (XElement xElement1 in xElement.Elements())
						{
							foreach (XElement xElement2 in xElement1.Elements())
							{
								foreach (XAttribute xAttribute in xElement2.Attributes())
								{
									string str12 = xAttribute.Name.ToString();
									string str13 = str12;
									if (str12 == null)
									{
										continue;
									}
									if (str13 == "НомерТерминала")
									{
										str = xAttribute.Value.ToString();
									}
									else if (str13 == "Дочерняяорганизация")
									{
										str11 = xAttribute.Value.ToString();
									}
								}
								foreach (XElement xElement3 in xElement2.Elements())
								{
									string str14 = xElement3.Name.LocalName.ToString();
									string str15 = str14;
									if (str14 != null)
									{
										switch (str15)
										{
											case "УникальныйНомерТранзакции":
											{
												str10 = xElement3.Value.ToString();
												break;
											}
											case "СистемаРасчетов":
											{
												str1 = xElement3.Value.ToString();
												break;
											}
											case "НомерКарты":
											{
												str2 = xElement3.Value.ToString();
												break;
											}
											case "ТипОперации":
											{
												str3 = xElement3.Value.ToString();
												break;
											}
											case "СуммаОперации":
											{
												str4 = xElement3.Value.ToString();
												break;
											}
											case "Комиссия":
											{
												str5 = xElement3.Value.ToString();
												break;
											}
											case "Возмещение":
											{
												str6 = xElement3.Value.ToString();
												break;
											}
											case "Валюта":
											{
												str7 = xElement3.Value.ToString();
												break;
											}
											case "ВремяОперации":
											{
												str8 = xElement3.Value.ToString();
												break;
											}
											case "КодАвторизации":
											{
												str9 = xElement3.Value.ToString();
												this.writeBD = true;
												break;
											}
										}
									}
									if (!this.writeBD)
									{
										continue;
									}
									this.ImportRSBToDB(_conn, str, str1, str2, str3, str4, str5, str6, str7, str8, str9, str10, str11);
									this.Import(_conn, str, str11);
									this.writeBD = false;
								}
							}
						}
					}
					xmlReader.Close();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					CTransfer.ErrorFile = string.Concat("error: ", _conn.ToString(), " file: ", fileName.FullName);
					this.writeBD = false;
					throw exception;
				}
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
			}
		}

		internal void ParseZReport(string teremok_folder, string kks, SqlConnection conn)
		{
			CConfig cConfig = new CConfig();
			CZReport cZReport = new CZReport();
			bool flag = true;
			string fullName = "";
			try
			{
				string[] mFolderZ = new string[] { cConfig.m_folder_Z, "\\", teremok_folder, "\\", kks };
				RBServer.Debug_classes.DirectoryInfo directoryInfo = new RBServer.Debug_classes.DirectoryInfo(string.Concat(mFolderZ));
				CTransfer.zreport_cl = new CTransfer.YZreport();
				CTransfer.yreport_cl = new CTransfer.YZreport();
				foreach (RBServer.Debug_classes.FileInfo file in directoryInfo.GetFiles())
				{
					fullName = file.FullName;
					if (file.Name.Substring(0, 1).ToLower() == "x" && !cZReport.IsZReportSent(file.Name, teremok_folder, conn))
					{
						cZReport.ZReportParse(file, teremok_folder, conn);
						flag = false;
					}
					if (file.Name.Substring(0, 1).ToLower() == "y" && !flag)
					{
						cZReport.YReportParse(file, teremok_folder, conn);
					}
					try
					{
						if (CTransfer.zreport_cl.Equals(CTransfer.yreport_cl) && (int.Parse(CTransfer.yreport_cl._num_fiscal) == 0 || decimal.Parse(CTransfer.yreport_cl._notrest_total.Replace('.', ',')) == new decimal(0)))
						{
							string emailRbsMessZ = cConfig.email_rbs_mess_z;
							string[] _teremokName = new string[] { "В У-отчете пустой номер фискального регистратора или необнуляемый итог. Не загрузится инкассация!\r\n Теремок: ", CTransfer.zreport_cl._teremok_name, " Номер кассы: ", CTransfer.zreport_cl._kkm_id, "\r\n" };
							this.SendMailSync(emailRbsMessZ, "Error RBSERVER", string.Concat(_teremokName), false, CTransfer.yreport_cl.file_name);
						}
					}
					catch (Exception exception)
					{
					}
					try
					{
						string str = file.FullName;
						string[] strArrays = new string[] { cConfig.m_folder_Z, "\\", kks, "\\", file.Name };
						RBServer.Debug_classes.File.Copy(str, string.Concat(strArrays), true);
						RBServer.Debug_classes.File.Delete(file.FullName);
					}
					catch (Exception exception2)
					{
						Exception exception1 = exception2;
						string str1 = string.Concat("filename:", file.FullName, " exception:", exception1.Message);
						this.SendMail(cConfig.email_rbs_err, "Error RBSERVER ParseZReport", str1, false);
						this.Log(str1, 2);
					}
				}
			}
			catch (Exception exception4)
			{
				Exception exception3 = exception4;
				string str2 = string.Concat("filename:", fullName, " exception:", exception3.Message);
				this.SendMail(cConfig.email_rbs_err, "Error RBSERVER ParseZReport", str2, false, fullName);
				this.Log(str2, 0);
				throw new Exception(str2);
			}
		}

		private void ParseZReportDX(string teremok_folder, string kks, SqlConnection conn)
		{
			CConfig cConfig = new CConfig();
			CZReport cZReport = new CZReport();
			try
			{
				string[] mFolderZ = new string[] { cConfig.m_folder_Z, "\\", teremok_folder, "\\", kks };
				foreach (RBServer.Debug_classes.FileInfo file in (new RBServer.Debug_classes.DirectoryInfo(string.Concat(mFolderZ))).GetFiles())
				{
					if (!RBServer.Debug_classes.Directory.Exists(string.Concat(cConfig.m_folder_Z, "\\", kks)))
					{
						RBServer.Debug_classes.Directory.CreateDirectory(string.Concat(cConfig.m_folder_Z, "\\", kks));
					}
					if (!(file.Name.Substring(0, 2).ToLower() == "dx") || cZReport.IsDXReportSent(file.Name, teremok_folder, conn))
					{
						continue;
					}
					cZReport.ZDXReportParse(file, teremok_folder, conn);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.SendMail(cConfig.email_rbs_err, "Error RBSERVER DXReport", exception.Message, false);
				this.Log(exception.Message, 0);
			}
		}

		internal void ParseZReportSpb(string teremok_folder, string kks, SqlConnection conn)
		{
			CConfig cConfig = new CConfig();
			CZReport cZReport = new CZReport();
			bool flag = true;
			string fullName = "";
			try
			{
				string[] mFolderZ = new string[] { cConfig.m_folder_Z, "\\", teremok_folder, "\\", kks };
				foreach (RBServer.Debug_classes.FileInfo file in (new RBServer.Debug_classes.DirectoryInfo(string.Concat(mFolderZ))).GetFiles())
				{
					fullName = file.FullName;
					if (file.Name.Substring(0, 1).ToLower() == "x" && !cZReport.IsZReportSent(file.Name, teremok_folder, conn))
					{
						cZReport.ZReportParseSpb(file, teremok_folder, conn);
						flag = false;
					}
					if (file.Name.Substring(0, 1).ToLower() == "y" && !flag)
					{
						cZReport.YReportParse(file, teremok_folder, conn);
					}
					try
					{
						string str = file.FullName;
						string[] strArrays = new string[] { cConfig.m_folder_Z, "\\", teremok_folder, "\\", file.Name };
						RBServer.Debug_classes.File.Copy(str, string.Concat(strArrays), true);
						RBServer.Debug_classes.File.Delete(file.FullName);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						string str1 = string.Concat("filename:", file.FullName, " exception:", exception.Message);
						this.SendMail(cConfig.email_rbs_err, "Error RBSERVER ParseZReport", str1, false, file.FullName);
						this.Log(str1, 2);
					}
				}
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				string str2 = string.Concat("filename:", fullName, " exception:", exception2.Message);
				this.SendMail(cConfig.email_rbs_err, "Error RBSERVER ParseZReport", str2, false, fullName);
				this.Log(str2, 0);
				throw new Exception(str2);
			}
		}

		private void PrepareTeremok(SqlConnection conn)
		{
			CConfig cConfig = new CConfig();
			try
			{
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT teremok_id, teremok_name, teremok_1C, teremok_pass FROM t_Teremok WHERE teremok_need_update = 1", conn);
				DataTable dataTable = new DataTable("t_Teremok");
				sqlDataAdapter.Fill(dataTable);
				if (dataTable.Rows.Count != 0)
				{
					sqlDataAdapter = new SqlDataAdapter("SELECT teremok_1C FROM t_Teremok WHERE teremok_use_ARM = 1", conn);
					DataTable dataTable1 = new DataTable("t_Teremok_Send");
					sqlDataAdapter.Fill(dataTable1);
					foreach (DataRow row in dataTable1.Rows)
					{
						object[] str = new object[] { row[0].ToString(), "_7_", null, null, null, null, null, null };
						str[2] = DateTime.Now.Year;
						str[3] = "_";
						str[4] = DateTime.Now.Month;
						str[5] = "_";
						str[6] = DateTime.Now.Day;
						str[7] = ".xml";
						string str1 = string.Concat(str);
						dataTable.WriteXml(string.Concat(cConfig.m_folder_ftp, row[0].ToString(), "\\OUT\\", str1));
						string[] mFolderFtp = new string[] { "Отправлен файл: ", cConfig.m_folder_ftp, row[0].ToString(), "\\OUT\\", str1 };
						this.Log(string.Concat(mFolderFtp), 0);
					}
				}
				SqlCommand sqlCommand = new SqlCommand()
				{
					CommandText = "UPDATE t_Teremok SET teremok_need_update = 0",
					Connection = conn
				};
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private int return_doc_count(SqlConnection _conn, string _doc_type_id, DateTime _dt, string _code_teremok)
		{
			int num;
			string str = "";
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = _conn
				};
				string[] _docTypeId = new string[] { "SELECT count(doc_id) FROM t_Doc WHERE doc_type_id='", _doc_type_id, "'AND doc_teremok_id = '", _code_teremok, "' AND doc_datetime='", _dt.Month.ToString(), ".", _dt.Year.ToString(), ".", _dt.Day.ToString(), " ", _dt.Hour.ToString(), ":", _dt.Minute.ToString(), ":", _dt.Second.ToString(), "'" };
				str = string.Concat(_docTypeId);
				sqlCommand.CommandText = str;
				num = Convert.ToInt32(sqlCommand.ExecuteScalar().ToString());
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				CTransfer.ErrorFile = string.Concat("error: ", exception.Message, " command: ", str);
				throw exception;
			}
			return num;
		}

		private int return_doc_status(int doc_id, SqlConnection conn)
		{
			int num;
			string str = "";
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				str = string.Concat("SELECT doc_status_id FROM t_Doc WHERE doc_id='", doc_id, "'");
				sqlCommand.CommandText = str;
				num = Convert.ToInt32(sqlCommand.ExecuteScalar().ToString());
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.Log(exception.Message, 0);
				CTransfer.ErrorFile = string.Concat("error: ", conn.ToString(), " command: ", str);
				throw exception;
			}
			return num;
		}

		private DataTable returnItems(SqlConnection conn)
		{
			DataTable dataTable;
			SqlCommand sqlCommand = null;
			string str = "";
			int commandTimeout = 0;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn,
					CommandTimeout = this._config.sql_timeout
				};
				str = "SELECT dbo.t_CheckItem.ch_dinner_card, dbo.t_Check.check_datetime, dbo.t_CheckItem.ch_mnome_id, dbo.t_CheckItem.ch_count, dbo.t_CheckItem.ch_amount2, dbo.t_Check.check_num, dbo.t_Teremok.teremok_1C  FROM dbo.t_CheckItem LEFT JOIN  dbo.t_Check ON dbo.t_CheckItem.ch_check_id = dbo.t_Check.check_id LEFT JOIN dbo.t_ZReport ON dbo.t_CheckItem.ch_zreport_id = dbo.t_ZReport.z_id LEFT JOIN  dbo.t_Teremok ON dbo.t_ZReport.z_teremok_id = dbo.t_Teremok.teremok_id  WHERE (dbo.t_ZReport.z_1c_load = '1') AND z_svb_load = '0' AND (dbo.t_CheckItem.ch_svb = 1)";
				sqlCommand.CommandText = str;
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(str, conn);
				DataTable dataTable1 = new DataTable("Svayznoi");
				commandTimeout = sqlCommand.CommandTimeout;
				this.Log("Начинаем выгрузку связного", 0);
				sqlDataAdapter.Fill(dataTable1);
				dataTable = dataTable1;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				CTransfer.ErrorFile = string.Concat(" error: ", conn.ToString(), " command: ", str);
				this.Log(string.Concat(exception.Message, CTransfer.ErrorFile, " timeout:", commandTimeout.ToString()), 0);
				throw exception;
			}
			return dataTable;
		}

		private DataTable returnItemsUpdate(SqlConnection conn)
		{
			DataTable dataTable;
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn,
					CommandTimeout = this._config.sql_timeout
				};
				string str = "SELECT DISTINCT dbo.t_ZReport.z_id  FROM dbo.t_CheckItem INNER JOIN  dbo.t_Check ON dbo.t_CheckItem.ch_check_id = dbo.t_Check.check_id INNER JOIN dbo.t_ZReport ON dbo.t_CheckItem.ch_zreport_id = dbo.t_ZReport.z_id INNER JOIN  dbo.t_Teremok ON dbo.t_ZReport.z_teremok_id = dbo.t_Teremok.teremok_id  WHERE z_svb_load = '0' AND (dbo.t_ZReport.z_1c_load = '1') AND (dbo.t_CheckItem.ch_svb = 1) AND (dbo.t_Teremok.teremok_1C NOT LIKE 'testres')";
				sqlCommand.CommandText = str;
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(str, conn);
				DataTable dataTable1 = new DataTable("Svayznoi");
				sqlDataAdapter.Fill(dataTable1);
				dataTable = dataTable1;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.Log(exception.Message, 0);
				object[] objArray = new object[] { "error: ", conn.ToString(), " command: ", sqlCommand };
				CTransfer.ErrorFile = string.Concat(objArray);
				throw exception;
			}
			return dataTable;
		}

		private void Rsb_Operation_perform(SqlConnection _conn)
		{
			try
			{
                Program._transfer.Log("Rsb_Operation_perform started", 0);
				foreach (RBServer.Debug_classes.FileInfo file in (new RBServer.Debug_classes.DirectoryInfo(this._config.m_folderRSB)).GetFiles())
				{
					this._errfile = file.FullName;
					if (!file.Name.EndsWith(".xml"))
					{
						continue;
					}
					this.parseFile(file, _conn);
					RBServer.Debug_classes.File.Copy(file.FullName, string.Concat(this._config.m_folderRSBarhiv, "\\", file.Name), true);
					file.Delete();
				}
                Program._transfer.Log("Rsb_Operation_perform ended", 0);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.SendMail(this._config.email_rbs_err, string.Concat("Error RBSERVER ", this._errfile), exception.Message, false);
			}
		}

		private void Send_Files_to_Restaurants(SqlConnection _conn)
		{
			foreach (RBServer.Debug_classes.FileInfo file in (new RBServer.Debug_classes.DirectoryInfo(this._config.m_folder_1c_out)).GetFiles())
			{
				if (DebugPanel.IgnoredFile.IsIgnored(file))
				{
					continue;
				}
				try
				{
					if (this.ExecuteOutboxTask(file, _conn))
					{
						this.Log(string.Concat("к отправке  ---- ", file.Name), 0);
					}
				}
				catch (Exception exception)
				{
					DebugPanel.OnErrorOccured(string.Concat("Ошибка с файлом ", file));
					DebugPanel.IgnoredFile.MakeFileIgnored(file);
				}
			}
			if (this._config.m_dep == "msk")
			{
				foreach (RBServer.Debug_classes.FileInfo fileInfo in (new RBServer.Debug_classes.DirectoryInfo(this._config.m_folder_1c_order_out)).GetFiles())
				{
					if (DebugPanel.IgnoredFile.IsIgnored(fileInfo))
					{
						continue;
					}
					try
					{
						if (this.ExecuteOutboxTask(fileInfo, _conn))
						{
							this.Log(string.Concat("к отправке  ---- ", fileInfo.Name), 0);
						}
					}
					catch (Exception exception1)
					{
						DebugPanel.OnErrorOccured(string.Concat("Ошибка с файлом ", fileInfo));
						DebugPanel.IgnoredFile.MakeFileIgnored(fileInfo);
					}
				}
			}
			if (this._config.m_dep == "spb")
			{
				foreach (RBServer.Debug_classes.FileInfo file1 in (new RBServer.Debug_classes.DirectoryInfo(this._config.m_folder_1c_order_out)).GetFiles())
				{
					if (DebugPanel.IgnoredFile.IsIgnored(file1))
					{
						continue;
					}
					try
					{
						if (this.ExecuteOutboxTask(file1, _conn))
						{
							this.Log(string.Concat("к отправке  ---- ", file1.Name), 0);
						}
					}
					catch (Exception exception2)
					{
						DebugPanel.OnErrorOccured(string.Concat("Ошибка с файлом ", file1));
						DebugPanel.IgnoredFile.MakeFileIgnored(file1);
					}
				}
			}
		}

		internal void send_svyaznoy_reestr(SqlConnection _conn, int hour, int minute)
		{
			if (this._config.m_dep == "msk")
			{
				object[] year = new object[] { "teremok_reestr_", null, null, null, null, null, null };
				year[1] = DateTime.Now.Year;
				year[2] = "_";
				year[3] = DateTime.Now.Month;
				year[4] = "_";
				year[5] = DateTime.Now.Day;
				year[6] = ".csv_sended";
				if (!RBServer.Debug_classes.File.Exists(string.Concat(year)))
				{
					string svyaznoyUploadDay = this._config.svyaznoy_upload_day;
					string[] strArrays = new string[] { "," };
                    if (svyaznoyUploadDay.Split(strArrays, StringSplitOptions.RemoveEmptyEntries).Contains<string>(((int)DateTime.Today.DayOfWeek).ToString()))
					{
						TimeSpan timeSpan = TimeSpan.Parse(this._config.svyaznoy_upload_time);
						if (DateTime.Now.Hour == timeSpan.Hours && DateTime.Now.Minute >= timeSpan.Minutes && DateTime.Now.Minute <= timeSpan.Minutes + this._config.svyaznoy_upload_sending_time_min)
						{
                            make_svyaznoy_reestr(_conn);

							this.FTPSVB();
							this.loadS = false;
						}
					}
				}
			}
		}

		private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
		{
			string userState = (string)e.UserState;
			if (e.Cancelled)
			{
				Console.WriteLine("[{0}] Send canceled.", userState);
				DebugPanel.Log(string.Concat("[{0}] Send canceled.", userState));
			}
			if (e.Error == null)
			{
				Console.WriteLine("Message sent.");
				DebugPanel.Log("Message sent.");
				return;
			}
			Console.WriteLine("[{0}] {1}", userState, e.Error.ToString());
			DebugPanel.Log(string.Concat("[{0}] {1} '", userState, "' ", e.Error.ToString()));
		}

		private void SendFileResuming(FtpSession m_client, RBServer.Debug_classes.FileInfo _fi)
		{
			try
			{
				long size = (long)0;
				string str = string.Concat(_fi.Name, ".tmp");
				bool flag = true;
				FtpFile ftpFile = m_client.CurrentDirectory.FindFile(_fi.Name);
				if (ftpFile != null)
				{
					if (ftpFile.Size >= _fi.Length)
					{
						flag = false;
					}
					else
					{
						m_client.CurrentDirectory.RemoveItem(ftpFile);
					}
				}
				if (flag)
				{
					FtpFile ftpFile1 = m_client.CurrentDirectory.FindFile(str);
					if (ftpFile1 != null)
					{
						size = ftpFile1.Size;
					}
					if (size < _fi.Length)
					{
						m_client.CurrentDirectory.PutFile(_fi.FullName, str, size);
					}
					m_client.CurrentDirectory = m_client.CurrentDirectory;
					ftpFile1 = m_client.CurrentDirectory.FindFile(str);
					if (ftpFile1 != null)
					{
						m_client.CurrentDirectory.RenameSubitem(ftpFile1, _fi.Name);
						Thread.Sleep(500);
					}
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		public string SendMail(string To, string Subject, string Message, bool isHTML)
		{
			return this.SendMail(To, Subject, Message, isHTML, "");
		}

		public string SendMail(string To, string Subject, string Message, bool isHTML, string attachment_filename)
		{
			ThreadPool.QueueUserWorkItem((object a) => this.SendMailSync(To, Subject, Message, isHTML, attachment_filename));
			return string.Empty;
		}

		public string SendMailSync(string To, string Subject, string Message, bool isHTML, string attachment_filename)
		{
            MailMessage mailMessage = null;
			this.Log(string.Concat("Пытаемся отправить email: ", Subject), 0);
			CConfig cConfig = new CConfig();
			string mSendFrom = cConfig.m_send_from;
			string empty = string.Empty;
			SmtpClient smtpClient = new SmtpClient(cConfig.m_smtp_server);
			string mSmtpLogin = cConfig.m_smtp_login;
			if (string.IsNullOrEmpty(mSendFrom))
			{
				mSendFrom = mSmtpLogin;
			}
			smtpClient.Credentials = new NetworkCredential(mSmtpLogin, cConfig.m_smtp_pass, "msk");
			try
			{
				try
				{
					//Message = string.Concat(Message, DebugPanel.InMethod(3, 7));
					mailMessage = new MailMessage(mSendFrom, To, Subject, Message);
					if (DebugPanel.IsON)
					{
						DebugPanel.SaveEmailToDisk(mailMessage);
					}
					if (attachment_filename != "")
					{
						mailMessage.Attachments.Add(new Attachment(attachment_filename));
					}
					if (Subject == "Сформирован отчет Связной")
					{
						object[] year = new object[] { "\\\\mskapp\\RB_server\\RBServer\\teremok_reestr_", null, null, null, null, null, null };
						year[1] = DateTime.Now.Year;
						year[2] = "_";
						year[3] = DateTime.Now.Month;
						year[4] = "_";
						year[5] = DateTime.Now.Day;
						year[6] = ".csv";
						Attachment attachment = new Attachment(string.Concat(year));
						mailMessage.Attachments.Add(attachment);
					}
					mailMessage.IsBodyHtml = isHTML;
					smtpClient.Send(mailMessage);
				}
				catch (FormatException formatException)
				{
					empty = string.Concat("Ошибка: ", formatException.Message);
				}
				catch (ArgumentException argumentException)
				{
					empty = string.Concat("Ошибка: ", argumentException.Message);
				}
				catch (SmtpFailedRecipientsException smtpFailedRecipientsException)
				{
					empty = string.Concat("Ошибка: ", smtpFailedRecipientsException.Message);
				}
				catch (SmtpException smtpException)
				{
					empty = string.Concat("Ошибка: ", smtpException.Message);
				}
				catch (InvalidOperationException invalidOperationException)
				{
					empty = string.Concat("Ошибка: ", invalidOperationException.Message);
				}
				catch (Exception exception)
				{
					empty = string.Concat("Ошибка: ", exception.Message);
				}
			}
			finally
			{
                if (mailMessage != null)
                {
                    mailMessage.Dispose();
                }
				if (empty == string.Empty)
				{
					this.Log(string.Concat("Отправили email: ", Subject), 0);
				}
				else
				{
					this.Log(empty, 0);
				}
				Thread.Sleep(2000);
			}
			return string.Empty;
		}

		private void UpdateSBVtable(SqlConnection _conn, DataTable _dt)
		{
			string str = "";
			DateTime today = DateTime.Today;
			try
			{
				foreach (DataRow row in _dt.Rows)
				{
					SqlCommand sqlCommand = new SqlCommand()
					{
						Connection = _conn
					};
					str = string.Concat("UPDATE t_ZReport SET z_svb_load = @z_svb_load, z_svb_date_upload = @z_svb_date_upload WHERE z_id = '", Convert.ToInt32(row[0].ToString()), "'");
					sqlCommand.Parameters.Clear();
					sqlCommand.CommandTimeout = this._config.sql_timeout;
					sqlCommand.Parameters.AddWithValue("@z_svb_load", 1);
					sqlCommand.Parameters.AddWithValue("@z_svb_date_upload", today);
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.Log(exception.Message, 0);
				CTransfer.ErrorFile = string.Concat("error: ", _conn.ToString(), " command: ", str);
				throw exception;
			}
		}

		private void Uptime(SqlConnection conn)
		{
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn,
					CommandText = "UPDATE t_State SET state=@state WHERE state_id=1"
				};
				sqlCommand.Parameters.AddWithValue("@state", DateTime.Now);
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		public int ZReportAdd(string z_num, DateTime z_date, string z_kkm, string z_file, double z_total, double z_total_return, string teremok_id, SqlConnection conn)
		{
			int num;
			try
			{
				SqlCommand sqlCommand = new SqlCommand("[sp_ZReport]", conn)
				{
					CommandType = CommandType.StoredProcedure
				};
				SqlParameter value = sqlCommand.Parameters.Add("@z_id", SqlDbType.Int);
				value.Direction = ParameterDirection.InputOutput;
				SqlParameter zNum = sqlCommand.Parameters.Add("@z_num", SqlDbType.NVarChar, 50);
				SqlParameter zFile = sqlCommand.Parameters.Add("@z_file", SqlDbType.NVarChar, 50);
				SqlParameter zDate = sqlCommand.Parameters.Add("@z_date", SqlDbType.SmallDateTime);
				SqlParameter zTotal = sqlCommand.Parameters.Add("@z_total", SqlDbType.Float);
				SqlParameter zTotalReturn = sqlCommand.Parameters.Add("@z_total_return", SqlDbType.Float);
				SqlParameter teremokId = sqlCommand.Parameters.Add("@z_teremok_id", SqlDbType.Int);
				value.Value = DBNull.Value;
				zNum.Value = z_num;
				zFile.Value = z_file;
				zDate.Value = z_date;
				zTotal.Value = z_total;
				zTotalReturn.Value = z_total_return;
				teremokId.Value = teremok_id;
				sqlCommand.ExecuteNonQuery();
				num = Convert.ToInt32(value.Value);
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return num;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pc_name">[sp_ZReport]</param>
        /// <param name="z_date"></param>
        /// <param name="z_kkm"></param>
        /// <param name="z_file"></param>
        /// <param name="z_total"></param>
        /// <param name="z_total_return"></param>
        /// <param name="teremok_id"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public DataTable ProcedureExecute_FillTable(string pc_name, Action<SqlCommand> addParams)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(pc_name, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (addParams != null)
                {
                    addParams(sqlCommand);
                }

                SqlDataAdapter _data_adapter = new SqlDataAdapter(sqlCommand);
                DataTable _table = new DataTable("FillTable");
                _data_adapter.Fill(_table);
                return _table;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

		public class YZreport
		{
			public DateTime _date
			{
				get;
				set;
			}

			public string _id
			{
				get;
				set;
			}

			public string _kkm_id
			{
				get;
				set;
			}

			public string _notrest_total
			{
				get;
				set;
			}

			public string _num
			{
				get;
				set;
			}

			public string _num_fiscal
			{
				get;
				set;
			}

			public string _num_turn
			{
				get;
				set;
			}

			public string _teremok_id
			{
				get;
				set;
			}

			public string _teremok_name
			{
				get;
				set;
			}

			public string file_name
			{
				get;
				set;
			}

			public YZreport()
			{
			}

			public override bool Equals(object obj)
			{
				bool flag;
				if (!(obj is CTransfer.YZreport))
				{
					return false;
				}
				CTransfer.YZreport yZreport = (CTransfer.YZreport)obj;
				try
				{
					flag = (int.Parse(this._kkm_id) != int.Parse(yZreport._kkm_id) || int.Parse(this._num_turn) != int.Parse(yZreport._num_turn) ? false : true);
				}
				catch (Exception exception)
				{
					flag = false;
				}
				return flag;
			}
		}
	}
}
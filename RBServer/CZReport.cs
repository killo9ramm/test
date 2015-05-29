using RBServer.Debug_classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace RBServer
{
	internal class CZReport
	{
		public bool dayMinus;

		public DateTime? CurrentZDate = null;

		public CZReport()
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

		public void CheckItemParse(SqlConnection _conn, int z_id, string line, int check_id, DateTime datetime, string check_num, string kkm, string teremok)
		{
			SqlCommand sqlCommand = null;
			string item = null;
			string str = null;
			int num = 0;
			List<string> strs = new List<string>();
			bool flag = false;
			try
			{
				string[] strArrays = line.Split(new char[] { ' ' });
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str1 = strArrays[i];
					if (str1 != "")
					{
						strs.Add(str1);
					}
				}
				string item1 = strs[0];
				string item2 = strs[1];
				string str2 = strs[2];
				string item3 = strs[3];
				item = strs[7];
				if (strs.Count > 8)
				{
					flag = true;
					if (strs[8] != " ")
					{
						if (strs[8] == "$P")
						{
							num = 1;
						}
						str = strs[8];
						if (strs.Count > 9)
						{
							if (strs[9] == "$P")
							{
								num = 1;
							}
							str = string.Concat(strs[8], " ", strs[9]);
							if (strs.Count > 10)
							{
								if (strs[10] == "$P")
								{
									num = 1;
								}
								string[] strArrays1 = new string[] { strs[8], " ", strs[9], " ", strs[10] };
								str = string.Concat(strArrays1);
							}
							if (strs.Count > 11 && strs[11] == "$P")
							{
								num = 1;
							}
						}
					}
				}
				sqlCommand = new SqlCommand()
				{
					Connection = _conn
				};
				string str3 = "INSERT INTO t_CheckItem (ch_mnome_id, ch_zreport_id, ch_check_id, ch_amount1, ch_amount2, ch_count, ch_datetime, ch_type_id, ch_dinner_card, ch_name_user, ch_combo, ch_svb)  VALUES(@ch_mnome_id, @ch_zreport_id, @ch_check_id, @ch_amount1, @ch_amount2, @ch_count, @ch_datetime, @ch_type_id, @ch_dinner_card, @ch_name_user, @ch_combo, @ch_svb)";
				sqlCommand.Parameters.AddWithValue("@ch_mnome_id", item1);
				sqlCommand.Parameters.AddWithValue("@ch_zreport_id", z_id);
				sqlCommand.Parameters.AddWithValue("@ch_check_id", check_id);
				sqlCommand.Parameters.AddWithValue("@ch_amount1", str2);
				sqlCommand.Parameters.AddWithValue("@ch_amount2", item3);
				sqlCommand.Parameters.AddWithValue("@ch_count", item2);
				sqlCommand.Parameters.AddWithValue("@ch_datetime", datetime);
				sqlCommand.Parameters.AddWithValue("@ch_type_id", 1);
				sqlCommand.Parameters.AddWithValue("@ch_dinner_card", item);
				sqlCommand.Parameters.AddWithValue("@ch_combo", num);
				if (!flag)
				{
					sqlCommand.Parameters.AddWithValue("@ch_name_user", " ");
					sqlCommand.Parameters.AddWithValue("@ch_svb", 0);
				}
				else
				{
					sqlCommand.Parameters.AddWithValue("@ch_name_user", str);
					if (str.ToLower() == "карта связной клуб")
					{
						sqlCommand.Parameters.AddWithValue("@ch_svb", 1);
					}
					if (str.ToLower() == "клиент карта связной")
					{
						sqlCommand.Parameters.AddWithValue("@ch_svb", 1);
					}
					if (str.ToLower() != "клиент карта связной" && str.ToLower() != "карта связной клуб")
					{
						sqlCommand.Parameters.AddWithValue("@ch_svb", 0);
					}
				}
				sqlCommand.CommandText = str3;
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		public void CheckItemParseReturn(SqlConnection _conn, int z_id, string line, int check_id, DateTime datetime)
		{
			SqlCommand sqlCommand = null;
			string str = null;
			try
			{
				string str1 = line.Substring(0, 13);
				double num = Convert.ToDouble(CUtilHelper.ParceAmount(line.Substring(13, 12), 1));
				double num1 = Convert.ToDouble(CUtilHelper.ParceAmount(line.Substring(25, 11), 1));
				double num2 = Convert.ToDouble(CUtilHelper.ParceAmount(line.Substring(36, 11), 1));
				str = line.Substring(74, 12);
				sqlCommand = new SqlCommand()
				{
					Connection = _conn
				};
				string str2 = "INSERT INTO t_CheckItem (ch_mnome_id, ch_zreport_id, ch_check_id, ch_amount1, ch_amount2, ch_count, ch_datetime, ch_type_id, ch_dinner_card)  VALUES(@ch_mnome_id, @ch_zreport_id, @ch_check_id, @ch_amount1, @ch_amount2, @ch_count, @ch_datetime, @ch_type_id, @ch_dinner_card)";
				sqlCommand.Parameters.AddWithValue("@ch_mnome_id", str1);
				sqlCommand.Parameters.AddWithValue("@ch_zreport_id", z_id);
				sqlCommand.Parameters.AddWithValue("@ch_check_id", check_id);
				sqlCommand.Parameters.AddWithValue("@ch_amount1", -num1);
				sqlCommand.Parameters.AddWithValue("@ch_amount2", -num2);
				sqlCommand.Parameters.AddWithValue("@ch_count", -num);
				sqlCommand.Parameters.AddWithValue("@ch_datetime", datetime);
				sqlCommand.Parameters.AddWithValue("@ch_type_id", 1);
				sqlCommand.Parameters.AddWithValue("@ch_dinner_card", str);
				sqlCommand.CommandText = str2;
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		public int CheckParse(SqlConnection _conn, int _z_id, string _check_num, string _code_operation, string _amount, DateTime _dt)
		{
			int num;
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = _conn
				};
				string str = "INSERT INTO t_Check (check_z_id, check_datetime, check_num, check_operation_code, check_amount) VALUES(@check_z_id, @check_datetime, @check_num, @check_operation_code, @check_amount)";
				sqlCommand.Parameters.AddWithValue("@check_z_id", _z_id);
				sqlCommand.Parameters.AddWithValue("@check_datetime", _dt);
				sqlCommand.Parameters.AddWithValue("@check_num", _check_num);
				sqlCommand.Parameters.AddWithValue("@check_operation_code", _code_operation);
				sqlCommand.Parameters.AddWithValue("@check_amount", _amount);
				sqlCommand.CommandText = str;
				sqlCommand.ExecuteNonQuery();
				sqlCommand.CommandText = "SELECT @@IDENTITY";
				sqlCommand.Parameters.Clear();
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return num;
		}

		public void CheckParsePayment(SqlConnection _conn, int _z_id, int _check_id, string _check_payment_code, string _payment_amount)
		{
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = _conn,
					CommandText = "INSERT INTO t_CheckPayment (pay_z_id, pay_check_id, pay_payment_code, pay_amount) VALUES (@pay_z_id, @pay_check_id, @pay_payment_code, @pay_amount)"
				};
				sqlCommand.Parameters.Clear();
				sqlCommand.Parameters.AddWithValue("@pay_z_id", _z_id);
				sqlCommand.Parameters.AddWithValue("@pay_check_id", _check_id);
				sqlCommand.Parameters.AddWithValue("@pay_payment_code", _check_payment_code);
				sqlCommand.Parameters.AddWithValue("@pay_amount", _payment_amount);
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		public void CheckParsePaymentReturn(SqlConnection _conn, int _z_id, int _check_id, string _check_payment_code, string _payment_amount)
		{
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = _conn,
					CommandText = "INSERT INTO t_CheckPayment (pay_z_id, pay_check_id, pay_payment_code, pay_amount) VALUES (@pay_z_id, @pay_check_id, @pay_payment_code, @pay_amount)"
				};
				sqlCommand.Parameters.Clear();
				sqlCommand.Parameters.AddWithValue("@pay_z_id", _z_id);
				sqlCommand.Parameters.AddWithValue("@pay_check_id", _check_id);
				sqlCommand.Parameters.AddWithValue("@pay_payment_code", _check_payment_code);
				sqlCommand.Parameters.AddWithValue("@pay_amount", string.Concat("-", _payment_amount));
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private void DeleteCard(SqlConnection conn, string _teremok, string _kkm, DateTime _date, string _referenceNumber, string _authorizationCode, string _cardTotal)
		{
			SqlCommand sqlCommand = null;
			string[] date = new string[] { this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " 0:00:00" };
			string str = string.Concat(date);
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				string[] strArrays = new string[] { "UPDATE t_CardPayment SET card_operation=@card_operation WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", str.ToString(), "' AND card_referenceNumber = '", _referenceNumber, "' AND card_authorizationCode = '", _authorizationCode, "'" };
				string str1 = string.Concat(strArrays);
				sqlCommand.Parameters.Clear();
				sqlCommand.Parameters.AddWithValue("@card_operation", 2);
				sqlCommand.CommandText = str1;
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		public void DXCheckItemParse(SqlConnection _conn, int _dx_id, int _check_id, string line, DateTime datetime)
		{
			SqlCommand sqlCommand = null;
			try
			{
				string str = line.Substring(0, 13);
				double num = Convert.ToDouble(CUtilHelper.ParceAmount(line.Substring(13, 13), 1));
				sqlCommand = new SqlCommand()
				{
					Connection = _conn
				};
				string str1 = "INSERT INTO t_DXCheckItem (c_dxreport_id, c_dxcheck_id, c_mnome_id, c_count, c_datetime)  VALUES(@c_dxreport_id, @c_dxcheck_id, @c_mnome_id, @c_count, @c_datetime)";
				sqlCommand.Parameters.AddWithValue("@c_dxreport_id", _check_id);
				sqlCommand.Parameters.AddWithValue("@c_dxcheck_id", _check_id);
				sqlCommand.Parameters.AddWithValue("@c_mnome_id", str);
				sqlCommand.Parameters.AddWithValue("@c_count", num);
				sqlCommand.Parameters.AddWithValue("@c_datetime", datetime);
				sqlCommand.CommandText = str1;
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		public int DXCheckParse(SqlConnection _conn, int _dx_id, string _check_num)
		{
			int num;
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = _conn
				};
				string str = "INSERT INTO t_DXCheck (dx_dxreport_id, dx_check_num) VALUES(@dx_dxreport_id, @dx_check_num)";
				sqlCommand.Parameters.AddWithValue("@dx_dxreport_id", _dx_id);
				sqlCommand.Parameters.AddWithValue("@dx_check_num", _check_num);
				sqlCommand.CommandText = str;
				sqlCommand.ExecuteNonQuery();
				sqlCommand.CommandText = "SELECT @@IDENTITY";
				sqlCommand.Parameters.Clear();
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return num;
		}

		private void findCard(SqlConnection conn, string _teremok, string _kkm, DateTime _date, string _referenceNumber, string _authorizationCode, string _cardTotal, string _card_number, double _check_total_sum, string teremokName)
		{
			bool flag = false;
			bool flag1 = false;
			bool flag2 = false;
			bool flag3 = false;
			SqlCommand sqlCommand = null;
			string[] date = new string[] { this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " 0:00:00" };
			string.Concat(date);
			CTransfer cTransfer = new CTransfer();
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				string[] str = new string[] { "SELECT count(*) FROM t_Card WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " ", null, null, null, null, null, null, null, null, null, null };
				str[11] = _date.Hour.ToString();
				str[12] = ":";
				str[13] = _date.Minute.ToString();
				str[14] = ":";
				str[15] = _date.Second.ToString();
				str[16] = "' AND card_referenceNumber = '";
				str[17] = _referenceNumber;
				str[18] = "' AND card_authorizationCode = '";
				str[19] = _authorizationCode;
				str[20] = "' AND card_operation = '1'";
				sqlCommand.CommandText = string.Concat(str);
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) != 0)
				{
					flag = true;
				}
				string[] strArrays = new string[] { "SELECT count(*) FROM t_Card WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " ", null, null, null, null, null, null, null, null, null, null };
				strArrays[11] = _date.Hour.ToString();
				strArrays[12] = ":";
				strArrays[13] = _date.Minute.ToString();
				strArrays[14] = ":";
				strArrays[15] = _date.Second.ToString();
				strArrays[16] = "' AND card_referenceNumber = '";
				strArrays[17] = _referenceNumber;
				strArrays[18] = "' AND card_authorizationCode = '";
				strArrays[19] = _authorizationCode;
				strArrays[20] = "' AND card_operation = '2'";
				sqlCommand.CommandText = string.Concat(strArrays);
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) != 0)
				{
					flag1 = true;
				}
				if (flag && flag1)
				{
					string[] str1 = new string[] { "UPDATE t_Card SET card_operation=@card_operation WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " ", null, null, null, null, null, null, null, null, null, null };
					str1[11] = _date.Hour.ToString();
					str1[12] = ":";
					str1[13] = _date.Minute.ToString();
					str1[14] = ":";
					str1[15] = _date.Second.ToString();
					str1[16] = "' AND card_referenceNumber = '";
					str1[17] = _referenceNumber;
					str1[18] = "' AND card_authorizationCode = '";
					str1[19] = _authorizationCode;
					str1[20] = "' AND card_operation = '1'";
					sqlCommand.CommandText = string.Concat(str1);
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@card_operation", 3);
					sqlCommand.ExecuteNonQuery();
					string[] strArrays1 = new string[] { "UPDATE t_Card SET card_operation=@card_operation WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " ", null, null, null, null, null, null, null, null, null, null };
					strArrays1[11] = _date.Hour.ToString();
					strArrays1[12] = ":";
					strArrays1[13] = _date.Minute.ToString();
					strArrays1[14] = ":";
					strArrays1[15] = _date.Second.ToString();
					strArrays1[16] = "' AND card_referenceNumber = '";
					strArrays1[17] = _referenceNumber;
					strArrays1[18] = "' AND card_authorizationCode = '";
					strArrays1[19] = _authorizationCode;
					strArrays1[20] = "' AND card_operation = '2'";
					sqlCommand.CommandText = string.Concat(strArrays1);
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@card_operation", 4);
					sqlCommand.ExecuteNonQuery();
				}
				if (!flag)
				{
					object[] _cardNumber = new object[] { "SELECT count(*) FROM t_Card WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " ", null, null, null, null, null, null, null, null, null, null };
					_cardNumber[11] = _date.Hour.ToString();
					_cardNumber[12] = ":";
					_cardNumber[13] = _date.Minute.ToString();
					_cardNumber[14] = ":";
					_cardNumber[15] = _date.Second.ToString();
					_cardNumber[16] = "' AND card_cardNumber = '";
					_cardNumber[17] = _card_number;
					_cardNumber[18] = "' AND card_operation = '1' AND card_cardTotal = '";
					_cardNumber[19] = _check_total_sum;
					_cardNumber[20] = "'";
					sqlCommand.CommandText = string.Concat(_cardNumber);
					if (Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0)
					{
						flag2 = true;
					}
					else
					{
						string[] _cardNumber1 = new string[] { "UPDATE t_Card SET card_operation=@card_operation WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " ", null, null, null, null, null, null, null, null };
						_cardNumber1[11] = _date.Hour.ToString();
						_cardNumber1[12] = ":";
						_cardNumber1[13] = _date.Minute.ToString();
						_cardNumber1[14] = ":";
						_cardNumber1[15] = _date.Second.ToString();
						_cardNumber1[16] = "' AND card_cardNumber = '";
						_cardNumber1[17] = _card_number;
						_cardNumber1[18] = "' AND card_operation = '1'";
						sqlCommand.CommandText = string.Concat(_cardNumber1);
						sqlCommand.Parameters.Clear();
						sqlCommand.Parameters.AddWithValue("@card_operation", 3);
						sqlCommand.ExecuteNonQuery();
						string[] str2 = new string[] { "UPDATE t_Card SET card_operation=@card_operation WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " ", null, null, null, null, null, null, null, null };
						str2[11] = _date.Hour.ToString();
						str2[12] = ":";
						str2[13] = _date.Minute.ToString();
						str2[14] = ":";
						str2[15] = _date.Second.ToString();
						str2[16] = "' AND card_cardNumber = '";
						str2[17] = _card_number;
						str2[18] = "' AND card_operation = '2'";
						sqlCommand.CommandText = string.Concat(str2);
						sqlCommand.Parameters.Clear();
						sqlCommand.Parameters.AddWithValue("@card_operation", 4);
						sqlCommand.ExecuteNonQuery();
					}
					object[] _checkTotalSum = new object[] { "SELECT count(*) FROM t_Card WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " ", null, null, null, null, null, null, null, null, null, null };
					_checkTotalSum[11] = _date.Hour.ToString();
					_checkTotalSum[12] = ":";
					_checkTotalSum[13] = _date.Minute.ToString();
					_checkTotalSum[14] = ":";
					_checkTotalSum[15] = _date.Second.ToString();
					_checkTotalSum[16] = "' AND card_cardNumber = '";
					_checkTotalSum[17] = _card_number;
					_checkTotalSum[18] = "' AND card_operation = '3' AND card_cardTotal = '";
					_checkTotalSum[19] = _check_total_sum;
					_checkTotalSum[20] = "'";
					sqlCommand.CommandText = string.Concat(_checkTotalSum);
					if (Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0)
					{
						flag3 = true;
					}
					if (flag2 && flag3)
					{
						object[] objArray = new object[] { "Теремок: ", teremokName, "\n Касса: ", _kkm, "\n Дата: ", _date, "\n referenceNumber: ", _referenceNumber, "\n authorizationCode: ", _authorizationCode, "\n Сумма чека: ", _cardTotal, "\n Номер карты: ", _card_number };
						string str3 = string.Concat(objArray);
						cTransfer.SendMail(cTransfer._config.email_rbs_err_return, "Не найдена продажа по возврату ", str3, false);
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				DebugPanel.OnErrorOccured(string.Concat("error: ", exception.Message, " ", DebugPanel.InMethod(0, 200)));
				throw exception;
			}
		}

		private void fixZDate(SqlConnection conn, string _znum, string _kkm, DateTime _date, string teremok_id, int countDay)
		{
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				object[] objArray = new object[] { "UPDATE t_ZReport SET z_date=@z_date WHERE z_num = '", _znum, "' AND z_teremok_id = '", teremok_id, "' AND z_kkm_id = '", Convert.ToInt32(_kkm), "' AND z_date = '", this.AttachZeroToDate(_date.Year), ".", this.AttachZeroToDate(_date.Month), ".", this.AttachZeroToDate(_date.Day), " 0:00:00'" };
				sqlCommand.CommandText = string.Concat(objArray);
				sqlCommand.Parameters.Clear();
				sqlCommand.Parameters.AddWithValue("@z_date", _date.AddDays((double)(-countDay)));
				sqlCommand.ExecuteNonQuery();
			}
			catch
			{
			}
		}

		internal DateTime getDateFromZreportCheckFirstString(string fileName)
		{
			DateTime dateTime;
			string str = RBServer.Debug_classes.File.ReadAllText(fileName, Encoding.GetEncoding(1251));
			//Regex regex = new Regex("(?<!3 )([0-9]{2}[.]{1}[0-9]{2}[.]{1}[0-9]{2}) ([0-9]{2}[:]{1}[0-9]{2}[:]{1}[0-9]{2}) 1");
            Regex regex = new Regex(@"(?<!3 )([0-9]{2}[.]{1}[0-9]{2}[.]{1}[0-9]{2}) ([0-9]{2}[:]{1}[0-9]{2}[:]{1}[0-9]{2}) \d");
			try
			{
				if (!regex.IsMatch(str))
				{
					RBServer.Debug_classes.FileInfo fileInfo = new RBServer.Debug_classes.FileInfo(fileName);
					dateTime = new DateTime(Convert.ToInt32(string.Concat("20", fileInfo.Name.Substring(1, 2))), Convert.ToInt32(fileInfo.Name.Substring(3, 2)), Convert.ToInt32(fileInfo.Name.Substring(5, 2)));
				}
				else
				{
					string value = regex.Match(str).Groups[1].Value;
					dateTime = DateTime.Parse(value);
				}
			}
			catch (Exception exception)
			{
				CTransfer cTransfer = new CTransfer();
				DateTime now = DateTime.Now;
				cTransfer.Log(string.Concat("Ошибка разбора пустого z-отчета проставлена дата ", now.ToString(), " имя файла ", fileName), 0);
				DebugPanel.Log(string.Concat("Ошибка разбора пустого z-отчета проставлена дата ", now.ToString(), " имя файла ", fileName));
				dateTime = now;
			}
			return dateTime;
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

		public bool IsDXReportSent(string file_name, string teremok_folder, SqlConnection conn)
		{
			bool flag;
			int num = 0;
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				string str = "SELECT dx_id FROM t_DXReport WHERE dx_file=@dx_file AND dx_teremok_id=@dx_teremok_id";
				sqlCommand.Parameters.AddWithValue("@dx_file", file_name);
				sqlCommand.Parameters.AddWithValue("@dx_teremok_id", this.GetTeremokID(teremok_folder, conn));
				sqlCommand.CommandText = str;
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
				if (num != 0)
				{
					str = "DELETE FROM t_DXCheckItem WHERE c_dxreport_id=@c_dxreport_id";
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@c_dxreport_id", num);
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
					str = "DELETE FROM t_DXCheck WHERE dx_dxreport_id = @dx_dxreport_id";
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@dx_dxreport_id", num);
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
					str = "DELETE FROM t_DXReport WHERE dx_id = @dx_id";
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@dx_id", num);
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

		public bool IsYReportSent(string file_name, string teremok_folder, SqlConnection conn)
		{
			bool flag;
			int num = 0;
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				string str = "SELECT y_id FROM t_YReport WHERE y_num_turn=@y_num_turn AND y_teremok_id=@y_teremok_id AND y_date = @y_date";
				DateTime dateTime = new DateTime(Convert.ToInt32(string.Concat("20", file_name.Substring(1, 2))), Convert.ToInt32(file_name.Substring(3, 2)), Convert.ToInt32(file_name.Substring(5, 2)));
				string[] strArrays = file_name.Split(new char[] { '.' });
				sqlCommand.Parameters.AddWithValue("@y_num_turn", strArrays[1]);
				sqlCommand.Parameters.AddWithValue("@y_teremok_id", this.GetTeremokID(teremok_folder, conn));
				sqlCommand.Parameters.AddWithValue("@y_date", dateTime);
				sqlCommand.CommandText = str;
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
				if (num != 0)
				{
					SqlCommand sqlCommand1 = new SqlCommand()
					{
						Connection = conn
					};
					string str1 = "SELECT y_1c_load FROM t_YReport WHERE y_num_turn=@y_num_turn AND y_teremok_id=@y_teremok_id AND y_date = @y_date";
					dateTime = new DateTime(Convert.ToInt32(string.Concat("20", file_name.Substring(1, 2))), Convert.ToInt32(file_name.Substring(3, 2)), Convert.ToInt32(file_name.Substring(5, 2)));
					string[] strArrays1 = file_name.Split(new char[] { '.' });
					sqlCommand1.Parameters.AddWithValue("@y_num_turn", strArrays1[1]);
					sqlCommand1.Parameters.AddWithValue("@y_teremok_id", this.GetTeremokID(teremok_folder, conn));
					sqlCommand1.Parameters.AddWithValue("@y_date", dateTime);
					sqlCommand1.CommandText = str1;
					if (Convert.ToInt32(sqlCommand1.ExecuteScalar()) != 0)
					{
						flag = true;
						return flag;
					}
					else
					{
						str = "DELETE FROM t_YReport WHERE y_id = @y_id";
						sqlCommand.Parameters.Clear();
						sqlCommand.Parameters.AddWithValue("@y_id", num);
						sqlCommand.CommandText = str;
						sqlCommand.ExecuteNonQuery();
					}
				}
				flag = false;
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return flag;
		}

		public bool IsZReportSent(string file_name, string teremok_folder, SqlConnection conn)
		{
			bool flag;
			int num = 0;
			int num1 = 0;
			try
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				sqlCommand.Parameters.Clear();
				string str = "SELECT z_id FROM t_ZReport WHERE z_file=@z_file AND z_teremok_id=@z_teremok_id";
				sqlCommand.Parameters.AddWithValue("@z_file", file_name);
				sqlCommand.Parameters.AddWithValue("@z_teremok_id", this.GetTeremokID(teremok_folder, conn));
				sqlCommand.CommandText = str;
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
				string str1 = "SELECT z_1c_load FROM t_ZReport WHERE z_file=@z_file AND z_teremok_id=@z_teremok_id";
				sqlCommand.Parameters.Clear();
				sqlCommand.Parameters.AddWithValue("@z_file", file_name);
				sqlCommand.Parameters.AddWithValue("@z_teremok_id", this.GetTeremokID(teremok_folder, conn));
				sqlCommand.CommandText = str1;
				object obj = sqlCommand.ExecuteScalar();
				if (obj != null)
				{
					int.TryParse(obj.ToString(), out num1);
				}
				if (num != 0)
				{
					if (num1 == 0)
					{
						str = "DELETE FROM t_CheckItem WHERE ch_zreport_id=@ch_zreport_id";
						sqlCommand.Parameters.Clear();
						sqlCommand.Parameters.AddWithValue("@ch_zreport_id", num);
						sqlCommand.CommandText = str;
						sqlCommand.ExecuteNonQuery();
						str = "DELETE FROM t_Check WHERE check_z_id = @check_z_id";
						sqlCommand.Parameters.Clear();
						sqlCommand.Parameters.AddWithValue("@check_z_id", num);
						sqlCommand.CommandText = str;
						sqlCommand.ExecuteNonQuery();
						str = "DELETE FROM t_ZReport WHERE z_id = @z_id";
						sqlCommand.Parameters.Clear();
						sqlCommand.Parameters.AddWithValue("@z_id", num);
						sqlCommand.CommandText = str;
						sqlCommand.ExecuteNonQuery();
					}
					if (num1 == 1)
					{
						flag = true;
						return flag;
					}
				}
				flag = false;
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return flag;
		}

		private int loadNewSVB(SqlConnection _conn, string check_num, DateTime datetime, string teremok, string kkm)
		{
			int num;
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = _conn
				};
				string[] checkNum = new string[] { "SELECT count(*) FROM t_SvayznoiCheck WHERE s_check_num = '", check_num, "' AND s_check_date = '", this.AttachZeroToDate(datetime.Year), ".", this.AttachZeroToDate(datetime.Month), ".", this.AttachZeroToDate(datetime.Day), " ", null, null, null, null, null, null, null, null, null, null };
				checkNum[9] = datetime.Hour.ToString();
				checkNum[10] = ":";
				checkNum[11] = datetime.Minute.ToString();
				checkNum[12] = ":";
				checkNum[13] = datetime.Second.ToString();
				checkNum[14] = "' AND s_kkm = '";
				checkNum[15] = kkm;
				checkNum[16] = "' AND s_teremok = '";
				checkNum[17] = teremok;
				checkNum[18] = "'";
				sqlCommand.CommandText = string.Concat(checkNum);
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0)
				{
					string str = "INSERT INTO t_SvayznoiCheck (s_check_num, s_check_date, s_teremok, s_kkm) VALUES(@s_check_num, @s_check_date, @s_teremok, @s_kkm)";
					sqlCommand.Parameters.AddWithValue("@s_check_num", check_num);
					sqlCommand.Parameters.AddWithValue("@s_check_date", datetime);
					sqlCommand.Parameters.AddWithValue("@s_teremok", teremok);
					sqlCommand.Parameters.AddWithValue("@s_kkm", kkm);
					sqlCommand.CommandText = str;
					sqlCommand.ExecuteNonQuery();
				}
				sqlCommand.CommandText = "SELECT @@IDENTITY";
				sqlCommand.Parameters.Clear();
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return num;
		}

		private void loadSva(SqlConnection _conn, string _menu_1C, string check_num, string _amount1, string _amount2, string _quantity, DateTime datetime, string _dinner_card, string kkm, string teremok)
		{
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = _conn
				};
				string[] checkNum = new string[] { "SELECT count(*) FROM t_Svayznoi WHERE s_check_num = '", check_num, "' AND s_datetime = '", this.AttachZeroToDate(datetime.Year), ".", this.AttachZeroToDate(datetime.Month), ".", this.AttachZeroToDate(datetime.Day), " ", null, null, null, null, null, null, null, null };
				checkNum[9] = datetime.Hour.ToString();
				checkNum[10] = ":";
				checkNum[11] = datetime.Minute.ToString();
				checkNum[12] = ":";
				checkNum[13] = datetime.Second.ToString();
				checkNum[14] = "' AND s_kkm = '";
				checkNum[15] = kkm;
				checkNum[16] = "'";
				string str = string.Concat(checkNum);
				sqlCommand.CommandText = str;
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0)
				{
					str = "INSERT INTO t_Svayznoi (s_mnome_id, s_check_num, s_amount1, s_amount2, s_count, s_datetime, s_dinner_card, s_load, s_kkm, s_teremok)  VALUES(@s_mnome_id, @s_check_num, @s_amount1, @s_amount2, @s_count, @s_datetime, @s_dinner_card, @s_load, @s_kkm, @s_teremok)";
					sqlCommand.Parameters.AddWithValue("@s_mnome_id", _menu_1C);
					sqlCommand.Parameters.AddWithValue("@s_check_num", check_num);
					sqlCommand.Parameters.AddWithValue("@s_amount1", _amount1);
					sqlCommand.Parameters.AddWithValue("@s_amount2", _amount2);
					sqlCommand.Parameters.AddWithValue("@s_count", _quantity);
					sqlCommand.Parameters.AddWithValue("@s_datetime", datetime);
					sqlCommand.Parameters.AddWithValue("@s_dinner_card", _dinner_card);
					sqlCommand.Parameters.AddWithValue("@s_load", 0);
					sqlCommand.Parameters.AddWithValue("@s_kkm", kkm);
					sqlCommand.Parameters.AddWithValue("@s_teremok", teremok);
				}
				sqlCommand.CommandText = str;
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		internal DateTime? make_date_from_fileName(string fileName)
		{
			DateTime? nullable;
			try
			{
				DateTime? nullable1 = null;
				Regex regex = new Regex("(?i:x)(\\d{2})(\\d{2})(\\d{2})");
				if (regex.IsMatch(fileName))
				{
					string str = string.Concat("20", regex.Match(fileName).Groups[1].Value);
					string value = regex.Match(fileName).Groups[2].Value;
					string value1 = regex.Match(fileName).Groups[3].Value;
					nullable1 = new DateTime?(new DateTime(int.Parse(str), int.Parse(value), int.Parse(value1)));
				}
				nullable = nullable1;
			}
			catch (Exception exception)
			{
				DebugPanel.Log(string.Concat("Ошибка формирования даты для проверки z-отчета. Ошибка: ", exception.Message));
				nullable = null;
			}
			return nullable;
		}

		private string MyTrim(string str)
		{
			string str1 = "";
			bool flag = false;
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i].ToString() != " ")
				{
					str1 = string.Concat(str1, str[i]);
					flag = false;
				}
				else if (!flag)
				{
					str1 = string.Concat(str1, str[i]);
					flag = true;
				}
			}
			return str1.Trim();
		}

		private int MyTrim1(string str)
		{
			string str1 = "";
			bool flag = false;
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i].ToString() != " ")
				{
					str1 = string.Concat(str1, str[i]);
					flag = false;
				}
				else if (!flag)
				{
					str1 = string.Concat(str1, str[i]);
					flag = true;
				}
			}
			return Convert.ToInt32(str1.Trim());
		}

		private int quantityCheck(SqlConnection conn, int _z_id)
		{
			int num;
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn,
					CommandText = string.Concat("SELECT COUNT(DISTINCT dbo.t_Check.check_num) FROM dbo.t_ZReport LEFT JOIN dbo.t_Check ON dbo.t_ZReport.z_id = dbo.t_Check.check_z_id LEFT JOIN  dbo.t_Teremok ON dbo.t_ZReport.z_teremok_id = dbo.t_Teremok.teremok_id LEFT JOIN dbo.t_CheckPayment ON dbo.t_Check.check_id = dbo.t_CheckPayment.pay_check_id  WHERE (dbo.t_ZReport.z_id = '", _z_id, "') AND (dbo.t_Check.check_operation_code = 1) AND (dbo.t_CheckPayment.pay_payment_code <> 4)")
				};
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return num;
		}

		private string return_count_check_sum(int z_id, SqlConnection conn)
		{
			string str;
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				string str1 = string.Concat("SELECT SUM(check_amount) FROM dbo.t_Check WHERE check_z_id =", z_id, " AND check_num NOT LIKE '%/000003' AND check_operation_code = 1");
				sqlCommand.Parameters.AddWithValue("@check_z_id", z_id);
				sqlCommand.CommandText = str1;
				string str2 = sqlCommand.ExecuteScalar().ToString();
				if (str2 == "")
				{
					str2 = "0";
				}
				str = str2;
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return str;
		}

		private string return_count_dinner_sum(int z_id, SqlConnection conn)
		{
			string str;
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				string str1 = string.Concat("SELECT SUM(check_amount) FROM dbo.t_Check WHERE check_z_id =", z_id, " AND check_num LIKE '%/000003' AND check_operation_code = 1");
				sqlCommand.Parameters.AddWithValue("@check_z_id", z_id);
				sqlCommand.CommandText = str1;
				string str2 = sqlCommand.ExecuteScalar().ToString();
				if (str2 == "")
				{
					str2 = "0";
				}
				str = str2;
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return str;
		}

		private void updateCheck(SqlConnection conn, int _countCheck, int _z_id)
		{
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn,
					CommandText = string.Concat("UPDATE t_ZReport SET z_quantity_check=@z_quantity_check WHERE z_id = ", _z_id)
				};
				sqlCommand.Parameters.Clear();
				sqlCommand.Parameters.AddWithValue("@z_quantity_check", _countCheck);
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private void writeCard(SqlConnection conn, int _check_id, int _z_id, DateTime _check_date, string _cardNumber, string _referenceNumber, string _authorizationCode, string _cardType, string _cardTotal, string _kkm, string _teremok, string _terminalID, int _card_operation)
		{
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				string[] str = new string[] { "SELECT count(*) FROM t_CardPayment WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_check_date.Year), ".", this.AttachZeroToDate(_check_date.Month), ".", this.AttachZeroToDate(_check_date.Day), " ", null, null, null, null, null, null, null, null, null, null };
				str[11] = _check_date.Hour.ToString();
				str[12] = ":";
				str[13] = _check_date.Minute.ToString();
				str[14] = ":";
				str[15] = _check_date.Second.ToString();
				str[16] = "' AND card_referenceNumber = '";
				str[17] = _referenceNumber;
				str[18] = "' AND card_authorizationCode = '";
				str[19] = _authorizationCode;
				str[20] = "'";
				sqlCommand.CommandText = string.Concat(str);
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) != 0)
				{
					string[] strArrays = new string[] { "UPDATE t_CardPayment SET card_check_id=@card_check_id, card_z_id=@card_z_id WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_check_date.Year), ".", this.AttachZeroToDate(_check_date.Month), ".", this.AttachZeroToDate(_check_date.Day), " ", null, null, null, null, null, null, null, null, null, null };
					strArrays[11] = _check_date.Hour.ToString();
					strArrays[12] = ":";
					strArrays[13] = _check_date.Minute.ToString();
					strArrays[14] = ":";
					strArrays[15] = _check_date.Second.ToString();
					strArrays[16] = "' AND card_referenceNumber = '";
					strArrays[17] = _referenceNumber;
					strArrays[18] = "' AND card_authorizationCode = '";
					strArrays[19] = _authorizationCode;
					strArrays[20] = "'";
					sqlCommand.CommandText = string.Concat(strArrays);
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@card_check_id", _check_id);
					sqlCommand.Parameters.AddWithValue("@card_z_id", _z_id);
					sqlCommand.ExecuteNonQuery();
				}
				else
				{
					sqlCommand = new SqlCommand()
					{
						Connection = conn
					};
					string str1 = "INSERT INTO t_CardPayment (card_check_id, card_z_id, card_date, card_cardNumber, card_referenceNumber, card_authorizationCode, card_cardType, card_cardTotal, card_kkm, card_teremok, card_repayment, card_terminal_id, card_operation) VALUES (@card_check_id, @card_z_id, @card_date, @card_cardNumber, @card_referenceNumber, @card_authorizationCode, @card_cardType, @card_cardTotal, @card_kkm, @card_teremok, @card_repayment, @card_terminal_id, @card_operation)";
					sqlCommand.Parameters.AddWithValue("@card_check_id", _check_id);
					sqlCommand.Parameters.AddWithValue("@card_z_id", _z_id);
					sqlCommand.Parameters.AddWithValue("@card_date", _check_date);
					sqlCommand.Parameters.AddWithValue("@card_cardNumber", _cardNumber);
					sqlCommand.Parameters.AddWithValue("@card_referenceNumber", _referenceNumber);
					sqlCommand.Parameters.AddWithValue("@card_authorizationCode", _authorizationCode);
					sqlCommand.Parameters.AddWithValue("@card_cardType", _cardType);
					sqlCommand.Parameters.AddWithValue("@card_cardTotal", _cardTotal);
					sqlCommand.Parameters.AddWithValue("@card_kkm", Convert.ToInt32(_kkm));
					sqlCommand.Parameters.AddWithValue("@card_teremok", Convert.ToInt32(_teremok));
					sqlCommand.Parameters.AddWithValue("@card_repayment", 0);
					sqlCommand.Parameters.AddWithValue("@card_terminal_id", _terminalID);
					sqlCommand.Parameters.AddWithValue("@card_operation", _card_operation);
					sqlCommand.CommandText = str1;
					sqlCommand.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		private void writeCardNew(SqlConnection conn, int _check_id, int _z_id, DateTime _check_date, string _cardNumber, string _referenceNumber, string _authorizationCode, string _cardType, string _cardTotal, string _kkm, string _teremok, string _terminalID, int _card_operation)
		{
			SqlCommand sqlCommand = null;
			int _cardOperation = _card_operation + 2;
			bool flag = false;
			bool flag1 = false;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = conn
				};
				object[] objArray = new object[] { "SELECT count(*) FROM t_Card WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_check_date.Year), ".", this.AttachZeroToDate(_check_date.Month), ".", this.AttachZeroToDate(_check_date.Day), " ", _check_date.Hour.ToString(), ":", _check_date.Minute.ToString(), ":", _check_date.Second.ToString(), "' AND card_referenceNumber = '", _referenceNumber, "' AND card_authorizationCode = '", _authorizationCode, "' AND card_operation = '", _card_operation, "'" };
				sqlCommand.CommandText = string.Concat(objArray);
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) != 0)
				{
					string[] str = new string[] { "UPDATE t_Card SET card_check_id=@card_check_id, card_z_id=@card_z_id WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_check_date.Year), ".", this.AttachZeroToDate(_check_date.Month), ".", this.AttachZeroToDate(_check_date.Day), " ", null, null, null, null, null, null, null, null, null, null };
					str[11] = _check_date.Hour.ToString();
					str[12] = ":";
					str[13] = _check_date.Minute.ToString();
					str[14] = ":";
					str[15] = _check_date.Second.ToString();
					str[16] = "' AND card_referenceNumber = '";
					str[17] = _referenceNumber;
					str[18] = "' AND card_authorizationCode = '";
					str[19] = _authorizationCode;
					str[20] = "'";
					sqlCommand.CommandText = string.Concat(str);
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@card_check_id", _check_id);
					sqlCommand.Parameters.AddWithValue("@card_z_id", _z_id);
					sqlCommand.ExecuteNonQuery();
				}
				else
				{
					flag1 = true;
				}
				object[] objArray1 = new object[] { "SELECT count(*) FROM t_Card WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_check_date.Year), ".", this.AttachZeroToDate(_check_date.Month), ".", this.AttachZeroToDate(_check_date.Day), " ", _check_date.Hour.ToString(), ":", _check_date.Minute.ToString(), ":", _check_date.Second.ToString(), "' AND card_referenceNumber = '", _referenceNumber, "' AND card_authorizationCode = '", _authorizationCode, "' AND card_operation = '", _cardOperation, "'" };
				sqlCommand.CommandText = string.Concat(objArray1);
				if (Convert.ToInt32(sqlCommand.ExecuteScalar()) != 0)
				{
					string[] strArrays = new string[] { "UPDATE t_Card SET card_check_id=@card_check_id, card_z_id=@card_z_id WHERE card_teremok = '", _teremok, "' AND card_kkm = '", _kkm, "' AND card_date = '", this.AttachZeroToDate(_check_date.Year), ".", this.AttachZeroToDate(_check_date.Month), ".", this.AttachZeroToDate(_check_date.Day), " ", null, null, null, null, null, null, null, null, null, null };
					strArrays[11] = _check_date.Hour.ToString();
					strArrays[12] = ":";
					strArrays[13] = _check_date.Minute.ToString();
					strArrays[14] = ":";
					strArrays[15] = _check_date.Second.ToString();
					strArrays[16] = "' AND card_referenceNumber = '";
					strArrays[17] = _referenceNumber;
					strArrays[18] = "' AND card_authorizationCode = '";
					strArrays[19] = _authorizationCode;
					strArrays[20] = "'";
					sqlCommand.CommandText = string.Concat(strArrays);
					sqlCommand.Parameters.Clear();
					sqlCommand.Parameters.AddWithValue("@card_check_id", _check_id);
					sqlCommand.Parameters.AddWithValue("@card_z_id", _z_id);
					sqlCommand.ExecuteNonQuery();
				}
				else
				{
					flag = true;
				}
				if (flag && flag1)
				{
					sqlCommand = new SqlCommand()
					{
						Connection = conn
					};
					string str1 = "INSERT INTO t_Card(card_check_id, card_z_id, card_date, card_cardNumber, card_referenceNumber, card_authorizationCode, card_cardType, card_cardTotal, card_kkm, card_teremok, card_repayment, card_terminal_id, card_operation) VALUES (@card_check_id, @card_z_id, @card_date, @card_cardNumber, @card_referenceNumber, @card_authorizationCode, @card_cardType, @card_cardTotal, @card_kkm, @card_teremok, @card_repayment, @card_terminal_id, @card_operation)";
					sqlCommand.Parameters.AddWithValue("@card_check_id", _check_id);
					sqlCommand.Parameters.AddWithValue("@card_z_id", _z_id);
					sqlCommand.Parameters.AddWithValue("@card_date", _check_date);
					sqlCommand.Parameters.AddWithValue("@card_cardNumber", _cardNumber);
					sqlCommand.Parameters.AddWithValue("@card_referenceNumber", _referenceNumber);
					sqlCommand.Parameters.AddWithValue("@card_authorizationCode", _authorizationCode);
					sqlCommand.Parameters.AddWithValue("@card_cardType", _cardType);
					sqlCommand.Parameters.AddWithValue("@card_cardTotal", _cardTotal);
					sqlCommand.Parameters.AddWithValue("@card_kkm", Convert.ToInt32(_kkm));
					sqlCommand.Parameters.AddWithValue("@card_teremok", Convert.ToInt32(_teremok));
					sqlCommand.Parameters.AddWithValue("@card_repayment", 0);
					sqlCommand.Parameters.AddWithValue("@card_terminal_id", _terminalID);
					sqlCommand.Parameters.AddWithValue("@card_operation", _card_operation);
					sqlCommand.CommandText = str1;
					sqlCommand.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		public void YReportParse(RBServer.Debug_classes.FileInfo file, string teremok_folder, SqlConnection conn)
		{
			DateTime dateTime;
			SqlCommand sqlCommand = null;
			RBServer.Debug_classes.StreamReader streamReader = null;
			int num = 0;
			List<string> strs = new List<string>();
			try
			{
				try
				{
					streamReader = new RBServer.Debug_classes.StreamReader(file.FullName, Encoding.GetEncoding(1251));
					while (true)
					{
						string str = streamReader.ReadLine();
						string str1 = str;
						if (str == null)
						{
							break;
						}
						this.MyTrim(str1);
						if (str1 != "")
						{
							strs.Add(str1);
						}
						num++;
						if (num == 5)
						{
							sqlCommand = new SqlCommand()
							{
								Connection = conn
							};
							string teremokID = this.GetTeremokID(teremok_folder, conn);
							CTransfer.YZreport yreportCl = CTransfer.yreport_cl;
							yreportCl._kkm_id = strs[0];
							yreportCl._num = strs[3];
							yreportCl._num_fiscal = strs[2];
							yreportCl._notrest_total = strs[4];
							yreportCl._num_turn = file.Extension.Replace(".", "");
							yreportCl._teremok_id = teremokID;
							yreportCl.file_name = file.FullName;
							if (CTransfer.zreport_cl == null || !CTransfer.zreport_cl.Equals(yreportCl))
							{
								dateTime = new DateTime(Convert.ToInt32(string.Concat("20", file.Name.Substring(1, 2))), Convert.ToInt32(file.Name.Substring(3, 2)), Convert.ToInt32(file.Name.Substring(5, 2)));
								if (this.dayMinus)
								{
									dateTime = dateTime.AddDays(-1);
								}
							}
							else
							{
								dateTime = CTransfer.zreport_cl._date;
							}
							yreportCl._date = dateTime;
							string str2 = "SELECT y_id FROM t_YReport WHERE y_file_name = @y_file_name AND y_num_z=@y_num_z AND y_teremok_id=@y_teremok_id AND y_date = @y_date AND y_num_kkm = @y_num_kkm";
							sqlCommand.Parameters.AddWithValue("@y_file_name", file.Name);
							sqlCommand.Parameters.AddWithValue("@y_num_z", strs[3]);
							sqlCommand.Parameters.AddWithValue("@y_teremok_id", this.GetTeremokID(teremok_folder, conn));
							sqlCommand.Parameters.AddWithValue("@y_date", dateTime);
							sqlCommand.Parameters.AddWithValue("@y_num_kkm", strs[0]);
							sqlCommand.CommandText = str2;
							if (Convert.ToInt32(sqlCommand.ExecuteScalar()) != 0)
							{
								object[] item = new object[] { "UPDATE t_YReport SET y_file_name = @y_file_name, y_num_turn = @y_num_turn, y_num_fiscal=@y_num_fiscal, y_notreset_total = @y_notreset_total, y_type=@y_type WHERE y_num_z = '", strs[3], "' AND y_num_kkm = '", Convert.ToInt32(strs[0]), "' AND y_teremok_id = '", teremokID, "'" };
								sqlCommand.CommandText = string.Concat(item);
								sqlCommand.Parameters.Clear();
								sqlCommand.Parameters.AddWithValue("@y_file_name", file.Name);
								sqlCommand.Parameters.AddWithValue("@y_num_turn", strs[1]);
								sqlCommand.Parameters.AddWithValue("@y_num_fiscal", strs[2]);
								sqlCommand.Parameters.AddWithValue("@y_notreset_total", strs[4]);
								sqlCommand.Parameters.AddWithValue("@y_type", 0);
								sqlCommand.ExecuteNonQuery();
								object[] objArray = new object[] { "UPDATE t_ZReport SET z_num_fiscal=@z_num_fiscal WHERE z_num = '", strs[3], "' AND z_teremok_id = '", teremokID, "' AND z_kkm_id = '", Convert.ToInt32(strs[0]), "' AND z_date = '", this.AttachZeroToDate(dateTime.Year), ".", this.AttachZeroToDate(dateTime.Month), ".", this.AttachZeroToDate(dateTime.Day), " 0:00:00'" };
								sqlCommand.CommandText = string.Concat(objArray);
								sqlCommand.Parameters.Clear();
								sqlCommand.Parameters.AddWithValue("@z_num_fiscal", strs[2]);
								sqlCommand.ExecuteNonQuery();
							}
							else
							{
								sqlCommand.CommandText = "INSERT INTO t_YReport (y_file_name,  y_date,  y_num_kkm,  y_num_turn,  y_num_fiscal,  y_num_z,  y_notreset_total,  y_teremok_id,  y_type)VALUES (@y_file_name, @y_date, @y_num_kkm, @y_num_turn, @y_num_fiscal, @y_num_z, @y_notreset_total, @y_teremok_id, @y_type)";
								sqlCommand.Parameters.Clear();
								sqlCommand.Parameters.AddWithValue("@y_file_name", file.Name);
								sqlCommand.Parameters.AddWithValue("@y_date", dateTime);
								sqlCommand.Parameters.AddWithValue("@y_num_kkm", strs[0]);
								sqlCommand.Parameters.AddWithValue("@y_num_turn", strs[1]);
								sqlCommand.Parameters.AddWithValue("@y_num_fiscal", strs[2]);
								sqlCommand.Parameters.AddWithValue("@y_num_z", strs[3]);
								sqlCommand.Parameters.AddWithValue("@y_notreset_total", strs[4]);
								sqlCommand.Parameters.AddWithValue("@y_teremok_id", teremokID);
								sqlCommand.Parameters.AddWithValue("@y_type", 0);
								sqlCommand.ExecuteNonQuery();
								object[] item1 = new object[] { "UPDATE t_ZReport SET z_num_fiscal=@z_num_fiscal WHERE z_num = '", strs[3], "' AND z_teremok_id = '", teremokID, "' AND z_kkm_id = '", Convert.ToInt32(strs[0]), "' AND z_date = '", this.AttachZeroToDate(dateTime.Year), ".", this.AttachZeroToDate(dateTime.Month), ".", this.AttachZeroToDate(dateTime.Day), " 0:00:00'" };
								sqlCommand.CommandText = string.Concat(item1);
								sqlCommand.Parameters.Clear();
								sqlCommand.Parameters.AddWithValue("@z_num_fiscal", strs[2]);
								sqlCommand.ExecuteNonQuery();
							}
						}
					}
				}
				catch (Exception exception)
				{
					throw exception;
				}
			}
			finally
			{
				if (streamReader != null)
				{
					streamReader.Close();
					streamReader.Dispose();
				}
				this.dayMinus = false;
			}
		}

		public int ZDXReportAdd(SqlConnection _conn, DateTime z_date, string z_kkm, string z_file, string teremok_id)
		{
			int num;
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = _conn,
					CommandText = "INSERT INTO t_DXReport(dx_file, dx_date, dx_kkm_id, dx_teremok_id) VALUES (@dx_file, @dx_date, @dx_kkm_id, @dx_teremok_id)"
				};
				sqlCommand.Parameters.Clear();
				sqlCommand.Parameters.AddWithValue("@dx_file", z_file);
				sqlCommand.Parameters.AddWithValue("@dx_date", z_date);
				sqlCommand.Parameters.AddWithValue("@dx_kkm_id", z_kkm);
				sqlCommand.Parameters.AddWithValue("@dx_teremok_id", teremok_id);
				sqlCommand.ExecuteNonQuery();
				sqlCommand.CommandText = "SELECT @@IDENTITY";
				sqlCommand.Parameters.Clear();
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return num;
		}

		public void ZDXReportParse(RBServer.Debug_classes.FileInfo file, string teremok_folder, SqlConnection conn)
		{
			RBServer.Debug_classes.StreamReader streamReader = null;
			try
			{
				try
				{
					char[] charArray = " ".ToCharArray();
					string str = null;
					bool flag = false;
					int num = 0;
					int num1 = 0;
					DateTime now = DateTime.Now;
					streamReader = new RBServer.Debug_classes.StreamReader(file.FullName, Encoding.GetEncoding(1251));
					int num2 = 0;
					while (true)
					{
						string str1 = streamReader.ReadLine();
						string str2 = str1;
						if (str1 == null)
						{
							break;
						}
						string str3 = this.MyTrim(str2);
						string[] strArrays = str3.Split(charArray);
						string str4 = strArrays[0];
						num2++;
						string str5 = str4;
						string str6 = str5;
						if (str5 != null)
						{
							if (str6 == "Касса")
							{
								str = strArrays[(int)strArrays.Length - 1];
								continue;
							}
							else if (str6 == "Смена")
							{
								string str7 = strArrays[(int)strArrays.Length - 1];
								DateTime dateTime = new DateTime(Convert.ToInt32(string.Concat("20", file.Name.Substring(2, 2))), Convert.ToInt32(file.Name.Substring(4, 2)), Convert.ToInt32(file.Name.Substring(6, 2)));
								num = this.ZDXReportAdd(conn, dateTime, str, file.Name, this.GetTeremokID(teremok_folder, conn));
								continue;
							}
							else if (str6 == "Чек")
							{
								flag = true;
								strArrays = str3.Split(charArray);
								str4 = strArrays[0];
								num1 = this.DXCheckParse(conn, num, strArrays[1]);
								continue;
							}
							else if (str6 == "")
							{
								continue;
							}
						}
						if (flag)
						{
							this.DXCheckItemParse(conn, num, num1, str2, now);
						}
					}
				}
				catch (Exception exception)
				{
					throw exception;
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

		public int ZReportAdd(SqlConnection _conn, string z_num, DateTime z_date, string z_kkm, string z_file, double z_total, double z_total_return, double z_cash, double z_cash_return, double z_card, double z_card_return, double z_cupon, double z_cupon_return, double z_dinner, double z_dinner_return, string teremok_id, DateTime dateLoad, int _z_num_turn, string _verPO)
		{
			int num;
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = new SqlCommand()
				{
					Connection = _conn,
					CommandText = "INSERT INTO t_ZReport(z_num, z_file, z_date, z_kkm_id, z_total, z_total_return, z_teremok_id,z_cash, z_cash_return, z_card, z_card_return, z_cupon, z_cupon_return, z_dinner, z_dinner_return, z_kkm, z_type_id, z_date_load, z_1c_load, z_svb_load, z_num_turn, z_verPO) VALUES (@z_num, @z_file, @z_date, @z_kkm_id, @z_total, @z_total_return, @z_teremok_id, @z_cash, @z_cash_return, @z_card, @z_card_return, @z_cupon, @z_cupon_return, @z_dinner, @z_dinner_return, @z_kkm, @z_type_id, @z_date_load, @z_1c_load, @z_svb_load, @z_num_turn, @z_verPO)"
				};
				sqlCommand.Parameters.Clear();
				sqlCommand.Parameters.AddWithValue("@z_num", z_num);
				sqlCommand.Parameters.AddWithValue("@z_file", z_file);
				sqlCommand.Parameters.AddWithValue("@z_date", z_date);
				sqlCommand.Parameters.AddWithValue("@z_kkm_id", z_kkm);
				sqlCommand.Parameters.AddWithValue("@z_total", z_total);
				sqlCommand.Parameters.AddWithValue("@z_total_return", z_total_return);
				sqlCommand.Parameters.AddWithValue("@z_teremok_id", teremok_id);
				sqlCommand.Parameters.AddWithValue("@z_cash", z_cash);
				sqlCommand.Parameters.AddWithValue("@z_cash_return", z_cash_return);
				sqlCommand.Parameters.AddWithValue("@z_card", z_card);
				sqlCommand.Parameters.AddWithValue("@z_card_return", z_card_return);
				sqlCommand.Parameters.AddWithValue("@z_cupon", z_cupon);
				sqlCommand.Parameters.AddWithValue("@z_cupon_return", z_cupon_return);
				sqlCommand.Parameters.AddWithValue("@z_dinner", z_dinner);
				sqlCommand.Parameters.AddWithValue("@z_dinner_return", z_dinner_return);
				sqlCommand.Parameters.AddWithValue("@z_kkm", z_kkm);
				sqlCommand.Parameters.AddWithValue("@z_type_id", 1);
				sqlCommand.Parameters.AddWithValue("@z_date_load", dateLoad);
				sqlCommand.Parameters.AddWithValue("@z_1c_load", 0);
				sqlCommand.Parameters.AddWithValue("@z_svb_load", 0);
				sqlCommand.Parameters.AddWithValue("@z_num_turn", _z_num_turn);
				sqlCommand.Parameters.AddWithValue("@z_verPO", _verPO);
				sqlCommand.ExecuteNonQuery();
				sqlCommand.CommandText = "SELECT @@IDENTITY";
				sqlCommand.Parameters.Clear();
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return num;
		}

		public int ZReportAddSpb(SqlConnection _conn, string z_num, DateTime z_date, string z_kkm, string z_file, double z_total, double z_total_return, double z_cash, double z_cash_return, double z_card, double z_card_return, double z_cupon, double z_cupon_return, double z_dinner, double z_dinner_return, string teremok_id, DateTime dateLoad, int _z_num_turn, string _verPO)
		{
			int num;
			SqlCommand sqlCommand = null;
			try
			{
				sqlCommand = SqlCommandCreator.Create();
				sqlCommand.Connection = _conn;
				sqlCommand.CommandText = "INSERT INTO t_ZReport(z_num, z_file, z_date, z_kkm_id, z_total, z_total_return, z_teremok_id,z_cash, z_cash_return, z_card, z_card_return, z_cupon, z_cupon_return, z_dinner, z_dinner_return, z_kkm, z_type_id, z_date_load, z_1c_load, z_num_turn, z_verPO) VALUES (@z_num, @z_file, @z_date, @z_kkm_id, @z_total, @z_total_return, @z_teremok_id, @z_cash, @z_cash_return, @z_card, @z_card_return, @z_cupon, @z_cupon_return, @z_dinner, @z_dinner_return, @z_kkm, @z_type_id, @z_date_load, @z_1c_load, @z_num_turn, @z_verPO)";
				sqlCommand.Parameters.Clear();
				sqlCommand.Parameters.AddWithValue("@z_num", z_num);
				sqlCommand.Parameters.AddWithValue("@z_file", z_file);
				sqlCommand.Parameters.AddWithValue("@z_date", z_date);
				sqlCommand.Parameters.AddWithValue("@z_kkm_id", z_kkm);
				sqlCommand.Parameters.AddWithValue("@z_total", z_total);
				sqlCommand.Parameters.AddWithValue("@z_total_return", z_total_return);
				sqlCommand.Parameters.AddWithValue("@z_teremok_id", teremok_id);
				sqlCommand.Parameters.AddWithValue("@z_cash", z_cash);
				sqlCommand.Parameters.AddWithValue("@z_cash_return", z_cash_return);
				sqlCommand.Parameters.AddWithValue("@z_card", z_card);
				sqlCommand.Parameters.AddWithValue("@z_card_return", z_card_return);
				sqlCommand.Parameters.AddWithValue("@z_cupon", z_cupon);
				sqlCommand.Parameters.AddWithValue("@z_cupon_return", z_cupon_return);
				sqlCommand.Parameters.AddWithValue("@z_dinner", z_dinner);
				sqlCommand.Parameters.AddWithValue("@z_dinner_return", z_dinner_return);
				sqlCommand.Parameters.AddWithValue("@z_kkm", z_kkm);
				sqlCommand.Parameters.AddWithValue("@z_type_id", 1);
				sqlCommand.Parameters.AddWithValue("@z_date_load", dateLoad);
				sqlCommand.Parameters.AddWithValue("@z_1c_load", 0);
				sqlCommand.Parameters.AddWithValue("@z_num_turn", _z_num_turn);
				sqlCommand.Parameters.AddWithValue("@z_verPO", _verPO);
				sqlCommand.ExecuteNonQuery();
				sqlCommand.CommandText = "SELECT @@IDENTITY";
				sqlCommand.Parameters.Clear();
				num = Convert.ToInt32(sqlCommand.ExecuteScalar());
			}
			catch (Exception exception)
			{
				throw exception;
			}
			return num;
		}

		public void ZReportParse(RBServer.Debug_classes.FileInfo file, string teremok_folder, SqlConnection conn)
		{
			double num;
			TimeSpan? nullable;
			object[] teremokName;
			TimeSpan? nullable1;
			RBServer.Debug_classes.StreamReader streamReader = null;
			string str = "-1";
			string name = file.Name;
			int num1 = 0;
			try
			{
				try
				{
					CConfig cConfig = new CConfig();
					CTransfer cTransfer = new CTransfer();
					char[] charArray = " ".ToCharArray();
					char[] chrArray = "/".ToCharArray();
					DateTime today = DateTime.Today;
					DateTime dateTime = DateTime.Today;
					DateTime today1 = DateTime.Today;
					int num2 = 0;
					int num3 = 1;
					string str1 = null;
					string str2 = null;
					double num4 = 0;
					double num5 = 0;
					double num6 = 0;
					double num7 = 0;
					double num8 = 0;
					double num9 = 0;
					double num10 = 0;
					double num11 = 0;
					double num12 = 0;
					double num13 = 0;
					string str3 = null;
					double num14 = 0;
					string teremokFolder = null;
					string str4 = null;
					string str5 = null;
					string str6 = null;
					string str7 = null;
					string str8 = null;
					string str9 = null;
					string str10 = null;
					string str11 = null;
					string str12 = "0";
					bool flag = false;
					string str13 = null;
					bool flag1 = false;
					string teremokName1 = "";
					bool flag2 = false;
					bool flag3 = false;
					int num15 = 0;
					int num16 = 0;
					DateTime now = DateTime.Now;
					DateTime now1 = DateTime.Now;
					streamReader = new RBServer.Debug_classes.StreamReader(file.FullName, Encoding.GetEncoding(1251));
					while (true)
					{
						string str14 = streamReader.ReadLine();
						string str15 = str14;
						if (str14 == null)
						{
							break;
						}
						num1++;
						string str16 = this.MyTrim(str15);
						str = str16;
						string[] strArrays = str16.Split(charArray);
						string str17 = strArrays[0];
						string str18 = str17;
						if (str17 != null)
						{
							switch (str18)
							{
								case "Касса":
								{
									str2 = strArrays[(int)strArrays.Length - 1];
									continue;
								}
								case "Версия":
								{
									str12 = strArrays[(int)strArrays.Length - 1];
									continue;
								}
								case "Смена":
								{
									num2 = Convert.ToInt32(strArrays[(int)strArrays.Length - 1]);
									continue;
								}
								case "Zномер":
								{
									str1 = strArrays[(int)strArrays.Length - 1];
									continue;
								}
								case "ИтогПродаж":
								{
									num4 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "Возврат/аннулирование":
								{
									num5 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаНал":
								{
									num6 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратНал":
								{
									num7 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаКарта":
								{
									num8 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратКарта":
								{
									num9 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаКупоны":
								{
									num10 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратКупоны":
								{
									num11 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаОбеды":
								{
									num12 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратОбеды":
								{
									num13 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									teremokFolder = teremok_folder;
									teremokName1 = this.GetTeremokName(teremok_folder, conn);
									today = this.getDateFromZreportCheckFirstString(file.FullName);
									this.CurrentZDate = new DateTime?(today);
									DateTime dateTime1 = DateTime.Now;
									DateTime? nullable2 = this.make_date_from_fileName(file.Name);
									if (nullable2.HasValue)
									{
										TimeSpan timeSpan = TimeSpan.FromDays(1);
										DateTime? nullable3 = nullable2;
										DateTime value = today;
										if (nullable3.HasValue)
										{
											nullable1 = new TimeSpan?(nullable3.GetValueOrDefault() - value);
										}
										else
										{
											nullable = null;
											nullable1 = nullable;
										}
										nullable = nullable1;
										TimeSpan timeSpan1 = timeSpan;
										if ((nullable.HasValue ? nullable.GetValueOrDefault() >= timeSpan1 : false))
										{
											string emailRbsMessZ = cConfig.email_rbs_mess_z;
											string[] shortDateString = new string[] { " Теремок: ", teremokName1, "\n Касса: ", str2, "\n Чек за дату ", null, null, null };
											value = nullable2.Value;
											shortDateString[5] = value.ToShortDateString();
											shortDateString[6] = " ищите в числе ";
											shortDateString[7] = today.ToShortDateString();
											cTransfer.SendMailSync(emailRbsMessZ, "Изменена дата загрузки z отчета", string.Concat(shortDateString), false, file.FullName);
										}
									}
									CTransfer.YZreport zreportCl = CTransfer.zreport_cl;
									zreportCl._kkm_id = str2;
									zreportCl._teremok_name = teremokName1;
									zreportCl._num = str1;
									zreportCl._num_turn = num2.ToString();
									zreportCl._teremok_id = this.GetTeremokID(teremok_folder, conn);
									zreportCl._date = today;
									num16 = this.ZReportAdd(conn, str1, today, str2, file.Name, num4, num5, num6, num7, num8, num9, num10, num11, num12, num13, zreportCl._teremok_id, dateTime1, num2, str12);
									zreportCl._id = num16.ToString();
									continue;
								}
								case "Чек":
								{
									flag2 = true;
									flag3 = false;
									Thread.Sleep(60);
									now = Convert.ToDateTime(string.Concat(strArrays[2], " ", strArrays[3]));
									now1 = Convert.ToDateTime(strArrays[2]);
									Convert.ToInt32(strArrays[4]);
									Convert.ToInt32(strArrays[4]);
									num15 = this.CheckParse(conn, num16, strArrays[1], strArrays[4], strArrays[5], now);
									str3 = strArrays[1];
									num14 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[5], 1));
									str13 = str3.Split(chrArray)[1];
									continue;
								}
								case "+":
								{
									if (str13.Replace("0", "") == "" && !flag1)
									{
										flag1 = true;
										string emailRbsErrZreport = cConfig.email_rbs_err_zreport;
										teremokName = new object[] { " Теремок: ", teremokName1, "\n Касса: ", str2, "\n Дата и Время: ", now, "\n Фискальный: ", str13 };
										cTransfer.SendMail(emailRbsErrZreport, "Не верный номер фискального регистратора", string.Concat(teremokName), false);
									}
									flag2 = false;
									flag = false;
									str4 = null;
									str5 = null;
									str6 = null;
									str11 = null;
									str7 = null;
									str8 = null;
									continue;
								}
								case "-":
								{
									flag2 = false;
									continue;
								}
								case "Оплата":
								{
									str9 = strArrays[1];
									str10 = strArrays[2];
									if (flag2)
									{
										this.CheckParsePayment(conn, num16, num15, str9, str10);
									}
									if (!flag3)
									{
										continue;
									}
									this.CheckParsePaymentReturn(conn, num16, num15, str9, str10);
									continue;
								}
								case "":
								{
									continue;
								}
								case "Возврат":
								{
									flag2 = false;
									flag3 = true;
									continue;
								}
								case "CardNumber:":
								{
									str4 = strArrays[1];
									continue;
								}
								case "ReferenceNumber:":
								{
									if ((int)strArrays.Length >= 2)
									{
										str5 = strArrays[1];
										continue;
									}
									else
									{
										str5 = "";
										teremokName = new object[] { "Пустой ReferenceNumber filename:", name, " line_num: ", num1, " line: ", str, "\r\nДата продажи: ", now, "; Номер кассы: ", str2, "; Номер чека: ", str3, " Теремок: ", teremokName1 };
										string str19 = string.Concat(teremokName);
										Program._transfer.SendMail(cConfig.email_noRefNum, "Error RBSERVER Разбора z-отчета", str19, false);
										continue;
									}
								}
								case "AuthorizationCode:":
								{
									str6 = strArrays[1];
									continue;
								}
								case "CardType:":
								{
									str7 = strArrays[1];
									continue;
								}
								case "CardTotal:":
								{
									str8 = strArrays[1];
									continue;
								}
								case "TerminalID:":
								{
									if ((int)strArrays.Length != 1)
									{
										str11 = strArrays[1];
									}
									if (Convert.ToInt32(str9) != 2)
									{
										continue;
									}
									if (flag)
									{
										num3 = 2;
									}
									this.writeCard(conn, num15, num16, now1, str4, str5, str6, str7, str8, str2, this.GetTeremokID(teremok_folder, conn), str11, 1);
									this.writeCardNew(conn, num15, num16, now1, str4, str5, str6, str7, str8, str2, this.GetTeremokID(teremok_folder, conn), str11, num3);
									if (!flag)
									{
										continue;
									}
									if (str5 != null)
									{
										this.DeleteCard(conn, this.GetTeremokID(teremok_folder, conn), str2, now, str5, str6, str8);
										this.findCard(conn, this.GetTeremokID(teremok_folder, conn), str2, now1, str5, str6, str8, str4, num14 * -1, teremokName1);
									}
									flag = false;
									num3 = 1;
									continue;
								}
							}
						}
						if (flag2)
						{
							this.CheckItemParse(conn, num16, str15, num15, now, str3, str2, teremokFolder);
						}
						if (flag3)
						{
							flag = true;
							this.CheckItemParseReturn(conn, num16, str15, num15, now);
						}
					}
					flag1 = false;
					double num17 = Convert.ToDouble(this.return_count_check_sum(num16, conn));
					double num18 = Convert.ToDouble(this.return_count_dinner_sum(num16, conn));
					if (num18 == num12)
					{
						num = num4 - num18;
					}
					else
					{
						num = num4;
						cTransfer.Log(str2, 1);
					}
					if (num != num17)
					{
						SqlCommand sqlCommand = null;
						sqlCommand = new SqlCommand()
						{
							Connection = conn
						};
						double num19 = num - num17;
						string str20 = "INSERT INTO t_CheckItem (ch_mnome_id, ch_zreport_id, ch_check_id, ch_amount1, ch_amount2, ch_count, ch_datetime, ch_type_id, ch_dinner_card, ch_name_user, ch_combo)  VALUES(@ch_mnome_id, @ch_zreport_id, @ch_check_id, @ch_amount1, @ch_amount2, @ch_count, @ch_datetime, @ch_type_id, @ch_dinner_card, @ch_name_user, @ch_combo)";
						sqlCommand.Parameters.AddWithValue("@ch_mnome_id", "9999999999999");
						sqlCommand.Parameters.AddWithValue("@ch_zreport_id", num16);
						sqlCommand.Parameters.AddWithValue("@ch_check_id", num15);
						sqlCommand.Parameters.AddWithValue("@ch_amount1", num19);
						sqlCommand.Parameters.AddWithValue("@ch_amount2", num19);
						sqlCommand.Parameters.AddWithValue("@ch_count", 1);
						sqlCommand.Parameters.AddWithValue("@ch_datetime", now);
						sqlCommand.Parameters.AddWithValue("@ch_type_id", 1);
						sqlCommand.Parameters.AddWithValue("@ch_dinner_card", "0000000000000");
						sqlCommand.Parameters.AddWithValue("@ch_name_user", "Товар без кода");
						sqlCommand.Parameters.AddWithValue("@ch_combo", "0");
						sqlCommand.Parameters.AddWithValue("@ch_svb", "0");
						sqlCommand.CommandText = str20;
						sqlCommand.ExecuteNonQuery();
						teremokName = new object[] { "Ресторан: ", this.GetTeremokName(teremok_folder, conn), "\nНомер смены: ", str1, " \nКасса: ", str2, " \nРасхождение на сумму: ", num19 };
						string str21 = string.Concat(teremokName);
						cTransfer.SendMail(cConfig.email_rbs_err_goods, "Товар без кода", str21, false);
					}
					this.updateCheck(conn, this.quantityCheck(conn, num16), num16);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					teremokName = new object[] { "exp:ParseZ filename:", name, " line_num: ", num1, " line: ", str, " exception:", exception.Message, " !!! " };
					string str22 = string.Concat(teremokName);
					DebugPanel.Log(str22);
					throw new Exception(str22);
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

		public void ZReportParse_test(RBServer.Debug_classes.FileInfo file, string teremok_folder, SqlConnection conn)
		{
			double num;
			RBServer.Debug_classes.StreamReader streamReader = null;
			try
			{
				try
				{
					CConfig cConfig = new CConfig();
					CTransfer cTransfer = new CTransfer();
					char[] charArray = " ".ToCharArray();
					char[] chrArray = "/".ToCharArray();
					DateTime today = DateTime.Today;
					DateTime dateTime = DateTime.Today;
					DateTime today1 = DateTime.Today;
					int num1 = 1;
					string str = null;
					string str1 = "9999";
					double num2 = 0;
					double num3 = 0;
					string str2 = null;
					double num4 = 0;
					string str3 = null;
					string str4 = null;
					string str5 = null;
					string str6 = null;
					string str7 = null;
					string str8 = null;
					string str9 = null;
					bool flag = false;
					string str10 = null;
					bool flag1 = false;
					string str11 = "";
					bool flag2 = false;
					bool flag3 = false;
					int num5 = 0;
					int num6 = 0;
					DateTime now = DateTime.Now;
					DateTime now1 = DateTime.Now;
					Encoding encoding = Encoding.GetEncoding(1251);
					streamReader = new RBServer.Debug_classes.StreamReader(file.FullName, encoding);
					while (true)
					{
						string str12 = streamReader.ReadLine();
						string str13 = str12;
						if (str12 == null)
						{
							break;
						}
						string[] strArrays = this.MyTrim(str13).Split(charArray);
						string str14 = strArrays[0];
						string str15 = str14;
						if (str14 != null)
						{
							switch (str15)
							{
								case "Касса":
								{
									str1 = strArrays[(int)strArrays.Length - 1];
									continue;
								}
								case "Версия":
								{
									string str16 = strArrays[(int)strArrays.Length - 1];
									continue;
								}
								case "Смена":
								{
									Convert.ToInt32(strArrays[(int)strArrays.Length - 1]);
									continue;
								}
								case "Zномер":
								{
									str = strArrays[(int)strArrays.Length - 1];
									continue;
								}
								case "ИтогПродаж":
								{
									num2 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "Возврат/аннулирование":
								{
									Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаНал":
								{
									Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратНал":
								{
									Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаКарта":
								{
									Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратКарта":
								{
									Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаКупоны":
								{
									Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратКупоны":
								{
									Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаОбеды":
								{
									num3 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратОбеды":
								{
									Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									today = this.getDateFromZreportCheckFirstString(file.FullName);
									DateTime dateTime1 = DateTime.Now;
									num6 = 999;
									str11 = "test9999";
									continue;
								}
								case "Чек":
								{
									flag2 = true;
									flag3 = false;
									Thread.Sleep(60);
									now = Convert.ToDateTime(string.Concat(strArrays[2], " ", strArrays[3]));
									now1 = Convert.ToDateTime(strArrays[2]);
									Convert.ToInt32(strArrays[4]);
									if (Convert.ToInt32(strArrays[4]) == 1)
									{
										dateTime = now;
									}
									str2 = strArrays[1];
									num4 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[5], 1));
									str10 = str2.Split(chrArray)[1];
									if (Convert.ToInt32(strArrays[4]) != 4 || today.Day <= dateTime.Day || now.Hour < 0)
									{
										continue;
									}
									int day = dateTime.Day;
									int day1 = today.Day;
									this.dayMinus = true;
									continue;
								}
								case "+":
								{
									if (str10 != "000002" && str10 != "000003" && !flag1)
									{
										flag1 = true;
									}
									flag2 = false;
									flag = false;
									str3 = null;
									str4 = null;
									str5 = null;
									str9 = null;
									str6 = null;
									str7 = null;
									continue;
								}
								case "-":
								{
									flag2 = false;
									continue;
								}
								case "Оплата":
								{
									str8 = strArrays[1];
									string str17 = strArrays[2];
									if (!flag3)
									{
										continue;
									}
									continue;
								}
								case "":
								{
									continue;
								}
								case "Возврат":
								{
									flag2 = false;
									flag3 = true;
									continue;
								}
								case "CardNumber:":
								{
									str3 = strArrays[1];
									continue;
								}
								case "ReferenceNumber:":
								{
									str4 = strArrays[1];
									continue;
								}
								case "AuthorizationCode:":
								{
									str5 = strArrays[1];
									continue;
								}
								case "CardType:":
								{
									str6 = strArrays[1];
									continue;
								}
								case "CardTotal:":
								{
									str7 = strArrays[1];
									continue;
								}
								case "TerminalID:":
								{
									if ((int)strArrays.Length != 1)
									{
										str9 = strArrays[1];
									}
									if (Convert.ToInt32(str8) != 2)
									{
										continue;
									}
									if (flag)
									{
										num1 = 2;
									}
									this.writeCard(conn, num5, num6, now1, str3, str4, str5, str6, str7, str1, "9999", str9, 1);
									this.writeCardNew(conn, num5, num6, now1, str3, str4, str5, str6, str7, str1, "9999", str9, num1);
									if (!flag)
									{
										continue;
									}
									if (str4 != null)
									{
										this.DeleteCard(conn, this.GetTeremokID(teremok_folder, conn), str1, now, str4, str5, str7);
										this.findCard(conn, this.GetTeremokID(teremok_folder, conn), str1, now1, str4, str5, str7, str3, num4 * -1, str11);
									}
									flag = false;
									num1 = 1;
									continue;
								}
							}
						}
						if (flag3)
						{
							flag = true;
						}
					}
					flag1 = false;
					double num7 = Convert.ToDouble(this.return_count_check_sum(num6, conn));
					double num8 = Convert.ToDouble(this.return_count_dinner_sum(num6, conn));
					if (num8 == num3)
					{
						num = num2 - num8;
					}
					else
					{
						num = num2;
						cTransfer.Log(str1, 1);
					}
					if (num != num7)
					{
						SqlCommand sqlCommand = null;
						sqlCommand = new SqlCommand()
						{
							Connection = conn
						};
						double num9 = num - num7;
						string str18 = "INSERT INTO t_CheckItem (ch_mnome_id, ch_zreport_id, ch_check_id, ch_amount1, ch_amount2, ch_count, ch_datetime, ch_type_id, ch_dinner_card, ch_name_user, ch_combo)  VALUES(@ch_mnome_id, @ch_zreport_id, @ch_check_id, @ch_amount1, @ch_amount2, @ch_count, @ch_datetime, @ch_type_id, @ch_dinner_card, @ch_name_user, @ch_combo)";
						sqlCommand.Parameters.AddWithValue("@ch_mnome_id", "9999999999999");
						sqlCommand.Parameters.AddWithValue("@ch_zreport_id", num6);
						sqlCommand.Parameters.AddWithValue("@ch_check_id", num5);
						sqlCommand.Parameters.AddWithValue("@ch_amount1", num9);
						sqlCommand.Parameters.AddWithValue("@ch_amount2", num9);
						sqlCommand.Parameters.AddWithValue("@ch_count", 1);
						sqlCommand.Parameters.AddWithValue("@ch_datetime", now);
						sqlCommand.Parameters.AddWithValue("@ch_type_id", 1);
						sqlCommand.Parameters.AddWithValue("@ch_dinner_card", "0000000000000");
						sqlCommand.Parameters.AddWithValue("@ch_name_user", "Товар без кода");
						sqlCommand.Parameters.AddWithValue("@ch_combo", "0");
						sqlCommand.Parameters.AddWithValue("@ch_svb", "0");
						sqlCommand.CommandText = str18;
						sqlCommand.ExecuteNonQuery();
						object[] teremokName = new object[] { "Ресторан: ", this.GetTeremokName(teremok_folder, conn), "\nНомер смены: ", str, " \nКасса: ", str1, " \nРасхождение на сумму: ", num9 };
						string str19 = string.Concat(teremokName);
						cTransfer.SendMail(cConfig.email_rbs_err_goods, "Товар без кода", str19, false);
					}
					this.updateCheck(conn, this.quantityCheck(conn, num6), num6);
				}
				catch (Exception exception)
				{
					throw exception;
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

		public void ZReportParseSpb(RBServer.Debug_classes.FileInfo file, string teremok_folder, SqlConnection conn)
		{
			DateTime now;
			double num;
			DateTime? nullable;
			DateTime value;
			TimeSpan? nullable1;
			TimeSpan timeSpan;
			string[] shortDateString;
			object[] teremokName;
			TimeSpan? nullable2;
			TimeSpan? nullable3;
			RBServer.Debug_classes.StreamReader streamReader = null;
			string str = "-1";
			string name = file.Name;
			int num1 = 0;
			bool flag = true;
			try
			{
				try
				{
					CConfig cConfig = new CConfig();
					CTransfer cTransfer = new CTransfer();
					char[] charArray = " ".ToCharArray();
					char[] chrArray = "/".ToCharArray();
					DateTime today = DateTime.Today;
					DateTime dateTime = DateTime.Today;
					DateTime today1 = DateTime.Today;
					int num2 = 0;
					int num3 = 1;
					string str1 = null;
					string str2 = null;
					double num4 = 0;
					double num5 = 0;
					double num6 = 0;
					double num7 = 0;
					double num8 = 0;
					double num9 = 0;
					double num10 = 0;
					double num11 = 0;
					double num12 = 0;
					double num13 = 0;
					string str3 = null;
					double num14 = 0;
					string teremokFolder = null;
					string str4 = null;
					string str5 = null;
					string str6 = null;
					string str7 = null;
					string str8 = null;
					string str9 = null;
					string str10 = null;
					string str11 = null;
					string str12 = "0";
					bool flag1 = false;
					string str13 = null;
					bool flag2 = false;
					string teremokName1 = "";
					bool flag3 = false;
					bool flag4 = false;
					int num15 = 0;
					int num16 = 0;
					DateTime now1 = DateTime.Now;
					DateTime dateTime1 = DateTime.Now;
					DateTime? nullable4 = null;
					streamReader = new RBServer.Debug_classes.StreamReader(file.FullName, Encoding.GetEncoding(1251));
					while (true)
					{
						string str14 = streamReader.ReadLine();
						string str15 = str14;
						if (str14 == null)
						{
							break;
						}
						num1++;
						string str16 = this.MyTrim(str15);
						str = str16;
						string[] strArrays = str16.Split(charArray);
						string str17 = strArrays[0];
						string str18 = str17;
						if (str17 != null)
						{
							switch (str18)
							{
								case "Касса":
								{
									str2 = strArrays[(int)strArrays.Length - 1];
									continue;
								}
								case "Версия":
								{
									str12 = strArrays[(int)strArrays.Length - 1];
									continue;
								}
								case "Смена":
								{
									num2 = Convert.ToInt32(strArrays[(int)strArrays.Length - 1]);
									continue;
								}
								case "Zномер":
								{
									str1 = strArrays[(int)strArrays.Length - 1];
									continue;
								}
								case "ИтогПродаж":
								{
									num4 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "Возврат/аннулирование":
								{
									num5 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаНал":
								{
									num6 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратНал":
								{
									num7 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаКарта":
								{
									num8 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратКарта":
								{
									num9 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаКупоны":
								{
									num10 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратКупоны":
								{
									num11 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ПродажаОбеды":
								{
									num12 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									continue;
								}
								case "ВозвратОбеды":
								{
									num13 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[(int)strArrays.Length - 1], 1));
									today = this.getDateFromZreportCheckFirstString(file.FullName);
									now = DateTime.Now;
									nullable4 = this.make_date_from_fileName(file.Name);
									if (nullable4.HasValue)
									{
										TimeSpan timeSpan1 = TimeSpan.FromDays(1);
										nullable = nullable4;
										value = today;
										if (nullable.HasValue)
										{
											nullable2 = new TimeSpan?(nullable.GetValueOrDefault() - value);
										}
										else
										{
											nullable1 = null;
											nullable2 = nullable1;
										}
										nullable1 = nullable2;
										timeSpan = timeSpan1;
										if ((nullable1.HasValue ? nullable1.GetValueOrDefault() >= timeSpan : false))
										{
											string emailRbsErrZreport = cConfig.email_rbs_err_zreport;
											shortDateString = new string[] { " Теремок: ", teremokName1, "\n Касса: ", str2, "\n Чек за дату ", null, null, null };
											value = nullable4.Value;
											shortDateString[5] = value.ToShortDateString();
											shortDateString[6] = " ищите в числе ";
											shortDateString[7] = today.ToShortDateString();
											cTransfer.SendMail(emailRbsErrZreport, "Изменена дата загрузки z отчета", string.Concat(shortDateString), false);
										}
									}
									num16 = this.ZReportAdd(conn, str1, today, str2, file.Name, num4, num5, num6, num7, num8, num9, num10, num11, num12, num13, this.GetTeremokID(teremok_folder, conn), now, num2, str12);
									flag = false;
									teremokFolder = teremok_folder;
									teremokName1 = this.GetTeremokName(teremok_folder, conn);
									continue;
								}
								case "Чек":
								{
									if (flag)
									{
										today = this.getDateFromZreportCheckFirstString(file.FullName);
										now = DateTime.Now;
										nullable4 = this.make_date_from_fileName(file.Name);
										if (nullable4.HasValue)
										{
											TimeSpan timeSpan2 = TimeSpan.FromDays(1);
											nullable = nullable4;
											value = today;
											if (nullable.HasValue)
											{
												nullable3 = new TimeSpan?(nullable.GetValueOrDefault() - value);
											}
											else
											{
												nullable1 = null;
												nullable3 = nullable1;
											}
											nullable1 = nullable3;
											timeSpan = timeSpan2;
											if ((nullable1.HasValue ? nullable1.GetValueOrDefault() >= timeSpan : false))
											{
												string emailRbsErrZreport1 = cConfig.email_rbs_err_zreport;
												shortDateString = new string[] { " Теремок: ", teremokName1, "\n Касса: ", str2, "\n Чек за дату ", null, null, null };
												value = nullable4.Value;
												shortDateString[5] = value.ToShortDateString();
												shortDateString[6] = " ищите в числе ";
												shortDateString[7] = today.ToShortDateString();
												cTransfer.SendMail(emailRbsErrZreport1, "Изменена дата загрузки z отчета", string.Concat(shortDateString), false);
											}
										}
										num16 = this.ZReportAdd(conn, str1, today, str2, file.Name, num4, num5, num6, num7, num8, num9, num10, num11, num12, num13, this.GetTeremokID(teremok_folder, conn), now, num2, str12);
										flag = false;
									}
									flag3 = true;
									flag4 = false;
									Thread.Sleep(60);
									now1 = Convert.ToDateTime(string.Concat(strArrays[2], " ", strArrays[3]));
									dateTime1 = Convert.ToDateTime(strArrays[2]);
									Convert.ToInt32(strArrays[4]);
									if (Convert.ToInt32(strArrays[4]) == 1)
									{
										dateTime = now1;
									}
									num15 = this.CheckParse(conn, num16, strArrays[1], strArrays[4], strArrays[5], now1);
									str3 = strArrays[1];
									num14 = Convert.ToDouble(CUtilHelper.ParceAmount(strArrays[5], 1));
									str13 = str3.Split(chrArray)[1];
									if (Convert.ToInt32(strArrays[4]) != 4 || today.Day <= dateTime.Day || now1.Hour < 0)
									{
										continue;
									}
									int day = dateTime.Day - today.Day;
									this.fixZDate(conn, str1, str2, today, this.GetTeremokID(teremok_folder, conn), day);
									this.dayMinus = true;
									continue;
								}
								case "+":
								{
									if (str13 != "000002" && str13 != "000003" && !flag2)
									{
										flag2 = true;
										string emailRbsErrZreport2 = cConfig.email_rbs_err_zreport;
										teremokName = new object[] { " Теремок: ", teremokName1, "\n Касса: ", str2, "\n Дата и Время: ", now1, "\n Фискальный: ", str13 };
										cTransfer.SendMail(emailRbsErrZreport2, "Не верный номер фискального регистратора", string.Concat(teremokName), false);
									}
									flag3 = false;
									flag1 = false;
									str4 = null;
									str5 = null;
									str6 = null;
									str11 = null;
									str7 = null;
									str8 = null;
									continue;
								}
								case "-":
								{
									flag3 = false;
									continue;
								}
								case "Оплата":
								{
									str9 = strArrays[1];
									str10 = strArrays[2];
									if (flag3)
									{
										this.CheckParsePayment(conn, num16, num15, str9, str10);
									}
									if (!flag4)
									{
										continue;
									}
									this.CheckParsePaymentReturn(conn, num16, num15, str9, str10);
									continue;
								}
								case "":
								{
									continue;
								}
								case "Возврат":
								{
									flag3 = false;
									flag4 = true;
									continue;
								}
								case "CardNumber:":
								{
									str4 = strArrays[1];
									continue;
								}
								case "ReferenceNumber:":
								{
									if ((int)strArrays.Length >= 2)
									{
										str5 = strArrays[1];
										continue;
									}
									else
									{
										str5 = "";
										teremokName = new object[] { "Пустой ReferenceNumber filename:", name, " line_num: ", num1, " line: ", str, "\r\nДата продажи: ", now1, "; Номер кассы: ", str2, "; Номер чека: ", str3, " Теремок: ", teremokName1 };
										string str19 = string.Concat(teremokName);
										Program._transfer.SendMail(cConfig.email_noRefNum, "Error RBSERVER Разбора z-отчета", str19, false);
										continue;
									}
								}
								case "AuthorizationCode:":
								{
									str6 = strArrays[1];
									continue;
								}
								case "CardType:":
								{
									str7 = strArrays[1];
									continue;
								}
								case "CardTotal:":
								{
									str8 = strArrays[1];
									continue;
								}
								case "TerminalID:":
								{
									if ((int)strArrays.Length != 1)
									{
										str11 = strArrays[1];
									}
									if (Convert.ToInt32(str9) != 2)
									{
										continue;
									}
									if (flag1)
									{
										num3 = 2;
									}
									this.writeCard(conn, num15, num16, dateTime1, str4, str5, str6, str7, str8, str2, this.GetTeremokID(teremok_folder, conn), str11, 1);
									this.writeCardNew(conn, num15, num16, dateTime1, str4, str5, str6, str7, str8, str2, this.GetTeremokID(teremok_folder, conn), str11, num3);
									if (!flag1)
									{
										continue;
									}
									if (str5 != null)
									{
										this.DeleteCard(conn, this.GetTeremokID(teremok_folder, conn), str2, now1, str5, str6, str8);
										this.findCard(conn, this.GetTeremokID(teremok_folder, conn), str2, dateTime1, str5, str6, str8, str4, num14 * -1, teremokName1);
									}
									flag1 = false;
									num3 = 1;
									continue;
								}
							}
						}
						if (flag3)
						{
							this.CheckItemParse(conn, num16, str15, num15, now1, str3, str2, teremokFolder);
						}
						if (flag4)
						{
							flag1 = true;
							this.CheckItemParseReturn(conn, num16, str15, num15, now1);
						}
					}
					flag2 = false;
					double num17 = Convert.ToDouble(this.return_count_check_sum(num16, conn));
					double num18 = Convert.ToDouble(this.return_count_dinner_sum(num16, conn));
					if (num18 == num12)
					{
						num = num4 - num18;
					}
					else
					{
						num = num4;
						cTransfer.Log(str2, 1);
					}
					if (num != num17)
					{
						SqlCommand sqlCommand = null;
						sqlCommand = new SqlCommand()
						{
							Connection = conn
						};
						double num19 = num - num17;
						string str20 = "INSERT INTO t_CheckItem (ch_mnome_id, ch_zreport_id, ch_check_id, ch_amount1, ch_amount2, ch_count, ch_datetime, ch_type_id, ch_dinner_card, ch_name_user, ch_combo)  VALUES(@ch_mnome_id, @ch_zreport_id, @ch_check_id, @ch_amount1, @ch_amount2, @ch_count, @ch_datetime, @ch_type_id, @ch_dinner_card, @ch_name_user, @ch_combo)";
						sqlCommand.Parameters.AddWithValue("@ch_mnome_id", "9999999999999");
						sqlCommand.Parameters.AddWithValue("@ch_zreport_id", num16);
						sqlCommand.Parameters.AddWithValue("@ch_check_id", num15);
						sqlCommand.Parameters.AddWithValue("@ch_amount1", num19);
						sqlCommand.Parameters.AddWithValue("@ch_amount2", num19);
						sqlCommand.Parameters.AddWithValue("@ch_count", 1);
						sqlCommand.Parameters.AddWithValue("@ch_datetime", now1);
						sqlCommand.Parameters.AddWithValue("@ch_type_id", 1);
						sqlCommand.Parameters.AddWithValue("@ch_dinner_card", "0000000000000");
						sqlCommand.Parameters.AddWithValue("@ch_name_user", "Товар без кода");
						sqlCommand.Parameters.AddWithValue("@ch_combo", "0");
						sqlCommand.Parameters.AddWithValue("@ch_svb", "0");
						sqlCommand.CommandText = str20;
						sqlCommand.ExecuteNonQuery();
						teremokName = new object[] { "Ресторан: ", this.GetTeremokName(teremok_folder, conn), "\nНомер смены: ", str1, " \nКасса: ", str2, " \nРасхождение на сумму: ", num19 };
						string str21 = string.Concat(teremokName);
						cTransfer.SendMail(cConfig.email_rbs_err_goods, "Товар без кода", str21, false);
					}
					this.updateCheck(conn, this.quantityCheck(conn, num16), num16);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					teremokName = new object[] { "exp:ParseZ filename:", name, " line_num: ", num1, " line: ", str, " exception:", exception.Message, " !!! " };
					string str22 = string.Concat(teremokName);
					DebugPanel.Log(str22);
					throw new Exception(str22);
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
	}
}
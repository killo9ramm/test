using RBServer.Debug_classes;
using System;
using System.Data.SqlClient;

namespace RBClient.Classes
{
	internal class SqlWorker
	{
		public SqlWorker()
		{
		}

		internal static string SelectTeremokNameFromDBSafe(SqlConnection conn, string teremok_1C)
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
			catch (Exception exception1)
			{
				Exception exception = exception1;
				DebugPanel.Log(string.Concat("Не получилось достать теремок ", teremok_1C, " exp:", exception.Message));
				str = "";
			}
			return str;
		}
	}
}
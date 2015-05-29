using RBServer;
using System;
using System.Data.SqlClient;

namespace RBServer.Debug_classes
{
	internal class SqlCommandCreator
	{
		public SqlCommandCreator()
		{
		}

		public static SqlCommand Create(int Timeout)
		{
			return new SqlCommand()
			{
				CommandTimeout = Timeout
			};
		}

		public static SqlCommand Create()
		{
			SqlCommand sqlCommand = new SqlCommand();
			try
			{
				sqlCommand.CommandTimeout = Program._config.sql_timeout;
			}
			catch (Exception exception)
			{
			}
			return sqlCommand;
		}
	}
}
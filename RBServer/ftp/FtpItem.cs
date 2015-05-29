using System;
//using System.Runtime.InteropServices;

namespace Ftp
{
	public interface IFtpItem
	{
		string		Name		{get;set;}
		string		FullName	{get;}
		bool		IsFile		{get;}
		bool		IsDirectory {get;}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RBServer.Debug_classes
{
	internal class DirectoryInfo
	{
		private System.IO.DirectoryInfo dir;

		private List<RBServer.Debug_classes.FileInfo> flist;

		private List<RBServer.Debug_classes.DirectoryInfo> dlist;

		internal string FullName
		{
			get
			{
				return this.dir.FullName;
			}
		}

		internal string Name
		{
			get
			{
				return this.dir.Name;
			}
		}

		private DirectoryInfo()
		{
		}

		internal DirectoryInfo(string path)
		{
			this.dir = new System.IO.DirectoryInfo(path);
			this.flist = new List<RBServer.Debug_classes.FileInfo>();
			this.dlist = new List<RBServer.Debug_classes.DirectoryInfo>();
		}

		internal List<RBServer.Debug_classes.DirectoryInfo> GetDirectories()
		{
			this.dlist = new List<RBServer.Debug_classes.DirectoryInfo>();
			try
			{
				this.dir.GetDirectories().ToList<System.IO.DirectoryInfo>().ForEach((System.IO.DirectoryInfo a) => this.dlist.Add(new RBServer.Debug_classes.DirectoryInfo(a.FullName)));
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = string.Concat("Нет папок в папке ", this.dir.FullName, " Exception ", exception.Message);
				DebugPanel.Log(str);
				DebugPanel.OnFatalErrorOccured(str);
			}
			return this.dlist;
		}

		internal List<RBServer.Debug_classes.FileInfo> GetFiles()
		{
			this.flist = new List<RBServer.Debug_classes.FileInfo>();
			try
			{
				this.dir.GetFiles().ToList<System.IO.FileInfo>().ForEach((System.IO.FileInfo a) => this.flist.Add(new RBServer.Debug_classes.FileInfo(a)));
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = string.Concat("Нет файлов в папке ", this.dir.FullName, " Exception ", exception.Message);
				DebugPanel.Log(str);
			}
			return this.flist;
		}
	}
}
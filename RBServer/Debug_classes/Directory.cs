using System;
using System.IO;

namespace RBServer.Debug_classes
{
	internal class Directory
	{
		public Directory()
		{
		}

		internal static RBServer.Debug_classes.DirectoryInfo CreateDirectory(string source)
		{
			return new RBServer.Debug_classes.DirectoryInfo(System.IO.Directory.CreateDirectory(source).FullName);
		}

		internal static bool Exists(string source)
		{
			return System.IO.Directory.Exists(source);
		}
	}
}
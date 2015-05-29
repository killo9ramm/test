using System;
using System.IO;

namespace RBServer.Debug_classes
{
	internal class FileStream : System.IO.FileStream
	{
		internal FileStream(string path, FileMode fm) : base(path, fm)
		{
		}
	}
}
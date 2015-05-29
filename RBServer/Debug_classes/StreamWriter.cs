using System;
using System.IO;
using System.Text;

namespace RBServer.Debug_classes
{
	internal class StreamWriter : System.IO.StreamWriter
	{
		internal StreamWriter(string fname, bool b) : base(fname, b)
		{

		}

		internal StreamWriter(string fname, bool b, System.Text.Encoding e) : base(fname, b, e)
		{

		}
	}
}
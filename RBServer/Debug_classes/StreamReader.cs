using System;
using System.IO;
using System.Text;

namespace RBServer.Debug_classes
{
	internal class StreamReader : System.IO.StreamReader
	{
		internal StreamReader(string s, Encoding e) : base(s, e)
		{
		}
	}
}
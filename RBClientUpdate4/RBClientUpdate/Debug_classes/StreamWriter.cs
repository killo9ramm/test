using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBServer.Debug_classes
{
    internal class StreamWriter:System.IO.StreamWriter
    {
        internal StreamWriter(string fname, bool b) : base(fname, b) { }

        internal StreamWriter(string fname, bool b,Encoding e) : base(fname, b,e) { }
    }
}

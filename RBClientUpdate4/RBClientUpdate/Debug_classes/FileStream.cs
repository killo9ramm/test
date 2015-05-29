using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBServer.Debug_classes
{
    class FileStream : System.IO.FileStream
    {
        internal FileStream(string path, System.IO.FileMode fm) : base(path, fm) { }
    }
}

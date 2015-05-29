using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProcessServiceWorker;

namespace ranus
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 4)
                {
                    ProcessWorker.StartProcess(args[0], args[1], args[2],args[3]);
                }
                if (args.Length == 3)
                {
                    ProcessWorker.StartProcess(args[0],args[1],args[2]);
                }
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
        }
    }
}

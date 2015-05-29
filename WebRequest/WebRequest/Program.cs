using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebRequest
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(@"http://localhost:56023/TeremokService1.svc/t_Teremok");
            Request.Method = "GET";
            Request.Accept = "application/json";
            HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
            using (StreamReader sr = new StreamReader(Response.GetResponseStream()))
            {
                Console.WriteLine(sr.ReadToEnd());
            }
            Console.ReadKey();
        }
    }
}

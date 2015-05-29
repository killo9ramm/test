using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace RBClient.Classes
{
    class Serializer
    {
        public static void binary_write(ArrayList arr,string p)
        {
            IFormatter formatter=new BinaryFormatter();
            FileStream stream = new FileStream(p,FileMode.Create,FileAccess.Write,FileShare.None);
            formatter.Serialize(stream,arr);
            stream.Close();
        }

        public static ArrayList binary_get(string p)
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.None);
            return (ArrayList)formatter.Deserialize(stream);
        }

        public static string JsonSerialize(object o)
        {
            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(o);
            return json;
        }
    }
}

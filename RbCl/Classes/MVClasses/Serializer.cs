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

        public static byte[] BinarySerialize(object o)
        {
            IFormatter formatter = new BinaryFormatter();
            string path = Path.Combine(Environment.GetEnvironmentVariable("temp"),DateTime.Now.Ticks.ToString());
            FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, o);
            stream.Close();
            byte[] b = File.ReadAllBytes(path);
            File.Delete(path);
            return b;
        }

        public static object BinaryDeSerialize(byte[] byte_array)
        {
            string path = Path.Combine(Environment.GetEnvironmentVariable("temp"), DateTime.Now.Ticks.ToString());
            File.WriteAllBytes(path, byte_array);
            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            object o = formatter.Deserialize(stream);
            stream.Close();
            File.Delete(path);
            return o;
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

        public static object JsonDeSerialize(string o)
        {
            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var json = jsonSerializer.Deserialize<object>(o);
            return json;
        }
    }
}

using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Script.Serialization;

namespace RBClient.Classes
{
	internal class Serializer
	{
		public Serializer()
		{
		}

		public static ArrayList binary_get(string p)
		{
			IFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(p, FileMode.Open, FileAccess.Read, FileShare.None);
			return (ArrayList)binaryFormatter.Deserialize(fileStream);
		}

		public static void binary_write(ArrayList arr, string p)
		{
			IFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = new FileStream(p, FileMode.Create, FileAccess.Write, FileShare.None);
			binaryFormatter.Serialize(fileStream, arr);
			fileStream.Close();
		}

		public static string JsonSerialize(object o)
		{
			return (new JavaScriptSerializer()).Serialize(o);
		}
	}
}
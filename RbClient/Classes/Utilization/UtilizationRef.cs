using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RBClient.Classes.Utilization
{
    internal interface IQueriable
    {
        string MakeInsertQuery();
        string MakeDeleteAllQuery();
        string MakeDeleteQuery();
    }
   
    class UtilizationRef : IQueriable
    {
        private string tablename = "t_UtilReasonRef";
    
        internal Dictionary<string, string> fields;

        internal UtilizationRef()
        {
            fields = new Dictionary<string, string>();
        }

        internal static UtilizationRef Create(XElement xutil)
        {
            UtilizationRef utilRef = new UtilizationRef();
            foreach (XAttribute xa in xutil.Attributes().ToList<XAttribute>())
            {
                utilRef.fields.Add(xa.Name.ToString(), xa.Value);
            }

            return utilRef;
        }
        internal static List<UtilizationRef> Create(List<XElement> xutil)
        {
            List<UtilizationRef> list = new List<UtilizationRef>();
            xutil.ForEach(xe =>
            {
                list.Add(UtilizationRef.Create(xe));
            });
            return list;
        }

        internal static bool InsertIntoDB(List<UtilizationRef> list, bool clearTableBeforeWrite)
        {
            return SqlWorker.InsertIntoDB(list, clearTableBeforeWrite);
        }

        public string MakeInsertQuery() 
        {
            string query = "INSERT INTO "+tablename+" (";
            
            fields.Keys.ToList<string>().ForEach(s =>
            {
                query += s + ",";
            });

            query = query.Substring(0,query.Length-1)+") VALUES(";

            fields.Values.ToList<string>().ForEach(s =>
            {
                query += "'"+s + "',";
            });
            query = query.Substring(0, query.Length - 1) + ")";
            return query;
        }

        public string MakeDeleteAllQuery()
        {
            string query = "DELETE FROM " + tablename;
            return query;
        }

        public string MakeDeleteQuery()
        {
            throw new Exception();
            string query = "";
            return query;
        }
    }

}

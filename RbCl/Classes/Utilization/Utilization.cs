using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace RBClient.Classes.Utilization
{
    class Utilization : IQueriable
    {
        private string tablename = "t_Utilization";

        public string util_nome_id {get; private set;}
        public string util_quantity { get; private set; }
        public string util_doc_id { get; private set; }
        public string util_reason_id { get; private set; }
        public string util_k { get; private set; }


        internal Utilization(string Util_nome_id, string Util_quantity, string Util_doc_id, string Util_reason_id,
            string Util_k)
        {
            util_nome_id = Util_nome_id;
            util_quantity = Util_quantity;
            util_doc_id=Util_doc_id;
            util_reason_id = Util_reason_id;
            util_k = Util_k;
        }


        #region trash
        //internal static Utilization Create(DataRow row)
        //{
        //    Utilization utilRef = new Utilization();
            
        //    return null;
            
        //}

        //internal static List<Utilization> Create(DataTable dt)
        //{
        //    List<Utilization> list = new List<Utilization>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        list.Add(Utilization.Create(dr));
        //    }
        //    return list;
        //}
        #endregion

        //internal static bool InsertIntoDB(List<UtilizationRef> list, bool clearTableBeforeWrite)
        //{
        //    return SqlWorker.InsertIntoDB(list, clearTableBeforeWrite);
        //}

        public string MakeInsertQuery() 
        {
            string query = "INSERT INTO "+tablename;

            Type type = this.GetType();
            List<PropertyInfo> pinfos = type.GetProperties().ToList();
            string query1 = "(";
            string query2 = " VALUES(";
            foreach (PropertyInfo pi in pinfos)
            {
                try
                {
                    double val = -99;
                    string value = pi.GetValue(this, null).ToString();
                    if (value == "" || value.IndexOf('0') == 0) continue;
                    query1 += String.Format("{0},", pi.Name);

                    try
                    {
                        val=double.Parse(value);
                        query2 += String.Format("{0},", val);
                    }
                    catch(Exception ex)
                    {
                        query2 += String.Format("'{0}',", value);
                    }
                    
                }
                catch (Exception e)
                {
                    throw new Exception("Не удалось создать запрос, побилось на свойстве "+pi.Name,e);
                }
            }
            query1 = query1.Substring(0,query1.Length-1)+")";
            query2 = query2.Substring(0, query2.Length - 1) + ")";
            query += query1 + query2;

            return query;
        }

        public string MakeDeleteAllQuery()
        {
            string query = "DELETE FROM " + tablename;
            return query;
        }
        public string MakeDeleteQuery()
        {
            string query = "DELETE FROM " + tablename;
            query+=" WHERE util_nome_id="+util_nome_id+" AND "+"util_doc_id="+util_doc_id;
            return query;
        }
    }
}
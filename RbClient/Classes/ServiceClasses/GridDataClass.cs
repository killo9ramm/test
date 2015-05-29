using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.ComponentModel;

namespace RBClient
{
    class GridDataClass
    {
        //public int opd_id {get;set;}
        public string opd_id { get; set; }
       
        public string  nome_name {get;set;} //позиция
        
        //public int opd_order {get;set;} //кол-во
        public string opd_order { get; set; } //кол-во

        public string opd_ed {get;set;}   //ед
        public string opd_K {get;set;} //коэфф
        public string opd_bed {get;set;} //баз ед
        public string opd_order_total {get;set;} //итого
        public string opd_order2 {get;set;} // кол-во 2
        public string opd_quota {get;set;} //норма
        public string orr_name {get;set;} //категория возврата
        public string orr_id {get;set;}
        public string opd_rerreason_desc {get;set;} // описание возврата
        public string opd_order3 {get;set;} //кол-во 3
        public string opd_maxquota {get;set;} //норма
        public string opd_ke {get;set;}   //коэф.А
        public string opd_ze {get;set;} //ед.
        public string opd_kke {get;set;} //коэф.
        public string opd_total1 {get;set;} //итого
        public string opd_bold {get;set;}  //
        public string nome_1C {get;set;} //
        public string opd_retreason_id {get;set;} //время
                
        public string urr_name {get;set;} //время
        public string urr_id {get;set;} //время

        public List<DataGridComboItem> urr_list { get; set; } //время

        public static List<GridDataClass> Create(DataTable dt)
        {
            List<GridDataClass> gdc = new List<GridDataClass>();
            foreach (DataRow row in dt.Rows)
            {
                GridDataClass gdcc = GridDataClass.Create(row);
                if (gdcc != null)
                {
                    gdc.Add(gdcc);
                }
            }
            return gdc;
        }

        public static GridDataClass Create(DataRow dt)
        {
            GridDataClass gdc = new GridDataClass();
            Type type = typeof(GridDataClass);
            List<PropertyInfo> pinfos = type.GetProperties().ToList();
            foreach (PropertyInfo pi in pinfos)
            {
                var value=CellHelper.FindCell(dt,pi.Name);

                pi.SetValue(gdc, value.ToString(), null);

                #region int define
                //if (null != value)
                //{
                //    if (pi.PropertyType.Name.IndexOf("int",StringComparison.OrdinalIgnoreCase) != -1)
                //    {
                //        pi.SetValue(gdc, int.Parse(value.ToString()),null);
                //    }
                //    else
                //    {
                //        pi.SetValue(gdc, value.ToString(), null);
                //    }
                //}
                #endregion
            }
            return gdc;
        }
        public static List<GridDataClass> Create(DataTable dt, bool SpisanOn)
        {
            if (!SpisanOn)
            {
                return Create(dt);
            }
            List<GridDataClass> gdc = new List<GridDataClass>();
            foreach (DataRow row in dt.Rows)
            {
                GridDataClass gdcc = GridDataClass.Create(row,SpisanOn);
                if (gdcc != null)
                {
                    gdc.Add(gdcc);
                }
            }
            return gdc;
        }

        public static GridDataClass Create(DataRow dt,bool SpisanOn)
        {
            if(!SpisanOn){
                return Create(dt);
            }

            GridDataClass gdc = new GridDataClass();
            Type type = typeof(GridDataClass);
            List<PropertyInfo> pinfos = type.GetProperties().ToList();
            foreach (PropertyInfo pi in pinfos)
            {
                try
                {
                    var value = CellHelper.FindCell(dt, pi.Name);
                    pi.SetValue(gdc, value.ToString(), null);
                }catch(Exception e)
                {
                    string k = pi.Name;
                }
            }   

            //вставить причины списания
            gdc.urr_list = 
                DataGridCombo.Create(
                    pinfos.Where<PropertyInfo>(pi => pi.Name == "nome_1C")
                    .FirstOrDefault<PropertyInfo>()
                    .GetValue(gdc,null).ToString());
            return gdc;
        }

    }
}

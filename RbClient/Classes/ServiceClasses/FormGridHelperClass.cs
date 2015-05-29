using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using RBClient.Classes;
using System.ComponentModel;

namespace RBClient
{
    class FormGridHelperClass
    {
    }

    class DataGridComboItem
    {
        public string urr_id;
        public string urr_name;

        internal bool IsSimple=true;

        private List<string> _nome_include;
        internal List<string> nome_include
        {
            get
            {
                return _nome_include;
            }
            set
            {
                _nome_include=value;
                if(_nome_include!=null || _nome_include.Count>0)
                {
                    IsSimple=false;
                }
            }
        }

        internal static DataGridComboItem Create(DataRow rowData)
        {
            DataGridComboItem dtci = new DataGridComboItem();
            dtci.urr_id = CellHelper.FindCell(rowData, "urr_id").ToString().Trim();
            dtci.urr_name = CellHelper.FindCell(rowData, "urr_name").ToString().Trim();
            string nome_inc = CellHelper.FindCell(rowData, "nome_include").ToString().Trim();

            if (nome_inc != "")
            {
                dtci.nome_include = nome_inc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            return dtci;
        }

        private DataGridComboItem(){}

        public override string ToString()
        {
            return this.urr_name;
        }

        #region thash
        

        //internal static BindingList<DataGridComboItem> Create(DataTable dt)
        //{
        //    BindingList<DataGridComboItem> list_items = new BindingList<DataGridComboItem>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        DataGridComboItem dgci = DataGridComboItem.Create(dr);
        //        list_items.Add(dgci);
        //    }

        //    if (list_items.Count > 0) return list_items;
        //    else return null;
        //}
        //internal static IEnumerable<string> CreateStringArray(DataRow dataRow)
        //{
        //    //найти номенклатуру
        //    string nome_id=CellHelper.FindCell(dataRow, "nome_1C").ToString();

        //    //посмотреть присоединяется ли к ней доп списание
        //    DataGridCombo.Create(nome_id);

        //    //сформировать список 
        //    return new string[] { "1", "2", "3", "4" };
        //}

        //internal static object CreateStringArray(DataTable dataTable)
        //{

        //    return new string[]{"1","2","3","4"};
        //}
#endregion
    }

    class DataGridCombo
    {
        private static List<DataGridComboItem> _compoundCombo;
        public static List<DataGridComboItem> _simpleCombo;
        internal static List<DataGridComboItem> Combo
        {
            get
            {
                if(_simpleCombo==null || _simpleCombo.Count==0)
                {
                    _simpleCombo = new List<DataGridComboItem>();
                    _compoundCombo = new List<DataGridComboItem>();
                    //заполнить комбо
                    foreach(DataRow row in UtilRef.Rows)
                    {
                         DataGridComboItem dtci = DataGridComboItem.Create(row);
                         if (dtci.IsSimple) _simpleCombo.Add(dtci);
                         else _compoundCombo.Add(dtci); 
                    }
                }
                return _simpleCombo;
            }
        }

        public static DataTable _UtilRef = null;
        internal static DataTable UtilRef
        {
            get
            {
                if (_UtilRef == null)
                {
                    _UtilRef=SqlWorker.SelectFromDB("SELECT t_UtilReasonRef.urr_id, t_UtilReasonRef.urr_name, "+
                    "t_UtilReasonRef.nome_include "+
                    "FROM t_UtilReasonRef;"
                     ,"t_UtilReasonRef");
                }
                return _UtilRef;
            }
        }

        #region trash
        //internal static DataGridViewComboBoxCell Create()
        //{
        //    //возвратить обычный комбо
        //    return Combo;
        //}

        //internal static void Create(DataGridViewComboBoxCell dtgvcc)
        //{
        //    //возвратить обычный комбоpublic int opd_order {get;set;} //кол-во
        //    dtgvcc.DataSource = Combo.Items;
        //  //  dtgvcc.DisplayMember = "urr_name";
        //   // dtgvcc.ValueMember = "urr_id";


        //    //foreach (DataGridComboItem o in Combo.Items)
        //    //{
                
        //    //}

        //}
        #endregion

        internal static List<DataGridComboItem> Create(string nome_id)
        {
            //сделать основной комбо
            List<DataGridComboItem> main_combo = new List<DataGridComboItem>();
            main_combo.AddRange(Combo);

            //добавить дополнительные номенклатуры
            main_combo.AddRange(GetCompCombo(nome_id));

            //возврат из метода
            if (main_combo.Count == 0) return null;
            else return main_combo;
        }

        private static List<DataGridComboItem> GetCompCombo(string nome_id)
        {
            //найти все дополнительные номенклатуры
            return  _compoundCombo.Where(dgvi => dgvi.nome_include.Contains(nome_id)).ToList<DataGridComboItem>();
            //throw new NotImplementedException();
        }

        internal static IEnumerable<string> CreateStringArray(DataRow dataRow)
        {
            //найти номенклатуру
            string nome_id = CellHelper.FindCell(dataRow, "nome_1C").ToString();

            //посмотреть присоединяется ли к ней доп списание
            List<DataGridComboItem> list_items = Create(nome_id);


            if (list_items == null) return null;
            //сформировать список 
            list_items.Select<DataGridComboItem,string>(dgci=>dgci.urr_name);

            return list_items.Select<DataGridComboItem, string>(dgci => dgci.urr_name);
        }
    }

    class CellHelper
    {
        internal int RowIndex;
        internal int ColumnIndex;
        internal int ColumnName;
        internal object CellValue;

        private static List<DataGridViewComboBoxCell> _parent;
        internal static List<DataGridViewComboBoxCell> Parent
        {
            get
            {
                if (_parent == null) _parent = new List<DataGridViewComboBoxCell>();
                return _parent;
            }
        }

        internal static int FindColumnIndex(DataGridView gridView, string colname)

        {
            int index = -1;
            foreach (DataGridViewColumn col in gridView.Columns)
            {
                if (col.DataPropertyName == colname)
                {
                    index = col.Index;
                    break;
                }
            }
            return index;
        }

        internal static int FindColumnIndex(DataTable gridView, string colname)
        {
            int index = -1;
            foreach (DataColumn col in gridView.Columns)
            {
                if (col.ColumnName == colname)
                {
                    index = gridView.Columns.IndexOf(col);
                    break;
                }
            }
            return index;
        }

        internal static DataRow FindRow(DataTable gridView, string colname,string value)
        {
            int colIndex=CellHelper.FindColumnIndex(gridView,colname);
            foreach (DataRow row in gridView.Rows)
            {
                if (row.ItemArray[colIndex].ToString() == value)
                {
                    return row;
                }
            }
            return null;
        }

        internal static object FindCell(DataGridView gridView, int rowIndex, string colName)
        {
            object val = null;
            int colIndex=-1;
            
            colIndex=FindColumnIndex(gridView,colName);

            if (colIndex == -1)
            {
                return null;
            }
            val = FindCell(gridView, rowIndex, colIndex);

            return val;
        }
                    
        internal static object FindCell(DataGridView gridView,int rowIndex,int colIndex) 
        {
            DataGridViewCell cell = gridView.Rows[rowIndex].Cells[colIndex];
            if (cell!=null) return cell;
            return null;
        }
        

        internal static int FindColumn(DataTable gridView, string colname) 
        {
            int index = -1;
            foreach (DataColumn col in gridView.Columns)
            {
                if (col.ColumnName.ToLower() == colname.ToLower())
                {
                    index=gridView.Columns.IndexOf(col);
                }
            }
            return index;
        }

        internal static object FindCell(DataTable gridView, int rowIndex, string colName)
        {
            object val = null;
            int colIndex = -1;

            colIndex = FindColumn(gridView, colName);

            if (colIndex == -1)
            {
                return null;
            }
            val = FindCell(gridView, rowIndex, colIndex);

            return val;
        }

        internal static object FindCell(DataTable gridView, int rowIndex, int colIndex)
        {
            object cell = gridView.Rows[rowIndex].ItemArray[colIndex];
            if (cell != null) return cell;
            return null;
        }

        internal static object FindCell(DataRow rowData, string colName)
        {
            object val = null;
            int colIndex = -1;

            DataTable gridView = rowData.Table;
            colIndex = FindColumn(gridView, colName);
            int rowIndex = gridView.Rows.IndexOf(rowData);


            if (colIndex == -1)
            {
                return null;
            }
            val = FindCell(gridView, rowIndex, colIndex);

            return val;
        }

        internal static object FindCell(DataRow rowData, int colIndex)
        {
            object val = null;
            DataTable gridView = rowData.Table;
            int rowIndex = gridView.Rows.IndexOf(rowData);
            if (colIndex == -1)
            {
                return null;
            }
            val = FindCell(gridView, rowIndex, colIndex);
            return val;
        }


        #region trash
        //internal static object SetCellValue<T,V>(T gridView, int rowIndex, string colName) 
        //    where T : DataGridView
        //    where V : object
        //{
        //    object val = null;
        //    int colIndex = -1;
        //    colIndex = FindColumnIndex(gridView, colName);
        //    val = FindCellValue(gridView, rowIndex, colIndex);

        //    if (colIndex == -1)
        //    {
        //        return null;
        //    }
        //    return val;
        //}

        //internal static object SetCellValue<T, V>(T gridView, int rowIndex, int colIndex)
        //    where T : DataGridView
        //    where V : object
        //{
        //    DataGridViewCell cell = gridView.Rows[rowIndex].Cells[colIndex];
        //    if (cell != null) return cell;
        //    return null;
        //}
        #endregion


        #region зачатки орм к базе

        internal static DataTable GetDocumentById(string docId)
        {
            return SqlWorker.SelectFromDBSafe("SELECT t_Doc.* FROM t_Doc WHERE (((t_Doc.doc_id)=" + docId + "));", "t_Doc");
        }

        public static T GetConfValue<T>(string propName, T default_value) //where T:struct
        {
            //заполнить поля
            T val = default_value;
            object o = GetConfValue(propName);

            if (o != null)
            {
                try
                {
                    val = (T)Convert.ChangeType(o, typeof(T));
                    return val;
                }
                catch (Exception e)
                { return default_value; }
            }
            return default_value;
        }

        internal static object GetConfValue(string value_name)
        {
            string query = "SELECT t_Conf.conf_value FROM t_Conf WHERE (((t_Conf.conf_param)=\"" + value_name + "\"));";
            try
            {
                DataTable dt = SqlWorker.SelectFromDBSafe(query, "t_Conf");
                object result = CellHelper.FindCell(dt, 0, 0);

                if (result == null) return null;
                else return result;
            }
            catch (Exception ex)
            {
                MDIParentMain.log.Error("Ошибка при выполнении запроса "+query+" : ",ex);
                return null;
            }
        }

        #endregion
    }
}

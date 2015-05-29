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


 }
}

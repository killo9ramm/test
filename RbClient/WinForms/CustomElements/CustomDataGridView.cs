using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace RBClient.WinForms.CustomElements
{
    class CustomDataGridView : DataGridView
    {
        public class DataGridViewCustomRow : DataGridViewRow
        {
        }

        public class DataGridViewCustomCell : DataGridViewTextBoxCell
        {
            public override DataGridViewAdvancedBorderStyle AdjustCellBorderStyle(
                DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput,
                DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceHolder,
                bool singleVerticalBorderAdded,
                bool singleHorizontalBorderAdded,
                bool firstVisibleColumn,
                bool firstVisibleRow)
            {
                // Customize the top border of cells in the first row and the 
                // right border of cells in the first column. Use the input style 
                // for all other borders.
                //dataGridViewAdvancedBorderStylePlaceHolder.Left =
                //    DataGridViewAdvancedCellBorderStyle.NotSet;

                dataGridViewAdvancedBorderStylePlaceHolder.Top =
                    DataGridViewAdvancedCellBorderStyle.OutsetDouble;

                //dataGridViewAdvancedBorderStylePlaceHolder.Right =
                //    DataGridViewAdvancedCellBorderStyle.NotSet;
                //dataGridViewAdvancedBorderStylePlaceHolder.Bottom =
                //    DataGridViewAdvancedCellBorderStyle.NotSet;

                return dataGridViewAdvancedBorderStylePlaceHolder;
            }

            public static Font SFont = new Font("TimesNewRoman", 10, FontStyle.Bold);

            public DataGridViewCustomCell()
                : base()
            {
                ReadOnly = false;
                Style.Font = SFont;
                Style.Alignment = DataGridViewContentAlignment.BottomCenter;
            }
        }
    }

}

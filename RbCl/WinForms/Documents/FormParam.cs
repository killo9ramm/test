using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RBClient
{
    public partial class FormParam : Form
    {
        public FormParam()
        {
            InitializeComponent();

            FormClosing+=new FormClosingEventHandler(FormParam_FormClosing);
        }

        private void FormParam_FormClosing(object sender, EventArgs e)
        {
            CParam.Init();
        }
        private void FormParam_Load(object sender, EventArgs e)
        {
            try
            {
                CBData _data = new CBData();

                // заполним данные по истории отправки сообщений
                dg_Param.DataSource = _data.GetParam();

                dg_Param.Columns[0].HeaderText = "Код";
                dg_Param.Columns[0].Width = 50;
                dg_Param.Columns[0].ReadOnly = true;

                dg_Param.Columns[1].HeaderText = "Параметр";
                dg_Param.Columns[1].Width = 150;
                dg_Param.Columns[1].ReadOnly = true;

                dg_Param.Columns[2].HeaderText = "Значение";
                dg_Param.Columns[2].Width = 400;
                dg_Param.Columns[2].ReadOnly = false;
                dg_Param.Columns[2].DefaultCellStyle.BackColor = Color.Pink;

                dg_Param.Columns[3].HeaderText = "Подразделение";
                dg_Param.Columns[3].Width = 150;
                dg_Param.Columns[3].ReadOnly = false;
                dg_Param.Columns[3].DefaultCellStyle.BackColor = Color.Pink;

            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        private void dg_Param_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                CBData _data = new CBData();
                _data.ParamUpdate(dg_Param[0, dg_Param.CurrentCell.RowIndex].Value.ToString(), dg_Param[2, dg_Param.CurrentCell.RowIndex].Value.ToString(), dg_Param[3, dg_Param.CurrentCell.RowIndex].Value.ToString());
            }
            catch (Exception exp)
            {
                throw exp;
            }

        }
    }
}

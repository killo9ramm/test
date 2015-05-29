using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace RBClient
{
    public partial class FormSalesReport : Form
    {
        public FormSalesReport()
        {
            InitializeComponent();
        }

        private void SalesReport2Form_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetBounds(0, 0, Parent.Width - 4, Parent.Height - 4);
                UpdateReport(checkBox_AllKKM.Checked);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateReport(checkBox_AllKKM.Checked);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private void UpdateReport(bool allKKM)
        {
            try
            {
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                reportViewer1.LocalReport.ReportPath = CParam.AppFolder + "\\SalesReport.rdlc";
                
                int m_teremok_id = Convert.ToInt32(CParam.TeremokId);

                CBData _data = new CBData();
                String name = _data.TeremokName(m_teremok_id);
                if (name == null)
                {
                    name = "";
                }

                ReportParameter[] parameters = new ReportParameter[2];
                parameters[0] = new ReportParameter("Date_From", DateTime.Today.ToString("dd/MM/yyyy"));
                parameters[1] = new ReportParameter("Name", name);
                reportViewer1.LocalReport.SetParameters(parameters);

                CBData data = new CBData();
                DataTable salesTable = data.SalesReport(DateTime.Today, allKKM);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("RBADataSet_SalesReport2_v_SalesReport2", salesTable));

                reportViewer1.RefreshReport();
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private void checkBox_AllKKM_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateReport(checkBox_AllKKM.Checked);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private void button_TypeReport_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateReport(checkBox_AllKKM.Checked);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
}

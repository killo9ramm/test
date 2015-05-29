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
    public partial class FormZReport : Form
    {
        public int m_doc_id;

        public FormZReport()
        {
            InitializeComponent();
        }

        private void FormZReport_Load(object sender, EventArgs e)
        {
            try
            {
                IninData();                
            }
            catch (Exception _exp)
            {
                MessageBox.Show(_exp.Message);
            }
        }

        private void IninData()
        {
            CZReportHelper _zreport = new CZReportHelper();
            string _z_rep = "";
            string _zkks = "";
            string _z_date = "";
            string _z_total = "";
            string _z_total_return = "";
            string _z_cash = "";
            string _z_cash_return = "";
            string _z_card = "";
            string _z_card_return = "";
            string _z_cupon = "";
            string _z_cupon_return = "";
            string _z_dinner = "";
            string _z_dinner_return = "";
            
            // формируем репорт            
            try
            {
                _zreport.GetZReportDetail(ref _z_rep, ref _z_date, ref _z_total, ref _z_total_return,
                    ref _z_cash, ref _z_cash_return, ref _z_card, ref _z_card_return, ref _z_cupon, ref _z_cupon_return,
                    ref _z_dinner, ref _z_dinner_return, ref _zkks, m_doc_id);
                tb_date.Text = _z_date;
                tb_smena.Text = _z_rep;
                tb_summ.Text = _z_total;
                tb_annul.Text = _z_total_return;
                tb_kks.Text = _zkks;
                tb_nal.Text = _z_cash;
                tb_rnal.Text = _z_cash_return;
                tb_card.Text = _z_card;
                tb_rcard.Text = _z_card_return;
                tb_cup.Text = _z_cupon;
                tb_rcup.Text = _z_cupon_return;
                tb_dinner.Text = _z_dinner;
                tb_rdinner.Text = _z_dinner_return;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        // закрыть окно
        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}


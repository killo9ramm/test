using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using RBClient.Classes;
using RBClient.Classes.InternalClasses.Models;

namespace RBClient
{
    public partial class FormSpisok : Form
    {
        public int doc_id = 0;
        public int nome_id;
        public bool is_updated = false; // true = данные обновлены
        public int _check_count = 0;
    
        public FormSpisok()
        {
            InitializeComponent();
        }

        private void FormSpisok_Load(object sender, EventArgs e)
        {
            try
            {
                CBData _data = new CBData();
                ddlTeremok.DisplayMember = "nome_name";
                ddlTeremok.ValueMember = "nome_id";

                
                ddlTeremok.DataSource = StaticConstants.FullNomenclatureList;//RbClientGlobalStaticMethods.GetFullNomeList(doc_id);

                //if (StaticConstants.FullNomenclatureList == null)
                //{
                //    DataTable dt=_data.NomenclatureFullList(doc_id);
                //    ddlTeremok.DataSource = dt;
                //    StaticConstants.FullNomenclatureList = dt;
                //}
                //else
                //{
                //    ddlTeremok.DataSource = StaticConstants.FullNomenclatureList;
                //}
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        

        private void button_OK_Click(object sender, EventArgs e)
        {
            AddNomenclature();
        }

        private void AddNomenclature()
        {
            try
            {
                nome_id = Convert.ToInt32(ddlTeremok.SelectedValue.ToString());

                // получить параметры номенклатуры
                CBData _data = new CBData();

                t_Doc doc = new t_Doc().SelectFirst<t_Doc>("doc_id=" + doc_id);

                if(doc!=null&&(doc.doc_type_id==9||doc.doc_type_id==13))
                {
                    t_Nomenclature nome = new t_Nomenclature().SelectFirst<t_Nomenclature>("nome_id=" + nome_id);
                    t_Order2ProdDetails new_ord_detail = new t_Order2ProdDetails() {
                        opd_doc_id=doc_id,
                        opd_nome_id=nome.nome_id,
                        opd_ed=nome.nome_ed,
                        opd_bed=nome.nome_bed,
                        opd_K = nome.nome_K,//decimal.Parse(CUtilHelper.ParceAmount(nome.nome_K, 1)),
                        opd_quota=0,
                        opd_maxquota=0
                    };
                    new_ord_detail.Create();
                }
                else{
                    _data.OrderAddNewItem(doc_id, nome_id);
                }
                is_updated = true;

                this.Close();
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormSpisok_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                AddNomenclature();
            }
        }    
    }  
}

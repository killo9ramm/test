using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.Classes.DocumentClasses;
using RBClient.Classes;
using RBClient.Classes.InternalClasses.Models;
using RBClient.WinForms.ViewModels;
using RBClient.Classes.CustomWindows;
using System.Collections;
using CustomControls;

namespace RBClient.WinForms.Views
{
    public partial class InkassForm : BaseForm
    {
        private InkassForm()
        {
            InitializeComponent();
        }
        public InkassForm(OrderClass order)
        {
            InitializeComponent();
            Order = order;
            Load += (s, e) =>
            {
                SafeLoad(form_loaded,s,e);
            };
        }

        class InvoiceType
        {
            public string StringName { get; set; }
            public string StringGuid { get; set; }
        }

        private void form_loaded(object sender, EventArgs e)
        {
            panel1.Enabled = !IsDocumentBlocked;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.DoubleBuffered(true);

            var ls = (from a in rht<t_Inkass_TypeRef>()
                             .Values.OfType<t_Inkass_TypeRef>()
                      select new InvoiceType
                      {
                          StringName = a.ri_name,
                          StringGuid = a.ri_guid
                      }).ToList();

            UpdateSource();

            StyleGrid(ls);
        }

        private void StyleGrid(List<InvoiceType> ls)
        {


            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            DataGridViewComboBoxColumn dcm = new DataGridViewComboBoxColumn();
            dcm.HeaderText = "Вид инкассации";
            dcm.DataSource = ls;
            dcm.DisplayMember = "StringName";
            dcm.ValueMember = "StringGuid";
            dcm.DataPropertyName = "Ink_type";
            dcm.Name = "InkTypeCol";
            dataGridView1.Columns.Add(dcm);
            dcm.Width = 140;
            dcm.DisplayIndex = 2;

            dataGridView1.Columns[0].Visible = false;
            
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[1].Width = 100;

            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].HeaderText = "Cумма инкассации(сдано)";
            dataGridView1.Columns[3].Width = 190;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[3].DefaultCellStyle.Format = "0.00 руб.";

            dataGridView1.Columns[4].HeaderText = "Остаток(деньги+размен)";
            dataGridView1.Columns[4].Width = 160;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[4].DefaultCellStyle.Format = "0.00 руб";

            dataGridView1.Columns[5].HeaderText = "Подготовлено к сдаче";
            dataGridView1.Columns[5].Width = 170;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[5].DefaultCellStyle.Format = "0.00 руб";

            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[8].Visible = false;

            
            dataGridView1.DataError += (s, e) =>
            {
                WpfCustomMessageBox.Show("Неверный формат числа!", "Внимание", System.Windows.MessageBoxButton.OK);
            };

            dataGridView1.CellParsing += (s, e) =>
            {
                try
                {
                    if (e.ColumnIndex != 6)
                    {
                        string val = ((string)e.Value).Replace(" руб", "");
                        if (val.IndexOf(".") != -1) val = val.Replace('.', ',');
                        e.Value = decimal.Parse(val, System.Globalization.NumberStyles.AllowDecimalPoint);
                        e.ParsingApplied = true;
                    }
                }catch(Exception ex)
                {
                    e.ParsingApplied = false;
                }
            };
        }

        private void btn_addRow_Click(object sender, EventArgs e)
        {
            AddInkass();
        }

        private void AddInkass()
        {
            t_Inkass o = new t_Inkass().SelectFirst<t_Inkass>("istemplate=-1");
            if (o != null)
            {
                ModelItemClass mi = OrderFactory.ConstructDetail(Order, o);
                AddToSource((InkassViewModelItem)Order.AddDetail(mi, true));
            }
        }

        private void AddToSource(InkassViewModelItem mi)
        {
            MarkList = Order.ViewsCollection.OfType<InkassViewModelItem>().ToList();
            ListHash.Add(mi.Id, mi);
            dataGridView1.DataSource = MarkList;
        }

        private void UpdateSource()
        {
            MarkList = Order.ViewsCollection.OfType<InkassViewModelItem>().ToList();
            MarkList.ForEach(a =>
            {
                ListHash.Add(a.Id, a);
            });
            dataGridView1.DataSource = MarkList;
        }

        private void DelFromSource(InkassViewModelItem mi)
        {
            MarkList = Order.ViewsCollection.OfType<InkassViewModelItem>().ToList();
            ListHash.Remove(mi.Id);
            dataGridView1.DataSource = MarkList;
        }

        List<InkassViewModelItem> MarkList;
        Hashtable ListHash=new Hashtable();

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (WpfCustomMessageBox.Show("Действительно удалить строку?", "Внимание", System.Windows.MessageBoxButton.OKCancel) ==
                System.Windows.MessageBoxResult.OK)
            {
                if (dataGridView1.CurrentCell != null)
                {
                    InkassViewModelItem mvmi = (InkassViewModelItem)ListHash[dataGridView1[0, dataGridView1.CurrentCell.RowIndex].Value];
                    Order.RemoveViewDetail(mvmi, true);
                    DelFromSource(mvmi);
                }
            }
        }
    }    
}

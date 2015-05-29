using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;

using System.Collections;
using RBClient.Classes;
using RBClient.Classes.CustomWindows;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes.DocumentClasses;

namespace RBClient
{
    public partial class FormExchange : Form
    {
        public bool m_update = false;   // true = требуется перезагрузка системы        

        public FormExchange()
        {
            InitializeComponent();
        }        

        private void InitData()
        {
            // ищем документы, которые нужно послать
            string _d;
            try
            {
                CBData _data = new CBData();
                DataTable _dt = _data.GetDoc2Send();
                checkedListBox_Tasks.Items.Clear();
                foreach (DataRow _row in _dt.Rows)
                {
                    _d = _row[2].ToString() + ": " + _row[0].ToString() + " №:" + _row[1].ToString() + ": (от " + _row[3].ToString() + ")";
                    checkedListBox_Tasks.Items.Add(_d, true);
                }

                // заполним данные по истории отправки сообщений
                dataGridView_HistoryExchange.DataSource = _data.GetExchangeHistory();

                dataGridView_HistoryExchange.Columns[0].HeaderText = "Документ";
                dataGridView_HistoryExchange.Columns[0].Width = 120;
                dataGridView_HistoryExchange.Columns[0].ReadOnly = true;

                dataGridView_HistoryExchange.Columns[1].HeaderText = "Номер";
                dataGridView_HistoryExchange.Columns[1].Width = 50;
                dataGridView_HistoryExchange.Columns[1].ReadOnly = true;

                dataGridView_HistoryExchange.Columns[2].HeaderText = "К отправке";
                dataGridView_HistoryExchange.Columns[2].Width = 130;
                dataGridView_HistoryExchange.Columns[2].ReadOnly = true;

                dataGridView_HistoryExchange.Columns[3].HeaderText = "Отправлен";
                dataGridView_HistoryExchange.Columns[3].Width = 130;
                dataGridView_HistoryExchange.Columns[3].ReadOnly = true;

                dataGridView_HistoryExchange.CellDoubleClick+=new DataGridViewCellEventHandler(dataGridView_HistoryExchange_CellDoubleClick);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public void dataGridView_HistoryExchange_CellDoubleClick(object sender,DataGridViewCellEventArgs e)
        {
            string doc_id = ((DataGridViewTextBoxCell)CellHelper.FindCell(dataGridView_HistoryExchange, e.RowIndex, 1)).Value.ToString();
            string doc_name = ((DataGridViewTextBoxCell)CellHelper.FindCell(dataGridView_HistoryExchange, e.RowIndex, 0)).Value.ToString();
            t_Doc doc_to_update = new t_Doc().SelectFirst<t_Doc>("doc_id=" + doc_id);
            t_DocStatusRef dsr = new t_DocStatusRef().SelectFirst<t_DocStatusRef>("doctype_id=" + doc_to_update.doc_type_id + " AND statustype_id=1");

          if(doc_to_update.doc_status_id==dsr.docstatusref_id)
          {
              return;
          }

            //запустить окно меню
          if (WpfCustomMessageBox.Show("Вы действительно хотите отправить документ \"" + doc_id + " " + doc_name + "\" повторно?", "Внимание!!!", System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
            {
                if (StaticConstants.MainWindow.EnterPasswordWindow() == DialogResult.OK)
                {
                    if (doc_to_update.doc_type_id==5)
                    {
                        WpfCustomMessageBox.Show("Чтобы повторно отправить Z-отчет удалите его из общего списка документов и он повторно отправится автоматически!", "Внимание!!!");
                        return;
                    }
                    if (new int[] { 4, 5, 7, 8, 22, 28, 30, 31, 32, 34, 35, 36, 37 }.Contains(doc_to_update.doc_type_id))
                    {
                        WpfCustomMessageBox.Show("Данный документ не может быть помечен на повторную отправку!", "Внимание!!!");
                        return;
                    }
                    
                    doc_to_update.doc_status_id = dsr.docstatusref_id;
                    doc_to_update.doc_desc = "Помечен на повторную отправку!";
                    doc_to_update.Update();
                    FormExchange_Load(sender,e);
                    return;
                }
                else
                {
                    WpfCustomMessageBox.Show("Пароль неверный!");
                }
            }
        }

        public void exchange_flash()
        {
            try
            {
                InitData();
                ToDo();
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private void button_Exchange_Click(object sender, EventArgs e)
        {
            try
            {
                ToDo();
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private void button_ReExchange_Click(object sender, EventArgs e)
        {
            WpfCustomMessageBox.Show("Чтобы повторно отправить документ, щелкните по нему 2 раза", "Внимание!!!");
        }

        public void ToDo()
        {
            string[] _s;
            ArrayList _array = new ArrayList();
            char[] _separator = ":".ToCharArray();
            CBData _data = new CBData();

            try
            {
                int _i = 0;
                for (; _i < checkedListBox_Tasks.Items.Count; _i++)
                {
                    if (checkedListBox_Tasks.GetItemChecked(_i))
                    {
                        _s = checkedListBox_Tasks.Items[_i].ToString().Split(_separator);
                        int doc_id = 0;
                        if (int.TryParse(_s[2], out doc_id) && doc_id!=0)
                        {
                            OrderClass order = OrderClass.CreateOrderClass(doc_id);
                            AddTaskToExchange(order);
                        }            
                    }
                }

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this.Close();
            }
        }

        private void AddTaskToExchange(OrderClass order)
        {
            if (order.SendType == (int)OrderClass.DocumentSendTypes.Ftp)
            {
                StaticConstants.CBData.AddTaskExchange(order.CurrentDocument.doc_id.ToString());
            }
            else
            {
                //webservicetask
                order.Document_MarkToSending();
            }
            
        }
 
        private void FormExchange_Load(object sender, EventArgs e)
        {
            try
            {
                InitData();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_SendZReport_Click(object sender, EventArgs e)
        {
            FormSendZReportManual _form = new FormSendZReportManual();
            _form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormFlash _form = new FormFlash();
            _form.ShowDialog();
        }       
    }
}

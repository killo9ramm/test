using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Xml;
using System.Xml.Linq;
using RBClient.Classes;
using RBClient.Classes.CustomClasses;
using RBClient.Classes.WindowMarochOtch;
using RBClient.Classes.CustomWindows;
using System.IO;
using RBClient.Classes.InternalClasses.Models;

namespace RBClient
{
    public partial class FormDoc : Form
    {
        public int m_teremok_id;
        private int m_doctype_id = 0; // 0 - id по умолчанию, значит все документы
        public int flash;

        public FormDoc()
        {
            InitializeComponent();
            StaticConstants.FormDocGrid = dataGridView_Order;

            this.Activated+=new EventHandler(FormDoc_Activated);
        }

        public void FormDoc_Activated(object sender,EventArgs e)
        {
            StaticConstants.MainGridUpdate();
        }

        public void RefreshForm()
        {
            this.dataGridView_Order.Refresh();
        }

        private void FormDoc_Load(object sender, EventArgs e)
        {
            OleDbConnection _conn = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                // список ресторанов
                InitTeremokList(_conn);

                // список типов документов
                InitDocType(_conn);

                // заполнить таблицу
                IninData();                
                FormatGrid();

                Cursor.Current = Cursors.Default;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        } 

        private void InitTeremokList(OleDbConnection conn)
        {
            try
            {
                CBData _data = new CBData();
                DataTable _dt = _data.TeremokView(conn);

                foreach (DataRow _row in _dt.Rows)
                {
                    toolStripComboBox_Teremok.Items.Add(_row["teremok_name"]);
                }
                // установим выбранный теремок
                toolStripComboBox_Teremok.SelectedItem = _data.TeremokName(m_teremok_id);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        private void InitDocType(OleDbConnection conn)
        {
            try
            {
                toolStripComboBox_Doc.Items.Add("Все документы");

                CBData _data = new CBData();
                DataTable _dt = _data.DocRef(conn);

                foreach (DataRow _row in _dt.Rows)
                {
                    toolStripComboBox_Doc.Items.Add(_row["doctype_name"]);
                }
                // установим тип ВСЕ                
                toolStripComboBox_Doc.SelectedIndex = 0;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        private void InitNomeclatureList(OleDbConnection conn)
        {
            try
            {
                CBData _data = new CBData();
                DataTable _dt = _data.TeremokView(conn);

                foreach (DataRow _row in _dt.Rows)
                {
                    toolStripComboBox_Teremok.Items.Add(_row["teremok_name"]);
                }
                // установим выбранный теремок
                toolStripComboBox_Teremok.SelectedItem = _data.TeremokName(m_teremok_id);
            }
            catch (Exception _exp)
            {
               throw _exp;
            }
        }
        
        public void FormatGrid()
        {
            try
            {
                // настройка стиля
                dataGridView_Order.Columns[0].HeaderText = "Код";
                dataGridView_Order.Columns[0].Width = 50;
                dataGridView_Order.Columns[0].ReadOnly = true;

                dataGridView_Order.Columns[1].HeaderText = "Дата";
                dataGridView_Order.Columns[1].Width = 120;
                dataGridView_Order.Columns[1].ReadOnly = true;

                dataGridView_Order.Columns[2].HeaderText = "Документ";
                dataGridView_Order.Columns[2].Width = 140;
                dataGridView_Order.Columns[2].ReadOnly = true;

                dataGridView_Order.Columns[3].HeaderText = "Статус";
                dataGridView_Order.Columns[3].Width = 140;
                dataGridView_Order.Columns[3].ReadOnly = true;

                dataGridView_Order.Columns[4].HeaderText = "Информация";
                dataGridView_Order.Columns[4].Width = 470;
                dataGridView_Order.Columns[4].ReadOnly = true;

                dataGridView_Order.Columns[5].Visible = false;
                dataGridView_Order.Columns[6].Visible = false;
                dataGridView_Order.Columns[7].Visible = false;
                /*
                dataGridView_Order.Columns[7].HeaderText = "Ресторан";
                dataGridView_Order.Columns[7].Width = 200;
                dataGridView_Order.Columns[7].ReadOnly = true;
                 */
            }
            catch
            {
            }
        }


        public void IninData()
        {
            CBData _ref = new CBData();
            try
            {
                // связать с данными
                dataGridView_Order.DataSource = _ref.ViewDoc(m_doctype_id, m_teremok_id);
                this.SetBounds(0, 0, Parent.Width - 4, Parent.Height - 4);
            }
            catch (Exception _exp)
            {
               MessageBox.Show(_exp.Message);
            }
        }

        // сделать подцветку
        private void dataGridView_Order_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {            
            CBData _data = new CBData();
            OleDbConnection _conn = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                foreach (DataGridViewRow _row in dataGridView_Order.Rows)
                    dataGridView_Order[3, _row.Index].Style.BackColor = Color.FromName(_data.GetStatusColor(_conn, _row.Cells[6].Value.ToString()));
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }      

        private void dataGridView_Order_DoubleClick(object sender, EventArgs e)
        {
            //MessageBox.Show("Документ заблокирован в 1С: " + "Кригель Сергей" + " \nРедатирование запрещено, октрыть для чтения?\nРедактирование возможно при закрытии документа в 1С, если вам нужно его редактировать, позвоните вашему управляющему.");
            Cursor.Current = Cursors.WaitCursor;
            OpenDoc();
        }

        // открыть документ
        private void OpenDoc()
        {
            try
            {
                CBData _data = new CBData();
                int _teremok_id = m_teremok_id;
                int _doc_type = Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[5].Value);
                int _doc_id = Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[0].Value);
                DateTime _doc_date = Convert.ToDateTime(dataGridView_Order.CurrentRow.Cells[1].Value);

                #region 28 - табель
                if (_doc_type == 28)
                {
                    WindowStarter.Открыть_планирование_смен(_doc_id);
                    return;
                }
                #endregion

                #region 30- posdisplay
                if (_doc_type == 38)
                {
                    if (WpfCustomMessageBox.Show("Вы действительно хотите обновить программу PosDisplay на кассах???", "Внимание!!!", System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                    {

                        Action act = () =>
                        {
                            UpdateClass updcls = new UpdateClass() { LogEvent = MDIParentMain.Log };
                            updcls.PosDisplayLoadOnKKm();
                            VideoUpdateClass.sendVideoFilesToKkm();
                            ImageUpdateClass.sendVideoFilesToKkm();
                        };
                        act.BeginInvoke(null, null);
                        
                    }
                        return;
                }
                #endregion

                #region 37 - marotch
                if (_doc_type == 37)
                {
                    WindowStarter.Открыть_смена_и_обеды(_doc_id);
                    return;
                }
                #endregion

                #region 8
                // Отработка нетиповых документов
                if (_doc_type == 8)
                {
                    MessageBox.Show("Информация об изменениях в шаблоне отправлется по электронной почте");
                    return;
                }
#endregion

                #region 31,32,33 - ref spisan
                //если пытаются открыть шаблоны списаний
                if (_doc_type == 31 || _doc_type == 32 || _doc_type == 33)
                {
                    MessageBox.Show("Информация об изменениях в шаблоне отправлется по электронной почте");
                    return;
                }
#endregion

                #region 22
                // Отработка нетиповых документов
                if (_doc_type == 22)
                {
                    MessageBox.Show("Для просмотра инструкции нажмите на значок 'Справка'");
                    return;
                }
#endregion

                #region 5 - Z-report
                if (_doc_type == 5) // это Z-отчет
                {
                    // Z-отчеты
                    FormZReport _form_z = new FormZReport();
                    _form_z.m_doc_id = _doc_id;
                    _form_z.ShowDialog();
                    return;
                }
                #endregion

                #region 4 - cards
                if (_doc_type == 4) // это карты
                {
                    //MessageBox.Show("Информация по картам доступна на кассе");
                    if(WpfCustomMessageBox.Show("Вы действительно хотите запустить процесс обновления карточек пользователей на кассы?",
                        "Внимание!!!!",System.Windows.MessageBoxButton.OKCancel)==System.Windows.MessageBoxResult.OK)
                    {
                       DirectoryInfo dir_in = new DirectoryInfo(CParam.AppFolder + "\\Card\\");

                       List<FileInfo> cards_file_list = dir_in.GetFiles().ToList();
                       List<t_Doc> cards_doc_list = new t_Doc().Select<t_Doc>("doc_type_id=4");
                       if (cards_doc_list != null && cards_file_list.Count>0)
                       {
                           StaticConstants.MainWindow.UpdateCardsToKkm(cards_file_list, cards_doc_list.Last());
                       }
                       
                    }
                    return;
                }
                #endregion

                #region 15 - вход. накладная
                if (_doc_type == 15) // это вх. накладная
                {
                    if (dataGridView_Order.CurrentCell.ColumnIndex == 3) // кликаем по статусу и проверяем, можно ли редактировать?
                    {
                        // накладная
                        if (Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[6].Value) == _data.GetStatusID(null, _doc_type, 4))
                        {
                            // корректировать?
                            if (MessageBox.Show("Внести изменения в накладную?", "Вопрос", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                            {
                                _data.DocUpdateState(_doc_id, _data.GetStatusID(null, _doc_type, 1), "коррекция "); // откатить на новый статус
                                IninData();
                            }
                        }
                    }
                }
                #endregion

                #region 19 - инкассация
                //если пытаются открыть шаблоны списаний
                #if(INKASS)
                if (_doc_type == 19)
                {
                    WindowStarter.ОткрытьИнкассацию(_doc_date);
                    return;
                }
                #endif
                #endregion

                if (dataGridView_Order.CurrentCell.ColumnIndex == 3) // кликаем по статусу и проверяем, можно ли редактировать?
                {
                    // если отправлен, то новые действия
                    if (Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[6].Value) == _data.GetStatusID(null, _doc_type, 3))
                    {
                        // проверка на дату
                        if (_doc_date.ToShortDateString() == DateTime.Today.ToShortDateString())
                        {
                            // корректировать?
                            if (MessageBox.Show("Внести изменения в документ?", "Вопрос", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                            {
                                _data.DocUpdateState(_doc_id, _data.GetStatusID(null, _doc_type, 1), "коррекция "); // откатить на новый статус
                                IninData();
                            }
                        }
                        else
                        {
                            if (_doc_type != 28)
                            {
                                MessageBox.Show("Запрещено изменять документы в старом периоде");
                            }
                        }
                    }
                }

                #region 7 - menu
                if (_doc_type == 7) // МЕНЮ
                {
                    int dth;
                    int _max_id;
                    dth = DateTime.Now.Hour;
                    // проверим дату меню
                    CMenuHelper _menu = new CMenuHelper();

                    _max_id = _menu.GetMaxDateMenu();
                    if (Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[0].Value) == _max_id)
                    {
                        if (_menu.GetDateMenu(Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[0].Value)) >= DateTime.Today)
                            if (MessageBox.Show("ВНИМАНИЕ! \n\nПредпологаемая дата обновления меню " + _menu.GetDateMenu(Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[0].Value)).ToLongDateString() + " после 22:00! \nВы уверены, что хотите обновить меню сейчас?! ", "Вопрос", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                UpdateMenu();
                            else
                                return;
                        else
                            UpdateMenu();
                    }
                    else
                    {
                        if (MessageBox.Show(" ВНИМАНИЕ! \n\n Есть более новая версия меню! \n Вы уверены, что хотите установить старое меню на кассы?!", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            UpdateMenu();
                        else
                            return;
                    }
                }
                #endregion

                #region 30 - реклама
                if (_doc_type == 30) // Emark
                {
                    int dth;
                    int _max_id;
                    dth = DateTime.Now.Hour;
                    // проверим дату меню
                    CMenuHelper _menu = new CMenuHelper();

                    _max_id = _menu.GetMaxDateEmark();
                    if (Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[0].Value) == _max_id)
                    {
                        if (_menu.GetDateMenu(Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[0].Value)) >= DateTime.Today)
                            if (MessageBox.Show("ВНИМАНИЕ! \n\nПредпологаемая дата обновления слайдов " + _menu.GetDateMenu(Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[0].Value)).ToLongDateString() + " после 22:00! \nВы уверены, что хотите обновить слайды сейчас?! ", "Вопрос", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                UpdateEmark();
                            else
                                return;
                        else
                            UpdateEmark();
                    }
                    else
                    {
                        if (MessageBox.Show(" ВНИМАНИЕ! \n\n Есть более новая версия слайдов! \n Вы уверены, что хотите установить старые слайды на кассы?!", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            UpdateEmark();
                        else
                            return;
                    }
                }
                #endregion

                #region 34 - реклама видео
                if (_doc_type == 34) // Emark video
                {
                    int dth;
                    int _max_id;
                    dth = DateTime.Now.Hour;
                    // проверим дату меню
                    
                        
                    if(MessageBox.Show(" ВНИМАНИЕ! \n\n Вы уверены, что хотите обновить видео на кассах?!\nНеобходимо включить ВСЕ КАССЫ (включая резервные).", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        VideoUpdateClass.sendVideoFilesToKkmAsync();
                    }
                }
                #endregion 

                #region 35 - реклама картинки
                if (_doc_type == 35) // Emark image
                {
                    int dth;
                    int _max_id;
                    dth = DateTime.Now.Hour;
                    // проверим дату меню


                    if (MessageBox.Show(" ВНИМАНИЕ! \n\n Вы уверены, что хотите обновить картинки на кассах?!\nНеобходимо включить ВСЕ КАССЫ (включая резервные).", "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ImageUpdateClass.sendVideoFilesToKkmAsync();
                    }
                }
                #endregion

                #region 36 - обучающее видео
                if (_doc_type == 36) 
                {
                    int dth;
                    int _max_id;
                    dth = DateTime.Now.Hour;
                    // проверим дату меню


                    MessageBox.Show(" ВНИМАНИЕ! \n\n Обновляем обучающее видео.", "Внимание");
                        VideoUpdateClass.OperateEducArchive();
                }
                #endregion

                #region 16 - возврат
                if (_doc_type == 16)
                {
                    WindowStarter.ОткрытьДокумент(_doc_id, _doc_type);
                    return;
                }
                #endregion

                #region 9,13- списания гп сп
                if (_doc_type == 9||_doc_type == 13) // Emark
                {
                    FormGrid _form_o = new FormGrid();
                    _form_o.MdiParent = this.ParentForm;
                    _form_o.m_doc_id = _doc_id;
                    _form_o.m_teremok_id = _teremok_id;
                    _form_o.Show();
                }
                #endregion

                #region все остальные !=30,7,28,34,35
                else // открываем документ
                {
                    if (_doc_type != 30 && _doc_type != 7 && _doc_type != 28 && _doc_type != 34 && _doc_type != 35)
                    {
                        FormGrid _form_o = new FormGrid();
                        _form_o.MdiParent = this.ParentForm;
                        _form_o.m_doc_id = _doc_id;
                        _form_o.m_teremok_id = _teremok_id;
                        _form_o.Show();
                    }
                }
                #endregion
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        private void UpdateMenu()
        {
            if (MessageBox.Show("ВНИМАНИЕ! \nНеобходимо включить ВСЕ КАССЫ (включая резервные). \nОбновить меню на кассах?", "Вопрос", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MDIParentMain _mdi = (MDIParentMain)this.MdiParent;
                _mdi.UpdateMenu(Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[0].Value));
                IninData();
            }
        }

        private void UpdateEmark()
        {
            if (MessageBox.Show("ВНИМАНИЕ! \nНеобходимо включить ВСЕ КАССЫ (включая резервные). \nОбновить слайды на кассах?", "Вопрос", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MDIParentMain _mdi = (MDIParentMain)this.MdiParent;
                _mdi.EmarkUpdate(Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[0].Value));
                IninData();
            }
        }

        private void toolStripComboBox_State_SelectedIndexChanged(object sender, EventArgs e)
        {
            IninData();
        }               

        private void toolStripButton_Del_Click(object sender, EventArgs e)
        {
            bool _need_refesh = false;
            try
            {
                int _doc_id = Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[0].Value);
                int _doc_type = Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[5].Value);
                int _doc_status_id = Convert.ToInt32(dataGridView_Order.CurrentRow.Cells[6].Value);
                DateTime _doc_datetime = Convert.ToDateTime(dataGridView_Order.CurrentRow.Cells[1].Value);

                if (MessageBox.Show("Вы уверены, что хотите удалить документ №" + _doc_id.ToString(), "Вопрос", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }

                CZReportHelper _zreport = new CZReportHelper();
                CBData _data = new CBData();

                #region Admin deletion
                if (Control.ModifierKeys == (Keys.Control | Keys.Shift))
                {
                    if (_doc_type != 5) // Z-отчет
                    {
                        _data.OrderDelete(_doc_id);
                        MessageBox.Show("Документ №" + _doc_id.ToString() + " удален.");
                        _need_refesh = true;
                    }
                    return;
                }
                     //(Control.ModifierKeys==Keys.Control)
                #endregion


                if (_doc_datetime.AddDays(1) <= DateTime.Now)
                {
                    if (_doc_type != 5)
                    {
                        MessageBox.Show("Нельзя удалять документы с датой меньше текущей");
                        return;
                    }
                }

                
                if (_doc_type == 5) // Z-отчет
                {
                    // удаляем
                    if (_doc_status_id == 22)
                    {
                        _zreport.ZReportDelete(_doc_id);
                        _need_refesh = true;
                    }
                    else
                    {
                        _zreport.ZReportDelete(_doc_id);
                        _need_refesh = true;
                    }
                }
                else
                {
                    if (_doc_status_id == _data.GetStatusID(null, _doc_type, 1))
                    {
                        // удаляем
                        _data.OrderDelete(_doc_id);
                        MessageBox.Show("Документ №" + _doc_id.ToString() + " удален.");
                        _need_refesh = true;
                    }
                    else
                        MessageBox.Show("Документ можно удалить только в статусе Новый");                
                }

                // обновить журнал
                if (_need_refesh)
                    IninData();
            }
            catch (Exception _exp)
            {
                MessageBox.Show(_exp.Message);
            }
        }

        private void toolStripComboBox_Teremok_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CBData _data = new CBData();
                m_teremok_id = Convert.ToInt32(_data.GetTeremokID(toolStripComboBox_Teremok.SelectedItem.ToString()));

                StaticConstants.Teremok_ID = m_teremok_id.ToString();
                StaticConstants.Current_Teremok_ID_int = m_teremok_id;

                IninData();
                
                MDIParentMain _mdi = (MDIParentMain)this.MdiParent;
                _mdi.ChangeTeremok(m_teremok_id, toolStripComboBox_Teremok.SelectedItem.ToString());
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private void toolStripComboBox_Doc_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CBData _data = new CBData();
                m_doctype_id = Convert.ToInt32(_data.GetDocTypeID(toolStripComboBox_Doc.SelectedItem.ToString()));
                
                IninData();
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private void FormDoc_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }

        private void dataGridView_Order_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {            
                OpenDoc();
            }
        }

        private void dataGridView_Order_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Configuration;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Globalization;
using RBClient.ru.teremok.msk;
using System.Threading;
using System.Net;
using System.Data.OleDb;
using RBClient.Classes;
using DevComponents.DotNetBar.Rendering;
using System.Text.RegularExpressions;
using RBClient.Classes.InternalClasses.Models;
using DevComponents.DotNetBar;
using System.Windows.Forms.Integration;
using RBClient.WPF.UserControls;
using RBClient.Classes.InternalClasses;
using System.Windows.Controls.Primitives;
using RBClient.Classes.WindowAddElement;
using RBClient.Classes.CustomClasses;
using RBClient.Classes.DocumentClasses;
using System.Windows.Threading;


namespace RBClient
{
    public partial class FormMark2 : Form
    {
        public int cell_count = 0;
        public string btnValue = "";
        public int _doc_id = 0;
        public string _btn_guid = "";
        public string _btn_guidSmena = "";
        private bool pressBtn = false;

        #region good
        Dictionary<string, string> dict_emploee = null;

        private bool _readdoc = false;
        public bool readDoc
        {
            get
            {
                return _readdoc;
            }
            set
            {
                _readdoc = value;
                if (_readdoc)
                {
                    dataGridViewMain.Enabled = false;           //выключить грид
                    IsDocumentTabelBlocked = true;
                }
            }
        }

        private bool IsDocumentTabelBlocked=false;
        public static DateTime Tabel_Opened_Date;
        private int _btn_time = 0;
        private int dates;
        private DateTime jobStart;
        private DateTime jobEnd;
        private int count_Layout = 0;
        private Rectangle dragBoxSrc;
        private int rowIndexSrc;
        private int rowIndexTar;
        private DataSet _ds = new DataSet();
        private int Total = 0;
        private DateTime _date;

        CBData bd = new CBData();
        ARMWeb systemService = StaticConstants.WebService;
        private BindingSource masterBindingSource = new BindingSource();

        public TabelBackgroundWorker tabWorker;

        public FormMark2()
        {
            InitializeComponent();
        }

        DevComponents.DotNetBar.ButtonX[] myBtn;
        object currentButtonPressed;
        Dictionary<string, Color> color_table=new Dictionary<string, Color>();

        Dictionary<string, System.Windows.Media.SolidColorBrush> color_table1 =
            new Dictionary<string, System.Windows.Media.SolidColorBrush>(); //таблица цветов

        private void FormMark2_KeyUp(object sender,KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
            {
                buttonX1_Click_1(this, null);
            }
        }

        private void dataGridViewMain_DataError(object sender,DataGridViewDataErrorEventArgs e) 
        { 

        }

        protected override void WndProc(ref Message message)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (message.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = message.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }

            base.WndProc(ref message);
        }

        MyPanel smena_button_panel;


        FormAddElement add_user_form;
        
        private void FormGrid_Load(object sender, EventArgs e)
        {
            try
            {
                MDIParentMain.log.Trace("Начинаем загружать табель");

                #region добавляем окно добавления пользователей
                FormAddElement _form = new FormAddElement();
                _form.wpfctl.txbl_Header.Text = "Выберите сотрудника";

                _form.wpfctl.listW_MainList.FontSize = 13;

                _form.wpfctl.main_dock_panel.Children.Clear();
                _form.wpfctl.main_dock_panel.Children.Add(new WpfLoadingControl() { Text = "Загружаю пользователей..." });
                
                _form.wpfctl.Loaded += (s, e1) =>
                {
                    if (!_form.wpfctl.main_dock_panel.Children.Contains(_form.wpfctl.mainListControl))
                    {
                        Dispatcher.CurrentDispatcher
                                    .BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                                {
                                    _form.wpfctl.main_dock_panel.Children.Clear();

                                    List<ModelItemClass> users =
                                            new t_Employee().Select("employee_WorkField=-1 AND Deleted=0")
                                            .Select(a => new ModelItemClass(a)).ToList();

                                    _form.wpfctl.listW_MainList.ItemsSource = users;
                                    _form.wpfctl.main_dock_panel.Children.Add(_form.wpfctl.mainListControl);
                                }));
                    }
                };

                add_user_form = _form;

                #endregion

                #region set hot buttons
                this.KeyPreview = true;
                    this.KeyUp+=new KeyEventHandler(FormMark2_KeyUp);
                #endregion

                #region set tabel date

                //DateTime tabel_date=new DateTime(DateTime.Now.Year,DateTime.Now.Month+1,DateTime.Now.Day);
                //System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                //month_label.Text = " " + mfi.GetMonthName(tabel_date.Month).ToString()+ " "+ tabel_date.Year.ToString();

                month_label.Text = StaticConstants.Tabel_Opened_Date.ToString("MMMMMMMMM", CultureInfo.CurrentCulture) + " " + StaticConstants.Tabel_Opened_Date.Year.ToString();

                #endregion

                #region включен/выключен документ
                if (readDoc == true)
                {
                    panelEx1.Enabled = false;
                }
                #endregion

                #region обновляем статус документа
                CBData bd = new CBData();
                bd.DocUpdateDocStateNew(_doc_id, 1, "новый ");

                this.SetBounds(0, 0, Parent.Width - 4, Parent.Height - 4);
                Cursor.Current = Cursors.Default;

                DataTable ptr = bd.ButtonFullList();
                DataTable idRow;
                DataTable idColumn;

                int totalWorkDays = 0;
                string totalstr = "";
                #endregion

                #region создаем кнопки
                MDIParentMain.log.Trace("Создаем кнопки");
                ElementHost host = new ElementHost();
                host.Size = new System.Drawing.Size(740, 300);
                host.Location=new Point(225,3);
                host.BackColor = Color.Transparent;
                this.panelEx1.Controls.Add(host);

                smena_button_panel = new MyPanel();
                smena_button_panel.margin = 5;
                host.Child = smena_button_panel;
                DataTable ptr1 = bd.ButtonFullList();
                List<t_ButtonTemplate> sm = ClassFactory.CreateClasses<t_ButtonTemplate>(ptr1);

                sm.ForEach(a =>
                {
                    ToggleButton tb = new ToggleButton();
                    tb.Focusable = false;
                    tb.Content = a.bnt_value;
                    tb.MinWidth = 40;
                    tb.Height = 20;
                    tb.ToolTip = a.btn_name;
                    tb.Background = color_table1
                        .CreateOrreturnElement<string, System.Windows.Media.SolidColorBrush, string>(a.btn_SmenaType,
                        a.btn_color, c => new System.Windows.Media.SolidColorBrush(StaticHelperClass.ReturnColor(c)));
                    tb.Click += this.myClickFunction;
                    tb.Tag = a;

                    smena_button_panel.Children.Add(tb);
                });

                fill_color_table();
                #endregion

                #region good
                #region просчет тотал
                MDIParentMain.log.Trace("Добавляем для каждой строки ToTal");

                idRow = bd.returnDTrow(_doc_id);
                foreach (DataRow item in idRow.Rows)
                {                    
                    idColumn = bd.returnDTTotalDays(item[0].ToString());
                    foreach (DataRow item1 in idColumn.Rows)
                    {
                        for (int a = 0; a < idColumn.Columns.Count; a++)
                        {
                            if (item1.ItemArray[a].ToString() != "")
                            {
                                int val = Convert.ToInt32(item1.ItemArray[a]);
                                if (val != 0)
                                {
                                    totalWorkDays++;
                                    Total = Total + val;
                                }
                                totalstr = totalWorkDays + " см. (" + Total + " ч.)";                               
                            }
                        }
                        bd.TotalUpdate(item[0].ToString(), totalstr);
                        Total = 0;
                        totalWorkDays = 0;
                    }
                }

                #endregion

                MDIParentMain.log.Trace("Начинаем загружать грид");
                LoadGrid();
                MDIParentMain.log.Trace("Грид загружен");

            }
            catch (Exception _ex)
            {
                MessageBox.Show(_ex.ToString());
            }
#endregion
        }

        #region itog
        public class Itog_class
        {
            public const string itog_1C = "999999999";
            public const string itog_name = "Итого:";

            internal DataGridViewRow itog_row;
            internal t_Employee employee;
            internal t_MarkItems mark_row;
        }

        Itog_class itog_class;
        

        //проверяет есть ли пользователь итог в таблице - если его нет то создает
        private void check_itog_employee()
        {
            //проверить есть ли итог в таблице пользователей    
            if (itog_class == null) { itog_class = new Itog_class(); }

            t_Employee itog_empl = new t_Employee().SelectFirst<t_Employee>("employee_1C='"+Itog_class.itog_1C+"'");

            //если нет то создать
            if (itog_empl == null)
            {
                itog_empl = new t_Employee() { employee_name = Itog_class.itog_name, employee_1C = Itog_class.itog_1C, employee_WorkField = false };
                itog_empl.Create();
                itog_class.employee = itog_empl;
            }
            else
            {
                itog_class.employee = itog_empl;
            }
        }
        
        //добавляет строку итога в таблицу табеля
        private void itog_insert_into_db(){
            t_MarkItems itog_mark_row = new t_MarkItems().SelectFirst<t_MarkItems>("mark_doc_id=" + _doc_id + " AND mark_name='"+Itog_class.itog_1C+"'");
            if (itog_mark_row == null)
            {
                itog_mark_row = new t_MarkItems() { mark_doc_id = _doc_id, mark_name = Itog_class.itog_1C, mark_row_id=99 };
                itog_mark_row.Create();
                itog_class.mark_row = itog_mark_row;
            }
            else
            {
                itog_class.mark_row = itog_mark_row;
            }
        }

        private void itog_add()
        {
            check_itog_employee(); //проверяет есть ли пользователь итог в таблице - если его нет то создает

            itog_remove();

            itog_insert_into_db(); //добавляет итог в таблицу t_markItems
            
            itog_update_db(false);
        }

        private void itog_set_row()
        {
            if (dataGridViewMain.Rows[dataGridViewMain.RowCount - 1].Cells[1].Value.ToString() == Itog_class.itog_name)
            {
                itog_class.itog_row = dataGridViewMain.Rows[dataGridViewMain.RowCount - 1];
            }
        }

        private void itog_remove()
        {
            t_MarkItems itog_mark_row = new t_MarkItems().SelectFirst<t_MarkItems>("mark_doc_id=" + _doc_id + " AND mark_name='" + Itog_class.itog_1C + "'");
            if (itog_mark_row != null)
            {
                itog_mark_row.Delete();
                if(itog_class!=null)
                    itog_class.mark_row = null;
            }
        }

        private void itog_update_db(bool updateGrid)
        {
            //сделать просчет итога и добавление его в базу
            //сделать запрос подсчета
            DataTable dt=SqlWorker.SelectFromDB("SELECT t_MarkItems.mark_doc_id, Count(t_MarkItems.mark_1) AS [Count-mark_1], Count(t_MarkItems.mark_2) AS [Count-mark_2], Count(t_MarkItems.mark_3) AS [Count-mark_3]" +
                    ", Count(t_MarkItems.mark_4) AS [Count-mark_4], Count(t_MarkItems.mark_5) AS [Count-mark_5], Count(t_MarkItems.mark_6) AS [Count-mark_6], Count(t_MarkItems.mark_7) AS [Count-mark_7]" +
                    ", Count(t_MarkItems.mark_8) AS [Count-mark_8], Count(t_MarkItems.mark_9) AS [Count-mark_9], Count(t_MarkItems.mark_10) AS [Count-mark_10], Count(t_MarkItems.mark_11) AS [Count-mark_11]" +
                    ", Count(t_MarkItems.mark_12) AS [Count-mark_12], Count(t_MarkItems.mark_13) AS [Count-mark_13], Count(t_MarkItems.mark_14) AS [Count-mark_14], Count(t_MarkItems.mark_15) AS [Count-mark_15]" +
                    ", Count(t_MarkItems.mark_16) AS [Count-mark_16], Count(t_MarkItems.mark_17) AS [Count-mark_17], Count(t_MarkItems.mark_18) AS [Count-mark_18], Count(t_MarkItems.mark_19) AS [Count-mark_19]" +
                    ", Count(t_MarkItems.mark_20) AS [Count-mark_20], Count(t_MarkItems.mark_21) AS [Count-mark_21], Count(t_MarkItems.mark_22) AS [Count-mark_22], Count(t_MarkItems.mark_23) AS [Count-mark_23]" +
                    ", Count(t_MarkItems.mark_24) AS [Count-mark_24], Count(t_MarkItems.mark_25) AS [Count-mark_25], Count(t_MarkItems.mark_26) AS [Count-mark_26], Count(t_MarkItems.mark_27) AS [Count-mark_27]" +
                    ", Count(t_MarkItems.mark_28) AS [Count-mark_28], Count(t_MarkItems.mark_29) AS [Count-mark_29], Count(t_MarkItems.mark_30) AS [Count-mark_30], Count(t_MarkItems.mark_31) AS [Count-mark_31]" +
                    "FROM t_MarkItems  " +
                    "WHERE (((t_MarkItems.mark_doc_id)="+_doc_id+")) " +
                    "GROUP BY t_MarkItems.mark_doc_id", "t_MarkItems");
            for (int i = 1; i<dt.Columns.Count; i++)
            {
                string value = ((int)CellHelper.FindCell(dt, 0, i)-1).ToString();
               // if (value == "0") continue;
                StaticHelperClass.SetClassItemValue(itog_class.mark_row.GetType(),itog_class.mark_row,i+5,value);
            }
            //обновить итог
            itog_class.mark_row.Update(true);

            if (updateGrid)
            {
                itog_set_row();
                for (int i = 3; i < itog_class.itog_row.Cells.Count; i++)
                {
                    itog_class.itog_row.Cells[i].Value = StaticHelperClass.ReturnClassItemValue(itog_class.mark_row, i+2);
                }
                    
                //обновить в гриде
            }
        }
        #endregion

        #region good
        private void fill_color_table()
        {
            if (color_table.Count == 0)
            {
                try
                {
        //            string query = "SELECT t_ShiftType.type_guid, t_ShiftType.type_value, t_ShiftType.type_color, t_ShiftType.type_name FROM t_ShiftType " +
        //"GROUP BY t_ShiftType.type_guid, t_ShiftType.type_value, t_ShiftType.type_color, t_ShiftType.type_name " +
        //"HAVING (((t_ShiftType.type_color)<>\"\")) ORDER BY t_ShiftType.type_name;";

                    string query = "SELECT t_ShiftType.type_guid, t_ShiftType.type_value, t_ShiftType.type_color, t_ShiftType.type_name, t_ShiftType.Deleted "+
"FROM t_ShiftType "+
"GROUP BY t_ShiftType.type_guid, t_ShiftType.type_value, t_ShiftType.type_color, t_ShiftType.type_name, t_ShiftType.Deleted "+
"HAVING (((t_ShiftType.type_color)<>\"\") AND ((t_ShiftType.Deleted)=False)) "+
"ORDER BY t_ShiftType.type_name;";


                    DataTable dt = SqlWorker.SelectFromDBSafe(query, "t_ShiftType");

                    foreach (DataRow row in dt.Rows)
                    {
                        string smena_name = CellHelper.FindCell(row, "type_value").ToString();
                        Color smena_color = ReturnColor(row, 2);
                        if (!color_table.ContainsKey(smena_name))
                            color_table.Add(smena_name, smena_color);
                    }
                }
                catch (Exception ex)
                {
                    MDIParentMain.log.Error("Ошибка заполнения таблицы цветов ", ex);
                }
            }
        }

        private Color ReturnColor(string color_str)
        {
            Color color = Color.FromArgb(0, 0, 0);

            try
            {
                int b = Convert.ToInt32(color_str.Substring(0, 2), 16);
                int g = Convert.ToInt32(color_str.Substring(2, 2), 16);
                int r = Convert.ToInt32(color_str.Substring(4, 2), 16);
                color = Color.FromArgb(r, g, b);
            }
            catch (Exception ex)
            {

            }
            return color;
        }

        private Color ReturnColor(DataRow row,int colIndex)
        {
            object color_obj = CellHelper.FindCell(row, colIndex);
            string color_str = color_obj.ToString();
            Color color = Color.FromArgb(0, 0, 0);

            try
            {
                int b = Convert.ToInt32(color_str.Substring(0, 2), 16);
                int g = Convert.ToInt32(color_str.Substring(2, 2), 16);
                int r = Convert.ToInt32(color_str.Substring(4, 2), 16);
                color = Color.FromArgb(r, g, b);
            }
            catch (Exception ex)
            {

            }
            return color;
        }



        private void myClickFunction(object sender, EventArgs e)
        {
            bool prsFlg = true;
            try
            {
                if (e is System.Windows.RoutedEventArgs)
                {
                    ToggleButton cbutton = ((System.Windows.RoutedEventArgs)e).Source as ToggleButton;
                    if (cbutton != null)
                    {
                        if ((bool)cbutton.IsChecked)
                        {
                            List<ToggleButton> blist = smena_button_panel.Children.OfType<ToggleButton>().ToList();
                            blist.ForEach(a =>
                            {
                                if (a != cbutton) a.IsChecked = false;
                            });

                            btnValue = cbutton.Content.ToString();
                            //Convert.ToString(((DevComponents.DotNetBar.ButtonX)sender).Text);
                            _btn_guid = ((t_ButtonTemplate)cbutton.Tag).btn_guid;
                            //Convert.ToString(((DevComponents.DotNetBar.ButtonX)sender).Name);
                            _btn_time = Convert.ToInt32(bd.GetValue(_btn_guid));
                            _btn_guidSmena = bd.GetValueSmena(_btn_guid);
                            currentButtonPressed = cbutton;
                            pressBtn = true;
                        }
                        else
                        {
                            currentButtonPressed = null;
                            pressBtn = false;
                        }
                    }
                }

                return;



                DevComponents.DotNetBar.ButtonX btn = (DevComponents.DotNetBar.ButtonX)sender;
                
                int pressedBnt = Convert.ToInt32(((DevComponents.DotNetBar.ButtonX)sender).Tag);
                btnValue = Convert.ToString(((DevComponents.DotNetBar.ButtonX)sender).Text);
                _btn_guid = Convert.ToString(((DevComponents.DotNetBar.ButtonX)sender).Name);
                _btn_time = Convert.ToInt32(bd.GetValue(_btn_guid));
                _btn_guidSmena = bd.GetValueSmena(_btn_guid);

                foreach (DevComponents.DotNetBar.ButtonX item in panelEx1.Controls.OfType<DevComponents.DotNetBar.ButtonX>())
                {
                    if (Convert.ToString(item.Tag) != "")
                    { 
                        if (Convert.ToInt32(item.Tag) == pressedBnt)
                        {
                            if (item.Checked)
                            {
                                btn.Checked = false;              //выключить кнопку
                                btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                                pressBtn = false;
                                prsFlg = false;
                                currentButtonPressed = null;       //обнулить ссылку на кнопку
                            }
                            if (!btn.Checked && prsFlg)
                            {      
                                //включить кнопку
                                btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                                btn.Checked = true;
                                pressBtn = true;
                                currentButtonPressed = btn;  //сохранить ссылку на нажатую кнопку

                                //btn.ColorTable = eButtonColor.Flat;
                            }
                        }                           
                        if (Convert.ToInt32(item.Tag) != pressedBnt)
                        {
                            item.Checked = false;
                        }
                    }
                }
            }
            catch(Exception exp)
            {
                MDIParentMain.log.Error("Ошибка нажатия кнопки выбора смены: ",exp);
            }
        }

        private DataGridViewComboBoxColumn comboCell;

        DataTable source;
        CBData db;
#endregion
#endregion

        


        private void LoadGrid()
        {
            db = new CBData();
            DataTable res = new DataTable();
            source = new DataTable();
            dataGridViewMain.AutoGenerateColumns = false;

            try
            {
                MDIParentMain.log.Trace("Формируем колонки");
                dataGridViewMain.Columns.Add(new DataGridViewTextBoxColumn());
                dataGridViewMain.Columns[0].DataPropertyName = "mark_id";
                dataGridViewMain.Columns[0].Visible = false;

                dataGridViewMain.Columns.Add(new DataGridViewTextBoxColumn());
                dataGridViewMain.Columns[1].DataPropertyName = "employee_name";
                dataGridViewMain.Columns[1].HeaderText = "Сотрудник";
                dataGridViewMain.Columns[1].ReadOnly = true;
                dataGridViewMain.Columns[1].Width = 200;
                dataGridViewMain.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                dataGridViewMain.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridViewMain.Columns[1].Frozen = true;

                comboCell = new DataGridViewComboBoxColumn();
                comboCell.Name = "Обязанность";
                comboCell.DataPropertyName = "mark_office";
                comboCell.ReadOnly = false;
                dataGridViewMain.Columns.Add(comboCell);
                comboCell.Frozen = true;
                comboCell.Width = 150;
                comboCell.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                comboCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

                dataGridViewMain.Columns.Add(new DataGridViewTextBoxColumn());
                dataGridViewMain.Columns[3].DataPropertyName = "mark_total";
                dataGridViewMain.Columns[3].HeaderText = "Итого";
                dataGridViewMain.Columns[3].ReadOnly = true;
                dataGridViewMain.Columns[3].Width = 100;
                dataGridViewMain.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                dataGridViewMain.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridViewMain.Columns[3].Frozen = true;


                #region получение даты из документа
                MDIParentMain.log.Trace("Получаем дату документа");

                DataTable dt = SqlWorker.SelectFromDBSafe("SELECT t_Doc.* FROM t_Doc WHERE (((t_Doc.doc_id)=" + _doc_id.ToString() + "));", "t_Doc");
                object o = CellHelper.FindCell(dt, 0, "doc_datetime");

                _date = DateTime.Parse(o.ToString());

                dates = DateTime.DaysInMonth(Convert.ToInt32(_date.Year), Convert.ToInt32(_date.Month));
                month_label.Text = _date.ToString("MMMMMMMMM", CultureInfo.CurrentCulture) + " " + _date.Year.ToString();

                #endregion

                for (int i = 1; i < dates + 1; i++)
                {
                    dataGridViewMain.Columns.Add(new DataGridViewTextBoxColumn());
                    dataGridViewMain.Columns[i + 3].DataPropertyName = "mark_" + i;
                    dataGridViewMain.Columns[i + 3].HeaderText = i.ToString();
                    dataGridViewMain.Columns[i + 3].ReadOnly = true;
                    dataGridViewMain.Columns[i + 3].Width = 50;
                    dataGridViewMain.Columns[i + 3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dataGridViewMain.Columns[i + 3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dataGridViewMain.Columns[i + 3].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                //вставить добавление колонки итога

                itog_add();

                dataGridViewMain.DataSourceChanged += (s, e) =>
                {
                    itog_set_row();
                };

                MDIParentMain.log.Trace("Получаем инфу из базы");
                source = db.OrderMark(_doc_id, 28);
                
                dataGridViewMain.DataSource = source;

                MDIParentMain.log.Trace("Добавили главное содержание в таблицу");


                res = db.Responsibility();
                if (comboCell != null && comboCell.Items.Count <= 0)
                {
                    for (int n = 0; n < res.Rows.Count; n++)
                    {
                        if (source.Rows.Count > 0)
                        //  if(dataGridViewMain.Rows.Count>0)
                        {

                            DataGridViewRow row0 = dataGridViewMain.Rows[0];
                            DataGridViewComboBoxCell comboCel = (DataGridViewComboBoxCell)row0.Cells[2];
                            string abc = res.Rows[n].ItemArray[2].ToString();
                            comboCell.Items.AddRange(abc);
                        }
                    }
                }
                MDIParentMain.log.Trace("Добавили обязанности в строки");
            }
            catch (Exception exp)
            {
                MDIParentMain.log.Error("form mark load error: ",exp);
                MessageBox.Show(exp.Message);
            }
            finally
            {
            }
        }

        private void InData()
        {
            CBData _rf = new CBData();
            try
            {
                // связать с данным
                dataGridViewMain.DataSource = _rf.OrderMark(_doc_id, 28);
            }
            catch (Exception _exp)
            {
                MessageBox.Show(_exp.Message);
            }
        }

        static DateTime LastDayOfYear(DateTime d)
        {
            DateTime n = new DateTime(d.Year + 1, 1, 1);
            return n.AddDays(-1);
        }

        private void dataGridViewMain_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CBData _data = new CBData();
            string lineOr = MyTrim(_data.returnColor(_btn_guid));
            string[] colorArr = lineOr.Split(',', ' ');
            DataTable idColumn;
            int totalWorkDays = 0;
            string totalstr = "";



            if (dataGridViewMain.CurrentCell.ColumnIndex != 0 && dataGridViewMain.CurrentCell.ColumnIndex != 1 
                && dataGridViewMain.CurrentCell.ColumnIndex != 2 && dataGridViewMain.CurrentCell.ColumnIndex != 3)
            {
                if (dataGridViewMain.CurrentRow.Cells[2].Value != DBNull.Value && dataGridViewMain.CurrentRow.Cells[2].Value.ToString() != "" )
                {
                    jobStart = Convert.ToDateTime(bd.returnJobTeremokStart(e.ColumnIndex));
                    jobEnd = Convert.ToDateTime(bd.returnJobTeremokEnd(e.ColumnIndex));
                    if (pressBtn == true)
                    {
                        if (btnValue == "Оч" || btnValue == "") btnValue = "";
                        dataGridViewMain.CurrentCell.Value = btnValue;
                        if (colorArr.Length == 3)
                        {
                            dataGridViewMain.CurrentCell.Style.BackColor = Color.FromArgb(Convert.ToInt32(colorArr[0]), Convert.ToInt32(colorArr[1]), Convert.ToInt32(colorArr[2]));
                        }
                        dataGridViewMain.CurrentCell.Style.ForeColor = Color.Black;
                        dataGridViewMain.CurrentCell.Style.Font = new Font("Tahoma", 8, FontStyle.Bold);
                        int starJobEmp = jobStart.Hour;
                        int EndJobEmp = _btn_time + starJobEmp;
                        if (EndJobEmp > jobEnd.Hour)
                        {
                            using (new CenterWinDialog(this))
                            {
                                MessageBox.Show("Oшибка ввода данных!\nCмена сотрудника не может превышать рабочий график работы ресторана.");
                            }
                        }
                     
                        _data.OrderMarkUpdate(_btn_guid, btnValue, e.ColumnIndex, Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value.ToString()),
                            jobStart, _btn_time, _btn_guidSmena);
                        idColumn = bd.returnDTTotalDays(dataGridViewMain.CurrentRow.Cells[0].Value.ToString());
                        foreach (DataRow item1 in idColumn.Rows)
                        {
                            for (int a = 0; a < idColumn.Columns.Count; a++)
                            {
                                if (item1.ItemArray[a].ToString() != "")
                                {
                                    int val = Convert.ToInt32(item1.ItemArray[a]);
                                    if (val != 0)
                                    {
                                        totalWorkDays++;
                                        Total = Total + val;
                                        
                                        //добавить логику подсчета итога

                                    }
                                    totalstr = totalWorkDays + " см. (" + Total + " ч.)";
                                }
                            }
                            bd.TotalUpdate(dataGridViewMain.CurrentRow.Cells[0].Value.ToString(), totalstr);

                            //обновить итог
                            itog_update_db(true);

                            dataGridViewMain.CurrentRow.Cells[3].Value = totalstr;
                            Total = 0;
                            totalWorkDays = 0;
                        }
                    }
                }
                else
                {
                    using (new CenterWinDialog(this))
                    {
                        if (MessageBox.Show("Не проставлена обязанность сотрудника. \nУстановите обязанность, после чего можно проставить смены.", "Внимание", MessageBoxButtons.OK) != DialogResult.OK)
                        {
                            return;
                        }
                    }
                }
            }
        }

        private string MyTrim(string str)
        {
            string res = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].ToString() != " ")
                {
                    res = res + str[i];
                }
            }
            return res.Trim();
        }

        private DayWork[] myDayArr1(int num)
        {
            CBData db = new CBData();
            DayWork[] myDayArr = new DayWork[dates];
            DataTable all = db.allMark(_doc_id);
            DataTable firstime = db.marktime(_doc_id);
            DataTable lasttime = db.marklasttime(_doc_id);
            DataTable Valuetime = db.valueTime(_doc_id);
            DataTable guidSmena = db.guidSmena(_doc_id);
            string DateTime = db.GetDateCreate(_doc_id);
            DateTime create = Convert.ToDateTime(DateTime);

            for (int i = 0; i < myDayArr.Length; i++)
            {
                {
                    DayWork Work = new DayWork
                    {
                        Number = 1 + i,
                        SmenaType = guidSmena.Rows[num].ItemArray[i].ToString(),
                        Value = Valuetime.Rows[num].ItemArray[i].ToString(),
                        FirstTime = Convert.ToDateTime(create.Year + "-" + AttachZeroToDate(create.Month) + "-" + AttachZeroToDate(1 + i) + " " + ToDate(firstime.Rows[num].ItemArray[i].ToString() + ":00")),
                        LastTime = Convert.ToDateTime(create.Year + "-" + AttachZeroToDate(create.Month) + "-" + AttachZeroToDate(1 + i) + " " + ToDate(lasttime.Rows[num].ItemArray[i].ToString() + ":00"))
                    };
                    myDayArr[i] = Work;
                }
            }
            return myDayArr;
        }

        public DayWork[] myDayArr2(int num, int doc_id)
        {
            CBData db = new CBData();
            DayWork[] myDayArr = new DayWork[31];
            DataTable all = db.allMark(doc_id);
            DataTable firstime = db.marktime(doc_id);
            DataTable lasttime = db.marklasttime(doc_id);
            DataTable Valuetime = db.valueTime(doc_id);
            DataTable guidSmena = db.guidSmena(doc_id);
            string DateTime = db.GetDateCreate(doc_id);
            DateTime create = Convert.ToDateTime(DateTime);

            for (int i = 0; i < myDayArr.Length; i++)
            {
                {
                    DayWork Work = new DayWork
                    {
                        Number = 1 + i,
                        SmenaType = guidSmena.Rows[num].ItemArray[i].ToString(),
                        Value = Valuetime.Rows[num].ItemArray[i].ToString(),
                        FirstTime = Convert.ToDateTime(create.Year + "-" + AttachZeroToDate(create.Month) + "-" + AttachZeroToDate(1 + i) + " " + ToDate(firstime.Rows[num].ItemArray[i].ToString() + ":00")),
                        LastTime = Convert.ToDateTime(create.Year + "-" + AttachZeroToDate(create.Month) + "-" + AttachZeroToDate(1 + i) + " " + ToDate(lasttime.Rows[num].ItemArray[i].ToString() + ":00"))
                    };
                    myDayArr[i] = Work;
                }
            }
            return myDayArr;
        }

        private string AttachZeroToDate(int c)
        {
            if (c < 10)
                return "0" + c.ToString();
            else
                return c.ToString();
        }

        private string ToDate(string c)
        {
            if (c == ":00")
                return "0:00" + c.ToString();
            else
                return c.ToString();
        }

        private void dataGridViewMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Delete)
            {
                deleteRow();
                InData();
            }
        }

        private void deleteRow()
        {
            CBData _data = new CBData();

            if (dataGridViewMain.CurrentRow.Cells[1].Value.ToString() == Itog_class.itog_name)
            {
                return;
            }

            try
            {
                int _markID = Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value);
                string _name = Convert.ToString(dataGridViewMain.CurrentRow.Cells[1].Value);

                if (readDoc == false)
                {
                    using (new CenterWinDialog(this))
                    {
                        if (MessageBox.Show("Вы уверены, что хотите удалить сотрудника: " + _name.ToString(), "Вопрос", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            return;
                        }
                        else
                        {
                            _data.MarkDelete(_markID);
                           // InData();
                        }
                    }
                }
                else
                {
                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show("Редактирование полей в режиме просмотра ''ЗАПРЕЩЕНО''");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private DataTable returnCheckID(DataTable dt, string ch_mnome_id)
        {
            DataTable qwe = null;

            var q = (from row in dt.AsEnumerable()
                     where (string)row["bnt_value"] == ch_mnome_id.ToString()
                     select row).ToArray();

            if (q.Length != 0)
            {
                qwe = q.CopyToDataTable();
            }
            return qwe;
        }

        private void paintGrid()
        {
            CBData _db = new CBData();
            DataTable _dt;
            DataTable _returnDT;

            try
            {
                _dt = _db.returnColorMarkNew();
                foreach (DataGridViewColumn _column in dataGridViewMain.Columns)
                {
                    foreach (DataGridViewRow _row in dataGridViewMain.Rows)
                    {
                        if (_column.Index != 0 && _column.Index != 1 && _column.Index != 2 && _column.Index != 3)
                        {
                            _returnDT = returnCheckID(_dt, dataGridViewMain[_column.Index, _row.Index].Value.ToString());
                            dataGridViewMain[_column.Index, _row.Index].Style.BackColor = Color.White;
                            if (_returnDT != null)
                            {
                                foreach (DataRow _check in _returnDT.Rows)
                                {
                                    string lineOr = _check[4].ToString();
                                    string[] colorArr = lineOr.Split(',', ' ');

                                    if (colorArr.Length == 5)
                                    {
                                        dataGridViewMain[_column.Index, _row.Index].Style.BackColor = Color.FromArgb(Convert.ToInt32(colorArr[0]), Convert.ToInt32(colorArr[2]), Convert.ToInt32(colorArr[4]));
                                    }
                                    else
                                    {
                                        dataGridViewMain[_column.Index, _row.Index].Style.BackColor = Color.White;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void jobOther()
        {
            CBData _db = new CBData();
            DataTable _rowCount;
            int row;
            DataTable _rdr;
            try
            {

                _rdr = _db.returnRDR();
                _rowCount = _db.returnRowCount();
                foreach (DataRow item in _rdr.Rows)
                {
                    row = _db.GetIdMark(item[1].ToString());
                    for (int i = 0; i < _rowCount.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(dataGridViewMain.Rows[i].Cells[0].Value) == row)
                        {
                            string value = dataGridViewMain.Rows[i].Cells[Convert.ToInt32(item[3]) + 3].Value.ToString();
                            if (value != "")
                            {
                                dataGridViewMain.Rows[i].Cells[Convert.ToInt32(item[3]) + 3].Value = value + " / РДР";
                            }
                            else
                            {
                                dataGridViewMain.Rows[i].Cells[Convert.ToInt32(item[3]) + 3].Value = value + "РДР";
                            }
                            dataGridViewMain.Rows[i].Cells[Convert.ToInt32(item[3]) + 3].ToolTipText = " Ресторан: Европейский \n Начало: 09:00 \n Окончание: 21:00";
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void dataGridViewMain_Layout(object sender, LayoutEventArgs e)
        {
            CBData _db = new CBData();
            DataTable _dt;
            DataTable _returnDT;

            try
            {
                _dt = _db.returnColorMarkNew();
                foreach (DataGridViewColumn _column in dataGridViewMain.Columns)
                {
                    foreach (DataGridViewRow _row in dataGridViewMain.Rows)
                    {
                        if (_column.Index != 0 && _column.Index != 1 && _column.Index != 2 && _column.Index != 3)
                        {
                            _returnDT = returnCheckID(_dt, dataGridViewMain[_column.Index, _row.Index].Value.ToString());
                            dataGridViewMain[_column.Index, _row.Index].Style.BackColor = Color.White;
                            if (_returnDT != null)
                            {
                                foreach (DataRow _check in _returnDT.Rows)
                                {
                                    string lineOr = _check[4].ToString();
                                    string[] colorArr = lineOr.Split(',', ' ');

                                    if (colorArr.Length == 5)
                                    {
                                        dataGridViewMain[_column.Index, _row.Index].Style.BackColor = Color.FromArgb(Convert.ToInt32(colorArr[0]), Convert.ToInt32(colorArr[2]), Convert.ToInt32(colorArr[4]));
                                    }
                                    else
                                    {
                                        dataGridViewMain[_column.Index, _row.Index].Style.BackColor = Color.White;
                                    }
                                }
                            }
                        }
                    }
                }
                count_Layout++;
                if (count_Layout == 3)
                {
                    jobOther();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }



        private void buttonX1_Click_1(object sender, EventArgs e)
        {
            try
            {


                if (add_user_form != null)
                {
                    if (add_user_form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (add_user_form.Result != null)
                        {
                            t_Employee emp = (t_Employee)((ModelItemClass)add_user_form.Result).t_table;
                            string guid_1C = emp.employee_1C;
                            db.OrderAddEmp(guid_1C, _doc_id, dataGridViewMain.Rows.Count);
                        }
                    }




                    InData();
                    paintGrid();

                    if (comboCell != null && comboCell.Items.Count <= 0)
                    {
                        CBData db = new CBData();
                        DataTable res = new DataTable();

                        res = db.Responsibility();

                        {
                            for (int n = 0; n < res.Rows.Count; n++)
                            {
                                string abc = res.Rows[n].ItemArray[2].ToString();
                                comboCell.Items.AddRange(abc);

                            }
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                MDIParentMain.log.Error("Ошибка в формировании смен buttonX1_Click_1", exp);
            }        
        }

        private void buttonSave_Click_1(object sender, EventArgs e)
        {
            CBData db = new CBData();
            systemService.Credentials = new NetworkCredential("iteremok", "Ge6!))~nC5@0");
            WorkEmployee[] arrWe = new WorkEmployee[db.CountMark(_doc_id)];
            string docGuid = db.returnDocGuid(_doc_id);
            DataTable all = db.allMark(_doc_id);

            for (int a = 0; a < arrWe.Length; a++)
            {
                WorkEmployee we = new WorkEmployee { Name = all.Rows[a].ItemArray[0].ToString(), Responsibility = all.Rows[a].ItemArray[1].ToString(), ArrayDayWork = myDayArr1(a) };
                arrWe[a] = we;
            }

            PlanDocument pd = new PlanDocument { Date = DateTime.Today, ID = docGuid.ToString(), ArrayWorkEmployee = arrWe };
            string result = systemService.PutDocument(Convert.ToInt32(CParam.TeremokId), pd);
            if (result == "1")
            {
                MessageBox.Show("Обмен завершен");

                //db.DocUpdateStateMark(_doc_id,82,"Отправлено ");
                db.DocUpdateDocStateNew(_doc_id, 3, "Отправлено ");
            }//
            else
            {
                MessageBox.Show("Сбой обмена");
            }            
        }

        private void dataGridViewMain_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CBData _data = new CBData();

            if (dataGridViewMain.CurrentCell.ColumnIndex > 3 || dataGridViewMain.CurrentCell.ColumnIndex == 2)
            {
                string giud = _data.resGUID(dataGridViewMain.CurrentRow.Cells[2].Value.ToString()); 
               _data.OrderMarkUpdateEmp(dataGridViewMain.CurrentRow.Cells[2].Value.ToString(), giud, Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value.ToString()));
            }
        }


        private void dataGridViewMain_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow _row in dataGridViewMain.Rows){
                foreach(DataGridViewTextBoxCell cell in _row.Cells.OfType<DataGridViewTextBoxCell>())
                {
                    if(cell.Value.ToString()!=""){
                        if (color_table.ContainsKey(cell.Value.ToString()))
                        {
                            Color color = color_table[cell.Value.ToString()];
                            if (color != null) cell.Style.BackColor = color;
                        }
                    }
                }
            }

            var height = 40;
            foreach (DataGridViewRow dr in dataGridViewMain.Rows)
            {
                height += dr.Height;
            }

            dataGridViewMain.Height = height;
        }



        //DataGridViewCellPaintingEventHandler(dataGridViewMain_CellPainting);
        private void dataGridViewMain_CellPainting(object sender,DataGridViewCellPaintingEventArgs e)
        {
            if (sender is DataGridView)
            {
                if (e.ColumnIndex == -1 || e.RowIndex == -1) return;
                object cell_obj = CellHelper.FindCell(dataGridViewMain, e.RowIndex, e.ColumnIndex);

                    DataGridViewTextBoxCell cell = cell_obj as DataGridViewTextBoxCell;
                    if (null != cell && cell.Value.ToString() != "")
                    {
                        string smena_type = returnSmenaType(cell.Value.ToString());
                        if (color_table.ContainsKey(smena_type))
                        {
                            Color color = color_table[smena_type];
                            if (color != null) cell.Style.BackColor = color;
                        }
                    }
            }
        }


        Regex reg = new Regex(@"[А-Я,а-я]*");
        //Regex reg = new Regex(@"(.*?)\d*");
        private string returnSmenaType(string cellValue)
        {
            string smena_type = "";

            if (reg.IsMatch(cellValue))
            {
                smena_type = reg.Match(cellValue).Value;
            }

            return smena_type;
        }


        private bool IsCellOrRowHeader(int x, int y)
        {
            DataGridViewHitTestType dgt = dataGridViewMain.HitTest(x, y).Type;
            return (dgt == DataGridViewHitTestType.Cell ||
                            dgt == DataGridViewHitTestType.RowHeader);
        }

        private void dataGridViewMain_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    if (!IsCellOrRowHeader(e.X, e.Y) && rowIndexSrc >= 0)
                    {
                        DragDropEffects dropEffect = dataGridViewMain.DoDragDrop(
                            dataGridViewMain.Rows[rowIndexSrc], DragDropEffects.Move);
                        return;
                    }

                    if (dragBoxSrc != Rectangle.Empty &&
                            !dragBoxSrc.Contains(e.X, e.Y))
                    {
                        DragDropEffects dropEffect = dataGridViewMain.DoDragDrop(
                            dataGridViewMain.Rows[rowIndexSrc], DragDropEffects.Move);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void dataGridViewMain_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                rowIndexSrc = dataGridViewMain.HitTest(e.X, e.Y).RowIndex;
                if (rowIndexSrc != -1)
                {
                    Size dragSize = SystemInformation.DragSize;
                    dragBoxSrc = new Rectangle(new Point(e.X, e.Y), dragSize);
                }
                else
                    dragBoxSrc = Rectangle.Empty;
            }
            catch (Exception)
            {

            }
        }

        private void dataGridViewMain_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Point clientPoint = dataGridViewMain.PointToClient(new Point(e.X, e.Y));
                rowIndexTar = dataGridViewMain.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
                if (e.Effect == DragDropEffects.Move)
                {
                    DataGridViewRow rowToMove = e.Data.GetData(
                        typeof(DataGridViewRow)) as DataGridViewRow;
                    MoveRow(rowIndexSrc, rowIndexTar);
                }
                paintGrid();
            }
            catch (Exception)
            {

            }
        }

        private void SwapCell(int c, int srcRow, int tarRow, out object tmp0, out object tmp1)
        {
            DataGridViewCell tarCell = dataGridViewMain.Rows[tarRow].Cells[c];
            DataGridViewCell srcCell = dataGridViewMain.Rows[srcRow].Cells[c];
            tmp0 = tarCell.Value;
            tmp1 = srcCell.Value;
            tarCell.Value = tmp1;
        }

        private void MoveRow(int srcRow, int tarRow)
        {
            try
            {
                int cellCount = dataGridViewMain.Rows[srcRow].Cells.Count;
                DataGridViewCell tarCell = dataGridViewMain.Rows[tarRow].Cells[0];
                DataGridViewCell srcCell = dataGridViewMain.Rows[srcRow].Cells[0];
                bd.dUpdate(_doc_id, srcRow, Convert.ToInt32(tarCell.Value));
                bd.dUpdate(_doc_id, tarRow, Convert.ToInt32(srcCell.Value));
                for (int c = 0; c < cellCount; c++)
                {
                    ShiftRows(srcRow, tarRow, c);
                }
            }
            catch (Exception)
            {

            }
        }

        private void ShiftRows(int srcRow, int tarRow, int c)
        {
            try
            {
                object tmp0, tmp1;
                SwapCell(c, srcRow, tarRow, out tmp0, out tmp1);
                int delta = tarRow < srcRow ? 1 : -1;
                for (int r = tarRow + delta; r != srcRow + delta; r += delta)
                {
                    tmp1 = dataGridViewMain.Rows[r].Cells[c].Value;
                    dataGridViewMain.Rows[r].Cells[c].Value = tmp0;
                    tmp0 = tmp1;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
            }
        }

        private void dataGridViewMain_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Point p = dataGridViewMain.PointToClient(new Point(e.X, e.Y));
                DataGridViewHitTestType dgt = dataGridViewMain.HitTest(p.X, p.Y).Type;
                if (IsCellOrRowHeader(p.X, p.Y))
                    e.Effect = DragDropEffects.Move;
                else e.Effect = DragDropEffects.None;
            }
            catch (Exception)
            {

            }
        }



        public static string GetTabelHash(PlanDocument pd)
        {
            try
            {
                string hash = "";
                hash += pd.ID;



                foreach (WorkEmployee wrk in pd.ArrayWorkEmployee)
                {
                    hash += wrk.Name.ReturnOrEmpty() + wrk.Responsibility.ReturnOrEmpty();
                    int days_count = DateTime.DaysInMonth(Convert.ToInt32(pd.Date.Year), Convert.ToInt32(pd.Date.Month));

                    //foreach (DayWork dw in wrk.ArrayDayWork)
                    for (int i = 0; i < days_count; i++)
                    {
                        DayWork dw = wrk.ArrayDayWork[i];
                        hash += dw.Number.ToString();
                        hash += dw.SmenaType.ReturnOrEmpty();
                        hash += dw.Value.ReturnOrEmpty();
                    }
                }
                return Hashing.GetMd5Hash(hash);
            }catch(Exception ex)
            {
                MDIParentMain.Log("GetTabelHash error " + ex.Message);
                return "";
            }
        }



        private void FormMark2_FormClosed(object sender, FormClosingEventArgs e)
        {
#if(!DEB)
                if (IsDocumentTabelBlocked != true)
#else
            /*if (IsDocumentTabelBlocked != true)*/if (true)
#endif
            {
                itog_remove();

                List<string[]> sortList1 = new List<string[]>();

                List<string> family_list = new List<string>();
                foreach (DataGridViewRow row in dataGridViewMain.Rows)
                {
                    string name = ((DataGridViewTextBoxCell)CellHelper.FindCell(dataGridViewMain, row.Index, 1)).Value.ToString();
                    family_list.Add(name);
                }

                DataTable source = db.OrderMark(_doc_id, 28);

                family_list.ForEach(a =>
                {
                    foreach (DataRow row in source.Rows)
                    {
                        string name = CellHelper.FindCell(row, "employee_name").ToString();
                        if (name == a)
                        {
                            string guid = CellHelper.FindCell(row, "mark_name").ToString();
                            sortList1.Add(new string[] { guid, name });
                            break;
                        }
                    }
                });

                TabelBackgroundWorker.CreateWorker(_doc_id, dates, _date);
                TabelBackgroundWorker.CurrentWorker.sortList = sortList1;
                TabelBackgroundWorker.CurrentWorker.Start(); //отправка табеля в потоке

                StaticConstants.IsTabelOpened = false;
            }
        }

        #region comments
        //private bool sended = false;
        //private void FormMark2_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    if(!sended){
        //        e.Cancel = true;
        //        this.Visible = false;

        //        //заблокировать кнопки табелей

        //        tabWorker.Start();
        //    }
        //}

        public void send_document_to_webservice()
        {
            try
            {
                //if (!StaticConstants.IsDocumentTabelBlocked)
#if(!DEB)
                if (IsDocumentTabelBlocked != true)
#else
                if (true)
#endif
                {
                    CBData _data = new CBData();
                    systemService.Credentials = new NetworkCredential("iteremok", "Ge6!))~nC5@0");
                    BlokDoc _bk = new BlokDoc { Date = StaticConstants.Tabel_Opened_Date, ID = _data.returnDocGuid(_doc_id) };
                    BlokResult _bkr = systemService.IsBlocked(Convert.ToInt32(CParam.TeremokId), _bk, false);

                    WorkEmployee[] arrWe = new WorkEmployee[_data.CountMark(_doc_id)];
                    string docGuid = _data.returnDocGuid(_doc_id);
                    DataTable all = _data.allMark(_doc_id);

                    for (int a = 0; a < arrWe.Length; a++)
                    {
                        string _name = all.Rows[a].ItemArray[0].ToString();
                        string _resp = all.Rows[a].ItemArray[1].ToString();
                        DayWork[] _allDwrk = myDayArr1(a);

                        WorkEmployee we = new WorkEmployee { Name = _name, Responsibility = _resp, ArrayDayWork = _allDwrk };
                        arrWe[a] = we;
                    }

                    PlanDocument pd = new PlanDocument { Date = _date, ID = docGuid.ToString(), ArrayWorkEmployee = arrWe };
                    string result = systemService.PutDocument(Convert.ToInt32(CParam.TeremokId), pd);
                    if (result == "1")
                    {
                        using (new CenterWinDialog(this))
                        {
                            MessageBox.Show("Обмен завершен");
                        }
                        //_data.DocUpdateStateMark(_doc_id, 82, "Отправлено ");
                        _data.DocUpdateDocStateNew(_doc_id, 3, "Отправлено ");
                    }
                    else
                    {
                        using (new CenterWinDialog(this))
                        {
                            MessageBox.Show("Сбой обмена");
                        }
                    }
                }
                else
                {
                    MDIParentMain.log.Debug("Документ табеля заблокирован!!");
                }

                //убираем флаг блокировки
                StaticConstants.IsDocumentTabelBlocked = false;
            }
            catch (Exception exp)
            {
                MDIParentMain.log.Error("Ошибка выгрузки табеля на вебсервис: ", exp);
                this.Close();
            }
        }

//        public void work_completed()
//        {
//            sended = true;

//            //разблокировать кнопки табелей

//            this.Close();
        //        }
        #endregion

        public string returnCheckedButton()
        {
            

            if(currentButtonPressed!=null)
                return ((ToggleButton)currentButtonPressed).Content.ToString();
            return "-1";

            //DevComponents.DotNetBar.ButtonX
            //foreach (var button in myBtn)
            //{
            //    if (button.Checked) return button.Text;
            //}
            //return "-1";
        }

        private void dataGridViewMain_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridViewMain_CellClick
            DataTable idColumn;
            int totalWorkDays = 0;
            string totalstr = "";

            try
            {
                #region подкрасить ячейку
                Color color = Color.FromArgb(255,255,255);
                if (currentButtonPressed != null)
                {
                    //ReturnColor(row, 4);
                    color = ReturnColor(((t_ButtonTemplate)((ToggleButton)currentButtonPressed).Tag).btn_color);
                    if (color == Color.FromArgb(0, 0, 0))
                    {
                        color = Color.FromArgb(255, 255, 255);
                    }
                    DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)CellHelper.FindCell((DataGridView)sender, e.RowIndex, e.ColumnIndex);
                    cell.Style.BackColor = color;
                }
                
                #endregion

                #region условие двойного клика

                string cell_str = ((DataGridViewTextBoxCell)CellHelper.FindCell(dataGridViewMain, e.RowIndex, e.ColumnIndex)).Value.ToString();
                //string sht = bd.returnShiftTypeGuid(e.ColumnIndex - 3, Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value));
                string button_str = returnCheckedButton();

                if ((cell_str == "" && button_str != "-1") || (cell_str != "" && button_str != "-1") || (cell_str != "" && button_str == "Оч"))
                {
                    dataGridViewMain_CellClick(this, e);
                    return;
                }

                #endregion

                #region открытие доп окна для ячейки если пустое
                    if (button_str == "-1")
                    {
                        dataGridViewMain_CellClick(this, e);
                    }
                #endregion

                if (dataGridViewMain.CurrentCell.ColumnIndex != 0 && dataGridViewMain.CurrentCell.ColumnIndex != 1 && dataGridViewMain.CurrentCell.ColumnIndex != 2 && dataGridViewMain.CurrentCell.ColumnIndex != 3)
                {
                    CellMark _cm = new CellMark();
                    _cm.starJob = jobStart;
                    _cm.endJob = jobEnd;
                    _cm.typeValue = bd.returnShiftTypeGuid(e.ColumnIndex - 3, Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value));
                    _cm.timeValueHour = bd.timeValue(_doc_id, e.ColumnIndex - 3, Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value));
                    if (_cm.timeValueHour == 0) return;
                    _cm.rowID = Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value);
                    _cm.columnID = e.ColumnIndex - 3;
                    _cm._dt = bd.OrderMarkDetails(_doc_id, e.ColumnIndex - 3, Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value));
                    _cm.ShowDialog();

                    idColumn = bd.returnDTTotalDays(dataGridViewMain.CurrentRow.Cells[0].Value.ToString());
                    foreach (DataRow item1 in idColumn.Rows)
                    {
                        for (int a = 0; a < idColumn.Columns.Count; a++)
                        {
                            if (item1.ItemArray[a].ToString() != "")
                            {
                                int val = Convert.ToInt32(item1.ItemArray[a]);
                                if (val != 0)
                                {
                                    totalWorkDays++;
                                    Total = Total + val;

                                    //добавить логику подсчета итога

                                }
                                totalstr = totalWorkDays + " см. (" + Total + " ч.)";
                            }
                        }
                        bd.TotalUpdate(dataGridViewMain.CurrentRow.Cells[0].Value.ToString(), totalstr);
                        dataGridViewMain.CurrentRow.Cells[3].Value = totalstr;
                        Total = 0;
                        totalWorkDays = 0;
                    }
                    //сохранить итог
                    itog_update_db(false);
                    InData();
                    
                }
                if (dataGridViewMain.CurrentCell.ColumnIndex == 2)
                {

                }
            }
            catch (Exception)
            {
            }
        }
    }
}

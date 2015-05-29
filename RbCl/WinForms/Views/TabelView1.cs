using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.WPF.ViewModels;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes.DocumentClasses;
using RBClient.WinForms.ViewModels;
using System.Collections;
using RBClient.Classes;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms.Integration;
using System.Windows.Controls.Primitives;
using RBClient.WPF.UserControls;
using RBClient.Classes.CustomWindows;
using RBClient.WinForms.CustomElements;
using System.Globalization;

namespace RBClient.WinForms.Views
{
    public partial class TabelView1 : BaseForm
    {
        OrderClass order;

        WFAddElementFormI users_window;
        EditSmenaWindow1 smena_window;

        Hashtable SmenaFilters;
        Hashtable SmenaFiltersGuids;

        public TabelView1(OrderClass order)
        {
            InitializeComponent();
            this.order = order;

            fill_sprav();

            Load += form_loaded;

            ЗаполнитьФильтрыСмен();

            UpdateItemsSource();

            FormClosing+=TabelView1_FormClosing;

        }

        private void ЗаполнитьФильтрыСмен()
        {
            SmenaFilters = new Hashtable();
            SmenaFiltersGuids = new Hashtable();
            var b=order.DetailsDictionary[typeof(t_Shifts_Allowed)];
            foreach(var resp in order.HDetailsDictionary[typeof(t_Responsibility)].Keys)
            {
                var c=b.Where(a=>((t_Shifts_Allowed)a.t_table).res_guid.Equals(resp));
                var oo = c.ToList();
                if(c.NotNullOrEmpty()){
                    SmenaFilters.Add(resp, oo.Select(a => (t_Shifts_Allowed)a.t_table));
                    SmenaFiltersGuids.Add(resp, oo.Select(a => ((t_Shifts_Allowed)a.t_table).shift_guid));
                }
            }
        }

        private void dataGridView1_KeyDown(object s, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Insert)
            {
                users_window.ShowDialoG();
            }
            if (e.KeyCode == Keys.Delete)
            {
                button2_Click(s, e);
            }
        }
        public void UpdateItemsSource()
        {
            MarkList = order.ViewsCollection.OfType<MarkViewModelItem>().ToList();
            MarkListHash = new Hashtable();
            MarkList.ForEach(a =>
            {
                MarkListHash.Add(a.Id, a);
            });
            ItemsSource = MarkList;
        }

        public void UpdateSource()
        {
            MarkList = SortList(order.ViewsCollection.OfType<MarkViewModelItem>().ToList());
            AddItog();
            dataGridView1.DataSource = MarkList;
            dataGridView1.Rows[MarkList.Count - 1].ReadOnly = true;
        }

        public void AddToSource(MarkViewModelItem mi)
        {
            MarkList = order.ViewsCollection.OfType<MarkViewModelItem>().ToList();
            MarkListHash.Add(mi.Id, mi);
            AddItog();
            dataGridView1.DataSource = MarkList;
            dataGridView1.Rows[MarkList.Count - 1].ReadOnly = true;
        }

        public void DelFromSource(MarkViewModelItem mi)
        {
            MarkList = order.ViewsCollection.OfType<MarkViewModelItem>().ToList();
            MarkListHash.Remove(mi.Id);
            AddItog();
             dataGridView1.DataSource = MarkList;
             dataGridView1.Rows[MarkList.Count - 1].ReadOnly = true;
        }

        Hashtable MarkListHash;

        MarkViewModelItem Itog_Row;

        List<MarkViewModelItem> _MarkList;
        List<MarkViewModelItem> MarkList
        {
            get { return _MarkList;}
            set 
            { 
                _MarkList=value;
                _MarkList.Sort((a, b) => a.RowId.CompareTo(b.RowId));
            }
        }

        private void AddItog()
        {
            if (Itog_Row == null)
            {
                Itog_Row = new MarkViewModelItem() { Id = 0, Сотрудник = "Итого:", RowId = 999 };
            }
            RecountItog();
            _MarkList.Add(Itog_Row);
        }

        DataGridViewCellStyle ItogStyle=null;
        
        private void MakeItogStyle()
        {
            int r=dataGridView1.Rows.Count-1;
            for (int i = 0; i < dataGridView1.Rows[r].Cells.Count;i++)
            {
                if (!(dataGridView1[i, r] is CustomDataGridView.DataGridViewCustomCell)){                    
                    dataGridView1[i, r] = new CustomDataGridView.DataGridViewCustomCell();
                    if (i == 1)
                    {
                        dataGridView1[i, r].Style.Alignment = DataGridViewContentAlignment.BottomLeft;
                    }
                }
            }
        }


        int GetDataGridViewHeight(DataGridView dataGridView)
        {
            if (dataGridView == null) return 0;
            var sum = (dataGridView.ColumnHeadersVisible ? dataGridView.ColumnHeadersHeight : 0) +
                      dataGridView.Rows.OfType<DataGridViewRow>().Where(r => r.Visible).Sum(r => r.Height);
            sum += 25;
            return sum;
        }
        private void SetGridViewSize()
        {
            if (dataGridView1 == null) return;
            var height = this.GetDataGridViewHeight(this.dataGridView1);
            this.dataGridView1.Height = height;
            dataGridView1.Width = this.Width-10;
        }

        protected override void OnResize(EventArgs e)
        {
            SetGridViewSize();
        }

        private void RecountItog()
        {
            if (Itog_Row != null)
            {
                for (int i = 1; i <= 31; i++)
                {
                    RecountItog(i);
                }
            }
        }
        private void RecountItog(int column_index)
        {
            if (Itog_Row != null)
            {
                int sum = 0;
                var c = from k in MarkList
                        where k != Itog_Row
                        select StaticHelperClass.ReturnClassItemValue(k.modelitem.t_table, "mark_work_" + column_index);
                int val = 0;
                foreach (var cc in c)
                {
                    if(cc!=null && (string)cc!="" && (string)cc!="0")
                        sum++;
                }
                StaticHelperClass.SetClassItemValue(Itog_Row, "_" + column_index, sum.ToString());
            }
        }

        private void DisableItog()
        {
            if (Itog_Row != null)
            {
                dataGridView1[2, MarkList.Count - 1] = new DataGridViewTextBoxCell();
                ((DataGridViewCell)dataGridView1[2, MarkList.Count - 1]).Style.BackColor = Color.Red;
                dataGridView1[1, 1].Style.BackColor = Color.Blue;
                dataGridView1[1, MarkList.Count - 1].Style.BackColor = Color.Blue;
                dataGridView1[2, MarkList.Count - 1].Style.BackColor = Color.Red;
                dataGridView1[3, MarkList.Count - 1].Style.BackColor = Color.Bisque;
                dataGridView1.Rows[MarkList.Count - 1].ReadOnly = true;
                
            }
        }

        private void fill_sprav()
        {
            IEnumerable<t_ButtonTemplate> shifts = order.HDetailsDictionary[typeof(t_ButtonTemplate)].Values.OfType<t_ButtonTemplate>();
            foreach (t_ButtonTemplate tb in
               shifts)
            {
                if (!Colors1.ContainsKey(tb.bnt_value))
                {
                    if (tb.btn_color != null || tb.btn_color != "")
                        Colors1.Add(tb.bnt_value, StaticHelperClass.ReturnColorWF(tb.btn_color));
                }
            }

            List<t_Shifts_Allowed> shifts_allowed = order.DetailsDictionary[typeof(t_Shifts_Allowed)].OfType<t_Shifts_Allowed>().ToList();

            dates = DateTime.DaysInMonth(Convert.ToInt32(order.CurrentDocument.doc_datetime.Year), 
                Convert.ToInt32(order.CurrentDocument.doc_datetime.Month));
        }

        MyPanel smena_button_panel;
        private void form_loaded(object sender,EventArgs e)
        {

            #region грузим кнопки
            MDIParentMain.log.Trace("Создаем кнопки");

            ElementHost host = elementHost1;
            //host.Size = new System.Drawing.Size(740, 300);
            //host.Location = new Point(225, 3);
            host.BackColor = Color.Transparent;

             smena_button_panel = new MyPanel();
                smena_button_panel.margin = 5;
                host.Child = smena_button_panel;
                smena_button_panel.Width = host.Width;


            List<t_ButtonTemplate> buttons = order.HDetailsDictionary[typeof(t_ButtonTemplate)].Values.OfType<t_ButtonTemplate>().ToList();
            buttons.Sort((a, b) => a.btn_Order.CompareTo(b.btn_Order));
            foreach (t_ButtonTemplate a in buttons)
            {
                ToggleButton tb = new ToggleButton();
                tb.Focusable = false;
                tb.Content = a.bnt_value;
                tb.MinWidth = 40;
                tb.Height = 20;
                tb.ToolTip = a.btn_name;
                if(a.btn_color!="" || a.btn_color!=null)
                {
                    tb.Background = new System.Windows.Media.SolidColorBrush(StaticHelperClass.ReturnColor(a.btn_color));
                }
                
                tb.Click += this.myClickFunction;
                tb.Tag = a;
                tb.IsEnabled = false;

                smena_button_panel.Children.Add(tb);
            }

            users_window = new WFAddElementFormI();
            
            IEnumerable<t_Employee> employees = order.HDetailsDictionary[typeof(t_Employee)].Values.OfType<t_Employee>();
            List<t_Employee> employees_list = employees.ToList();
            employees_list.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
            users_window.mainControl.DataSource = employees_list;
            users_window.ReturnObject = (o) =>
                {
                    if (o != null)
                    {
                        AddUser((t_Employee)o);
                    }
                };

            smena_window = new EditSmenaWindow1(order.HDetailsDictionary[typeof(t_ShiftType)]);
            smena_window.ReturnObject = (o) =>
            {
                EditDay((SmenaViewClass1)o);
            };

            #endregion

            label_tinfo.Text = String.Format(label_tinfo.Text, order.CurrentDocument.doc_datetime.ToString("MMMM", CultureInfo.CurrentCulture));

        }

        private void EditDay(SmenaViewClass1 smena)
        {
           MarkViewModelItem mi= (MarkViewModelItem)MarkListHash[smena.ID];
           mi.EditSmenaCell(smena);
         //  mi.Update(smena.Day);
           dataGridView1.CurrentCell = null;
        }

        int max_row_number;
        private void AddUser(t_Employee o)
        {
            IEnumerable<MarkViewModelItem> f =
                MarkList.Where(a =>
                    {
                        try
                        {
                            if (a.modelitem!=null&&((t_MarkItems)a.modelitem.t_table).mark_name == o.employee_1C)
                            {
                                return true;
                            }
                            return false;
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    });
            if (f.Count() > 0)
            {
                WpfCustomMessageBox.Show("Сотрудник "+o.employee_name+" уже есть в таблице!");
                return;
            }

            ModelItemClass mi = OrderFactory.ConstructDetail(order, o);

            StaticHelperClass.SetClassItemValue(mi.t_table, "mark_row_id",
                ++max_row_number);
            
            AddToSource((MarkViewModelItem)order.AddDetail(mi,true));
        }

        t_ButtonTemplate current_pressed_button;
        private void myClickFunction(object sender, EventArgs e)
        {
                if (e is System.Windows.RoutedEventArgs)
                {
                    ToggleButton cbutton = ((System.Windows.RoutedEventArgs)e).Source as ToggleButton;
                    if (cbutton != null)
                    {
                        if ((bool)cbutton.IsChecked)
                        {
                            current_pressed_button = (t_ButtonTemplate)cbutton.Tag;
                            List<ToggleButton> blist = smena_button_panel.Children.OfType<ToggleButton>().ToList();
                            blist.ForEach(a =>
                            {
                                if (a != cbutton)
                                    if ((bool)a.IsChecked == true)
                                        a.IsChecked = false;
                            });
                        }
                        else
                        {
                            current_pressed_button = null;
                        }
                    }
                }
        }

        class Respons
        {
            public string StringName{get;set;}
            public string StringGuid{get;set;}
        }

        Hashtable Colors = new Hashtable();
        Hashtable Colors1= new Hashtable();

        private Hashtable rht<T>() where T : ModelClass
        {
            return order.HDetailsDictionary[typeof(T)];
        }

        int sum1 = 0, sum2 = 0;
        int ms = 0, ms1 = 0;

        private void CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex > headColCount)
            {
                if ((string)e.Value != "" && (string)e.Value != null)
                {
                    if (Colors1[e.Value] != null)
                    {
                        dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor =
                            (Color)Colors1[e.Value];
                    }
                }
                else
                {
                    dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                }
            }
           
        }


        private void CellPaint(object sender, DataGridViewCellPaintingEventArgs e)
        {
            #region trash
            //if (e.RowIndex >= 0 && e.ColumnIndex > 3 && e.FormattedValue != "")
            //{
            //    ms = DateTime.Now.Millisecond;
            //    Debug.WriteLine(DateTime.Now.ToString() + DateTime.Now.Millisecond.ToString() + "----" + " красим [" + e.ColumnIndex + " " + e.RowIndex + "]");

            //    t_MarkItems mi = (t_MarkItems)rht<t_MarkItems>()[dataGridView1[0, e.RowIndex].Value];

            //    object o = StaticHelperClass.ReturnClassItemValue(mi,
            //        "mark_guidsmena_" + dataGridView1.Columns[e.ColumnIndex].HeaderText);
            //    if (o != null)
            //    {
            //        if (o != "")
            //        {
            //            Debug.WriteLine(DateTime.Now.ToString() + DateTime.Now.Millisecond.ToString() + "----" + " рассчитали ключ");

            //            dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = (Color)Colors[o];
            //            ms1 = DateTime.Now.Millisecond;
            //            Debug.WriteLine(DateTime.Now.ToString() + DateTime.Now.Millisecond.ToString() + "----" + "покрасили " + (ms1 - ms));
            //            sum1++;
            //            sum2 += (ms1 - ms);
            //            Debug.WriteLine("сумма по времени " + sum2 + " кол-во ячеек " + sum1);
            //        }
            //    }
            //}


            #endregion

            if (e.RowIndex >= 0 && e.ColumnIndex > headColCount)
            {
                if ((string)e.FormattedValue != "")
                {
                    if (Colors1[e.FormattedValue] != null)
                    {
                        dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = (Color)Colors1[e.FormattedValue];
                    }
                }
                else
                {
                    dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                }
            }
            
        }

        private MarkViewModelItem ReturnMVMI(int row_ind)
        {
            return (MarkViewModelItem)MarkListHash[dataGridView1[0, row_ind].Value];
        }

        private void dataGridView1_Paint(object sender,PaintEventArgs e)
        {
            SetGridViewSize();
            MakeItogStyle();
            
        }

        DataGridViewCell LastCurrentCell = null;

        private void CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1) return;
                dataGridView1.Rows[e.RowIndex].Selected = true;

                FilterEnabledButtons(e);
            }catch(Exception ex)
            {
                Log(ex, "CellClick error");
            }
            
        }

        private void FilterEnabledButtons(DataGridViewCellEventArgs e)
        {
            try
            {
                MarkViewModelItem mvmi = ReturnMVMI(e.RowIndex);

                if (mvmi == null) return;
                if (mvmi.Обязанность == String.Empty)
                {
                    LockButtons(null);
                }
                else
                {
                    var o = ((IEnumerable<t_Shifts_Allowed>)SmenaFilters[mvmi.Обязанность]);
                    if (o != null && o.Count() > 0)
                    {
                        var a = (IEnumerable<t_Shifts_Allowed>)SmenaFilters[mvmi.Обязанность];
                        var oo = a.ToList();
                        LockButtons(a);
                    }
                    else
                    {
                        LockButtons(null);
                    }
                }
            }catch(Exception ex)
            {
                Log(ex, "FilterEnabledButtons error");
            }
        }
        private void FilterEnabledButtons(string respons)
        {
            try
            {
                //MarkViewModelItem mvmi = ReturnMVMI(RowIndex);

                //if (mvmi == null) return;
                if (respons == String.Empty)
                {
                    LockButtons(null);
                }
                else
                {
                    var o = ((IEnumerable<t_Shifts_Allowed>)SmenaFilters[respons]);
                    if (o != null && o.Count() > 0)
                    {
                        var a = (IEnumerable<t_Shifts_Allowed>)SmenaFilters[respons];
                        var oo = a.ToList();
                        LockButtons(a);
                    }
                    else
                    {
                        LockButtons(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex, "FilterEnabledButtons error");
            }
        }

        private void LockButtons(IEnumerable<t_Shifts_Allowed> unlockButtons)
        {
            if (unlockButtons == null)
            {
                foreach (var a in smena_button_panel.Children.OfType<ToggleButton>())
                {
                    a.IsEnabled = false;
                    if (a.IsChecked == true)
                    {
                        a.IsChecked = false;
                    }
                }
            }
            else
            {
                var o = unlockButtons.Select(a => a.shift_guid).ToArray();
                foreach (var a in smena_button_panel.Children.OfType<ToggleButton>())
                {
                    var b = (t_ButtonTemplate)a.Tag;
                    if (o.Contains(b.btn_SmenaType))
                    {
                        a.IsEnabled = true;
                    }
                    else
                    {
                        a.IsEnabled = false;
                    }
                }
            }
        }

        

        private void CellDClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1) return;
                if (e.ColumnIndex > headColCount)
                {
                    if (current_pressed_button != null)
                    {
                        MarkViewModelItem mvmi = ReturnMVMI(e.RowIndex);

                        if (mvmi == null) return;


                        if (IsShiftAllowed(mvmi))
                        {
                            mvmi.SetSmenaButtonToCell(current_pressed_button, dataGridView1.Columns[e.ColumnIndex].HeaderText);
                            LastCurrentCell = dataGridView1.CurrentCell;
                            //dataGridView1.CurrentCell = null;
                        }
                    }
                    else
                    {
                        MarkViewModelItem mvmi = ReturnMVMI(e.RowIndex);

                        if (mvmi == null || mvmi.Id == 0) return;
                        OpenSmenaWindow(mvmi, dataGridView1.Columns[e.ColumnIndex].HeaderText);

                        LastCurrentCell = dataGridView1.CurrentCell;
                        dataGridView1.CurrentCell = dataGridView1[3, e.RowIndex];
                        // dataGridView1.CurrentCell = null;    
                    }
                    RecountItog((e.ColumnIndex - headColCount));
                }

            }
            catch (Exception ex)
            {
                Log(ex, "CellDClick error");
            }
        }

        private bool IsShiftAllowed(MarkViewModelItem mvmi)
        {
            if (SmenaFiltersGuids.ContainsKey(mvmi._responsibility))
            {
                return ((IEnumerable<string>)SmenaFiltersGuids[mvmi._responsibility])
                                        .Contains(current_pressed_button.btn_SmenaType);
            }
            return false;
        }

        int headColCount = 3;

        private void OpenSmenaWindow(MarkViewModelItem mvmi, string day)
        {
            string smena_hour=(string)StaticHelperClass.ReturnClassItemValue(mvmi.modelitem.t_table,
                "mark_work_" + day);

            //найти вреям работы теремка
            t_WorkTeremok teremok_work_time = new t_WorkTeremok().SelectFirst<t_WorkTeremok>(String.Format("teremok_id='{0}' AND " +
                       "teremok_day='{1}' AND teremok_month='{2}' AND teremok_year='{3}'", CParam.TeremokId, day, order.CurrentDocument.doc_datetime.Month
                       , order.CurrentDocument.doc_datetime.Year));//DocDate.Month,DocDate.Year
            

            if (smena_hour != "0")
            {
                smena_window.Show((t_MarkItems)mvmi.modelitem.t_table, day, teremok_work_time,SmenaFiltersGuids);
            }
        }

        private List<MarkViewModelItem> SortList(List<MarkViewModelItem> list)
        {
            List<MarkViewModelItem> _list = list;
            _list.Sort((a, b) => a.RowId.CompareTo(b.RowId));
            max_row_number = _list.Last().RowId;
            return _list;
        }

        int dates;
        List<Respons> ls;
        private List<MarkViewModelItem> ItemsSource
        {

            set
            {
                try
                {
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.AllowUserToAddRows = false;
                    
                    
                   dataGridView1.DoubleBuffered(true);



                    ls= (from a in rht<t_Responsibility>()
                              .Values.OfType<t_Responsibility>()
                              select new Respons
                              {
                                  StringName = a.res_name,
                                  StringGuid = a.res_guid
                              }).ToList();

                    DataGridViewTextBoxColumn dcm1 = new DataGridViewTextBoxColumn();
                    dcm1.HeaderText = "Сотрудник";
                    dcm1.Name = "SName";
                    dcm1.DataPropertyName = "Сотрудник";
                    dataGridView1.Columns.Add(dcm1);

                    DataGridViewComboBoxColumn dcm = new DataGridViewComboBoxColumn();
                    dcm.HeaderText = "Обязанность";
                    dcm.DataSource = ls;
                    dcm.DisplayMember = "StringName";
                    dcm.ValueMember = "StringGuid";
                    dcm.DataPropertyName = "Обязанность";
                    dcm.Name = "SResp";
                    dataGridView1.Columns.Add(dcm);

                    if(value.NotNullOrEmpty())MarkList=SortList(value);

                    AddItog();
                    dataGridView1.DataSource = MarkList;

                    dataGridView1.CellFormatting += CellFormatting;

                    dataGridView1.CellValidating+=new DataGridViewCellValidatingEventHandler(dataGridView1_CellValidating);
                    

                    dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
                    dataGridView1.Paint+=new PaintEventHandler(dataGridView1_Paint);
                    dataGridView1.CellDoubleClick += CellDClick;
                    dataGridView1.CellClick += CellClick;
                    dataGridView1.DataError += (s, e) =>
                    {
                    };

                    for (int i = 4; i < dataGridView1.Columns.Count; i++)
                    {
                        DataGridViewColumn dc = dataGridView1.Columns[i];
                        dc.Name = i.ToString();
                        dc.HeaderText = dc.HeaderText.Replace("_", "").Trim();
                        dc.Width = 40;
                        dc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
                        dc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    }
                    dcm.Width = 120;

                    dataGridView1.Columns[0].Visible = false;
                    dataGridView1.Columns[dataGridView1.Columns.Count - 1].Visible = false;

                    dataGridView1.Columns[1].Width = 200;
                    dataGridView1.Columns[1].Frozen = true;
                    dataGridView1.Columns[2].Width = 145;
                    dataGridView1.Columns[2].Frozen = true;
                    dataGridView1.Columns[3].Width = 100;
                    dataGridView1.Columns[3].Frozen = true;

                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if(i!=2)
                        dataGridView1.Columns[i].ReadOnly = true;
                    }

                    dataGridView1.AllowDrop = true;
                    dataGridView1.MouseMove += dataGridView1_MouseMove;
                    dataGridView1.MouseDown += dataGridView1_MouseDown;
                    dataGridView1.DragOver += dataGridView1_DragOver;
                    dataGridView1.DragDrop += dataGridView1_DragDrop;
                    
                    
                    dataGridView1.KeyDown += dataGridView1_KeyDown;
                 
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
            }
        }

        
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                if (WarnOnDeleteIfAnotherResponsibility(e.RowIndex, e.FormattedValue.ToString()))
                {
                    MarkViewModelItem mvmi = ReturnMVMI(e.RowIndex);
                    if (mvmi != null)
                    {
                        mvmi.ClearAllSmenaCell();
                        RecountItog();
                    }
                }
                else
                {
                   // e.Cancel = true;
                    dataGridView1.CancelEdit();
                    dataGridView1.EndEdit();
                }
            }
        }

        private bool WarnOnDeleteIfAnotherResponsibility(int rowindex,string value)
        {
            MarkViewModelItem mvmi = ReturnMVMI(rowindex);
            if (mvmi != null)
            {
                if (mvmi._responsibility == null) return true;

                var o = ls.WhereFirst(a => a.StringGuid == mvmi._responsibility);
                if (o != null)
                {
                    if (o.StringName != value)
                    {
                        if (System.Windows.MessageBoxResult.OK ==
                        WpfCustomMessageBox.Show(
                        "При изменении обязанности сотрудника вся информация о его сменах будет удалена!!! Продолжить?"
                        , "Внимание!"
                        ,System.Windows.MessageBoxButton.OKCancel))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox combo = e.Control as ComboBox;
            if (combo != null)
            {
                combo.SelectedIndexChanged -= new EventHandler(ComboBox_SelectedIndexChanged);
                combo.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            if (cb.SelectedValue != null)
            {
                string item = cb.SelectedValue.ToString();
                FilterEnabledButtons(item);
            }
        }

        #region grid draganddrop


        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        
        private int columnIndexFromMouseDown;
        private int validIndexToDragDrop=1;

        private int rowIndexOfItemUnderMouseToDrop;

        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {

                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dataGridView1.DoDragDrop(
                    dataGridView1.Rows[rowIndexFromMouseDown],
                    DragDropEffects.Move);
                }
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = dataGridView1.HitTest(e.X, e.Y).RowIndex;
            columnIndexFromMouseDown= dataGridView1.HitTest(e.X, e.Y).ColumnIndex;

            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                               e.Y - (dragSize.Height / 2)),
                                    dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void dataGridView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = dataGridView1.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop =
                dataGridView1.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                if (columnIndexFromMouseDown != validIndexToDragDrop) return;

                SwitchRows(rowIndexFromMouseDown, rowIndexOfItemUnderMouseToDrop);
            }
        }

        private void SwitchRows(int rowIndexFromMouseDown, int rowIndexOfItemUnderMouseToDrop)
        {

            MarkViewModelItem mvmi1 = ReturnMVMI(rowIndexFromMouseDown);
            MarkViewModelItem mvmi2 = ReturnMVMI(rowIndexOfItemUnderMouseToDrop);

            if (mvmi1 == null || mvmi2 == null) return;
            if (mvmi1.Id == 0 || mvmi2.Id == 0) return;

            int rowid = mvmi1.RowId;
            mvmi1.ChangeRowIndex(mvmi2.RowId);
            mvmi2.ChangeRowIndex(rowid);
            UpdateSource();
        }



        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            users_window.ShowDialoG();
        }

        private void DeleteUser(MarkViewModelItem item)
        {
            order.RemoveViewDetail(item, true);
            DelFromSource(item);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (WpfCustomMessageBox.Show("Действительно удалить сотрудника?", "Внимание", System.Windows.MessageBoxButton.OKCancel) ==
                System.Windows.MessageBoxResult.OK)
            {
                if (dataGridView1.CurrentCell != null)
                {
                    MarkViewModelItem mvmi = (MarkViewModelItem)
                        MarkListHash[dataGridView1[0, dataGridView1.CurrentCell.RowIndex].Value];
                    DeleteUser(mvmi);
                }
            }
        }

        public void  TabelView1_FormClosing(object sender, FormClosingEventArgs e)
        {
#if(!DEB)
                if (IsDocumentTabelBlocked != true)
#else
            /*if (IsDocumentTabelBlocked != true)*/
            if (true)
#endif
            {
                List<string[]> sortList1 = new List<string[]>();

                List<string> family_list = new List<string>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string name = ((DataGridViewTextBoxCell)CellHelper.
                        FindCell(dataGridView1, row.Index, 1)).Value.ToString();
                    family_list.Add(name);
                }

                DataTable source =StaticConstants.CBData.OrderMark(order.CurrentDocument.doc_id, 28);

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

                send_tabel(order.CurrentDocument.doc_id, dates, order.CurrentDocument.doc_datetime, sortList1);
                StaticConstants.IsTabelOpened = false;
            }
        }

        public static void send_tabel(int doc_id, int dates, DateTime doc_datetime, List<string[]> sortList1)
        {
            //if (dates == 0)
            //{
            //    dates = DateTime.DaysInMonth(Convert.ToInt32(order.CurrentDocument.doc_datetime.Year),
            //    Convert.ToInt32(order.CurrentDocument.doc_datetime.Month));
            //}

            TabelBackgroundWorker.CreateWorker(doc_id, dates, doc_datetime);
            TabelBackgroundWorker.CurrentWorker.sortList = sortList1;
            TabelBackgroundWorker.CurrentWorker.Start(); //отправка табеля в потоке
        }

        

        private bool _readdoc = false;
        public bool IsDocumentTabelBlocked
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
                    DisableAll();
                    //this.Enabled = false;           //выключить 
                }
            }
        }

        private void DisableAll()
        {
            foreach(var a in this.Controls)
            {
                ((Control)a).Enabled = false;
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}

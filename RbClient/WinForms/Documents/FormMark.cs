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

namespace RBClient
{
    public partial class FormMark : Form
    {
        public int cell_count = 0;
        public string btnValue = "";
        public int _doc_id = 0;
        public string _btn_guid = "";
        public string _btn_guidSmena = ""; 
        private bool pressBtn = false;
        public bool readDoc = false;
        private int dates;
        //private bool openDoc = false;
        private int _btn_time = 0;
        private DateTime jobStart;
        private DateTime jobEnd;
        private int count_Layout = 0;

        private Rectangle dragBoxSrc;
        private int rowIndexSrc;
        private int rowIndexTar;
        private DataSet _ds = new DataSet();

        CBData bd = new CBData();
        ARMWeb systemService = StaticConstants.WebService;
        private BindingSource masterBindingSource = new BindingSource();

        public FormMark()
        {
            InitializeComponent();
        }

        private void FormGrid_Load(object sender, EventArgs e)
        {
            try
            {            
            
            if (readDoc == true)
            {
                panelEx1.Enabled = false;
            }

            CBData bd = new CBData();
            this.SetBounds(0, 0, Parent.Width - 4, Parent.Height - 4);
            LoadGrid();
            Cursor.Current = Cursors.Default;
           
                DataTable ptr = bd.ButtonFullList();

                DevComponents.DotNetBar.ButtonX[] myBtn = new DevComponents.DotNetBar.ButtonX[ptr.Rows.Count];
                int lastRange = 225;
                int i = 0;
                foreach (DataRow row in ptr.Rows)
                {
                    myBtn[i] = new DevComponents.DotNetBar.ButtonX();
                    myBtn[i].Size = new Size(50, 23);
                    if (i == 0)
                    {
                        myBtn[i].Location = new Point(lastRange * 1, 5);
                        lastRange = myBtn[i].Location.X + 50;
                    }
                    if (i >= 1 && i <= 9)
                    {
                        myBtn[i].Location = new Point((lastRange + 5), 5);
                        lastRange = myBtn[i].Location.X + 50;
                    }
                    if (i == 10)
                    {
                        lastRange = 225;
                        myBtn[i].Location = new Point(lastRange * 1, 30); 
                        lastRange = myBtn[i].Location.X + 50;
                    }
                    if (i > 10)
                    {
                        myBtn[i].Location = new Point((lastRange + 5), 30);
                        lastRange = myBtn[i].Location.X + 50;
                    }
                    myBtn[i].AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
                    myBtn[i].ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
                    myBtn[i].FocusCuesEnabled = false;
                    myBtn[i].Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    myBtn[i].Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
                    myBtn[i].Tag = i;
                    myBtn[i].Name = row[1].ToString();
                    myBtn[i].Text = row[3].ToString();
                    myBtn[i].Tooltip = row[2].ToString();
                    myBtn[i].Click += new System.EventHandler(this.myClickFunction);

                    this.panelEx1.Controls.Add(myBtn[i]);
                    i++;
                }
            }
            catch (Exception _ex)
            {
                MessageBox.Show(_ex.ToString());
            }
        }

        private void FormMark_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                CBData _data = new CBData();
                systemService.Credentials = new NetworkCredential("iteremok", "Ge6!))~nC5@0");
                BlokDoc _bk = new BlokDoc { Date = DateTime.Now, ID = _data.returnDocGuid(_doc_id) };
                BlokResult _bkr = systemService.IsBlocked(Convert.ToInt32(CParam.TeremokId), _bk, false);
            }
            catch (Exception)
            {

            }
        }

        private void LoadGrid()
        {
            CBData db = new CBData();

            dataGridViewMain.AutoGenerateColumns = false;
            OrderMark(_doc_id, 28);
            try
            {
                dataGridViewMain.Columns.Add(new DataGridViewTextBoxColumn());
                dataGridViewMain.Columns[0].DataPropertyName = "mark_id";
                dataGridViewMain.Columns[0].Visible = false;

                dataGridViewMain.Columns.Add(new DataGridViewTextBoxColumn());
                dataGridViewMain.Columns[1].DataPropertyName = "employee_name";
                dataGridViewMain.Columns[1].HeaderText = "Сотрудник";
                //dataGridViewMain.Columns[1].ReadOnly = true;
                dataGridViewMain.Columns[1].Width = 200;
                dataGridViewMain.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                dataGridViewMain.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridViewMain.Columns[1].Frozen = true;

                //dataGridViewMain.Columns.Add(new DataGridViewComboBoxColumn());
                //dataGridViewMain.Columns[2].DataPropertyName = _ds.Tables[1].Columns[2].ToString(); 
                //dataGridViewMain.Columns[2].HeaderText = "Должность";
                //dataGridViewMain.Columns[2].ReadOnly = true;
                //dataGridViewMain.Columns[2].Width = 100;
                //dataGridViewMain.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                //dataGridViewMain.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                //dataGridViewMain.Columns[2].Frozen = true;
                ////dataGridViewMain.Columns[2].DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
               
                DataGridViewComboBoxColumn comboCell = new DataGridViewComboBoxColumn();
                comboCell.Name = "Обязанность";
                comboCell.DataPropertyName = "";
                comboCell.ReadOnly = false;
                dataGridViewMain.Columns.Add(comboCell);
                comboCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;

                dataGridViewMain.Columns.Add(new DataGridViewTextBoxColumn());
                dataGridViewMain.Columns[3].DataPropertyName = "";
                dataGridViewMain.Columns[3].HeaderText = "Итого";
                dataGridViewMain.Columns[3].ReadOnly = true;
                dataGridViewMain.Columns[3].Width = 100;
                dataGridViewMain.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                dataGridViewMain.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridViewMain.Columns[3].Frozen = true;

                dates = DateTime.DaysInMonth(Convert.ToInt32(DateTime.Today.Year), Convert.ToInt32(DateTime.Today.Month));

                for (int i = 1; i < dates + 1; i++)
                {
                    dataGridViewMain.Columns.Add(new DataGridViewTextBoxColumn());
                    dataGridViewMain.Columns[i + 3].DataPropertyName = "mark_" + i;
                    //dataGridViewMain.Columns[i + 3].DataPropertyName = "";
                    dataGridViewMain.Columns[i + 3].HeaderText = i.ToString();
                    dataGridViewMain.Columns[i + 3].ReadOnly = true;
                    dataGridViewMain.Columns[i + 3].Width = 65;
                    dataGridViewMain.Columns[i + 3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dataGridViewMain.Columns[i + 3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dataGridViewMain.Columns[i + 3].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                dataGridViewMain.DataSource = masterBindingSource;

                for (int n = 0; n < _ds.Tables[1].Rows.Count; n++)
                {
                    DataGridViewRow row0 = dataGridViewMain.Rows[0];
                    DataGridViewComboBoxCell comboCel = (DataGridViewComboBoxCell)row0.Cells[2];
                    string abc = _ds.Tables[1].Rows[n].ItemArray[2].ToString();
                    comboCell.Items.AddRange(abc);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
            }
        }

        public void OrderMark(int doc_id, int type_doc)
        {
            DataTable _table;
            DataTable _table2;
            string _str_connect = CParam.ConnString;
            string _str_command;
            string _str_command2;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            try
            {
                _str_command = "SELECT t_MarkItems.*, t_Employee.employee_name, t_Employee.employee_FunctionName " +
                               "FROM t_MarkItems INNER JOIN t_Employee ON t_MarkItems.mark_name = t_Employee.employee_1C " +
                               "WHERE mark_doc_id =" + doc_id + " ORDER BY mark_row_id ASC";

                _str_command2 = "SELECT * FROM t_Responsibility";

                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("t_MarkItems");
                _data_adapter.Fill(_table);

                OleDbDataAdapter _data_adapter2 = new OleDbDataAdapter(_str_command2, conn);
                _table2 = new DataTable("t_Responsibility");
                _data_adapter2.Fill(_table2);

                _ds.Tables.Add(_table);
                _ds.Tables.Add(_table2);
                masterBindingSource.DataSource = _ds;
                masterBindingSource.DataMember = "t_MarkItems";
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        private void myClickFunction(object sender, EventArgs e)
        {
            
            DevComponents.DotNetBar.ButtonX btn = (DevComponents.DotNetBar.ButtonX)sender;
            int pressedBnt = Convert.ToInt32(((DevComponents.DotNetBar.ButtonX)sender).Tag);
            btnValue = Convert.ToString(((DevComponents.DotNetBar.ButtonX)sender).Text);
            _btn_guid = Convert.ToString(((DevComponents.DotNetBar.ButtonX)sender).Name);
            _btn_time = Convert.ToInt32(bd.GetValue(_btn_guid));
            _btn_guidSmena = bd.GetValueSmena(_btn_guid);

            List<String> tagButtn = new List<string>();

            foreach (Control control in panelEx1.Controls)
            {
                DevComponents.DotNetBar.ButtonX tb = control as DevComponents.DotNetBar.ButtonX;
                if (tb != null)
                {
                    tb.Checked = false;
                    pressBtn = false;
                }
            }

            for (int i = 0; i < panelEx1.Controls.Count; i++)
            {
                string tag = Convert.ToString(panelEx1.Controls[i].Tag);
                tagButtn.Add(tag);
            }

            for (int b = 0; b < tagButtn.Count; b++)
            {
                if (Convert.ToInt32(tagButtn[b]) == pressedBnt)
                {
                    btn.Checked = true;
                    pressBtn = true;
                }
            }
        }


        private void InData()
        {
            CBData _rf = new CBData();
            try
            {
                // связать с данными
                //dataGridViewMain.DataSource = OrderMark(_doc_id, 28);
                dataGridViewMain.DataSource = masterBindingSource;
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

        private void buttonX1_Click(object sender, EventArgs e)
        {
            try
            {
                FormEmployee emp = new FormEmployee();
                emp.docID = _doc_id;
                emp.rowCount = dataGridViewMain.Rows.Count;
                emp.ShowDialog();
                InData(); 
                paintGrid();

                jobOther();
            }
            catch (Exception)
            {
                //
            }
        }

        private void dataGridViewMain_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CBData _data = new CBData();
            string lineOr = MyTrim(_data.returnColor(_btn_guid));
            string[] colorArr = lineOr.Split(',', ' ');


            if (dataGridViewMain.CurrentCell.ColumnIndex != 0 && dataGridViewMain.CurrentCell.ColumnIndex != 1 && dataGridViewMain.CurrentCell.ColumnIndex != 2)
            {
                jobStart = Convert.ToDateTime(bd.returnJobTeremokStart(e.ColumnIndex));
                jobEnd = Convert.ToDateTime(bd.returnJobTeremokEnd(e.ColumnIndex));
                if (pressBtn == true)
                {
                    dataGridViewMain.CurrentCell.Value = btnValue;
                    if (colorArr.Length == 3)
                    {
                        dataGridViewMain.CurrentCell.Style.BackColor = Color.FromArgb(Convert.ToInt32(colorArr[0]), Convert.ToInt32(colorArr[1]), Convert.ToInt32(colorArr[2]));
                    }
                    dataGridViewMain.CurrentCell.Style.ForeColor = Color.Black;
                    dataGridViewMain.CurrentCell.Style.Font = new Font("Tahoma", 8, FontStyle.Bold);
                    _data.OrderMarkUpdate(_btn_guid, btnValue, e.ColumnIndex, Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value.ToString()), jobStart, _btn_time, _btn_guidSmena);
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

        private void buttonSave_Click(object sender, EventArgs e)
        {
            CBData db = new CBData();
            systemService.Credentials = new NetworkCredential("iteremok", "Ge6!))~nC5@0");
            WorkEmployee[] arrWe = new WorkEmployee[db.CountMark(_doc_id)];
            string docGuid = db.returnDocGuid(_doc_id);
            DataTable all = db.allMark(_doc_id);

            for (int a = 0; a < arrWe.Length; a++)
            { 
                WorkEmployee we = new WorkEmployee { Name = all.Rows[a].ItemArray[0].ToString(), ArrayDayWork = myDayArr1(a) };
                arrWe[a] = we;
            }

            PlanDocument pd = new PlanDocument { Date = DateTime.Today, ID = docGuid.ToString(), ArrayWorkEmployee = arrWe };
            string result = systemService.PutDocument(Convert.ToInt32(CParam.TeremokId), pd);
            if (result == "1")
            {
                MessageBox.Show("Обмен завершен");
            }
            else
            {
                MessageBox.Show("Сбой обмена");
            }
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
            }
        }

        private void deleteRow()
        {
            CBData _data = new CBData();

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
                            InData();
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
            catch(Exception) 
            {
            }
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

        private void dataGridViewMain_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridViewMain.CurrentCell.ColumnIndex != 0 && dataGridViewMain.CurrentCell.ColumnIndex != 1 && dataGridViewMain.CurrentCell.ColumnIndex != 2 && dataGridViewMain.CurrentCell.ColumnIndex != null)
                {
                    //DataTable _dt = bd.OrderMark(_doc_id, 28);
                    CellMark _cm = new CellMark();
                    _cm.starJob = jobStart;
                    _cm.endJob = jobEnd;
                    _cm.rowID = Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value);
                    _cm.columnID = e.ColumnIndex - 3;
                    _cm._dt = bd.OrderMarkDetails(_doc_id, e.ColumnIndex - 3, Convert.ToInt32(dataGridViewMain.CurrentRow.Cells[0].Value));
                    _cm.ShowDialog();
                }
                if (dataGridViewMain.CurrentCell.ColumnIndex == 2)
                {

                }
            }
            catch (Exception)
            {
            }
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

        private void dataGridViewMain_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewComboBoxEditingControl)
            {
                ((ComboBox)e.Control).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)e.Control).AutoCompleteSource = AutoCompleteSource.ListItems;
            } 
        }

        private void dataGridViewMain_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}


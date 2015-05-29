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
    public partial class CellMark : Form
    {
        public DateTime starJob;
        public DateTime endJob;
        public int rowID;
        public int columnID;
        public DataTable _dt;
        public int timeValueHour = 0;
        public string typeValue = null;
        private bool timeJob = false;
        private bool hourChange = false;
        
        public CellMark()
        {
            InitializeComponent();

            textBox_start.TextChanged+=new EventHandler(textBox_start_TextChanged);
        }

        private void textBox_start_TextChanged(object sender,EventArgs e)
        {
            integerInput1_ValueChanged(sender,e);
        }

        private void CellMark_Load(object sender, EventArgs e)
        {
            CBData _db = new CBData();

            try
            {
                comboBoxEx1.Text = _db.returnShiftType(typeValue);
                integerInput1.Text = timeValueHour.ToString();
                if (_dt.Rows[0][0].ToString() == "" || _dt.Rows[0][0].ToString() == "0:00")
                {
                    textBox_start.Text = starJob.ToString();
                    timeJob = false;
                }
                else
                {
                    textBox_start.Text = _dt.Rows[0][0].ToString();
                    textBox_end.Text = _dt.Rows[0][1].ToString();
                    timeJob = true;
                }
            }
            catch (Exception exp)
            {
                MDIParentMain.log.Error("Ошибка создания окна CellMark: "+exp);
            }
        }

        private void comboTree_hour_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void buttonX_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            CBData _db = new CBData();
            int starJobEmp = 0;
            int valTime = 0;
            try
            {
                if (integerInput1.Value != 0)
                {
                    valTime = integerInput1.Value;
                }
                //if (comboTree_hour.Text != "")
                //{
                //    valTime = Convert.ToInt32(comboTree_hour.Text);
                //}
                if (timeJob == false)
                {
                    starJobEmp = int.Parse(textBox_start.Text.Substring(0, textBox_start.Text.IndexOf(":")));
                    //starJobEmp = starJob.Hour;
                }
                else
                {
                    string[] time = _dt.Rows[0][0].ToString().Split(':');
                    starJobEmp = Convert.ToInt32(time[0]);
                }
                int EndJobEmp = valTime + starJobEmp;
                if (EndJobEmp > endJob.Hour)
                {
                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show("Oшибка ввода данных");
                        comboTree_hour.SelectedIndex = -1;
                        textBox_end.Text = "0:00:00";
                    }
                }
                if (hourChange == true)
                {
                    textBox_end.Text = new DateTime(2000, 10, 10, starJobEmp + valTime, 0, 0).ToString();
                    //textBox_end.Text = starJob.AddHours(valTime).ToString();
                    string shortName = _db.returnShortName(_db.returnShift(comboBoxEx1.Text));
                    if (integerInput1.Value != 0)
                    {
                        _db.OrderMarkUpdate2(rowID, columnID, textBox_start.Text, textBox_end.Text, integerInput1.Value.ToString(), _db.returnShift(comboBoxEx1.Text), shortName.ToString() + integerInput1.Value.ToString());
                    }
                    else
                    {
                        _db.OrderMarkUpdate2(rowID, columnID, textBox_start.Text, textBox_end.Text, integerInput1.Value.ToString(), _db.returnShift(comboBoxEx1.Text), shortName.ToString());
                    }
                }
                else
                {
                    // _db.OrderMarkUpdate2(rowID, columnID, textBox_start.Text, textBox_end.Text, valTime.ToString(), comboBoxEx1.SelectedValue.ToString());
                }
            }
            catch (Exception)
            {
                //throw exp;
            }
            finally
            {
                this.Close();
            }
        }

        private void integerInput1_ValueChanged(object sender, EventArgs e)
         {
            CBData _db = new CBData();
            hourChange = true;
            try
            {
                int valTime = integerInput1.Value;
                    //Convert.ToInt32(((DevComponents.Editors.IntegerInput)sender).Value);

                int starJobEmp = int.Parse(textBox_start.Text.Substring(0,textBox_start.Text.IndexOf(":")));
                int EndJobEmp = valTime + starJobEmp;
                if (EndJobEmp > endJob.Hour)
                {
                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show("Oшибка ввода данных!\nCмена сотрудника не может превышать рабочий график работы ресторана.");
                        textBox_end.Text = "0:00:00";
                    }
                }
                else
                {
                   // textBox_end.Text = starJob.AddHours(valTime).ToString();

                    textBox_end.Text = new DateTime(2000, 10, 10, starJobEmp + valTime, 0, 0).ToString();
                    #region trash
                    //_db.OrderMarkUpdate2(rowID, columnID, textBox_start.Text, textBox_end.Text, valTime.ToString(), comboBoxEx1.SelectedValue.ToString());
                    #endregion
                }
            }
            catch (Exception exp)
            {
                //
            }
        }

        private void comboBoxEx1_DropDown(object sender, EventArgs e)
        {
            CBData _data = new CBData();
            comboBoxEx1.DisplayMember = "type_name";
            comboBoxEx1.ValueMember = "type_guid";

            
            comboBoxEx1.DataSource = _data.ShiftTypeFullList_Group();

            //comboBoxEx1.DataSource = _data.ShiftTypeFullList();

        }        
    }
}

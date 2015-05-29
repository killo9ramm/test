using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
//using System.Data.SqlClient;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using DevComponents.DotNetBar.Controls;
using RBClient.Classes;
using System.Collections;
using RBClient.Classes.Utilization;
using RBClient.ModalWindows;
using RBClient.Classes.CustomClasses;
using RBClient.Classes.InternalClasses.Models;

namespace RBClient
{
    public partial class FormGrid : Form
    {
        public int m_doc_id;        // код документа
        private int m_doc_state_id; // статус документа
        public int m_doc_type_id;   // тип документа
        public int m_teremok_id = 0;
        public int m_teremok_dep;
        public int m_doc_ban;
        public bool open_doc = false;
        //private bool m_is_cell_edit = false; // для корректного выхода по ескейпу
        private bool m_is_close = false; // для проверки квоты перед закрытием
        private bool img_error = false;
        public int i;


        public FormGrid()
        {
            InitializeComponent();

            gridView.RowsAdded += new DataGridViewRowsAddedEventHandler(gridView_RowsAdded);

            CultureInfo curCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo newCulture = new CultureInfo("ru-RU");
            newCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = newCulture;

            

        }

        private void gridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
        }

        private void FormGrid_Load(object sender, EventArgs e)
        {
            if (!StaticConstants.GetNomesThreadWorking)
            {
                new Thread(new ThreadStart(delegate()
                {
                    StaticConstants.FullNomenclatureList = null;
                    StaticConstants.GetNomesThreadWorking = true;
                    RbClientGlobalStaticMethods.GetFullNomeList(m_doc_id);
                    StaticConstants.GetNomesThreadWorking = false;
                })).Start();
            }

            // установить главное окно
            this.SetBounds(0, 0, Parent.Width - 4, Parent.Height - 4);
            LoadGrid();
            Cursor.Current = Cursors.Default;
        }


        // загрузить грид
        private void LoadGrid()
        {
            gridView.AutoGenerateColumns = false;
            gridView.ShowCellToolTips = false;

            try
            {
                CBData _data = new CBData();

                // форматируем окно 
                this.Text = _data.GetTypeNameDoc(m_doc_id) + " № " + m_doc_id.ToString() + "    Статус документа: " + _data.GetDocStateName(_data.GetDocState(m_doc_id));
                m_doc_state_id = _data.GetDocState(m_doc_id); // статус документа
                m_doc_type_id = _data.GetDocType(m_doc_id);
                m_teremok_dep = _data.GetTeremokDep(m_teremok_id);
                m_doc_ban = _data.getBanid(m_doc_type_id);

                // стиль для всех документов предустановлен
                //-----------------------------------------
                // 0 - ID
                // 1 - наименование
                // 2 - кол-во для заказа
                // 3 - единица заказа
                // 4 - коэф
                // 5 - базовая единица
                // 6 - всего в базовой единице
                // 7 - доп. поле ввода (число)
                // 8 - доп. поле ввода (текст)
                //-----------------------------------------
                // в зависимости от типа документа столбцы либо доступны, либо нет
                #region grid_definition
                // по умолчанию отключим все столбцы и настроим форматирование
                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[0].DataPropertyName = "opd_id";
                gridView.Columns[0].Visible = false;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[1].DataPropertyName = "nome_name";
                gridView.Columns[1].HeaderText = "Позиция";
                gridView.Columns[1].ReadOnly = true;
                gridView.Columns[1].Width = 430;
                gridView.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                gridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[2].DataPropertyName = "opd_order";
                gridView.Columns[2].HeaderText = "Кол-во";
                gridView.Columns[2].ReadOnly = true;
                gridView.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                gridView.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[2].DefaultCellStyle.NullValue = "";
                gridView.Columns[2].DefaultCellStyle.Format = "N";
                gridView.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;


                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[3].DataPropertyName = "opd_ed";
                gridView.Columns[3].Visible = false;
                gridView.Columns[3].ReadOnly = true;
                gridView.Columns[3].HeaderText = "Ед.";
                gridView.Columns[3].Width = 70;
                gridView.Columns[3].DefaultCellStyle.NullValue = "";
                gridView.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[4].DataPropertyName = "opd_K";
                gridView.Columns[4].Visible = false;
                gridView.Columns[4].ReadOnly = true;
                gridView.Columns[4].HeaderText = "Kоэф.";
                gridView.Columns[4].Width = 60;
                gridView.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[4].DefaultCellStyle.Format = "N";
                gridView.Columns[4].DefaultCellStyle.NullValue = "";
                gridView.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[5].DataPropertyName = "opd_bed";
                gridView.Columns[5].Visible = false;
                gridView.Columns[5].ReadOnly = true;
                gridView.Columns[5].HeaderText = "Баз.ед.";
                gridView.Columns[5].Width = 50;
                gridView.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[6].DataPropertyName = "opd_order_total";
                gridView.Columns[6].Visible = false;
                gridView.Columns[6].ReadOnly = true;
                gridView.Columns[6].HeaderText = "Итого";
                gridView.Columns[6].Width = 80;
                gridView.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[6].DefaultCellStyle.BackColor = Color.AliceBlue;
                gridView.Columns[6].DefaultCellStyle.Format = "N";
                gridView.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[7].DataPropertyName = "opd_order2";
                gridView.Columns[7].Visible = false;
                gridView.Columns[7].ReadOnly = true;
                gridView.Columns[7].HeaderText = "Довоз";
                gridView.Columns[7].Width = 80;
                gridView.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[7].DefaultCellStyle.Format = "N";
                gridView.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[8].DataPropertyName = "opd_quota";
                gridView.Columns[8].Visible = false;
                gridView.Columns[8].ReadOnly = true;
                gridView.Columns[8].HeaderText = "Норма";
                gridView.Columns[8].Width = 70;
                gridView.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[8].DefaultCellStyle.Format = "G";
                gridView.Columns[8].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[9].DataPropertyName = "orr_name";
                gridView.Columns[9].Visible = false;
                gridView.Columns[9].ReadOnly = true;
                gridView.Columns[9].HeaderText = "Категория возврата";
                gridView.Columns[9].Width = 250;
                gridView.Columns[9].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[9].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[10].DataPropertyName = "orr_id";
                gridView.Columns[10].Width = 70;
                gridView.Columns[10].ReadOnly = true;
                gridView.Columns[10].Visible = false;
                gridView.Columns[10].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[10].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[11].DataPropertyName = "opd_rerreason_desc";
                gridView.Columns[11].Visible = false;
                gridView.Columns[11].ReadOnly = true;
                gridView.Columns[11].HeaderText = "Описание возврата";
                gridView.Columns[11].Width = 250;
                gridView.Columns[11].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[12].DataPropertyName = "opd_order3";
                gridView.Columns[12].Visible = false;
                gridView.Columns[12].ReadOnly = true;
                gridView.Columns[12].HeaderText = "Кол-во 3";
                gridView.Columns[12].Width = 80;
                gridView.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[12].DefaultCellStyle.Format = "N";
                gridView.Columns[12].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[12].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[13].DataPropertyName = "opd_maxquota";
                gridView.Columns[13].Visible = false;
                gridView.Columns[13].ReadOnly = true;
                gridView.Columns[13].HeaderText = "Норма";
                gridView.Columns[13].Width = 70;
                gridView.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[13].DefaultCellStyle.Format = "G";
                gridView.Columns[13].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[13].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[14].DataPropertyName = "opd_ke";
                gridView.Columns[14].Visible = false;
                gridView.Columns[14].ReadOnly = true;
                gridView.Columns[14].HeaderText = "Kоэф. A";
                gridView.Columns[14].Width = 60;
                gridView.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[14].DefaultCellStyle.Format = "N";
                gridView.Columns[14].DefaultCellStyle.NullValue = "";
                gridView.Columns[14].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[14].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[15].DataPropertyName = "opd_ze";
                gridView.Columns[15].Visible = false;
                gridView.Columns[15].ReadOnly = true;
                gridView.Columns[15].HeaderText = "Ед.";
                gridView.Columns[15].Width = 50;
                gridView.Columns[15].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[15].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[16].DataPropertyName = "opd_kke";
                gridView.Columns[16].Visible = false;
                gridView.Columns[16].ReadOnly = true;
                gridView.Columns[16].HeaderText = "Kоэф.";
                gridView.Columns[16].Width = 60;
                gridView.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[16].DefaultCellStyle.Format = "N";
                gridView.Columns[16].DefaultCellStyle.NullValue = "";
                gridView.Columns[16].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[16].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[17].DataPropertyName = "opd_total1";
                gridView.Columns[17].Visible = false;
                gridView.Columns[17].ReadOnly = true;
                gridView.Columns[17].HeaderText = "ИТОГО";
                gridView.Columns[17].Width = 80;
                gridView.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[17].DefaultCellStyle.BackColor = Color.AliceBlue;
                gridView.Columns[17].DefaultCellStyle.Format = "N";
                gridView.Columns[17].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[17].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[18].DataPropertyName = "opd_bold";
                gridView.Columns[18].Visible = false;
                gridView.Columns[18].ReadOnly = true;
                gridView.Columns[18].HeaderText = "Bold";
                gridView.Columns[18].Width = 80;
                gridView.Columns[18].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[18].DefaultCellStyle.BackColor = Color.BlanchedAlmond;
                gridView.Columns[18].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[18].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[19].DataPropertyName = "nome_1C";
                gridView.Columns[19].Visible = false;
                gridView.Columns[19].ReadOnly = true;
                gridView.Columns[19].Width = 80;
                gridView.Columns[19].SortMode = DataGridViewColumnSortMode.NotSortable;
                gridView.Columns[19].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[20].DataPropertyName = "opd_retreason_id";
                gridView.Columns[20].HeaderText = "Время";
                gridView.Columns[20].Visible = false;
                gridView.Columns[20].ReadOnly = false;
                gridView.Columns[20].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                gridView.Columns[20].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[20].DefaultCellStyle.NullValue = "";
                gridView.Columns[20].DefaultCellStyle.Format = "HH:mm";
                gridView.Columns[20].SortMode = DataGridViewColumnSortMode.NotSortable;

                gridView.Columns.Add(new DataGridViewTextBoxColumn());
                gridView.Columns[21].DataPropertyName = "urr_name";
                gridView.Columns[21].HeaderText = "Списания";
                gridView.Columns[21].Visible = false;
                gridView.Columns[21].ReadOnly = true;
                gridView.Columns[21].Width = 180;
                gridView.Columns[21].HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                gridView.Columns[21].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridView.Columns[21].SortMode = DataGridViewColumnSortMode.NotSortable;
#endregion

                #region good

                int _status_type = _data.GetStatusTypeID(m_doc_state_id);

                // форматируем в зависимости от статуса документа                
                if (_status_type == 1 ) // 1. новый
                {
                    gridView.Columns[2].ReadOnly = false;    // кол-во
                    gridView.Columns[7].ReadOnly = false;    // кол-во
                    gridView.Columns[9].ReadOnly = false;    // кол-во
                    gridView.Columns[11].ReadOnly = false;   // кол-во
                    gridView.Columns[12].ReadOnly = false;   // кол-во
                    gridView.Columns[20].ReadOnly = false;   // кол-во
                    gridView.Columns[2].DefaultCellStyle.BackColor = Color.AliceBlue;
                    gridView.Columns[7].DefaultCellStyle.BackColor = Color.AliceBlue;
                    gridView.Columns[9].DefaultCellStyle.BackColor = Color.AliceBlue;
                    gridView.Columns[11].DefaultCellStyle.BackColor = Color.AliceBlue;
                    gridView.Columns[12].DefaultCellStyle.BackColor = Color.AliceBlue;
                    gridView.Columns[17].DefaultCellStyle.BackColor = Color.AliceBlue;
                    gridView.Columns[20].DefaultCellStyle.BackColor = Color.AliceBlue;
                }
                if (_status_type == 2 || _status_type == 3 || _status_type == 4)  // отправляется, оптправлен
                {
                    gridView.Columns[2].ReadOnly = true;    // кол-во
                    gridView.Columns[7].ReadOnly = true;    // кол-во
                    gridView.Columns[9].ReadOnly = true;    // кол-во
                    gridView.Columns[11].ReadOnly = true;   // кол-во
                    gridView.Columns[12].ReadOnly = true;   // кол-во
                }

                // форматируем разницу СПб/Москва + открываем столбцы на спросмотр
                switch (m_doc_type_id)
                {
                    case 1:
                        if (CParam.AppCity == 1)    //питер
                        {
                            gridView.Columns[2].DefaultCellStyle.Format = "N4";
                            gridView.Columns[6].DefaultCellStyle.Format = "N4";
                            gridView.Columns[16].DefaultCellStyle.Format = "N0";
                            gridView.Columns[5].HeaderText = "Ед.измер";
                            gridView.Columns[7].Visible = true;
                            gridView.Columns[7].DisplayIndex = 3;
                        }
                        gridView.Columns[2].Width = 100;
                        gridView.Columns[15].Visible = _data.returnStatusColumnID(15, m_doc_type_id);
                        gridView.Columns[15].DisplayIndex = 4;
                        gridView.Columns[15].Width = 75;
                        gridView.Columns[16].Visible = _data.returnStatusColumnID(16, m_doc_type_id);
                        gridView.Columns[16].DisplayIndex = 5;
                        gridView.Columns[16].Width = 75;
                        gridView.Columns[5].Visible = _data.returnStatusColumnID(5, m_doc_type_id);
                        gridView.Columns[5].DisplayIndex = 6;
                        gridView.Columns[5].Width = 100;
                        gridView.Columns[6].Visible = true;
                        gridView.Columns[6].DisplayIndex = 7;
                        
                        gridView.Columns[6].Width = 100;
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;

                    case 29:
                        if (CParam.AppCity == 1)
                        {
                            gridView.Columns[2].DefaultCellStyle.Format = "N4";
                            gridView.Columns[6].DefaultCellStyle.Format = "N4";
                            gridView.Columns[16].DefaultCellStyle.Format = "N0";
                            gridView.Columns[5].HeaderText = "Ед.измер";
                        }
                        gridView.Columns[2].Width = 100;
                        gridView.Columns[15].Visible = _data.returnStatusColumnID(15, m_doc_type_id);
                        gridView.Columns[15].DisplayIndex = 3;
                        gridView.Columns[15].Width = 75;
                        gridView.Columns[16].Visible = _data.returnStatusColumnID(16, m_doc_type_id);
                        gridView.Columns[16].DisplayIndex = 4;
                        gridView.Columns[16].Width = 75;
                        gridView.Columns[5].Visible = _data.returnStatusColumnID(5, m_doc_type_id);
                        gridView.Columns[5].DisplayIndex = 5;
                        gridView.Columns[5].Width = 100;
                        gridView.Columns[6].Visible = true;
                        gridView.Columns[6].DisplayIndex = 6;
                        gridView.Columns[6].Width = 100;
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;

                    case 2:
                        gridView.Columns[2].Width = 150;
                        gridView.Columns[15].Visible = _data.returnStatusColumnID(15, m_doc_type_id);
                        gridView.Columns[15].DisplayIndex = 3;
                        gridView.Columns[15].Width = 100;
                        gridView.Columns[16].Visible = _data.returnStatusColumnID(16, m_doc_type_id);
                        gridView.Columns[16].DisplayIndex = 4;
                        gridView.Columns[16].Width = 100;
                        gridView.Columns[5].Visible = _data.returnStatusColumnID(5, m_doc_type_id);
                        gridView.Columns[5].DisplayIndex = 5;
                        gridView.Columns[5].Width = 100;
                        gridView.Columns[6].Visible = true;
                        gridView.Columns[6].DisplayIndex = 6;
                        gridView.Columns[6].Width = 150;
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        if (CParam.AppCity == 2)
                        {
                            gridView.Columns[7].HeaderText = " Кол-во \n в баз. ед";
                        }
                        else
                        {
                            gridView.Columns[16].HeaderText = "Мин. заказ";
                            gridView.Columns[5].HeaderText = "Ед.измер";
                        }
                        break;

                    case 3: // остатки
                    case 10: // 10-ные остатки
                    case 14: // остатки инвентаря
                        gridView.Columns[1].Width = 500;
                        gridView.Columns[2].HeaderText = "Остатки";
                        gridView.Columns[2].Width = 150;
                        gridView.Columns[5].Visible = _data.returnStatusColumnID(5, m_doc_type_id);
                        gridView.Columns[5].DisplayIndex = 7;
                        gridView.Columns[5].Width = 100;
                        gridView.Columns[15].Visible = _data.returnStatusColumnID(15, m_doc_type_id);
                        gridView.Columns[15].DisplayIndex = 4;
                        gridView.Columns[15].Width = 100;
                        gridView.Columns[16].Visible = _data.returnStatusColumnID(16, m_doc_type_id);
                        gridView.Columns[16].DisplayIndex = 6;
                        gridView.Columns[16].Width = 100;

                        gridView.Columns[12].HeaderText = "Вылет";
                        gridView.Columns[12].Width = 150;
                        gridView.Columns[12].Visible = true;
                        if (CParam.AppCity == 1)
                        {
                            gridView.Columns[5].HeaderText = "Ед.измер";
                        }
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;

                    case 16: // перемещение
                        gridView.Columns[1].Width = 332;
                        gridView.Columns[5].Visible = _data.returnStatusColumnID(5, m_doc_type_id);
                        gridView.Columns[5].DisplayIndex = 7;
                        gridView.Columns[5].Width = 150;
                        gridView.Columns[15].Visible = _data.returnStatusColumnID(15, m_doc_type_id);
                        gridView.Columns[15].DisplayIndex = 4;
                        gridView.Columns[15].Width = 100;
                        gridView.Columns[16].Visible = _data.returnStatusColumnID(16, m_doc_type_id);
                        gridView.Columns[16].DisplayIndex = 6;
                        gridView.Columns[16].Width = 100;
                        gridView.Columns[2].HeaderText = "Кол-во";
                        gridView.Columns[2].Width = 100;
                        gridView.Columns[9].Visible = true;
                        gridView.Columns[9].Width = 250;
                        gridView.Columns[11].Visible = true;
                        gridView.Columns[11].Width = 250;
                        toolStripSpisok.Visible = true;
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;
                #endregion
                    #region good
                    case 6:
                    case 9:
                    case 13: // списание
                        toolStripSpisok.Visible = true;
                        gridView.Columns[1].Width = 500;
                        gridView.Columns[2].DefaultCellStyle.Format = "N2";
                        gridView.Columns[5].Visible = _data.returnStatusColumnID(5, m_doc_type_id);
                        gridView.Columns[5].DisplayIndex = 7;
                        gridView.Columns[5].Width = 150;
                        gridView.Columns[15].Visible = _data.returnStatusColumnID(15, m_doc_type_id);
                        gridView.Columns[15].DisplayIndex = 4;
                        gridView.Columns[15].Width = 100;
                        gridView.Columns[16].Visible = _data.returnStatusColumnID(16, m_doc_type_id);
                        gridView.Columns[16].DisplayIndex = 6;
                        gridView.Columns[16].Width = 100;
                        gridView.Columns[2].HeaderText = "К списанию";
                        gridView.Columns[2].Width = 150;

                        gridView.Columns[21].Visible = true;
                        gridView.Columns[21].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                        DataGridCombo._simpleCombo = null;
                        DataGridCombo._UtilRef = null;
#region trash
                        //DataGridViewComboBoxColumn spis_column = new DataGridViewComboBoxColumn()
                        //{
                        //    DataPropertyName = "urr_list",
                        //    HeaderText = "Списания",
                        //    Visible = true,
                        //    ReadOnly = false,
                        //    Width = 430
                        //};

                        //DataGridViewTextBoxColumn spis_column = new DataGridViewTextBoxColumn()
                        //{
                        //    DataPropertyName = "urr_name",
                        //    HeaderText = "Списания",
                        //    Visible = true,
                        //    ReadOnly = false,
                        //    Width = 100,
                        //    DisplayIndex = 6
                        //};

                        //spis_column.Items.AddRange(new string[] { "11", "22" });

                        // gridView.Columns.Add(spis_column);
#endregion
                        if (CParam.AppCity == 1)
                        {
                            gridView.Columns[5].HeaderText = "Ед.измер";
                        }

                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;

                    
                    case 15: // входящая накладная   
                        gridView.Columns[5].Visible = _data.returnStatusColumnID(5, m_doc_type_id);
                        gridView.Columns[5].DisplayIndex = 7;
                        gridView.Columns[5].Width = 100;
                        gridView.Columns[15].Visible = _data.returnStatusColumnID(15, m_doc_type_id);
                        gridView.Columns[15].DisplayIndex = 4;
                        gridView.Columns[15].Width = 100;
                        gridView.Columns[16].Visible = _data.returnStatusColumnID(16, m_doc_type_id);
                        gridView.Columns[16].DisplayIndex = 6;
                        gridView.Columns[16].Width = 100;
                        toolStripSpisok.Visible = true;
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;

                    case 17:// заказ хоз средств
                    case 18:
                        gridView.Columns[3].Visible = true;
                        gridView.Columns[4].Visible = true;
                        gridView.Columns[5].Visible = _data.returnStatusColumnID(5, m_doc_type_id);
                        gridView.Columns[5].DisplayIndex = 7;
                        gridView.Columns[5].Width = 100;
                        gridView.Columns[15].Visible = _data.returnStatusColumnID(15, m_doc_type_id);
                        gridView.Columns[15].DisplayIndex = 4;
                        gridView.Columns[15].Width = 100;
                        gridView.Columns[16].Visible = _data.returnStatusColumnID(16, m_doc_type_id);
                        gridView.Columns[16].DisplayIndex = 6;
                        gridView.Columns[16].Width = 100;
                        gridView.Columns[6].Visible = true;
                        gridView.Columns[13].Visible = true;
                        gridView.Columns[13].HeaderText = "Норма";
                        gridView.Columns[13].DefaultCellStyle.Format = "N2";
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;
#endregion

                    case 19: // инкасация  
                        gridView.AllowUserToAddRows = true;



                        if (m_teremok_dep == 2)
                        {

                            gridView.Columns[2].Visible = true;
                            gridView.Columns[5].Visible = true;
                            gridView.Columns[7].Visible = true;
                            gridView.Columns[12].Visible = true;
                            gridView.Columns[2].HeaderText = "Отдано Инкассатору";
                            gridView.Columns[5].HeaderText = "Валюта";
                            gridView.Columns[7].HeaderText = "Снятие";
                            gridView.Columns[12].HeaderText = "Выручка";
                            expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        }
                        else
                        {
                            if (CParam.AppCity == 1)
                            {
                                gridView.Columns[11].Visible = true;
                                gridView.Columns[11].Width = 200;
                                gridView.Columns[11].HeaderText = "Номер баула";

                                gridView.Columns[20].Visible = true;
                                gridView.Columns[20].Width = 75;
                            }
                            gridView.Columns[1].Width = 300;
                            gridView.Columns[2].Visible = true;
                            gridView.Columns[2].HeaderText = "Отдано Инкассатору";
                            gridView.Columns[2].Width = 150;
                            gridView.Columns[4].Visible = true;
                            gridView.Columns[5].HeaderText = "Валюта";
                            gridView.Columns[4].Width = 150;
                            gridView.Columns[5].Visible = true;
                            gridView.Columns[5].Width = 150;
                            //gridView.Columns[6].Visible = true;
                            //gridView.Columns[6].Width = 150;
                            expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        }

                        break;
                    #region good
                    case 21: // Перемещение                   
                        gridView.Columns[3].Visible = true;
                        gridView.Columns[3].DisplayIndex = 3;
                        gridView.Columns[4].Visible = true;
                        gridView.Columns[4].DisplayIndex = 4;
                        gridView.Columns[4].Width = 100;
                        //gridView.Columns[5].Visible = _data.returnStatusColumnID(5, m_doc_type_id);
                        //gridView.Columns[5].DisplayIndex = 5;
                        //gridView.Columns[5].Width = 100;
                        gridView.Columns[15].Visible = _data.returnStatusColumnID(15, m_doc_type_id);
                        gridView.Columns[15].DisplayIndex = 5;
                        gridView.Columns[15].Width = 100;
                        gridView.Columns[16].Visible = _data.returnStatusColumnID(16, m_doc_type_id);
                        gridView.Columns[16].DisplayIndex = 6;
                        gridView.Columns[16].Width = 100;
                        gridView.Columns[6].Visible = true;
                        gridView.Columns[6].DisplayIndex = 7;
                        //gridView.Columns[7].Visible = true;
                        //gridView.Columns[7].DisplayIndex = 5;

                        if (CParam.AppCity == 2)
                            gridView.Columns[7].HeaderText = " Кол-во \n в баз. ед";
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;

                    case 23:
                        gridView.Columns[2].DefaultCellStyle.Format = "N0";
                        gridView.Columns[4].DefaultCellStyle.Format = "N0";
                        gridView.Columns[4].Visible = true;
                        gridView.Columns[2].Width = 150;
                        gridView.Columns[3].Width = 150;
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;

                    case 24:
                        gridView.Columns[2].DefaultCellStyle.Format = "N0";
                        gridView.Columns[4].DefaultCellStyle.Format = "N0";
                        gridView.Columns[4].Visible = true;
                        gridView.Columns[2].Width = 150;
                        gridView.Columns[5].Width = 150;
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;

                    case 26:
                        gridView.Columns[2].DefaultCellStyle.Format = "N0";
                        gridView.Columns[4].DefaultCellStyle.Format = "N0";
                        gridView.Columns[4].Visible = true;
                        gridView.Columns[2].Width = 150;
                        gridView.Columns[3].Width = 150;
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;

                    case 27: // остатки утро
                        gridView.Columns[2].Width = 150;
                        gridView.Columns[5].Visible = _data.returnStatusColumnID(5, m_doc_type_id);
                        gridView.Columns[5].DisplayIndex = 7;
                        gridView.Columns[5].Width = 100;
                        gridView.Columns[15].Visible = _data.returnStatusColumnID(15, m_doc_type_id);
                        gridView.Columns[15].DisplayIndex = 4;
                        gridView.Columns[15].Width = 100;
                        gridView.Columns[16].Visible = _data.returnStatusColumnID(16, m_doc_type_id);
                        gridView.Columns[16].DisplayIndex = 6;
                        gridView.Columns[16].Width = 100;
                        gridView.Columns[12].HeaderText = "Вылет";
                        gridView.Columns[12].Width = 150;
                        gridView.Columns[12].Visible = true;
                        expandablePanel1.Expanded = _data.returnStatusColumnID(100, m_doc_type_id);
                        break;
                    #endregion
                }

                //gridView.CellBeginEdit -= gridView_CellBeginEdit;
                //gridView.CellEndEdit -= gridView_CellEndEdit;
                //gridView.CellEnter -= gridView_CellEnter;
                //gridView.CellLeave -= gridView_CellLeave;
                //gridView.CellValidating -= gridView_CellValidating;
                //gridView.DataBindingComplete -= gridView_DataBindingComplete;
                //gridView.DataError -= gridView_DataError;
                //gridView.KeyPress -= gridView_KeyPress;
                //gridView.KeyUp -= gridView_KeyUp;
                //gridView.DataBindingComplete -= gridView_DataBindingComplete;
                //gridView.DataError -= gridView_DataError;


                // связать с данными
                dt = _data.OrderDetailsView(m_doc_id, m_doc_type_id);


                //Fill_Spisanie(dt);
                //fill_selects();

                gridView.DataSource = dt;

#if(ELICK)
                if (m_doc_type_id == 24)
                {
                    DataTable dt_status=SqlWorker.SelectFromDBSafe("SELECT t_DocStatusRef.statustype_id, doc_id "+
                                                "FROM t_DocStatusRef INNER JOIN t_doc ON t_DocStatusRef.docstatusref_id = t_doc.doc_status_id "+
                                                "WHERE (((t_doc.[doc_id])="+m_doc_id+"));","t_doc_status");
                    if (CellHelper.FindCell(dt_status, 0, "statustype_id").ToString() != "1") return;


                    //загрузить справочник счетчика эл.
                    dt_sprav = SqlWorker.SelectFromDBSafe("SELECT TOP 1 * from t_counts_service where doc_type_id=" + m_doc_type_id, "t_count_service");
                    if (dt_sprav.Rows.Count == 0) return;

                    gridView.Columns[2].ReadOnly = true;
                    gridView.Columns[2].DefaultCellStyle.Format = "N4";

                    gridView.CellClick += new DataGridViewCellEventHandler(delegate(object a, DataGridViewCellEventArgs b)
                    {
                        Open_masked_elick_window(b);
                        gridView_CellEndEdit(a, b);
                    });

                    //gv.Columns[3].DefaultCellStyle.Format = "#,###"
                    DataGridViewCell cell = (DataGridViewCell)CellHelper.FindCell(gridView, 0, 4);
                    cell.Value=Decimal.Parse(CellHelper.FindCell(dt_sprav, 0, "count_koeff").ToString());
                    
                }
#endif
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {

            }
        }

        DataTable dt = null;
        DataTable dt_sprav = null;
        private void fill_selects()
        {
            try
            {
                for (int i = 0; i < gridView.RowCount; i++)
                {
                    DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)CellHelper.FindCell(gridView, i, "urr_list");
                    cell.DataSource = new string[] { "12", "323", "dsds", "asd" };
                    cell.Value = "12";
                }
            }
            catch (Exception e)
            {

            }
        }

        


        #region trash
        //private void Fill_Spisanie_Cells<T>(T gridView, DataTable dt) where T : DataGridView
        //{
        //    foreach (DataGridViewRow row in gridView.Rows)
        //    {
        //        DataGridViewCell cell = (DataGridViewCell)CellHelper.FindCell(gridView, row.Index, "urr_id");


        //        string flag = "";
        //        if (cell.Value == null || cell.Value.ToString() == "")
        //        {
        //            cell.Value = DataGridCombo.Create().Combo;
        //            flag = "null";
        //        }
        //        else
        //        {
        //            flag = "not null";
        //            cell.Value = DataGridCombo.Create(cell.Value.ToString()).Combo;
        //        }
        //    }
        //}
        #endregion

        private bool Check_Elick_Value_Cell(object sender, DataGridViewCellEventArgs e)
        {
            //проверить есть ли шаблон в базе

            DataTable dt = SqlWorker.SelectFromDBSafe("SELECT t_Counts_Service.count_length FROM t_Counts_Service WHERE (((t_Counts_Service.doc_type_id)=" + m_doc_type_id + "));", "t_Counts_Service");

            if (0 != dt.Rows.Count) //если есть то сделать проверку ввода числа
            {
                //получить значение из шаблона
                object shabl_value = CellHelper.FindCell(dt, 0, "count_length");
                int shabl_value_int = int.Parse(shabl_value.ToString());

                //получить значение в ячейке
                DataGridViewTextBoxCell cell = CellHelper.FindCell(gridView, e.RowIndex, e.ColumnIndex) as DataGridViewTextBoxCell;
                

                //проверить значение
                int cell_val_length = ((int)((decimal)cell.Value)).ToString().Length;
                if (cell_val_length != shabl_value_int)
                {
                    MessageBox.Show("Значение счетчика должно именть " + shabl_value_int + " разрядов");
                    return false;
                }
            }

             return true;
        }

        private void gridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            #region baul
#if(BAUL)

            if (m_doc_type_id == 19)
            {
                //проверить это не 0я строка
                if (e.RowIndex != 0)
                {
                    //проверить на ненулевое введенное значение
                    string value = ((DataGridViewTextBoxCell)CellHelper.FindCell(gridView,e.RowIndex,e.ColumnIndex)).Value.ToString();
                    int val=-1;
                    int.TryParse(value,out val);
                    if (val > 0)
                    {
                        DataGridViewRow row0 = gridView.Rows[0];
                        DataGridViewRow row_this = gridView.Rows[e.RowIndex];



                        List<DataGridViewCell> cells_this=row_this.Cells.OfType<DataGridViewCell>().ToList();
                        cells_this.ForEach(a=>{
                            DataGridViewCell cell0 = row0.Cells[a.ColumnIndex];
                            if (a.ReadOnly == true && a.Value is System.DBNull && row0.Cells[a.ColumnIndex].Value!=null)
                            {
                                a.Value = row0.Cells[a.ColumnIndex].Value;
                            }
                        });
                        
                        //если нет то скопировать в новую строку все залоченные поля
                    }
                }
                

                //if (!Check_Elick_Value_Cell(sender, e))  //обработать счетчик электричества
                //{
                //    DataGridViewTextBoxCell cell = gridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewTextBoxCell;
                //    if (cell != null)
                //    {
                //        gridView.CurrentCell = cell;
                //        gridView.BeginEdit(true);
                //    }
                //}
            }

            #endif
            #endregion

            #region elick
#if(ELICK)

            if (m_doc_type_id == 24)
            {
                //if (!Check_Elick_Value_Cell(sender, e))  //обработать счетчик электричества
                //{
                //    DataGridViewTextBoxCell cell = gridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewTextBoxCell;
                //    if (cell != null)
                //    {
                //        gridView.CurrentCell = cell;
                //        gridView.BeginEdit(true);
                //    }
                //}
            }

            #endif
            #endregion

#if(SPISAN)
            if (m_doc_type_id == 9 || m_doc_type_id == 13)
            {
                try
                {
                    if (e.ColumnIndex == 2)
                    {
                        //сформировать список списаний
                        //List<string> str = new List<string>() { "11", "22", "33", "44" };

                        List<string> str = DataGridCombo.CreateStringArray(dt.Rows[e.RowIndex]).ToList<string>();

                        //вывести окно диалога
                        //если ок то получить в тексте причину списания
                        FormListBoxShow<string> listForm = new FormListBoxShow<string>(str);


                        DataGridViewTextBoxCell cell = gridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewTextBoxCell;
                        DataGridViewTextBoxCell cell_name = (DataGridViewTextBoxCell)CellHelper.FindCell(gridView, e.RowIndex, "urr_name");
                        if (listForm.ShowDialog() == DialogResult.OK)
                        {
                            if ((decimal)cell.Value > 0)
                            {
                                string item = listForm.outItem;
                                cell_name.Value = item;
                            }
                            else
                            {
                                cell_name.Value = "";
                            }
                        }
                        else
                        {

                            if (cell != null)
                            {

                                cell.Value = 0;
                                cell_name.Value = "";

                                //gridView.CurrentCell = cell;
                                //gridView.BeginEdit(true);
                            }
                        }
                    }
                }catch(Exception ex)
                {
                    MDIParentMain.Log(ex, "Не удалось открыть справочник списаний");
                }

                //обновить ячейку
            }


            if ((m_doc_type_id == 9 || m_doc_type_id == 13) && e.ColumnIndex==2)
            {
                UpdateData(e);
            }
            else
            {
                UpdateData();
            }
#else
            UpdateData();
#endif


            //добавить графу списаний

            #region good
            double _val = 0;
            double _maxquota = 0;
            double _quota = 0;
            string _be = "";
            string _ed = "";
            string _nome_1C = "";
            string _nome_name = "";
            string[] time;

            _val = gridView.CurrentRow.Cells[2].Value is System.DBNull?0:Convert.ToDouble(gridView.CurrentRow.Cells[2].Value);
            _ed = gridView[3, gridView.CurrentCell.RowIndex].Value.ToString();
            _be = gridView[5, gridView.CurrentCell.RowIndex].Value.ToString();
            _maxquota =gridView.CurrentRow.Cells[13].Value is System.DBNull?0:Convert.ToDouble(gridView.CurrentRow.Cells[13].Value);
            _quota = gridView.CurrentRow.Cells[8].Value is System.DBNull?0:Convert.ToDouble(gridView.CurrentRow.Cells[8].Value);
            _nome_1C = Convert.ToString(gridView.CurrentRow.Cells[19].Value);
            _nome_name = Convert.ToString(gridView.CurrentRow.Cells[1].Value);
            gridView[20, gridView.CurrentCell.RowIndex].Value = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 1);
            time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 1).Split(':');
            if (time.Length == 1)
            {
                gridView[20, gridView.CurrentCell.RowIndex].Value = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 0);
                time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 0).Split(':');
            }

            if (time.Length == 1)
            {
                gridView[20, gridView.CurrentCell.RowIndex].Value = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 2);
                time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 2).Split(':');
            }

            try
            {

                if (Convert.ToInt32(time[0]) > 23)
                {
                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show("Введен неверный формат времени", "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        gridView[20, gridView.CurrentCell.RowIndex].Value = "";
                    }
                }
                if (Convert.ToInt32(time[1]) > 59)
                {
                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show("Введен неверный формат времени", "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        gridView[20, gridView.CurrentCell.RowIndex].Value = "";
                    }
                }
                if (m_doc_ban == 2)
                {
                    // максимум квоты 
                    if (gridView.CurrentRow.Cells[13].Value != DBNull.Value)
                    {
                        if (_val != 0)
                        {
                            if (_maxquota != 0)
                            {
                                if (_val > _maxquota)
                                {
                                    if (m_doc_type_id == 1)
                                    {
                                        using (new CenterWinDialog(this))
                                        {
                                            MessageBox.Show("Значение не может быть больше " + _maxquota + " " + _ed, "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                    else
                                    {
                                        using (new CenterWinDialog(this))
                                        {
                                            MessageBox.Show("Значение не может быть больше " + _maxquota + " " + _ed, "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //минимум квоты
                    if (gridView.CurrentRow.Cells[8].Value != DBNull.Value)
                    {
                        if (_val != 0)
                        {
                            if (_quota != 0)
                            {
                                if (_val < _quota)
                                {
                                    if (m_doc_type_id == 1)
                                    {
                                        using (new CenterWinDialog(this))
                                        {
                                            MessageBox.Show("Значение не может быть меньше " + _quota + " " + _ed, "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                    else
                                    {
                                        using (new CenterWinDialog(this))
                                        {
                                            MessageBox.Show("Значение не может быть меньше " + _quota + " " + _be, "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                if (gridView.CurrentCell.ColumnIndex == 20)
                {
                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show("Введен неверный формат времени", "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    gridView[20, gridView.CurrentCell.RowIndex].Value = "";
                }
            }
            #endregion
        }



        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2) // presuming "car" in first column
            { // presuming "ColorsAvailable" in second column
                var cbCell = gridView.Rows[e.RowIndex].Cells[21] as DataGridViewComboBoxCell;
                string[] colors = { "white", "black" };
                //switch (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString())
                //{
                //    case "Pilot": colors = new string[] { "silver", "sage" }; break;
                //    // case "other": add other colors
                //}
                cbCell.DataSource = colors;
            }
        }

        private void setCellComboBoxItems(DataGridViewX dataGrid, int rowIndex, int colIndex, object[] itemsToAdd)
        {
            DataGridViewComboBoxCell dgvcbc = (DataGridViewComboBoxCell)dataGrid.Rows[rowIndex].Cells[colIndex];
            // You might pass a boolean to determine whether to clear or not.
            dgvcbc.Items.Clear();
            foreach (object itemToAdd in itemsToAdd)
            {
                dgvcbc.Items.Add(itemToAdd);
            }
        }

        private void columnVisible()
        {

        }

        private void UpdateData()
        {
            // пишем в базу
            CBData _data = new CBData();
            double _q;
            double _q2;
            double _q3;
            double _k;
            double _t1;
            string _reason;
            double _ke;
            string _time = "";
            string[] time;
            try
            {
                //для корректного выхода если значение затерли бекспейсом.

#if(BAUL)
                if (gridView.CurrentRow.Cells[2].Value == DBNull.Value || gridView.CurrentRow.Cells[7].Value == DBNull.Value || gridView.CurrentRow.Cells[12].Value == DBNull.Value)
                {
                    if (m_doc_type_id == 19)
                    {
                        _q = gridView.CurrentRow.Cells[2].Value == DBNull.Value ? Convert.ToDouble(gridView[2, gridView.CurrentCell.RowIndex].Value = 0) : Convert.ToDouble(gridView[2, gridView.CurrentCell.RowIndex].Value);
                        _q2 = gridView.CurrentRow.Cells[7].Value == DBNull.Value ? Convert.ToDouble(gridView[7, gridView.CurrentCell.RowIndex].Value = 0) : Convert.ToDouble(gridView[7, gridView.CurrentCell.RowIndex].Value);
                        _q3 = gridView.CurrentRow.Cells[12].Value == DBNull.Value ? Convert.ToDouble(gridView[12, gridView.CurrentCell.RowIndex].Value = 0) : Convert.ToDouble(gridView[12, gridView.CurrentCell.RowIndex].Value);


                    }
                    else
                    {
                        _q = Convert.ToDouble(gridView[2, gridView.CurrentCell.RowIndex].Value = 0);
                        _q2 = Convert.ToDouble(gridView[7, gridView.CurrentCell.RowIndex].Value = 0);
                        _q3 = Convert.ToDouble(gridView[12, gridView.CurrentCell.RowIndex].Value = 0);
                    }

                }
#else
                if (gridView.CurrentRow.Cells[2].Value == DBNull.Value || gridView.CurrentRow.Cells[7].Value == DBNull.Value || gridView.CurrentRow.Cells[12].Value == DBNull.Value)
                {
                    _q = Convert.ToDouble(gridView[2, gridView.CurrentCell.RowIndex].Value = 0);
                    _q2 = Convert.ToDouble(gridView[7, gridView.CurrentCell.RowIndex].Value = 0);
                    _q3 = Convert.ToDouble(gridView[12, gridView.CurrentCell.RowIndex].Value = 0);
                }
#endif

                //Получим данные.
                _q = Convert.ToDouble(gridView[2, gridView.CurrentCell.RowIndex].Value.ToString());
                _q2 = Convert.ToDouble(gridView[7, gridView.CurrentCell.RowIndex].Value.ToString());
                _q3 = Convert.ToDouble(gridView[12, gridView.CurrentCell.RowIndex].Value.ToString());
                _k = Convert.ToDouble(gridView[4, gridView.CurrentCell.RowIndex].Value);
                _ke = Convert.ToDouble(gridView[14, gridView.CurrentCell.RowIndex].Value);
                _reason = gridView[11, gridView.CurrentCell.RowIndex].Value.ToString();
                //time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 1);
                // time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 0);

                gridView[20, gridView.CurrentCell.RowIndex].Value = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 1);
                time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 1).Split(':');

                if (time.Length == 1)
                {
                    gridView[20, gridView.CurrentCell.RowIndex].Value = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 0);
                    time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 0).Split(':');
                }

                if (time.Length == 1)
                {
                    gridView[20, gridView.CurrentCell.RowIndex].Value = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 2);
                    time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 2).Split(':');
                }
                if (time.Length != 1)
                {
                    _time = time[0] + ":" + time[1];
                }

                if (CParam.AppCity == 1)
                {
                    if (m_doc_type_id == 2)
                    {
                        gridView[6, gridView.CurrentCell.RowIndex].Value = _q;
                    }
                    else
                    {
                        gridView[6, gridView.CurrentCell.RowIndex].Value = _q * _k * _ke;
                    }
                }
                if (CParam.AppCity == 2)
                {
                    gridView[6, gridView.CurrentCell.RowIndex].Value = _q * _k * _ke;
                }

                if (m_doc_type_id == 21)
                {
                    gridView[6, gridView.CurrentCell.RowIndex].Value = _q * _k;
                }

                if (m_doc_type_id == 1 || m_doc_type_id == 21 || m_doc_type_id == 2 || m_doc_type_id == 29)
                {
                    _t1 = Convert.ToDouble(gridView[17, gridView.CurrentCell.RowIndex].Value = _q * _k * _ke);
                }
                else
                {
                    _t1 = Convert.ToDouble(gridView[17, gridView.CurrentCell.RowIndex].Value = _q);
                }

                // внести изменения в базу  
                //_data.Order2ProdUpdateItem(gridView.CurrentRow.Cells[0].Value.ToString(), _q.ToString(), _q2.ToString(), _reason, _q3.ToString(), _t1.ToString());

#if(BAUL)
                if (m_doc_type_id == 19)
                {
                    string line_num = gridView.CurrentRow.Cells[0].Value.ToString();

                    if (line_num == "")
                    {
                        //m_teremok_id//m_doc_type_id//m_doc_id
                        int ord_doc_id = _data.Order2ProdAddItem(m_doc_type_id, m_teremok_id, m_doc_id);
                        _data.Order2ProdUpdateItem(ord_doc_id.ToString(), _q.ToString(), _q2.ToString(), _reason, _q3.ToString(), _t1.ToString(), _time.ToString());
                        gridView.CurrentRow.Cells[0].Value = ord_doc_id;
                    }
                    else
                    {
                        if (_q > 0)
                        {
                            _data.Order2ProdUpdateItem(gridView.CurrentRow.Cells[0].Value.ToString(), _q.ToString(), _q2.ToString(), _reason, _q3.ToString(), _t1.ToString(), _time.ToString());
                        }
                        else
                        {
                            //удалить строку
                            if (gridView.CurrentRow.Index != 0)
                            {
                                _data.Order2ProdDeleteItem(line_num);
                                gridView.Rows.Remove(gridView.CurrentRow);
                            }
                        }
                    }

                }
                else
                {
                    _data.Order2ProdUpdateItem(gridView.CurrentRow.Cells[0].Value.ToString(), _q.ToString(), _q2.ToString(), _reason, _q3.ToString(), _t1.ToString(), _time.ToString());
                }
#else
                _data.Order2ProdUpdateItem(gridView.CurrentRow.Cells[0].Value.ToString(), _q.ToString(), _q2.ToString(), _reason, _q3.ToString(), _t1.ToString(), _time.ToString());
#endif


                // внести изменения в базу по списаниям
                //if (m_doc_type_id == 9 || m_doc_type_id == 13)
                //{
                //    Utilization util_line=new Utilization(
                //        CellHelper.FindCell(
                //    string query= QueryHelper.MakeInsertQuery("t_Utilization",
                //    SqlWorker.ExecuteQuery(query);
                //}

                // цветовое оформление шрифта
                gridView.CurrentRow.Cells[2].Style.ForeColor = Color.Black;
                gridView.CurrentRow.Cells[6].Style.ForeColor = Color.Black;
                gridView.CurrentRow.Cells[7].Style.ForeColor = Color.Black;
                gridView.CurrentRow.Cells[12].Style.ForeColor = Color.Black;
                gridView.CurrentRow.Cells[14].Style.ForeColor = Color.Black;
                gridView.CurrentRow.Cells[15].Style.ForeColor = Color.Black;

                if (m_doc_type_id == 16)
                {
                    if (gridView.CurrentCell.ColumnIndex == 2)
                    {
                        if (_q != 0)
                            OpenReasonDialog();
                        else
                            ReasonDelete();
                    }
                }
            }
            catch (Exception _exp)
            {
                MessageBox.Show(_exp.Message);
            }
        }


        private void UpdateData(DataGridViewCellEventArgs e)
        {
            // пишем в базу
            CBData _data = new CBData();
            double _q;
            double _q2;
            double _q3;
            double _k;
            double _t1;
            string _reason;
            double _ke;
            string _time = "";
            string[] time;
            try
            {
                //для корректного выхода если значение затерли бекспейсом.
                if (gridView.CurrentRow.Cells[2].Value == DBNull.Value || gridView.CurrentRow.Cells[7].Value == DBNull.Value || gridView.CurrentRow.Cells[12].Value == DBNull.Value)
                {
                    _q = Convert.ToDouble(gridView[2, gridView.CurrentCell.RowIndex].Value = 0);
                    _q2 = Convert.ToDouble(gridView[7, gridView.CurrentCell.RowIndex].Value = 0);
                    _q3 = Convert.ToDouble(gridView[12, gridView.CurrentCell.RowIndex].Value = 0);
                }
                //Получим данные.
                _q = Convert.ToDouble(gridView[2, gridView.CurrentCell.RowIndex].Value.ToString());
                _q2 = Convert.ToDouble(gridView[7, gridView.CurrentCell.RowIndex].Value.ToString());
                _q3 = Convert.ToDouble(gridView[12, gridView.CurrentCell.RowIndex].Value.ToString());
                _k = Convert.ToDouble(gridView[4, gridView.CurrentCell.RowIndex].Value);
                _ke = Convert.ToDouble(gridView[14, gridView.CurrentCell.RowIndex].Value);
                _reason = gridView[11, gridView.CurrentCell.RowIndex].Value.ToString();
                //time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 1);
                // time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 0);

                gridView[20, gridView.CurrentCell.RowIndex].Value = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 1);
                time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 1).Split(':');

                if (time.Length == 1)
                {
                    gridView[20, gridView.CurrentCell.RowIndex].Value = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 0);
                    time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 0).Split(':');
                }

                if (time.Length == 1)
                {
                    gridView[20, gridView.CurrentCell.RowIndex].Value = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 2);
                    time = TimeHelper.ParceAmount(gridView[20, gridView.CurrentCell.RowIndex].Value.ToString(), 2).Split(':');
                }
                if (time.Length != 1)
                {
                    _time = time[0] + ":" + time[1];
                }

                if (CParam.AppCity == 1)
                {
                    if (m_doc_type_id == 2)
                    {
                        gridView[6, gridView.CurrentCell.RowIndex].Value = _q;
                    }
                    else
                    {
                        gridView[6, gridView.CurrentCell.RowIndex].Value = _q * _k * _ke;
                    }
                }
                if (CParam.AppCity == 2)
                {
                    gridView[6, gridView.CurrentCell.RowIndex].Value = _q * _k * _ke;
                }

                if (m_doc_type_id == 21)
                {
                    gridView[6, gridView.CurrentCell.RowIndex].Value = _q * _k;
                }

                if (m_doc_type_id == 1 || m_doc_type_id == 21 || m_doc_type_id == 2)
                {
                    _t1 = Convert.ToDouble(gridView[17, gridView.CurrentCell.RowIndex].Value = _q * _k * _ke);
                }
                else
                {
                    _t1 = Convert.ToDouble(gridView[17, gridView.CurrentCell.RowIndex].Value = _q);
                }

                // внести изменения в базу  
                //_data.Order2ProdUpdateItem(gridView.CurrentRow.Cells[0].Value.ToString(), _q.ToString(), _q2.ToString(), _reason, _q3.ToString(), _t1.ToString());
                _data.Order2ProdUpdateItem(gridView.CurrentRow.Cells[0].Value.ToString(), _q.ToString(), _q2.ToString(), _reason, _q3.ToString(), _t1.ToString(), _time.ToString());


                // внести изменения в базу по списаниям
                if (m_doc_type_id == 9 || m_doc_type_id == 13)
                {
                    string urr_name=((DataGridViewTextBoxCell)CellHelper.FindCell(gridView, e.RowIndex, "urr_name"))
                        .Value.ToString();

                    if (urr_name != "")
                    {
                        DataRow row = CellHelper.FindRow(DataGridCombo.UtilRef, "urr_name", urr_name);
                        string urr_id = CellHelper.FindCell(row, "urr_id").ToString();
                        if (urr_id != "")
                        {
                            if (!StaticConstants.UpdateHelper.IsNewerVersion("5.6.4.0"))
                            {
                                #region new utilization
                                t_Utilization utilization = new t_Utilization().SelectFirst<t_Utilization>("util_opd_id=" + CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_id").ToString());

                                if (utilization == null)
                                {
                                    utilization = new t_Utilization();

                                    utilization.util_nome_id = (int)CellHelper.FindCell(dt.Rows[e.RowIndex], "nome_id");
                                    utilization.util_quantity = (int)(decimal)CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_order");
                                    utilization.util_doc_id = (int)CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_doc_id");
                                    utilization.util_reason_id = int.Parse(urr_id);
                                    utilization.util_opd_id = (int)CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_id");
                                    utilization.util_K = (int)(decimal)CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_K");
                                    utilization.Create();
                                }
                                else
                                {
                                    utilization.util_nome_id = (int)CellHelper.FindCell(dt.Rows[e.RowIndex], "nome_id");
                                    utilization.util_quantity = (int)(decimal)CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_order");
                                    utilization.util_doc_id = (int)CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_doc_id");
                                    utilization.util_reason_id = int.Parse(urr_id);
                                    utilization.util_opd_id = (int)CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_id");
                                    utilization.util_K = (int)(decimal)CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_K");
                                    utilization.Update();
                                }
                                #endregion
                            }
                            else
                            {
                                #region old_utilization
                                Utilization util_line = new Utilization(
                                    CellHelper.FindCell(dt.Rows[e.RowIndex], "nome_id").ToString(),
                                    CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_order").ToString(),
                                    CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_doc_id").ToString(),
                                    urr_id,
                                    CellHelper.FindCell(dt.Rows[e.RowIndex], "opd_K").ToString());
                                string query = util_line.MakeInsertQuery();
                                SqlWorker.InsertOrUpdateDB(new List<Utilization>() { util_line });
                                #endregion
                            }
                        }
                    }
                }

                // цветовое оформление шрифта
                gridView.CurrentRow.Cells[2].Style.ForeColor = Color.Black;
                gridView.CurrentRow.Cells[6].Style.ForeColor = Color.Black;
                gridView.CurrentRow.Cells[7].Style.ForeColor = Color.Black;
                gridView.CurrentRow.Cells[12].Style.ForeColor = Color.Black;
                gridView.CurrentRow.Cells[14].Style.ForeColor = Color.Black;
                gridView.CurrentRow.Cells[15].Style.ForeColor = Color.Black;

                if (m_doc_type_id == 16)
                {
                    if (gridView.CurrentCell.ColumnIndex == 2)
                    {
                        if (_q != 0)
                            OpenReasonDialog();
                        else
                            ReasonDelete();
                    }
                }
            }
            catch (Exception _exp)
            {
                MessageBox.Show(_exp.Message);
            }
        }



        // котроль ввода реквизитов в табличную часть, динамический расчет показателей
        private void gridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                //m_is_cell_edit = false;
                double _val = 0;
                double _val2 = 0;
                double _val3 = 0;
                double _quota = 0;
                double _maxquota = 0;
                double _t = 0;
                int _bl = 0;
                //int _int = 0;

                foreach (DataGridViewRow _row in gridView.Rows)
                {
                    if (_row.Cells[2].Value != DBNull.Value)
                        _val = Convert.ToDouble(_row.Cells[2].Value);

                    else
                        _val = 0;

                    if (_val == 0)
                    {
                        if (m_doc_state_id == 1) // если статус = новый
                        {
                            _row.Cells[2].Style.ForeColor = Color.AliceBlue;
                            _row.Cells[6].Style.ForeColor = Color.AliceBlue;

                        }
                        else // иначае редактирование запрещено
                        {
                            _row.Cells[2].Style.ForeColor = Color.AliceBlue;
                            _row.Cells[6].Style.ForeColor = Color.AliceBlue;
                        }
                    }
                    else
                    {
                        //_int = Convert.ToInt32(_row.Cells[20].Value); 
                        //if (_int != 0)
                        //{
                        //    string[] qq = _row.Cells[2].Value.ToString().Split('.' , ',');

                        //  //  foreach (string s in qq)
                        //    {
                        //        if (qq[1] != "0000")
                        //        {
                        //            MessageBox.Show("Можно вносить только целое число!", "Внимание");
                        //            _val = Convert.ToDouble(gridView[2, gridView.CurrentCell.RowIndex].Value = 0);
                        //        }
                        //    }                            
                        //}
                        _row.Cells[2].Style.ForeColor = Color.Black;
                        _row.Cells[6].Style.ForeColor = Color.Black;
                    }

                    // доп 2.
                    if (_row.Cells[7].Value != DBNull.Value)
                        _val2 = Convert.ToDouble(_row.Cells[7].Value);
                    else
                        _val2 = 0;

                    if (_val2 == 0)
                    {
                        if (m_doc_state_id == 1)// если статус = новый
                        {
                            _row.Cells[7].Style.ForeColor = Color.AliceBlue;
                        }
                        else // редактирование запрещено
                        {
                            _row.Cells[7].Style.ForeColor = Color.AliceBlue;
                        }
                    }
                    else
                    {
                        _row.Cells[9].Style.ForeColor = Color.Black;
                        _row.Cells[6].Style.ForeColor = Color.Black;
                    }

                    // доп 3.
                    if (_row.Cells[12].Value != DBNull.Value)
                        _val3 = Convert.ToDouble(_row.Cells[12].Value);
                    else
                        _val3 = 0;

                    if (_val3 == 0)
                    {
                        if (m_doc_state_id == 1)// если статус = новый
                        {
                            _row.Cells[12].Style.ForeColor = Color.AliceBlue;
                        }
                        else // редактирование запрещено
                        {
                            _row.Cells[12].Style.ForeColor = Color.AliceBlue;
                        }
                    }
                    // Минимум квоты
                    if (_row.Cells[8].Value != DBNull.Value)
                        _quota = Convert.ToDouble(_row.Cells[8].Value);
                    {
                        if (_quota != 0)
                        {
                            if (_val != 0)
                            {
                                if (_val < _quota)
                                {
                                    m_is_close = true;
                                    _row.Cells[2].Style.BackColor = Color.Yellow;
                                    _row.Cells[8].Style.BackColor = Color.Yellow;
                                }
                            }
                            if (_val >= _quota || _val == 0)
                            {
                                // если ячейка желтого цвета
                                if (_row.Cells[2].Style.BackColor == Color.Yellow)
                                {
                                    m_is_close = false;
                                    // вернуть предыдущее состояние цвета                                        
                                    _row.Cells[2].Style.BackColor = Color.AliceBlue;
                                    _row.Cells[8].Style.BackColor = Color.AliceBlue;
                                }
                            }
                        }
                    }

                    // Максимум квоты
                    if (_row.Cells[13].Value != DBNull.Value)
                        _maxquota = Convert.ToDouble(_row.Cells[13].Value);
                    {
                        if (_maxquota != 0)
                        {
                            //if (m_doc_ban == 1)
                            {
                                if (_val != 0)
                                {
                                    if (_val > _maxquota)
                                    {
                                        m_is_close = true;
                                        _row.Cells[2].Style.BackColor = Color.Red;
                                        _row.Cells[8].Style.BackColor = Color.Red;
                                    }
                                }
                                if (_val <= _maxquota || _val == 0)
                                {
                                    // если ячейка красного цвета
                                    if (_row.Cells[2].Style.BackColor == Color.Red)
                                    {
                                        m_is_close = false;
                                        // вернуть предыдущее состояние цвета
                                        _row.Cells[2].Style.BackColor = Color.AliceBlue;
                                        _row.Cells[8].Style.BackColor = Color.AliceBlue;
                                    }
                                }
                            }
                        }
                    }
                    if (_row.Cells[6].Value != DBNull.Value)
                        _t = Convert.ToDouble(_row.Cells[6].Value);
                    {
                        {
                            if (_val2 != 0)
                            {
                                _row.Cells[7].Style.BackColor = Color.AliceBlue;
                                if (_val2 == _t)
                                {
                                    _row.Cells[7].Style.BackColor = Color.AliceBlue;
                                }
                            }
                            if (_val2 == 0)
                            {
                                _row.Cells[7].Style.BackColor = Color.AliceBlue;
                            }
                        }
                    }
                    if (_row.Cells[18].Value != DBNull.Value)
                        _bl = Convert.ToInt32(_row.Cells[18].Value);
                    {

                        if (_bl == 1)
                        {
                            _row.Cells[1].Style.Font = new Font("Tahoma", 10, FontStyle.Bold);
                        }

                    }
                }
            }
            catch (Exception _exp)
            {
                MessageBox.Show(_exp.Message);
            }
        }

        private void gridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //m_is_cell_edit = true;
            gridView.CurrentRow.Cells[2].Style.ForeColor = Color.Black;
            gridView.CurrentRow.Cells[7].Style.ForeColor = Color.Black;
            gridView.CurrentRow.Cells[6].Style.ForeColor = Color.Black;
            gridView.CurrentRow.Cells[12].Style.ForeColor = Color.Black;
            gridView.CurrentRow.Cells[14].Style.ForeColor = Color.Black;
            gridView.CurrentRow.Cells[15].Style.ForeColor = Color.Black;
        }

        private void gridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show("Ошибка ввода данных.", "Внимание");
            }
        }

        private void gridView_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void gridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

        private void InData()
        {
            CBData _rf = new CBData();
            try
            {
                dt = _rf.OrderDetailsView(m_doc_id, m_doc_type_id);
                gridView.DataSource = dt;
                // _rf.OrderDetailsView(m_doc_id, m_doc_type_id);
                //dt = gridView.DataSource;
            }

            catch (Exception _exp)
            {
                if (_exp.Message.IndexOf("SetCurrentCellAddressCore") == -1)
                {
                    MessageBox.Show(_exp.Message);
                }
                MDIParentMain.Log(_exp, "Ошибка при вводе значения в возврат");
            }
        }


        private void toolStripSpisok_Click(object sender, EventArgs e)
        {
            AddNomenclature();
        }

        private void AddNomenclature()
        {
            //Если статус отправляется или отправлен.
            if (m_doc_state_id == 56 || m_doc_state_id == 57 || m_doc_state_id == 60 || m_doc_state_id == 61)
                return;
            // запросим код ресторана и список номеклатуры.
            FormSpisok _fs = new FormSpisok();
            _fs.doc_id = m_doc_id;
            int m_nome_id = _fs.nome_id;
            _fs.ShowDialog();

            if (_fs.is_updated)
            {
                // обновить грид
                InData();
            }
        }

        private void OpenReasonDialog()
        {
            //Если статус отправляется или отправлен.
            if (m_doc_state_id == 53 || m_doc_state_id == 54)
                return;
            {
                FormOrderReason _for = new FormOrderReason();
                _for.opd_id = Convert.ToInt32(gridView.CurrentRow.Cells[0].Value);
                int orr_id = _for.orr_id;
                _for.ShowDialog();

                if (_for.is_updated)
                {
                    // обновить грид                    
                    InData();
                }
            }
        }

        private void ReasonDelete()
        {
            FormOrderReason _for = new FormOrderReason();
            _for.opd_id = Convert.ToInt32(gridView.CurrentRow.Cells[0].Value);
            _for.Delete_Comm();
            {
                // обновить грид
                InData();
            }
        }

        private void DeleteItem()
        {
            if (gridView.CurrentCell.ColumnIndex == 2)
            {
                gridView.CurrentRow.Cells[2].Value = 0;
                UpdateData();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SearchTextBox();
        }

        private void SearchTextBox()
        {
            for (; i < gridView.RowCount; i++)
                if (gridView[1, i].FormattedValue.ToString().ToLower().Contains(SearchBox.Text.Trim().ToLower()))
                {
                    gridView.CurrentCell = gridView[1, i];
                    if (i < gridView.RowCount - 1)
                        i++;
                    else
                        i = 0;
                    return;
                }
            i = 0;
        }
        private void SearchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            SearchTextBox();
        }

        private void Open_masked_elick_window(DataGridViewCellEventArgs e)
        {
            if (m_doc_type_id == 24 && e.ColumnIndex == 2)
            {
                Rectangle rect =
                   this.gridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                MaskedFormInput mi = new MaskedFormInput();
                mi.maskedTextBox1.Mask = CellHelper.FindCell(dt_sprav, 0, "count_mask").ToString();
                mi.StartPosition = FormStartPosition.CenterParent;
                if (mi.ShowDialog() == DialogResult.OK)
                {
                    DataGridViewTextBoxCell txt_bx_cell = (DataGridViewTextBoxCell)CellHelper.FindCell(gridView, 0, 2);//mi.maskedTextBox1.Text=
                    txt_bx_cell.Value = mi.maskedTextBox1.Text;

                }
                else
                {
                    return;
                }

            }
        }


        private void gridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                double _val = 0;
                double _maxquota = 0;
                double _quota = 0;
                string _be = "";
                string _ed = "";
                string _nome_1C = "";
                string _nome_name = "";

                //Open_masked_elick_window(e);

                {
                    _val = Convert.ToDouble(gridView.CurrentRow.Cells[2].Value);
                    _ed = gridView[3, gridView.CurrentCell.RowIndex].Value.ToString();
                    _be = gridView[5, gridView.CurrentCell.RowIndex].Value.ToString();
                    _maxquota = Convert.ToDouble(gridView.CurrentRow.Cells[13].Value);
                    _quota = Convert.ToDouble(gridView.CurrentRow.Cells[8].Value);
                    _nome_1C = Convert.ToString(gridView.CurrentRow.Cells[19].Value);
                    _nome_name = Convert.ToString(gridView.CurrentRow.Cells[1].Value);


                    // максимум квоты 
                    if (gridView.CurrentRow.Cells[13].Value != DBNull.Value)
                    {
                        labelX1.Text = " ";
                        if (_val != 0)
                        {
                            if (_maxquota != 0)
                            {
                                if (_val > _maxquota)
                                {
                                    labelX1.Visible = true;
                                    labelX2.Visible = false;
                                    if (m_doc_type_id == 1)
                                    {
                                        labelX1.Text = "   Значение не может быть больше " + _maxquota + " " + _ed;
                                    }
                                    else
                                    {
                                        labelX1.Text = "   Значение не может быть больше " + _maxquota + " " + _be;
                                    }
                                }
                            }
                        }
                    }
                    //минимум квоты
                    if (gridView.CurrentRow.Cells[8].Value != DBNull.Value)
                    {
                        labelX2.Text = " ";
                        if (_val != 0)
                        {
                            if (_quota != 0)
                            {
                                if (_val < _quota)
                                {
                                    labelX1.Visible = false;
                                    labelX2.Visible = true;
                                    if (m_doc_type_id == 1)
                                    {
                                        labelX2.Text = "   Значение не может быть меньше " + _quota + " " + _ed;
                                    }
                                    else
                                    {
                                        labelX2.Text = "   Значение не может быть меньше " + _quota + " " + _be;
                                    }
                                }
                            }
                        }
                    }
                    if (gridView.CurrentRow.Cells[19].Value != DBNull.Value)
                    {
                        try
                        {
                            pictureBox1.Image = Image.FromFile(CParam.AppFolder + "\\Img\\" + _nome_1C + ".bmp");
                            label_name.Text = _nome_name;
                            //label_info.Text = "Используется для свежевыжатых соков";
                            img_error = false;
                        }
                        catch (Exception)
                        {
                            img_error = true;
                        }
                        if (img_error)
                        {
                            pictureBox1.Image = Image.FromFile(CParam.AppFolder + "\\Img\\error.bmp");
                            label_name.Text = _nome_name;
                            label_info.Text = "Нет данных";
                        }
                    }
                    if (m_doc_ban == 2)
                    {
                        // максимум квоты 
                        if (gridView.CurrentRow.Cells[13].Value != DBNull.Value)
                        {
                            if (_val != 0)
                            {
                                if (_maxquota != 0)
                                {
                                    if (_val > _maxquota)
                                    {
                                        if (m_doc_type_id == 1)
                                        {
                                            using (new CenterWinDialog(this))
                                            {
                                                MessageBox.Show("Значение не может быть больше " + _maxquota + " " + _ed, "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                        }
                                        else
                                        {
                                            using (new CenterWinDialog(this))
                                            {
                                                MessageBox.Show("Значение не может быть больше " + _maxquota + " " + _ed, "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //минимум квоты
                        if (gridView.CurrentRow.Cells[8].Value != DBNull.Value)
                        {
                            if (_val != 0)
                            {
                                if (_quota != 0)
                                {
                                    if (_val < _quota)
                                    {
                                        if (m_doc_type_id == 1)
                                        {
                                            using (new CenterWinDialog(this))
                                            {
                                                MessageBox.Show("Значение не может быть меньше " + _quota + " " + _ed, "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                        }
                                        else
                                        {
                                            using (new CenterWinDialog(this))
                                            {
                                                MessageBox.Show("Значение не может быть меньше " + _quota + " " + _be, "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //if (m_doc_type_id == 24)
                //{
                //    gridView_CellEndEdit(gridView,new DataGridViewCellEventArgs(0,2));
                //}
            }
            catch (Exception _exp)
            {
               // MessageBox.Show(_exp.Message);
            }
        }

        private void FormGrid_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_is_close)
            {
                if (MessageBox.Show(" Допущено превышение по количеству товара! \n \n Уверены, что хотите отправить документ?", "Внимание!", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            FormSettingsDoc _form = new FormSettingsDoc();
            _form.column_doc_id = m_doc_type_id;
            _form.ShowDialog();
        }

        private void gridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void gridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //gridView.CurrentRow.Cells[2].Value

            if (gridView.CurrentRow.Cells[2].Value is System.DBNull) return;

            double _val = 0;
            double _maxquota = 0;
            double _quota = 0;
            string _be = "";
            string _ed = "";
            string _nome_1C = "";
            string _nome_name = "";
            string[] time;

            _val = gridView.CurrentRow.Cells[2].Value is System.DBNull ? 0 : Convert.ToDouble(gridView.CurrentRow.Cells[2].Value);
            _ed = gridView[3, gridView.CurrentCell.RowIndex].Value.ToString();
            _be = gridView[5, gridView.CurrentCell.RowIndex].Value.ToString();
            _maxquota =gridView.CurrentRow.Cells[13].Value is System.DBNull?0:Convert.ToDouble(gridView.CurrentRow.Cells[13].Value);
            _quota = gridView.CurrentRow.Cells[8].Value is System.DBNull?0:Convert.ToDouble(gridView.CurrentRow.Cells[8].Value);
            _nome_1C =gridView.CurrentRow.Cells[19].Value is System.DBNull?"":Convert.ToString(gridView.CurrentRow.Cells[19].Value);
            _nome_name = Convert.ToString(gridView.CurrentRow.Cells[1].Value);

            if (gridView.Rows[e.RowIndex].IsNewRow) { return; }
            if (gridView.CurrentRow.Cells[20].Value != null)
            {
                time = gridView.CurrentRow.Cells[20].Value.ToString().Split(':');
            }

            if (m_doc_ban == 3)
            {
                gridView.Rows[e.RowIndex].ErrorText = "";
                if (_maxquota != 0)
                {
                    if (e.ColumnIndex == gridView.Columns[2].Index)
                    {

                        if (gridView.Rows[e.RowIndex].IsNewRow) { return; }

                        if (!double.TryParse(e.FormattedValue.ToString(), out _val) || _val > _maxquota)
                        {
                            if (m_doc_type_id == 1)
                            {
                                using (new CenterWinDialog(this))
                                {
                                    MessageBox.Show("Значение не может быть больше " + _maxquota + " " + _ed, "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    gridView.Rows[e.RowIndex].ErrorText = " ";
                                }
                            }
                            else
                            {
                                using (new CenterWinDialog(this))
                                {
                                    MessageBox.Show("Значение не может быть больше " + _maxquota + " " + _be, "Внимание!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    gridView.Rows[e.RowIndex].ErrorText = " ";
                                }
                            }
                            e.Cancel = true;
                        }
                    }
                }
            }
        }
    }
}


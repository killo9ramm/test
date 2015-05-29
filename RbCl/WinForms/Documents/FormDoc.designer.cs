namespace RBClient
{
    partial class FormDoc
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataSet_Order2Prod = new System.Data.DataSet();
            this.bar1 = new DevComponents.DotNetBar.Bar();
            this.toolStripButton_Del = new DevComponents.DotNetBar.ButtonX();
            this.toolStripComboBox_Doc = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.toolStripComboBox_Teremok = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.dataGridView_Order = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.bar2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet_Order2Prod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).BeginInit();
            this.bar1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Order)).BeginInit();
            this.bar2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataSet_Order2Prod
            // 
            this.dataSet_Order2Prod.DataSetName = "NewDataSet";
            // 
            // bar1
            // 
            this.bar1.Controls.Add(this.toolStripButton_Del);
            this.bar1.Controls.Add(this.toolStripComboBox_Doc);
            this.bar1.Controls.Add(this.toolStripComboBox_Teremok);
            this.bar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.bar1.DockSide = DevComponents.DotNetBar.eDockSide.Top;
            this.bar1.Location = new System.Drawing.Point(0, 0);
            this.bar1.Name = "bar1";
            this.bar1.Size = new System.Drawing.Size(284, 25);
            this.bar1.Stretch = true;
            this.bar1.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.bar1.TabIndex = 5;
            this.bar1.TabStop = false;
            this.bar1.Text = "bar1";
            // 
            // toolStripButton_Del
            // 
            this.toolStripButton_Del.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.toolStripButton_Del.BackColor = System.Drawing.Color.Transparent;
            this.toolStripButton_Del.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.toolStripButton_Del.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStripButton_Del.Image = global::RBClient.Properties.Resources.button_cancel1;
            this.toolStripButton_Del.Location = new System.Drawing.Point(541, 1);
            this.toolStripButton_Del.Name = "toolStripButton_Del";
            this.toolStripButton_Del.Size = new System.Drawing.Size(92, 17);
            this.toolStripButton_Del.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.toolStripButton_Del.TabIndex = 2;
            this.toolStripButton_Del.Text = "Удалить";
            this.toolStripButton_Del.Click += new System.EventHandler(this.toolStripButton_Del_Click);
            // 
            // toolStripComboBox_Doc
            // 
            this.toolStripComboBox_Doc.DisplayMember = "Text";
            this.toolStripComboBox_Doc.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.toolStripComboBox_Doc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox_Doc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripComboBox_Doc.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStripComboBox_Doc.FormattingEnabled = true;
            this.toolStripComboBox_Doc.ItemHeight = 17;
            this.toolStripComboBox_Doc.Location = new System.Drawing.Point(262, 0);
            this.toolStripComboBox_Doc.Name = "toolStripComboBox_Doc";
            this.toolStripComboBox_Doc.Size = new System.Drawing.Size(273, 23);
            this.toolStripComboBox_Doc.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.toolStripComboBox_Doc.TabIndex = 12;
            this.toolStripComboBox_Doc.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_Doc_SelectedIndexChanged);
            // 
            // toolStripComboBox_Teremok
            // 
            this.toolStripComboBox_Teremok.DisplayMember = "Text";
            this.toolStripComboBox_Teremok.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.toolStripComboBox_Teremok.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox_Teremok.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.toolStripComboBox_Teremok.FocusCuesEnabled = false;
            this.toolStripComboBox_Teremok.FocusHighlightColor = System.Drawing.Color.Cyan;
            this.toolStripComboBox_Teremok.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStripComboBox_Teremok.ItemHeight = 17;
            this.toolStripComboBox_Teremok.Location = new System.Drawing.Point(8, 0);
            this.toolStripComboBox_Teremok.Name = "toolStripComboBox_Teremok";
            this.toolStripComboBox_Teremok.Size = new System.Drawing.Size(247, 23);
            this.toolStripComboBox_Teremok.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.toolStripComboBox_Teremok.TabIndex = 10;
            this.toolStripComboBox_Teremok.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_Teremok_SelectedIndexChanged);
            // 
            // dataGridView_Order
            // 
            this.dataGridView_Order.AllowUserToAddRows = false;
            this.dataGridView_Order.AllowUserToDeleteRows = false;
            this.dataGridView_Order.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.dataGridView_Order.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_Order.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_Order.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Order.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridView_Order.Location = new System.Drawing.Point(0, 25);
            this.dataGridView_Order.Name = "dataGridView_Order";
            this.dataGridView_Order.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Order.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_Order.Size = new System.Drawing.Size(284, 237);
            this.dataGridView_Order.TabIndex = 1;
            this.dataGridView_Order.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Order_CellClick);
            this.dataGridView_Order.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView_Order_DataBindingComplete);
            this.dataGridView_Order.DoubleClick += new System.EventHandler(this.dataGridView_Order_DoubleClick);
            this.dataGridView_Order.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_Order_KeyDown);
            // 
            // bar2
            // 
            this.bar2.Controls.Add(this.bar1);
            this.bar2.Dock = System.Windows.Forms.DockStyle.Top;
            this.bar2.Location = new System.Drawing.Point(0, 0);
            this.bar2.Name = "bar2";
            this.bar2.Size = new System.Drawing.Size(284, 25);
            this.bar2.TabIndex = 5;
            this.bar2.Text = "bar2";
            // 
            // FormDoc
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.dataGridView_Order);
            this.Controls.Add(this.bar2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDoc";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Документы";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDoc_FormClosing);
            this.Load += new System.EventHandler(this.FormDoc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataSet_Order2Prod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).EndInit();
            this.bar1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Order)).EndInit();
            this.bar2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Data.DataSet dataSet_Order2Prod;
        private DevComponents.DotNetBar.Bar bar1;
        private DevComponents.DotNetBar.ButtonX toolStripButton_Del;
        private DevComponents.DotNetBar.Controls.ComboBoxEx toolStripComboBox_Doc;
        private DevComponents.DotNetBar.Controls.ComboBoxEx toolStripComboBox_Teremok;
        public DevComponents.DotNetBar.Controls.DataGridViewX dataGridView_Order;
        private System.Windows.Forms.Panel bar2;
        
    }
}
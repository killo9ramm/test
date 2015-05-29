using System.Windows.Forms;
using System.Drawing;
namespace RBClient
{
    partial class FormGrid
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.expandablePanel1 = new DevComponents.DotNetBar.ExpandablePanel();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.label_info = new System.Windows.Forms.Label();
            this.label_inf = new System.Windows.Forms.Label();
            this.label_name = new System.Windows.Forms.Label();
            this.label_nam = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bar1 = new DevComponents.DotNetBar.Bar();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.SearchBox = new DevComponents.DotNetBar.TextBoxItem();
            this.toolStripButton1 = new DevComponents.DotNetBar.ButtonItem();
            this.toolStripSpisok = new DevComponents.DotNetBar.ButtonItem();
            this.buttonSettings = new DevComponents.DotNetBar.ButtonItem();

            //this.gridView = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.gridView = new DataGridView();
            

            this.buttonItem2 = new DevComponents.DotNetBar.ButtonItem();
            this.expandablePanel1.SuspendLayout();
            this.groupPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).BeginInit();
            this.bar1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // expandablePanel1
            // 
            this.expandablePanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.expandablePanel1.CollapseDirection = DevComponents.DotNetBar.eCollapseDirection.TopToBottom;
            this.expandablePanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.expandablePanel1.Controls.Add(this.groupPanel1);
            this.expandablePanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.expandablePanel1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.expandablePanel1.Location = new System.Drawing.Point(0, 542);
            this.expandablePanel1.Name = "expandablePanel1";
            this.expandablePanel1.Size = new System.Drawing.Size(1126, 201);
            this.expandablePanel1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.expandablePanel1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.expandablePanel1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.expandablePanel1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.expandablePanel1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.expandablePanel1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.expandablePanel1.Style.GradientAngle = 90;
            this.expandablePanel1.TabIndex = 0;
            this.expandablePanel1.TitleStyle.Alignment = System.Drawing.StringAlignment.Center;
            this.expandablePanel1.TitleStyle.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.expandablePanel1.TitleStyle.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.expandablePanel1.TitleStyle.Border = DevComponents.DotNetBar.eBorderType.RaisedInner;
            this.expandablePanel1.TitleStyle.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandablePanel1.TitleStyle.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.expandablePanel1.TitleStyle.GradientAngle = 90;
            this.expandablePanel1.TitleText = "Информация о номенклатуре";
            // 
            // groupPanel1
            // 
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.label_info);
            this.groupPanel1.Controls.Add(this.label_inf);
            this.groupPanel1.Controls.Add(this.label_name);
            this.groupPanel1.Controls.Add(this.label_nam);
            this.groupPanel1.Controls.Add(this.pictureBox1);
            this.groupPanel1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupPanel1.Location = new System.Drawing.Point(11, 33);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(1103, 162);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.Class = "";
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.Class = "";
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.Class = "";
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 1;
            // 
            // label_info
            // 
            this.label_info.AutoSize = true;
            this.label_info.BackColor = System.Drawing.Color.Transparent;
            this.label_info.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_info.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_info.Location = new System.Drawing.Point(250, 90);
            this.label_info.Name = "label_info";
            this.label_info.Size = new System.Drawing.Size(0, 16);
            this.label_info.TabIndex = 7;
            // 
            // label_inf
            // 
            this.label_inf.AutoSize = true;
            this.label_inf.BackColor = System.Drawing.Color.Transparent;
            this.label_inf.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_inf.Location = new System.Drawing.Point(250, 74);
            this.label_inf.Name = "label_inf";
            this.label_inf.Size = new System.Drawing.Size(180, 16);
            this.label_inf.TabIndex = 6;
            this.label_inf.Text = "Описание номенклатуры:";
            // 
            // label_name
            // 
            this.label_name.AutoSize = true;
            this.label_name.Font = new System.Drawing.Font("Tahoma", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_name.Location = new System.Drawing.Point(248, 24);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(0, 16);
            this.label_name.TabIndex = 5;
            // 
            // label_nam
            // 
            this.label_nam.AutoSize = true;
            this.label_nam.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_nam.Location = new System.Drawing.Point(248, 8);
            this.label_nam.Name = "label_nam";
            this.label_nam.Size = new System.Drawing.Size(215, 16);
            this.label_nam.TabIndex = 4;
            this.label_nam.Text = "Наименование номенклатуры:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(226, 152);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // bar1
            // 
            this.bar1.Controls.Add(this.labelX2);
            this.bar1.Controls.Add(this.labelX1);
            this.bar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.bar1.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.SearchBox,
            this.toolStripButton1,
            this.toolStripSpisok,
            this.buttonSettings});
            this.bar1.Location = new System.Drawing.Point(0, 0);
            this.bar1.Name = "bar1";
            this.bar1.Size = new System.Drawing.Size(1126, 24);
            this.bar1.Stretch = true;
            this.bar1.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.bar1.TabIndex = 6;
            this.bar1.TabStop = false;
            this.bar1.Text = "bar1";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelX2.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelX2.Location = new System.Drawing.Point(447, -1);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(301, 23);
            this.labelX2.TabIndex = 2;
            this.labelX2.Text = "labelX2";
            this.labelX2.Visible = false;
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelX1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelX1.Location = new System.Drawing.Point(447, -1);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(301, 23);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "labelX1";
            this.labelX1.Visible = false;
            // 
            // SearchBox
            // 
            this.SearchBox.Name = "SearchBox";
            this.SearchBox.TextBoxWidth = 200;
            this.SearchBox.WatermarkColor = System.Drawing.SystemColors.GrayText;
            this.SearchBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SearchBox_KeyPress);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.toolStripButton1.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripButton1.FixedSize = new System.Drawing.Size(75, 10);
            this.toolStripButton1.FontBold = true;
            this.toolStripButton1.Image = global::RBClient.Properties.Resources.search;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Text = "  Поиск";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSpisok
            // 
            this.toolStripSpisok.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.toolStripSpisok.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.toolStripSpisok.FixedSize = new System.Drawing.Size(150, 10);
            this.toolStripSpisok.FontBold = true;
            this.toolStripSpisok.Image = global::RBClient.Properties.Resources.edit_add;
            this.toolStripSpisok.Name = "toolStripSpisok";
            this.toolStripSpisok.Text = "  Добавить позицию";
            this.toolStripSpisok.Visible = false;
            this.toolStripSpisok.Click += new System.EventHandler(this.toolStripSpisok_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonSettings.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.buttonSettings.FixedSize = new System.Drawing.Size(95, 10);
            this.buttonSettings.FontBold = true;
            this.buttonSettings.Image = global::RBClient.Properties.Resources.run;
            this.buttonSettings.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Text = "Настройки";
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // gridView
            // 
            this.gridView.AllowUserToAddRows = false;
            this.gridView.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridView.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.gridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(255, 192, 128);
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;

            //this.gridView.EditMode = DataGridViewEditMode.EditOnEnter;

            this.gridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.gridView.Location = new System.Drawing.Point(0, 24);
            this.gridView.Name = "gridView";
            this.gridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gridView.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.gridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.gridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridView.Size = new System.Drawing.Size(1126, 518);
            this.gridView.TabIndex = 7;
            this.gridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.gridView_CellBeginEdit);
            this.gridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridView_CellEndEdit);
            this.gridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridView_CellEnter);
            this.gridView.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridView_CellLeave);
            this.gridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.gridView_CellValidating);
            this.gridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridView_DataBindingComplete);
            this.gridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.gridView_DataError);
            this.gridView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridView_KeyPress);
            this.gridView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridView_KeyUp);
            // 
            // buttonItem2
            // 
            this.buttonItem2.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.buttonItem2.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground;
            this.buttonItem2.FixedSize = new System.Drawing.Size(75, 10);
            this.buttonItem2.FontBold = true;
            this.buttonItem2.Image = global::RBClient.Properties.Resources.search;
            this.buttonItem2.Name = "buttonItem2";
            this.buttonItem2.Text = "  Поиск";
            // 
            // FormGrid
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1126, 743);
            this.Controls.Add(this.gridView);
            this.Controls.Add(this.bar1);
            this.Controls.Add(this.expandablePanel1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormGrid";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormGrid";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGrid_FormClosing);
            this.Load += new System.EventHandler(this.FormGrid_Load);
            this.expandablePanel1.ResumeLayout(false);
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).EndInit();
            this.bar1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonItem buttonItem2;
        private DevComponents.DotNetBar.ExpandablePanel expandablePanel1;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.Label label_info;
        private System.Windows.Forms.Label label_inf;
        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.Label label_nam;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevComponents.DotNetBar.Bar bar1;
        private DevComponents.DotNetBar.TextBoxItem SearchBox;
        private DevComponents.DotNetBar.ButtonItem toolStripButton1;
        private DevComponents.DotNetBar.ButtonItem toolStripSpisok;
        private DataGridView gridView;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.ButtonItem buttonSettings;


    }
}
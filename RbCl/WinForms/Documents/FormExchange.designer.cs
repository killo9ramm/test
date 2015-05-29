namespace RBClient
{
    partial class FormExchange
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExchange));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            this.button_Cancel = new DevComponents.DotNetBar.ButtonX();
            this.button_Exchange = new DevComponents.DotNetBar.ButtonX();
            this.button_ReExchange = new DevComponents.DotNetBar.ButtonX();
            this.button_SendZReport = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel2 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.dataGridView_HistoryExchange = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.checkedListBox_Tasks = new System.Windows.Forms.CheckedListBox();
            this.styleManager1 = new DevComponents.DotNetBar.StyleManager(this.components);
            this.highlighter1 = new DevComponents.DotNetBar.Validator.Highlighter();
            this.panelEx1.SuspendLayout();
            this.groupPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_HistoryExchange)).BeginInit();
            this.groupPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Windows7;
            this.panelEx1.Controls.Add(this.button_Cancel);
            this.panelEx1.Controls.Add(this.button_Exchange);
            this.panelEx1.Controls.Add(this.button_ReExchange);
            this.panelEx1.Controls.Add(this.button_SendZReport);
            this.panelEx1.Controls.Add(this.groupPanel2);
            this.panelEx1.Controls.Add(this.groupPanel1);
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(635, 520);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 12;
            // 
            // button_Cancel
            // 
            this.button_Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_Cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_Cancel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Cancel.Image = global::RBClient.Properties.Resources.button_cancel;
            this.button_Cancel.Location = new System.Drawing.Point(386, 479);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(115, 33);
            this.button_Cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_Cancel.TabIndex = 20;
            this.button_Cancel.Text = "Отмена";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Exchange
            // 
            this.button_Exchange.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_Exchange.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_Exchange.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Exchange.Image = global::RBClient.Properties.Resources.forward;
            this.button_Exchange.Location = new System.Drawing.Point(507, 479);
            this.button_Exchange.Name = "button_Exchange";
            this.button_Exchange.Size = new System.Drawing.Size(115, 33);
            this.button_Exchange.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_Exchange.TabIndex = 19;
            this.button_Exchange.Text = "Отправить";
            this.button_Exchange.Click += new System.EventHandler(this.button_Exchange_Click);
            //
            //button_ReExchange
            //
            this.button_ReExchange.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_ReExchange.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_ReExchange.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            
            this.button_ReExchange.Location = new System.Drawing.Point(265, 479);
            this.button_ReExchange.Name = "button_ReExchange";
            this.button_ReExchange.Size = new System.Drawing.Size(115, 33);
            this.button_ReExchange.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_ReExchange.TabIndex = 19;
            this.button_ReExchange.Text = "Отправить повторно";
            this.button_ReExchange.Click += new System.EventHandler(this.button_ReExchange_Click);
            // 
            // button_SendZReport
            // 
            this.button_SendZReport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_SendZReport.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_SendZReport.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_SendZReport.Image = global::RBClient.Properties.Resources.date;
            this.button_SendZReport.Location = new System.Drawing.Point(9, 479);
            this.button_SendZReport.Name = "button_SendZReport";
            this.button_SendZReport.Size = new System.Drawing.Size(223, 33);
            this.button_SendZReport.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_SendZReport.TabIndex = 17;
            this.button_SendZReport.Text = "Отправка Z-отчетов за период";
            this.button_SendZReport.Click += new System.EventHandler(this.button_SendZReport_Click);
            // 
            // groupPanel2
            // 
            this.groupPanel2.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel2.Controls.Add(this.dataGridView_HistoryExchange);
            this.groupPanel2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupPanel2.Location = new System.Drawing.Point(9, 248);
            this.groupPanel2.Name = "groupPanel2";
            this.groupPanel2.Size = new System.Drawing.Size(617, 225);
            // 
            // 
            // 
            this.groupPanel2.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel2.Style.BackColorGradientAngle = 90;
            this.groupPanel2.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel2.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderBottomWidth = 1;
            this.groupPanel2.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel2.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderLeftWidth = 1;
            this.groupPanel2.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderRightWidth = 1;
            this.groupPanel2.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderTopWidth = 1;
            this.groupPanel2.Style.Class = "";
            this.groupPanel2.Style.CornerDiameter = 4;
            this.groupPanel2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel2.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel2.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseDown.Class = "";
            this.groupPanel2.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseOver.Class = "";
            this.groupPanel2.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel2.TabIndex = 16;
            this.groupPanel2.Text = "   История обмена";
            this.groupPanel2.TitleImage = ((System.Drawing.Image)(resources.GetObject("groupPanel2.TitleImage")));
            // 
            // dataGridView_HistoryExchange
            // 
            this.dataGridView_HistoryExchange.AllowUserToAddRows = false;
            this.dataGridView_HistoryExchange.AllowUserToDeleteRows = false;
            this.dataGridView_HistoryExchange.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_HistoryExchange.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_HistoryExchange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_HistoryExchange.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridView_HistoryExchange.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_HistoryExchange.Name = "dataGridView_HistoryExchange";
            this.dataGridView_HistoryExchange.Size = new System.Drawing.Size(611, 201);
            this.dataGridView_HistoryExchange.TabIndex = 11;
            // 
            // groupPanel1
            // 
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.checkedListBox_Tasks);
            this.groupPanel1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupPanel1.Location = new System.Drawing.Point(9, 3);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(617, 241);
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
            this.groupPanel1.TabIndex = 15;
            this.groupPanel1.Text = "   Документы к отправке";
            this.groupPanel1.TitleImage = ((System.Drawing.Image)(resources.GetObject("groupPanel1.TitleImage")));
            // 
            // checkedListBox_Tasks
            // 
            this.checkedListBox_Tasks.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkedListBox_Tasks.FormattingEnabled = true;
            this.checkedListBox_Tasks.Location = new System.Drawing.Point(3, 2);
            this.checkedListBox_Tasks.Name = "checkedListBox_Tasks";
            this.checkedListBox_Tasks.Size = new System.Drawing.Size(605, 202);
            this.checkedListBox_Tasks.TabIndex = 0;
            // 
            // styleManager1
            // 
            this.styleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue;
            // 
            // highlighter1
            // 
            this.highlighter1.ContainerControl = this;
            // 
            // FormExchange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 520);
            this.ControlBox = false;
            this.Controls.Add(this.panelEx1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExchange";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Отправить документы...";
            this.Load += new System.EventHandler(this.FormExchange_Load);
            this.panelEx1.ResumeLayout(false);
            this.groupPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_HistoryExchange)).EndInit();
            this.groupPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.PanelEx panelEx1;
        private DevComponents.DotNetBar.ButtonX button_Cancel;
        private DevComponents.DotNetBar.ButtonX button_Exchange;
        private DevComponents.DotNetBar.ButtonX button_ReExchange;
        private DevComponents.DotNetBar.ButtonX button_SendZReport;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel2;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridView_HistoryExchange;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.CheckedListBox checkedListBox_Tasks;
        private DevComponents.DotNetBar.StyleManager styleManager1;
        private DevComponents.DotNetBar.Validator.Highlighter highlighter1;



        
    }
}
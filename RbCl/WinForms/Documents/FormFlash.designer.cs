namespace RBClient
{
    partial class FormFlash
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
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.button_Cancel = new DevComponents.DotNetBar.ButtonX();
            this.button_import = new DevComponents.DotNetBar.ButtonX();
            this.button_export = new DevComponents.DotNetBar.ButtonX();
            this.checkedListBox_Tasks = new System.Windows.Forms.CheckedListBox();
            this.button_export2 = new DevComponents.DotNetBar.ButtonX();
            this.progressBarX1 = new DevComponents.DotNetBar.Controls.ProgressBarX();
            this.button_import2 = new DevComponents.DotNetBar.ButtonX();
            this.panelEx1.SuspendLayout();
            this.groupPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.panelEx1.Controls.Add(this.groupPanel1);
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(369, 275);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 6;
            // 
            // groupPanel1
            // 
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.button_import2);
            this.groupPanel1.Controls.Add(this.progressBarX1);
            this.groupPanel1.Controls.Add(this.button_export2);
            this.groupPanel1.Controls.Add(this.checkedListBox_Tasks);
            this.groupPanel1.Controls.Add(this.button_Cancel);
            this.groupPanel1.Controls.Add(this.button_import);
            this.groupPanel1.Controls.Add(this.button_export);
            this.groupPanel1.Location = new System.Drawing.Point(7, 7);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(356, 261);
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
            this.groupPanel1.TabIndex = 6;
            this.groupPanel1.Text = "Выберите действие!";
            this.groupPanel1.TitleImage = global::RBClient.Properties.Resources.documentinfo1;
            // 
            // button_Cancel
            // 
            this.button_Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_Cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_Cancel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Cancel.Image = global::RBClient.Properties.Resources.button_cancel;
            this.button_Cancel.Location = new System.Drawing.Point(108, 197);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(129, 33);
            this.button_Cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_Cancel.TabIndex = 22;
            this.button_Cancel.Text = " Закрыть";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_import
            // 
            this.button_import.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_import.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_import.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_import.Image = global::RBClient.Properties.Resources.agt_back2;
            this.button_import.Location = new System.Drawing.Point(38, 157);
            this.button_import.Name = "button_import";
            this.button_import.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button_import.Size = new System.Drawing.Size(129, 33);
            this.button_import.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_import.TabIndex = 23;
            this.button_import.Text = "Загрузить с флеш карты";
            this.button_import.Click += new System.EventHandler(this.button_import_Click);
            // 
            // button_export
            // 
            this.button_export.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_export.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_export.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_export.Image = global::RBClient.Properties.Resources.agt_forward;
            this.button_export.Location = new System.Drawing.Point(179, 157);
            this.button_export.Name = "button_export";
            this.button_export.Size = new System.Drawing.Size(129, 33);
            this.button_export.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_export.TabIndex = 21;
            this.button_export.Text = "Записать на флеш карту";
            this.button_export.Click += new System.EventHandler(this.button_export_Click);
            // 
            // checkedListBox_Tasks
            // 
            this.checkedListBox_Tasks.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkedListBox_Tasks.FormattingEnabled = true;
            this.checkedListBox_Tasks.Location = new System.Drawing.Point(3, 3);
            this.checkedListBox_Tasks.Name = "checkedListBox_Tasks";
            this.checkedListBox_Tasks.Size = new System.Drawing.Size(342, 148);
            this.checkedListBox_Tasks.TabIndex = 24;
            // 
            // button_export2
            // 
            this.button_export2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_export2.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_export2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_export2.Image = global::RBClient.Properties.Resources.agt_forward;
            this.button_export2.Location = new System.Drawing.Point(179, 157);
            this.button_export2.Name = "button_export2";
            this.button_export2.Size = new System.Drawing.Size(129, 33);
            this.button_export2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_export2.TabIndex = 25;
            this.button_export2.Text = "Записать на флеш карту";
            this.button_export2.Visible = false;
            this.button_export2.Click += new System.EventHandler(this.button_export2_Click);
            // 
            // progressBarX1
            // 
            // 
            // 
            // 
            this.progressBarX1.BackgroundStyle.Class = "";
            this.progressBarX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.progressBarX1.Location = new System.Drawing.Point(2, 185);
            this.progressBarX1.Name = "progressBarX1";
            this.progressBarX1.Size = new System.Drawing.Size(206, 23);
            this.progressBarX1.TabIndex = 27;
            this.progressBarX1.TextVisible = true;
            this.progressBarX1.Visible = false;
            // 
            // button_import2
            // 
            this.button_import2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_import2.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_import2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_import2.Image = global::RBClient.Properties.Resources.agt_back2;
            this.button_import2.Location = new System.Drawing.Point(39, 157);
            this.button_import2.Name = "button_import2";
            this.button_import2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button_import2.Size = new System.Drawing.Size(129, 33);
            this.button_import2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_import2.TabIndex = 28;
            this.button_import2.Text = "Загрузить с флеш карты";
            this.button_import2.Click += new System.EventHandler(this.button_import2_Click);
            // 
            // FormFlash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 275);
            this.ControlBox = false;
            this.Controls.Add(this.panelEx1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFlash";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Обмен с флеш картой.";
            this.Load += new System.EventHandler(this.FormFlesh_Load);
            this.panelEx1.ResumeLayout(false);
            this.groupPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.PanelEx panelEx1;
        private DevComponents.DotNetBar.ButtonX button_Cancel;
        private DevComponents.DotNetBar.ButtonX button_export;
        private DevComponents.DotNetBar.ButtonX button_import;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.CheckedListBox checkedListBox_Tasks;
        private DevComponents.DotNetBar.ButtonX button_export2;
        private DevComponents.DotNetBar.Controls.ProgressBarX progressBarX1;
        private DevComponents.DotNetBar.ButtonX button_import2;
    }
}


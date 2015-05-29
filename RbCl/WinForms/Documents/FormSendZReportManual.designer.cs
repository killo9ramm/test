namespace RBClient
{
    partial class FormSendZReportManual
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
            this.button_Cancel = new DevComponents.DotNetBar.ButtonX();
            this.button_Send = new DevComponents.DotNetBar.ButtonX();
            this.checkBox_SentArch = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.monthCalendar_To = new DevComponents.Editors.DateTimeAdv.MonthCalendarAdv();
            this.monthCalendar_From = new DevComponents.Editors.DateTimeAdv.MonthCalendarAdv();
            this.panelEx1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.panelEx1.Controls.Add(this.button_Cancel);
            this.panelEx1.Controls.Add(this.button_Send);
            this.panelEx1.Controls.Add(this.checkBox_SentArch);
            this.panelEx1.Controls.Add(this.monthCalendar_To);
            this.panelEx1.Controls.Add(this.monthCalendar_From);
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(374, 239);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 0;
            // 
            // button_Cancel
            // 
            this.button_Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_Cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_Cancel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Cancel.Image = global::RBClient.Properties.Resources.button_cancel;
            this.button_Cancel.Location = new System.Drawing.Point(67, 158);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(115, 33);
            this.button_Cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_Cancel.TabIndex = 22;
            this.button_Cancel.Text = "Отмена";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Send
            // 
            this.button_Send.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_Send.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_Send.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Send.Image = global::RBClient.Properties.Resources.forward;
            this.button_Send.Location = new System.Drawing.Point(192, 158);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new System.Drawing.Size(115, 33);
            this.button_Send.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_Send.TabIndex = 21;
            this.button_Send.Text = "Отправить";
            this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
            // 
            // checkBox_SentArch
            // 
            // 
            // 
            // 
            this.checkBox_SentArch.BackgroundStyle.Class = "";
            this.checkBox_SentArch.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.checkBox_SentArch.Location = new System.Drawing.Point(70, 201);
            this.checkBox_SentArch.Name = "checkBox_SentArch";
            this.checkBox_SentArch.Size = new System.Drawing.Size(236, 23);
            this.checkBox_SentArch.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.checkBox_SentArch.TabIndex = 2;
            this.checkBox_SentArch.Text = "Включая полный архив каталога кассы";
            this.checkBox_SentArch.CheckedChanged += new System.EventHandler(this.checkBox_SentArch_CheckedChanged);
            // 
            // monthCalendar_To
            // 
            this.monthCalendar_To.AnnuallyMarkedDates = new System.DateTime[0];
            this.monthCalendar_To.AutoSize = true;
            // 
            // 
            // 
            this.monthCalendar_To.BackgroundStyle.BackColor = System.Drawing.SystemColors.Window;
            this.monthCalendar_To.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.monthCalendar_To.BackgroundStyle.BorderBottomWidth = 1;
            this.monthCalendar_To.BackgroundStyle.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.monthCalendar_To.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.monthCalendar_To.BackgroundStyle.BorderLeftWidth = 1;
            this.monthCalendar_To.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.monthCalendar_To.BackgroundStyle.BorderRightWidth = 1;
            this.monthCalendar_To.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.monthCalendar_To.BackgroundStyle.BorderTopWidth = 1;
            this.monthCalendar_To.BackgroundStyle.Class = "";
            this.monthCalendar_To.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.monthCalendar_To.CommandsBackgroundStyle.Class = "";
            this.monthCalendar_To.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.monthCalendar_To.ContainerControlProcessDialogKey = true;
            this.monthCalendar_To.DisplayMonth = new System.DateTime(2011, 11, 2, 0, 0, 0, 0);
            this.monthCalendar_To.FirstDayOfWeek = System.DayOfWeek.Monday;
            this.monthCalendar_To.Location = new System.Drawing.Point(192, 12);
            this.monthCalendar_To.MarkedDates = new System.DateTime[0];
            this.monthCalendar_To.MonthlyMarkedDates = new System.DateTime[0];
            this.monthCalendar_To.Name = "monthCalendar_To";
            // 
            // 
            // 
            this.monthCalendar_To.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.monthCalendar_To.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.monthCalendar_To.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.monthCalendar_To.NavigationBackgroundStyle.Class = "";
            this.monthCalendar_To.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.monthCalendar_To.Size = new System.Drawing.Size(170, 128);
            this.monthCalendar_To.TabIndex = 1;
            this.monthCalendar_To.Text = "monthCalendarAdv2";
            this.monthCalendar_To.WeeklyMarkedDays = new System.DayOfWeek[0];
            // 
            // monthCalendar_From
            // 
            this.monthCalendar_From.AnnuallyMarkedDates = new System.DateTime[0];
            this.monthCalendar_From.AutoSize = true;
            // 
            // 
            // 
            this.monthCalendar_From.BackgroundStyle.BackColor = System.Drawing.SystemColors.Window;
            this.monthCalendar_From.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.monthCalendar_From.BackgroundStyle.BorderBottomWidth = 1;
            this.monthCalendar_From.BackgroundStyle.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.monthCalendar_From.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.monthCalendar_From.BackgroundStyle.BorderLeftWidth = 1;
            this.monthCalendar_From.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.monthCalendar_From.BackgroundStyle.BorderRightWidth = 1;
            this.monthCalendar_From.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.monthCalendar_From.BackgroundStyle.BorderTopWidth = 1;
            this.monthCalendar_From.BackgroundStyle.Class = "";
            this.monthCalendar_From.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.monthCalendar_From.CommandsBackgroundStyle.Class = "";
            this.monthCalendar_From.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.monthCalendar_From.ContainerControlProcessDialogKey = true;
            this.monthCalendar_From.DisplayMonth = new System.DateTime(2011, 11, 2, 0, 0, 0, 0);
            this.monthCalendar_From.FirstDayOfWeek = System.DayOfWeek.Monday;
            this.monthCalendar_From.Location = new System.Drawing.Point(12, 12);
            this.monthCalendar_From.MarkedDates = new System.DateTime[0];
            this.monthCalendar_From.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            this.monthCalendar_From.MinDate = new System.DateTime(2009, 1, 1, 0, 0, 0, 0);
            this.monthCalendar_From.MonthlyMarkedDates = new System.DateTime[0];
            this.monthCalendar_From.Name = "monthCalendar_From";
            // 
            // 
            // 
            this.monthCalendar_From.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.monthCalendar_From.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.monthCalendar_From.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.monthCalendar_From.NavigationBackgroundStyle.Class = "";
            this.monthCalendar_From.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.monthCalendar_From.Size = new System.Drawing.Size(170, 128);
            this.monthCalendar_From.TabIndex = 0;
            this.monthCalendar_From.Text = "monthCalendarAdv1";
            this.monthCalendar_From.WeeklyMarkedDays = new System.DayOfWeek[0];
            // 
            // FormSendZReportManual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 239);
            this.Controls.Add(this.panelEx1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSendZReportManual";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Выберите период для отправки отчетов";
            this.Load += new System.EventHandler(this.FormSendZReportManual_Load);
            this.panelEx1.ResumeLayout(false);
            this.panelEx1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.PanelEx panelEx1;
        private DevComponents.DotNetBar.Controls.CheckBoxX checkBox_SentArch;
        private DevComponents.Editors.DateTimeAdv.MonthCalendarAdv monthCalendar_To;
        private DevComponents.Editors.DateTimeAdv.MonthCalendarAdv monthCalendar_From;
        private DevComponents.DotNetBar.ButtonX button_Cancel;
        private DevComponents.DotNetBar.ButtonX button_Send;


    }
}
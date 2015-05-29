namespace RBClient
{
    partial class FormChooseTeremok
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
            this.button_OK = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.ddlTeremok = new System.Windows.Forms.ComboBox();
            this.panelEx1.SuspendLayout();
            this.groupPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.panelEx1.Controls.Add(this.button_Cancel);
            this.panelEx1.Controls.Add(this.button_OK);
            this.panelEx1.Controls.Add(this.groupPanel1);
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(331, 159);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 3;
            // 
            // button_Cancel
            // 
            this.button_Cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_Cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_Cancel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Cancel.Image = global::RBClient.Properties.Resources.button_cancel;
            this.button_Cancel.Location = new System.Drawing.Point(9, 118);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(153, 34);
            this.button_Cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_Cancel.TabIndex = 9;
            this.button_Cancel.Text = "  Отменить";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_OK
            // 
            this.button_OK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_OK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_OK.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_OK.Image = global::RBClient.Properties.Resources.forward;
            this.button_OK.Location = new System.Drawing.Point(168, 118);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(153, 34);
            this.button_OK.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_OK.TabIndex = 8;
            this.button_OK.Text = "  Продолжить";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // groupPanel1
            // 
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.ddlTeremok);
            this.groupPanel1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupPanel1.Location = new System.Drawing.Point(9, 6);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(312, 100);
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
            this.groupPanel1.Text = "   Выбрать ресторан";
            this.groupPanel1.TitleImage = global::RBClient.Properties.Resources.edit_add1;
            // 
            // ddlTeremok
            // 
            this.ddlTeremok.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ddlTeremok.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ddlTeremok.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ddlTeremok.FormattingEnabled = true;
            this.ddlTeremok.Location = new System.Drawing.Point(5, 25);
            this.ddlTeremok.MaxDropDownItems = 10;
            this.ddlTeremok.Name = "ddlTeremok";
            this.ddlTeremok.Size = new System.Drawing.Size(297, 27);
            this.ddlTeremok.TabIndex = 1;
            // 
            // FormChooseTeremok
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 159);
            this.ControlBox = false;
            this.Controls.Add(this.panelEx1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormChooseTeremok";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Выберите куда везти";
            this.Load += new System.EventHandler(this.FormInventChooseTeremok_Load);
            this.panelEx1.ResumeLayout(false);
            this.groupPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.PanelEx panelEx1;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.ComboBox ddlTeremok;
        private DevComponents.DotNetBar.ButtonX button_Cancel;
        private DevComponents.DotNetBar.ButtonX button_OK;
    }
}
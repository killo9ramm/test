namespace RBClient
{
    partial class FormNewDoc
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.bar1 = new DevComponents.DotNetBar.Bar();
            this.treeView = new System.Windows.Forms.TreeView();
            this.navigationPanel = new DevComponents.DotNetBar.ExpandablePanel();
            this.comboBoxItem1 = new DevComponents.DotNetBar.ComboBoxItem();
            this.itemContainer1 = new DevComponents.DotNetBar.ItemContainer();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labelTime = new System.Windows.Forms.Label();
            this.bar2 = new DevComponents.DotNetBar.Bar();
            this.SeekTrackBar = new System.Windows.Forms.TrackBar();
            this.buttonRight = new DevComponents.DotNetBar.ButtonX();
            this.buttonStop = new DevComponents.DotNetBar.ButtonX();
            this.buttonPlay = new DevComponents.DotNetBar.ButtonX();
            this.buttonLeft = new DevComponents.DotNetBar.ButtonX();
            this.videoBox = new System.Windows.Forms.PictureBox();
            this.buttonX2 = new DevComponents.DotNetBar.ButtonX();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).BeginInit();
            this.bar1.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bar2)).BeginInit();
            this.bar2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SeekTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(255, 31);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(898, 531);
            this.webBrowser1.TabIndex = 9;
            this.webBrowser1.Visible = false;
            // 
            // bar1
            // 
            this.bar1.AntiAlias = true;
            this.bar1.Controls.Add(this.buttonX2);
            this.bar1.Controls.Add(this.buttonX1);
            this.bar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.bar1.DockedBorderStyle = DevComponents.DotNetBar.eBorderType.RaisedInner;
            this.bar1.Location = new System.Drawing.Point(255, 0);
            this.bar1.Name = "bar1";
            this.bar1.Size = new System.Drawing.Size(898, 31);
            this.bar1.Stretch = true;
            this.bar1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.bar1.TabIndex = 8;
            this.bar1.TabStop = false;
            this.bar1.Text = "bar1";
            this.bar1.Visible = false;
            // 
            // treeView
            // 
            this.treeView.BackColor = System.Drawing.Color.AliceBlue;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView.Location = new System.Drawing.Point(0, 26);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(255, 536);
            this.treeView.TabIndex = 1;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // navigationPanel
            // 
            this.navigationPanel.CanvasColor = System.Drawing.SystemColors.Control;
            this.navigationPanel.CollapseDirection = DevComponents.DotNetBar.eCollapseDirection.RightToLeft;
            this.navigationPanel.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.navigationPanel.Controls.Add(this.treeView);
            this.navigationPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.navigationPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.navigationPanel.ExpandOnTitleClick = true;
            this.navigationPanel.Location = new System.Drawing.Point(0, 0);
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Size = new System.Drawing.Size(255, 562);
            this.navigationPanel.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.navigationPanel.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.navigationPanel.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.navigationPanel.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.navigationPanel.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.navigationPanel.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.navigationPanel.Style.GradientAngle = 90;
            this.navigationPanel.TabIndex = 7;
            this.navigationPanel.TitleStyle.Alignment = System.Drawing.StringAlignment.Center;
            this.navigationPanel.TitleStyle.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.navigationPanel.TitleStyle.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.navigationPanel.TitleStyle.Border = DevComponents.DotNetBar.eBorderType.RaisedInner;
            this.navigationPanel.TitleStyle.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.navigationPanel.TitleStyle.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.navigationPanel.TitleStyle.GradientAngle = 90;
            this.navigationPanel.TitleText = "Последний материал";
            // 
            // comboBoxItem1
            // 
            this.comboBoxItem1.DropDownHeight = 106;
            this.comboBoxItem1.Name = "comboBoxItem1";
            // 
            // itemContainer1
            // 
            // 
            // 
            // 
            this.itemContainer1.BackgroundStyle.Class = "";
            this.itemContainer1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.itemContainer1.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.itemContainer1.Name = "itemContainer1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labelTime
            // 
            this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTime.AutoSize = true;
            this.labelTime.BackColor = System.Drawing.Color.Transparent;
            this.labelTime.Location = new System.Drawing.Point(590, 4);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(84, 15);
            this.labelTime.TabIndex = 4;
            this.labelTime.Text = "0:00:00/0:00:00";
            // 
            // bar2
            // 
            this.bar2.AntiAlias = true;
            this.bar2.Controls.Add(this.SeekTrackBar);
            this.bar2.Controls.Add(this.labelTime);
            this.bar2.Controls.Add(this.buttonRight);
            this.bar2.Controls.Add(this.buttonStop);
            this.bar2.Controls.Add(this.buttonPlay);
            this.bar2.Controls.Add(this.buttonLeft);
            this.bar2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bar2.Location = new System.Drawing.Point(255, 537);
            this.bar2.Name = "bar2";
            this.bar2.Size = new System.Drawing.Size(898, 25);
            this.bar2.Stretch = true;
            this.bar2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.bar2.TabIndex = 11;
            this.bar2.TabStop = false;
            this.bar2.Text = "bar2";
            this.bar2.Visible = false;
            // 
            // SeekTrackBar
            // 
            this.SeekTrackBar.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.SeekTrackBar.AutoSize = false;
            this.SeekTrackBar.BackColor = System.Drawing.Color.LightBlue;
            this.SeekTrackBar.Location = new System.Drawing.Point(232, 0);
            this.SeekTrackBar.Name = "SeekTrackBar";
            this.SeekTrackBar.Size = new System.Drawing.Size(334, 18);
            this.SeekTrackBar.TabIndex = 5;
            this.SeekTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.SeekTrackBar.Scroll += new System.EventHandler(this.SeekTrackBar_Scroll);
            // 
            // buttonRight
            // 
            this.buttonRight.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonRight.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonRight.Image = global::RBClient.Properties.Resources.player_fwd;
            this.buttonRight.Location = new System.Drawing.Point(86, 2);
            this.buttonRight.Name = "buttonRight";
            this.buttonRight.Size = new System.Drawing.Size(26, 17);
            this.buttonRight.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonRight.TabIndex = 3;
            this.buttonRight.Click += new System.EventHandler(this.buttonRight_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonStop.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonStop.Image = global::RBClient.Properties.Resources.player_stop;
            this.buttonStop.Location = new System.Drawing.Point(58, 2);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(26, 17);
            this.buttonStop.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonPlay.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonPlay.Image = global::RBClient.Properties.Resources.player_play;
            this.buttonPlay.Location = new System.Drawing.Point(30, 2);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(26, 17);
            this.buttonPlay.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonPlay.TabIndex = 1;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonLeft
            // 
            this.buttonLeft.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonLeft.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonLeft.Image = global::RBClient.Properties.Resources.player_start;
            this.buttonLeft.Location = new System.Drawing.Point(2, 2);
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.Size = new System.Drawing.Size(26, 17);
            this.buttonLeft.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonLeft.TabIndex = 0;
            this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
            // 
            // videoBox
            // 
            this.videoBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.videoBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoBox.Location = new System.Drawing.Point(255, 31);
            this.videoBox.Name = "videoBox";
            this.videoBox.Size = new System.Drawing.Size(898, 531);
            this.videoBox.TabIndex = 10;
            this.videoBox.TabStop = false;
            this.videoBox.Visible = false;
            // 
            // buttonX2
            // 
            this.buttonX2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX2.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX2.Image = global::RBClient.Properties.Resources.agt_forward;
            this.buttonX2.Location = new System.Drawing.Point(48, 4);
            this.buttonX2.Name = "buttonX2";
            this.buttonX2.Size = new System.Drawing.Size(30, 15);
            this.buttonX2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX2.TabIndex = 1;
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1.Image = global::RBClient.Properties.Resources.agt_back;
            this.buttonX1.Location = new System.Drawing.Point(12, 4);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Size = new System.Drawing.Size(30, 15);
            this.buttonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1.TabIndex = 0;
            // 
            // FormNewDoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 562);
            this.Controls.Add(this.bar2);
            this.Controls.Add(this.videoBox);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.bar1);
            this.Controls.Add(this.navigationPanel);
            this.Name = "FormNewDoc";
            this.Text = "Последний материал.";
            this.Load += new System.EventHandler(this.FormNewDoc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).EndInit();
            this.bar1.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bar2)).EndInit();
            this.bar2.ResumeLayout(false);
            this.bar2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SeekTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private DevComponents.DotNetBar.ButtonX buttonX2;
        private DevComponents.DotNetBar.Bar bar1;
        private DevComponents.DotNetBar.ButtonX buttonX1;
        private System.Windows.Forms.TreeView treeView;
        private DevComponents.DotNetBar.ExpandablePanel navigationPanel;
        private System.Windows.Forms.PictureBox videoBox;
        private System.Windows.Forms.Timer timer1;
        private DevComponents.DotNetBar.ComboBoxItem comboBoxItem1;
        private DevComponents.DotNetBar.ItemContainer itemContainer1;
        private DevComponents.DotNetBar.ButtonX buttonLeft;
        private DevComponents.DotNetBar.ButtonX buttonPlay;
        private DevComponents.DotNetBar.ButtonX buttonStop;
        private DevComponents.DotNetBar.ButtonX buttonRight;
        private System.Windows.Forms.Label labelTime;
        private DevComponents.DotNetBar.Bar bar2;
        private System.Windows.Forms.TrackBar SeekTrackBar;
    }
}
namespace RBClient
{
    partial class FormDBVideo
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
            this.navigationPanel = new DevComponents.DotNetBar.ExpandablePanel();
            this.treeView = new System.Windows.Forms.TreeView();
            this.bar1 = new DevComponents.DotNetBar.Bar();
            this.labelTime = new System.Windows.Forms.Label();
            this.buttonRight = new DevComponents.DotNetBar.ButtonX();
            this.buttonStop = new DevComponents.DotNetBar.ButtonX();
            this.buttonPlay = new DevComponents.DotNetBar.ButtonX();
            this.buttonLeft = new DevComponents.DotNetBar.ButtonX();
            this.SeekTrackBar = new System.Windows.Forms.TrackBar();
            this.videoBox = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.controlContainerItem1 = new DevComponents.DotNetBar.ControlContainerItem();
            this.navigationPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).BeginInit();
            this.bar1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SeekTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // navigationPanel
            // 
            this.navigationPanel.CanvasColor = System.Drawing.SystemColors.Control;
            this.navigationPanel.CollapseDirection = DevComponents.DotNetBar.eCollapseDirection.RightToLeft;
            this.navigationPanel.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.navigationPanel.Controls.Add(this.treeView);
            this.navigationPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.navigationPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.navigationPanel.Location = new System.Drawing.Point(0, 0);
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Size = new System.Drawing.Size(200, 600);
            this.navigationPanel.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.navigationPanel.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.navigationPanel.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.navigationPanel.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.navigationPanel.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.navigationPanel.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.navigationPanel.Style.GradientAngle = 90;
            this.navigationPanel.TabIndex = 3;
            this.navigationPanel.TitleStyle.Alignment = System.Drawing.StringAlignment.Center;
            this.navigationPanel.TitleStyle.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.navigationPanel.TitleStyle.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.navigationPanel.TitleStyle.Border = DevComponents.DotNetBar.eBorderType.RaisedInner;
            this.navigationPanel.TitleStyle.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.navigationPanel.TitleStyle.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.navigationPanel.TitleStyle.GradientAngle = 90;
            this.navigationPanel.TitleText = "Видео";
            // 
            // treeView
            // 
            this.treeView.BackColor = System.Drawing.Color.AliceBlue;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView.Location = new System.Drawing.Point(0, 26);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(200, 574);
            this.treeView.TabIndex = 1;
           // this.treeView.HideSelection = false;
            
            // 
            // bar1
            // 
            this.bar1.AntiAlias = true;
            this.bar1.Controls.Add(this.labelTime);
            this.bar1.Controls.Add(this.buttonRight);
            this.bar1.Controls.Add(this.buttonStop);
            this.bar1.Controls.Add(this.buttonPlay);
            this.bar1.Controls.Add(this.buttonLeft);
            this.bar1.Controls.Add(this.SeekTrackBar);
            this.bar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bar1.Location = new System.Drawing.Point(200, 575);
            this.bar1.Name = "bar1";
            this.bar1.Size = new System.Drawing.Size(636, 25);
            this.bar1.Stretch = true;
            this.bar1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.bar1.TabIndex = 5;
            this.bar1.TabStop = false;
            this.bar1.Text = "bar1";
            // 
            // labelTime
            // 
            this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTime.AutoSize = true;
            this.labelTime.BackColor = System.Drawing.Color.Transparent;
            this.labelTime.Location = new System.Drawing.Point(590, 4);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(38, 15);
            this.labelTime.TabIndex = 4;
            this.labelTime.Text = "label1";
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
            // SeekTrackBar
            // 
            this.SeekTrackBar.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.SeekTrackBar.AutoSize = false;
            this.SeekTrackBar.BackColor = System.Drawing.Color.LightBlue;
            this.SeekTrackBar.Location = new System.Drawing.Point(3586, 2);
            this.SeekTrackBar.Name = "SeekTrackBar";
            this.SeekTrackBar.Size = new System.Drawing.Size(334, 18);
            this.SeekTrackBar.TabIndex = 4;
            this.SeekTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.SeekTrackBar.Scroll += new System.EventHandler(this.SeekTrackBar_Scroll);
            // 
            // videoBox
            // 
            this.videoBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.videoBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoBox.Location = new System.Drawing.Point(200, 0);
            this.videoBox.Name = "videoBox";
            this.videoBox.Size = new System.Drawing.Size(636, 600);
            this.videoBox.TabIndex = 4;
            this.videoBox.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // controlContainerItem1
            // 
            this.controlContainerItem1.AllowItemResize = false;
            this.controlContainerItem1.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            this.controlContainerItem1.Name = "controlContainerItem1";
            // 
            // FormDBVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 600);
            this.Controls.Add(this.bar1);
            this.Controls.Add(this.videoBox);
            this.Controls.Add(this.navigationPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormDBVideo";
            this.Text = "Видео";
            this.Load += new System.EventHandler(this.FormDBVideo_Load);
            this.navigationPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bar1)).EndInit();
            this.bar1.ResumeLayout(false);
            this.bar1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SeekTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ExpandablePanel navigationPanel;
        private DevComponents.DotNetBar.Bar bar1;
        private DevComponents.DotNetBar.ButtonX buttonRight;
        private DevComponents.DotNetBar.ButtonX buttonStop;
        private DevComponents.DotNetBar.ButtonX buttonPlay;
        private DevComponents.DotNetBar.ButtonX buttonLeft;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.PictureBox videoBox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TrackBar SeekTrackBar;
        private System.Windows.Forms.Label labelTime;
        private DevComponents.DotNetBar.ControlContainerItem controlContainerItem1;
    }
}
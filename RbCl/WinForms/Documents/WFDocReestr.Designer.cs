namespace RBClient.WinForms.Documents
{
    partial class WFDocReestr
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WFDocReestr));
            
            this.treeView = new System.Windows.Forms.TreeView();
            
            this.SuspendLayout();
            
            // 
            // treeView
            // 
            this.treeView.BackColor = System.Drawing.Color.AliceBlue;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(200, 482);
            this.treeView.TabIndex = 2;
            // 
            // WFDocReestr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 482);
            this.Controls.Add(this.treeView);
           
            this.Name = "WFDocReestr";
            this.Text = "WFDocReestr";
           
            this.ResumeLayout(false);

        }

        #endregion

       // private AxAcroPDFLib.AxAcroPDF acroPDF1;
        private System.Windows.Forms.TreeView treeView;
    }
}
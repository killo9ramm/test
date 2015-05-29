namespace RBClient.WinForms
{
    partial class EditSmenaWindow1
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
            this.button_save = new System.Windows.Forms.Button();
            this.button_exit = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timeIntervalControl1 = new RBClient.WinForms.UserControls.TimeIntervalControl();
            this.SuspendLayout();
            // 
            // button_save
            // 
            this.button_save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_save.Location = new System.Drawing.Point(48, 190);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(98, 27);
            this.button_save.TabIndex = 0;
            this.button_save.Text = "Сохранить";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_exit
            // 
            this.button_exit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_exit.Location = new System.Drawing.Point(152, 190);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new System.Drawing.Size(98, 27);
            this.button_exit.TabIndex = 1;
            this.button_exit.Text = "Выход";
            this.button_exit.UseVisualStyleBackColor = true;
            this.button_exit.Click += new System.EventHandler(this.button_exit_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 56);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(260, 26);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Выберите тип смены";
            // 
            // timeIntervalControl1
            // 
            this.timeIntervalControl1.BackColor = System.Drawing.Color.Transparent;
            this.timeIntervalControl1.Location = new System.Drawing.Point(25, 97);
            this.timeIntervalControl1.Name = "timeIntervalControl1";
            this.timeIntervalControl1.Size = new System.Drawing.Size(224, 77);
            this.timeIntervalControl1.TabIndex = 4;
            // 
            // EditSmenaWindow1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 230);
            this.Controls.Add(this.timeIntervalControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button_exit);
            this.Controls.Add(this.button_save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditSmenaWindow1";
            this.Text = "Изменить смену";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_exit;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private UserControls.TimeIntervalControl timeIntervalControl1;
    }
}
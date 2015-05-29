namespace RBClient.WinForms.UserControls
{
    partial class TimeIntervalControl
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.from_mtb = new System.Windows.Forms.MaskedTextBox();
            this.to_mtb = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.hours_txtbx = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // from_mtb
            // 
            this.from_mtb.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.from_mtb.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.from_mtb.Location = new System.Drawing.Point(49, 13);
            this.from_mtb.Mask = "00:00";
            this.from_mtb.Name = "from_mtb";
            this.from_mtb.Size = new System.Drawing.Size(85, 24);
            this.from_mtb.TabIndex = 0;
            this.from_mtb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.from_mtb.ValidatingType = typeof(System.DateTime);
            // 
            // to_mtb
            // 
            this.to_mtb.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.to_mtb.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.to_mtb.Location = new System.Drawing.Point(49, 47);
            this.to_mtb.Mask = "00:00";
            this.to_mtb.Name = "to_mtb";
            this.to_mtb.Size = new System.Drawing.Size(85, 24);
            this.to_mtb.TabIndex = 1;
            this.to_mtb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.to_mtb.ValidatingType = typeof(System.DateTime);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(8, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "До:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(8, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "C:";
            // 
            // hours_txtbx
            // 
            this.hours_txtbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hours_txtbx.Location = new System.Drawing.Point(140, 13);
            this.hours_txtbx.Name = "hours_txtbx";
            this.hours_txtbx.ReadOnly = true;
            this.hours_txtbx.Size = new System.Drawing.Size(73, 24);
            this.hours_txtbx.TabIndex = 4;
            // 
            // TimeIntervalControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.hours_txtbx);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.to_mtb);
            this.Controls.Add(this.from_mtb);
            this.Name = "TimeIntervalControl";
            this.Size = new System.Drawing.Size(224, 77);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox from_mtb;
        private System.Windows.Forms.MaskedTextBox to_mtb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox hours_txtbx;
    }
}

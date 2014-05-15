namespace WindowsFormsApplicationToWPF
{
    partial class Form1
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.autoCompleteBox1 = new WindowsFormsApplicationToWPF.AutoCompleteBox();
            this.elementHost2 = new System.Windows.Forms.Integration.ElementHost();
            this.wpfLoadingControl2 = new WindowsFormsApplicationToWPF.WpfLoadingControl();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(106, 39);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(305, 110);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.autoCompleteBox1;
            // 
            // elementHost2
            // 
            this.elementHost2.Location = new System.Drawing.Point(31, 178);
            this.elementHost2.Name = "elementHost2";
            this.elementHost2.Size = new System.Drawing.Size(619, 263);
            this.elementHost2.TabIndex = 1;
            this.elementHost2.Text = "elementHost2";
            this.elementHost2.Child = this.wpfLoadingControl2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 456);
            this.Controls.Add(this.elementHost2);
            this.Controls.Add(this.elementHost1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.Integration.ElementHost elementHost2;
        private WpfLoadingControl wpfLoadingControl2;
        private AutoCompleteBox autoCompleteBox1;
    }
}


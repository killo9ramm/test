using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using RBClient.WPF.UserControls;

namespace RBClient.Classes.WindowAddElement
{
    
    public partial class FormAddElement : Form
    {
        public ElementHost elhost;
        //public AddElement wpfctl;
        public AddElementI wpfctl;

        
        public FormAddElement()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            elhost = new ElementHost();
            elhost.Size = new Size(this.Width - 20, this.Height - 40);
            elhost.Location = new Point(0, 0);

            //wpfctl = new AddElement();
            wpfctl = new AddElementI();
            wpfctl.Parent = this;
            elhost.Child = wpfctl;

            this.Controls.Add(elhost);
            Resize += Resized;
        }

        private void Resized(object sender, EventArgs e)
        {
            elhost.Size = new Size(this.Width-20, this.Height-40);
        }
        public object Result
        {
            get
            {
                return wpfctl.ReturnedObject;
            }
        }
    }
}

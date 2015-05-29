using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Config_classes;

namespace RBClientUpdateApplication
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Loaded+=new RoutedEventHandler(SettingsWindow_Loaded);
        }
        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigClass.LoadConfigFile();
            text_Settings.Text = ConfigClass.Document.ToString();
        }

        private int return_text_hash(string text)
        {
            List<Char> list = text.ToList<Char>();
            int sum_txt_bx = 0;
            list.ForEach(a => sum_txt_bx += (int)a);
            return sum_txt_bx;
        }

        private void button_Ok_Click(object sender, RoutedEventArgs e)
        {
            //если конфиг новее то записать
            int xml_hash=return_text_hash(ConfigClass.Document.ToString());
            int text_hast = return_text_hash(text_Settings.Text);

            if (text_hast != xml_hash)
            {
                ConfigClass.SaveConfigFile(text_Settings.Text);
            }
            this.Close();
        }
    }
}

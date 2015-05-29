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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using Microsoft.Office.Interop.Word;
using System.IO;
using RBClient.Classes;
using RBClient.Classes.CustomClasses;

namespace RBClient.WPF.UserControls.Documents
{
    /// <summary>
    /// Логика взаимодействия для WPFDocReestr.xaml
    /// </summary>
    public partial class WPFDocReestr : UserControl
    {
        public WPFDocReestr()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                try
                {

                    //DirectoryInfo dir = new DirectoryInfo(@"H:\trashH\doc_to_html");
                    //dir.CreateOrReturn();

                    //dir.DeleteOldFilesInDir(0);

                    //FileInfo doc = new FileInfo(@"H:\trashH\a2dd04f2-7675-11e4-a98e-3085a9967c881.doc");

                    //WordAutomation.ConvertFromDocToHtml(doc, dir);


                    XpsDocument document1 = ConvertWordDocToXPSDoc(@"H:\trashH\a2dd04f2-7675-11e4-a98e-3085a9967c881.doc",
                        @"H:\trashH\a2dd04f2-7675-11e4-a98e-3085a9967c88.xps");
                        //new XpsDocument(@"H:\pdf.pdf", System.IO.FileAccess.Read);
                    DocViewer.Document = document1.GetFixedDocumentSequence();

                }catch(Exception ex)
                {
                }
                //DocViewer.Document =;
            };
        }
        private XpsDocument ConvertWordDocToXPSDoc(string wordDocName, string xpsDocName)
        {
            Microsoft.Office.Interop.Word.Application
                wordApplication = new Microsoft.Office.Interop.Word.Application();
            wordApplication.Documents.Add(wordDocName);

            Document doc = wordApplication.ActiveDocument;
            try
            {
                doc.SaveAs(xpsDocName, WdSaveFormat.wdFormatXPS);
                wordApplication.Quit();

                XpsDocument xpsDoc = new XpsDocument(xpsDocName, System.IO.FileAccess.Read);
                return xpsDoc;
            }
            catch (Exception exp)
            {
                string str = exp.Message;
            }
            return null;
        }
    }
}

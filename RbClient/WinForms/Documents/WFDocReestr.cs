using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.Classes.InternalClasses.Models;
using System.IO;
using RBClient.Classes;

namespace RBClient.WinForms.Documents
{
    public partial class WFDocReestr : BaseForm
    {
        public WFDocReestr()
        {
            InitializeComponent();

            treeView.AfterSelect += treeView_AfterSelect;
            
            Load += form_loaded;
           // Load += (s, e) =>
           //{
           //    try
           //    {
           //        //H:\trashH\a2dd04f2-7675-11e4-a98e-3085a9967c88.docx

           //        //Document d=

           //        //acroPDF1.src = @"H:\pdf.pdf";
           //        //XpsDocument document1 = ConvertWordDocToXPSDoc(@"H:\trashH\a2dd04f2-7675-11e4-a98e-3085a9967c881.doc",
           //        //    @"H:\trashH\a2dd04f2-7675-11e4-a98e-3085a9967c88.xps");
           //        //    //new XpsDocument(@"H:\pdf.pdf", System.IO.FileAccess.Read);
           //        //DocViewer.Document = document1.GetFixedDocumentSequence();

           //    }
           //    catch (Exception ex)
           //    {
           //    }
           //};
        }
        private void form_loaded(object sender,EventArgs e)
        {
            CreateDocTree();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            CBData _data = new CBData();
            try
            {
                if (e.Node.Tag != null)
                {
                    //acroPDF1.src = (string)e.Node.Tag;
                }
            }
            catch (Exception ex)
            {
                Log(ex, "treeView_AfterSelect error");
            }
        }

        private void CreateDocTree()
        {
            DirectoryInfo edu_folder = FileSystemHelper.GetInnerDocsDir();
            props = new t_PropValue().Select<t_PropValue>();

            TreeNode tn = new TreeNode();
            var a = props.WhereFirst(b => b.prop_name == "GetDocumentsSD");
            tn.Text = "Реестр документов.";
            if (a != null) tn.Text += " Релиз:" + a.prop_value;

            CreateTreeNode(tn, edu_folder);
            
            treeView.Nodes.Add(tn);
            props = null;
        }

        private void CreateTreeNode(TreeNode tn, DirectoryInfo dir)
        {
            foreach (var a in dir.GetDirectories().ToList())
            {
                TreeNode t = CreateTreeNode(a);
                if (t != null)
                {
                    tn.Nodes.Add(t);
                    CreateTreeNode(t, a);
                }
            }

            foreach (var a in dir.GetFiles().ToList())
            {
                TreeNode t = CreateNode(a);
                if (t != null) tn.Nodes.Add(t);
            }
        }

        private TreeNode CreateNode(FileInfo f)
        {
            TreeNode t = new TreeNode();
            t.Text = GetStringAssociation(f.Name);
            t.Tag = f.FullName;
            return t;
        }

        List<t_PropValue> props;

        private string GetStringAssociation1(string guid)
        {
            var a = from k in props
                    where k.prop_name == guid && k.prop_type == StaticConstants.Teremok_ID
                    select k;
            if (a == null || a.Count() == 0) return "Нет описания";
            return a.First().prop_value;
        }

        private string GetStringAssociation(string guid)
        {
            var a = from k in props
                    where k.prop_name == guid && k.prop_type == StaticConstants.Teremok_ID
                    select k;
            if (a == null || a.Count() == 0) return guid;
            return a.First().prop_value;
        }

        private TreeNode CreateTreeNode(DirectoryInfo a)
        {
            TreeNode t = new TreeNode();
            t.Text = GetStringAssociation(a.Name);
            t.Tag = null;
            return t;
        }
 
    }
}

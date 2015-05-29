using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RBClient.Classes;
using RBClient.Classes.InternalClasses.Models;

namespace RBClient
{
    public partial class FormDBReestr : Form
    {
        public string docPath;

        public FormDBReestr()
        {
            InitializeComponent();
        }

        private void FormDBReestr_Load(object sender, EventArgs e)
        {
            this.SetBounds(0, 0, Parent.Width - 4, Parent.Height - 4);
            LoadForm();
            Cursor.Current = Cursors.Default;
        }

        private void LoadForm()
        {
            CBData _data = new CBData();
            DataTable _category = _data.categoryReestr();
            DataTable _table;

            try
            {
                CreateDocTree();

                //for (int y = 0; y <= _category.Rows.Count; y++)
                //{
                //    treeView.Nodes.Add(_category.Rows[y].ItemArray[0].ToString());
                //    _table = _data.TreeReestr(_category.Rows[y].ItemArray[0].ToString());


                //    for (int i = 0; i <= _table.Rows.Count - 1; i++)
                //    {
                //        treeView.Nodes[y].Nodes.Add(_table.Rows[i].ItemArray[3].ToString());

                //        this.Text = "Реестр документов версия: " + _table.Rows[i].ItemArray[0].ToString();
                //    }
                //}
            }
            catch (Exception ex)
            {
            }
        }

        List<t_PropValue> props;
        private void CreateDocTree()
        {
            DirectoryInfo edu_folder = FileSystemHelper.GetInnerDocsDir();
            props = new t_PropValue().Select<t_PropValue>();

            TreeNode tn = new TreeNode();
            var a=props.WhereFirst(b=>b.prop_name=="GetDocumentsSD");
            tn.Text = "Реестр документов.";
            if(a!=null)tn.Text += " Релиз:"+a.prop_value;

            CreateTreeNode(tn, edu_folder);

            treeView.Nodes.Add(tn);
            props = null;
        }

        private void IsDirectoryTreeNode(string dirname)
        {

        }

        private TreeNode CreateTreeNode(DirectoryInfo a)
        {
            TreeNode t = new TreeNode();
            t.Text = GetStringAssociation(a.Name);
            t.Tag = null;
            return t;
        }

        private string GetStringAssociation(string guid)
        {
            var a = from k in props
                    where k.prop_name == guid && k.prop_type == StaticConstants.Teremok_ID
                    select k;
            if (a == null || a.Count() == 0) return null;
            return a.First().prop_value;
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

        //private object GetFileNameFromInnerDir(DirectoryInfo f)
        //{
        //    try
        //    {
        //        FileInfo fi = f.FindFileNotStrict(f.Name, false);
        //        if (fi.Exists)
        //        {
        //            return fi.FullName;
        //        }
        //        else
        //        {
        //            MDIParentMain.Log("DocReestr GetFileNameFromInnerConfig no such file "+f.Name+
        //                " in "+f.FullName);
        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MDIParentMain.Log(ex,"DocReestr GetFileNameFromInnerConfig");
        //        return "";
        //    }
        //}

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            CBData _data = new CBData();
            try
            {
                //string _name = _data.videoPath(treeView.SelectedNode.Text.ToString());
                string docPath = (string)treeView.SelectedNode.Tag;
                //docPath = CParam.AppFolder + "\\help\\" + _name;
                Uri _uri = new Uri(docPath);
                webBrowser1.Url = _uri;
            }
            catch(Exception ex)
            {
                MDIParentMain.Log(ex, "DocReestr treeView_AfterSelect error");
            }
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }
                
        private void buttonX1_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void bar1_ItemClick(object sender, EventArgs e)
        {
            //
        }
    }
}

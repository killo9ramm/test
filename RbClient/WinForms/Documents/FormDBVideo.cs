using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Video = Microsoft.DirectX.AudioVideoPlayback.Video;
using RBClient.Classes;
using RBClient.Classes.InternalClasses.Models;

namespace RBClient
{
    public partial class FormDBVideo : Form
    {
        /// <summary>
        /// Высота заголовка окна
        /// </summary>
        internal const int cap = 4;

        /// <summary>
        /// Толщина рамки окна
        /// </summary>
        internal const int fr = 33;

        Video video;
        public string PlayingPosition, Duration;
        public string videoPath;

        public FormDBVideo()
        {
            InitializeComponent();

            
        }


        public TreeNode previousSelectedNode = null;
        private void treeView1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            treeView.SelectedNode.BackColor = SystemColors.Highlight;
            treeView.SelectedNode.ForeColor = Color.White;
            previousSelectedNode = treeView.SelectedNode;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (previousSelectedNode != null)
            {
                previousSelectedNode.BackColor = treeView.BackColor;
                previousSelectedNode.ForeColor = treeView.ForeColor;
            }
        }

        public void FormDBVideo_Load(object sender, EventArgs e)
        {
            this.SetBounds(0, 0, Parent.ClientRectangle.Width - 4, Parent.ClientRectangle.Height - 4);
            LoadForm();
            Cursor.Current = Cursors.Default;
            SeekTrackBar.Location = new Point(15, 2);
        }

        //Dictionary<string,string> video_files

        private void LoadForm()
        {
            CBData _data = new CBData();
            DataTable _category = _data.categoryVideo();
            DataTable _table;

            //рекурсивный просмотр папок
            //добавление нужных папок в лист

            try
            {
                #region video from folder
                CreateVideoTree();
                //CreateVideoTree_old();

                treeView.AfterSelect += treeView1_AfterSelect;
                this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
                treeView.Validating += treeView1_Validating;

                #endregion

                #region video from db
                //for (int y = 0; y <= _category.Rows.Count; y++)
                //{
                //    treeView.Nodes.Add(_category.Rows[y].ItemArray[0].ToString());
                //    _table = _data.TreeVideo(_category.Rows[y].ItemArray[0].ToString());

                //    for (int i = 0; i <= _table.Rows.Count - 1; i++)
                //    {
                //        treeView.Nodes[y].Nodes.Add(_table.Rows[i].ItemArray[3].ToString());
                //        this.Text = "Видео версия сборки:  " + _table.Rows[i].ItemArray[0].ToString();
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                MDIParentMain.log.Error("Ошибка инициализации формы видео :",ex);
            }
        }

        private void CreateVideoTree()
        {
            DirectoryInfo edu_folder = FileSystemHelper.GetEducVideoDir();
            props = new t_PropValue().Select<t_PropValue>();

            Log("FormDbVideo начинаем создание TreeNode");

            TreeNode tn = new TreeNode();
            tn.Text = "Обучающее видео";
            
            Log("FormDbVideo начинаем создание дочерних Nodes");

            CreateTreeNode(tn, edu_folder);

            Log("FormDbVideo добавляем дочерние узлы в TreeNode");

            treeView.Nodes.Add(tn);
            props = null;
        }


        private void CreateTreeNode(TreeNode tn,DirectoryInfo dir)
        {
            List<object> lo = new List<object>();
            lo.AddRange(dir.GetDirectories());
            lo.AddRange(dir.GetFiles());
            Dictionary<t_PropValue,object> dtp = new Dictionary<t_PropValue,object>();

            foreach (var o in lo)
            {
                if (o is FileSystemInfo)
                {
                    LogDebug("FormDBVideo find file association: " + ((FileSystemInfo)o).Name);
                    var oo=GetAssociation(((FileSystemInfo)o).Name);

                    LogDebug("FormDBVideo file association is null: " + (oo==null).ToString());

                    if (oo != null)
                    {
                        LogDebug("FormDBVideo adding to dtp: " + Serializer.JsonSerialize(oo));
                        dtp.Add(oo, o);
                        
                    }
                }
            }

            var ltp=dtp.Keys.ToList();

            

            ltp.Sort((a,b)=>a.add_prop2.CompareTo(b.add_prop2));

            LogDebug("FormDBVideo files is: " + Serializer.JsonSerialize(ltp));

            LogDebug("FormDBVideo dtp.notnullorempty: " + dtp.NotNullOrEmpty().ToString());

          //  LogDebug("FormDBVideo dictionary propValues is: " + Serializer.JsonSerialize(dtp));

            foreach (var l in ltp)
            {
                LogDebug("FormDBVideo dtp key: " + l.ToString());

                var o = dtp[l];

                LogDebug("FormDBVideo dtp value: " + o.ToString());

                if (o is FileInfo)
                {
                    LogDebug("FormDBVideo o is file ");
                    LogDebug("FormDBVideo create Node for: " + Serializer.JsonSerialize(((FileInfo)o).FullName));

                    TreeNode t = CreateNode((FileInfo)o,l);

                    LogDebug("FormDBVideo created Node is: " + Serializer.JsonSerialize(t.ToString()));

                    if (t != null) tn.Nodes.Add(t);
                }
                if (o is DirectoryInfo)
                {
                    LogDebug("FormDBVideo o is folder ");
                    LogDebug("FormDBVideo create TreeNode for: " + Serializer.JsonSerialize(((DirectoryInfo)o).FullName));

                    TreeNode t = CreateTreeNode(l);



                    if (t != null)
                    {
                        LogDebug("FormDBVideo created TreeNode is: " + t.ToString());
                        tn.Nodes.Add(t);
                        CreateTreeNode(t, (DirectoryInfo)o);
                    }
                    else
                    {
                        LogDebug("FormDBVideo TreeNode is null");
                    }
                }
            }
        }

        private TreeNode CreateNode(FileInfo f)
        {
            TreeNode t = new TreeNode();
            t.Text = GetStringAssociation(f.Name);
            t.Tag = f.FullName;
            return t;
        }

        private TreeNode CreateNode(FileInfo f,t_PropValue tp)
        {
            TreeNode t = new TreeNode();
            t.Text = tp.prop_value;
            t.Tag = f.FullName;
            return t;
        }

        List<t_PropValue> props;

        private string GetStringAssociation1(string guid)
        {
            var a=from k in props
                  where k.prop_name==guid //&& k.prop_type==StaticConstants.Teremok_ID
                       select k;
            if (a==null||a.Count()==0) return "Нет описания";
            return a.First().prop_value;
        }

        private string GetStringAssociation(string guid)
        {
            var a = from k in props
                    where k.prop_name == guid //&& k.prop_type == StaticConstants.Teremok_ID
                    select k;
            if (a == null || a.Count() == 0) return guid;
            return a.First().prop_value;
        }

        private t_PropValue GetAssociation(string guid)
        {
            var a = from k in props
                    where k.prop_name == guid// && k.prop_type == StaticConstants.Teremok_ID
                    select k;
            if (a == null || a.Count() == 0) return null;
            return a.First();
        }

        private TreeNode CreateTreeNode(t_PropValue tp)
        {
            LogDebug("FormDBVideo creating TreeNode for: " + tp.prop_value);
            TreeNode t = new TreeNode();
            t.Text = tp.prop_value;
            t.Tag = null;
            LogDebug("FormDBVideo created TreeNode: " + t.ToString());
            return t;
        }
        private TreeNode CreateTreeNode(DirectoryInfo a,t_PropValue tp)
        {
            TreeNode t = new TreeNode();
            t.Text = tp.prop_value;
            t.Tag = null;
            return t;
        }

        private void CreateVideoTree_old()
        {
            DirectoryInfo videoDir1 = new DirectoryInfo(CParam.AppFolder + "\\video");
            DirectoryInfo videoDir2 = new DirectoryInfo(CParam.AppFolder + "\\ADV_VIDEO");
            List<DirectoryInfo> dir_list = new List<DirectoryInfo>() { videoDir1, videoDir2 };

            dir_list.ForEach(d =>
            {
                TreeNode tn = new TreeNode();
                tn.Text = d.Name;
                if (d.Name == "video") tn.Text = "Обучающее видео";
                if (d.Name == "ADV_VIDEO") tn.Text = "Рекламное видео";


                List<FileInfo> video_list = d.GetFiles().ToList();
                video_list.ForEach(a =>
                {
                    TreeNode tn1 = new TreeNode();
                    tn1.Text = a.Name;
                    tn1.Tag = a.FullName;
                    tn.Nodes.Add(tn1);

                });
                treeView.Nodes.Add(tn);
            });
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            video.CurrentPosition -= 2;
        }

        private void Log(string message)
        {
            MDIParentMain.Log(message);
        }

        private void LogDebug(string message)
        {
#if(RELEASE_DEBUG)
            MDIParentMain.Log(message);
#endif
        }

        private void Log(Exception ex, string message)
        {
            MDIParentMain.Log(ex, message);
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Играем видео "+ video);
                if (video != null)
                {
                    if (video.Playing)
                    {
                        video.Pause();
                        timer1.Stop();
                        buttonPlay.Image = Image.FromFile(CParam.AppFolder + "\\Img\\player_play.png");
                    }
                    else
                    {
                        video.Play();
                        timer1.Start();
                        buttonPlay.Image = Image.FromFile(CParam.AppFolder + "\\Img\\player_pause.png");
                    }
                }
            }
            catch(Exception ex)
            {
                MDIParentMain.Log(ex,"Не получилось проиграть видео ");
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (video != null)
                    timer1.Stop();
                video.StopWhenReady();
            }
            catch
            { 
            }
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            video.CurrentPosition += 2;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            CBData _data = new CBData();
            try
            {
                buttonPlay.Image = Image.FromFile(CParam.AppFolder + "\\Img\\player_play.png");

                videoPath = e.Node.Tag.ToString();

                StartUp();
            }
            catch(Exception ex)
            {
                Log(ex, "treeView_AfterSelect error");
            }
        }

        public void StartUp()
        {
            try
            {
                Log("0");
                if (video != null)
                    video.Dispose();
                videoBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                //
                Log("before create " + videoPath);
                video = new Video(videoPath);

                Log("create video " + videoPath + " " + video.Size);

                Size size = new Size();
                video.Owner = videoBox;
                size.Width = Parent.Width - 4;
                size.Height = Parent.Height - 4;
                this.Width = size.Width;
                this.Height = size.Height;

                SeekTrackBar.Minimum = Convert.ToInt32(video.CurrentPosition);
                SeekTrackBar.Maximum = Convert.ToInt32(video.Duration);
                SeekTrackBar.Value = 0;

                Duration = CalculateTime(video.Duration);
                PlayingPosition = "00:00:00";
                labelTime.Text = PlayingPosition + "/" + Duration;
            }
            catch(Exception ex)
            {
                Log(ex, "Video startup exception");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Log("1");
                SeekTrackBar.Value = Convert.ToInt32(video.CurrentPosition);
                Log("2");
                PlayingPosition = CalculateTime(video.CurrentPosition);
                Log("3");
                labelTime.Text = PlayingPosition + "/" + Duration;
                Log("4");
                if (PlayingPosition == Duration)
                {
                    Log("5");
                    timer1.Stop();
                    Log("6");
                    StartUp();
                    Log("7");
                    buttonPlay.Image = Image.FromFile(CParam.AppFolder + "\\Img\\player_play.png");
                    Log("8");
                }
            }
            catch(Exception ex)
            {
                Log(ex, "Video timer tick exception");
            }
        }

        private void SeekTrackBar_Scroll(object sender, EventArgs e)
        {
            if (video != null)
            {
                video.CurrentPosition = SeekTrackBar.Value;
            }
            else
            {
                SeekTrackBar.Value = 0;
            }
        }

        public string CalculateTime(double Time)
        {
            string mm, ss, CalculatedTime;
            int h, m, s, T;

            Time = Math.Round(Time);
            T = Convert.ToInt32(Time);

            h = (T / 3600);
            T = T % 3600;
            m = (T / 60);
            s = T % 60;

            if (m < 10)
                mm = string.Format("0{0}", m);
            else
                mm = m.ToString();
            if (s < 10)
                ss = string.Format("0{0}", s);
            else
                ss = s.ToString();

            CalculatedTime = string.Format("{0}:{1}:{2}", h, mm, ss);

            return CalculatedTime;
        }
    }
}

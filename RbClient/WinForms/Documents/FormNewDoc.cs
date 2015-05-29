using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Video = Microsoft.DirectX.AudioVideoPlayback.Video;

namespace RBClient
{
    public partial class FormNewDoc : Form
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
        public string docPath;

        public FormNewDoc()
        {
            InitializeComponent();
        }


        private void FormNewDoc_Load(object sender, EventArgs e)
        {
            this.SetBounds(0, 0, Parent.Width - 4, Parent.Height - 4);
            LoadForm();
            Cursor.Current = Cursors.Default;
        }

        private void LoadForm()
        {
            CBData _data = new CBData();
            DataTable _newDoc = _data.NewDoc();

            try
            {
                for (int a = 0; a <= _newDoc.Rows.Count; a++)
                {
                    treeView.Nodes.Add(_newDoc.Rows[a].ItemArray[2].ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                SeekTrackBar.Value = Convert.ToInt32(video.CurrentPosition);
                PlayingPosition = CalculateTime(video.CurrentPosition);
                labelTime.Text = PlayingPosition + "/" + Duration;

                if (PlayingPosition == Duration)
                {
                    timer1.Stop();
                    StartUp();
                    buttonPlay.Image = Image.FromFile(CParam.AppFolder + "\\Img\\player_play.png");
                }
            }
            catch
            {
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

        public void StartUp()
        {
            try
            {
                if (video != null)
                    video.Dispose();
                videoBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;

                video = new Video(videoPath);

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
            catch
            {
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            CBData _data = new CBData();
            string _name = "";
            try
            {
                buttonPlay.Image = Image.FromFile(CParam.AppFolder + "\\Img\\player_play.png");
                _name = _data.newDocPath(treeView.SelectedNode.Text.ToString());
                string[] _s = _name.Split(".".ToCharArray());

                if (_s[1] == "avi")
                {
                    webBrowser1.Visible = false;
                    bar1.Visible = false;
                    videoBox.Visible = true;
                    bar2.Visible = true;
                    videoPath = CParam.AppFolder + "\\video\\" + _name;
                    StartUp();
                }
                if (_s[1] == "mht")
                {
                    videoBox.Visible = false;
                    bar2.Visible = false;
                    webBrowser1.Visible = true;
                    bar1.Visible = true;
                    docPath = CParam.AppFolder + "\\help\\" + _name;
                    Uri _uri = new Uri(docPath);
                    webBrowser1.Url = _uri;
                }
            }
            catch (Exception)
            {
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            try
            {
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
            catch
            {
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

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            video.CurrentPosition -= 2;
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
    }
}

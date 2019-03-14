using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GDIBuffer_Sample_2012_07_25
{
    public partial class Neruon_Form : Form
    {
        public Rectangle FormRect;
        public static Neruon_Form nf;
        public Neruon_Form()
        {
            InitializeComponent();
            Neruon_Form.nf = this;
            FormRect = new Rectangle(
                 ClientRectangle.X,
                 ClientRectangle.Y,
                 ClientRectangle.Width,
                 ClientRectangle.Height
                 );
        }
      
        private void TestStart(object sender, EventArgs e)
        {
            MainForm.mf.TestStart();
        }

        private void Neruon_Form_Load(object sender, EventArgs e)
        {
            SettingLoad(Setting.Instance());
        }
        private void SettingLoad(Setting bs)
        {
            bfset_Time.Text = MainForm.mf.frameTimer.Interval.ToString();
            bfset_Mapsize.Text = bs.mapSize_Height.ToString();
            bfset_Blocksize.Text = bs.블럭크기.ToString();
            bfset_MaxFrame.Text = bs.최대프레임.ToString();
            bfset_MinFrame.Text = bs.최소프레임.ToString();
            bfset_hiddenlayer.Text = bs.히든레이어수.ToString();
            bfset_Sextimes.Text = bs.교배횟수.ToString();
            bfset_MPer.Text = bs.돌연변이확률.ToString();
            bfset_MSextimes.Text = bs.돌연변이교환횟수.ToString();
            bfset_UnitSize.Text = bs.모집단.ToString();
            bfset_TestSize.Text = bs.테스트단위.ToString();
            bfset_loadcycle.Text = bs.저장간격.ToString();
            bfset_storeLocation.Text = bs.저장위치;
            bfset_loadcount.Text = bs.저장개체수.ToString();

        }

        private void LoadSetting(object sender, EventArgs e)
        {
            Setting _temp = new Setting();


            MainForm.mf.frameTimer.Interval = int.Parse(bfset_Time.Text);
            _temp.mapSize_Height = int.Parse(bfset_Mapsize.Text);
            _temp.mapSize_Width = int.Parse(bfset_Mapsize.Text);
            _temp.블럭크기 = int.Parse(bfset_Blocksize.Text); 

            _temp.최대프레임 = int.Parse(bfset_MaxFrame.Text);
            _temp.최소프레임 = int.Parse(bfset_MinFrame.Text);

            _temp.히든레이어수 = int.Parse(bfset_hiddenlayer.Text);

            _temp.교배횟수 = int.Parse(bfset_Sextimes.Text);

            _temp.돌연변이확률 = int.Parse(bfset_MPer.Text);
            _temp.돌연변이교환횟수 = int.Parse(bfset_MSextimes.Text);

            _temp.저장간격 = int.Parse(bfset_loadcycle.Text);
            _temp.저장위치 = bfset_storeLocation.Text;
            _temp.저장개체수 = int.Parse(bfset_loadcount.Text);

            _temp.모집단 = int.Parse(bfset_UnitSize.Text);
            if (_temp.모집단 < int.Parse(bfset_TestSize.Text))
            {
                _temp.테스트단위 = int.Parse(bfset_UnitSize.Text);
                bfset_TestSize.Text = bfset_UnitSize.Text;
            }
            else
                _temp.테스트단위 = int.Parse(bfset_TestSize.Text);


            Setting.st = _temp;
        }

        private void reset(object sender, EventArgs e)
        {
            MainForm.mf.frameTimer.Enabled = false;
            MainForm.mf.resetFrame();
        }

        private void bfset_loadplay(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.Length > 0)
            {
                IFormatter formatter = new BinaryFormatter();

                Stream stream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                Setting set = (Setting)formatter.Deserialize(stream);

                set.모집단 = Setting.Instance().저장개체수;
                set.테스트단위 = Setting.Instance().저장개체수;

                SettingLoad(set);

                Setting.st = set;

                Gen[] gen = new Gen[Setting.Instance().저장개체수];
                Stream stream_G = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                for (int i = 0; i < Setting.Instance().저장개체수; i++)
                    gen[i] = (Gen)formatter.Deserialize(stream);

                //현재 상태 리셋
                MainForm.mf.frameTimer.Enabled = false;
                MainForm.mf.resetFrame();

                MainForm.mf.ReplayStart(gen);



                stream.Close();
                stream_G.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            save_folder.ShowDialog();
            bfset_storeLocation.Text = save_folder.SelectedPath;
        }
    }
}


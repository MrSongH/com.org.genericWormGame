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
namespace GDIBuffer_Sample_2012_07_25// ours
{
    public partial class MainForm : Form
    {
        public static MainForm mf;

        public static Random random = new Random((int)DateTime.Now.Ticks);
        static int blocksize = 10;
        static int window_width= Setting.Instance().mapSize_Width * blocksize, window_height= Setting.Instance().mapSize_Height*blocksize; // 맵 초기 좌표 x, 맵 초기 좌표 y, 맵 크기 x, 맵 크기 y 

        private int rotate = 0;
        public int FrameCount = 0;
        public static int _Generation = 1;
        public List<Trainer> trainers = new List<Trainer>();
        public Trainer warms;

        public MainForm()
        {
            MainForm.mf = this;
            InitializeComponent();
            this.SuspendLayout();
            Setting.Instance();

            warms = new Trainer(window_width, window_height, blocksize);
            warms.train_setting();
            Load += new EventHandler(MainForm_Load);
            this.ResumeLayout(false);
        }
 

        private void MainForm_Load(object sender, EventArgs e)
        {
            GDIBuffer.Instance(ClientRectangle.Width, ClientRectangle.Height);
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);                
            Neruon_Form nf = new Neruon_Form();
            nf.Show();
        }

 

        private void FrameUpdate()
        {
     
                GDIBuffer.Instance().getGraphics.Clear(Color.White);   
                int leftWarm = (from t in trainers where t.dead == 0 select t).Count();
                Font _font = new System.Drawing.Font("dotum", 10, FontStyle.Regular);
                TextRenderer.DrawText(GDIBuffer.Instance().getGraphics, "alive Warms : " + leftWarm, _font, new Point(30, 30), Color.Black);
                TextRenderer.DrawText(GDIBuffer.Instance().getGraphics, "Generation : " + _Generation, _font, new Point(160, 30), Color.Black);

        }

        private void FrameRender()
        {
            Graphics g = CreateGraphics();

            int start, end;

            if(Setting.Instance().테스트단위 > Setting.Instance().모집단)
            {
                start = 0;
                end = Setting.Instance().모집단;
            }
            else
            {
                if(rotate < Setting.Instance().모집단 / Setting.Instance().테스트단위)
                {
                    start = rotate * Setting.Instance().테스트단위;
                    end = Setting.Instance().테스트단위 + Setting.Instance().테스트단위 * rotate;
                }
                else
                {
                    start = rotate * Setting.Instance().테스트단위;
                    end = Setting.Instance().모집단 % Setting.Instance().테스트단위
                        + Setting.Instance().테스트단위 * rotate;
                }
            }
            for (int i = start; i < end; i++)
            {
                trainers[i].WarmAction();
            }
            g.DrawImage(GDIBuffer.Instance().GetImages, new Point(0, 0));
            g.Dispose();
           
        }

        private void frameTimer_Tick(object sender, EventArgs e)
        {
            FrameCount++;

            bool endcheck = false;

            int start, end;
            if (Setting.Instance().테스트단위 > Setting.Instance().모집단)
            {
                start = 0;
                end = Setting.Instance().모집단;
            }
            else
            {
                if (rotate < Setting.Instance().모집단 / Setting.Instance().테스트단위)
                {
                    start = rotate * Setting.Instance().테스트단위;
                    end = Setting.Instance().테스트단위 + Setting.Instance().테스트단위 * rotate;
                }
                else
                {
                    start = rotate * Setting.Instance().테스트단위;
                    end = Setting.Instance().모집단 % Setting.Instance().테스트단위 + Setting.Instance().테스트단위 * rotate;
                }
            }
            for (int i = start; i < end; i++)
            {
                trainers[i].FrameUpdate();
            }
            for (int i = start, k = 0; i < end; i++)
            {
                if (warms._Genetic.score <= trainers[i]._Genetic.score) warms = trainers[i];
                if (trainers[i].dead == 1) // 부딪히는 함수 추가필요
                   k++;
                if (k == Setting.Instance().테스트단위 || k == Setting.Instance().모집단) endcheck = true;
            }
            if (endcheck) // boolean = true -> 끝값이 수신되면
            {
                rotate = (rotate + 1);
                FrameCount = 0;
                warms = new Trainer(window_width, window_height, blocksize);
                warms.train_setting();
            }

            if (rotate == (Setting.Instance().모집단 / Setting.Instance().테스트단위))
            {
                rotate = 0; // rotate 초기화

                trainers.Sort((Trainer x, Trainer y) => y._Genetic.score.CompareTo(x._Genetic.score));           //내림차순 정렬
                

                try
                {
                    if (_Generation % Setting.Instance().저장간격 == 0)
                    {
                        IFormatter formatter = new BinaryFormatter();

                        Stream stream = new FileStream(Setting.Instance().저장위치 + @"\BestGeneration[" + _Generation + "].bin", FileMode.Create, FileAccess.Write, FileShare.None);

                        formatter.Serialize(stream, Setting.Instance());
                        for (int i = 0; i < Setting.Instance().저장개체수; i++)
                        {
                            formatter.Serialize(stream, trainers[i]._Genetic);
                        }
                        stream.Close();
                    }
                    using (StreamWriter w = File.AppendText(Setting.Instance().저장위치 + @"\BestGeneration.txt"))
                    {
                        //w.Write(log);
                    }
                }
                catch (Exception ee)
                {
                    using (StreamWriter w = File.AppendText("BestGeneration.txt"))
                    {
                       // w.Write(log);
                    }
                }
                trainers = Generic.check(trainers, Setting.Instance().모집단); // 확률분포리스트를 뽑음
                trainers = Generic.repList(trainers, Setting.Instance().모집단);  // 교배 및 돌연변이
                warms._Genetic.score = 1; // 점수 초기화
                _Generation++; // 세대 +1
            }
            FrameUpdate();
            FrameRender();
        }

        public void TestStart()
        {
            FrameCount = 0;
            rotate = 0;
            _Generation = 0;
            trainers.Clear();
            GDIBuffer.Instance(window_width,window_height);
            for (int i=0; i<Setting.Instance().모집단; i++)
            {
                Trainer t = new Trainer(window_width, window_height, blocksize);
                t.train_setting();
                trainers.Add(t);
            }
            frameTimer.Enabled = true;
        }
        public void resetFrame()
        {
            warms = new Trainer(window_width, window_height, blocksize);
            warms.train_setting();       
            Graphics g = CreateGraphics();

            FrameUpdate();
            g.DrawImage(GDIBuffer.Instance().GetImages, new Point(0, 0));
            g.Dispose();
        }
        public void ReplayStart(Gen[] GData)
        {
            FrameCount = 0;
            rotate = 0;
            _Generation = 0;
            trainers.Clear();

            for (int i = 0; i < GData.Length; i++)
            {
                GData[i].score = 1;
                warms = new Trainer(window_width, window_height, blocksize);
                warms.train_setting();
            }
            frameTimer.Enabled = true;
        }
    }
}







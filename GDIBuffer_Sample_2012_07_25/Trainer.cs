using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GDIBuffer_Sample_2012_07_25
{
    public class Trainer
    {
        // Random random = new Random();
        public Gen _Genetic = new Gen();
        public Gen SetGeneric { set { _Genetic = value; } }
        public int[] findob;

        public int[] findbouns;

        public int[] mapObstacle;
        public int[] headCordinate;
        public int[] bonusCordinate;
        public int[] inputarr;
        public int[,] MemoryStorage;
        // 유전자 부
        public int[] new_arr;
        public double Preceed_dist;
        public double Present_dist;

        public double turn_left;
        public double turn_right;
        public double up;
        public double down;
        public int count_eat_bonus = 0;
        public int frame_count = 0;
        public int check_dead = 0;
        public int countMemory = 0;
        // 출력 부 -> 지렁이가 전진,좌회전,우회전

        public int mapX { get; set; }
        public int mapY { get; set; }
        public int bonus_X { get; set; }
        public int bonus_Y { get; set; }
        public int blocksize { get; set; }
        // 맵 설정 부

        public int check_eat_bonus = 0;
        public int check_eat_bonus_count = 0;
        public int dead = 0;
        public double Total_score = 0;
        public Warm Warms_total = new Warm();
        pixel_Map Pixel_Map;
        public Trainer() { }
        public bool far = false;

        public Trainer(int Width, int Height, int blocksize)
        {
            this.mapX = Width;
            this.mapY = Height;
            this.blocksize = blocksize;
            Pixel_Map = new pixel_Map(Width, Height, blocksize);
            MemoryStorage = new int[Setting.Instance().mapSize_Height, Setting.Instance().mapSize_Width];
        }

        public void train_setting()
        {
            Warms_total.make_head(
            Setting.Instance().mapSize_Width / 2,
            Setting.Instance().mapSize_Height / 2);
            Warms_total.mapWidth = mapX;
            Warms_total.mapHeight = mapY;
            Warms_total.blocksize = blocksize;
            make_Bonus();
        }

        public Trainer Clone()
        {
            Trainer New_Trainer = new Trainer(Setting.Instance().mapSize_Width, Setting.Instance().mapSize_Height, blocksize);

            New_Trainer.train_setting();
            New_Trainer._Genetic = _Genetic.Clone();

            return New_Trainer;
        } // 사본생산

        public void make_Bonus()
        {
            int random_x = MainForm.random.Next(1, Setting.Instance().mapSize_Width - 1);
            int random_y = MainForm.random.Next(1, Setting.Instance().mapSize_Height - 1);

            for (int i = 0; i < Warms_total.warm_parts.Count; i++)
            {
                if (Warms_total.warm_parts[i].x == random_x && Warms_total.warm_parts[i].y == random_y)
                {
                    i = 0;
                    random_x = MainForm.random.Next(1, Setting.Instance().mapSize_Width - 1);
                    random_y = MainForm.random.Next(1, Setting.Instance().mapSize_Height - 1);
                }
            }

            bonus_X = random_x;
            bonus_Y = random_y;
            Pixel_Map.mapPixel[bonus_Y, bonus_X] = 2;

            return;
        } // 보너스 생산

        public void check_Bonus() // 보너스 체크
        {
            if (Warms_total.warm_parts[0].x == bonus_X && Warms_total.warm_parts[0].y == bonus_Y) // 머리의 xy좌표와 보너스의 xy좌표가 같으면
            {
                frame_count = 0;
                count_eat_bonus++;
                check_eat_bonus = 1;
                make_Bonus();
                Total_score = Total_score + count_eat_bonus * count_eat_bonus * 1000 + 1000;
            }

            return;
        }

        public void new_Bonus() // 새로운 보너스 생산
        {
            check_eat_bonus_count++;
            Warms_total.make_tail();

            if (check_eat_bonus_count == 3)
            {
                check_eat_bonus_count = 0;
                check_eat_bonus = 0;
            }
        }

        public void future_see() // 지렁이가 죽었는지 살았는지 확인
        {
            if (frame_count > Setting.Instance().최소프레임 * MainForm._Generation + 200)
                dead = 1;
            if (Warms_total.moveState == 1 && (
                 Pixel_Map.mapPixel[Warms_total.warm_parts[0].y, Warms_total.warm_parts[0].x - 1] == 1
                 ||
                 Pixel_Map.mapPixel[Warms_total.warm_parts[0].y, Warms_total.warm_parts[0].x - 1] == 4)
                 ) dead = 1;
            else if (Warms_total.moveState == 2 && (Pixel_Map.mapPixel[Warms_total.warm_parts[0].y, Warms_total.warm_parts[0].x + 1] == 1 || Pixel_Map.mapPixel[Warms_total.warm_parts[0].y, Warms_total.warm_parts[0].x + 1] == 4)) dead = 1;
            else if (Warms_total.moveState == 3 && (Pixel_Map.mapPixel[Warms_total.warm_parts[0].y - 1, Warms_total.warm_parts[0].x] == 1 || Pixel_Map.mapPixel[Warms_total.warm_parts[0].y - 1, Warms_total.warm_parts[0].x] == 4)) dead = 1;
            else if (Warms_total.moveState == 4 && (Pixel_Map.mapPixel[Warms_total.warm_parts[0].y + 1, Warms_total.warm_parts[0].x] == 1 || Pixel_Map.mapPixel[Warms_total.warm_parts[0].y + 1, Warms_total.warm_parts[0].x] == 4)) dead = 1;
        }

        public bool Getboolean()
        {
            if (Preceed_dist - Present_dist < 0)
                return true;
            else
                return false;
        }

        public double GetDist()
        {
            double dist;

            dist = Math.Sqrt(Math.Pow(Warms_total.warm_parts[0].x - bonus_X, 2) + Math.Pow(Warms_total.warm_parts[0].y - bonus_Y, 2));

            if (Getboolean())
                Total_score += 1;
            else
                Total_score -= 1.5;

            return dist;
        }

        public void WarmAction()
        {
            frame_count++;
            future_see();

            if (dead != 1)
            {
                Pixel_Map.Map_data(Warms_total, bonus_X, bonus_Y);

                Preceed_dist = GetDist();
                Warms_total.move_monster();

                int warm_x = Warms_total.warm_parts[0].x;
                int warm_y = Warms_total.warm_parts[0].y;
                if (MemoryStorage[warm_y, warm_x] != 1)
                    MemoryStorage[warm_y, warm_x] = 1;

                Present_dist = GetDist();

                check_Bonus();

                if (check_eat_bonus != 0)
                {
                    new_Bonus();
                }
                Pixel_Map.pixel_draw();
            }
            else
            {
                if (dead == 1 && check_dead == 0)
                {
                    if (Total_score < 0)
                        Total_score = 0;
                    _Genetic.score += Total_score;
                    check_dead = 1;
                }

                Point centerPos = new Point(Warms_total.warm_parts[0].x * blocksize, Warms_total.warm_parts[0].y * blocksize);
                Font _font = new System.Drawing.Font("dotum", 12, FontStyle.Bold);
                TextRenderer.DrawText(GDIBuffer.Instance().getGraphics, String.Format("{0}", _Genetic.score), _font, centerPos, Color.LightSkyBlue);
            }
        }

        public int find_max()
        {
            double temp = 0;

            int check_move = 0;

            if (turn_left > temp && Warms_total.moveState != 2)
            {
                temp = turn_left;
                check_move = 1;
            }
            if (turn_right > temp && Warms_total.moveState != 1)
            {
                temp = turn_right;
                check_move = 2;
            }
            if (up > temp && Warms_total.moveState != 4)
            {
                temp = up;
                check_move = 3;
            }
            if (down > temp && Warms_total.moveState != 3)
            {
                temp = down;
                check_move = 4;
            }

            return check_move;
        }

        public int[] FindOb()
        {
            int[] d1arr = new int[4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < Setting.Instance().mapSize_Height; j++)
                {
                    if (i == 0 &&
                        (Pixel_Map.mapPixel[Warms_total.warm_parts[0].y, Warms_total.warm_parts[0].x + (j + 1)] == 1
                        || Pixel_Map.mapPixel[Warms_total.warm_parts[0].y, Warms_total.warm_parts[0].x + (j + 1)] == 4))
                    {
                        d1arr[i] = j + 1;
                        break;
                    }
                    if (i == 1 &&
                        (Pixel_Map.mapPixel[Warms_total.warm_parts[0].y, Warms_total.warm_parts[0].x - (j + 1)] == 1
                        || Pixel_Map.mapPixel[Warms_total.warm_parts[0].y, Warms_total.warm_parts[0].x - (j + 1)] == 4))
                    {
                        d1arr[i] = j + 1;
                        break;
                    }
                    if (i == 2 &&
                        (Pixel_Map.mapPixel[Warms_total.warm_parts[0].y + (j + 1), Warms_total.warm_parts[0].x] == 1
                        || Pixel_Map.mapPixel[Warms_total.warm_parts[0].y + (j + 1), Warms_total.warm_parts[0].x] == 4))
                    {
                        d1arr[i] = j + 1;
                        break;
                    }
                    if (i == 3 &&
                        (Pixel_Map.mapPixel[Warms_total.warm_parts[0].y - (j + 1), Warms_total.warm_parts[0].x] == 1
                        || Pixel_Map.mapPixel[Warms_total.warm_parts[0].y - (j + 1), Warms_total.warm_parts[0].x] == 4))
                    {
                        d1arr[i] = j + 1;
                        break;
                    }
                }
            }

            return d1arr;
        }

        public int[] Findbouns()
        {
            int[] d2arr = new int[2];

            d2arr[0] = Warms_total.warm_parts[0].x - bonus_X;
            d2arr[1] = Warms_total.warm_parts[0].y - bonus_Y;

            return d2arr;
        }

        public void countMemoryStorage()
        {
            countMemory = 0;
            for (int i = 0; i < Setting.Instance().mapSize_Height; i++)
                for (int j = 0; j < Setting.Instance().mapSize_Width; j++)
                    if (MemoryStorage[i, j] == 1)
                        countMemory++;

            return;
        }

        public void FrameUpdate()
        {
            float[] MOut = new float[Setting.Instance().히든레이어수]; // 입력부

            findob = FindOb();
            findbouns = Findbouns();
            new_arr = Pixel_Map.inputarr(findob, findbouns);
            countMemoryStorage();

            for (int i = 0; i < Setting.Instance().히든레이어수; i++)
                MOut[i] = _Genetic.Middle_Neurons[i].GetOutput(new_arr);

            turn_left = _Genetic.Output_Neurons[0].GetOutput(MOut);
            turn_right = _Genetic.Output_Neurons[1].GetOutput(MOut);
            down = _Genetic.Output_Neurons[2].GetOutput(MOut);
            up = _Genetic.Output_Neurons[3].GetOutput(MOut);

            Warms_total.moveState = find_max();
        }
    }
}

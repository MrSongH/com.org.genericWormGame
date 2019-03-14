using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace GDIBuffer_Sample_2012_07_25
{
    [Serializable]
    public class Gen
    {
        public Neuron[] Middle_Neurons = new Neuron[Setting.Instance().히든레이어수]; // 히든뉴런
        public Neuron[] Output_Neurons = { // 출력뉴런
            new Neuron(Setting.Instance().히든레이어수),new Neuron(Setting.Instance().히든레이어수),
            new Neuron(Setting.Instance().히든레이어수),new Neuron(Setting.Instance().히든레이어수)
        };

        public double score = 1;
        
        public Gen()
          {
            for(int i = 0; i<Setting.Instance().히든레이어수; i++)
            {
                 Middle_Neurons[i] = new Neuron(6); // 7은 임시값
            }
        }
        
        public Gen Clone()
        {
            Gen new_Gen = new Gen();
            
            new_Gen.Middle_Neurons = new Neuron[Middle_Neurons.Length];

            for(int i = 0; i<Middle_Neurons.Length; i++)
            {
                new_Gen.Middle_Neurons[i] = Middle_Neurons[i].Clone();
            }
            
            new_Gen.Output_Neurons = new Neuron[Output_Neurons.Length];

            for (int i = 0; i < Output_Neurons.Length; i++)
            {
                new_Gen.Output_Neurons[i] = Output_Neurons[i].Clone();
            }

            new_Gen.score = score;

            return new_Gen;
        }
    }

    static class Generic
    {
        public static float get_float(String number)           // Binary To Float
        {
            float tmp = 0;
            float point_n = 0;

            byte i = 0;
            bool sig = false;
            int exp = 0;
            String real = "";
            String r_real = "";

            foreach (char x in number)
            {
                if (i == 0)
                {
                    if (x == '0') sig = false;
                    else sig = true;
                }
                else if (x == ' ') continue;
                else if (i < 9) exp = (exp * 2) + (x - 48);
                else
                {
                    real += x;
                }
                i++;
            }

            exp -= 127;

            if (exp < 0)
            {
                exp++;
                real = "1" + real;
                while (exp != 0)
                {
                    real = "0" + real;
                    exp++;
                }
            }

            else tmp = 1;

            foreach (char r in real)
            {
                if (exp > 0)
                {
                    tmp = (tmp * 2) + (r - 48);
                    exp--;
                }
                else
                {
                    r_real = r + r_real;
                }
            }

            foreach (char k in r_real)
            {
                point_n = (point_n + (k - 48)) / 2;
            }

            tmp += point_n;

            if (sig) return tmp * -1;
            else return tmp;
        }

        public static String get_bin(float number)                 //Float to Binary
        {
            String str_bin = "";

            byte[] bin_num = BitConverter.GetBytes(number);

            foreach (byte b in bin_num)
                str_bin = Convert.ToString(b, 2).PadLeft(8, '0') + str_bin;

            return str_bin;
        }

        public static List<Trainer> check(List<Trainer> _Trainer, int num)  // 선별
        {
            List<Trainer> _temp = new List<Trainer>(); // 우수한 지렁이들을 임시 저장할 리스트 선언
            double[] percentage = new double[_Trainer.Count]; // 지렁이들의 확률 저장할 배열 선언

            _Trainer.Sort((Trainer x, Trainer y) => y._Genetic.score.CompareTo(x._Genetic.score)); // 람-다 식 => 웜즈 객체 2개의 스코어값을 기준으로 비교후 정렬하라.
            double ScoreSum = _Trainer.Select(x => x._Genetic.score).Sum(); // 확률의 분모 값을 정의하기 위해

            for (int i = 0; i < _Trainer.Count; i++) // 확률분포 계산
            {
                if (i != 0) // i가 0이 아니면 
                {
                    percentage[i] = percentage[i - 1];
                }
                else // 
                {
                    percentage[0] = 0;
                }
                percentage[i] += _Trainer[i]._Genetic.score / (double)ScoreSum;
            }

            percentage[_Trainer.Count - 1] = 1;

            for (int i = 0; i < num; i++)
            {
                double select = MainForm.random.NextDouble();
                for (int j = 0; j < _Trainer.Count; j++)
                {
                    if (percentage[j] > select)
                    {
                        _temp.Add(_Trainer[j].Clone());
                        break;
                    }
                }
            }

            return _temp;
        }

        public static List<Trainer> repList(List<Trainer> worm, int Evennum) // 두개의 뉴런값을 섞
        {
            List<Trainer> temp = new List<Trainer>();                       // 우월한 유전자들 끼리 섞는 함수
            Random r = MainForm.random;

            int 교배수 = Setting.Instance().교배횟수;

            for (int k = 0; k < Evennum / 2; k++)
            {
                Trainer child1 =new Trainer(Setting.Instance().mapSize_Width, Setting.Instance().mapSize_Height, Setting.Instance().블럭크기);
                child1.train_setting();
                Trainer child2 = new Trainer(Setting.Instance().mapSize_Width, Setting.Instance().mapSize_Height, Setting.Instance().블럭크기);
                child2.train_setting();

                child1._Genetic = worm[k % worm.Count]._Genetic.Clone(); // 부 유전자
                child2._Genetic = worm[r.Next(0, worm.Count - 1)]._Genetic.Clone(); // 모 유전자
                child1._Genetic.score = 1;
                child2._Genetic.score = 1;

                for(int i=0; i<child1._Genetic.Middle_Neurons.Length; i++)
                {
                    for(int j=0; j<child1._Genetic.Middle_Neurons[i].Weighted_value.Length; j++)
                    {
                        if(r.Next(0, 100) < 40) 
                        replaced(ref child1._Genetic.Middle_Neurons[i].Weighted_value[j], ref child2._Genetic.Middle_Neurons[i].Weighted_value[j], 교배수);
                    } // ref는 사용하기 전에 초기화
                    replaced(ref child1._Genetic.Middle_Neurons[i].Threshold_value, ref child2._Genetic.Middle_Neurons[i].Threshold_value, 교배수);
                }
                
                for (int i = 0; i < child1._Genetic.Output_Neurons.Length; i++)
                {
                    for (int j = 0; j < child1._Genetic.Output_Neurons[i].Weighted_value.Length; j++)
                        if (r.Next(0, 100) < 40)
                            replaced(ref child1._Genetic.Output_Neurons[i].Weighted_value[j], ref child2._Genetic.Output_Neurons[i].Weighted_value[j], 교배수);

                    replaced(ref child1._Genetic.Output_Neurons[i].Threshold_value, ref child2._Genetic.Output_Neurons[i].Threshold_value, 교배수);
                }

                temp.Add(child1.Clone());
                temp.Add(child2.Clone());
            }

            return temp;
        }

        public static void replaced(ref float g1, ref float g2, int many) //  돌연변이
        {
            String gen1 = get_bin(g1);
            String gen2 = get_bin(g2);
            Random r = MainForm.random;

            int 돌연변이교환수 = Setting.Instance().돌연변이교환횟수;
            int 돌연변이발생확률 = Setting.Instance().돌연변이확률;

            if (g1 != g2)
            {
                char temp;
                char[] father_gen = gen1.ToCharArray(); // 아버지 유ㅜ전자
                char[] mother_gen = gen2.ToCharArray(); // 어머니 유전자

                int select = r.Next(1, 31-many); // 교배할 영역이라고 써있는데 29 의미 모르겠음

                for (int j = 0; j < many; j++)
                {
                    temp = father_gen[select + j];
                    father_gen[(select + j)] = mother_gen[(select + j) % 32];
                    mother_gen[(select + j)] = temp;
                }
                g1 = get_float(new String(father_gen));
                g2 = get_float(new String(mother_gen));
                if (r.Next(0, 99) < 돌연변이발생확률) mutation(ref g1, 돌연변이교환수);
                if (r.Next(0, 99) < 돌연변이발생확률) mutation(ref g2, 돌연변이교환수);
            }
            else // 아버지 노드와 어머니 노드가 동일할 때 -> 근친방지코드
            {
                if (r.Next(0, 99) < 돌연변이발생확률 * 3) mutation(ref g1, 돌연변이교환수);
                if (r.Next(0, 99) < 돌연변이발생확률 * 3) mutation(ref g2, 돌연변이교환수);
            }
        }

        public static void mutation(ref float g1, int many)
        {
            Random r = MainForm.random;
            String dna1 = get_bin(g1);

            char[] dna_temp = dna1.ToCharArray();

            for (int i = 0; i < many; i++)
            {
                dna_temp[r.Next(0, 31)] = Convert.ToChar(r.Next(0, 1));
            }

            g1 = get_float(new String(dna_temp));
        }
    }
}

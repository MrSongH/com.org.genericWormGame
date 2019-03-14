using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDIBuffer_Sample_2012_07_25
{
    public class Neuron
    {
        public float[] Weighted_value; // 가중치
        public float Threshold_value; // 임계치
        public int inputSize = 0;

        public Neuron(int size)
        {
            inputSize = size;

            RandNeuron();
        }
        public Neuron(float Threshold, params float[] w)
        {
            float[] inW = new float[w.Length];
            for (int i = 0; i < w.Length; i++)
                inW[i] = w[i];
            Threshold_value = Threshold;
        }

        public Neuron Clone() // 뉴런을 복제하기 위해 필요한 함수
        {
            Neuron new_Neuron = new Neuron(inputSize); // 뉴런 복제를 위해 새로운 뉴런 생성

            float[] New_Weighted_value = new float[inputSize]; // 원본 뉴런의 가중치를 저장할 배열 선언

            for (int i = 0; i < inputSize; i++)
            {
                New_Weighted_value[i] = Weighted_value[i]; // 임시 배열에 가중치 복사
            }
            new_Neuron.Weighted_value = New_Weighted_value; // 복제한 뉴런에 원본의 가중치 부여

            new_Neuron.Threshold_value = Threshold_value; // 복제한 뉴런에 원본의 임계치 부여

            return new_Neuron; // 새로만든 뉴런객체로 리턴
        }

        public void RandNeuron()
        {
            Random random = MainForm.random; // 가중치 랜덤 초기화
            Weighted_value = new float[inputSize];

            for (int i = 0; i < inputSize; i++)
            {
                Weighted_value[i] = (float)(random.NextDouble() * 2.0 - 1.0);
            }

            Threshold_value = (float)(random.NextDouble() * 2.0 - 1.0);

        }

        public float GetOutput(params int[] values) 
        {
            float sum = 0;
            for (int i = 0; i < inputSize; i++)
            {
                if (i < values.Length) sum += values[i] * Weighted_value[i];
                else sum += 0;
            }
            sum -= Threshold_value;
            return Sigmoid(sum);
        }

        public float GetOutput(params float[] values)
        {
            float sum = 0;
            for (int i = 0; i < inputSize; i++)
            {
                if (i < values.Length) sum += values[i] * Weighted_value[i];
                else sum += 0;
            }
            sum -= Threshold_value;
            return Sigmoid(sum);
        }

        public float GetOutSum(params float[] values)
        {
            float sum = 0;

            for (int i = 0; i < inputSize; i++)
            {
                if (i < values.Length) sum += values[i] * Weighted_value[i];
                else sum += 0;
            }
            sum -= Threshold_value;
            return sum;
        }
        private float Sigmoid(float u)
        {
            return (float)(1.0 / (1.0 + Math.Exp(-u)));
        }
    }
}

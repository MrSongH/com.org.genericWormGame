using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDIBuffer_Sample_2012_07_25
{
    class  Setting
    {
        public static Setting st;
        public int 히든레이어수;
        public int 교배횟수;
        public int 모집단;
        public int 테스트단위;
        public int 돌연변이확률;
        public int 돌연변이교환횟수;
        public int 체크포인트가중치;
        public int mapSize_Width;
        public int mapSize_Height;
        public int 최대프레임;
        public int 최소프레임;
        public int 블럭크기;
        public int 저장간격;
        public int 저장개체수;
        public String 저장위치;

        public Setting()
        {
            모집단 = 500;
            히든레이어수 = 10;
            테스트단위 = 50;
            교배횟수 = 3;
            돌연변이확률 = 4;
            돌연변이교환횟수 = 2;
            체크포인트가중치 = 1;
            mapSize_Width = 50;
            mapSize_Height =50;
            최대프레임 = 3000;
            최소프레임 = 10;
            블럭크기 = 10;
            저장간격 = 5;
            저장개체수 = 10;
            저장위치 = "";
        }

        public static Setting Instance()
        {
            if (st == null)
            {
                st = new Setting();
            }

            return st;
        }

        public static Setting Instance(Setting _st)
        {
            st = _st;

            return st;
        }
    }
}

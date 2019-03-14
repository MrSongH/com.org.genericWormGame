using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GDIBuffer_Sample_2012_07_25
{
    public class pixel_Map
    {
        public int mapWidth { get; set; }
        public int mapHeight { get; set; }
        public int blockSize { get; set; }
        public int[,] mapPixel { get; set; }
        public int[] new_arr;

        public pixel_Map() { }       
        
        public pixel_Map(int Width, int Height, int blockSize)
        {
            this.mapWidth = Width;
            this.mapHeight = Height;
            this.blockSize = blockSize;
            mapPixel = new int[Setting.Instance().mapSize_Width, Setting.Instance().mapSize_Height];

            for (int i = 0; i < Setting.Instance().mapSize_Width; i++)
                for (int j = 0; j < Setting.Instance().mapSize_Height; j++)
                    mapPixel[i, j] = 4;
            for (int i = 1; i < Setting.Instance().mapSize_Width - 1; i++)
                for (int j = 1; j < Setting.Instance().mapSize_Height - 1; j++)
                    mapPixel[i, j] = 0;
        }
        
        public void Map_data(Warm warm_total,int bonus_X,int bonus_y)
        {
            for (int i = 1; i < Setting.Instance().mapSize_Width- 1; i++)
                for (int j = 1; j < Setting.Instance().mapSize_Height- 1; j++)
                    mapPixel[i, j] = 0;
            for (int i =0;i< warm_total.warm_parts.Count;i++)
            {
                if (i == 0)
                    mapPixel[warm_total.warm_parts[i].y, warm_total.warm_parts[i].x] = 3;
                else
                    mapPixel[warm_total.warm_parts[i].y, warm_total.warm_parts[i].x] = 1;
            }

            mapPixel[bonus_y, bonus_X] = 2;
        }
        
        public void pixel_draw()
        {
            for (int i = 0; i < Setting.Instance().mapSize_Height; i++)
                for (int j = 0; j < Setting.Instance().mapSize_Width; j++)
                {
                    if (mapPixel[i, j] == 3)
                    {
                        GDIBuffer.Instance().getGraphics.DrawRectangle(new Pen(Brushes.Orange, 1), j * blockSize, i * blockSize, blockSize, blockSize);
                        GDIBuffer.Instance().getGraphics.FillRectangle(new SolidBrush(Color.Orange), j * blockSize, i * blockSize, blockSize, blockSize);
                    }
                    if (mapPixel[i, j] == 1)
                    {
                        GDIBuffer.Instance().getGraphics.DrawRectangle(new Pen(Brushes.Green, 1), j * blockSize, i * blockSize, blockSize, blockSize);
                        GDIBuffer.Instance().getGraphics.FillRectangle(new SolidBrush(Color.Green), j * blockSize, i * blockSize, blockSize, blockSize);
                    }
                    if (mapPixel[i, j] == 4)
                    {
                        GDIBuffer.Instance().getGraphics.DrawRectangle(new Pen(Brushes.Black, 1), j * blockSize, i * blockSize, blockSize, blockSize);
                        GDIBuffer.Instance().getGraphics.FillRectangle(new SolidBrush(Color.Black), j * blockSize, i * blockSize, blockSize, blockSize);
                    }
                    if (mapPixel[i, j] == 2)
                    {
                        GDIBuffer.Instance().getGraphics.DrawRectangle(new Pen(Brushes.Blue, 1), j * blockSize, i * blockSize, blockSize, blockSize);
                        GDIBuffer.Instance().getGraphics.FillRectangle(new SolidBrush(Color.Blue), j * blockSize, i * blockSize, blockSize, blockSize);
                    }
                }
        }

        public int[] mapObstacle()
        {
            int []d1arr = new int[(Setting.Instance().mapSize_Width) * (Setting.Instance().mapSize_Height)];

            for (int i = 0; i < Setting.Instance().mapSize_Width; i++)
                for (int j = 0; j < Setting.Instance().mapSize_Height; j++)
                    if (mapPixel[i, j] == 1 || mapPixel[i, j] == 4)
                        d1arr[i* Setting.Instance().mapSize_Width + j] = 1;

            return d1arr;
        }

        public int[] bonusCordinate(int bonus_X, int bonus_y)
        {
            int[] d1arr = new int[(Setting.Instance().mapSize_Width) + (Setting.Instance().mapSize_Height)];

            for (int i = 0; i < Setting.Instance().mapSize_Width; i++)
                if (bonus_X == i)
                    d1arr[i] = 1;

            for (int j = 0; j < Setting.Instance().mapSize_Height; j++)
                if (bonus_y == j)
                    d1arr[Setting.Instance().mapSize_Width + j] = 1;

            return d1arr;
        }

        public int[] inputarr(int[] mapObstacle, int[] bonusCordinate)
        {
            int[] d1arr = new int[6];

            for (int i = 0; i < 4; i++)
                d1arr[i] = mapObstacle[i];

            for (int i = 0; i < 2; i++)
                d1arr[4 + i] = bonusCordinate[i];

            return d1arr;
        }
    }
}

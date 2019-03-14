using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDIBuffer_Sample_2012_07_25
{
    public  class worm_part
    {
        public int x { set; get; }
        public int y { set; get; }
        public worm_part(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public class Warm
    {
        public enum warmState : int { stop = 0, left, right, up, down }
        public int moveState = (int)warmState.stop;
        public List<worm_part> warm_parts = new List<worm_part>();
        public int mapWidth { get; set; }
        public int mapHeight { get; set; }
        public int blocksize { get; set; }
        worm_part Beforewarm;
        public int[] head_location;

        public void make_tail()
        {
            warm_parts.Add(Beforewarm);
            return;
        }

        public void make_head(int x, int y)
        {
            worm_part head = new worm_part(x, y);
            moveState = 0;
            warm_parts.Add(head);

        }
        public worm_part input_data(worm_part A)
        {
            worm_part copy_warm = new worm_part(A.x, A.y);
            return copy_warm;
        }
   
       
        public void follow_head()
        {
            for (int i = warm_parts.Count -1 ; i > 0; i--)
            {
                warm_parts[i] = input_data(warm_parts[i - 1]);
            }
        }

        public void move_monster()
        {

            follow_head();

            Beforewarm = input_data(warm_parts[warm_parts.Count-1]);


            switch (moveState)
            {
                case (int)warmState.stop:
                    break;
                case (int)warmState.left:
                    warm_parts[0].x -= 1;
                    break;
                case (int)warmState.right:
                    warm_parts[0].x += 1;
                    break;
                case (int)warmState.up:
                    warm_parts[0].y -= 1;
                    break;
                case (int)warmState.down:
                    warm_parts[0].y += 1;
                    break;
            }
            return;
        }

/*
        public int check_dead()
        {
            int dead = 0;
            if(warm_parts[0].x < 2 || warm_parts[0].x > mapWidth/blocksize-3 || warm_parts[0].y < 2 || warm_parts[0].y > mapHeight / blocksize - 3) dead = 1;
            for (int i = 1; i < warm_parts.Count; i++)
                if (warm_parts[0].x == warm_parts[i].x && warm_parts[0].y == warm_parts[i].y) dead = 1;
            return dead;
        }
*/


    }
}

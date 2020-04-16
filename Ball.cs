using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace CrazyBalls
{
    class Ball
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public static int BallD = 35;
        public static int speed = 40;
        public Color BallColor;
        public bool ready;
        //public bool IsAlive { get { return t != null && t.IsAlive; } }
        public int Num { get; private set; }

        private int dx, dy;
        private Random rand = new Random();

        private Thread t;
        private bool stop;
        public Ball(int way)
        {
            ready = false;
            stop = true;
            Num = way;
            switch (way)
            {
                case 0:
                    X = -20;
                    Y = rand.Next(0, Animator.Height);
                    BallColor = Color.FromArgb(rand.Next(40,255),255, 0, 0);
                    break;
                case 1:
                    X = rand.Next(0, Animator.Width);
                    Y = -20;
                    BallColor = Color.FromArgb(rand.Next(40, 255), 0, 255, 0);
                    break;
                case 2:
                    X = Animator.Width + 20;
                    Y = rand.Next(0, Animator.Height);
                    BallColor = Color.FromArgb(rand.Next(40, 255), 0, 0, 255);
                    break;
            }
            dx = rand.Next(2,5);
            dy = rand.Next(1,3);
        }

        public void Start()
        {
            if (t == null || !t.IsAlive)
            {
                stop = false;
                ThreadStart th = new ThreadStart(Move);
                t = new Thread(th);
                t.Start();
            }
        }
        private void Move()
        {
            while (!stop)
            {
                Thread.Sleep(speed);
                if ((X >= Ring.Area.X) && (X <= Ring.Area.X + Ring.Area.Width) && (Y >= Ring.Area.Y) && (Y <= Ring.Area.Y + Ring.Area.Height))
                {
                    ready = true;
                    Stop();
                }
                if (X < Ring.Area.X+(Ring.Area.Width*0.35))
                {
                    X += dx;
                }
                else if (X > (Ring.Area.X + Ring.Area.Width*0.65))
                {
                    X -= dx;
                }
                if (Y < (Ring.Area.Y +Ring.Area.Y* 0.05))
                {
                    Y += dy;
                }
                else if(Y > (Ring.Area.Y + Ring.Area.Height *0.45))
                {
                    Y -= dy;
                }
            }
        }
        public void Stop()
        {
            stop = true;
        }
        public void Abort()
        {
            try
            {
                stop = false;
                t.Abort();
                t.Join();
                
            }
            catch (Exception e)
            {
            }
            finally
            {
                t = null;
            }
        }
    }
}

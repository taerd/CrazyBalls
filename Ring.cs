using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace CrazyBalls
{
    class Ring
    {
        public static Rectangle Area { get; set; }
        public Color color;
        public int X { get; private set; }
        public int Y { get; private set; }
        private int maxRadius
        {
            get { return (Animator.Width > Animator.Height) ? Animator.Width : Animator.Height; }
        }
        public int Radius { get; private set; }

        private bool stop;
        private Thread t;
        public bool IsAlive { get { return t != null && t.IsAlive; } }

        public Ring(Color cl)
        {
            color = cl;
            Radius = 0;
            X = Area.X + Area.Width / 2;
            Y = Area.Y + Area.Height / 2;
        }
        public void Start()
        {
            if (t == null || !t.IsAlive)
            {
                stop = false;
                ThreadStart th = new ThreadStart(Enc);
                t = new Thread(th);
                t.Start();
            }
        }
        private void Enc()
        {
            while (!stop)
            {
                if(Radius > maxRadius)
                {
                    Stop();
                }
                Radius += 2;
                X -= 2;
                Y -= 2;
                Thread.Sleep((int)(Ball.speed/2));
                int alpha = Math.Abs((int)((1.0 - (float)Radius / maxRadius) * 255)) % 255;
                color = Color.FromArgb(
                    (int)(alpha),
                    color.R,
                    color.G,
                    color.B
                );
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
                stop = true;
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

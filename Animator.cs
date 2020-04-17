using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace CrazyBalls
{
    class Animator
    {
        public static int Height { get; private set; }
        public static int Width { get; private set; }

        private BufferedGraphics bg;
        private Graphics mainG, bgg;
        private Pen p = new Pen(Color.Black);
        private Brush br;

        private object obj = new object();
        private bool bgChanged = false;
        private bool stop;


        private Thread t;

        private Producer[] producers;
        private Consumer consumer;
        private bool[] ready = new bool[3];

        public Animator(Graphics g, Rectangle r,Producer[] prod,Consumer cons)
        {
            producers = prod;
            consumer = cons;
            for (int i = 0; i < 3; i++)
            {
                ready[i] = false;
            }
            Update(g, r);
        }
        public void Update(Graphics g, Rectangle r)
        {
            mainG = g;
            Width = r.Width;
            Height = r.Height;
            Ring.Area = new Rectangle((int)(Width * 0.45), (int)(Height * 0.4), (int)(Width * 0.1), (int)(Height * 0.2));
            Monitor.Enter(obj);
            bgChanged = true;
            bg = BufferedGraphicsManager.Current.Allocate(
                mainG,
                new Rectangle(0, 0, Width, Height)
            );
            Monitor.PulseAll(obj);
            Monitor.Exit(obj);
        }
        private void Animate()
        {
            int cnt=0;
            while (!stop)
            {
                Thread.Sleep( (int) (Ball.speed));

                if (bgChanged)
                {
                    Monitor.Enter(obj);
                    bgChanged = false;
                    bgg = bg.Graphics;
                    Monitor.PulseAll(obj);
                    Monitor.Exit(obj);
                }
                bgg.Clear(Color.White);
                bgg.DrawRectangle(p, Ring.Area);

                //прорисовка колец и удаление лишних
                for (int i = 0; i < 3; i++)
                {
                    cnt = producers[i].balls.Count;
                    if(cnt != 0)
                    {
                        ready[i] = producers[i].balls[0].ready;
                    }   
                }
                
                if(ready[0] && ready[1] && ready[2])
                {
                    consumer.CreateRing();
                    for(int i = 0; i < 3; i++)//удаление шариков с вершин
                    {
                        Monitor.Enter(producers[i].balls);
                        producers[i].balls.RemoveAt(0);
                        Monitor.PulseAll(producers[i].balls);
                        Monitor.Exit(producers[i].balls);
                    }
                }

                cnt = consumer.rings.Count;
                for(int i = 0; i < cnt; i++) 
                {
                    if (consumer.rings.Count != 0)
                    {
                        if (consumer.rings[i].IsAlive)
                        {
                            br = new SolidBrush(consumer.rings[i].color);
                            bgg.FillEllipse(br, consumer.rings[i].X, consumer.rings[i].Y, consumer.rings[i].Radius * 2, consumer.rings[i].Radius * 2);
                        }
                        else
                        {
                            Monitor.Enter(consumer.rings);
                            consumer.rings.Remove(consumer.rings[i]);
                            /*
                            if (consumer.rings.Count != 0)
                            {
                                consumer.rings.Remove(consumer.rings[i]);
                            }
                            */
                            Monitor.PulseAll(consumer.rings);
                            Monitor.Exit(consumer.rings);
                            i--;
                            cnt--;
                        }
                    }
                }

                //прорисовка шариков

                for (int i = 0; i < 3; i++)
                {
                    cnt = producers[i].balls.Count;
                    
                    if (cnt == 0)
                    {
                        producers[i].CreateBall();
                    }
                    else if (producers[i].balls[cnt-1].ready)//тк как отчитыватся с 0
                    {
                        producers[i].CreateBall();
                    }
                    br = new SolidBrush(producers[i].balls[0].BallColor);
                    foreach(var b in producers[i].balls)
                    {
                        bgg.FillEllipse(br, b.X, b.Y, Ball.BallD, Ball.BallD);
                        bgg.DrawEllipse(p, b.X, b.Y, Ball.BallD, Ball.BallD);
                    }
                }

                Monitor.Enter(obj);
                if (!bgChanged)
                {
                    try
                    {
                        bg.Render();
                    }
                    catch (Exception e)
                    {
                    }
                }
                Monitor.Exit(obj);
            }
        }
        public void Start()
        {
            if (t == null || !t.IsAlive)
            {
                stop = false;
                ThreadStart th = new ThreadStart(Animate);
                t = new Thread(th);
                t.Start();
            }
        }
        public void Abort()
        {
            try
            {
                stop = false;
                consumer.Stop();
                for (int i = 0; i < 3; i++)
                {
                    producers[i].Stop();
                }
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

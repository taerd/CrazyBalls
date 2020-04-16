using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CrazyBalls
{
    class Producer
    {
        public int Num { get; private set; }
        public List<Ball> balls = new List<Ball>();
        public Producer(int num)
        {
            Num = num;
        }
        public void CreateBall()
        {
            Monitor.Enter(balls);
            Ball b = new Ball(Num);
            b.Start();
            balls.Add(b);
            Monitor.PulseAll(balls);
            Monitor.Exit(balls);
        }
        public void Stop()
        {
            Monitor.Enter(balls);
            foreach (var b in balls)
            {
                b.Abort();
            }
            balls.Clear();
            Monitor.Exit(balls);
        }
    }
}

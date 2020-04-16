using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace CrazyBalls
{
    class Consumer
    {
        public List<Ring> rings = new List<Ring>();
        private Random rand = new Random();
        public Consumer()
        {
        }
        public void CreateRing()
        {
            Monitor.Enter(rings);
            Ring r = new Ring(Color.FromArgb(rand.Next(40,255),rand.Next(40,255),rand.Next(40,255)));
            r.Start();
            rings.Add(r);
            Monitor.PulseAll(rings);
            Monitor.Exit(rings);
        }
        public void Stop()
        {
            Monitor.Enter(rings);
            foreach (var r in rings)
            {
                r.Abort();
            }
            rings.Clear();
            Monitor.Exit(rings);
        }
    }
}

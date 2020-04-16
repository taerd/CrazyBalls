using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace CrazyBalls
{
    public partial class Form1 : Form
    {
        private Animator a;
        private Producer[] producers = new Producer[3];
        private Consumer cons;
        private Thread t;
        public Form1()
        {
            InitializeComponent();
            Ring.Area = new Rectangle((int)(panelMain.Width * 0.45), (int)(panelMain.Height * 0.4), (int)(panelMain.Width * 0.1), (int)(panelMain.Height * 0.2));
            for (int i = 0; i < 3; i++)
            {
                producers[i] = new Producer(i);
            }
            cons = new Consumer();
            a = new Animator(panelMain.CreateGraphics(), panelMain.ClientRectangle,producers,cons);
        }

        private void panelMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Stop();
            }
            else if (e.Button == MouseButtons.Left)
            {
                Start();
            }
        }
        private void Move()
        {
            a.Start();
        }
        private void Start()
        {
            if (t == null || !t.IsAlive)
            {
                ThreadStart th = new ThreadStart(Move);
                t = new Thread(th);
                t.Start();
            }
        }
        private void Stop()
        {
            a.Abort();
        }

        private void panelMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != System.Windows.Forms.FormWindowState.Minimized && a != null)
            {
                a.Update(panelMain.CreateGraphics(), panelMain.ClientRectangle);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            a.Abort();
        }
    }
}

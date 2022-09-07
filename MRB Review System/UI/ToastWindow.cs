using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QualityDefectsRepair
{
    public partial class ToastWindow : Form
    {
        string _PathFile = string.Empty;
        bool _FlagToClose = true;

        public ToastWindow(string PathFile)
        {
            InitializeComponent();
            DesignRoundObjects();
            _PathFile = PathFile;
            timer1.Start();
        }


        void DesignRoundObjects()
        {
            Region = Region.FromHrgn(DesignGlobals.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            btnOpen.Region = Region.FromHrgn(DesignGlobals.CreateRoundRectRgn(0, 0, btnOpen.Width, btnOpen.Height, 15, 15));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(_FlagToClose) this.Close();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            _FlagToClose = false;
            System.Diagnostics.Process.Start("Explorer.exe", _PathFile);
            _FlagToClose = true;
        }


        Timer t1 = new Timer();
        private void ToastWindow_Load(object sender, EventArgs e)
        {
            Opacity = 0;      //first the opacity is 0

            t1.Interval = 10;  //we'll increase the opacity every 10ms
            t1.Tick += new EventHandler(fadeIn);  //this calls the function that changes opacity 
            t1.Start();
        }

        void fadeIn(object sender, EventArgs e)
        {
            if (Opacity >= 1)
                t1.Stop();   //this stops the timer if the form is completely displayed
            else
                Opacity += 0.05;
        }

        private void ToastWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;    //cancel the event so the form won't be closed

            t1.Tick += new EventHandler(fadeOut);  //this calls the fade out function
            t1.Start();

            if (Opacity == 0)  //if the form is completly transparent
                e.Cancel = false;   //resume the event - the program can be closed
        }

        void fadeOut(object sender, EventArgs e)
        {
            if (Opacity <= 0)     //check if opacity is 0
            {
                t1.Stop();    //if it is, we stop the timer
                Close();   //and we try to close the form
            }
            else
                Opacity -= 0.05;
        }
    }
}

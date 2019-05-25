using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FallingBody
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void myBall1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            myBall1.setRate(0.8);
            myBall1.setV0(1, 1);
            myBall1.setMass(2);
            myBall1.setG(2);
            myBall1.setPoint_H(480, myBall1.Top);
            myBall1.setPoint_X(myBall1.Left);
            myBall1.start();
        }
    }
}

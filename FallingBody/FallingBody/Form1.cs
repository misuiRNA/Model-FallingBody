using Ball_Class;
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
            Coordinate referencePoint = new Coordinate(0, this.Height);
            myBall1.setReferencePoint(referencePoint);
            myBall1.setReboundRate(0.8);
            myBall1.setMass(2);
            Coordinate location = new Coordinate(myBall1.Left - referencePoint.locationX,  myBall1.Top - referencePoint.locationY);
            myBall1.setLocation(location);
            myBall1.setVelocity(new Velocity( 0, 0));
            //myBall1.putForce(new Force(0, 8));

            myBall1.start();
        }
    }
}

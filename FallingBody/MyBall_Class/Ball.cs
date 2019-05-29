using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Ball_Class
{
    public partial class Ball : UserControl
    {
        private const double BALLSIZE = 64;

        private double mass = 0;
        private readonly Coordinate location = new Coordinate(0, 0);
        private readonly Velocity velocity = new Velocity(0, 0);
        private readonly Force force = new Force(0, 0);
        private int movementTime = 0;
        private int time_Y = 0;

        private readonly Coordinate referencePoint = new Coordinate(0, 0);
        private Point mouseFirstPoint = Point.Empty;
        private delegate void DSetPoint(double H, double X);       //代理，帮助控制该对象在窗体中的位置
        private double reboundRate = 0.6;

        private double startH = 0;
        private double startX = 0;

        public Ball(double mass, Coordinate reference) {
            setMass(mass);
            InitializeComponent();
            initPara(reference);
        }

        private void initPara(Coordinate reference)
        {
            resetForce();
            setReferencePoint(reference);
        }

        public void setMass(double mass) { this.mass = mass; }

        public void setCoordinate(Coordinate c) {
            this.location.locationX = c.locationX;
            this.location.locationY = c.locationY;
        }

        public void setVelocity(Velocity v) {
            this.velocity.velocityX = v.velocityX;
            this.velocity.velocityY = v.velocityY;
        }

        public void putForce(Force f)
        {
            this.force.forceX += f.forceX;
            this.force.forceY += f.forceY;
        }
        //该方法不符合存在施力物体逻辑性错误，待改善
        public void resetForce() {
            putForce(new Force(0, -10 * mass));
        }

        public void setReboundRate(double rate) { this.reboundRate = rate; }

        public void setReferencePoint(Coordinate reference) {
            this.referencePoint.locationY = reference.locationY - BALLSIZE;
            this.referencePoint.locationX = reference.locationX;
        }

        public void setLocation(Coordinate location) {
            this.location.locationX = location.locationX;
            this.location.locationY = location.locationY;
            //物体高度只能从容器顶部定义，所以计算值为负数，为向外部暴露正数接口，将参数取相反数后计算
            this.startH = (- this.location.locationY) - 2 * BALLSIZE;
            this.startX = this.location.locationX;
        }

        private void timeGoing() {
            Thread.Sleep(20);       //休眠一秒
            this.movementTime++;
            this.time_Y++;
        }

        private void restTime_Y() {
            this.time_Y = 0;
        }

        public void start() {
            ThreadStart upcastThreadS = new ThreadStart(upcast);       //数学模型线程
            ThreadStart frashThreadS = new ThreadStart(changePoint);       //显示线程
            Thread upcastThread = new Thread(upcastThreadS);
            Thread frashThread = new Thread(frashThreadS);
            upcastThread.IsBackground = true;
            frashThread.IsBackground = true;
            upcastThread.Start();
            frashThread.Start();
        }
        private void changePoint() {
            DSetPoint dSetPoint = new DSetPoint(setBodyPoint);
            while (startH > 0 || this.velocity.velocityY > 0) {
                Thread.Sleep(10);     //刷新坐标的延迟，要求为timeGoing延迟的一半
                this.Invoke(dSetPoint, location.locationY, location.locationX);
            }
        }
        private void setBodyPoint(double Y, double X) {
            this.Left = Convert.ToInt32(X);
            this.Top = Convert.ToInt32(this.referencePoint.locationY - BALLSIZE - Y);
        }

        public void stop() {
            this.velocity.velocityY = -1;
            this.location.locationY = -1;
        }
        /***********************************************
         * 运动模型与刷新图像
         * ********************************************/
        private void upcast()  {
            double Vy = this.velocity.velocityY;
            double Vx = this.velocity.velocityX;
            double aY = this.force.forceY / mass;
            double aX = this.force.forceX / mass;
            location.locationY = startH;
            location.locationX = startX;
            double derVy0 = 0;
            while (this.velocity.velocityY > 0 || startH > 0)
            {
                while ((location.locationY + Vy) >= 0)
                {
                    location.locationY += Vy;
                    Vy += aY;
                    location.locationX += Vx;
                    Vx += aX;
                    timeGoing();
                }
                location.locationY = 0;
                startH = 0;
                restTime_Y();
                derVy0 = this.velocity.velocityY + (Vy - aY) * reboundRate;
                if (derVy0 > -aY || derVy0 < aY) { this.velocity.velocityY = -Vy * reboundRate; }
                else if (reboundRate == 1.0) { }
                else if (this.velocity.velocityY > 1) { this.velocity.velocityY--; }
                else { this.velocity.velocityY = -1; break; }
                Vy = this.velocity.velocityY;
            }
        }
        
        /*******************************************
         * 该对象实现可拖动属性的系列操作，包括事件方法
         * ***************************************/
        private void MyBall_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.stop();
                mouseFirstPoint = e.Location;
            }
        }
        private void MyBall_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = pointToWorkSpase(new Point(e.Location.X - mouseFirstPoint.X, e.Location.Y - mouseFirstPoint.Y));
                this.Refresh();
            }
        }
        private void MyBall_MouseUp(object sender, MouseEventArgs e)
        {
            this.Location = Location;
            this.Refresh();
        }
        private void rest()
        {
            mouseFirstPoint = Point.Empty;
        }
        private Point pointToWorkSpase(Point p)
        {
            return this.Parent.PointToClient(this.PointToScreen(p));
        }

    }
}

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

        private Point mouseFirstPoint = Point.Empty;
        private delegate void DSetPoint(double H, double X);       //代理，帮助控制该对象在窗体中的位置
        private double rate = 0.6;

        
        /// <summary>
        /// 水平初始移速，向右为正
        /// </summary>
        private double Vx0 = 0;
        
        /// <summary>
        /// 竖直方向初始移速，向上为正
        /// </summary>
        private double Vy0 = 0;
        /// <summary>
        /// 物体的初始高度——对地高度
        /// </summary>
        private double startH = 0;
        /// <summary>
        /// 竖直方向的距离（对地高度——从下向上）......Hr-Hw-Ho=H
        /// </summary>
        private double H = 0;
        /// <summary>
        /// 物体在窗体中的Y方向位置——距窗体顶端位置......Hr-Hw-Ho=H
        /// </summary>
        private double Hw = 0;
        /// <summary>
        /// 参照物对窗体的高度——参照物距窗体顶端的位置......Hr-Hw-Ho=H
        /// </summary>
        private double Hr = 0;
        
        /// <summary>
        /// 水平方向的初始位置
        /// </summary>
        private double startX = 0;
        /// <summary>
        /// 水平方向的距离（从左到右，距离参照物的距离）
        /// </summary>
        private double X = 0;

        public Ball(double mass) {
            setMass(mass);
            InitializeComponent();
            initPara();
        }

        private void initPara()
        {
            Force f = new Force(0, -10 * mass);
            this.putForce(f);
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
            this.force.forceX = 0;
            this.force.forceY = 0;
        }

        /// <summary>
        /// 设置水平初速度
        /// </summary>
        /// <param name="pVx0">水平初速度</param>
        private void setV0_X(double pVx0) { this.Vx0 = pVx0; }
        /// <summary>
        /// 设置竖直方向的初始速度
        /// </summary>
        /// <param name="pVy0">竖直方向初始速度</param>
        private void setV0_H(double pVy0) { this.Vy0 = pVy0; }
        /// <summary>
        /// 设置初始速度——对外可用
        /// </summary>
        /// <param name="pVx0">水平初速度</param>
        /// <param name="pVy0">竖直方向的初始速度</param>
        public void setV0(double pVx0, double pVy0) { setV0_X(pVx0); setV0_H(pVy0); }
        /// <summary>
        /// 设置触地后反弹率
        /// </summary>
        /// <param name="pRate">取值在0-1之间</param>
        public void setRate(double pRate) { this.rate = pRate; }

        /// <summary>
        /// 设置初始水平位置
        /// </summary>
        /// <param name="pStartX">物体初始水平位置</param>
        public void setPoint_X(double pStartX) { this.startX = pStartX; }
        /// <summary>
        /// 设置物体高度相关的取值......Hr-Hw-Ho=H
        /// </summary>
        /// <param name="pHr">参照物对窗体的高度——参照物距窗体顶端的位置</param>
        /// <param name="pHw">物体在窗体中的Y方向位置——距窗体顶端位置</param>
        /// <param name="pHo">物体本身的高度，即Height</param>
        public void setPoint_H(double pHr, double pHw, double pHo = BALLSIZE)
        {
            this.Hw = pHw; 
            this.Hr = pHr;
            this.startH = Hr - Hw - BALLSIZE;         //计算初始位置高度
        }
        



        /****************************************
         * 参数初始化方法
         * *************************************/

        
        /// <summary>
        /// 使时间自增
        /// </summary>
        /// <returns></returns>
        private void timeGoing()
        {
            Thread.Sleep(20);       //休眠一秒
            this.movementTime++;
            this.time_Y++;
        }
        /// <summary>
        /// 将时间归零
        /// </summary>
        private void restTime_Y()
        {
            this.time_Y = 0;
        }

        /// <summary>
        /// 开始运动，由对象建立者调用,在开始之前必须调用setPara对该对象进行初始化
        /// </summary>
        public void start()
        {
            ThreadStart upcastThreadS = new ThreadStart(upcast);       //数学模型线程
            ThreadStart frashThreadS = new ThreadStart(changePoint);       //显示线程
            Thread upcastThread = new Thread(upcastThreadS);
            Thread frashThread = new Thread(frashThreadS);
            upcastThread.IsBackground = true;
            frashThread.IsBackground = true;
            upcastThread.Start();
            frashThread.Start();
        }
        /// <summary>
        /// 外部调用，强行停止小球的运动
        /// </summary>
        public void stop()
        {
            setV0_H(-1);
            this.H = -1;
        }
        /***********************************************
         * 运动模型与刷新图像
         * ********************************************/
        /// <summary>
        /// 更具数学模型建立重物的运动模式，和显示方法
        /// </summary>
        private void upcast()
        {
            double Vy = this.velocity.velocityY;
            double Vx = this.velocity.velocityX;
            double aY = this.force.forceY / mass;
            double aX = this.force.forceX / mass;
            H = startH;
            X = startX;
            Vy = Vy0;
            Vx = Vx0;
            double derVy0 = 0;         //检测相邻两次回弹速度是否符合要求
            while (Vy0 > 0 || startH > 0)
            {
                while ((H + Vy) >= 0)
                {
                    H += Vy;
                    Vy += aY;
                    X += Vx;
                    Vx += aX;
                    timeGoing();
                }
                H = 0;
                startH = 0;
                restTime_Y();
                derVy0 = Vy0 + (Vy - aY) * rate;
                if (derVy0 > -aY || derVy0 < aY) { Vy0 = -Vy * rate; }
                else if (rate == 1.0) { }
                else if (Vy0 > 1) { Vy0--; }
                else { Vy0 = -1; break; }
                Vy = Vy0;
            }
        }
        /// <summary>
        /// 设置重物的显示位置
        /// </summary>
        /// <param name="H">距地高度</param>
        /// <param name="X">水平位置</param>
        private void setBodyPoint(double H, double X)
        {
            this.Left = Convert.ToInt32(X);
            this.Top = Convert.ToInt32(Hr - BALLSIZE - H);
        }
        /// <summary>
        /// 以多线程的方式刷新重物位置
        /// </summary>
        private void changePoint()
        {
            DSetPoint dSetPoint = new DSetPoint(setBodyPoint);
            while (startH > 0 || Vy0 > 0)
            {
                Thread.Sleep(10);     //刷新坐标的延迟，要求为timeGoing延迟的一半
                this.Invoke(dSetPoint, H, X);
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

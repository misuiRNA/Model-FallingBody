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

namespace MyBall_Class
{
    /// <summary>
    /// 重物小球
    /// 可以任意拖动
    /// 使用步骤
    /// 1、新建对象；2、设置参数，调用setPara()；3、设置重物受力,调用setForce()；4、开始抛动重物，调用start()
    /// </summary>
    public partial class MyBall : UserControl
    {
        private Point mouseFirstPoint = Point.Empty;
        private delegate void DSetPoint(double H, double X);       //代理，帮助控制该对象在窗体中的位置
        /// <summary>
        /// 重物的质量——预留
        /// </summary>
        private double mass = 1;
        /// <summary>
        /// 水平方向上受到的力——预留
        /// </summary>
        private double Fx = 0;
        /// <summary>
        /// 竖直方向上受到的力——预留
        /// </summary>
        private double Fh = 0;
        /// <summary>
        /// 反弹率，取值在0-1之间
        /// </summary>
        private double rate = 6.0 / 10;
        /// <summary>
        /// 加速度
        /// </summary>
        private int G = -1;
        /// <summary>
        /// 水平加速度
        /// </summary>
        private double Ax = 0;
        /// <summary>
        /// 竖直方向上的加速度——合力加速度
        /// </summary>
        private double AH = 0;
        /// <summary>
        /// 运动时间
        /// </summary>
        private int time = 0;
        /// <summary>
        /// 运动时间——竖直运动，有回弹时使用
        /// </summary>
        private int time_H = 0;
        /// <summary>
        /// 水平移动速度
        /// </summary>
        private double Vx = 0;
        /// <summary>
        /// 水平初始移速，向右为正
        /// </summary>
        private double Vx0 = 0;
        /// <summary>
        /// 竖直移动速度
        /// </summary>
        private double Vh = 0;
        /// <summary>
        /// 竖直方向初始移速，向上为正
        /// </summary>
        private double Vh0 = 0;
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
        /// 物体本身的高度，即Height......Hr-Hw-Ho=H
        /// </summary>
        private double Ho = 0;
        /// <summary>
        /// 水平方向的初始位置
        /// </summary>
        private double startX = 0;
        /// <summary>
        /// 水平方向的距离（从左到右，距离参照物的距离）
        /// </summary>
        private double X = 0;
        public MyBall()
        {
            InitializeComponent();
            initPara();        //初始化参数
        }

        /*******************************************
         * set方法
         * *****************************************/
        /// <summary>
        /// 设置物体质量
        /// </summary>
        /// <param name="pMass">物体质量</param>
        public void setMass(double pMass) { this.mass = pMass; }
        /// <summary>
        /// 设置物体受力
        /// </summary>
        /// <param name="pFx">水平受力</param>
        /// <param name="pFh">竖直受力</param>
        public void setForce(double pFx, double pFh)
        { this.Fx = pFx; this.Fh = pFh; this.Ax = Fx / mass; this.AH = (Fh / mass) + G; }
        /// <summary>
        /// 设置加速度G
        /// </summary>
        /// <param name="pG">加速度，默认值为1</param>
        public void setG(int pG) { this.G = -pG; }
        /// <summary>
        /// 设置水平初速度
        /// </summary>
        /// <param name="pVx0">水平初速度</param>
        private void setV0_X(double pVx0) { this.Vx0 = pVx0; }
        /// <summary>
        /// 设置竖直方向的初始速度
        /// </summary>
        /// <param name="pVh0">竖直方向初始速度</param>
        private void setV0_H(double pVh0) { this.Vh0 = pVh0; }
        /// <summary>
        /// 设置初始速度——对外可用
        /// </summary>
        /// <param name="pVx0">水平初速度</param>
        /// <param name="pVh0">竖直方向的初始速度</param>
        public void setV0(double pVx0, double pVh0) { setV0_X(pVx0); setV0_H(pVh0); }
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
        public void setPoint_H(double pHr, double pHw, double pHo = 64)
        {
            this.Hw = pHw; this.Ho = pHo; this.Hr = pHr;
            this.startH = Hr - Hw - Ho;         //计算初始位置高度
        }
        /// <summary>
        /// 在开始落体前，设置各项参数
        /// </summary>
        /// <param name="pHr">参照物对窗体的高度——参照物距窗体顶端的位置</param>
        /// <param name="pHw">物体在窗体中的Y方向位置——距窗体顶端位置</param>
        /// <param name="pX">物体水平位置——距窗体左边框距离</param>
        /// <param name="pVx0">重物加速度</param>
        /// <param name="pVh0">重物加速度</param>
        /// <param name="pG">水平初始速度</param>
        /// <param name="pRate">触地后反弹率</param>
        public void setPara(double pHr, double pHw, double pX, double pVx0 = 0, double pVh0 = 0, int pG = 1, double pRate = 0.6)
        {
            this.setG(pG);
            this.setRate(pRate);
            setV0(pVx0, pVh0);
            this.setPoint_H(pHr, pHw, 0.6);
            this.setPoint_X(pX);
        }
        /****************************************
         * get方法
         * **************************************/





        /****************************************
         * 参数初始化方法
         * *************************************/
        /// <summary>
        /// 参数初始化，供内部方法使用
        /// </summary>
        /// <param name="pG">重物加速度</param>
        /// <param name="pVx0">水平初始速度</param>
        /// <param name="pRate">触地后反弹率</param>
        private void initPara(int pG = 1, double pRate = 0.6, double pVx0 = 0, double pVh0 = 0)
        {
            this.setG(pG);
            this.setRate(pRate);
            this.setV0(pVx0, pVh0);
            this.setForce(0, 0);
        }
        /// <summary>
        /// 使时间自增
        /// </summary>
        /// <returns></returns>
        private void timeGoing()
        {
            Thread.Sleep(20); //休眠一秒
            this.time++;
            this.time_H++;
            //return time;
        }
        /// <summary>
        /// 将时间归零
        /// </summary>
        private void restTime()
        {
            this.time_H = 0;
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
            H = startH;
            X = startX;
            Vh = Vh0;
            Vx = Vx0;
            double derVh0 = 0;         //检测相邻两次回弹速度是否符合要求
            while (Vh0 > 0 || startH > 0)
            {
                while ((H + Vh) >= 0)
                {
                    H += Vh;
                    Vh += AH;
                    X += Vx;
                    Vx += Ax;
                    timeGoing();
                }
                H = 0;
                startH = 0;
                restTime();
                derVh0 = Vh0 + (Vh - AH) * rate;
                if (derVh0 > -AH || derVh0 < AH) { Vh0 = -Vh * rate; }
                else if (rate == 1.0) { }
                else if (Vh0 > 1) { Vh0--; }
                else { Vh0 = -1; break; }
                Vh = Vh0;
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
            this.Top = Convert.ToInt32(Hr - Ho - H);
        }
        /// <summary>
        /// 以多线程的方式刷新重物位置
        /// </summary>
        private void changePoint()
        {
            DSetPoint dSetPoint = new DSetPoint(setBodyPoint);
            while (startH > 0 || Vh0 > 0)
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
        /*******************************************************
         * 
         * *****************************************************/







    }
}

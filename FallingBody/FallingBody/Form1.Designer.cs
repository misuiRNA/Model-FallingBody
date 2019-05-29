using Ball_Class;
namespace FallingBody
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Coordinate referencePoint = new Coordinate(0, this.Height);
            this.myBall1 = new Ball_Class.Ball(1, referencePoint);
            this.SuspendLayout();
            // 
            // myBall1
            // 
            this.myBall1.BackColor = System.Drawing.Color.Transparent;
            this.myBall1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("myBall1.BackgroundImage")));
            this.myBall1.Location = new System.Drawing.Point(101, 58);
            this.myBall1.MaximumSize = new System.Drawing.Size(64, 64);
            this.myBall1.MinimumSize = new System.Drawing.Size(64, 64);
            this.myBall1.Name = "myBall1";
            this.myBall1.Size = new System.Drawing.Size(64, 64);
            this.myBall1.TabIndex = 0;
            this.myBall1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.myBall1_MouseDoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 479);
            this.Controls.Add(this.myBall1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Ball_Class.Ball myBall1;
    }
}


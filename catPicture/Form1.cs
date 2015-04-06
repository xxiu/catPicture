using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace catPicture
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        PictureBox catImage;
        Image AllImage;

        Rectangle recttangle;

       
        private void Form1_Load(object sender, EventArgs e)
        {
 
            catImage = new PictureBox();
            catImage.BackColor = Color.Transparent;
            catImage.BorderStyle = BorderStyle.FixedSingle;
            catImage.MouseMove += new MouseEventHandler(catImage_MouseMove);
            catImage.MouseDown += new MouseEventHandler(catImage_MouseDown);
            catImage.MouseUp += new MouseEventHandler(catImage_MouseUp);
            this.Controls.Add(catImage);

            recttangle = new Rectangle();



            CatScreen();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            //if (point != null)
            //{
            try
            {
                e.Graphics.DrawImage(CatSize(), catImage.Location);
                e.Graphics.DrawEllipse(new Pen(Color.Red), recttangle);
            }
            catch
            { }

            //}
            
            base.OnPaint(e);
        }
       

        private void CatScreen()
        {
            //隐藏窗体截图 显示窗体
            this.Hide();
            AllImage = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);//创建一个屏幕大小的图片
            Graphics g = Graphics.FromImage(AllImage);//创建绘图区域
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height));
           
            this.Show();
          
        }
        protected Image  CatSize()
        { 
            if (catImage.Width > 1 && catImage.Height > 1)
            {
                Image image = new Bitmap(catImage.Width, catImage.Height);//创建一个屏幕大小的图片
                Graphics g = Graphics.FromImage(image);//创建绘图区域

                g.CopyFromScreen(catImage.Location, new Point(0, 0), new Size(image.Width, image.Height));
                return image;
            }
            return null; 
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            //Image myImage = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);//创建一个屏幕大小的图片
            //Graphics g = Graphics.FromImage(myImage);//创建绘图区域
            //g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height));
            ////this.BackgroundImage = myImage;
            //pictureBox1.Image = myImage;
        }
        Point point;
        int s = 0;
        //按下鼠标
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            s = 1;
            point = new Point(e.X, e.Y);//记录起始坐标
            catImage.Location = point ;//设置图片的起点；
        }
        //放开鼠标
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        { 
            s = 3;
        }
        //移动鼠标
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
                s = 2;

              


                GetImage(point, new Point(e.X, e.Y), catImage);

            }

        }
        //图片鼠标
        void catImage_MouseMove(object sender, MouseEventArgs e)
        {
            if( s==3 && e.Button ==MouseButtons .Left  )
            {
                catImage .Cursor =System.Windows.Forms.Cursors.SizeAll;
              //  Point py = new Point(e.X, e.Y);//当前位置
                Point location = catImage.Location;
                location.Offset(e.Location .X - point.X, e.Location .Y  - point.Y);//平移
                catImage.Location = location;

                CatSize();
               // point = location;

            }
            else 
            {
                if (e.Button == MouseButtons.Left)
                { 
                    s = 2;
                    GetImage(point, new Point(e.X, e.Y), catImage);
                } 
            }
        }
      
        void catImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (s == 3 && e.Button ==MouseButtons .Left )
            {
                point = new Point(e.X, e.Y);
            }
        }

        void catImage_MouseUp(object sender, MouseEventArgs e)
        {
            s = 3;
        }




        private void  GetImage(Point a,Point b,PictureBox picture)
        {
            int width = a.X - b.X;
            int height = a.Y - b.Y;

            Point start = new Point(width <= 0 ? a.X : b.X, height <= 0 ? a.Y : b.Y);
            //picture.Location = start;//设定开始点
            //picture.Size = new Size(width < 0 ? -width : width, height < 0 ? -height : height);//设定图片大小

            //CatSize();
            recttangle.Location = start;
            recttangle.Size = new Size(width < 0 ? -width : width, height < 0 ? -height : height); 
 
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.WindowState = FormWindowState.Minimized;
 
            }
        }

    }
}

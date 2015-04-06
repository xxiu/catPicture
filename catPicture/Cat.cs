using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace catPicture
{
    public partial class Cat : Form
    {
        public Cat()
        {
            InitializeComponent();
        }
        Rectangle recttangle;//截图区域

        MouseSize mouseSize=MouseSize .None ;//鼠标所在位置

        PointShow pointShow;//操作窗体

        Bitmap  imageScreen;//主屏幕截图

        KeyHook keyHook;

   
        private void Cat_Load(object sender, EventArgs e)
        {
            //截取主屏幕
            CatScreen();

            recttangle = new Rectangle();

            mouseInfo = new Rectangle(new Point(0, 0), new Size(120, 50));
            pen = new Pen(Color.Gold , 3);
        
            pointShow = new PointShow();
            pointShow.dcopy = Copy;
            pointShow.dclose = close;
            pointShow.dsave = Save;
            pointShow.dSavescreen = SavaScreen;
 
            keyHook = new KeyHook();//初始化键盘Hook 
            keyHook.OnKeyChange += new KeyHook.KeyHookEventHandler(keyHook_OnKeyChange);
            keyHook.HookStart();//添加钩子
        
        }

        bool  keyHook_OnKeyChange(object sender, KeyHookEventArgs e)
        {
            KeyMSG keyMsg = (KeyMSG)System.Runtime.InteropServices.Marshal.PtrToStructure(e.lParam, typeof(KeyMSG));

            //进入截图截图 Ctrl+F1
            if ((keyMsg.vkCode == (int)Keys.Q) && (Control.ModifierKeys == Keys.Control)  && e.wParam ==0x101)
            {
                this.FormShow();
                return true;

            }
            if (this.Visible == true && e.wParam ==0x101)
            {
                //取消 ESC
                if (keyMsg.vkCode == (int)Keys.Escape)
                {
                    this.FormHide();
                    return true;
                }
                //快捷键截图 Ctrl+C
                if (keyMsg.vkCode == (int)Keys.C && Control.ModifierKeys == Keys.Control)
                {
                    if (recttangle.Height > 1 && recttangle.Width > 1)
                    {
                        Copy();
                        return true;
                    }
                }
                //快捷键保存
                if (keyMsg.vkCode == (int)Keys.S && Control.ModifierKeys == Keys.Control)
                {
                    if (recttangle.Height > 1 && recttangle.Width > 1)
                    {
                       lock(this )
                       {
                          Save();
                       } 
                        return true;
                    }
                }
            }
            
            
            return false; 
        }
        //截取主屏幕
        private void CatScreen()
        {
            //隐藏窗体截图 显示窗体 
            imageScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);//创建一个屏幕大小的图片
            Graphics g = Graphics.FromImage(imageScreen);//创建绘图区域
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)); 

        }

       // Rectangle all;

        Rectangle mouseInfo;//鼠标提示信息显示
        Pen pen; 
        Font font = new Font("宋体", 9);
        Brush brush = Brushes.White;
        protected override void OnPaint(PaintEventArgs e)
        {
            
            try
            {  
                e.Graphics.DrawRectangle(new Pen(Color.Black, 4), recttangle);//绘制矩形

                e.Graphics.DrawImage(CatSize(), recttangle.Location);//给矩形填充图片 CatSize是返回了recttangle区域的图片

                //绘制坐标和大小等信息
                e.Graphics.DrawRectangle(pen, mouseInfo);
                e.Graphics.FillRectangle(Brushes.Black, mouseInfo);
                e.Graphics.DrawString("当前大小："+recttangle .Width +"*"+recttangle .Height , font, brush,new Point(mouseInfo .Location .X +5,mouseInfo .Location .Y +5));
                e.Graphics.DrawString("双击直接复制" , font, brush, new Point(mouseInfo.Location.X + 5, mouseInfo.Location.Y + 20));
                e.Graphics.DrawString("ESC退出", font, brush, new Point(mouseInfo.Location.X + 5, mouseInfo.Location.Y + 35));
                
                 
            }
            catch { }

        }



        Point MouseLeftDown=new Point (0,0);//固定点
        Point MouseLeftUp=new Point (0,0);//移动点
        bool mouseClick = false;          //当前是否是处于绘制状态
        private void Cat_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Cursor == Cursors.Default)
                {
                    mouseClick = true;
                    pointShow.Show();
                }
                MouseLeftDown = e.Location;
                MouseLeftUp = e.Location;
                mouseSize = GetPosition(recttangle.Location, new Point(recttangle.Location.X + recttangle.Width, recttangle.Location.Y + recttangle.Height), e);
         
            }
        }

        private void Cat_MouseMove(object sender, MouseEventArgs e)
        {
          // pointShow.Location = new Point(e.Location.X + 20, e.Location.Y - 60);

            if (e.Button!= MouseButtons.Left)
            {
                mouseSize = GetPosition(recttangle.Location, new Point(recttangle.Location.X + recttangle.Width, recttangle.Location.Y + recttangle.Height), e);
            }

            if (e.Button == MouseButtons.Left && mouseClick)//
            {
                GetPosition(recttangle.Location, new Point(recttangle.Location.X + recttangle.Width, recttangle.Location.Y + recttangle.Height), e);

                switch (mouseSize)
                {
                    case MouseSize .UpLeft:
                    case MouseSize .UpRight:
                    case MouseSize .DownLeft:
                    case MouseSize .DownRight:
                        MouseLeftUp = e.Location;
                        break;
                    case MouseSize .CentreLeft:
                        MouseLeftUp = new Point(e.Location.X, MouseLeftDown.Y - recttangle.Height);
                        break;

                    case MouseSize .CentreRight:
                        MouseLeftUp =new Point(e.Location.X, MouseLeftDown .Y +recttangle .Height);
                        break;
                    case MouseSize .DownCentre:
                        MouseLeftUp =new Point(MouseLeftDown.X+recttangle .Width, e.Location.Y);
                        break;
                    case MouseSize .UpCentre:
                       MouseLeftUp = new Point(MouseLeftDown .Y+recttangle .Height , e.Location.Y);
                       
                        break;
                    default :
                        MouseLeftUp = e.Location;
                        break;
                }
                GetRecttangleSize(MouseLeftUp, MouseLeftDown,e);
                this.Invalidate();//指定的区域无效重新绘制
                return;
            }
            if (e.Button == MouseButtons.Left && !mouseClick)
            {
                //框架位置
                Point start = recttangle.Location;
                Point end = new Point(recttangle.X + recttangle.Width, recttangle.Y + recttangle.Height);

              //  label1.Text = "["+start .X +","+start .Y +"/"+end .X +","+end .Y +"]";

                switch (mouseSize)
                {
                    case MouseSize .Centre : //移动位置

                        Point p = recttangle.Location; 
                        p.Offset(e.X - MouseLeftDown.X, e.Y - MouseLeftDown.Y);
                        //控制窗体不移到了外面 
                        if (p.X < 0)
                            p.X = 0;
                        if (p.Y < 0)
                            p.Y = 0;
                        if (p.X > Screen.PrimaryScreen.Bounds.Width-recttangle.Width)
                            p.X = Screen.PrimaryScreen.Bounds.Width - recttangle.Width;
                        if (p.Y > Screen.PrimaryScreen.Bounds.Height - recttangle.Height)
                            p.Y = Screen.PrimaryScreen.Bounds.Height - recttangle.Height; 

                        recttangle.Location = p;
                        MouseLeftDown = e.Location;
                        FormMove();
                        
                        break;
                        //四个顶点的变动  设置两个变动
                    case MouseSize .UpLeft:  
                         MouseLeftDown = end;
                        mouseClick = true;
                        break;
                    case MouseSize .UpRight:
                         MouseLeftDown = new Point(start.X, end.Y);
                        mouseClick = true;
                        break;
                    case MouseSize .DownRight:
                         MouseLeftDown = start;
                        mouseClick = true;
                        break;
                    case MouseSize .DownLeft:
                         MouseLeftDown = new Point(end.X, start.Y);
                        mouseClick = true;
                        break;
                
                   //左右变动
                    case MouseSize .UpCentre:
                        MouseLeftDown = end;
                        mouseClick = true;
                         break;
                     case MouseSize.DownCentre:
                        MouseLeftDown = start;
                        mouseClick = true;
                        break;

                    case MouseSize .CentreLeft:
                        MouseLeftDown = end;
                        mouseClick = true;
                         break;
                    case MouseSize .CentreRight:
                        MouseLeftDown = start;
                        mouseClick = true;
                        break;
                   

                }
                this.Invalidate();
                

            }
            
        }

        private void Cat_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseClick = false;
                MouseLeftUp = e.Location;
            }
        }

        private void Cat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }
        /// <summary>
        /// 绘制两个点固定的位置
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void GetRecttangleSize(Point a, Point b,MouseEventArgs e)
        { 
  
            SetMousePoint(ref a,ref  b); 
            recttangle.Location = a;
            recttangle.Size = new Size(b.X -a.X ,b.Y -a.Y );

             //设置操作窗体的位置
            FormMove();
        //    MouseChange(a, e.Location );
           
        }
        //操作窗体和消息提示框的位置
        private void FormMove( )
        {
            Point sr = new Point(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Point a=recttangle .Location ;
            Point b=new Point (recttangle .Location .X +recttangle .Width ,recttangle .Location .Y +recttangle .Height );
            if (sr.Y - b.Y > 40)
            {
                pointShow.Location = new Point(a.X, b.Y);
            }
            else
            {
                if (a.Y > 40)
                {
                    pointShow.Location = new Point(a.X, a.Y - 30);
                }
                else
                {
                    pointShow.Location = new Point(a.X, b.Y - 30);

                }
            }
            if (sr.X - b.X > 150)
            {
                mouseInfo.Location = new Point(b.X, a.Y);
            }
            else
            {
                if (a.X > 120)
                {
                    mouseInfo.Location = new Point(a.X - 120, a.Y);
                }
                else
                {
                    mouseInfo.Location = new Point(b.X - 120, a.Y);
                }
            }
        }


        /// <summary>
        /// 修正两个点的位置转换成左上右下型
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void SetMousePoint(ref  Point a,ref  Point b)
        {
            int width = a.X - b.X;
            int height = a.Y - b.Y;

            Point start = new Point(width <= 0 ? a.X : b.X, height <= 0 ? a.Y : b.Y);
            Point End = new Point(width >= 0 ? a.X : b.X, height >= 0 ? a.Y : b.Y);
            a = start;
            b = End;
        }

        /// <summary>
        /// 在区域内绘制一个图片 即我们要截取的图片
        /// </summary>
        /// <returns></returns>
        protected Image CatSize()
        {
            try
            {
                Image image = new Bitmap(recttangle.Width, recttangle.Height);//创建一个屏幕大小的图片
               
                Graphics g = Graphics.FromImage(image);//创建绘图区域
              
                g.CopyFromScreen(recttangle.Location, new Point(0, 0), new Size(image.Width, image.Height));
               // g.DrawImage(imageScreen, recttangle.Location);
                return image;
            }
            catch {
                return null;
            }
            
        }

        /// <summary>
        /// 判断鼠标所在的位置 设置鼠标的形状
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="e"></param>
        /// <returns>鼠标鼠标的位置</returns>

        private MouseSize  GetPosition(Point a, Point b,MouseEventArgs e)
        {
            //鼠标所在的位置有10个方位 对应的10个方位
            SetMousePoint(ref a, ref b);
            //左上
            if (GetPintSize (new Point (a.X -5,a.Y -5),new Point (a.X +5,a.Y +5),e.Location))
            {
                this.Cursor =Cursors.SizeNWSE;
                return MouseSize .UpLeft;
            }
            //上中
            if (GetPintSize(new Point(a.X + 5, a.Y - 5), new Point(b.X -5, a.Y + 5), e.Location))
            {
                this.Cursor =Cursors.SizeNS;
                return MouseSize.UpCentre ;
            }
            //上右
            if (GetPintSize(new Point(b.X -5, a.Y - 5), new Point(b.X+ 5, a.Y + 5), e.Location))
            {
                this.Cursor =Cursors.SizeNESW;
                return MouseSize .UpRight;
            }
            //右中
            if (GetPintSize(new Point(b.X - 5, a.Y + 5), new Point(b.X + 5, b.Y - 5), e.Location))
            {
                this.Cursor =Cursors.SizeWE;
                return MouseSize .CentreRight;
            }
            //右下
            if (GetPintSize(new Point(b.X - 5, b.Y - 5), new Point(b.X + 5, b.Y +5), e.Location))
            {
                this.Cursor =Cursors.SizeNWSE;
                return MouseSize .DownRight;
            }
            //下中
            if (GetPintSize(new Point(a.X + 5, b.Y - 5), new Point(b.X - 5, b.Y + 5), e.Location))
            {
                this.Cursor =Cursors.SizeNS ;
                return MouseSize .DownCentre;
            }
            //下左
            if (GetPintSize(new Point(a.X - 5, b.Y-5), new Point(a.X + 5, b.Y +5), e.Location))
            {
                this.Cursor =Cursors.SizeNESW;
                return MouseSize .DownLeft;
            }
            //中左
            if (GetPintSize(new Point(a.X - 5, a.Y + 5), new Point(a.X + 5, b.Y - 5), e.Location))
            {
                this.Cursor =Cursors.SizeWE;
                return MouseSize .CentreLeft;
            }
            //中心
            if (GetPintSize(new Point(a.X + 5, a.Y + 5), new Point(b.X - 5, b.Y - 5), e.Location))
            {
                this.Cursor =Cursors.SizeAll;
                return MouseSize .Centre;
            }
            else
            {
                this.Cursor =Cursors.Default ;
                return MouseSize .None ;
 
            }
 
        }

       
        //用来规定两个定点重合的时候 表示这个区域不存在了 重新开始绘制区域 a为固定点 b为鼠标位置
        private void MouseChange(Point a, Point b)
        {
            int x = a.X - b.X;
            int y = a.Y = b.Y;
            //需要防止多次变换

            if (x ==0 || y==0 )
            {
                MouseLeftDown = a;
                MouseLeftUp = b;
                mouseClick = true; 
            }
 
        }
        
        /// <summary>
        /// 判断点point在a，b构成的区域之内，还是之外
        /// </summary>
        /// <param name="a">顶点</param>
        /// <param name="b">底点</param>
        /// <param name="point"></param>
        /// <returns></returns>

        private bool  GetPintSize(Point a, Point b, Point point)
        {
            if (point.X > a.X && point.X < b.X && point.Y > a.Y && point.Y < b.Y)
            {
                return true;

            }
            else
            {
                return false;
            }
 
        }


        public enum MouseSize
        {
            None=0,
            UpLeft,
            UpCentre,
            UpRight,
            CentreRight,
            DownRight,
            DownCentre,
            DownLeft,
            CentreLeft,
            Centre
           
        }


        //复制
        public void Copy()
        {
            try
            {
                Bitmap image = imageScreen.Clone(recttangle, System.Drawing.Imaging.PixelFormat.DontCare); //new Bitmap(recttangle.Width, recttangle.Height);
                Clipboard.SetImage((Image)image);
                this.FormHide();
                       }
            catch
            { }
        }
        //保存
        public void Save()
        {
            Bitmap image = imageScreen.Clone(recttangle, System.Drawing.Imaging.PixelFormat.DontCare); //new Bitmap(recttangle.Width, recttangle.Height);
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "BMP 文件 |*.BMP|GIF文件|*.GIF|JPG文件|*.jpg|PNG文件|*.png"; 
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string fileName = sfd.FileName;
                image.Save(fileName);
                this.FormHide();
            }
        }
        //保存全屏
        public void SavaScreen()
        {
           // Bitmap image = imageScreen.Clone();//new Bitmap(recttangle.Width, recttangle.Height);
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "BMP 文件 |*.BMP|GIF文件|*.GIF|JPG文件|*.jpg|PNG文件|*.png";
           
            Clipboard.SetImage((Image)imageScreen);

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string fileName = sfd.FileName;
                //image.Save(fileName);
                imageScreen.Save(fileName);
                this.FormHide();
            }
        
        }
        //退出
        public void close()
        {
            this.FormHide();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            keyHook.HookStop();
            this.Close();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about ab = new about();
            ab.ShowDialog();
        }

        private void Cat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (keyHook !=null )
            {
                keyHook .HookStop();//卸载钩子
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.Show();
        }
        //鼠标双击时如果鼠标在选区内 直接复制截图
        private void Cat_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (GetPintSize(recttangle.Location, new Point(recttangle.Location.X + recttangle.Width, recttangle.Location.Y + recttangle.Height), e.Location))
            {
                Copy();
            }

        }
        
        /// <summary>
        /// 隐藏窗体 并最小化
        /// </summary> 
        public void FormHide()
        {
             recttangle.Height = recttangle.Width = 0;
            this.Invalidate();
            this.Hide();
            this.pointShow.Hide();
            this.WindowState = FormWindowState.Minimized;
            
          
        }
        public void FormShow()
        {
            this.CatScreen();//开始截图时截取全屏
            this.Show();
            this.WindowState = FormWindowState.Maximized;
        }

    }
}

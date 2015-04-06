using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace catPicture
{
    public partial class PointShow : Form
    {

       public  delegate void dCopy();
       public delegate void dClose();
       public delegate void dSave();
       public delegate void dSaveScreen();

        public dCopy dcopy;
        public dClose dclose;
        public dSave dsave;
        public dSaveScreen dSavescreen;

        public PointShow()
        {
            InitializeComponent();
           
        }
         
        protected override void OnPaint(PaintEventArgs e)
        {
            
            
            base.OnPaint(e);
        }
        private void PointShow_Load(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        //复制
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            dcopy();
            this.Hide();
        }
        //保存
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            dsave();
            this.Hide();

        }
        //取消
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            dclose();
            this.Hide();

        }
        //全屏截图
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            dSavescreen();
            this.Hide();
        }
 
    }
}

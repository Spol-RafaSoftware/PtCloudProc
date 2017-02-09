using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TaoComp1
{
    partial class DLG_SHowSurface : Form
    { 
        
        public DLG_SHowSurface()
        {
            InitializeComponent();
        }

        public void addsurfacedata(string[] s)
        {
            foreach(string str in s)
            {
                checkedListBox1.Items.Add(str);
            }
        }

        public void getCheckedModels(ref List<int> chked)
        {  
             for (int i=0; i< checkedListBox1.CheckedItems.Count;i++)
             {
                 string[] num = checkedListBox1.CheckedItems[i].ToString().Split(' ');
                 chked.Add(int.Parse(num[0]));
             }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Close();
            
        }

    }
}

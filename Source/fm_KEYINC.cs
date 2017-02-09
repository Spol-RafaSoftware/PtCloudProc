using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TaoComp1
{
    public partial class fm_Keyinc : Form
    {
        private double _POSINC;
        private double _ANGINC;
        public fm_Keyinc()
        {
            InitializeComponent();
            TXTBX_POSINC.Text =_POSINC.ToString();
            TXTBX_ANGINC.Text = _ANGINC.ToString();
        }

        public void fromInc(double POSINC, double ANGINC)
        {
            _POSINC= POSINC;
            _ANGINC = ANGINC;
            TXTBX_POSINC.Text = POSINC.ToString();
            TXTBX_ANGINC.Text = ANGINC.ToString();
        }

        public double POSINC
        {
            get
            {
                return _POSINC;
            }
            set
            {
                _POSINC = value;
            }
        }

        public double ANGINC
        {
            get
            {
                return _ANGINC;
            }
            set
            {
                _ANGINC = value;
            }
        }

        private void BTN_SETPOSINC_Click(object sender, EventArgs e)
        {
            _POSINC=double.Parse(TXTBX_POSINC.Text);
        
        }

        private void BTN_SETANGINC_Click(object sender, EventArgs e)
        {
           _ANGINC= double.Parse(TXTBX_ANGINC.Text); 

        }
 
    }
}

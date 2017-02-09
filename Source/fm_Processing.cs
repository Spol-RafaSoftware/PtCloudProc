using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TaoComp1
{
    public partial class fm_Processing : Form
    {

        private EventFiringListener listener;
        delegate void SetTextCallback(string text);
        public fm_Processing()
        {
            InitializeComponent(); 
            listener = new EventFiringListener();
            listener.WriteEvent += new EventHandler<EventListenerArgs>(listener_WriteEvent);
            Trace.Listeners.Add(listener);
        }

        private void fm_Processing_Load(object sender, EventArgs e)
        {

        }

        
        public void UpDateLsts(List<string> surf,List<string> model)
        {
            LSTBX_Surf.Items.Clear();
            LSTBX_Model.Items.Clear(); 

            foreach (string s in surf)
            {
                LSTBX_Surf.Items.Add(s);
            }

            
            foreach (string s in model)
            { 
                LSTBX_Model.Items.Add(s);
            } 
        }

        public void UpDateLsts(string[] surf, string[] model,string[] oct)
        {
            LSTBX_Surf.Items.Clear();
            LSTBX_Model.Items.Clear();
            LSTBX_Oct.Items.Clear();
            LSTBX_Oct.Items.AddRange(oct);
            LSTBX_Surf.Items.AddRange(surf); 
            LSTBX_Model.Items.AddRange(model); 
        }
        public void UpDateLstsTST( string s,  string  m)
        { 
            LSTBX_Surf.Items.Add(s); 

            LSTBX_Model.Items.Add(m);  
        }
 
        void listener_WriteEvent(object sender, EventListenerArgs e)
        {
            this.SetText(e.Message);  
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.LSTBX_Msg.InvokeRequired)
            {
                if (!this.Disposing)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    this.Invoke(d, new object[] { text });
                }
            }
            else
            {
                LSTBX_Msg.Items.Add(text);
                LSTBX_Msg.SetSelected(LSTBX_Msg.Items.Count - 1, true);
            }
        }

        private void fm_Processing_FormClosing(object sender, FormClosingEventArgs e)
        {
            Trace.Listeners.Remove(this.listener);
        }
    }
        

    public class EventListenerArgs : EventArgs
    {
        private string _message;

        public string Message

        {
            get { return _message; }
            set { _message = value; }
        }

        public EventListenerArgs(string Message)
        {
            this.Message = Message;
        }
    } 

    public class EventFiringListener : TraceListener
    {
        public event EventHandler<EventListenerArgs> WriteEvent;

        public override void WriteLine(string message)
        {
            EventHandler<EventListenerArgs> temp = WriteEvent;
            if (temp != null)
            {
                temp(this, new EventListenerArgs(message));
            }

        }

        public override void Write(string s)
        {
            EventHandler<EventListenerArgs> temp = WriteEvent;
            if (temp != null)
            {
                temp(this, new EventListenerArgs(s));
            }

        }           
    }

 
}

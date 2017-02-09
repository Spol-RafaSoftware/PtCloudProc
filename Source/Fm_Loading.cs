using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing; 
using System.Windows.Forms;
using System.Threading;
using Model;

namespace TaoComp1
{
    public partial class Fm_Loading : Form
    {
        public Fm_Loading()
        {
            InitializeComponent();
            _LoadingBar1.Value = 0;
            _LoadingBar1.Step = 1;
            _LoadingBar1.Maximum = 100;
            _LoadingBar1.Minimum = 0;
            workerObject = new Worker();
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            timer2.Start();
            // Create the thread object. This does not start the thread.
            workerThread = new Thread(workerObject.DoWork);

            // Start the worker thread.
            workerThread.Start();
            Trace.WriteLine("main thread: Starting worker thread...");

            // Loop until worker thread activates.
            while (!workerThread.IsAlive) ;

        }

        private void closethread()
        {

            // Use the Join method to block the current thread 
            // until the object's thread terminates.
            if (workerThread != null)
            {
                workerThread.Join();
                Trace.WriteLine("main thread: Worker thread has terminated.");
            
                workerThread = null;
                timer2.Dispose();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (_LoadingBar1 != null)
            {
                _LoadingBar1.Value = (int)Reader._progress;
                if (_LoadingBar1.Value >= 99)
                {
                    timer2.Stop(); 
                    closethread();
                    this.Close();
                }
            }

        }

        public string FileName 
        {
            set
            {
                workerObject.filename = value;
            }

        }

        private Worker workerObject;
        private Thread workerThread;
    }
}

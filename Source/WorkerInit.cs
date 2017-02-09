using System;
using System.Collections.Generic;  
using System.Threading;
using Model;

namespace TaoComp1
{

    public class WorkerInit
    {
        // This method will be called when the thread is started.
        public void DoWork()
        {
             fm1.Show();   
        }
        public fm_Processing fm1;
    }
 
}

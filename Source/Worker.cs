using System;
using System.Collections.Generic;  
using System.Threading;
using Model;

namespace TaoComp1
{

    public class Worker
    {
        // This method will be called when the thread is started.
        public void DoWork()
        {
            Reader.Read(filename, false);   
        }
        public string filename;
    }
 
}

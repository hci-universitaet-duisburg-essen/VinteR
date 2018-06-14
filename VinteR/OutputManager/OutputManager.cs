using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VinteR.Model;
using VinteR.OutputAdapter;

namespace VinteR.OutputManager
{
    public delegate void OutPutEventHander(MocapFrame mocapFrame);
    class OutputManager : IOutputManager
    {
        public event OutPutEventHander OutputNotification;


        /*
         * Passive called by Datamerger or Streamingmanager. 
         * Consider the identical output Forma from datamerger, is here an 1 to N broadcast by rasing event.
         * All outputadapter receive same source to out put preparing.
         */
        public void ReadyToOutput(MocapFrame mocapFrame)
        {
            OutputNotification?.Invoke(mocapFrame);
        }
    }
}

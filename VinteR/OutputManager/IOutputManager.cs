using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinteR.Model;

namespace VinteR.OutputManager
{
    public interface IOutputManager
    {
        event OutPutEventHander OutputNotification;
        void ReadyToOutput(MocapFrame mocapFrame);
    }
}

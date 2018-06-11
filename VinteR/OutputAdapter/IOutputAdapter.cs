using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinteR.Model;

namespace VinteR.OutputAdapter
{
    public interface IOutputAdapter
    {

        // receive the notification from Output manager.
        void OnDataReceived(MocapFrame mocapFrame);
    }
}

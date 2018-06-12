using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinteR.Model;

namespace VinteR.OutputAdapter
{

    /*
     * Only an example to make Output Manager complete
     * can be deleted or redesigned in future
     */
    class ConsoleOutputAdapter : IOutputAdapter
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private string _homeDir;
        public void OnDataReceived(MocapFrame mocapFrame)
        {
            
           // Logger.Info(mocapFrame.ToString);

        }

        public void SetHomeDir(string homeDir)
        {
            this._homeDir = homeDir;
        }

        public void Start()
        {
            //do nothing for now
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace VinteR.MainApplication
{
    public interface IMainApplication
    {
        bool IsAvailable { get; set; }
        void Start(IKernel kernel);
        void Stop();
    }
}

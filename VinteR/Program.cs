using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ninject;
using VinteR.Adapter;
using VinteR.Configuration;
using VinteR.Datamerge;
using VinteR.MainApplication;
using VinteR.Stream;

namespace VinteR
{
    internal class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger(); // may be for future use.
        public static void Main(string[] args)
        {
            // create and load dependency injection kernel
            var kernel = new StandardKernel(new VinterNinjectModule());
            
            var application = kernel.Get<IMainApplication>();
            application.Start(kernel);
        }
    }
}
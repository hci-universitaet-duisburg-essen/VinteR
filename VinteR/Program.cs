using System;
using Ninject;
using VinteR.MainApplication;

namespace VinteR
{
    internal class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger(); // may be for future use.
        
        public static void Main(string[] args)
        {
            // create and load dependency injection kernel
            var kernel = new StandardKernel(new VinterNinjectModule());
            
            // create the application
            var application = kernel.Get<IMainApplication>();

            // Event for stopping the program
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                application.Stop();
            };

            // start the program
            application.Start(kernel);

            while (application.IsAvailable)
            {
            }
        }
    }
}
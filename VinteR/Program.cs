using System;
using System.Threading;
using Ninject;
using VinteR.MainApplication;

namespace VinteR
{
    internal class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger(); // may be for future use.
        
        public static void Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);

            // create and load dependency injection kernel
            var kernel = new StandardKernel(new VinterNinjectModule());
            
            // create the application
            var application = kernel.Get<IMainApplication>();

            // Event for stopping the program
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                exitEvent.Set();
            };

            try
            {
                // start the application based upon config start.mode
                application.Start();

                // wait for exit event
                exitEvent.WaitOne();

                // stop the server
                application.Stop();
            }
            catch (ApplicationException e)
            {
                Logger.Error("Could not start application, cause: {0}", e.Message);
                Console.ReadKey();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using VinteR.Configuration;
using VinteR.Model;

namespace VinteR.OutputAdapter
{

    public class JsonFileOutputAdapter: IOutputAdapter
    {
        private readonly NLog.Logger _logger;

        public JsonFileOutputAdapter(IConfigurationService configurationService)
        {

            // get the out put path from the configuration
            var homeDir = configurationService.GetConfiguration().HomeDir;
            var filePath = homeDir + "\\" +"LoggingData" + @"\${shortdate}.json";

            // create a new target of Nlog config
            var logfile = new NLog.Targets.FileTarget("JsonLogger");

            
            //set the layout of json format
            var jsonLayout = new JsonLayout
            {
                Attributes =
                {
                    new JsonAttribute("time", "${longdate}"),
                    new JsonAttribute("level", "${level:upperCase=true}"),
                    new JsonAttribute("message", "${message}"),
                    new JsonAttribute("eventProperties", new JsonLayout
                    {
                        IncludeAllProperties = true,
                        MaxRecursionLimit = 2
                    }, false)
                }

            };

            // set the attribute of the new target
            logfile.Name = "JsonLogger";
            logfile.FileName = filePath;
            logfile.Layout = jsonLayout;

            // add the new target to current configuration
            NLog.LogManager.Configuration.AddTarget(logfile);

            // create new rule
            var rule = new LoggingRule("JsonLogger", LogLevel.Info, logfile);
            NLog.LogManager.Configuration.LoggingRules.Add(rule);

            /*
             * reload the new configuration. It's very important here.
             * Do not use NLog.LogManager.Configuration = config;
             * This will destory current configuration.
             * So just add and reload.
             */
            LogManager.Configuration.Reload();


            // get the specified Logger
            _logger = NLog.LogManager.GetLogger("JsonLogger");

        }

        public void OnDataReceived(MocapFrame mocapFrame)
        {
            
            // logging the mocapFrame into JsonFile. 
            _logger.Info("mocapFrame {frame}", mocapFrame);
        }

        public void Start()
        {            
          
            //nothing to do for now

        }

        public void Stop()
        {
            //nothing to do for now
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using VinteR.Configuration;
using VinteR.Input;
using VinteR.Model;

namespace VinteR.OutputAdapter
{

    public class JsonFileOutputAdapter: IOutputAdapter
    {
        private static NLog.Logger _logger;
        private static Session _currentSession;
        private readonly string _homeDir;


        public JsonFileOutputAdapter(IConfigurationService configurationService)
        {
            // get the out put path from the configuration
            _homeDir = configurationService.GetConfiguration().HomeDir;

        }

        public void OnDataReceived(MocapFrame mocapFrame)
        {
            // logging the mocapFrame into JsonFile. 
            _logger.Trace("mocapFrame {MocapFrame}", mocapFrame);
        }

        public void Start(Session session)
        {

            if (_currentSession == null)
            {
                _currentSession = session;
                
                InitTargetFile(_currentSession);
            }

        }

        public void Stop()
        {

            try
            {
                DateTime endTime = DateTime.Now;
                TimeSpan ts1 = new TimeSpan(_currentSession.Datetime.Ticks);
                TimeSpan ts2 = new TimeSpan(endTime.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();

                var logFile = (FileTarget)LogManager.Configuration.FindTargetByName("JsonLogger");

                logFile.FileName = Path.Combine(_homeDir, "LoggingData", "sessions.json");
                logFile.Layout = new JsonLayout
                {
                    Attributes =
                    {
                        new JsonAttribute("Name", _currentSession.Name),
                        new JsonAttribute("EndFlag", "true"),
                        new JsonAttribute("Datetime", _currentSession.Datetime.ToString("dd-MM-yyyy HH:mm:ss.fff")),
                        new JsonAttribute("EndTime", endTime.ToString("dd-MM-yyyy HH:mm:ss.fff")),
                        new JsonAttribute("Duration", ts.TotalMilliseconds.ToString("####"))
                    }

                };
                LogManager.ReconfigExistingLoggers();

                _logger?.Trace(DateTime.Now.ToString);

                _currentSession = null;
            }
            catch (SystemException e)
            {
                // nothing for now
            }

            
        }

        public void InitTargetFile(Session session)
        {

            //string dataTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");


            //":" in Path is not possible by windows
            // _filePath = _homeDir + "\\" +"LoggingData" + @"\${date:format=dd-MM-yyyy HH\:mm\:ss}.json";

            /*
             * Set and hold the file Path by every runing 
             */

            var filePath = Path.Combine(_homeDir, "LoggingData", session.Name + ".json");
            var logfile = new NLog.Targets.FileTarget("JsonLogger");


            //set the layout of json format, MaxRecursionLimit can make sub object serialized, too!!!
            var jsonLayout = new JsonLayout
            {
                Attributes =
                {
                    new JsonAttribute("session", session.Name),
                    new JsonAttribute("time", "${longdate}"),
                    new JsonAttribute("level", "${level:upperCase=true}"),
                    new JsonAttribute("message", "${message}"),
                    new JsonAttribute("eventProperties", new JsonLayout
                    {
                        IncludeAllProperties = true,
                        MaxRecursionLimit = 10
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
            var rule = new LoggingRule("JsonLogger", LogLevel.Trace, logfile);
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

    }



}

using System;
using System.Diagnostics;
using VinteR.Configuration;
using VinteR.Model;

namespace VinteR.MainApplication
{
    public class MainApplication : IMainApplication
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private enum ApplicationMode
        {
            Record,
            Playback,
            Waiting
        }

        private readonly string _startMode;
        private readonly IRecordService _recordService;
        private readonly IPlaybackService _playbackService;
        private ApplicationMode _currentMode;

        public MainApplication(IConfigurationService configurationService, IRecordService recordService, IPlaybackService playbackService)
        {
            _startMode = configurationService.GetConfiguration().StartMode;
            _recordService = recordService;
            _playbackService = playbackService;
            _currentMode = ApplicationMode.Waiting;
        }

        public void Start()
        {
            switch (_startMode)
            {
                case "record":
                    StartRecord();
                    break;
                case "playback":
                    // do nothing as a session is needed for playback
                    break;
            }
        }

        public void StartRecord()
        {
            switch (_currentMode)
            {
                case ApplicationMode.Record:
                    Logger.Warn("Already recording");
                    break;
                case ApplicationMode.Playback:
                    Stop();
                    _recordService.Start();
                    break;
                case ApplicationMode.Waiting:
                    _recordService.Start();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _currentMode = ApplicationMode.Record;
        }

        public void StartPlayback(Session session)
        {
            switch (_currentMode)
            {
                case ApplicationMode.Record:
                    Stop();
                    _playbackService.Start(session);
                    break;
                case ApplicationMode.Playback:
                    Logger.Warn("Playback already running");
                    break;
                case ApplicationMode.Waiting:
                    _playbackService.Start(session);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Stop()
        {
            switch (_currentMode)
            {
                case ApplicationMode.Record:
                    _recordService.Stop();
                    break;
                case ApplicationMode.Playback:
                    _playbackService.Stop();
                    break;
                case ApplicationMode.Waiting:
                    Logger.Info("All modes already stopped");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Logger.Info("Application entered wait");
            _currentMode = ApplicationMode.Waiting;
        }

    }
}
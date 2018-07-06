using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VinteR.Model;

namespace VinteR.Streaming
{
    public class SessionPlayer : ISessionPlayer
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event PlayMocapFrameEventHandler FrameAvailable;

        public Session Session
        {
            get => _session;
            set
            {
                Stop();
                _session = value;
                _playbackDataLoaded = false;
            }
        }

        /// <summary>
        /// Contains the duration of the session calculated through all given
        /// frames.
        /// </summary>
        public long PlayDuration { get; private set; }

        private Session _session;

        private IGrouping<long, MocapFrame>[] _groupedFrames;

        /// <summary>
        /// Stopwatch with which the frames that should be played is
        /// calculated.
        /// </summary>
        private readonly Stopwatch _playStopwatch;

        /// <summary>
        /// Current position which frames should be played.
        /// </summary>
        private long _position;

        /// <summary>
        /// Start point from which the session should be played. Also used
        /// to calculate the duration.
        /// </summary>
        private uint _start;

        /// <summary>
        /// End point until the session should be played. Also used
        /// to calculate the duration.
        /// </summary>
        private uint _end;

        /// <summary>
        /// Used to save the elapsed millis of the stopwatch and
        /// increment the current position.
        /// </summary>
        private long _lastElapsed;

        /// <summary>
        /// Contains the timer, that is used to get high frequency callbacks.
        /// The timer is just used to get callbacks and has nothing to do with
        /// the calculation which frame should be played!
        /// </summary>
        private HighResolutionTimer _timer;

        /// <summary>
        /// Returns true if all calculations were done to play the session
        /// and the session is currently playing.
        /// </summary>
        private bool _playbackDataLoaded;

        public bool IsPlaying => _timer != null && _timer.IsRunning;

        public SessionPlayer()
        {
            _playStopwatch = new Stopwatch();
        }

        public void Play()
        {
            if (!_playbackDataLoaded) Start();
            else Continue();
        }

        private void Start()
        {
            // validation checks
            if (Session == null)
            {
                Logger.Error("No session to play");
                return;
            }

            var frames = Session.MocapFrames;
            if (frames.Count == 0)
            {
                Logger.Warn("No frames to play in session {0}", Session.Name);
                return;
            }

            // sort and group all frames by their elapsed milliseconds
            _groupedFrames = frames
                .OrderBy(f => f.ElapsedMillis)
                .GroupBy(f => f.ElapsedMillis, f => f)
                .ToArray();

            // starting point of session play
            _start = Convert.ToUInt32(_groupedFrames.First().Key);
            // end point of session play
            _end = Convert.ToUInt32(_groupedFrames.Last().Key);
            // time of play duration, NOT session duration
            PlayDuration = _end - _start;

            Continue();
            _playbackDataLoaded = _timer.IsRunning;
        }

        private void Continue()
        {
            StopTimer();
            _playStopwatch.Start();
            StartTimer();
        }

        private void OnTimerElapsed(object sender, HighResolutionTimerElapsedEventArgs e)
        {
            _position += _playStopwatch.ElapsedMilliseconds - _lastElapsed;
            _lastElapsed = _playStopwatch.ElapsedMilliseconds;

            // reset if needed
            if (_position > _end)
                _position = _start;

            /*
             * As frames millis are stored as long values the current position
             * has to be converted to long. Do NOT change the position to long
             * as it may lead to stalling as e.Delay - _lastDelay may be 0.
             * The frame groups contain all frames that occured on given
             * position.
             */
            var frameGroups = _groupedFrames.Where(g => g.Key == _position);

            // Deliver each frame on millisecond with _position
            foreach (var group in frameGroups)
            {
                foreach (var mocapFrame in group)
                {
                    FrameAvailable?.Invoke(mocapFrame);
                }
            }
        }

        public void Pause()
        {
            StopTimer();
            _playStopwatch.Stop();
        }

        public void Stop()
        {
            StopTimer();
            _playStopwatch.Stop();
            _playStopwatch.Reset();
            _lastElapsed = 0;
        }

        public void Jump(uint millis)
        {
            if (millis > _end || millis < _start)
            {
                Logger.Warn("Can not jump to {0}", millis);
                return;
            }

            _position = millis;
        }

        private void StartTimer()
        {
            // start a timer that tries to fire events each millisecond
            _timer = new HighResolutionTimer(1);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private void StopTimer()
        {
            if (_timer == null) return;

            _timer.Elapsed -= OnTimerElapsed;
            _timer.Stop();
        }
    }
}
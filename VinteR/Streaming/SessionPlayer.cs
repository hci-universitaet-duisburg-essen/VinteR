using System;
using System.Collections.Generic;
using System.Linq;
using VinteR.Model;

namespace VinteR.Streaming
{
    public delegate void PlayMocapFrameEventHandler(MocapFrame frame);

    public class SessionPlayer : ISessionPlayer
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event PlayMocapFrameEventHandler FrameAvailable;

        public Session Session
        {
            get => _session;
            set
            {
                if (_hasStarted)
                {
                    Stop();
                    _session = value;
                    Start();
                }
                else
                {
                    _session = value;
                }
            }
        }

        /// <summary>
        /// Contains the duration of the session calculated through all given
        /// frames.
        /// </summary>
        public long Duration { get; private set; }

        private Session _session;
        private IEnumerable<IGrouping<long, MocapFrame>> _groupedFrames;

        /// <summary>
        /// Current position which frames should be played. Use a double here
        /// regarding unprecise clock calculation that may lead to no moving
        /// position in the worst case.
        /// </summary>
        private double _position;
        private double _lastDelay;

        /// <summary>
        /// Contains the timer, with which the position is calculated
        /// which frame has to be played
        /// </summary>
        private HighResolutionTimer _timer;

        /// <summary>
        /// Returns true if all calculations were done to play the session
        /// and the session is currently playing.
        /// </summary>
        private bool _hasStarted;

        public void Play()
        {
            if (!_hasStarted) Start();
            else Continue();
        }

        private void Start()
        {
            // validation checks
            if (_session == null)
            {
                Logger.Error("No session to play");
                return;
            }

            var frames = _session.MocapFrames;
            if (frames.Count == 0)
            {
                Logger.Warn("No frames to play in session {0}", _session.Name);
                return;
            }

            // sort and group all frames by their elapsed milliseconds
            _groupedFrames = frames
                .OrderBy(f => f.ElapsedMillis)
                .GroupBy(f => f.ElapsedMillis, f => f);
            _lastDelay = 0;
            Duration = Convert.ToInt64(_groupedFrames.Last().Key);

            RunTimer();
            _hasStarted = _timer.IsRunning;
        }

        private void Continue()
        {
            RunTimer();
        }

        private void OnTimerElapsed(object sender, HighResolutionTimerElapsedEventArgs e)
        {
            // add some milli(s) to the current position
            _position += e.Delay - _lastDelay;

            // reset if needed
            if (_position > Duration)
                _position = 0;

            // save last delay
            _lastDelay = e.Delay;

            /*
             * As frames millis are stored as long values the current position
             * has to be converted to long. Do NOT change the position to long
             * as it may lead to stalling as e.Delay - _lastDelay may be 0.
             * The frame groups contain all frames that occured on given
             * position.
             */
            var frameGroups = _groupedFrames.Where(g => g.Key == Convert.ToInt64(_position));

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
        }

        public void Stop()
        {
            StopTimer();
            _position = 0;
        }

        public void Jump(uint millis)
        {
            if (millis > Duration)
            {
                Logger.Warn("Can not jump to {0}", millis);
                return;
            }

            _position = millis;
        }

        private void RunTimer()
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
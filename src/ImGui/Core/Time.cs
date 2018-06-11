using System;
using System.Diagnostics;

namespace ImGui.Core
{
    internal static class Time
    {
        private static readonly Stopwatch _applicationWatch = new Stopwatch();
        
        private static long _frameStartTime;
        private static long _deltaTime;
        
        /// <summary>
        /// The time in ms since the application started.
        /// </summary>
        public static long time
        {
            get
            {
                if(!_applicationWatch.IsRunning)
                {
                    throw new InvalidOperationException(
                        "The time cannot be obtained because the internal time isn't running. Call Application.Run to run it first.");
                }
                return _applicationWatch.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// The time in ms it took to complete the last frame
        /// </summary>
        public static long deltaTime => _deltaTime;

        public static void Init()
        {
            _applicationWatch.Start();
            _frameStartTime = time;
        }

        public static void OnFrameBegin()
        {
            _frameStartTime = time;
        }

        public static void OnFrameEnd()
        {
            _deltaTime = time - _frameStartTime;
        }
    }
}
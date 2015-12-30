#region Using declarations

using System;
using System.ComponentModel.Design;
using System.Diagnostics;

#endregion

namespace RageEngine
{
    public struct TimeInfo {
        public double Elapsed;
        public double TotalTime;

        public TimeInfo(double elapsedTime, double totalTime) {
            this.Elapsed = elapsedTime;
            this.TotalTime = totalTime;
        }
    }

	public class Timer 
    {
        public long Ticks = 0;

		private long lastUpdateTime;
		private double totalTime;
		private bool started;

		private double lastFPSTime;
		private int frameNumber;

        public Stopwatch stopwatch = new Stopwatch();
        public TimeInfo Time { get; private set; }
        public int FramesPerSecond { get; private set; }

        public Timer() { }

		public void Start()
		{
			if (started)
				throw new InvalidOperationException("Timer was already started.");
            stopwatch.Start();
			started = true;
			Reset();
		}

		public void Stop()
		{
			if (!started)
				throw new InvalidOperationException("Timer has not been started yet.");

			started = false;
            stopwatch.Stop();
		}

		public void Reset()
		{
			if (!started)
				throw new InvalidOperationException("Timer has not been started yet.");

			lastUpdateTime = Environment.TickCount;
			totalTime = 0;
			lastFPSTime = 0;
            stopwatch.Reset();
            stopwatch.Start();
		}

		public void Update()
		{
            Ticks++;
			var currentTime = Environment.TickCount;
			double elapsedTime = (double)(currentTime - lastUpdateTime) / 1000;
			totalTime += elapsedTime;
			lastUpdateTime = currentTime;
			frameNumber += 1;
			if (totalTime - lastFPSTime >= 1)
			{
				FramesPerSecond = frameNumber;
				frameNumber = 0;
				lastFPSTime = totalTime;
			}

			Time = new TimeInfo(elapsedTime, totalTime);
		}
	}
}
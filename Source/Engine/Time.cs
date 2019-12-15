using Microsoft.Xna.Framework;
using System;

namespace Topaz.Engine
{
    public class Time
    {
        public TimeSpan ElapsedGameTime { get; private set; }
        public TimeSpan TotalGameTime { get; private set; }
        public long TotalFrameCount { get; private set; }
        public double FramesPerSecond { get; private set; }

        public Time()
        {
            TotalFrameCount = 0;
        }

        public void Update(GameTime gameTime)
        {
            ElapsedGameTime = gameTime.ElapsedGameTime;
            TotalGameTime = gameTime.TotalGameTime;
            TotalFrameCount++;
            FramesPerSecond = Math.Round(1f / ElapsedGameTime.TotalSeconds);
        }
    }
}

using Microsoft.Xna.Framework;
using System;

namespace StardewClone.Systems
{
    public class TimeSystem
    {
        public int Day { get; private set; } = 1;
        public Season CurrentSeason { get; private set; } = Season.Spring;
        public int Year { get; private set; } = 1;
        public int Hour { get; private set; } = 6;
        public int Minute { get; private set; } = 0;
        public Weather CurrentWeather { get; private set; } = Weather.Sunny;

        private double _timeAccumulator = 0;
        private const double MINUTE_DURATION = 0.7; // Real seconds per game minute
        private Random _random = new Random();

        public string TimeString => $"{Hour:D2}:{Minute:D2}";
        public string DateString => $"{CurrentSeason} {Day}, Year {Year}";

        public void Update(GameTime gameTime)
        {
            _timeAccumulator += gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeAccumulator >= MINUTE_DURATION)
            {
                _timeAccumulator -= MINUTE_DURATION;
                AdvanceTime(10); // Advance by 10 minutes
            }
        }

        private void AdvanceTime(int minutes)
        {
            Minute += minutes;

            if (Minute >= 60)
            {
                Minute = 0;
                Hour++;

                if (Hour >= 24)
                {
                    Hour = 6; // Start next day at 6 AM
                    AdvanceDay();
                }
            }

            // Time-based events
            if (Hour == 22 && Minute == 0)
            {
                // Player should sleep soon
            }
        }

        private void AdvanceDay()
        {
            Day++;

            if (Day > 28)
            {
                Day = 1;
                AdvanceSeason();
            }

            // Update weather
            DetermineWeather();

            // Advance crops
            Game1.World.OnNewDay();

            // Restore player energy
            Game1.Player.Energy = Game1.Player.MaxEnergy;
        }

        private void AdvanceSeason()
        {
            CurrentSeason = CurrentSeason switch
            {
                Season.Spring => Season.Summer,
                Season.Summer => Season.Fall,
                Season.Fall => Season.Winter,
                Season.Winter => { Year++; return Season.Spring; },
                _ => Season.Spring
            };
        }

        private void DetermineWeather()
        {
            // Simple weather system
            if (CurrentSeason == Season.Winter)
            {
                CurrentWeather = _random.Next(100) < 70 ? Weather.Snowy : Weather.Sunny;
            }
            else if (CurrentSeason == Season.Spring)
            {
                CurrentWeather = _random.Next(100) < 40 ? Weather.Rainy : Weather.Sunny;
            }
            else
            {
                CurrentWeather = _random.Next(100) < 20 ? Weather.Rainy : Weather.Sunny;
            }

            // Rainy days water all crops
            if (CurrentWeather == Weather.Rainy)
            {
                WaterAllCrops();
            }
        }

        private void WaterAllCrops()
        {
            for (int y = 0; y < Game1.World.Height; y++)
            {
                for (int x = 0; x < Game1.World.Width; x++)
                {
                    var tile = Game1.World.GetTile(x, y);
                    if (tile != null && tile.Crop != null)
                    {
                        tile.IsWatered = true;
                    }
                }
            }
        }

        public void Sleep()
        {
            Hour = 6;
            Minute = 0;
            AdvanceDay();
        }
    }
}

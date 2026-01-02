using Newtonsoft.Json;
using System;
using System.IO;

namespace StardewClone.Systems
{
    [Serializable]
    public class GameSaveData
    {
        public PlayerData Player { get; set; }
        public InventoryData Inventory { get; set; }
        public TimeData Time { get; set; }
        public WorldData World { get; set; }
    }

    [Serializable]
    public class PlayerData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Energy { get; set; }
    }

    [Serializable]
    public class InventoryData
    {
        public Item[] Items { get; set; }
        public int Money { get; set; }
        public int SelectedSlot { get; set; }
    }

    [Serializable]
    public class TimeData
    {
        public int Day { get; set; }
        public Season Season { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public Weather Weather { get; set; }
    }

    [Serializable]
    public class WorldData
    {
        public TileData[,] Tiles { get; set; }
    }

    [Serializable]
    public class TileData
    {
        public TileType Type { get; set; }
        public CropData Crop { get; set; }
        public bool IsWatered { get; set; }
    }

    [Serializable]
    public class CropData
    {
        public ItemType SeedType { get; set; }
        public int GrowthStage { get; set; }
        public int DaysGrowing { get; set; }
        public int DaysToMaturity { get; set; }
    }

    public class SaveSystem
    {
        private const string SAVE_DIRECTORY = "Saves";
        private const string SAVE_FILE = "savegame.json";

        public SaveSystem()
        {
            if (!Directory.Exists(SAVE_DIRECTORY))
            {
                Directory.CreateDirectory(SAVE_DIRECTORY);
            }
        }

        public void SaveGame()
        {
            try
            {
                var saveData = new GameSaveData
                {
                    Player = new PlayerData
                    {
                        X = Game1.Player.Position.X,
                        Y = Game1.Player.Position.Y,
                        Energy = Game1.Player.Energy
                    },
                    Inventory = new InventoryData
                    {
                        Items = Game1.InventorySystem.Items.ToArray(),
                        Money = Game1.InventorySystem.Money,
                        SelectedSlot = Game1.InventorySystem.SelectedSlot
                    },
                    Time = new TimeData
                    {
                        Day = Game1.TimeSystem.Day,
                        Season = Game1.TimeSystem.CurrentSeason,
                        Year = Game1.TimeSystem.Year,
                        Hour = Game1.TimeSystem.Hour,
                        Minute = Game1.TimeSystem.Minute,
                        Weather = Game1.TimeSystem.CurrentWeather
                    },
                    World = SerializeWorld()
                };

                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                string filePath = Path.Combine(SAVE_DIRECTORY, SAVE_FILE);
                File.WriteAllText(filePath, json);

                Console.WriteLine("Game saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game: {ex.Message}");
            }
        }

        public void LoadGame()
        {
            try
            {
                string filePath = Path.Combine(SAVE_DIRECTORY, SAVE_FILE);
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("No save file found.");
                    return;
                }

                string json = File.ReadAllText(filePath);
                var saveData = JsonConvert.DeserializeObject<GameSaveData>(json);

                // Restore player
                Game1.Player.Position = new Microsoft.Xna.Framework.Vector2(
                    saveData.Player.X,
                    saveData.Player.Y
                );
                Game1.Player.Energy = saveData.Player.Energy;

                // Restore inventory
                Game1.InventorySystem.Items.Clear();
                foreach (var item in saveData.Inventory.Items)
                {
                    Game1.InventorySystem.Items.Add(item);
                }
                Game1.InventorySystem.Money = saveData.Inventory.Money;
                Game1.InventorySystem.SelectSlot(saveData.Inventory.SelectedSlot);

                // Restore time
                // Note: You'd need to add setters or a restore method to TimeSystem

                // Restore world
                DeserializeWorld(saveData.World);

                Console.WriteLine("Game loaded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game: {ex.Message}");
            }
        }

        private WorldData SerializeWorld()
        {
            var worldData = new WorldData
            {
                Tiles = new TileData[Game1.World.Width, Game1.World.Height]
            };

            for (int y = 0; y < Game1.World.Height; y++)
            {
                for (int x = 0; x < Game1.World.Width; x++)
                {
                    var tile = Game1.World.GetTile(x, y);
                    worldData.Tiles[x, y] = new TileData
                    {
                        Type = tile.Type,
                        IsWatered = tile.IsWatered,
                        Crop = tile.Crop != null ? new CropData
                        {
                            SeedType = tile.Crop.SeedType,
                            GrowthStage = tile.Crop.GrowthStage,
                            DaysGrowing = tile.Crop.DaysGrowing,
                            DaysToMaturity = tile.Crop.DaysToMaturity
                        } : null
                    };
                }
            }

            return worldData;
        }

        private void DeserializeWorld(WorldData worldData)
        {
            for (int y = 0; y < Game1.World.Height; y++)
            {
                for (int x = 0; x < Game1.World.Width; x++)
                {
                    var tileData = worldData.Tiles[x, y];
                    var tile = Game1.World.GetTile(x, y);

                    tile.Type = tileData.Type;
                    tile.IsWatered = tileData.IsWatered;

                    if (tileData.Crop != null)
                    {
                        tile.Crop = new Crop
                        {
                            SeedType = tileData.Crop.SeedType,
                            GrowthStage = tileData.Crop.GrowthStage,
                            DaysGrowing = tileData.Crop.DaysGrowing,
                            DaysToMaturity = tileData.Crop.DaysToMaturity
                        };
                    }
                    else
                    {
                        tile.Crop = null;
                    }
                }
            }
        }
    }
}

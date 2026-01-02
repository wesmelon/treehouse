using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewClone.Systems
{
    public class Tile
    {
        public TileType Type { get; set; }
        public Crop Crop { get; set; }
        public bool IsWatered { get; set; }
    }

    public class Crop
    {
        public ItemType SeedType { get; set; }
        public int GrowthStage { get; set; }
        public int DaysGrowing { get; set; }
        public int DaysToMaturity { get; set; }

        public bool IsHarvestable => DaysGrowing >= DaysToMaturity;

        public void AdvanceGrowth()
        {
            DaysGrowing++;
            GrowthStage = Math.Min(4, (int)((float)DaysGrowing / DaysToMaturity * 5));
        }
    }

    public class World
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private Tile[,] _tiles;
        private Random _random;

        public World(int width, int height)
        {
            Width = width;
            Height = height;
            _tiles = new Tile[width, height];
            _random = new Random();

            InitializeWorld();
        }

        private void InitializeWorld()
        {
            // Generate a simple world
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _tiles[x, y] = new Tile { Type = TileType.Grass };

                    // Add some variety
                    double noise = SimplexNoise(x * 0.1, y * 0.1);

                    if (noise < -0.3)
                        _tiles[x, y].Type = TileType.Dirt;
                    else if (noise > 0.4 && y > 10 && y < Height - 10)
                        _tiles[x, y].Type = TileType.Stone;
                }
            }

            // Create a small pond
            int pondX = Width / 3;
            int pondY = Height / 3;
            for (int y = -3; y <= 3; y++)
            {
                for (int x = -4; x <= 4; x++)
                {
                    if (x * x + y * y < 12)
                    {
                        int px = pondX + x;
                        int py = pondY + y;
                        if (IsValidTile(px, py))
                            _tiles[px, py].Type = TileType.Water;
                    }
                }
            }

            // Create a farmable area around spawn
            int farmX = Width / 2;
            int farmY = Height / 2;
            for (int y = -15; y <= 15; y++)
            {
                for (int x = -15; x <= 15; x++)
                {
                    int px = farmX + x;
                    int py = farmY + y;
                    if (IsValidTile(px, py))
                        _tiles[px, py].Type = TileType.Grass;
                }
            }
        }

        private double SimplexNoise(double x, double y)
        {
            // Simple pseudo-random noise
            return Math.Sin(x * 12.9898 + y * 78.233) * 43758.5453 % 1.0;
        }

        public void Update(GameTime gameTime)
        {
            // World updates happen here
            // Crops are advanced in TimeSystem.OnNewDay()
        }

        public void OnNewDay()
        {
            // Advance all crops and reset watering
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var tile = _tiles[x, y];

                    if (tile.Crop != null && tile.IsWatered)
                    {
                        tile.Crop.AdvanceGrowth();
                    }

                    // Reset watering
                    if (tile.Type == TileType.Watered)
                    {
                        tile.Type = TileType.Tilled;
                        tile.IsWatered = false;
                    }
                }
            }
        }

        public bool TillSoil(int x, int y)
        {
            if (!IsValidTile(x, y)) return false;

            var tile = _tiles[x, y];
            if (tile.Type == TileType.Grass || tile.Type == TileType.Dirt)
            {
                tile.Type = TileType.Tilled;
                return true;
            }

            return false;
        }

        public bool WaterTile(int x, int y)
        {
            if (!IsValidTile(x, y)) return false;

            var tile = _tiles[x, y];
            if (tile.Type == TileType.Tilled)
            {
                tile.Type = TileType.Watered;
                tile.IsWatered = true;
                return true;
            }

            return false;
        }

        public bool PlantSeed(int x, int y, ItemType seedType)
        {
            if (!IsValidTile(x, y)) return false;

            var tile = _tiles[x, y];
            if ((tile.Type == TileType.Tilled || tile.Type == TileType.Watered) && tile.Crop == null)
            {
                tile.Crop = new Crop
                {
                    SeedType = seedType,
                    GrowthStage = 0,
                    DaysGrowing = 0,
                    DaysToMaturity = ItemDatabase.GetGrowthTime(seedType)
                };
                return true;
            }

            return false;
        }

        public ItemType HarvestCrop(int x, int y)
        {
            if (!IsValidTile(x, y)) return ItemType.None;

            var tile = _tiles[x, y];
            if (tile.Crop != null && tile.Crop.IsHarvestable)
            {
                var cropType = ItemDatabase.GetCropFromSeed(tile.Crop.SeedType);
                tile.Crop = null;
                tile.Type = TileType.Tilled;
                return cropType;
            }

            return ItemType.None;
        }

        public void ClearGrass(int x, int y)
        {
            if (!IsValidTile(x, y)) return;

            var tile = _tiles[x, y];
            if (tile.Type == TileType.Grass)
            {
                tile.Type = TileType.Dirt;

                // Chance to get fiber
                if (_random.Next(100) < 50)
                {
                    Game1.InventorySystem.AddItem(new Item { Type = ItemType.Fiber, Quantity = 1 });
                }
            }
        }

        private bool IsValidTile(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Calculate visible tiles based on camera
            int startX = Math.Max(0, (int)(Game1.Player.Position.X / Game1.TILE_SIZE) - 30);
            int startY = Math.Max(0, (int)(Game1.Player.Position.Y / Game1.TILE_SIZE) - 30);
            int endX = Math.Min(Width, startX + 60);
            int endY = Math.Min(Height, startY + 60);

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    DrawTile(spriteBatch, x, y);
                }
            }
        }

        private void DrawTile(SpriteBatch spriteBatch, int x, int y)
        {
            var tile = _tiles[x, y];
            Rectangle destRect = new Rectangle(x * Game1.TILE_SIZE, y * Game1.TILE_SIZE, Game1.TILE_SIZE, Game1.TILE_SIZE);

            Color tileColor = tile.Type switch
            {
                TileType.Grass => new Color(60, 179, 113),
                TileType.Dirt => new Color(139, 90, 43),
                TileType.Tilled => new Color(101, 67, 33),
                TileType.Watered => new Color(78, 53, 28),
                TileType.Stone => new Color(128, 128, 128),
                TileType.Water => new Color(65, 105, 225),
                TileType.Sand => new Color(238, 214, 175),
                _ => Color.Green
            };

            spriteBatch.Draw(Game1.PixelTexture, destRect, tileColor);

            // Draw crop if present
            if (tile.Crop != null)
            {
                Rectangle cropRect = SpriteLibrary.GetCropSprite(tile.Crop.SeedType, tile.Crop.GrowthStage);
                Rectangle cropDestRect = new Rectangle(
                    x * Game1.TILE_SIZE + 4,
                    y * Game1.TILE_SIZE + 4,
                    Game1.TILE_SIZE - 8,
                    Game1.TILE_SIZE - 8
                );
                spriteBatch.Draw(Game1.SpriteLibrary.Texture, cropDestRect, cropRect, Color.White);
            }

            // Grid lines (optional, for debugging)
            // DrawHelper.DrawRectangleOutline(spriteBatch, destRect, Color.Black * 0.1f, 1);
        }

        public Tile GetTile(int x, int y)
        {
            if (!IsValidTile(x, y)) return null;
            return _tiles[x, y];
        }
    }
}

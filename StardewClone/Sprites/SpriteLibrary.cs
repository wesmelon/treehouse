using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace StardewClone
{
    public class SpriteLibrary
    {
        public Texture2D Texture { get; private set; }
        private const int SPRITE_SIZE = 16;

        public SpriteLibrary(GraphicsDevice graphicsDevice)
        {
            // Create a simple sprite atlas
            // In a real game, you'd load this from a PNG file
            CreateSpriteAtlas(graphicsDevice);
        }

        private void CreateSpriteAtlas(GraphicsDevice graphicsDevice)
        {
            // Create a 256x256 atlas with simple sprites
            int atlasSize = 256;
            Texture = new Texture2D(graphicsDevice, atlasSize, atlasSize);
            Color[] colorData = new Color[atlasSize * atlasSize];

            // Fill with transparent
            for (int i = 0; i < colorData.Length; i++)
                colorData[i] = Color.Transparent;

            // Draw player sprites (row 0-3, different directions and frames)
            DrawPlayerSprites(colorData, atlasSize);

            // Draw crop sprites (row 4-10)
            DrawCropSprites(colorData, atlasSize);

            Texture.SetData(colorData);
        }

        private void DrawPlayerSprites(Color[] data, int width)
        {
            // Simple player representation - just colored rectangles
            Color playerColor = new Color(255, 200, 150);

            for (int dir = 0; dir < 4; dir++) // 4 directions
            {
                for (int frame = 0; frame < 4; frame++) // 4 animation frames
                {
                    int col = frame;
                    int row = dir;
                    DrawSimpleCharacter(data, width, col, row, playerColor);
                }
            }
        }

        private void DrawCropSprites(Color[] data, int width)
        {
            // Define colors for different crop growth stages
            Color[][] cropColors = new Color[][]
            {
                // Parsnip
                new Color[] { new Color(139, 90, 43), new Color(100, 140, 80), new Color(80, 160, 100), new Color(60, 180, 120), new Color(255, 220, 150) },
                // Cauliflower
                new Color[] { new Color(139, 90, 43), new Color(100, 140, 80), new Color(120, 180, 130), new Color(140, 200, 150), new Color(245, 245, 220) },
                // Potato
                new Color[] { new Color(139, 90, 43), new Color(100, 140, 80), new Color(120, 160, 110), new Color(140, 180, 130), new Color(210, 180, 140) },
                // Tomato
                new Color[] { new Color(139, 90, 43), new Color(100, 140, 80), new Color(120, 180, 130), new Color(160, 200, 150), new Color(255, 99, 71) },
                // Corn
                new Color[] { new Color(139, 90, 43), new Color(100, 140, 80), new Color(140, 180, 130), new Color(180, 200, 150), new Color(255, 215, 0) },
                // Pumpkin
                new Color[] { new Color(139, 90, 43), new Color(100, 140, 80), new Color(140, 180, 130), new Color(180, 200, 150), new Color(255, 140, 0) },
                // Wheat
                new Color[] { new Color(139, 90, 43), new Color(100, 140, 80), new Color(160, 180, 100), new Color(200, 200, 120), new Color(238, 232, 170) },
            };

            for (int cropType = 0; cropType < cropColors.Length; cropType++)
            {
                for (int stage = 0; stage < 5; stage++)
                {
                    int col = stage;
                    int row = 4 + cropType; // Start at row 4
                    DrawSimplePlant(data, width, col, row, cropColors[cropType][stage], stage);
                }
            }
        }

        private void DrawSimpleCharacter(Color[] data, int width, int col, int row, Color color)
        {
            int startX = col * SPRITE_SIZE;
            int startY = row * SPRITE_SIZE;

            // Draw a simple character (rectangle with head)
            for (int y = 0; y < SPRITE_SIZE; y++)
            {
                for (int x = 0; x < SPRITE_SIZE; x++)
                {
                    // Simple humanoid shape
                    if ((y > 4 && y < 12 && x > 4 && x < 12) || // Body
                        (y > 2 && y < 6 && x > 5 && x < 11))   // Head
                    {
                        int index = (startY + y) * width + (startX + x);
                        data[index] = color;
                    }
                }
            }
        }

        private void DrawSimplePlant(Color[] data, int width, int col, int row, Color color, int stage)
        {
            int startX = col * SPRITE_SIZE;
            int startY = row * SPRITE_SIZE;

            // Draw plant based on growth stage
            int plantHeight = 4 + stage * 2;
            int plantWidth = 3 + stage;

            int centerX = SPRITE_SIZE / 2;
            int bottomY = SPRITE_SIZE - 2;

            for (int y = 0; y < plantHeight; y++)
            {
                for (int x = 0; x < plantWidth; x++)
                {
                    int px = centerX - plantWidth / 2 + x;
                    int py = bottomY - y;

                    if (px >= 0 && px < SPRITE_SIZE && py >= 0 && py < SPRITE_SIZE)
                    {
                        int index = (startY + py) * width + (startX + px);
                        data[index] = color;
                    }
                }
            }
        }

        public static Rectangle GetPlayerSprite(Direction direction, int frame)
        {
            int row = direction switch
            {
                Direction.Down => 0,
                Direction.Up => 1,
                Direction.Left => 2,
                Direction.Right => 3,
                _ => 0
            };

            return new Rectangle(frame * SPRITE_SIZE, row * SPRITE_SIZE, SPRITE_SIZE, SPRITE_SIZE);
        }

        public static Rectangle GetCropSprite(ItemType seedType, int growthStage)
        {
            int cropIndex = seedType switch
            {
                ItemType.Seed_Parsnip => 0,
                ItemType.Seed_Cauliflower => 1,
                ItemType.Seed_Potato => 2,
                ItemType.Seed_Tomato => 3,
                ItemType.Seed_Corn => 4,
                ItemType.Seed_Pumpkin => 5,
                ItemType.Seed_Wheat => 6,
                _ => 0
            };

            int row = 4 + cropIndex;
            int col = System.Math.Min(4, growthStage);

            return new Rectangle(col * SPRITE_SIZE, row * SPRITE_SIZE, SPRITE_SIZE, SPRITE_SIZE);
        }
    }
}

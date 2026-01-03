using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewClone
{
    public class SpriteLibrary
    {
        public Texture2D Texture { get; private set; }
        private const int SPRITE_SIZE = 16;

        public SpriteLibrary(GraphicsDevice graphicsDevice)
        {
            CreateSpriteAtlas(graphicsDevice);
        }

        private void CreateSpriteAtlas(GraphicsDevice graphicsDevice)
        {
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

        private void SetPixel(Color[] data, int width, int x, int y, Color color)
        {
            if (x >= 0 && x < width && y >= 0 && y < width)
                data[y * width + x] = color;
        }

        private void DrawPlayerSprites(Color[] data, int width)
        {
            Color skin = new Color(255, 220, 177);
            Color shirt = new Color(100, 150, 255);
            Color pants = new Color(60, 100, 140);
            Color hair = new Color(101, 67, 33);
            Color outline = new Color(40, 40, 40);

            for (int dir = 0; dir < 4; dir++)
            {
                for (int frame = 0; frame < 4; frame++)
                {
                    int startX = frame * SPRITE_SIZE;
                    int startY = dir * SPRITE_SIZE;

                    // Draw detailed character based on direction
                    switch (dir)
                    {
                        case 0: // Down
                            DrawPlayerDown(data, width, startX, startY, frame, skin, shirt, pants, hair, outline);
                            break;
                        case 1: // Up
                            DrawPlayerUp(data, width, startX, startY, frame, skin, shirt, pants, hair, outline);
                            break;
                        case 2: // Left
                            DrawPlayerLeft(data, width, startX, startY, frame, skin, shirt, pants, hair, outline);
                            break;
                        case 3: // Right
                            DrawPlayerRight(data, width, startX, startY, frame, skin, shirt, pants, hair, outline);
                            break;
                    }
                }
            }
        }

        private void DrawPlayerDown(Color[] data, int width, int x, int y, int frame, Color skin, Color shirt, Color pants, Color hair, Color outline)
        {
            // Head outline
            for (int i = 4; i <= 11; i++)
                SetPixel(data, width, x + i, y + 2, outline);
            SetPixel(data, width, x + 3, y + 3, outline);
            SetPixel(data, width, x + 3, y + 4, outline);
            SetPixel(data, width, x + 12, y + 3, outline);
            SetPixel(data, width, x + 12, y + 4, outline);

            // Hair
            for (int i = 4; i <= 11; i++)
                SetPixel(data, width, x + i, y + 3, hair);

            // Face
            for (int py = 4; py <= 6; py++)
                for (int px = 4; px <= 11; px++)
                    SetPixel(data, width, x + px, y + py, skin);

            // Eyes
            SetPixel(data, width, x + 6, y + 5, outline);
            SetPixel(data, width, x + 9, y + 5, outline);

            // Body - shirt
            for (int py = 7; py <= 10; py++)
                for (int px = 5; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, shirt);

            // Pants/legs
            int legOffset = (frame % 2) == 0 ? 0 : 1;
            for (int py = 11; py <= 14; py++)
            {
                SetPixel(data, width, x + 6 - legOffset, y + py, pants);
                SetPixel(data, width, x + 7 - legOffset, y + py, pants);
                SetPixel(data, width, x + 8 + legOffset, y + py, pants);
                SetPixel(data, width, x + 9 + legOffset, y + py, pants);
            }
        }

        private void DrawPlayerUp(Color[] data, int width, int x, int y, int frame, Color skin, Color shirt, Color pants, Color hair, Color outline)
        {
            // Back of head
            for (int i = 4; i <= 11; i++)
                SetPixel(data, width, x + i, y + 3, hair);
            for (int i = 4; i <= 11; i++)
                SetPixel(data, width, x + i, y + 4, hair);
            for (int i = 5; i <= 10; i++)
                SetPixel(data, width, x + i, y + 5, hair);

            // Ears
            SetPixel(data, width, x + 4, y + 5, skin);
            SetPixel(data, width, x + 11, y + 5, skin);

            // Body
            for (int py = 7; py <= 10; py++)
                for (int px = 5; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, shirt);

            // Legs
            for (int py = 11; py <= 14; py++)
            {
                SetPixel(data, width, x + 6, y + py, pants);
                SetPixel(data, width, x + 7, y + py, pants);
                SetPixel(data, width, x + 8, y + py, pants);
                SetPixel(data, width, x + 9, y + py, pants);
            }
        }

        private void DrawPlayerLeft(Color[] data, int width, int x, int y, int frame, Color skin, Color shirt, Color pants, Color hair, Color outline)
        {
            // Head
            for (int py = 3; py <= 6; py++)
                for (int px = 6; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, skin);

            // Hair
            for (int i = 6; i <= 10; i++)
                SetPixel(data, width, x + i, y + 2, outline);
            for (int i = 6; i <= 10; i++)
                SetPixel(data, width, x + i, y + 3, hair);

            // Eye
            SetPixel(data, width, x + 8, y + 4, outline);

            // Body
            for (int py = 7; py <= 10; py++)
                for (int px = 6; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, shirt);

            // Legs
            for (int py = 11; py <= 14; py++)
            {
                SetPixel(data, width, x + 7, y + py, pants);
                SetPixel(data, width, x + 8, y + py, pants);
            }
        }

        private void DrawPlayerRight(Color[] data, int width, int x, int y, int frame, Color skin, Color shirt, Color pants, Color hair, Color outline)
        {
            // Head
            for (int py = 3; py <= 6; py++)
                for (int px = 5; px <= 9; px++)
                    SetPixel(data, width, x + px, y + py, skin);

            // Hair
            for (int i = 5; i <= 9; i++)
                SetPixel(data, width, x + i, y + 2, outline);
            for (int i = 5; i <= 9; i++)
                SetPixel(data, width, x + i, y + 3, hair);

            // Eye
            SetPixel(data, width, x + 7, y + 4, outline);

            // Body
            for (int py = 7; py <= 10; py++)
                for (int px = 5; px <= 9; px++)
                    SetPixel(data, width, x + px, y + py, shirt);

            // Legs
            for (int py = 11; py <= 14; py++)
            {
                SetPixel(data, width, x + 7, y + py, pants);
                SetPixel(data, width, x + 8, y + py, pants);
            }
        }

        private void DrawCropSprites(Color[] data, int width)
        {
            // Each crop gets unique colors and shapes
            DrawParsnipCrop(data, width, 4);
            DrawCauliflowerCrop(data, width, 5);
            DrawPotatoCrop(data, width, 6);
            DrawTomatoCrop(data, width, 7);
            DrawCornCrop(data, width, 8);
            DrawPumpkinCrop(data, width, 9);
            DrawWheatCrop(data, width, 10);
        }

        private void DrawParsnipCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),    // Brown seed
                new Color(85, 140, 70),    // Light green sprout
                new Color(70, 160, 90),    // Medium green
                new Color(60, 180, 110),   // Dark green leaves
                new Color(255, 235, 205)   // Pale yellow parsnip
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawCropStage(data, width, startX, startY, stage, stages[stage], "parsnip");
            }
        }

        private void DrawCauliflowerCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(90, 150, 80),
                new Color(110, 180, 120),
                new Color(130, 200, 140),
                new Color(245, 245, 220)   // White cauliflower
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawCropStage(data, width, startX, startY, stage, stages[stage], "cauliflower");
            }
        }

        private void DrawPotatoCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(100, 140, 80),
                new Color(120, 160, 110),
                new Color(140, 180, 130),
                new Color(210, 180, 140)   // Tan potato
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawCropStage(data, width, startX, startY, stage, stages[stage], "potato");
            }
        }

        private void DrawTomatoCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(100, 140, 80),
                new Color(120, 180, 130),
                new Color(160, 200, 150),
                new Color(255, 99, 71)     // Red tomato
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawCropStage(data, width, startX, startY, stage, stages[stage], "tomato");
            }
        }

        private void DrawCornCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(100, 140, 80),
                new Color(140, 180, 130),
                new Color(180, 200, 150),
                new Color(255, 215, 0)     // Golden corn
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawCropStage(data, width, startX, startY, stage, stages[stage], "corn");
            }
        }

        private void DrawPumpkinCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(100, 140, 80),
                new Color(140, 180, 130),
                new Color(180, 200, 150),
                new Color(255, 140, 0)     // Orange pumpkin
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawCropStage(data, width, startX, startY, stage, stages[stage], "pumpkin");
            }
        }

        private void DrawWheatCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(100, 140, 80),
                new Color(160, 180, 100),
                new Color(200, 200, 120),
                new Color(238, 232, 170)   // Golden wheat
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawCropStage(data, width, startX, startY, stage, stages[stage], "wheat");
            }
        }

        private void DrawCropStage(Color[] data, int width, int startX, int startY, int stage, Color color, string cropType)
        {
            Color stem = new Color(60, 120, 60);
            Color darkGreen = new Color(40, 100, 50);

            switch (stage)
            {
                case 0: // Seed
                    // Small brown seed
                    for (int py = 12; py <= 13; py++)
                        for (int px = 7; px <= 8; px++)
                            SetPixel(data, width, startX + px, startY + py, color);
                    break;

                case 1: // Sprout
                    // Tiny sprout
                    SetPixel(data, width, startX + 7, startY + 13, stem);
                    SetPixel(data, width, startX + 8, startY + 13, stem);
                    SetPixel(data, width, startX + 7, startY + 12, color);
                    SetPixel(data, width, startX + 8, startY + 12, color);
                    SetPixel(data, width, startX + 7, startY + 11, color);
                    SetPixel(data, width, startX + 8, startY + 11, color);
                    break;

                case 2: // Growing
                    // Medium plant
                    for (int py = 12; py <= 13; py++)
                        SetPixel(data, width, startX + 8, startY + py, stem);
                    for (int py = 9; py <= 11; py++)
                        for (int px = 7; px <= 9; px++)
                            SetPixel(data, width, startX + px, startY + py, color);
                    break;

                case 3: // Mature
                    // Large plant with leaves
                    for (int py = 11; py <= 13; py++)
                        SetPixel(data, width, startX + 8, startY + py, stem);
                    // Leaves
                    for (int py = 7; py <= 10; py++)
                        for (int px = 6; px <= 10; px++)
                            SetPixel(data, width, startX + px, startY + py, darkGreen);
                    break;

                case 4: // Harvestable
                    // Full grown with visible crop
                    DrawHarvestableCrop(data, width, startX, startY, color, stem, cropType);
                    break;
            }
        }

        private void DrawHarvestableCrop(Color[] data, int width, int startX, int startY, Color cropColor, Color stem, string cropType)
        {
            Color leaves = new Color(40, 100, 50);

            // Draw based on crop type
            if (cropType == "tomato" || cropType == "corn")
            {
                // Tall plants - stem and leaves
                for (int py = 10; py <= 13; py++)
                    SetPixel(data, width, startX + 8, startY + py, stem);

                // Leaves
                for (int py = 8; py <= 11; py++)
                    for (int px = 6; px <= 10; px++)
                        if ((px == 6 && py < 10) || (px == 10 && py < 10) || (px > 6 && px < 10))
                            SetPixel(data, width, startX + px, startY + py, leaves);

                // Crop (tomato/corn)
                if (cropType == "tomato")
                {
                    // Round tomatoes
                    SetPixel(data, width, startX + 6, startY + 9, cropColor);
                    SetPixel(data, width, startX + 10, startY + 10, cropColor);
                    for (int i = 7; i <= 9; i++)
                        SetPixel(data, width, startX + i, startY + 9, cropColor);
                }
                else // corn
                {
                    // Corn ears
                    for (int py = 8; py <= 9; py++)
                        for (int px = 7; px <= 9; px++)
                            SetPixel(data, width, startX + px, startY + py, cropColor);
                }
            }
            else if (cropType == "cauliflower" || cropType == "pumpkin")
            {
                // Ground crops - large visible vegetable
                // Stem
                SetPixel(data, width, startX + 8, startY + 12, stem);

                // Small leaves
                SetPixel(data, width, startX + 6, startY + 10, leaves);
                SetPixel(data, width, startX + 10, startY + 10, leaves);
                SetPixel(data, width, startX + 7, startY + 9, leaves);
                SetPixel(data, width, startX + 9, startY + 9, leaves);

                // Large crop on ground
                for (int py = 10; py <= 12; py++)
                    for (int px = 6; px <= 10; px++)
                        if (py == 10 || (py == 11 && px >= 6 && px <= 10) || (py == 12 && px >= 7 && px <= 9))
                            SetPixel(data, width, startX + px, startY + py, cropColor);
            }
            else if (cropType == "wheat")
            {
                // Wheat stalks
                for (int px = 6; px <= 10; px++)
                {
                    for (int py = 10; py <= 13; py++)
                        SetPixel(data, width, startX + px, startY + py, stem);

                    // Wheat heads
                    SetPixel(data, width, startX + px, startY + 8, cropColor);
                    SetPixel(data, width, startX + px, startY + 9, cropColor);
                }
            }
            else // parsnip or potato (underground)
            {
                // Leafy top
                for (int py = 9; py <= 11; py++)
                    for (int px = 6; px <= 10; px++)
                        SetPixel(data, width, startX + px, startY + py, leaves);

                // Hint of underground crop
                for (int px = 7; px <= 9; px++)
                    SetPixel(data, width, startX + px, startY + 12, cropColor);
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
            int col = Math.Min(4, growthStage);

            return new Rectangle(col * SPRITE_SIZE, row * SPRITE_SIZE, SPRITE_SIZE, SPRITE_SIZE);
        }
    }
}

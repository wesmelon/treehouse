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

            // Draw NPC sprites (row 11-13)
            DrawNPCSprites(colorData, atlasSize);

            Texture.SetData(colorData);
        }

        private void SetPixel(Color[] data, int width, int x, int y, Color color)
        {
            if (x >= 0 && x < width && y >= 0 && y < width)
                data[y * width + x] = color;
        }

        private void DrawPlayerSprites(Color[] data, int width)
        {
            // Farmer character - distinct blue shirt and brown hair
            Color skin = new Color(255, 220, 177);
            Color shirt = new Color(65, 105, 225);  // Royal blue - very recognizable
            Color pants = new Color(101, 67, 33);   // Brown pants
            Color hair = new Color(139, 69, 19);    // Saddle brown
            Color outline = new Color(30, 30, 30);
            Color eyes = new Color(50, 50, 50);

            for (int dir = 0; dir < 4; dir++)
            {
                for (int frame = 0; frame < 4; frame++)
                {
                    int startX = frame * SPRITE_SIZE;
                    int startY = dir * SPRITE_SIZE;

                    switch (dir)
                    {
                        case 0: // Down
                            DrawPlayerDown(data, width, startX, startY, frame, skin, shirt, pants, hair, outline, eyes);
                            break;
                        case 1: // Up
                            DrawPlayerUp(data, width, startX, startY, frame, skin, shirt, pants, hair, outline);
                            break;
                        case 2: // Left
                            DrawPlayerLeft(data, width, startX, startY, frame, skin, shirt, pants, hair, outline, eyes);
                            break;
                        case 3: // Right
                            DrawPlayerRight(data, width, startX, startY, frame, skin, shirt, pants, hair, outline, eyes);
                            break;
                    }
                }
            }
        }

        private void DrawPlayerDown(Color[] data, int width, int x, int y, int frame, Color skin, Color shirt, Color pants, Color hair, Color outline, Color eyes)
        {
            // Head outline
            for (int i = 5; i <= 10; i++)
                SetPixel(data, width, x + i, y + 3, outline);
            SetPixel(data, width, x + 4, y + 4, outline);
            SetPixel(data, width, x + 4, y + 5, outline);
            SetPixel(data, width, x + 11, y + 4, outline);
            SetPixel(data, width, x + 11, y + 5, outline);

            // Hair - messy farm hair
            for (int i = 5; i <= 10; i++)
            {
                SetPixel(data, width, x + i, y + 4, hair);
                if (i == 5 || i == 10)
                    SetPixel(data, width, x + i, y + 5, hair);
            }

            // Face
            for (int py = 5; py <= 7; py++)
                for (int px = 5; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, skin);

            // Eyes - dot eyes
            SetPixel(data, width, x + 6, y + 6, eyes);
            SetPixel(data, width, x + 9, y + 6, eyes);

            // Smile
            SetPixel(data, width, x + 7, y + 7, outline);
            SetPixel(data, width, x + 8, y + 7, outline);

            // Body - bright blue shirt
            for (int py = 8; py <= 11; py++)
                for (int px = 5; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, shirt);

            // Shirt buttons
            SetPixel(data, width, x + 8, y + 9, outline);
            SetPixel(data, width, x + 8, y + 10, outline);

            // Arms
            SetPixel(data, width, x + 4, y + 9, shirt);
            SetPixel(data, width, x + 11, y + 9, shirt);

            // Pants/legs with animation
            int legOffset = (frame % 2) == 0 ? 0 : 1;
            for (int py = 12; py <= 14; py++)
            {
                SetPixel(data, width, x + 6 - legOffset, y + py, pants);
                SetPixel(data, width, x + 7 - legOffset, y + py, pants);
                SetPixel(data, width, x + 8 + legOffset, y + py, pants);
                SetPixel(data, width, x + 9 + legOffset, y + py, pants);
            }

            // Boots
            SetPixel(data, width, x + 6 - legOffset, y + 14, outline);
            SetPixel(data, width, x + 7 - legOffset, y + 14, outline);
            SetPixel(data, width, x + 8 + legOffset, y + 14, outline);
            SetPixel(data, width, x + 9 + legOffset, y + 14, outline);
        }

        private void DrawPlayerUp(Color[] data, int width, int x, int y, int frame, Color skin, Color shirt, Color pants, Color hair, Color outline)
        {
            // Back of head - hair
            for (int i = 5; i <= 10; i++)
            {
                SetPixel(data, width, x + i, y + 4, hair);
                SetPixel(data, width, x + i, y + 5, hair);
            }
            for (int i = 6; i <= 9; i++)
                SetPixel(data, width, x + i, y + 6, hair);

            // Ears peeking out
            SetPixel(data, width, x + 5, y + 6, skin);
            SetPixel(data, width, x + 10, y + 6, skin);

            // Body - blue shirt
            for (int py = 8; py <= 11; py++)
                for (int px = 5; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, shirt);

            // Arms raised
            SetPixel(data, width, x + 4, y + 9, skin);
            SetPixel(data, width, x + 11, y + 9, skin);

            // Pants
            for (int py = 12; py <= 14; py++)
            {
                SetPixel(data, width, x + 6, y + py, pants);
                SetPixel(data, width, x + 7, y + py, pants);
                SetPixel(data, width, x + 8, y + py, pants);
                SetPixel(data, width, x + 9, y + py, pants);
            }
        }

        private void DrawPlayerLeft(Color[] data, int width, int x, int y, int frame, Color skin, Color shirt, Color pants, Color hair, Color outline, Color eyes)
        {
            // Head
            for (int py = 4; py <= 7; py++)
                for (int px = 6; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, skin);

            // Hair - side profile
            for (int i = 6; i <= 9; i++)
            {
                SetPixel(data, width, x + i, y + 3, outline);
                SetPixel(data, width, x + i, y + 4, hair);
            }
            SetPixel(data, width, x + 6, y + 5, hair);
            SetPixel(data, width, x + 6, y + 6, hair);

            // Eye
            SetPixel(data, width, x + 9, y + 5, eyes);

            // Nose
            SetPixel(data, width, x + 10, y + 6, outline);

            // Body
            for (int py = 8; py <= 11; py++)
                for (int px = 6; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, shirt);

            // Arm extended
            SetPixel(data, width, x + 11, y + 9, skin);
            SetPixel(data, width, x + 11, y + 10, skin);

            // Legs
            for (int py = 12; py <= 14; py++)
            {
                SetPixel(data, width, x + 7, y + py, pants);
                SetPixel(data, width, x + 8, y + py, pants);
            }
        }

        private void DrawPlayerRight(Color[] data, int width, int x, int y, int frame, Color skin, Color shirt, Color pants, Color hair, Color outline, Color eyes)
        {
            // Head - mirror of left
            for (int py = 4; py <= 7; py++)
                for (int px = 5; px <= 9; px++)
                    SetPixel(data, width, x + px, y + py, skin);

            // Hair
            for (int i = 6; i <= 9; i++)
            {
                SetPixel(data, width, x + i, y + 3, outline);
                SetPixel(data, width, x + i, y + 4, hair);
            }
            SetPixel(data, width, x + 9, y + 5, hair);
            SetPixel(data, width, x + 9, y + 6, hair);

            // Eye
            SetPixel(data, width, x + 6, y + 5, eyes);

            // Nose
            SetPixel(data, width, x + 5, y + 6, outline);

            // Body
            for (int py = 8; py <= 11; py++)
                for (int px = 5; px <= 9; px++)
                    SetPixel(data, width, x + px, y + py, shirt);

            // Arm
            SetPixel(data, width, x + 4, y + 9, skin);
            SetPixel(data, width, x + 4, y + 10, skin);

            // Legs
            for (int py = 12; py <= 14; py++)
            {
                SetPixel(data, width, x + 7, y + py, pants);
                SetPixel(data, width, x + 8, y + py, pants);
            }
        }

        private void DrawNPCSprites(Color[] data, int width)
        {
            // Pierre - Merchant (green shirt, mustache)
            int row = 11;
            for (int frame = 0; frame < 4; frame++)
            {
                int startX = frame * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawNPC(data, width, startX, startY, new Color(34, 139, 34), new Color(85, 107, 47), "merchant");
            }

            // Emily - Villager (red hair, purple dress)
            row = 12;
            for (int frame = 0; frame < 4; frame++)
            {
                int startX = frame * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawNPC(data, width, startX, startY, new Color(147, 112, 219), new Color(255, 69, 0), "villager");
            }

            // Shane - Villager (gray shirt, dark hair)
            row = 13;
            for (int frame = 0; frame < 4; frame++)
            {
                int startX = frame * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawNPC(data, width, startX, startY, new Color(105, 105, 105), new Color(47, 79, 79), "villager2");
            }
        }

        private void DrawNPC(Color[] data, int width, int x, int y, Color clothing, Color hairColor, string npcType)
        {
            Color skin = new Color(255, 220, 177);
            Color outline = new Color(30, 30, 30);

            // Head
            for (int py = 4; py <= 7; py++)
                for (int px = 5; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, skin);

            // Hair
            for (int i = 5; i <= 10; i++)
                SetPixel(data, width, x + i, y + 4, hairColor);

            // Eyes
            SetPixel(data, width, x + 6, y + 6, outline);
            SetPixel(data, width, x + 9, y + 6, outline);

            // Special features
            if (npcType == "merchant")
            {
                // Mustache
                SetPixel(data, width, x + 6, y + 7, hairColor);
                SetPixel(data, width, x + 7, y + 7, hairColor);
                SetPixel(data, width, x + 8, y + 7, hairColor);
                SetPixel(data, width, x + 9, y + 7, hairColor);
            }

            // Body
            for (int py = 8; py <= 11; py++)
                for (int px = 5; px <= 10; px++)
                    SetPixel(data, width, x + px, y + py, clothing);

            // Legs
            Color legColor = new Color(60, 60, 60);
            for (int py = 12; py <= 14; py++)
            {
                SetPixel(data, width, x + 6, y + py, legColor);
                SetPixel(data, width, x + 7, y + py, legColor);
                SetPixel(data, width, x + 8, y + py, legColor);
                SetPixel(data, width, x + 9, y + py, legColor);
            }
        }

        private void DrawCropSprites(Color[] data, int width)
        {
            // Each crop gets unique, highly recognizable appearance
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
                new Color(101, 67, 33),      // Dark brown seed
                new Color(85, 140, 70),      // Green sprout
                new Color(70, 160, 90),      // Medium green
                new Color(60, 180, 110),     // Bright green leaves
                new Color(255, 248, 220)     // Creamy white parsnip
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawDetailedCrop(data, width, startX, startY, stage, stages[stage], "parsnip");
            }
        }

        private void DrawCauliflowerCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(90, 150, 80),
                new Color(110, 180, 120),
                new Color(130, 200, 140),
                new Color(255, 255, 240)     // Pure white cauliflower head
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawDetailedCrop(data, width, startX, startY, stage, stages[stage], "cauliflower");
            }
        }

        private void DrawPotatoCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(100, 140, 80),
                new Color(120, 160, 110),
                new Color(140, 180, 130),
                new Color(222, 184, 135)     // Tan/brown potato
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawDetailedCrop(data, width, startX, startY, stage, stages[stage], "potato");
            }
        }

        private void DrawTomatoCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(100, 140, 80),
                new Color(120, 180, 130),
                new Color(160, 200, 150),
                new Color(255, 69, 0)        // Bright red tomato
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawDetailedCrop(data, width, startX, startY, stage, stages[stage], "tomato");
            }
        }

        private void DrawCornCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(100, 140, 80),
                new Color(140, 180, 130),
                new Color(180, 200, 150),
                new Color(255, 215, 0)       // Golden yellow corn
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawDetailedCrop(data, width, startX, startY, stage, stages[stage], "corn");
            }
        }

        private void DrawPumpkinCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(100, 140, 80),
                new Color(140, 180, 130),
                new Color(180, 200, 150),
                new Color(255, 117, 24)      // Vivid orange pumpkin
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawDetailedCrop(data, width, startX, startY, stage, stages[stage], "pumpkin");
            }
        }

        private void DrawWheatCrop(Color[] data, int width, int row)
        {
            Color[] stages = new Color[] {
                new Color(101, 67, 33),
                new Color(100, 140, 80),
                new Color(160, 180, 100),
                new Color(200, 200, 120),
                new Color(245, 222, 179)     // Wheat/golden color
            };

            for (int stage = 0; stage < 5; stage++)
            {
                int startX = stage * SPRITE_SIZE;
                int startY = row * SPRITE_SIZE;
                DrawDetailedCrop(data, width, startX, startY, stage, stages[stage], "wheat");
            }
        }

        private void DrawDetailedCrop(Color[] data, int width, int startX, int startY, int stage, Color cropColor, string cropType)
        {
            Color stem = new Color(34, 139, 34);      // Forest green stem
            Color darkGreen = new Color(0, 100, 0);    // Dark green leaves
            Color soil = new Color(101, 67, 33);

            switch (stage)
            {
                case 0: // Seed - very obvious
                    // Dirt mound
                    for (int py = 12; py <= 13; py++)
                        for (int px = 6; px <= 9; px++)
                            SetPixel(data, width, startX + px, startY + py, soil);
                    // Seed
                    SetPixel(data, width, startX + 7, startY + 12, cropColor);
                    SetPixel(data, width, startX + 8, startY + 12, cropColor);
                    break;

                case 1: // Sprout - tiny green shoot
                    for (int py = 12; py <= 13; py++)
                    {
                        SetPixel(data, width, startX + 7, startY + py, stem);
                        SetPixel(data, width, startX + 8, startY + py, stem);
                    }
                    // Leaves
                    SetPixel(data, width, startX + 6, startY + 11, darkGreen);
                    SetPixel(data, width, startX + 7, startY + 11, darkGreen);
                    SetPixel(data, width, startX + 8, startY + 11, darkGreen);
                    SetPixel(data, width, startX + 9, startY + 11, darkGreen);
                    break;

                case 2: // Growing - clear plant shape
                    for (int py = 11; py <= 13; py++)
                        SetPixel(data, width, startX + 8, startY + py, stem);

                    for (int py = 9; py <= 11; py++)
                        for (int px = 6; px <= 10; px++)
                            SetPixel(data, width, startX + px, startY + py, darkGreen);
                    break;

                case 3: // Mature - large plant
                    for (int py = 10; py <= 13; py++)
                        SetPixel(data, width, startX + 8, startY + py, stem);

                    for (int py = 7; py <= 10; py++)
                        for (int px = 5; px <= 11; px++)
                            if (px == 5 || px == 11 || py < 10)
                                SetPixel(data, width, startX + px, startY + py, darkGreen);
                    break;

                case 4: // Harvestable - show the actual crop clearly
                    DrawHarvestableCropDetailed(data, width, startX, startY, cropColor, stem, darkGreen, cropType);
                    break;
            }
        }

        private void DrawHarvestableCropDetailed(Color[] data, int width, int startX, int startY, Color cropColor, Color stem, Color leaves, string cropType)
        {
            Color highlight = new Color(
                Math.Min(255, cropColor.R + 50),
                Math.Min(255, cropColor.G + 50),
                Math.Min(255, cropColor.B + 50)
            );
            Color shadow = new Color(
                cropColor.R / 2,
                cropColor.G / 2,
                cropColor.B / 2
            );

            if (cropType == "tomato")
            {
                // Tall vine
                for (int py = 9; py <= 13; py++)
                    SetPixel(data, width, startX + 8, startY + py, stem);

                // Leaves
                for (int px = 6; px <= 10; px++)
                {
                    SetPixel(data, width, startX + px, startY + 9, leaves);
                    if (px == 6 || px == 10)
                        SetPixel(data, width, startX + px, startY + 10, leaves);
                }

                // Red tomatoes - multiple!
                // Tomato 1
                for (int i = 6; i <= 8; i++)
                    SetPixel(data, width, startX + i, startY + 10, cropColor);
                SetPixel(data, width, startX + 7, startY + 9, cropColor);
                SetPixel(data, width, startX + 7, startY + 11, cropColor);
                SetPixel(data, width, startX + 7, startY + 10, highlight);

                // Tomato 2
                SetPixel(data, width, startX + 10, startY + 11, cropColor);
                SetPixel(data, width, startX + 10, startY + 12, cropColor);
                SetPixel(data, width, startX + 9, startY + 11, cropColor);
            }
            else if (cropType == "corn")
            {
                // Tall stalks
                for (int px = 6; px <= 10; px++)
                    for (int py = 9; py <= 13; py++)
                        SetPixel(data, width, startX + px, startY + py, stem);

                // Corn ears - golden and obvious
                for (int py = 7; py <= 9; py++)
                    for (int px = 6; px <= 10; px++)
                        SetPixel(data, width, startX + px, startY + py, cropColor);

                // Corn kernels detail
                SetPixel(data, width, startX + 7, startY + 8, highlight);
                SetPixel(data, width, startX + 9, startY + 8, highlight);
                SetPixel(data, width, startX + 8, startY + 7, highlight);
            }
            else if (cropType == "pumpkin")
            {
                // Small stem
                SetPixel(data, width, startX + 8, startY + 11, stem);

                // Large round pumpkin with ridges
                for (int py = 10; py <= 13; py++)
                    for (int px = 5; px <= 11; px++)
                    {
                        if (py == 10 && (px < 6 || px > 10)) continue;
                        if (py == 13 && (px < 7 || px > 9)) continue;

                        SetPixel(data, width, startX + px, startY + py, cropColor);
                    }

                // Pumpkin ridges
                SetPixel(data, width, startX + 7, startY + 11, shadow);
                SetPixel(data, width, startX + 7, startY + 12, shadow);
                SetPixel(data, width, startX + 9, startY + 11, shadow);
                SetPixel(data, width, startX + 9, startY + 12, shadow);

                // Highlight
                SetPixel(data, width, startX + 8, startY + 11, highlight);
                SetPixel(data, width, startX + 8, startY + 12, highlight);
            }
            else if (cropType == "cauliflower")
            {
                // Stem
                SetPixel(data, width, startX + 8, startY + 12, stem);

                // Small leaves around
                SetPixel(data, width, startX + 6, startY + 11, leaves);
                SetPixel(data, width, startX + 10, startY + 11, leaves);
                SetPixel(data, width, startX + 5, startY + 12, leaves);
                SetPixel(data, width, startX + 11, startY + 12, leaves);

                // Large white head
                for (int py = 9; py <= 12; py++)
                    for (int px = 6; px <= 10; px++)
                    {
                        if (py == 9 && (px == 6 || px == 10)) continue;
                        SetPixel(data, width, startX + px, startY + py, cropColor);
                    }

                // Cauliflower texture
                SetPixel(data, width, startX + 7, startY + 10, shadow);
                SetPixel(data, width, startX + 9, startY + 10, shadow);
                SetPixel(data, width, startX + 8, startY + 11, shadow);
            }
            else if (cropType == "wheat")
            {
                // Multiple wheat stalks
                for (int px = 5; px <= 11; px++)
                {
                    for (int py = 10; py <= 13; py++)
                        SetPixel(data, width, startX + px, startY + py, stem);

                    // Wheat heads - textured
                    SetPixel(data, width, startX + px, startY + 8, cropColor);
                    SetPixel(data, width, startX + px, startY + 9, cropColor);
                    if (px % 2 == 0)
                        SetPixel(data, width, startX + px, startY + 7, cropColor);
                }
            }
            else // parsnip or potato (underground)
            {
                // Leafy green top
                for (int py = 9; py <= 11; py++)
                    for (int px = 6; px <= 10; px++)
                        SetPixel(data, width, startX + px, startY + py, leaves);

                // Underground crop hint - visible at surface
                for (int px = 6; px <= 10; px++)
                    SetPixel(data, width, startX + px, startY + 12, cropColor);

                for (int px = 7; px <= 9; px++)
                    SetPixel(data, width, startX + px, startY + 13, cropColor);

                // Add some detail
                SetPixel(data, width, startX + 8, startY + 12, highlight);
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

        public static Rectangle GetNPCSprite(string name)
        {
            int row = name switch
            {
                "Pierre" => 11,
                "Emily" => 12,
                "Shane" => 13,
                _ => 11
            };

            return new Rectangle(0, row * SPRITE_SIZE, SPRITE_SIZE, SPRITE_SIZE);
        }
    }
}

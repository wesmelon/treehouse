using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewClone.Systems;

namespace StardewClone
{
    public class Player
    {
        public Vector2 Position { get; set; }
        public Direction Facing { get; private set; }
        public float Speed { get; set; } = 150f; // pixels per second
        public int Energy { get; set; } = 100;
        public int MaxEnergy { get; set; } = 100;

        private Rectangle _bounds;
        private float _animationTime;
        private int _animationFrame;

        public Player(Vector2 startPosition)
        {
            Position = startPosition;
            Facing = Direction.Down;
            _bounds = new Rectangle(0, 0, 24, 32);
        }

        public void Update(GameTime gameTime, KeyboardState keyState, KeyboardState previousKeyState)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 movement = Vector2.Zero;

            // Movement input
            if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
            {
                movement.Y -= 1;
                Facing = Direction.Up;
            }
            if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
            {
                movement.Y += 1;
                Facing = Direction.Down;
            }
            if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
            {
                movement.X -= 1;
                Facing = Direction.Left;
            }
            if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
            {
                movement.X += 1;
                Facing = Direction.Right;
            }

            // Normalize diagonal movement
            if (movement.Length() > 0)
            {
                movement.Normalize();
                Position += movement * Speed * dt;
                _animationTime += dt;
            }

            // Keep player in bounds
            Position = new Vector2(
                MathHelper.Clamp(Position.X, 0, Game1.World.Width * Game1.TILE_SIZE),
                MathHelper.Clamp(Position.Y, 0, Game1.World.Height * Game1.TILE_SIZE)
            );

            // Action buttons
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.Space) ||
                WasKeyJustPressed(keyState, previousKeyState, Keys.E))
            {
                UseCurrentTool();
            }

            // Inventory toggle
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.I))
            {
                Game1.InventorySystem.ToggleInventory();
            }

            // Tool switching
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.D1))
                Game1.InventorySystem.SelectSlot(0);
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.D2))
                Game1.InventorySystem.SelectSlot(1);
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.D3))
                Game1.InventorySystem.SelectSlot(2);
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.D4))
                Game1.InventorySystem.SelectSlot(3);

            // Shop toggle
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.B))
            {
                // Check if near merchant
                var nearbyNPC = Game1.NPCManager.GetNearbyNPC(Position, 64f);
                if (nearbyNPC != null && nearbyNPC.Type == NPCType.Merchant)
                {
                    Game1.ShopSystem.ToggleShop();
                }
            }

            // Save/Load
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.F5))
            {
                Game1.SaveSystem.SaveGame();
            }
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.F9))
            {
                Game1.SaveSystem.LoadGame();
            }

            // Update animation
            if (movement.Length() > 0)
            {
                if (_animationTime > 0.15f)
                {
                    _animationFrame = (_animationFrame + 1) % 4;
                    _animationTime = 0;
                }
            }
            else
            {
                _animationFrame = 0;
            }
        }

        private bool WasKeyJustPressed(KeyboardState current, KeyboardState previous, Keys key)
        {
            return current.IsKeyDown(key) && previous.IsKeyUp(key);
        }

        private void UseCurrentTool()
        {
            var currentItem = Game1.InventorySystem.GetCurrentItem();
            if (currentItem == null) return;

            // Get tile in front of player
            Vector2 targetTile = GetTargetTile();
            int tileX = (int)(targetTile.X / Game1.TILE_SIZE);
            int tileY = (int)(targetTile.Y / Game1.TILE_SIZE);

            if (Energy <= 0) return;

            switch (currentItem.Type)
            {
                case ItemType.Tool_Hoe:
                    Game1.World.TillSoil(tileX, tileY);
                    ConsumeEnergy(2);
                    break;

                case ItemType.Tool_WateringCan:
                    Game1.World.WaterTile(tileX, tileY);
                    ConsumeEnergy(2);
                    break;

                case ItemType.Tool_Scythe:
                    Game1.World.ClearGrass(tileX, tileY);
                    ConsumeEnergy(1);
                    break;

                case ItemType.Tool_Axe:
                    // Could chop trees
                    ConsumeEnergy(5);
                    break;

                case ItemType.Tool_Pickaxe:
                    // Could break rocks
                    ConsumeEnergy(5);
                    break;

                default:
                    // Check if it's a seed
                    if (currentItem.IsSeed())
                    {
                        if (Game1.World.PlantSeed(tileX, tileY, currentItem.Type))
                        {
                            Game1.InventorySystem.RemoveItem(currentItem.Type, 1);
                            ConsumeEnergy(2);
                        }
                    }
                    // Check if trying to harvest
                    else if (keyState.IsKeyDown(Keys.Space))
                    {
                        var harvestedItem = Game1.World.HarvestCrop(tileX, tileY);
                        if (harvestedItem != ItemType.None)
                        {
                            Game1.InventorySystem.AddItem(new Item { Type = harvestedItem, Quantity = 1 });
                            ConsumeEnergy(3);
                        }
                    }
                    break;
            }
        }

        private KeyboardState keyState;

        private void ConsumeEnergy(int amount)
        {
            Energy = System.Math.Max(0, Energy - amount);
        }

        private Vector2 GetTargetTile()
        {
            Vector2 offset = Facing switch
            {
                Direction.Up => new Vector2(0, -Game1.TILE_SIZE),
                Direction.Down => new Vector2(0, Game1.TILE_SIZE),
                Direction.Left => new Vector2(-Game1.TILE_SIZE, 0),
                Direction.Right => new Vector2(Game1.TILE_SIZE, 0),
                _ => Vector2.Zero
            };

            return Position + offset;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw player using sprite library
            Rectangle sourceRect = SpriteLibrary.GetPlayerSprite(Facing, _animationFrame);
            Rectangle destRect = new Rectangle((int)Position.X - 16, (int)Position.Y - 24, 32, 32);

            spriteBatch.Draw(Game1.SpriteLibrary.Texture, destRect, sourceRect, Color.White);

            // Draw direction indicator (simple arrow)
            Vector2 targetTile = GetTargetTile();
            DrawHelper.DrawRectangle(spriteBatch,
                new Rectangle((int)targetTile.X, (int)targetTile.Y, Game1.TILE_SIZE, Game1.TILE_SIZE),
                Color.Yellow * 0.3f);
        }
    }
}

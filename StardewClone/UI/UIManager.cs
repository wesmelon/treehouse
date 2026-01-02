using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StardewClone.UI
{
    public class UIManager
    {
        private const int PANEL_PADDING = 10;
        private const int HOTBAR_SLOT_SIZE = 48;
        private const int INVENTORY_SLOT_SIZE = 40;

        public void Update(GameTime gameTime, MouseState mouseState, MouseState previousMouseState)
        {
            // Handle UI interactions here
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawHUD(spriteBatch);

            if (Game1.InventorySystem.IsInventoryOpen)
            {
                DrawInventory(spriteBatch);
            }

            if (Game1.ShopSystem.IsShopOpen)
            {
                DrawShop(spriteBatch);
            }
        }

        private void DrawHUD(SpriteBatch spriteBatch)
        {
            int screenWidth = Game1.SCREEN_WIDTH;
            int screenHeight = Game1.SCREEN_HEIGHT;

            // Top bar background
            DrawHelper.DrawRectangle(spriteBatch,
                new Rectangle(0, 0, screenWidth, 80),
                new Color(0, 0, 0, 180));

            // Time and Date
            string timeText = $"{Game1.TimeSystem.TimeString}  {Game1.TimeSystem.DateString}";
            DrawHelper.DrawText(spriteBatch, timeText, new Vector2(20, 15), Color.White);

            // Weather
            string weatherText = $"Weather: {Game1.TimeSystem.CurrentWeather}";
            DrawHelper.DrawText(spriteBatch, weatherText, new Vector2(20, 35), Color.LightBlue);

            // Money
            string moneyText = $"${Game1.InventorySystem.Money}";
            DrawHelper.DrawText(spriteBatch, moneyText, new Vector2(screenWidth - 150, 15), Color.Gold);

            // Energy
            DrawEnergyBar(spriteBatch, new Vector2(screenWidth - 250, 45), 200, 20);

            // Hotbar
            DrawHotbar(spriteBatch);

            // Controls hint
            DrawHelper.DrawText(spriteBatch, "Controls: WASD/Arrows=Move, Space=Action, I=Inventory, B=Shop, F5=Save, F9=Load",
                new Vector2(20, screenHeight - 30), Color.White);
        }

        private void DrawEnergyBar(SpriteBatch spriteBatch, Vector2 position, int width, int height)
        {
            // Background
            DrawHelper.DrawRectangle(spriteBatch,
                new Rectangle((int)position.X, (int)position.Y, width, height),
                Color.DarkGray);

            // Energy fill
            float energyPercent = (float)Game1.Player.Energy / Game1.Player.MaxEnergy;
            int fillWidth = (int)(width * energyPercent);
            Color energyColor = energyPercent > 0.5f ? Color.LimeGreen :
                                energyPercent > 0.25f ? Color.Yellow : Color.Red;

            DrawHelper.DrawRectangle(spriteBatch,
                new Rectangle((int)position.X, (int)position.Y, fillWidth, height),
                energyColor);

            // Border
            DrawHelper.DrawRectangleOutline(spriteBatch,
                new Rectangle((int)position.X, (int)position.Y, width, height),
                Color.White, 2);

            // Text
            DrawHelper.DrawText(spriteBatch, $"Energy: {Game1.Player.Energy}/{Game1.Player.MaxEnergy}",
                new Vector2(position.X + 5, position.Y + 2), Color.White);
        }

        private void DrawHotbar(SpriteBatch spriteBatch)
        {
            int screenWidth = Game1.SCREEN_WIDTH;
            int screenHeight = Game1.SCREEN_HEIGHT;
            int hotbarY = screenHeight - HOTBAR_SLOT_SIZE - 20;
            int hotbarX = (screenWidth - (HOTBAR_SLOT_SIZE * Game1.InventorySystem.HotbarSize)) / 2;

            for (int i = 0; i < Game1.InventorySystem.HotbarSize; i++)
            {
                int x = hotbarX + i * HOTBAR_SLOT_SIZE;
                Rectangle slotRect = new Rectangle(x, hotbarY, HOTBAR_SLOT_SIZE, HOTBAR_SLOT_SIZE);

                // Slot background
                Color bgColor = i == Game1.InventorySystem.SelectedSlot ?
                    new Color(100, 200, 255, 200) : new Color(60, 60, 60, 200);
                DrawHelper.DrawRectangle(spriteBatch, slotRect, bgColor);
                DrawHelper.DrawRectangleOutline(spriteBatch, slotRect, Color.White, 2);

                // Item
                if (i < Game1.InventorySystem.Items.Count)
                {
                    var item = Game1.InventorySystem.Items[i];
                    DrawItem(spriteBatch, item, slotRect);
                }

                // Slot number
                DrawHelper.DrawText(spriteBatch, (i + 1).ToString(),
                    new Vector2(x + 5, hotbarY + 5), Color.White);
            }
        }

        private void DrawInventory(SpriteBatch spriteBatch)
        {
            int screenWidth = Game1.SCREEN_WIDTH;
            int screenHeight = Game1.SCREEN_HEIGHT;

            // Semi-transparent background
            DrawHelper.DrawRectangle(spriteBatch,
                new Rectangle(0, 0, screenWidth, screenHeight),
                new Color(0, 0, 0, 200));

            // Inventory panel
            int panelWidth = 600;
            int panelHeight = 500;
            int panelX = (screenWidth - panelWidth) / 2;
            int panelY = (screenHeight - panelHeight) / 2;

            DrawHelper.DrawRectangle(spriteBatch,
                new Rectangle(panelX, panelY, panelWidth, panelHeight),
                new Color(40, 40, 40, 255));

            DrawHelper.DrawText(spriteBatch, "INVENTORY (Press I to close)",
                new Vector2(panelX + 20, panelY + 20), Color.White);

            // Draw inventory grid
            int cols = 6;
            int rows = 6;
            int startX = panelX + 20;
            int startY = panelY + 60;

            for (int i = 0; i < Game1.InventorySystem.Items.Count && i < cols * rows; i++)
            {
                int col = i % cols;
                int row = i / cols;
                int x = startX + col * (INVENTORY_SLOT_SIZE + 5);
                int y = startY + row * (INVENTORY_SLOT_SIZE + 5);

                Rectangle slotRect = new Rectangle(x, y, INVENTORY_SLOT_SIZE, INVENTORY_SLOT_SIZE);
                DrawHelper.DrawRectangle(spriteBatch, slotRect, Color.DarkGray);
                DrawHelper.DrawRectangleOutline(spriteBatch, slotRect, Color.White, 1);

                DrawItem(spriteBatch, Game1.InventorySystem.Items[i], slotRect);
            }
        }

        private void DrawShop(SpriteBatch spriteBatch)
        {
            int screenWidth = Game1.SCREEN_WIDTH;
            int screenHeight = Game1.SCREEN_HEIGHT;

            // Semi-transparent background
            DrawHelper.DrawRectangle(spriteBatch,
                new Rectangle(0, 0, screenWidth, screenHeight),
                new Color(0, 0, 0, 200));

            // Shop panel
            int panelWidth = 500;
            int panelHeight = 600;
            int panelX = (screenWidth - panelWidth) / 2;
            int panelY = (screenHeight - panelHeight) / 2;

            DrawHelper.DrawRectangle(spriteBatch,
                new Rectangle(panelX, panelY, panelWidth, panelHeight),
                new Color(60, 40, 20, 255));

            DrawHelper.DrawText(spriteBatch, "PIERRE'S SHOP (Press B to close)",
                new Vector2(panelX + 20, panelY + 20), Color.White);

            DrawHelper.DrawText(spriteBatch, $"Your Money: ${Game1.InventorySystem.Money}",
                new Vector2(panelX + 20, panelY + 45), Color.Gold);

            // Draw shop items
            int startY = panelY + 80;
            for (int i = 0; i < Game1.ShopSystem.ShopInventory.Count; i++)
            {
                var shopItem = Game1.ShopSystem.ShopInventory[i];
                int y = startY + i * 40;
                bool isSelected = i == Game1.ShopSystem.SelectedItemIndex;

                Color bgColor = isSelected ? new Color(100, 200, 100, 100) : Color.Transparent;
                DrawHelper.DrawRectangle(spriteBatch,
                    new Rectangle(panelX + 20, y, panelWidth - 40, 35),
                    bgColor);

                string itemName = shopItem.Type.ToString().Replace("_", " ");
                string stockText = shopItem.Stock == -1 ? "" : $" (Stock: {shopItem.Stock})";
                string text = $"{itemName} - ${shopItem.Price}{stockText}";

                DrawHelper.DrawText(spriteBatch, text,
                    new Vector2(panelX + 30, y + 8),
                    isSelected ? Color.Yellow : Color.White);
            }

            DrawHelper.DrawText(spriteBatch, "Use Arrow Keys to select, Enter/Space to buy",
                new Vector2(panelX + 20, panelY + panelHeight - 40), Color.LightGray);
        }

        private void DrawItem(SpriteBatch spriteBatch, Item item, Rectangle rect)
        {
            if (item == null) return;

            // Simple colored square for item
            Color itemColor = GetItemColor(item.Type);
            Rectangle itemRect = new Rectangle(
                rect.X + 8, rect.Y + 8,
                rect.Width - 16, rect.Height - 16
            );
            DrawHelper.DrawRectangle(spriteBatch, itemRect, itemColor);

            // Quantity
            if (!item.IsTool() && item.Quantity > 1)
            {
                DrawHelper.DrawText(spriteBatch, item.Quantity.ToString(),
                    new Vector2(rect.X + rect.Width - 15, rect.Y + rect.Height - 15),
                    Color.White);
            }
        }

        private Color GetItemColor(ItemType type)
        {
            if (type.ToString().StartsWith("Seed_")) return Color.Brown;
            if (type.ToString().StartsWith("Crop_")) return Color.Green;
            if (type.ToString().StartsWith("Tool_")) return Color.Silver;
            return Color.Gray;
        }
    }
}

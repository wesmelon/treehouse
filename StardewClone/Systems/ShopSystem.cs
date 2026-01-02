using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace StardewClone.Systems
{
    public class ShopItem
    {
        public ItemType Type { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; } = -1; // -1 means unlimited
    }

    public class ShopSystem
    {
        public bool IsShopOpen { get; private set; } = false;
        public List<ShopItem> ShopInventory { get; private set; }
        public int SelectedItemIndex { get; private set; } = 0;

        public ShopSystem()
        {
            InitializeShop();
        }

        private void InitializeShop()
        {
            ShopInventory = new List<ShopItem>
            {
                // Seeds
                new ShopItem { Type = ItemType.Seed_Parsnip, Price = 20 },
                new ShopItem { Type = ItemType.Seed_Cauliflower, Price = 80 },
                new ShopItem { Type = ItemType.Seed_Potato, Price = 50 },
                new ShopItem { Type = ItemType.Seed_Tomato, Price = 50 },
                new ShopItem { Type = ItemType.Seed_Corn, Price = 150 },
                new ShopItem { Type = ItemType.Seed_Pumpkin, Price = 100 },
                new ShopItem { Type = ItemType.Seed_Wheat, Price = 10 },

                // Tools (limited stock)
                new ShopItem { Type = ItemType.Tool_Axe, Price = 2000, Stock = 1 },
                new ShopItem { Type = ItemType.Tool_Pickaxe, Price = 2000, Stock = 1 },
            };
        }

        public void Update(KeyboardState keyState, KeyboardState previousKeyState)
        {
            if (!IsShopOpen) return;

            // Navigation
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.Up))
            {
                SelectedItemIndex = System.Math.Max(0, SelectedItemIndex - 1);
            }
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.Down))
            {
                SelectedItemIndex = System.Math.Min(ShopInventory.Count - 1, SelectedItemIndex + 1);
            }

            // Buy item
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.Enter) ||
                WasKeyJustPressed(keyState, previousKeyState, Keys.Space))
            {
                BuySelectedItem();
            }

            // Close shop
            if (WasKeyJustPressed(keyState, previousKeyState, Keys.Escape) ||
                WasKeyJustPressed(keyState, previousKeyState, Keys.B))
            {
                ToggleShop();
            }
        }

        private bool WasKeyJustPressed(KeyboardState current, KeyboardState previous, Keys key)
        {
            return current.IsKeyDown(key) && previous.IsKeyUp(key);
        }

        private void BuySelectedItem()
        {
            if (SelectedItemIndex < 0 || SelectedItemIndex >= ShopInventory.Count)
                return;

            var shopItem = ShopInventory[SelectedItemIndex];

            // Check stock
            if (shopItem.Stock == 0) return;

            // Try to buy
            if (Game1.InventorySystem.BuyItem(shopItem.Type, 1))
            {
                if (shopItem.Stock > 0)
                {
                    shopItem.Stock--;
                }
            }
        }

        public void ToggleShop()
        {
            IsShopOpen = !IsShopOpen;
            if (IsShopOpen)
            {
                SelectedItemIndex = 0;
            }
        }

        public void SellItem(ItemType type, int quantity)
        {
            Game1.InventorySystem.SellItem(type, quantity);
        }
    }
}

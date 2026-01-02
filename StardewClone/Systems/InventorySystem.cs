using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace StardewClone.Systems
{
    public class InventorySystem
    {
        public List<Item> Items { get; private set; }
        public int MaxSlots { get; set; } = 36;
        public int HotbarSize { get; set; } = 10;
        public int SelectedSlot { get; private set; } = 0;
        public int Money { get; set; } = 0;
        public bool IsInventoryOpen { get; private set; } = false;

        public InventorySystem()
        {
            Items = new List<Item>();
        }

        public void AddItem(Item item)
        {
            // Try to stack with existing item
            var existingItem = Items.FirstOrDefault(i => i.Type == item.Type);
            if (existingItem != null && !existingItem.IsTool())
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                if (Items.Count < MaxSlots)
                {
                    Items.Add(item);
                }
            }
        }

        public void RemoveItem(ItemType type, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.Type == type);
            if (item != null)
            {
                item.Quantity -= quantity;
                if (item.Quantity <= 0)
                {
                    Items.Remove(item);
                }
            }
        }

        public Item GetCurrentItem()
        {
            if (SelectedSlot >= 0 && SelectedSlot < Items.Count)
                return Items[SelectedSlot];
            return null;
        }

        public void SelectSlot(int slot)
        {
            if (slot >= 0 && slot < HotbarSize)
                SelectedSlot = slot;
        }

        public void ToggleInventory()
        {
            IsInventoryOpen = !IsInventoryOpen;
        }

        public bool HasItem(ItemType type, int quantity = 1)
        {
            var item = Items.FirstOrDefault(i => i.Type == type);
            return item != null && item.Quantity >= quantity;
        }

        public int GetItemCount(ItemType type)
        {
            var item = Items.FirstOrDefault(i => i.Type == type);
            return item?.Quantity ?? 0;
        }

        public void SellItem(ItemType type, int quantity)
        {
            if (HasItem(type, quantity))
            {
                RemoveItem(type, quantity);
                Money += ItemDatabase.GetSellPrice(type) * quantity;
            }
        }

        public bool BuyItem(ItemType type, int quantity)
        {
            int cost = ItemDatabase.GetBuyPrice(type) * quantity;
            if (Money >= cost)
            {
                Money -= cost;
                AddItem(new Item { Type = type, Quantity = quantity });
                return true;
            }
            return false;
        }
    }
}

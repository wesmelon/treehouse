using System.Collections.Generic;

namespace StardewClone
{
    public class Item
    {
        public ItemType Type { get; set; }
        public int Quantity { get; set; }

        public string GetName()
        {
            return Type.ToString().Replace("_", " ");
        }

        public int GetSellPrice()
        {
            return ItemDatabase.GetSellPrice(Type);
        }

        public int GetBuyPrice()
        {
            return ItemDatabase.GetBuyPrice(Type);
        }

        public bool IsTool()
        {
            return Type.ToString().StartsWith("Tool_");
        }

        public bool IsSeed()
        {
            return Type.ToString().StartsWith("Seed_");
        }

        public bool IsCrop()
        {
            return Type.ToString().StartsWith("Crop_");
        }
    }

    public static class ItemDatabase
    {
        private static Dictionary<ItemType, int> _sellPrices = new Dictionary<ItemType, int>
        {
            // Crops
            { ItemType.Crop_Parsnip, 35 },
            { ItemType.Crop_Cauliflower, 175 },
            { ItemType.Crop_Potato, 80 },
            { ItemType.Crop_Tomato, 60 },
            { ItemType.Crop_Corn, 50 },
            { ItemType.Crop_Pumpkin, 320 },
            { ItemType.Crop_Wheat, 25 },

            // Resources
            { ItemType.Wood, 2 },
            { ItemType.Stone, 2 },
            { ItemType.Fiber, 1 },
        };

        private static Dictionary<ItemType, int> _buyPrices = new Dictionary<ItemType, int>
        {
            // Seeds
            { ItemType.Seed_Parsnip, 20 },
            { ItemType.Seed_Cauliflower, 80 },
            { ItemType.Seed_Potato, 50 },
            { ItemType.Seed_Tomato, 50 },
            { ItemType.Seed_Corn, 150 },
            { ItemType.Seed_Pumpkin, 100 },
            { ItemType.Seed_Wheat, 10 },

            // Tools
            { ItemType.Tool_Axe, 2000 },
            { ItemType.Tool_Pickaxe, 2000 },
        };

        public static int GetSellPrice(ItemType type)
        {
            return _sellPrices.ContainsKey(type) ? _sellPrices[type] : 0;
        }

        public static int GetBuyPrice(ItemType type)
        {
            return _buyPrices.ContainsKey(type) ? _buyPrices[type] : 0;
        }

        public static ItemType GetCropFromSeed(ItemType seedType)
        {
            var seedName = seedType.ToString().Replace("Seed_", "");
            return (ItemType)System.Enum.Parse(typeof(ItemType), "Crop_" + seedName);
        }

        public static int GetGrowthTime(ItemType seedType)
        {
            // Return days to maturity
            switch (seedType)
            {
                case ItemType.Seed_Parsnip: return 4;
                case ItemType.Seed_Cauliflower: return 12;
                case ItemType.Seed_Potato: return 6;
                case ItemType.Seed_Tomato: return 11;
                case ItemType.Seed_Corn: return 14;
                case ItemType.Seed_Pumpkin: return 13;
                case ItemType.Seed_Wheat: return 4;
                default: return 7;
            }
        }
    }
}

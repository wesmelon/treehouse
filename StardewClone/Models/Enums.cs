namespace StardewClone
{
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

    public enum Weather
    {
        Sunny,
        Rainy,
        Snowy,
        Stormy
    }

    public enum ItemType
    {
        // Seeds
        Seed_Parsnip,
        Seed_Cauliflower,
        Seed_Potato,
        Seed_Tomato,
        Seed_Corn,
        Seed_Pumpkin,
        Seed_Wheat,

        // Crops
        Crop_Parsnip,
        Crop_Cauliflower,
        Crop_Potato,
        Crop_Tomato,
        Crop_Corn,
        Crop_Pumpkin,
        Crop_Wheat,

        // Tools
        Tool_Hoe,
        Tool_WateringCan,
        Tool_Axe,
        Tool_Pickaxe,
        Tool_Scythe,

        // Resources
        Wood,
        Stone,
        Fiber,

        // Special
        None
    }

    public enum TileType
    {
        Grass,
        Dirt,
        Tilled,
        Watered,
        Stone,
        Wood,
        Water,
        Sand
    }

    public enum CropGrowthStage
    {
        Seed,
        Sprout,
        Growing,
        Mature,
        Harvestable
    }

    public enum NPCType
    {
        Villager,
        Merchant,
        Quest
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}

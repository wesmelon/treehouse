# Stardew Valley Clone

A full-fledged farming simulation game inspired by Stardew Valley, built with C# and MonoGame for Windows.

## Features

### Core Gameplay
- **Player Character**: Smooth movement with WASD/Arrow keys and animated sprites
- **Tile-based World**: Large explorable 100x100 tile world with varied terrain
- **Farming System**: Complete crop lifecycle with planting, watering, growing, and harvesting
- **Tool System**: Hoe, Watering Can, Scythe, Axe, and Pickaxe
- **Inventory Management**: 36-slot inventory with 10-slot hotbar
- **Time System**: Dynamic day/night cycle with seasons (Spring, Summer, Fall, Winter)
- **Weather System**: Sunny, Rainy, Snowy, and Stormy weather with crop effects
- **Energy System**: Stamina management for performing actions
- **Economy**: Money system with buying and selling items
- **NPC System**: Interactive villagers with dialogue and friendship levels
- **Shop System**: Buy seeds and tools from Pierre's shop
- **Save/Load**: Full game state persistence with JSON serialization

### Crops
- **Parsnip** - 4 days to maturity
- **Cauliflower** - 12 days to maturity
- **Potato** - 6 days to maturity
- **Tomato** - 11 days to maturity
- **Corn** - 14 days to maturity
- **Pumpkin** - 13 days to maturity
- **Wheat** - 4 days to maturity

### Game Mechanics
- Crops must be watered daily to grow
- Rainy weather automatically waters all crops
- Energy is consumed when using tools
- Energy fully restores each new day
- Crops advance growth stages each day when watered
- Different crops have different sell prices
- Seasons change every 28 days
- NPCs wander around and have unique dialogue

## Controls

### Movement
- **WASD** or **Arrow Keys**: Move player
- **Mouse**: (Future) Click to move

### Actions
- **Space** or **E**: Use equipped tool/item
- **1-9 Keys**: Select hotbar slot
- **I**: Toggle inventory
- **B**: Open shop (when near merchant NPC)
- **F5**: Quick save
- **F9**: Quick load
- **Escape**: Exit game

### Tool Usage
1. Select a tool from your hotbar (keys 1-4)
2. Face the tile you want to use it on
3. Press Space to use the tool

### Farming Workflow
1. **Till soil**: Equip Hoe (slot 2) and press Space on grass/dirt
2. **Plant seeds**: Equip seeds and press Space on tilled soil
3. **Water crops**: Equip Watering Can (slot 1) and press Space on tilled/planted soil
4. **Harvest**: When crops are fully grown, press Space to harvest

## Building and Running

### Prerequisites
- Windows 10/11
- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/download/dotnet/6.0)
- (Optional) Visual Studio 2022 or Visual Studio Code

### Quick Start

#### Option 1: Using Batch Files (Easiest)
1. Double-click `build.bat` to compile the game
2. Double-click `run.bat` to start playing

#### Option 2: Command Line
```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build -c Release

# Run the game
dotnet run
```

#### Option 3: Visual Studio
1. Open `StardewClone.csproj` in Visual Studio 2022
2. Press F5 to build and run
3. Or use Build → Build Solution, then Debug → Start Without Debugging

### Creating a Standalone Executable

To create a distributable Windows executable:

```bash
# Publish as self-contained Windows application
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

# The executable will be in:
# bin/Release/net6.0-windows/win-x64/publish/StardewClone.exe
```

You can then copy the entire `publish` folder to any Windows PC and run `StardewClone.exe` without needing .NET installed.

## Game Structure

```
StardewClone/
├── Content/              # Game assets (sprites, sounds, etc.)
├── Models/              # Data models and enums
│   ├── Enums.cs         # Season, Weather, ItemType, etc.
│   ├── Item.cs          # Item and ItemDatabase classes
│   └── Player.cs        # Player character logic
├── Systems/             # Core game systems
│   ├── Camera.cs        # Camera follow system
│   ├── World.cs         # Tile-based world with farming
│   ├── TimeSystem.cs    # Time, day, season, weather
│   ├── InventorySystem.cs # Item management
│   ├── ShopSystem.cs    # Buying and selling
│   ├── NPCManager.cs    # NPC behavior and dialogue
│   └── SaveSystem.cs    # Save/load functionality
├── Sprites/             # Rendering helpers
│   ├── SpriteLibrary.cs # Sprite atlas management
│   └── DrawHelper.cs    # Drawing utilities
├── UI/                  # User interface
│   └── UIManager.cs     # HUD, inventory, shop UI
├── Game1.cs             # Main game class
├── Program.cs           # Entry point
└── README.md           # This file
```

## Save System

Game saves are stored in JSON format in the `Saves/` directory:
- **F5**: Quick save
- **F9**: Quick load
- Save file: `Saves/savegame.json`

The save includes:
- Player position and energy
- Inventory and money
- All world tiles and crops
- Time, day, season, and year
- Weather conditions

## Tips for Playing

1. **Start Small**: Plant a few parsnips to get started quickly
2. **Water Daily**: Crops only grow when watered (except on rainy days!)
3. **Manage Energy**: Keep an eye on your energy bar - you need it to work
4. **Talk to NPCs**: Build friendships by talking to villagers
5. **Buy Smart**: Invest in profitable crops like cauliflower and pumpkin
6. **Save Often**: Use F5 to save your progress regularly
7. **Use the Scythe**: Clear grass with the scythe to get fiber and clear farmland

## Future Enhancements

Potential features for future development:
- Animal husbandry (chickens, cows, etc.)
- Fishing system
- Mining and combat
- Crafting system
- Marriage and relationships
- Community center bundles
- Seasonal festivals
- Achievements
- Multiple farms/maps
- Multiplayer support

## Technical Details

- **Engine**: MonoGame 3.8.1
- **Framework**: .NET 6.0
- **Platform**: Windows (DirectX)
- **Resolution**: 1280x720 (windowed)
- **Rendering**: 2D sprite-based with pixel art scaling
- **Save Format**: JSON with Newtonsoft.Json

## License

This is a fan project created for educational purposes. Stardew Valley is created by ConcernedApe.

## Credits

- Built with [MonoGame](https://www.monogame.net/)
- Inspired by [Stardew Valley](https://www.stardewvalley.net/) by ConcernedApe
- Created as a learning project

---

**Enjoy farming! 🌱🚜🌾**

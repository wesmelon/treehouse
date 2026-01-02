using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewClone.Systems;
using StardewClone.UI;
using System;

namespace StardewClone
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Core systems
        public static World World { get; private set; }
        public static Player Player { get; private set; }
        public static TimeSystem TimeSystem { get; private set; }
        public static InventorySystem InventorySystem { get; private set; }
        public static ShopSystem ShopSystem { get; private set; }
        public static NPCManager NPCManager { get; private set; }
        public static SaveSystem SaveSystem { get; private set; }

        // Rendering
        private Camera _camera;
        private UIManager _uiManager;

        // Assets
        public static SpriteFont Font { get; private set; }
        public static Texture2D PixelTexture { get; private set; }
        public static SpriteLibrary SpriteLibrary { get; private set; }

        // Input tracking
        private KeyboardState _previousKeyState;
        private MouseState _previousMouseState;

        public const int TILE_SIZE = 32;
        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 720;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Initialize camera
            _camera = new Camera(GraphicsDevice.Viewport);

            // Initialize systems
            TimeSystem = new TimeSystem();
            World = new World(100, 100); // 100x100 tile world
            Player = new Player(new Vector2(50 * TILE_SIZE, 50 * TILE_SIZE));
            InventorySystem = new InventorySystem();
            ShopSystem = new ShopSystem();
            NPCManager = new NPCManager();
            SaveSystem = new SaveSystem();
            _uiManager = new UIManager();

            // Give player starting items
            InventorySystem.AddItem(new Item { Type = ItemType.Seed_Parsnip, Quantity = 15 });
            InventorySystem.AddItem(new Item { Type = ItemType.Tool_WateringCan, Quantity = 1 });
            InventorySystem.AddItem(new Item { Type = ItemType.Tool_Hoe, Quantity = 1 });
            InventorySystem.AddItem(new Item { Type = ItemType.Tool_Scythe, Quantity = 1 });
            InventorySystem.Money = 500;

            // Spawn NPCs
            NPCManager.SpawnNPC("Pierre", new Vector2(45 * TILE_SIZE, 45 * TILE_SIZE), NPCType.Merchant);
            NPCManager.SpawnNPC("Emily", new Vector2(55 * TILE_SIZE, 48 * TILE_SIZE), NPCType.Villager);
            NPCManager.SpawnNPC("Shane", new Vector2(52 * TILE_SIZE, 52 * TILE_SIZE), NPCType.Villager);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a 1x1 white pixel texture for drawing shapes
            PixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            PixelTexture.SetData(new[] { Color.White });

            // Load sprite library
            SpriteLibrary = new SpriteLibrary(GraphicsDevice);

            // Note: In a real MonoGame project, you'd load this through Content Pipeline
            // For this demo, we'll create a basic font texture
            Font = CreateBasicFont();
        }

        private SpriteFont CreateBasicFont()
        {
            // This is a placeholder - in a real project you'd use Content.Load<SpriteFont>
            // For now we'll handle text rendering differently
            return null;
        }

        protected override void Update(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyState.IsKeyDown(Keys.Escape))
                Exit();

            // Update systems
            TimeSystem.Update(gameTime);
            Player.Update(gameTime, keyState, _previousKeyState);
            NPCManager.Update(gameTime);
            World.Update(gameTime);
            ShopSystem.Update(keyState, _previousKeyState);
            _uiManager.Update(gameTime, mouseState, _previousMouseState);

            // Camera follows player
            _camera.Follow(Player.Position);

            _previousKeyState = keyState;
            _previousMouseState = mouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(34, 139, 34)); // Green background

            // Draw world space (with camera transform)
            _spriteBatch.Begin(transformMatrix: _camera.Transform, samplerState: SamplerState.PointClamp);

            World.Draw(_spriteBatch);
            NPCManager.Draw(_spriteBatch);
            Player.Draw(_spriteBatch);

            _spriteBatch.End();

            // Draw UI (screen space)
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _uiManager.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

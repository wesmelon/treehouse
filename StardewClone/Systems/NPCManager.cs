using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewClone.Systems
{
    public class NPC
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public NPCType Type { get; set; }
        public int FriendshipLevel { get; set; } = 0;
        public List<string> Dialogue { get; set; } = new List<string>();
        public int CurrentDialogueIndex { get; set; } = 0;

        private Vector2 _targetPosition;
        private Random _random = new Random();
        private float _moveTimer = 0;
        private const float MOVE_INTERVAL = 3.0f;

        public NPC(string name, Vector2 position, NPCType type)
        {
            Name = name;
            Position = position;
            Type = type;
            _targetPosition = position;
            InitializeDialogue();
        }

        private void InitializeDialogue()
        {
            switch (Name)
            {
                case "Pierre":
                    Dialogue.Add("Welcome to my shop! I have the best seeds in town.");
                    Dialogue.Add("The crops are growing well this season.");
                    Dialogue.Add("Don't forget to water your crops daily!");
                    break;
                case "Emily":
                    Dialogue.Add("Hi there! Beautiful day, isn't it?");
                    Dialogue.Add("I love watching things grow in the garden.");
                    Dialogue.Add("Have you tried growing cauliflower? It's wonderful!");
                    break;
                case "Shane":
                    Dialogue.Add("Hey... what do you want?");
                    Dialogue.Add("I'm busy. Talk later.");
                    Dialogue.Add("The farm life... it's peaceful, I guess.");
                    break;
                default:
                    Dialogue.Add("Hello!");
                    Dialogue.Add("Nice weather today.");
                    break;
            }
        }

        public string GetDialogue()
        {
            if (Dialogue.Count == 0) return "...";
            string dialogue = Dialogue[CurrentDialogueIndex];
            CurrentDialogueIndex = (CurrentDialogueIndex + 1) % Dialogue.Count;
            return dialogue;
        }

        public void Update(GameTime gameTime)
        {
            if (Type == NPCType.Villager)
            {
                // Simple wandering behavior
                _moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_moveTimer >= MOVE_INTERVAL)
                {
                    _moveTimer = 0;
                    // Pick a new random target nearby
                    float angle = (float)(_random.NextDouble() * Math.PI * 2);
                    float distance = 32 + (float)_random.NextDouble() * 96;
                    _targetPosition = Position + new Vector2(
                        (float)Math.Cos(angle) * distance,
                        (float)Math.Sin(angle) * distance
                    );
                }

                // Move towards target
                Vector2 direction = _targetPosition - Position;
                if (direction.Length() > 1)
                {
                    direction.Normalize();
                    Position += direction * 30f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }

        public void IncreaseFriendship(int amount)
        {
            FriendshipLevel = Math.Min(100, FriendshipLevel + amount);
        }
    }

    public class NPCManager
    {
        private List<NPC> _npcs = new List<NPC>();

        public void SpawnNPC(string name, Vector2 position, NPCType type)
        {
            _npcs.Add(new NPC(name, position, type));
        }

        public void Update(GameTime gameTime)
        {
            foreach (var npc in _npcs)
            {
                npc.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var npc in _npcs)
            {
                // Draw NPC
                Rectangle npcRect = new Rectangle(
                    (int)npc.Position.X - 16,
                    (int)npc.Position.Y - 24,
                    32,
                    32
                );

                Color npcColor = npc.Type switch
                {
                    NPCType.Merchant => new Color(255, 215, 0),
                    NPCType.Villager => new Color(100, 149, 237),
                    _ => Color.Purple
                };

                spriteBatch.Draw(Game1.PixelTexture, npcRect, npcColor);

                // Draw name tag
                if (Game1.Font != null)
                {
                    DrawHelper.DrawText(spriteBatch, npc.Name,
                        new Vector2(npc.Position.X - 20, npc.Position.Y - 40),
                        Color.White);
                }
            }
        }

        public NPC GetNearbyNPC(Vector2 position, float maxDistance)
        {
            return _npcs.FirstOrDefault(npc =>
                Vector2.Distance(npc.Position, position) <= maxDistance);
        }

        public List<NPC> GetAllNPCs()
        {
            return new List<NPC>(_npcs);
        }
    }
}

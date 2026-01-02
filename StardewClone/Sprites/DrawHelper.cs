using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewClone
{
    public static class DrawHelper
    {
        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            spriteBatch.Draw(Game1.PixelTexture, rect, color);
        }

        public static void DrawRectangleOutline(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness)
        {
            // Top
            spriteBatch.Draw(Game1.PixelTexture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            // Bottom
            spriteBatch.Draw(Game1.PixelTexture, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
            // Left
            spriteBatch.Draw(Game1.PixelTexture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            // Right
            spriteBatch.Draw(Game1.PixelTexture, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
        }

        public static void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            // Simple bitmap font rendering
            // In a real game, you'd use SpriteFont
            // For now, we'll draw simple rectangles as placeholder text
            int charWidth = 8;
            int charHeight = 12;

            for (int i = 0; i < text.Length; i++)
            {
                Vector2 charPos = position + new Vector2(i * charWidth, 0);
                Rectangle charRect = new Rectangle((int)charPos.X, (int)charPos.Y, charWidth - 2, charHeight);

                // Very simple character representation
                spriteBatch.Draw(Game1.PixelTexture, charRect, color * 0.8f);
            }
        }
    }
}

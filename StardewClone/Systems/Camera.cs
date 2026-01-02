using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewClone.Systems
{
    public class Camera
    {
        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 2.0f; // 2x zoom for pixel art
        public Matrix Transform { get; private set; }

        private Viewport _viewport;

        public Camera(Viewport viewport)
        {
            _viewport = viewport;
            Position = Vector2.Zero;
        }

        public void Follow(Vector2 target)
        {
            // Smooth camera follow
            Vector2 desiredPosition = target - new Vector2(_viewport.Width / (2 * Zoom), _viewport.Height / (2 * Zoom));
            Position = Vector2.Lerp(Position, desiredPosition, 0.1f);

            UpdateTransform();
        }

        private void UpdateTransform()
        {
            Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                       Matrix.CreateScale(Zoom) *
                       Matrix.CreateTranslation(_viewport.Width / 2f, _viewport.Height / 2f, 0);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(Transform));
        }
    }
}

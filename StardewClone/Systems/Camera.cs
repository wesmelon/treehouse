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
            // Center camera on target (player)
            // Account for zoom when calculating the center offset
            Vector2 desiredPosition = target - new Vector2(_viewport.Width / (2 * Zoom), _viewport.Height / (2 * Zoom));

            // Smooth camera follow with higher lerp factor for more responsive following
            Position = Vector2.Lerp(Position, desiredPosition, 0.2f);

            UpdateTransform();
        }

        private void UpdateTransform()
        {
            // Create transformation matrix that centers the camera on the target
            Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                       Matrix.CreateScale(Zoom, Zoom, 1) *
                       Matrix.CreateTranslation(_viewport.Width / 2f, _viewport.Height / 2f, 0);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(Transform));
        }
    }
}

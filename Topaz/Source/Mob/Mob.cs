using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topaz.Mob
{
    class Mob
    {
        Vector2 _position;

        public Texture2D Sprite { get; set; }
        public Vector2 Position { get => _position; set => _position = value; }
        public Rectangle SpriteBounds { get; set; }
        public Rectangle CollisionBounds { get; set; }
        public int Speed { get; set; }
        public int Direction { get; set; }
        public float AnimationFrame { get; set; }

        public Mob()
        {
            this.Position = new Vector2(5, 5);
            this.Speed = 10;
            Direction = 0;
            AnimationFrame = 0;
        }

        public void Move(float deltaX, float deltaY)
        {
            _position.X += deltaX;
            _position.Y += deltaY;
        }
    }
}

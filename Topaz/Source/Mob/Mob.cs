using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topaz.Mob
{
    class Mob
    {
        private Vector2 _position;

        public Texture2D Sprite { get; set; }
        public Vector2 Position { get => _position; set => _position = value; }
        public int Speed { get; set; }
        public int Direction { get; set; }
        public float AnimationFrame { get; set; }

        public Mob()
        {
            this.Position = new Vector2(5 * Scene.WorldScene.TILE_WIDTH * Scene.WorldScene.SCALE, 5 * Scene.WorldScene.TILE_WIDTH * Scene.WorldScene.SCALE);
            this.Speed = 200;
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

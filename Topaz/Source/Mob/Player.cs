using Microsoft.Xna.Framework;

namespace Topaz.Mob
{
    class Player : Mob
    {
        public Player() : base()
        {
            SpriteBounds = new Rectangle(0, 0, 32, 32);
            CollisionBounds = new Rectangle(3, 20, 26, 12);
        }

        public void LoadContent()
        {
            Sprite = Engine.Content.Instance.GetTexture("Temp/lucas");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;

namespace Topaz.Entity
{
    class Player : Entity
    {
        public Player() : base()
        {
            SpriteBounds = new Rectangle(0, 0, 16, 16);
            CollisionBounds = new Rectangle(1, 10, 13, 6);
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

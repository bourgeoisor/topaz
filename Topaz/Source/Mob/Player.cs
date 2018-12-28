using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Topaz.Mob
{
    class Player : Mob
    {
        public bool IsClient { get; set; }

        public Player() : base()
        {
            // @todo: only on LoadContent
            Sprite = Engine.Content.Instance.GetTexture("Temp/lucas");

            SpriteBounds = new Rectangle(0, 0, 32, 32);
            CollisionBounds = new Rectangle(3, 20, 26, 12);
        }

        public void Update(GameTime gameTime)
        {

        }

        // @todo: actually put this in mob.cs
        public void Draw(GameTime gameTime)
        {
            int step = (int)Math.Floor(AnimationFrame);
            if (step == 3) step = 1;

            Vector2 origin = new Vector2(Engine.Window.Instance.GetViewport().Width / 2, Engine.Window.Instance.GetViewport().Height / 2);
            Vector2 position = new Vector2(
                origin.X + (Position.X - Networking.Client.Instance.Player.Position.X) * (Scene.WorldScene.TILE_WIDTH * Engine.Content.DEFAULT_SCALE),
                origin.Y + (Position.Y - Networking.Client.Instance.Player.Position.Y) * (Scene.WorldScene.TILE_WIDTH * Engine.Content.DEFAULT_SCALE)
            );
            Rectangle source = new Rectangle(step * SpriteBounds.Width, Direction * SpriteBounds.Height, SpriteBounds.Width, SpriteBounds.Height);

            if (IsClient)
                position = origin;
            
            Engine.Content.Instance.SpriteBatch.Draw(
                Sprite,
                position,
                source,
                Color.White,
                0f,
                GetSpriteOrigin(),
                Engine.Content.DEFAULT_SCALE,
                SpriteEffects.None,
                0f
            );

            if (Scene.SceneManager.Instance.DisplayBoundaries)
                Engine.Content.Instance.SpriteBatch.Draw(
                    Engine.Content.Instance.AlphaRedPixel,
                    position,
                    CollisionBounds,
                    Color.White,
                    0f,
                    new Vector2(CollisionBounds.Width / 2, CollisionBounds.Height / 2),
                    Engine.Content.DEFAULT_SCALE,
                    SpriteEffects.None,
                    0f
                );
        }

        Vector2 GetSpriteOrigin()
        {
            return new Vector2(CollisionBounds.X + CollisionBounds.Width/2, CollisionBounds.Y + CollisionBounds.Height/2);
        }
    }
}

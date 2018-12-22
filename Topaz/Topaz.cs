using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Topaz
{
    public class Topaz : Game
    {
        GraphicsDeviceManager graphics;

        public Topaz()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Engine.Window.Instance.Initialize(this, graphics);
            Engine.Content.Instance.Initialize(this, graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Engine.Content.Instance.LoadContent();
            Engine.Window.Instance.LoadContent();
        }

        protected override void UnloadContent()
        {
            Engine.Window.Instance.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Engine.Window.Instance.State == Engine.Window.WindowState.Terminating)
                Exit();

            Engine.Window.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Engine.Window.Instance.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}

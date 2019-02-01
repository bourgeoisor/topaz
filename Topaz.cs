using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Topaz
{
    public class Topaz : Game
    {
        private readonly GraphicsDeviceManager _graphics;

        public Topaz()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Engine.Window.Instance.Initialize(this, _graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
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

        protected override void OnExiting(object sender, EventArgs args)
        {
            Engine.Window.Instance.Terminate();

            base.OnExiting(sender, args);
        }
    }
}

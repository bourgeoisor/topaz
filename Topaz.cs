using Microsoft.Xna.Framework;
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
            Scene.SceneManager.Instance.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Engine.Window.Instance.LoadContent();
            Scene.SceneManager.Instance.LoadContent();
        }

        protected override void UnloadContent()
        {
            Engine.Window.Instance.UnloadContent();
            Scene.SceneManager.Instance.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Engine.Window.Instance.State == Engine.Window.WindowState.Terminating)
                Exit();

            Engine.Window.Instance.Update(gameTime);
            Scene.SceneManager.Instance.Update(gameTime);
            Engine.Input.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Engine.Window.Instance.Draw(gameTime);
            Scene.SceneManager.Instance.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Engine.Window.Instance.State = Engine.Window.WindowState.Terminating;
            Engine.Window.Instance.SaveSettings();

            Networking.Client.Instance.Disconnect();
            Networking.Server.Instance.Terminate();

            base.OnExiting(sender, args);
        }
    }
}

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
            Engine.Core.Instance.Initialize(this, _graphics);
            Scene.SceneManager.Instance.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Engine.Core.Instance.LoadContent();
            Scene.SceneManager.Instance.LoadContent();
        }

        protected override void UnloadContent()
        {
            Engine.Core.Instance.UnloadContent();
            Scene.SceneManager.Instance.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Engine.Core.Instance.State == Engine.Core.EngineState.Terminating)
                Exit();

            Engine.Core.Instance.Update(gameTime);
            Scene.SceneManager.Instance.Update(gameTime);
            Engine.Input.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Engine.Core.Instance.Draw(gameTime);
            Scene.SceneManager.Instance.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Engine.Core.Instance.State = Engine.Core.EngineState.Terminating;
            Engine.Core.Instance.SaveSettings();

            Networking.Client.Instance.Disconnect();
            Networking.Server.Instance.Terminate();

            base.OnExiting(sender, args);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Scene
{
    public sealed class SceneManager
    {
        Interface.DebugInfo debugInfo;

        private WorldScene world;

        private static readonly Lazy<SceneManager> lazy =
            new Lazy<SceneManager>(() => new SceneManager());

        public static SceneManager Instance { get { return lazy.Value; } }

        internal WorldScene World { get => world; set => world = value; }

        private SceneManager()
        {
        }

        public void Initialize()
        {
            world = new WorldScene();
            debugInfo = new Interface.DebugInfo(world);

            // @todo: Move this
            // @todo: Do in separate thread to not block drawing
            // @todo: Only start server if single player or host
            Networking.Server.Instance.Initialize();
            Networking.Client.Instance.Initialize();
        }

        public void LoadContent()
        {
            world.LoadContent();
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (Engine.Input.Instance.IsKeyPressed(Keys.F1))
                debugInfo.Toggle();

            Networking.Client.Instance.HandleMessages();

            world.Update(gameTime);
            debugInfo.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            world.Draw(gameTime);
            debugInfo.Draw(gameTime);
        }
    }
}

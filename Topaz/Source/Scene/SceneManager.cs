using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Scene
{
    public sealed class SceneManager
    {
        Interface.DebugInfo debugInfo;

        private static readonly Lazy<SceneManager> lazy =
            new Lazy<SceneManager>(() => new SceneManager());

        public static SceneManager Instance { get { return lazy.Value; } }

        internal WorldScene World { get; set; }
        public bool DisplayBoundaries { get; set; }

        private SceneManager()
        {
        }

        public void Initialize()
        {
            World = new WorldScene();
            debugInfo = new Interface.DebugInfo(World);

            DisplayBoundaries = false;

            // @todo: Move this
            // @todo: Do in separate thread to not block drawing
            // @todo: Only start server if single player or host
            Networking.Server.Instance.Initialize();
            Networking.Client.Instance.Initialize();
        }

        public void LoadContent()
        {
            World.LoadContent();
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (Engine.Input.Instance.IsKeyPressed(Keys.F1))
                debugInfo.Toggle();

            if (Engine.Input.Instance.IsKeyPressed(Keys.F2))
                DisplayBoundaries = !DisplayBoundaries;

            Networking.Client.Instance.HandleMessages();

            World.Update(gameTime);
            debugInfo.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            World.Draw(gameTime);
            debugInfo.Draw(gameTime);
        }
    }
}

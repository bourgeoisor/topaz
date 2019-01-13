using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Scene
{
    public sealed class SceneManager
    {
        Interface.DebugInfo _debugInfo;
        private WorldScene _worldScene;
        private static readonly Lazy<SceneManager> lazy =
            new Lazy<SceneManager>(() => new SceneManager());

        public static SceneManager Instance { get { return lazy.Value; } }

        public bool DisplayBoundaries { get; set; }

        private SceneManager()
        {
        }

        public void Initialize()
        {
            _worldScene = new WorldScene();
            _debugInfo = new Interface.DebugInfo(_worldScene);

            DisplayBoundaries = false;

            Networking.Client.Instance.Initialize();
        }

        public void LoadContent()
        {
            _worldScene.LoadContent();
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (Engine.Input.Instance.IsKeyPressed(Keys.F1))
                _debugInfo.ToggleDisplay();

            if (Engine.Input.Instance.IsKeyPressed(Keys.F2))
                DisplayBoundaries = !DisplayBoundaries;

            if (Engine.Input.Instance.IsKeyPressed(Keys.F8))
                Networking.Server.Instance.ForwardPort();

            if (Engine.Input.Instance.IsKeyPressed(Keys.F9))
                Networking.Server.Instance.Initialize();

            if (Engine.Input.Instance.IsKeyPressed(Keys.F10))
            {
                Networking.Client.Instance.Connect("127.0.0.1", 12345);
                Engine.Content.Instance.PlaySong("Temp/song");
            }   

            if (Engine.Input.Instance.IsKeyPressed(Keys.F11))
            {
                Networking.Client.Instance.Connect("174.112.39.222", 12345);
                Engine.Content.Instance.PlaySong("Temp/song");
            }   

            if (Engine.Input.Instance.IsKeyPressed(Keys.F12))
            {
                Networking.Client.Instance.Disconnect();
                Networking.Server.Instance.Terminate();
            }

            Networking.Client.Instance.HandleMessages();

            _worldScene.Update(gameTime);
            _debugInfo.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            _worldScene.Draw(gameTime);
            _debugInfo.Draw(gameTime);
        }
    }
}

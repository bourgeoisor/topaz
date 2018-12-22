using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Scene
{
    public sealed class SceneManager
    {
        Interface.DebugInfo debugInfo;

        //Map map;

        private static readonly Lazy<SceneManager> lazy =
            new Lazy<SceneManager>(() => new SceneManager());

        public static SceneManager Instance { get { return lazy.Value; } }

        private SceneManager()
        {
        }

        public void Initialize()
        {
            //map = new Map();
            debugInfo = new Interface.DebugInfo();
        }

        public void LoadContent()
        {
            //map.LoadContent();
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (Engine.Input.Instance.IsKeyPressed(Keys.F1))
                debugInfo.Toggle();

            //map.Update(gameTime);
            debugInfo.Update(gameTime);

            Engine.Input.Instance.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            //map.Draw(gameTime);
            debugInfo.Draw(gameTime);
        }
    }
}

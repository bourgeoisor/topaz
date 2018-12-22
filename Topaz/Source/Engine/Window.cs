using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Engine
{
    public sealed class Window
    {
        const int DEFAULT_WINDOW_WIDTH = 1280;
        const int DEFAULT_WINDOW_HEIGHT = 800;

        public GraphicsDeviceManager Graphics { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public WindowState State { get; set; }

        public enum WindowState { Running, Terminating }

        private static readonly Lazy<Window> lazy =
            new Lazy<Window>(() => new Window());

        public static Window Instance { get { return lazy.Value; } }

        private Window()
        {
            State = WindowState.Running;
        }

        public void Initialize(Game game, GraphicsDeviceManager graphics)
        {
            this.Graphics = graphics;
            this.GraphicsDevice = game.GraphicsDevice;

            string title = Properties.Resources.Title;
            if (Properties.Resources.DevMode == "true")
            {
                title += " " + Properties.Resources.Version + "-" + Properties.Resources.GitCount + "-" + Properties.Resources.GitHash;
            }

            game.Window.Title = title;
            game.Window.AllowUserResizing = true;
            game.IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT;
            graphics.ApplyChanges();

            Scene.SceneManager.Instance.Initialize();
        }

        public void LoadContent()
        {
            Scene.SceneManager.Instance.LoadContent();
        }

        public void UnloadContent()
        {
            Scene.SceneManager.Instance.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                State = WindowState.Terminating;

            if (Engine.Input.Instance.IsKeyDown(Keys.LeftControl) && Engine.Input.Instance.IsKeyDown(Keys.Enter))
            {
                if (Graphics.IsFullScreen)
                {
                    Graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH;
                    Graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT;
                }
                else
                {
                    Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }

                Graphics.IsFullScreen = !Graphics.IsFullScreen;
                Graphics.ApplyChanges();
            }

            Scene.SceneManager.Instance.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            Scene.SceneManager.Instance.Draw(gameTime);
        }

        public Viewport GetViewport()
        {
            return GraphicsDevice.Viewport;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Engine
{
    public sealed class Window
    {
        private const string SETTINGS_FILE_PATH = "settings.txt";

        public WindowState State { get; set; }
        public Settings Settings { get; set; }
        public Game Game { get; set; }
        public GraphicsDeviceManager Graphics { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public enum WindowState { Running, Terminating }

        private static readonly Lazy<Window> lazy =
            new Lazy<Window>(() => new Window());

        public static Window Instance { get { return lazy.Value; } }

        private Window()
        {
            State = WindowState.Running;

            Settings = Engine.Util.XmlSerialization.ReadFromXmlFile<Engine.Settings>(SETTINGS_FILE_PATH);
            if (Settings == null)
                Settings = new Engine.Settings();

            Engine.Util.XmlSerialization.WriteToXmlFile<Engine.Settings>(SETTINGS_FILE_PATH, Settings);
        }

        public void Initialize(Game game, GraphicsDeviceManager graphics)
        {
            Game = game;
            Graphics = graphics;
            GraphicsDevice = game.GraphicsDevice;

            string title = Properties.Resources.Title;
            if (Properties.Resources.DevMode == "true")
                title += " " + Properties.Resources.Version + "-" + Properties.Resources.GitCount + "-" + Properties.Resources.GitHash;

            Game.Window.Title = title;
            Game.Window.AllowUserResizing = true;
            Game.IsMouseVisible = true;
            Game.IsFixedTimeStep = Settings.Video.Vsync;
            Graphics.SynchronizeWithVerticalRetrace = Settings.Video.Vsync;

            ToggleFullscreen(Settings.Video.Fullscreen);

            Engine.Content.Instance.Initialize(game, graphics);
        }

        public void LoadContent()
        {
            Engine.Content.Instance.LoadContent();
        }

        public void UnloadContent()
        {
            Engine.Content.Instance.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (Engine.Input.Instance.IsKeyDown(Keys.Escape))
                State = WindowState.Terminating;

            if (Engine.Input.Instance.IsKeyDown(Keys.LeftControl) && Engine.Input.Instance.IsKeyDown(Keys.Enter))
                ToggleFullscreen(!Graphics.IsFullScreen);
        }

        public void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }

        public void SaveSettings()
        {
            Settings.Video.Fullscreen = Graphics.IsFullScreen;
            if (!Graphics.IsFullScreen)
            {
                Settings.Video.WindowedScreenWidth = GetViewport().Width;
                Settings.Video.WindowedScreenHeight = GetViewport().Height;
            }
            Engine.Util.XmlSerialization.WriteToXmlFile<Engine.Settings>(SETTINGS_FILE_PATH, Settings);
        }
        
        public Viewport GetViewport()
        {
            return GraphicsDevice.Viewport;
        }

        public void ToggleFullscreen(bool toggle)
        {
            if (toggle)
            {
                Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                Graphics.PreferredBackBufferWidth = Settings.Video.WindowedScreenWidth;
                Graphics.PreferredBackBufferHeight = Settings.Video.WindowedScreenHeight;
            }

            Graphics.IsFullScreen = toggle;
            Graphics.ApplyChanges();
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Engine
{
    public sealed class Window
    {
        const string SETTINGS_FILE_PATH = "settings.txt";

        private WindowState _state;
        private Settings _settings;
        private Game _game;
        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _graphicsDevice;

        public WindowState State { get => _state; set => _state = value; }
        public Settings Settings { get => _settings; set => _settings = value; }
        public Game Game { get => _game; set => _game = value; }
        public GraphicsDeviceManager Graphics { get => _graphics; private set => _graphics = value; }
        public GraphicsDevice GraphicsDevice { get => _graphicsDevice; private set => _graphicsDevice = value; }

        public enum WindowState { Running, Terminating }

        private static readonly Lazy<Window> lazy =
            new Lazy<Window>(() => new Window());

        public static Window Instance { get { return lazy.Value; } }

        private Window()
        {
            _state = WindowState.Running;

            _settings = Engine.Util.XmlSerialization.ReadFromXmlFile<Engine.Settings>(SETTINGS_FILE_PATH);
            if (_settings == null)
                _settings = new Engine.Settings();

            Engine.Util.XmlSerialization.WriteToXmlFile<Engine.Settings>(SETTINGS_FILE_PATH, _settings);
        }

        public void Initialize(Game game, GraphicsDeviceManager graphics)
        {
            _game = game;
            _graphics = graphics;
            _graphicsDevice = game.GraphicsDevice;

            string title = Properties.Resources.Title;
            if (Properties.Resources.DevMode == "true")
                title += " " + Properties.Resources.Version + "-" + Properties.Resources.GitCount + "-" + Properties.Resources.GitHash;

            _game.Window.Title = title;
            _game.Window.AllowUserResizing = true;
            _game.IsMouseVisible = true;
            _game.IsFixedTimeStep = _settings.Video.Vsync;
            _graphics.SynchronizeWithVerticalRetrace = _settings.Video.Vsync;

            ToggleFullscreen(_settings.Video.Fullscreen);

            Engine.Content.Instance.Initialize(game, graphics);
            Scene.SceneManager.Instance.Initialize();
        }

        public void LoadContent()
        {
            Engine.Content.Instance.LoadContent();
            Scene.SceneManager.Instance.LoadContent();
        }

        public void UnloadContent()
        {
            Engine.Content.Instance.UnloadContent();
            Scene.SceneManager.Instance.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Terminate();

            if (Engine.Input.Instance.IsKeyDown(Keys.LeftControl) && Engine.Input.Instance.IsKeyDown(Keys.Enter))
                ToggleFullscreen(!_graphics.IsFullScreen);

            Scene.SceneManager.Instance.Update(gameTime);
            Input.Instance.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            Scene.SceneManager.Instance.Draw(gameTime);
        }

        public void Terminate()
        {
            _state = WindowState.Terminating;

            Networking.Client.Instance.Disconnect();
            Networking.Server.Instance.Terminate();

            SaveSettings();
        }

        public void SaveSettings()
        {
            _settings.Video.Fullscreen = _graphics.IsFullScreen;
            if (!_graphics.IsFullScreen)
            {
                _settings.Video.WindowedScreenWidth = GetViewport().Width;
                _settings.Video.WindowedScreenHeight = GetViewport().Height;
            }
            Engine.Util.XmlSerialization.WriteToXmlFile<Engine.Settings>(SETTINGS_FILE_PATH, _settings);
        }
        
        public Viewport GetViewport()
        {
            return _graphicsDevice.Viewport;
        }

        public void ToggleFullscreen(bool toggle)
        {
            if (toggle)
            {
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = _settings.Video.WindowedScreenWidth;
                _graphics.PreferredBackBufferHeight = _settings.Video.WindowedScreenHeight;
            }

            _graphics.IsFullScreen = toggle;
            _graphics.ApplyChanges();
        }
    }
}

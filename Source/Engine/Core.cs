using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Engine
{
    public sealed class Core : Game
    {
        private Logger _logger = new Logger("Engine");

        public readonly string BASE_STORAGE_PATH;
        private const string SETTINGS_FILE_PATH = "settings.xml";

        public EngineState State { get; set; }
        public enum EngineState { Running, Terminating }

        public Settings Settings { get; set; }
        public GraphicsDeviceManager Graphics { get; private set; }
        public Time Time { get; private set; }
        public Input Input { get; private set; }

        private static readonly Lazy<Core> lazy =
            new Lazy<Core>(() => new Core());

        public static Core Instance { get { return lazy.Value; } }

        private Core()
        {
            _logger.Info("Starting...");
            _logger.Info("Operating System: " + System.Environment.OSVersion.VersionString + " (" + System.Environment.OSVersion.Platform + ")");
            State = EngineState.Running;

            BASE_STORAGE_PATH = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Properties.Resources.Company,
                Properties.Resources.Title
            );

            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _logger.Info("Loading settings...");
            Settings = Engine.Util.XmlSerialization.ReadFromXmlFile<Engine.Settings>(SETTINGS_FILE_PATH);
            if (Settings == null)
            {
                _logger.Info("Could not find settings, creating...");
                Settings = new Engine.Settings();
            }
            Util.XmlSerialization.WriteToXmlFile<Engine.Settings>(SETTINGS_FILE_PATH, Settings);
            _logger.Info("Settings loaded.");

            string title = Properties.Resources.Title;
            if (Properties.Resources.DevMode == "true")
                title += " " + Properties.Resources.Version + "-" + Properties.Resources.GitCount + "-" + Properties.Resources.GitHash;

            Window.Title = title;
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            IsFixedTimeStep = false;
            Graphics.SynchronizeWithVerticalRetrace = Settings.Video.Vsync;

            Time = new Time();
            Input = new Input();

            ToggleFullscreen(Settings.Video.Fullscreen);

            Engine.Content.Instance.Initialize(this, Graphics);
            Scene.SceneManager.Instance.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Engine.Content.Instance.LoadContent();
            Scene.SceneManager.Instance.LoadContent();
        }

        protected override void UnloadContent()
        {
            Engine.Content.Instance.UnloadContent();
            Scene.SceneManager.Instance.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (State == EngineState.Terminating)
                Exit();

            Time.Update(gameTime);

            if (Input.IsKeyDown(Keys.Escape))
                State = EngineState.Terminating;

            if (Input.IsKeyDown(Keys.LeftControl) && Input.IsKeyDown(Keys.Enter))
                ToggleFullscreen(!Graphics.IsFullScreen);

            Scene.SceneManager.Instance.Update();

            Input.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Scene.SceneManager.Instance.Draw();

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            State = EngineState.Terminating;
            SaveSettings();

            Networking.Client.Instance.Disconnect();
            Networking.Server.Instance.Terminate();

            base.OnExiting(sender, args);
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

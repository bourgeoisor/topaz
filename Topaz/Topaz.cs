using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Topaz
{
    public class Topaz : Game
    {
        const string GAME_TITLE = "Topaz";
        const string VERSION = "v.0.0.1";
        const bool DEV_MODE = true;

        const int DEFAULT_WINDOW_WIDTH = 800;
        const int DEFAULT_WINDOW_HEIGHT = 600;

        GraphicsDeviceManager graphics;

        //Map map;
        Interface.DebugInfo debugInfo;

        public Topaz()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.Title = GAME_TITLE + " " + VERSION + "-" + Properties.Resources.GitCount + "-" + Properties.Resources.GitHash;
            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT;
            graphics.ApplyChanges();

            //map = new Map();
            debugInfo = new Interface.DebugInfo();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Engine.Content.Instance.Graphics = graphics;
            Engine.Content.Instance.GraphicsDevice = GraphicsDevice;
            Engine.Content.Instance.SpriteBatch = new SpriteBatch(GraphicsDevice);
            Engine.Content.Instance.ContentManager = this.Content;
            Engine.Content.Instance.LoadContent();

            //map.LoadContent();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (graphics.IsFullScreen)
                {
                    graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH;
                    graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT;
                }
                else
                {
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }

                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            if (Engine.Input.Instance.IsKeyPressed(Keys.F1))
                debugInfo.Toggle();

            //GameState.Instance.Viewport = GraphicsDevice.Viewport;

            //map.Update(gameTime);
            debugInfo.Update(gameTime);

            Engine.Input.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //map.Draw(gameTime);
            debugInfo.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Topaz.Engine
{
    public sealed class Content
    {
        public ContentManager ContentManager { get; private set; }
        public GraphicsDeviceManager Graphics { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public SpriteFont Font { get; private set; }
        public Texture2D BlackPixel { get; private set; }

        Dictionary<string, Texture2D> loadedTextures;

        private static readonly Lazy<Content> lazy =
            new Lazy<Content>(() => new Content());

        public static Content Instance { get { return lazy.Value; } }

        private Content()
        {
            loadedTextures = new Dictionary<string, Texture2D>();
        }

        public void Initialize(Game game, GraphicsDeviceManager graphics)
        {
            this.Graphics = graphics;
            this.GraphicsDevice = game.GraphicsDevice;
            this.SpriteBatch = new SpriteBatch(game.GraphicsDevice);
            this.ContentManager = game.Content;
        }

        public void LoadContent()
        {
            Font = ContentManager.Load<SpriteFont>("Font/VeraMono");

            BlackPixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            BlackPixel.SetData<Color>(new Color[] { Color.Black });
        }

        public Texture2D GetTexture(string path)
        {
            if (!loadedTextures.ContainsKey(path))
            {
                loadedTextures.Add(path, ContentManager.Load<Texture2D>(path));
            }

            return loadedTextures[path];
        }
    }
}

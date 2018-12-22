using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Topaz.Engine
{
    public sealed class Content
    {
        ContentManager contentManager;
        GraphicsDeviceManager graphics;
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Texture2D blackPixel;

        Dictionary<string, Texture2D> loadedTextures;

        public ContentManager ContentManager { get => contentManager; set => contentManager = value; }
        public GraphicsDeviceManager Graphics { get => graphics; set => graphics = value; }
        public GraphicsDevice GraphicsDevice { get => graphicsDevice; set => graphicsDevice = value; }
        public SpriteBatch SpriteBatch { get => spriteBatch; set => spriteBatch = value; }

        public SpriteFont Font { get => font; }
        public Texture2D BlackPixel { get => blackPixel; }

        private static readonly Lazy<Content> lazy =
            new Lazy<Content>(() => new Content());

        public static Content Instance { get { return lazy.Value; } }

        private Content()
        {
            loadedTextures = new Dictionary<string, Texture2D>();
        }

        public void LoadContent()
        {
            font = contentManager.Load<SpriteFont>("Font/VeraMono");

            blackPixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blackPixel.SetData<Color>(new Color[] { Color.Black });
        }

        public Texture2D GetTexture(string path)
        {
            if (!loadedTextures.ContainsKey(path))
            {
                loadedTextures.Add(path, contentManager.Load<Texture2D>(path));
            }

            return loadedTextures[path];
        }
    }
}

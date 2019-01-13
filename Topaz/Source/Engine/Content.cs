using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Topaz.Engine
{
    public sealed class Content
    {
        public const float DEFAULT_SCALE = 2f;

        public ContentManager ContentManager { get; private set; }
        public GraphicsDeviceManager Graphics { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public SpriteFont Font { get; private set; }
        public Texture2D BlackPixel { get; private set; }
        public Texture2D AlphaRedPixel { get; private set; }

        Dictionary<string, Texture2D> _loadedTextures;
        Dictionary<string, SoundEffect> _loadedSounds;
        Dictionary<string, Song> _loadedSongs;

        private static readonly Lazy<Content> lazy =
            new Lazy<Content>(() => new Content());

        public static Content Instance { get { return lazy.Value; } }

        private Content()
        {
            _loadedTextures = new Dictionary<string, Texture2D>();
            _loadedSounds = new Dictionary<string, SoundEffect>();
            _loadedSongs = new Dictionary<string, Song>();
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
            Font = ContentManager.Load<SpriteFont>("Font/VeraMono14");

            BlackPixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            BlackPixel.SetData<Color>(new Color[] { Color.Black });

            AlphaRedPixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            AlphaRedPixel.SetData<Color>(new Color[] { new Color(Color.Red, 0.2f) });
        }

        public void UnloadContent()
        {
            SpriteBatch.Dispose();
            BlackPixel.Dispose();
            AlphaRedPixel.Dispose();
        }

        public Texture2D GetTexture(string path)
        {
            if (!_loadedTextures.ContainsKey(path))
            {
                _loadedTextures.Add(path, ContentManager.Load<Texture2D>(path));
            }

            return _loadedTextures[path];
        }

        public void DrawStringOutline(SpriteFont spriteFont, string text, Vector2 position, Color colorBg, Color colorFg, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            text = Regex.Replace(text, @"[^(\u000a)(\u0020-\u007F)]+", string.Empty);

            Vector2 positionTL = new Vector2(position.X - 1, position.Y - 1);
            Vector2 positionBL = new Vector2(position.X - 1, position.Y + 1);
            Vector2 positionTR = new Vector2(position.X + 1, position.Y - 1);
            Vector2 positionBR = new Vector2(position.X + 1, position.Y + 1);
            
            SpriteBatch.DrawString(spriteFont, text, positionTL, colorBg, 0, origin, scale, effects, layerDepth);
            SpriteBatch.DrawString(spriteFont, text, positionBL, colorBg, 0, origin, scale, effects, layerDepth);
            SpriteBatch.DrawString(spriteFont, text, positionTR, colorBg, 0, origin, scale, effects, layerDepth);
            SpriteBatch.DrawString(spriteFont, text, positionBR, colorBg, 0, origin, scale, effects, layerDepth);
            SpriteBatch.DrawString(spriteFont, text, position, colorFg, 0, origin, scale, effects, layerDepth);
        }

        public void PlaySong(string path)
        {
            if (!_loadedSongs.ContainsKey(path))
            {
                _loadedSongs.Add(path, ContentManager.Load<Song>(path));
            }

            MediaPlayer.Volume = 0.2f;
            MediaPlayer.Play(_loadedSongs[path]);
        }

        public void PlaySound(string path)
        {
            if (!_loadedSounds.ContainsKey(path))
            {
                _loadedSounds.Add(path, ContentManager.Load<SoundEffect>(path));
            }

            SoundEffectInstance sound = _loadedSounds[path].CreateInstance();
            sound.Volume = 0.3f;
            //SoundEffect.MasterVolume
            sound.Play();
        }
    }
}

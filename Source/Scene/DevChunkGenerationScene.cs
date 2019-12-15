using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Scene
{
    class DevChunkGenerationScene : IScene
    {
        public const int TILE_WIDTH = 4;

        public Engine.Core Core = Engine.Core.Instance;

        float[][] mapgen;
        private Texture2D _tileset;

        public int Octave = 8;
        public float Persistence = 0.7f;
        public int Seed;

        private Random random = new Random();

        private Interface.DevChunkGenerate _devChunkGenerate;

        public DevChunkGenerationScene()
        {

        }
           
        public void LoadContent()
        {
            _tileset = Engine.Content.Instance.GetTexture("Temp/tileset2");

            Seed = random.Next(0, 10000000);
            RegenerateMap();

            _devChunkGenerate = new Interface.DevChunkGenerate(this);
            _devChunkGenerate.Visible = true;
        }

        public void Update()
        {
            if (Core.Input.IsKeyPressed(Keys.P))
            {
                Seed = random.Next(0, 10000000);
                RegenerateMap();
            }
            if (Core.Input.IsKeyPressed(Keys.Q))
            {
                Octave--;
                RegenerateMap();
            }
            if (Core.Input.IsKeyPressed(Keys.W))
            {
                Octave++;
                RegenerateMap();
            }
            if (Core.Input.IsKeyPressed(Keys.A))
            {
                Persistence -= 0.1f;
                RegenerateMap();
            }
            if (Core.Input.IsKeyPressed(Keys.S))
            {
                Persistence += 0.1f;
                RegenerateMap();
            }

            _devChunkGenerate.Update();
        }

        public void Draw()
        {
            SpriteBatch spriteBatch = Engine.Content.Instance.SpriteBatch;
            spriteBatch.Begin();

            for (int j = 0; j < 200; j++)
            {
                for (int i = 0; i < 350; i++)
                {
                    int tile = (int) (mapgen[i][j] * 10);
                    DrawTile(tile, new Vector2(i * TILE_WIDTH, j * TILE_WIDTH));
                }
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);
            _devChunkGenerate.Draw();
            spriteBatch.End();
        }

        public void DrawTile(int tile, Vector2 position)
        {
            int row = tile / 16;
            int col = tile % 16;

            Vector2 origin = new Vector2(Engine.Core.Instance.GetViewport().Width / 2, Engine.Core.Instance.GetViewport().Height / 2);
            Rectangle source = new Rectangle(col * TILE_WIDTH, row * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH);

            Engine.Content.Instance.SpriteBatch.Draw(
                _tileset,
                position,
                source,
                Color.White,
                0f,
                new Vector2(0, 0),
                1f,
                SpriteEffects.None,
                0f
            );
        }

        public void RegenerateMap()
        {
            mapgen = Engine.Util.PerlinNoise.GeneratePerlinNoise(350, 200, Seed, Octave, Persistence);
        }
    }
}

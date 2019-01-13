using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Topaz.Scene
{
    class WorldScene
    {
        public const int TILE_WIDTH = 32;

        Texture2D _tileset;
        Networking.Client _client;

        int _selectedItem;

        public WorldScene()
        {
            _client = Networking.Client.Instance;
        }

        public void LoadContent()
        {
            _tileset = Engine.Content.Instance.GetTexture("Temp/winter");
        }

        public void Update(GameTime gameTime)
        {
            if (_client.Map.Layer1 == null)
                return;

            var mstate = Mouse.GetState();
            _selectedItem = (_selectedItem + Engine.Input.Instance.GetScrollWheelDelta() + 3) % 3;

            var kstate = Keyboard.GetState();

            Mob.Mob.Direction oldDirection = _client.Player.GetDirection();

            if (kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.A))
                _client.Player.SetDirection(Mob.Mob.Direction.NorthWest);
            else if (kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.D))
                _client.Player.SetDirection(Mob.Mob.Direction.NorthEast);
            else if (kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.A))
                _client.Player.SetDirection(Mob.Mob.Direction.SouthWest);
            else if (kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.D))
                _client.Player.SetDirection(Mob.Mob.Direction.SouthEast);
            else if (kstate.IsKeyDown(Keys.W))
                _client.Player.SetDirection(Mob.Mob.Direction.North);
            else if (kstate.IsKeyDown(Keys.A))
                _client.Player.SetDirection(Mob.Mob.Direction.West);
            else if (kstate.IsKeyDown(Keys.S))
                _client.Player.SetDirection(Mob.Mob.Direction.South);
            else if (kstate.IsKeyDown(Keys.D))
                _client.Player.SetDirection(Mob.Mob.Direction.East);
            else
                _client.Player.SetDirection(Mob.Mob.Direction.None);

            // Send direction update
            if (_client.Player.GetDirection() != oldDirection)
                _client.SendPlayerMove();

            // Send map changes
            if (Engine.Input.Instance.LeftButtonDown() || Engine.Input.Instance.RightButtonDown())
            {
                int tileX = (int)Math.Floor(GetMouseTileCoordinates().X);
                int tileY = (int)Math.Floor(GetMouseTileCoordinates().Y);

                int i = (int)Math.Floor(GetMouseTileCoordinates().X);
                int j = (int)Math.Floor(GetMouseTileCoordinates().Y);

                int tileId = -1;
                if (Engine.Input.Instance.LeftButtonDown())
                {
                    tileId = _selectedItem + 224;
                }

                if (tileX > 0 && tileX < _client.Map.Layer2.GetLength(1) - 1 && tileY > 0 && tileY < _client.Map.Layer2.GetLength(0) - 1)
                {
                    if (_client.Map.Layer2[j, i] != tileId)
                    {
                        _client.Map.Layer2[j, i] = tileId;
                        Networking.Client.Instance.SendMapChange(j, i);
                        Engine.Content.Instance.PlaySound("Temp/wat");
                    }
                }
            }

            // Update each players
            foreach (Mob.Player player in _client.Players.Values)
                player.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (_client.Map.Layer1 == null)
                return;

            Vector2 origin = new Vector2(Engine.Window.Instance.GetViewport().Width / 2, Engine.Window.Instance.GetViewport().Height / 2);
            Engine.Content.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);

            // Layer 1
            for (int j = 0; j < _client.Map.Layer1.GetLength(0); j++)
            {
                for (int i = 0; i < _client.Map.Layer1.GetLength(1); i++)
                {
                    Vector2 position = new Vector2(origin.X + (i - _client.Player.GetCoordinates().X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + (j - _client.Player.GetCoordinates().Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
                    DrawTile(_client.Map.Layer1[j, i], position);
                }
            }

            // Layer 2
            for (int j = 0; j < _client.Map.Layer2.GetLength(0); j++)
            {
                for (int i = 0; i < _client.Map.Layer2.GetLength(1); i++)
                {
                    if (_client.Map.Layer2[j, i] != -1)
                    {
                        Vector2 position = new Vector2(origin.X + (i - _client.Player.GetCoordinates().X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + (j - _client.Player.GetCoordinates().Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
                        DrawTile(_client.Map.Layer2[j, i], position);

                        if (SceneManager.Instance.DisplayBoundaries)
                            Engine.Content.Instance.SpriteBatch.Draw(
                                Engine.Content.Instance.AlphaRedPixel,
                                position,
                                new Rectangle(0, 0, TILE_WIDTH, TILE_WIDTH),
                                Color.White,
                                0f,
                                new Vector2(0, 0),
                                Engine.Content.DEFAULT_SCALE,
                                SpriteEffects.None,
                                0f
                            );
                    }
                }
            }

            // Draw selector
            {
                Vector2 position = new Vector2(origin.X + ((int)Math.Floor(GetMouseTileCoordinates().X) - _client.Player.GetCoordinates().X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + ((int)Math.Floor(GetMouseTileCoordinates().Y) - _client.Player.GetCoordinates().Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
                DrawTile(10, position);
            }

            // Draw players
            List<Mob.Player> players = new List<Mob.Player>(_client.Players.Values);
            players.Sort((a, b) => a.GetCoordinates().Y.CompareTo(b.GetCoordinates().Y));
            foreach (Mob.Player player in players)
                player.Draw(gameTime);

            // Draw item selection
            DrawTile(_selectedItem + 224, new Vector2(10, Engine.Window.Instance.GetViewport().Height - 74));

            Engine.Content.Instance.SpriteBatch.End();
        }

        public void DrawTile(int tile, Vector2 position)
        {
            int row = tile / 16;
            int col = tile % 16;

            Vector2 origin = new Vector2(Engine.Window.Instance.GetViewport().Width / 2, Engine.Window.Instance.GetViewport().Height / 2);
            Rectangle source = new Rectangle(col * TILE_WIDTH, row * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH);

            Engine.Content.Instance.SpriteBatch.Draw(
                _tileset,
                position,
                source,
                Color.White,
                0f,
                new Vector2(0, 0),
                Engine.Content.DEFAULT_SCALE,
                SpriteEffects.None,
                0f
            );
        }

        public Vector2 GetMouseTileCoordinates()
        {
            float dx = Mouse.GetState().X - (Engine.Window.Instance.GetViewport().Width / 2);
            float dy = Mouse.GetState().Y - (Engine.Window.Instance.GetViewport().Height / 2);

            return new Vector2(_client.Player.GetCoordinates().X + (dx / TILE_WIDTH / Engine.Content.DEFAULT_SCALE), _client.Player.GetCoordinates().Y + (dy / TILE_WIDTH / Engine.Content.DEFAULT_SCALE));
        }
    }
}

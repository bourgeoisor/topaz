using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Topaz.Scene
{
    class WorldScene : Scene
    {
        public const int TILE_WIDTH = 16;

        private Texture2D _tileset;
        private Networking.Client _client;

        private Interface.OptionsPanel _optionsPanel;

        private int _selectedItem;

        public WorldScene()
        {
            _client = Networking.Client.Instance;
        }

        public void LoadContent()
        {
            _tileset = Engine.Content.Instance.GetTexture("Temp/tileset");

            _optionsPanel = new Interface.OptionsPanel();
        }

        public void Update()
        {
            if (_client.Map.Layer1 == null)
                return;

            if (Core.Input.IsKeyPressed(Keys.O))
                _optionsPanel.ToggleDisplay();

            if (_optionsPanel.IsDisplaying)
            {
                _optionsPanel.Update();
                if (_optionsPanel.MouseIsIntersecting()) return;
            }

            var mstate = Mouse.GetState();
            _selectedItem = (_selectedItem + Core.Input.GetScrollWheelDelta() + 3) % 3;

            var kstate = Keyboard.GetState();

            Entity.Entity.Direction oldDirection = _client.Player.MovementDirection;

            if (kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.A))
                _client.Player.SetDirection(Entity.Entity.Direction.NorthWest);
            else if (kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.D))
                _client.Player.SetDirection(Entity.Entity.Direction.NorthEast);
            else if (kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.A))
                _client.Player.SetDirection(Entity.Entity.Direction.SouthWest);
            else if (kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.D))
                _client.Player.SetDirection(Entity.Entity.Direction.SouthEast);
            else if (kstate.IsKeyDown(Keys.W))
                _client.Player.SetDirection(Entity.Entity.Direction.North);
            else if (kstate.IsKeyDown(Keys.A))
                _client.Player.SetDirection(Entity.Entity.Direction.West);
            else if (kstate.IsKeyDown(Keys.S))
                _client.Player.SetDirection(Entity.Entity.Direction.South);
            else if (kstate.IsKeyDown(Keys.D))
                _client.Player.SetDirection(Entity.Entity.Direction.East);
            else
                _client.Player.SetDirection(Entity.Entity.Direction.None);

            // Send direction update
            if (_client.Player.MovementDirection != oldDirection)
                _client.SendPlayerMove();

            // Send map changes
            if (Core.Input.LeftButtonDown() || Core.Input.RightButtonDown())
            {
                int tileX = (int)Math.Floor(GetMouseTileCoordinates().X);
                int tileY = (int)Math.Floor(GetMouseTileCoordinates().Y);

                int i = (int)Math.Floor(GetMouseTileCoordinates().X);
                int j = (int)Math.Floor(GetMouseTileCoordinates().Y);

                int tileId = -1;
                if (Core.Input.LeftButtonDown())
                {
                    tileId = _selectedItem + 1;
                }

                if (tileX > 0 && tileX < _client.Map.Layer2.GetLength(1) - 1 && tileY > 0 && tileY < _client.Map.Layer2.GetLength(0) - 1)
                {
                    if (_client.Map.Layer2[j, i] != tileId)
                    {
                        _client.Map.Layer2[j, i] = tileId;
                        Networking.Client.Instance.SendMapChange(j, i);
                        Engine.Content.Instance.PlaySound("Temp/hit");
                    }
                }
            }

            // Update each players
            foreach (Entity.Player player in _client.Players.Values)
                player.Update();
        }

        public void Draw()
        {
            if (_client.Map.Layer1 == null)
                return;

            Vector2 origin = new Vector2(Engine.Core.Instance.GetViewport().Width / 2, Engine.Core.Instance.GetViewport().Height / 2);
            Engine.Content.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);

            // Layer 1
            for (int j = 0; j < _client.Map.Layer1.GetLength(0); j++)
            {
                for (int i = 0; i < _client.Map.Layer1.GetLength(1); i++)
                {
                    Vector2 position = new Vector2(origin.X + (i - _client.Player.Coordinates.X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + (j - _client.Player.Coordinates.Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
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
                        Vector2 position = new Vector2(origin.X + (i - _client.Player.Coordinates.X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + (j - _client.Player.Coordinates.Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
                        DrawTile(_client.Map.Layer2[j, i], position);

                        if (SceneManager.Instance.IsDisplayedDebug)
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
                Vector2 position = new Vector2(origin.X + ((int)Math.Floor(GetMouseTileCoordinates().X) - _client.Player.Coordinates.X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + ((int)Math.Floor(GetMouseTileCoordinates().Y) - _client.Player.Coordinates.Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
                DrawTile(4, position);
            }

            // Draw players
            List<Entity.Player> players = new List<Entity.Player>(_client.Players.Values);
            players.Sort((a, b) => a.Coordinates.Y.CompareTo(b.Coordinates.Y));
            foreach (Entity.Player player in players)
                player.Draw();

            // Draw item selection
            DrawTile(_selectedItem + 1, new Vector2(10, Engine.Core.Instance.GetViewport().Height - 74));

            Engine.Content.Instance.SpriteBatch.End();

            // Draw panels
            _optionsPanel.Draw();
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
                Engine.Content.DEFAULT_SCALE,
                SpriteEffects.None,
                0f
            );
        }

        public Vector2 GetMouseTileCoordinates()
        {
            float dx = Mouse.GetState().X - (Engine.Core.Instance.GetViewport().Width / 2);
            float dy = Mouse.GetState().Y - (Engine.Core.Instance.GetViewport().Height / 2);

            return new Vector2(_client.Player.Coordinates.X + (dx / TILE_WIDTH / Engine.Content.DEFAULT_SCALE), _client.Player.Coordinates.Y + (dy / TILE_WIDTH / Engine.Content.DEFAULT_SCALE));
        }
    }
}

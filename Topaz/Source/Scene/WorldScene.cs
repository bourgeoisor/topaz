using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Scene
{
    class WorldScene
    {
        public const int TILE_WIDTH = 32;

        Texture2D tileset;
        Networking.Client client;

        public WorldScene()
        {
            client = Networking.Client.Instance;
        }

        public void LoadContent()
        {
            tileset = Engine.Content.Instance.GetTexture("Temp/winter");
        }

        public void Update(GameTime gameTime)
        {
            if (client.Map.Map1 == null)
                return;

            var kstate = Keyboard.GetState();

            Mob.Mob.Direction oldDirection = client.Player.GetDirection();

            if (kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.A))
                client.Player.SetDirection(Mob.Mob.Direction.NorthWest);
            else if (kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.D))
                client.Player.SetDirection(Mob.Mob.Direction.NorthEast);
            else if (kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.A))
                client.Player.SetDirection(Mob.Mob.Direction.SouthWest);
            else if (kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.D))
                client.Player.SetDirection(Mob.Mob.Direction.SouthEast);
            else if (kstate.IsKeyDown(Keys.W))
                client.Player.SetDirection(Mob.Mob.Direction.North);
            else if (kstate.IsKeyDown(Keys.A))
                client.Player.SetDirection(Mob.Mob.Direction.West);
            else if (kstate.IsKeyDown(Keys.S))
                client.Player.SetDirection(Mob.Mob.Direction.South);
            else if (kstate.IsKeyDown(Keys.D))
                client.Player.SetDirection(Mob.Mob.Direction.East);
            else
                client.Player.SetDirection(Mob.Mob.Direction.None);

            // Send direction update
            if (client.Player.GetDirection() != oldDirection)
                client.SendPlayerMove();

            // Send map changes
            if (Engine.Input.Instance.LeftButtonDown())
            {
                int tileX = (int)Math.Floor(GetMouseCoordinates().X);
                int tileY = (int)Math.Floor(GetMouseCoordinates().Y);

                if (tileX > 0 && tileX < client.Map.Map2.GetLength(1) - 1 && tileY > 0 && tileY < client.Map.Map2.GetLength(0) - 1)
                {
                    client.Map.Map2[(int)Math.Floor(GetMouseCoordinates().Y), (int)Math.Floor(GetMouseCoordinates().X)] = 16 * 14;
                    Networking.Client.Instance.SendMapChange((int)Math.Floor(GetMouseCoordinates().Y), (int)Math.Floor(GetMouseCoordinates().X));
                }
            }
            else if (Engine.Input.Instance.RightButtonDown())
            {
                int tileX = (int)Math.Floor(GetMouseCoordinates().X);
                int tileY = (int)Math.Floor(GetMouseCoordinates().Y);

                if (tileX > 0 && tileX < client.Map.Map2.GetLength(1) - 1 && tileY > 0 && tileY < client.Map.Map2.GetLength(0) - 1)
                {
                    client.Map.Map2[(int)Math.Floor(GetMouseCoordinates().Y), (int)Math.Floor(GetMouseCoordinates().X)] = -1;
                    Networking.Client.Instance.SendMapChange((int)Math.Floor(GetMouseCoordinates().Y), (int)Math.Floor(GetMouseCoordinates().X));
                }
            }

            // Update each players
            foreach (Mob.Player player in client.Players.Values)
                player.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (client.Map.Map1 == null)
                return;

            Vector2 origin = new Vector2(Engine.Window.Instance.GetViewport().Width / 2, Engine.Window.Instance.GetViewport().Height / 2);
            Engine.Content.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);

            // tiles1
            for (int j = 0; j < client.Map.Map1.GetLength(0); j++)
            {
                for (int i = 0; i < client.Map.Map1.GetLength(1); i++)
                {
                    Vector2 position = new Vector2(origin.X + (i - client.Player.GetPosition().X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + (j - client.Player.GetPosition().Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
                    DrawTile(client.Map.Map1[j, i], position);
                }
            }

            // tiles2
            for (int j = 0; j < client.Map.Map2.GetLength(0); j++)
            {
                for (int i = 0; i < client.Map.Map2.GetLength(1); i++)
                {
                    if (client.Map.Map2[j, i] != -1)
                    {
                        Vector2 position = new Vector2(origin.X + (i - client.Player.GetPosition().X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + (j - client.Player.GetPosition().Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
                        DrawTile(client.Map.Map2[j, i], position);

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
                Vector2 position = new Vector2(origin.X + ((int)Math.Floor(GetMouseCoordinates().X) - client.Player.GetPosition().X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + ((int)Math.Floor(GetMouseCoordinates().Y) - client.Player.GetPosition().Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
                DrawTile(10, position);
            }

            // Draw players
            foreach (Mob.Player player in client.Players.Values)
                player.Draw(gameTime);

            Engine.Content.Instance.SpriteBatch.End();
        }

        public void DrawTile(int tile, Vector2 position)
        {
            int row = tile / 16;
            int col = tile % 16;

            Vector2 origin = new Vector2(Engine.Window.Instance.GetViewport().Width / 2, Engine.Window.Instance.GetViewport().Height / 2);
            Rectangle source = new Rectangle(col * TILE_WIDTH, row * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH);

            Engine.Content.Instance.SpriteBatch.Draw(
                tileset,
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

        public Vector2 GetCoordinates()
        {
            return client.Player.GetPosition();
        }

        public Vector2 GetMouseCoordinates()
        {
            float dx = Mouse.GetState().X - (Engine.Window.Instance.GetViewport().Width / 2);
            float dy = Mouse.GetState().Y - (Engine.Window.Instance.GetViewport().Height / 2);

            return new Vector2(client.Player.GetPosition().X + (dx / TILE_WIDTH / Engine.Content.DEFAULT_SCALE), client.Player.GetPosition().Y + (dy / TILE_WIDTH / Engine.Content.DEFAULT_SCALE));
        }
    }
}

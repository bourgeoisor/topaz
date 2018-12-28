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

            float deltaMovementAxis = client.Player.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float deltaMovementDiag = 0.7f * deltaMovementAxis;

            float deltaX = 0;
            float deltaY = 0;

            if (kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.D))
            {
                deltaX += deltaMovementDiag;
                deltaY -= deltaMovementDiag;
            }
            else if (kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.A))
            {
                deltaX -= deltaMovementDiag;
                deltaY -= deltaMovementDiag;
            }
            else if (kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.D))
            {
                deltaX += deltaMovementDiag;
                deltaY += deltaMovementDiag;
            }
            else if (kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.A))
            {
                deltaX -= deltaMovementDiag;
                deltaY += deltaMovementDiag;
            }

            else if (kstate.IsKeyDown(Keys.W))
            {
                deltaY -= deltaMovementAxis;
                client.Player.Direction = 3;
            }

            else if (kstate.IsKeyDown(Keys.S))
            {
                deltaY += deltaMovementAxis;
                client.Player.Direction = 0;
            }

            else if (kstate.IsKeyDown(Keys.A))
            {
                deltaX -= deltaMovementAxis;
                client.Player.Direction = 1;
            }

            else if (kstate.IsKeyDown(Keys.D))
            {
                deltaX += deltaMovementAxis;
                client.Player.Direction = 2;
            }
            else
            {
                client.Player.AnimationFrame = 1;
            }

            client.Player.Move(deltaX, 0);
            if (IsCollision(client.Player))
            {
                client.Player.Move(-deltaX, 0);
            }
            client.Player.Move(0, deltaY);
            if (IsCollision(client.Player))
            {
                client.Player.Move(0, -deltaY);
            }

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

            if (deltaX != 0 || deltaY != 0)
            {
                client.Player.AnimationFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * 5;
                if (client.Player.AnimationFrame >= 4) client.Player.AnimationFrame = 0;

                client.SendPlayerMove();
            }
        }

        public bool IsCollision(Mob.Player player)
        {
            int currX = (int)Math.Floor(GetCoordinates().X);
            int currY = (int)Math.Floor(GetCoordinates().Y);

            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (client.Map.Map2[currY + j, currX + i] != -1 && AABB(currY + j, currX + i, player))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AABB(int tileJ, int tileI, Mob.Player player)
        {
            bool AisToTheRightOfB = tileI * TILE_WIDTH + 0.1 > GetCoordinates().X * TILE_WIDTH + (player.CollisionBounds.Width/2);
            bool AisToTheLeftOfB = tileI * TILE_WIDTH + TILE_WIDTH - 0.1 < GetCoordinates().X * TILE_WIDTH - (player.CollisionBounds.Width/2);
            bool AisAboveB = tileJ * TILE_WIDTH + TILE_WIDTH - 0.1 < GetCoordinates().Y * TILE_WIDTH - (player.CollisionBounds.Height/2);
            bool AisBelowB = tileJ * TILE_WIDTH + 0.1 > GetCoordinates().Y * TILE_WIDTH + (player.CollisionBounds.Height / 2);
            return !(AisToTheRightOfB
              || AisToTheLeftOfB
              || AisAboveB
              || AisBelowB);
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
                    Vector2 position = new Vector2(origin.X + (i - client.Player.Position.X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + (j - client.Player.Position.Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
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
                        Vector2 position = new Vector2(origin.X + (i - client.Player.Position.X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + (j - client.Player.Position.Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
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

            // selector
            {
                Vector2 position = new Vector2(origin.X + ((int)Math.Floor(GetMouseCoordinates().X) - client.Player.Position.X) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE), origin.Y + ((int)Math.Floor(GetMouseCoordinates().Y) - client.Player.Position.Y) * (TILE_WIDTH * Engine.Content.DEFAULT_SCALE));
                DrawTile(10, position);
            }

            // character
            client.Player.Draw(gameTime);

            // other characters
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
            return client.Player.Position;
        }

        public Vector2 GetMouseCoordinates()
        {
            float dx = Mouse.GetState().X - (Engine.Window.Instance.GetViewport().Width / 2);
            float dy = Mouse.GetState().Y - (Engine.Window.Instance.GetViewport().Height / 2);

            return new Vector2(client.Player.Position.X + (dx / TILE_WIDTH / Engine.Content.DEFAULT_SCALE), client.Player.Position.Y + (dy / TILE_WIDTH / Engine.Content.DEFAULT_SCALE));
        }
    }
}

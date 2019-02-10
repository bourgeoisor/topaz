using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Topaz.Entity
{
    class Entity
    {
        public const float OFFSET_TOLERANCE = 0.15f;

        public Engine.Logger Logger { get; set; }

        public Vector2 Coordinates { get; private set; }
        public Direction MovementDirection { get; private set; }

        public Texture2D Sprite { get; set; }

        public Rectangle SpriteBounds { get; set; }
        public Rectangle CollisionBounds { get; set; }
        public int Speed { get; set; }
        public int AnimationDirection { get; set; }
        public float AnimationFrame { get; set; }
        public bool DrawAtOrigin { get; set; }

        public Entity()
        {
            Logger = new Engine.Logger("Entity");
            MovementDirection = Direction.None;
            SetCoordinates(new Vector2(5, 5));
            Speed = 5;
            AnimationDirection = 0;
            AnimationFrame = 1;
        }

        public enum Direction
        {
            None,
            North,
            West,
            South,
            East,
            NorthWest,
            NorthEast,
            SouthWest,
            SouthEast
        }

        public virtual void Update(GameTime gameTime)
        {
            if (MovementDirection == Direction.None)
                return;

            AnimationFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * 5;
            if (AnimationFrame >= 4) AnimationFrame = 0;

            float deltaMovementAxis = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float deltaMovementDiag = 0.7f * deltaMovementAxis;

            Vector2 delta = new Vector2(0, 0);

            switch (MovementDirection)
            {
                case Direction.NorthWest:
                    delta.X -= deltaMovementDiag;
                    delta.Y -= deltaMovementDiag;
                    if (AnimationDirection != 1 && AnimationDirection != 3)
                        AnimationDirection = 3;
                    break;
                case Direction.NorthEast:
                    delta.X += deltaMovementDiag;
                    delta.Y -= deltaMovementDiag;
                    if (AnimationDirection != 2 && AnimationDirection != 3)
                        AnimationDirection = 3;
                    break;
                case Direction.SouthWest:
                    delta.X -= deltaMovementDiag;
                    delta.Y += deltaMovementDiag;
                    if (AnimationDirection != 0 && AnimationDirection != 1)
                        AnimationDirection = 0;
                    break;
                case Direction.SouthEast:
                    delta.X += deltaMovementDiag;
                    delta.Y += deltaMovementDiag;
                    if (AnimationDirection != 0 && AnimationDirection != 2)
                        AnimationDirection = 0;
                    break;
                case Direction.North:
                    delta.Y -= deltaMovementAxis;
                    AnimationDirection = 3;
                    break;
                case Direction.West:
                    delta.X -= deltaMovementAxis;
                    AnimationDirection = 1;
                    break;
                case Direction.South:
                    delta.Y += deltaMovementAxis;
                    AnimationDirection = 0;
                    break;
                case Direction.East:
                    delta.X += deltaMovementAxis;
                    AnimationDirection = 2;
                    break;
            }

            delta = GetDeltaBeforeCollision(this, delta);
        }

        public virtual void Draw(GameTime gameTime)
        {
            int step = (int)Math.Floor(AnimationFrame);
            if (step == 3) step = 1;

            Vector2 origin = new Vector2(Engine.Core.Instance.GetViewport().Width / 2, Engine.Core.Instance.GetViewport().Height / 2);
            Vector2 position = new Vector2(
                origin.X + (Coordinates.X - Networking.Client.Instance.Player.Coordinates.X) * (Scene.WorldScene.TILE_WIDTH * Engine.Content.DEFAULT_SCALE),
                origin.Y + (Coordinates.Y - Networking.Client.Instance.Player.Coordinates.Y) * (Scene.WorldScene.TILE_WIDTH * Engine.Content.DEFAULT_SCALE)
            );
            Rectangle source = new Rectangle(step * SpriteBounds.Width, AnimationDirection * SpriteBounds.Height, SpriteBounds.Width, SpriteBounds.Height);

            if (DrawAtOrigin)
                position = origin;

            Engine.Content.Instance.SpriteBatch.Draw(
                Sprite,
                position,
                source,
                Color.White,
                0f,
                GetSpriteOrigin(),
                Engine.Content.DEFAULT_SCALE,
                SpriteEffects.None,
                0f
            );

            if (Scene.SceneManager.Instance.IsDisplayedDebug)
                Engine.Content.Instance.SpriteBatch.Draw(
                    Engine.Content.Instance.AlphaRedPixel,
                    position,
                    CollisionBounds,
                    Color.White,
                    0f,
                    new Vector2(CollisionBounds.Width / 2, CollisionBounds.Height / 2),
                    Engine.Content.DEFAULT_SCALE,
                    SpriteEffects.None,
                    0f
                );
        }

        public void SetDirection(Direction direction)
        {
            if (direction != MovementDirection)
            {
                MovementDirection = direction;

                if (MovementDirection == Direction.None)
                    AnimationFrame = 1;
            }
        }

        public void SetCoordinates(Vector2 position)
        {
            float deltaX = Math.Abs(position.X - Coordinates.X);
            float deltaY = Math.Abs(position.Y - Coordinates.Y);
            
            if (deltaX > OFFSET_TOLERANCE || deltaY > OFFSET_TOLERANCE)
            {
                if (Coordinates.X != 0 || Coordinates.Y != 0)
                    Logger.Debug("Correcting entity offset of " + deltaX + "," + deltaY);
                Coordinates = position;
            }
        }

        public void Move(float deltaX, float deltaY)
        {
            Coordinates = new Vector2(Coordinates.X + deltaX, Coordinates.Y + deltaY);
        }

        Vector2 GetSpriteOrigin()
        {
            return new Vector2(CollisionBounds.X + CollisionBounds.Width / 2, CollisionBounds.Y + CollisionBounds.Height / 2);
        }

        public Vector2 GetDeltaBeforeCollision(Entity entity, Vector2 delta)
        {
            if (delta.X > 0)
            {
                while (delta.X > 0)
                {
                    entity.Move(delta.X, 0);
                    if (IsCollision(entity))
                        entity.Move(-delta.X, 0);
                    else
                        break;
                    delta.X -= 0.03f;
                    if (delta.X < 0) delta.X = 0;
                }
            }
            else
            {
                while (delta.X < 0)
                {
                    entity.Move(delta.X, 0);
                    if (IsCollision(entity))
                        entity.Move(-delta.X, 0);
                    else
                        break;
                    delta.X += 0.03f;
                    if (delta.X > 0) delta.X = 0;
                }
            }

            if (delta.Y > 0)
            {
                while (delta.Y > 0)
                {
                    entity.Move(0, delta.Y);
                    if (IsCollision(entity))
                        entity.Move(0, -delta.Y);
                    else
                        break;
                    delta.Y -= 0.03f;
                    if (delta.Y < 0) delta.Y = 0;
                }
            }
            else
            {
                while (delta.Y < 0)
                {
                    entity.Move(0, delta.Y);
                    if (IsCollision(entity))
                        entity.Move(0, -delta.Y);
                    else
                        break;
                    delta.Y += 0.03f;
                    if (delta.Y > 0) delta.Y = 0;
                }
            }

            return delta;
        }

        public bool IsCollision(Entity entity)
        {
            int currX = (int)Math.Floor(entity.Coordinates.X);
            int currY = (int)Math.Floor(entity.Coordinates.Y);

            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (Networking.Client.Instance.Map.Layer2[currY + j, currX + i] != -1 && AABB(currY + j, currX + i, entity))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AABB(int tileJ, int tileI, Entity entity)
        {
            int tileWidth = Scene.WorldScene.TILE_WIDTH;
            
            bool AisToTheRightOfB = tileI * tileWidth + 0.1 > entity.Coordinates.X * tileWidth + (entity.CollisionBounds.Width / 2);
            bool AisToTheLeftOfB = tileI * tileWidth + tileWidth - 0.1 < entity.Coordinates.X * tileWidth - (entity.CollisionBounds.Width / 2);
            bool AisAboveB = tileJ * tileWidth + tileWidth - 0.1 < entity.Coordinates.Y * tileWidth - (entity.CollisionBounds.Height / 2);
            bool AisBelowB = tileJ * tileWidth + 0.1 > entity.Coordinates.Y * tileWidth + (entity.CollisionBounds.Height / 2);
            return !(AisToTheRightOfB
              || AisToTheLeftOfB
              || AisAboveB
              || AisBelowB);
        }
    }
}

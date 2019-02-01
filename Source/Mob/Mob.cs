﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Topaz.Mob
{
    class Mob
    {
        public const float OFFSET_TOLERANCE = 0.15f;

        Engine.Logger logger = new Engine.Logger("Mob");

        Vector2 _coordinates;
        Direction _direction;

        public Texture2D Sprite { get; set; }

        public Rectangle SpriteBounds { get; set; }
        public Rectangle CollisionBounds { get; set; }
        public int Speed { get; set; }
        public int AnimationDirection { get; set; }
        public float AnimationFrame { get; set; }
        public bool DrawAtOrigin { get; set; }

        public Mob()
        {
            _direction = Direction.None;
            this.SetCoordinates(new Vector2(5, 5));
            this.Speed = 5;
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
            if (_direction == Direction.None)
                return;

            AnimationFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * 5;
            if (AnimationFrame >= 4) AnimationFrame = 0;

            float deltaMovementAxis = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float deltaMovementDiag = 0.7f * deltaMovementAxis;

            Vector2 delta = new Vector2(0, 0);

            switch (_direction)
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

            Vector2 origin = new Vector2(Engine.Window.Instance.GetViewport().Width / 2, Engine.Window.Instance.GetViewport().Height / 2);
            Vector2 position = new Vector2(
                origin.X + (_coordinates.X - Networking.Client.Instance.Player._coordinates.X) * (Scene.WorldScene.TILE_WIDTH * Engine.Content.DEFAULT_SCALE),
                origin.Y + (_coordinates.Y - Networking.Client.Instance.Player._coordinates.Y) * (Scene.WorldScene.TILE_WIDTH * Engine.Content.DEFAULT_SCALE)
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

            if (Scene.SceneManager.Instance.DisplayBoundaries)
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

        public Direction GetDirection()
        {
            return _direction;
        }

        public void SetDirection(Direction direction)
        {
            if (direction != _direction)
            {
                _direction = direction;

                if (_direction == Direction.None)
                    AnimationFrame = 1;
            }
        }

        public Vector2 GetCoordinates()
        {
            return _coordinates;
        }

        public void SetCoordinates(Vector2 position)
        {
            float deltaX = Math.Abs(position.X - _coordinates.X);
            float deltaY = Math.Abs(position.Y - _coordinates.Y);
            
            if (deltaX > OFFSET_TOLERANCE || deltaY > OFFSET_TOLERANCE)
            {
                logger.Debug("Correcting entity offset of " + deltaX + "," + deltaY);
                _coordinates = position;
            }
        }

        public void Move(float deltaX, float deltaY)
        {
            _coordinates.X += deltaX;
            _coordinates.Y += deltaY;
        }

        Vector2 GetSpriteOrigin()
        {
            return new Vector2(CollisionBounds.X + CollisionBounds.Width / 2, CollisionBounds.Y + CollisionBounds.Height / 2);
        }

        public Vector2 GetDeltaBeforeCollision(Mob mob, Vector2 delta)
        {
            if (delta.X > 0)
            {
                while (delta.X > 0)
                {
                    mob.Move(delta.X, 0);
                    if (IsCollision(mob))
                        mob.Move(-delta.X, 0);
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
                    mob.Move(delta.X, 0);
                    if (IsCollision(mob))
                        mob.Move(-delta.X, 0);
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
                    mob.Move(0, delta.Y);
                    if (IsCollision(mob))
                        mob.Move(0, -delta.Y);
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
                    mob.Move(0, delta.Y);
                    if (IsCollision(mob))
                        mob.Move(0, -delta.Y);
                    else
                        break;
                    delta.Y += 0.03f;
                    if (delta.Y > 0) delta.Y = 0;
                }
            }

            return delta;
        }

        public bool IsCollision(Mob mob)
        {
            int currX = (int)Math.Floor(mob.GetCoordinates().X);
            int currY = (int)Math.Floor(mob.GetCoordinates().Y);

            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (Networking.Client.Instance.Map.Layer2[currY + j, currX + i] != -1 && AABB(currY + j, currX + i, mob))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AABB(int tileJ, int tileI, Mob mob)
        {
            int tileWidth = Scene.WorldScene.TILE_WIDTH;
            
            bool AisToTheRightOfB = tileI * tileWidth + 0.1 > mob.GetCoordinates().X * tileWidth + (mob.CollisionBounds.Width / 2);
            bool AisToTheLeftOfB = tileI * tileWidth + tileWidth - 0.1 < mob.GetCoordinates().X * tileWidth - (mob.CollisionBounds.Width / 2);
            bool AisAboveB = tileJ * tileWidth + tileWidth - 0.1 < mob.GetCoordinates().Y * tileWidth - (mob.CollisionBounds.Height / 2);
            bool AisBelowB = tileJ * tileWidth + 0.1 > mob.GetCoordinates().Y * tileWidth + (mob.CollisionBounds.Height / 2);
            return !(AisToTheRightOfB
              || AisToTheLeftOfB
              || AisAboveB
              || AisBelowB);
        }
    }
}
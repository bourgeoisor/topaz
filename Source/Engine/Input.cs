﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Engine
{
    public sealed class Input
    {
        const int WHEEL_DELTA = 120;

        private KeyboardState _lastKeyboardState;
        private  MouseState _lastMouseState;

        private static readonly Lazy<Input> lazy =
            new Lazy<Input>(() => new Input());

        public static Input Instance { get { return lazy.Value; } }

        private Input()
        {
        }

        public void Update(GameTime gameTime)
        {
            _lastKeyboardState = Keyboard.GetState();
            _lastMouseState = Mouse.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) && !_lastKeyboardState.IsKeyDown(key);
        }

        public bool LeftButtonPressed()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released;
        }

        public bool LeftButtonDown()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed;
        }

        public bool RightButtonPressed()
        {
            return Mouse.GetState().RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released;
        }

        public bool RightButtonDown()
        {
            return Mouse.GetState().RightButton == ButtonState.Pressed;
        }

        public int GetScrollWheelDelta()
        {
            return (Mouse.GetState().ScrollWheelValue - _lastMouseState.ScrollWheelValue) / WHEEL_DELTA;
        }
    }
}

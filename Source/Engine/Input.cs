using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Engine
{
    public static class Input
    {
        private const int WHEEL_DELTA = 120;

        private static KeyboardState _lastKeyboardState;
        private static MouseState _lastMouseState;

        public static void Update(GameTime gameTime)
        {
            _lastKeyboardState = Keyboard.GetState();
            _lastMouseState = Mouse.GetState();
        }

        public static bool IsKeyDown(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) && !_lastKeyboardState.IsKeyDown(key);
        }

        public static bool LeftButtonPressed()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released;
        }

        public static bool LeftButtonDown()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed;
        }

        public static bool RightButtonPressed()
        {
            return Mouse.GetState().RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released;
        }

        public static bool RightButtonDown()
        {
            return Mouse.GetState().RightButton == ButtonState.Pressed;
        }

        public static int GetScrollWheelDelta()
        {
            return (Mouse.GetState().ScrollWheelValue - _lastMouseState.ScrollWheelValue) / WHEEL_DELTA;
        }
    }
}

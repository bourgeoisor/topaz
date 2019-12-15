using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Topaz.Engine
{
    public class Input
    {
        private const int WHEEL_DELTA = 120;

        private static KeyboardState _lastKeyboardState;
        private static MouseState _lastMouseState;

        public void Update()
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

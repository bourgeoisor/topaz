using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Engine
{
    public sealed class Input
    {
        KeyboardState lastKeyboardState;

        private static readonly Lazy<Input> lazy =
            new Lazy<Input>(() => new Input());

        public static Input Instance { get { return lazy.Value; } }

        private Input()
        {
        }

        public void Update(GameTime gameTime)
        {
            lastKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key);
        }
    }
}

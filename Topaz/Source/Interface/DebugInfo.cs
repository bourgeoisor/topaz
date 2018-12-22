using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Interface
{
    class DebugInfo
    {
        const int POSITION_X = 10;
        const int POSITION_Y = 10;
        const int PADDING = 10;

        //GameState state;
        Engine.Content graphicsContent;
        string debugInfo;
        bool show;

        public DebugInfo()
        {
            //state = GameState.Instance;
            graphicsContent = Engine.Content.Instance;
            debugInfo = "";
            show = false;
        }

        public void Update(GameTime gameTime)
        {
            //Rectangle viewport = state.Viewport.Bounds;
            MouseState mouse = Mouse.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            double fps = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);

            debugInfo = "Video\n";
            debugInfo += " FPS: " + fps + "\n";
            //debugInfo += " Width: " + viewport.Width + " Height: " + viewport.Height + "\n";
            debugInfo += "Input\n";
            debugInfo += " Mouse Pos: x: " + mouse.Position.ToString() + "\n";
            debugInfo += " Mouse Input: LB: " + mouse.LeftButton + ", RB: " + mouse.RightButton + "\n";
            debugInfo += " GamePad: A: " + (gamePad.Buttons.A == ButtonState.Pressed) + ", B: " + (gamePad.Buttons.B == ButtonState.Pressed) + "\n";
            debugInfo += " GamePad LS: " + gamePad.ThumbSticks.Left.ToString() + "\n";
            debugInfo += " GamePad RS: " + gamePad.ThumbSticks.Right.ToString() + "\n";
            debugInfo += "Player\n";
            //debugInfo += " Coordinates: " + state.Player.GetCoordinates();
        }

        public void Draw(GameTime gameTime)
        {
            if (show)
            {
                Vector2 stringRectangle = graphicsContent.Font.MeasureString(debugInfo);
                Vector2 textMiddlePoint = new Vector2(0, 0);
                Vector2 textPosition = new Vector2(POSITION_X + PADDING, POSITION_Y + PADDING);
                Rectangle panelRectangle = new Rectangle(POSITION_X, POSITION_Y, (int)stringRectangle.X + 2 * PADDING, (int)stringRectangle.Y + 2 * PADDING);

                graphicsContent.SpriteBatch.Begin();
                graphicsContent.SpriteBatch.Draw(graphicsContent.BlackPixel, panelRectangle, null, Color.White * 0.5f);
                graphicsContent.SpriteBatch.DrawString(graphicsContent.Font, debugInfo, textPosition, Color.White, 0, textMiddlePoint, 1.0f, SpriteEffects.None, 0.5f);
                graphicsContent.SpriteBatch.End();
            }
        }

        public void Toggle()
        {
            show = !show;
        }
    }
}

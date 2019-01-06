﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Interface
{
    class DebugInfo
    {
        const int POSITION_X = 20;
        const int POSITION_Y = 20;

        Scene.WorldScene world;
        Engine.Content graphicsContent;
        string debugInfo;
        bool display;

        public DebugInfo(Scene.WorldScene world)
        {
            this.world = world;
            graphicsContent = Engine.Content.Instance;
            debugInfo = "";
            display = true;
        }

        public void Update(GameTime gameTime)
        {
            Rectangle viewport = Engine.Window.Instance.GetViewport().Bounds;
            MouseState mouse = Mouse.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            double fps = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);

            debugInfo = "Video\n";
            debugInfo += " FPS: " + fps + "\n";
            debugInfo += " Width: " + viewport.Width + ", Height: " + viewport.Height + "\n";
            debugInfo += "Input\n";
            debugInfo += " Mouse Coords: " + mouse.Position.ToString() + "\n";
            debugInfo += " Tile Coords: " + world.GetMouseCoordinates() + "\n";
            debugInfo += "Player\n";
            debugInfo += " Coordinates: " + world.GetCoordinates() + "\n";
            debugInfo += "Network\n";
            debugInfo += " Status: " + Networking.Client.Instance.GetClientConnectionStatus() + " (" + Networking.Client.Instance.LastLatency + "ms)" + "\n";
            debugInfo += " Last Msg: " + Networking.Client.Instance.GetLastMessage();
        }

        public void Draw(GameTime gameTime)
        {
            if (display)
            {
                Vector2 origin = new Vector2(0, 0);
                Vector2 position = new Vector2(POSITION_X, POSITION_Y);

                graphicsContent.SpriteBatch.Begin();
                graphicsContent.DrawStringOutline(graphicsContent.Font, debugInfo, position, Color.Black, Color.White, 0, origin, 1.0f, SpriteEffects.None, 0.5f);
                graphicsContent.SpriteBatch.End();
            }
        }

        public void Toggle()
        {
            display = !display;
        }
    }
}

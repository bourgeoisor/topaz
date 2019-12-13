using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topaz.Interface
{
    class DevChunkGenerate : Engine.Interface.Panel
    {
        Scene.DevChunkGenerationScene _scene;

        Engine.Interface.Button _generateButton;
        Engine.Interface.Label _generateLabel;

        Engine.Interface.Label _octaveLabel;

        public DevChunkGenerate(Scene.DevChunkGenerationScene scene) : base()
        {
            _scene = scene;

            RelativePosition = new Vector2(1, 1);
            Width = 85;
            Height = 55;
            ParentAnchor = Anchor.TopRight;
            Skin = Engine.Content.Instance.GetTexture("Interface/generate");

            _generateButton = new Engine.Interface.Button(this);
            _generateButton.RelativePosition = new Vector2(5, 5);
            _generateButton.SetOnClicked(delegate
            {
                _scene.RegenerateMap();
            });

            _generateLabel = new Engine.Interface.Label(this);
            _generateLabel.RelativePosition = new Vector2(11, 6);
            _generateLabel.SetText("Generate");

            _octaveLabel = new Engine.Interface.Label(this);
            _octaveLabel.RelativePosition = new Vector2(6, 17);
            _octaveLabel.SetText("Octave: ");
        }

        public void Update(GameTime gameTime)
        {
            _generateButton.Update(gameTime);

            _octaveLabel.SetText("Seed: " + _scene.Seed + "\nOctave: " + _scene.Octave + "\nPersistence: " + _scene.Persistence);
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsDisplaying) return;

            Engine.Content.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);

            Engine.Content.Instance.SpriteBatch.Draw(
                Skin,
                AbsolutePosition(),
                null,
                Color.White,
                0f,
                OriginPoint(),
                Engine.Content.DEFAULT_SCALE,
                SpriteEffects.None,
                0f
            );

            _generateButton.Draw(gameTime);
            _generateLabel.Draw(gameTime);
            _octaveLabel.Draw(gameTime);

            Engine.Content.Instance.SpriteBatch.End();
        }
    }
}

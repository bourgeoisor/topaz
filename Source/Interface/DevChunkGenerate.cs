using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topaz.Interface
{
    class DevChunkGenerate : Engine.Interface.Panel
    {
        Scene.DevChunkGenerationScene _scene;

        Engine.Interface.Label _infoLabel;

        public DevChunkGenerate(Scene.DevChunkGenerationScene scene) : base()
        {
            _scene = scene;

            RelativePosition = new Vector2(1, 1);
            Width = 85;
            Height = 55;
            ParentAnchor = Anchor.TopRight;
            Skin = Engine.Content.Instance.GetTexture("Interface/generate");

            _infoLabel = new Engine.Interface.Label(this);
            _infoLabel.RelativePosition = new Vector2(5, 5);
            _infoLabel.SetText("Data N/A");
            Children.Add(_infoLabel);
        }

        public override void Update()
        {
            _infoLabel.SetText("Perlin Noise\nSeed: " + _scene.Seed + "\nOctave: " + _scene.Octave + "\nPersistence: " + _scene.Persistence);

            base.Update();
        }

        public override void Draw()
        {
            if (!Visible) return;

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

            base.Draw();
        }
    }
}

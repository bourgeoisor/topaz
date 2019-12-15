namespace Topaz.Engine
{
    interface IGameObject
    {
        bool Visible { get; set; }

        void Update();
        void Draw();
    }
}

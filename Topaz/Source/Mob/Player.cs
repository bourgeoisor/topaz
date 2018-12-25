namespace Topaz.Mob
{
    class Player : Mob
    {
        public Player() : base()
        {
            Sprite = Engine.Content.Instance.GetTexture("Temp/lucas");
        }
    }
}

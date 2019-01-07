namespace Topaz.World
{
    class Chunk
    {
        int[,] _layer1;
        int[,] _layer2;

        public Chunk()
        {
        }

        public int[,] Layer1 { get => _layer1; set => _layer1 = value; }
        public int[,] Layer2 { get => _layer2; set => _layer2 = value; }

        public void GenerateRandom()
        {
            int rows = 15;
            int cols = 20;

            _layer1 = new int[rows, cols];
            for (int j = 0; j < _layer1.GetLength(0); j++)
            {
                for (int i = 0; i < _layer1.GetLength(1); i++)
                {
                    _layer1[j, i] = 222;
                }
            }

            _layer2 = new int[rows, cols];
            for (int j = 0; j < _layer2.GetLength(0); j++)
            {
                for (int i = 0; i < _layer2.GetLength(1); i++)
                {
                    if (j < 1 || i < 1 || j > _layer2.GetLength(0)-2 || i > _layer2.GetLength(1)-2)
                    {
                        _layer2[j, i] = 63;
                    }
                    else
                    {
                        _layer2[j, i] = -1;
                    }
                }
            }
        }
    }
}

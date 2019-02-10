namespace Topaz.World
{
    class Chunk
    {
        public Chunk()
        {
        }

        public int[,] Layer1 { get; set; }
        public int[,] Layer2 { get; set; }

        public void GenerateRandom()
        {
            int rows = 15;
            int cols = 20;

            Layer1 = new int[rows, cols];
            for (int j = 0; j < Layer1.GetLength(0); j++)
            {
                for (int i = 0; i < Layer1.GetLength(1); i++)
                {
                    Layer1[j, i] = 0;
                }
            }

            Layer2 = new int[rows, cols];
            for (int j = 0; j < Layer2.GetLength(0); j++)
            {
                for (int i = 0; i < Layer2.GetLength(1); i++)
                {
                    if (j < 1 || i < 1 || j > Layer2.GetLength(0)-2 || i > Layer2.GetLength(1)-2)
                    {
                        Layer2[j, i] = 1;
                    }
                    else
                    {
                        Layer2[j, i] = -1;
                    }
                }
            }
        }
    }
}

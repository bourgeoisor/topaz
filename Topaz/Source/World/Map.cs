namespace Topaz.World
{
    class Map
    {
        int[,] map1;
        int[,] map2;

        public Map()
        {

        }

        public int[,] Map1 { get => map1; set => map1 = value; }
        public int[,] Map2 { get => map2; set => map2 = value; }

        public void GenerateRandom()
        {
            int rows = 10;
            int cols = 15;

            Map1 = new int[rows, cols];
            for (int j = 0; j < Map1.GetLength(0); j++)
            {
                for (int i = 0; i < Map1.GetLength(1); i++)
                {
                    Map1[j, i] = 222;
                }
            }

            Map2 = new int[rows, cols];
            for (int j = 0; j < Map2.GetLength(0); j++)
            {
                for (int i = 0; i < Map2.GetLength(1); i++)
                {
                    if (j == 0 || i == 0 || j == Map2.GetLength(0)-1 || i == Map2.GetLength(1)-1)
                    {
                        Map2[j, i] = 63;
                    }
                    else
                    {
                        Map2[j, i] = -1;
                    }
                }
            }
        }
    }
}

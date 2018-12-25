using System;

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
            Map1 = new int[30, 30];
            for (int j = 0; j < Map1.GetLength(0); j++)
            {
                for (int i = 0; i < Map1.GetLength(1); i++)
                {
                    Map1[j, i] = 222;
                }
            }

            Map2 = new int[30, 30];
            Random rn = new Random();
            for (int j = 0; j < Map2.GetLength(0); j++)
            {
                for (int i = 0; i < Map2.GetLength(1); i++)
                {
                    if (j == 0 || i == 0 || j == Map2.GetLength(0)-1 || i == Map2.GetLength(1)-1 || rn.Next(20) == 0)
                    {
                        Map2[j, i] = 16 * 14;
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

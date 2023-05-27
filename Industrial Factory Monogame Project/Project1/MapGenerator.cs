using Microsoft.Xna.Framework.Graphics;
using System;



namespace Industrial_Factory
{
    internal class MapGenerator
    {
        FastNoiseLite noise;
        double[,] map;
        public int[,] GetCount { get; set; }
        int height, width;
        public MapGenerator(int height, int width, int seed = 1337, float amplitude = 3)
        {
            this.height = height;
            this.width = width;
            NewNoise(seed);
        }
        public void NewNoise(int seed)
        {
            noise = new FastNoiseLite(seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFractalOctaves(1);
            map = new double[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y] = noise.GetNoise(x, y);
                    
                }
            }
        }
        public MapGenerator() { }

        public Texture2D[,] rendermap() { return new Texture2D[1, 1]; }

        public Texture2D[,] rendermap(Texture2D Dirt, Texture2D Water, Texture2D Sand) //render map based on noise generetion
        {
            Texture2D[,] render = new Texture2D[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[y, x] > 0)
                    {
                        Texture2D dirt = Dirt;
                        dirt.Tag = "dirt";
                        render[x, y] = dirt;

                    }
                    else if (map[y, x] > -0.66)
                    {
                        Texture2D sand = Sand;
                        sand.Tag = "sand";
                        render[x, y] = sand;
                    }
                    else if (map[y, x] > -1)
                    {
                        Texture2D water = Water;
                        water.Tag = "water";
                        render[x, y] = water;

                    }
                    else
                    {
                        Texture2D dirt = Dirt;
                        dirt.Tag = "dirt";
                        render[x, y] = dirt;

                    }
                }
            }
            return render;
        }

        public Texture2D[,] oreGenerator(Texture2D Ore, int seed, double sizeOfField)
        {
            int[,] temp = new int[width, height];
            NewNoise(seed);
            Texture2D[,] render = new Texture2D[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[y, x] < sizeOfField)
                    {
                        temp[x, y] = Math.Abs((int)(map[x, y] * 100)) + 1;
                        render[x, y] = Ore;
                    }
                }
            }
            GetCount = temp;
            return render;
        }
    }
}

using UnityEngine;

namespace FG
{
    public class AStarNodeGrid
    {
        // CONFIG
        private int width;
        private int height;

        // MEMORY
        private AStarNode[,] nodes;

        public AStarNodeGrid(int width, int height)
        {
            this.width = width;
            this.height = height;

            GenerateGrid();
        }

        private void GenerateGrid()
        {
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    nodes[x, y] = new AStarNode(new Vector2Int(x, y));
                }
            }
        }

        public AStarNode GetNode(int x, int y)
        {
            if (x >= width || y >= height)
                return null;

            return nodes[x, y];
        }
    }
}

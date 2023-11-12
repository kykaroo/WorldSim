using System.Collections.Generic;
using MapGenerator;

namespace Pathfinding
{
    public class Node
    {
        public Tile Tile;
        public readonly List<Edge> Edges;

        public Node(Tile tile)
        {
            Tile = tile;
            Edges = new();
        }
    }
}
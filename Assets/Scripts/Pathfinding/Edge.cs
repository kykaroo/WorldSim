namespace Pathfinding
{
    public class Edge
    {
        private float _moveCost;
        
        public Node Node;

        public Edge(Node node, float moveCost)
        {
            Node = node;
            _moveCost = moveCost;
        }
    }
}
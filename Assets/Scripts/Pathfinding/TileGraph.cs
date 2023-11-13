using System.Collections.Generic;
using Data;
using MapGenerator;
using UnityEngine;

namespace Pathfinding
{
    public class TileGraph
    {
        private WorldController _worldController;
        private readonly WorldData _worldData;
        
        public Dictionary<Tile, Node> Nodes { get; }
        
        public TileGraph(WorldController worldController)
        {
            _worldController = worldController;
            _worldData = worldController.GetWorldData();
            Nodes = new();
            
            for (var x = 0; x < _worldData.Width; x++)
            {
                for (var y = 0; y < _worldData.Height; y++)
                {
                    var tile = _worldController.GetTile(x, y);

                    var node = new Node(tile);
                    Nodes.Add(tile, node);
                }
            }

            foreach (var tile in Nodes.Keys)
            {
                var node = Nodes[tile];
                var neighbours = _worldController.GetTileNeighbours(tile);

                foreach (var neighbourTile in neighbours.Values)
                {
                    if (neighbourTile == null) continue;
                    if (neighbourTile.MoveSpeedMultiplier == 0) continue;
                    
                    var edge = new Edge(Nodes[neighbourTile], neighbourTile.MoveSpeedMultiplier);
                    node.Edges.Add(edge);
                }
            }
            
            Debug.Log($"Created {Nodes.Count}");
        }

        public void ChangeNode()
        {
            
        }
    }
}
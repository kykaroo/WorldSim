using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using MapGenerator;
using Priority_Queue;
using UnityEngine;
using Zenject;

namespace Pathfinding
{
    public class Pathfinder
    {
        private WorldController _worldController;
        private TileGraph _tileGraph;

        [Inject]
        public Pathfinder(WorldController worldController)
        {
            _worldController = worldController;
            _worldController.OnMapGenerated += () => _tileGraph = new(_worldController);
        }

        public Queue<Tile> FindPath(Tile tileStart, Tile tileEnd)
        {
            if (!_tileGraph.Nodes.ContainsKey(tileStart))
            {
                throw new ArgumentException(nameof(tileStart), $"Ноды тайла {tileStart} не существует");
            }
            
            if (!_tileGraph.Nodes.ContainsKey(tileEnd))
            {
                throw new ArgumentException(nameof(tileStart), $"Ноды тайла {tileEnd} не существует");
            }
            
            var openSet = new SimplePriorityQueue<Node>();
            var startNode = _tileGraph.Nodes[tileStart];
            var endNode = _tileGraph.Nodes[tileEnd];
            
            openSet.Enqueue(startNode, 0);

            var cameFrom = new Dictionary<Node, Node>();
            var gScore = new Dictionary<Node, float>();

            foreach (var node in _tileGraph.Nodes.Values)
            {
                gScore[node] = Mathf.Infinity;
            }

            gScore[startNode] = 0;
            
            var fScore = new Dictionary<Node, float>();

            foreach (var node in _tileGraph.Nodes.Values)
            {
                fScore[node] = Mathf.Infinity;
            }

            fScore[startNode] = DistanceBetween(startNode, endNode);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current == endNode)
                {
                    return ReconstructMap(cameFrom, current);
                }

                foreach (var neighbour in current.Edges)
                {
                    var tentativeGScore = gScore[current] + DistanceBetween(current, neighbour.Node) / neighbour.Node.Tile.MoveSpeedMultiplier;

                    if (tentativeGScore >= gScore[neighbour.Node]) continue;

                    cameFrom[neighbour.Node] = current;
                    gScore[neighbour.Node] = tentativeGScore;
                    fScore[neighbour.Node] = gScore[neighbour.Node] + DistanceBetween(neighbour.Node, endNode);

                    if (!openSet.Contains(neighbour.Node))
                    {
                        openSet.Enqueue(neighbour.Node, fScore[neighbour.Node]);
                    }
                }
            }

            Debug.Log("Путь не найден");
            return null;
        }

        private Queue<Tile> ReconstructMap(Dictionary<Node, Node> cameFrom, Node current)
        {
            var totalPath = new Queue<Tile>();
            // totalPath.Enqueue(current.Tile); // Место назначения включительно

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Enqueue(current.Tile);
            }

            var reverse = totalPath.Reverse();
            var path = new Queue<Tile>(reverse);
            path.Dequeue(); //Исключая стартовый таил

            return path;
        }

        float DistanceBetween(Node start, Node end)
        {
            return Mathf.Sqrt(Mathf.Pow(start.Tile.X - end.Tile.X, 2) + Mathf.Pow(start.Tile.Y - end.Tile.Y, 2));
        }
    }
}
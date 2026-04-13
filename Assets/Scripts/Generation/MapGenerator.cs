using CaravanRoguelite.Data;
using UnityEngine;

namespace CaravanRoguelite.Generation
{
    public class MapGenerator
    {
        public MapGraph Create(int seed)
        {
            Random.InitState(seed);
            var graph = new MapGraph();
            int lanes = 3;
            int depth = 6;
            int id = 0;

            for (int d = 0; d < depth; d++)
            {
                for (int l = 0; l < lanes; l++)
                {
                    var type = PickType(d, l, lanes, depth);
                    graph.Nodes.Add(new MapNode
                    {
                        Id = id++,
                        Type = type,
                        Position = new Vector2(d * 2.6f, (l - 1) * 2.1f + Random.Range(-0.3f, 0.3f))
                    });
                }
            }

            for (int d = 0; d < depth - 1; d++)
            {
                for (int l = 0; l < lanes; l++)
                {
                    var from = graph.Nodes[d * lanes + l];
                    for (int offset = -1; offset <= 1; offset++)
                    {
                        int toLane = l + offset;
                        if (toLane < 0 || toLane >= lanes)
                        {
                            continue;
                        }

                        if (Random.value > 0.48f)
                        {
                            from.Links.Add((d + 1) * lanes + toLane);
                        }
                    }

                    if (from.Links.Count == 0)
                    {
                        from.Links.Add((d + 1) * lanes + l);
                    }
                }
            }

            graph.Nodes[0].Type = NodeType.Start;
            graph.Nodes[depth * lanes - 2].Type = NodeType.Boss;
            return graph;
        }

        private NodeType PickType(int depth, int lane, int lanes, int maxDepth)
        {
            if (depth == maxDepth - 1)
            {
                return lane == lanes / 2 ? NodeType.Boss : NodeType.Combat;
            }

            float roll = Random.value;
            if (roll < 0.25f) return NodeType.City;
            if (roll < 0.58f) return NodeType.Event;
            return NodeType.Combat;
        }
    }
}

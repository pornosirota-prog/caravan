using System.Collections.Generic;
using UnityEngine;

namespace CaravanRoguelite.Data
{
    public class MapNode
    {
        public int Id;
        public NodeType Type;
        public Vector2 Position;
        public readonly List<int> Links = new();
        public bool Visited;
    }
}

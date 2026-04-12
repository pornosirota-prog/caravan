using System.Collections.Generic;

namespace CaravanRoguelite.Data
{
    public class MapGraph
    {
        public readonly List<MapNode> Nodes = new();

        public MapNode Get(int id)
        {
            return Nodes.Find(node => node.Id == id);
        }
    }
}

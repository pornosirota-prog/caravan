using System;
using UnityEngine.Tilemaps;

namespace CaravanRoguelite.GridMap
{
    [Serializable]
    public sealed class GridTileDefinition
    {
        public GridTileType Type = GridTileType.Grass;
        public TileBase Tile;
        public bool IsPassable = true;
    }
}

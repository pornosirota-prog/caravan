using UnityEngine;

namespace CaravanRoguelite.GridMap
{
    public readonly struct GridMapCell
    {
        public GridMapCell(Vector2Int position, GridTileType tileType)
        {
            Position = position;
            TilemapPosition = new Vector3Int(position.x, position.y, 0);
            TileType = tileType;
        }

        public Vector2Int Position { get; }
        public Vector3Int TilemapPosition { get; }
        public GridTileType TileType { get; }
    }
}

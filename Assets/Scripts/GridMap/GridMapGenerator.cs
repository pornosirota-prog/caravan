using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CaravanRoguelite.GridMap
{
    [RequireComponent(typeof(Grid))]
    public sealed class GridMapGenerator : MonoBehaviour
    {
        [SerializeField] private int width = 50;
        [SerializeField] private int height = 50;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private GridTileDefinition[] tileDefinitions =
        {
            new GridTileDefinition { Type = GridTileType.Grass, IsPassable = true },
            new GridTileDefinition { Type = GridTileType.Water, IsPassable = false },
            new GridTileDefinition { Type = GridTileType.Mountain, IsPassable = false },
            new GridTileDefinition { Type = GridTileType.Road, IsPassable = true }
        };

        private readonly Dictionary<Vector2Int, GridMapCell> cells = new Dictionary<Vector2Int, GridMapCell>();
        private readonly Dictionary<GridTileType, GridTileDefinition> definitionsByType = new Dictionary<GridTileType, GridTileDefinition>();
        private Grid grid;
        private TileBase fallbackGrassTile;
        private Sprite fallbackGrassSprite;

        public int Width => width;
        public int Height => height;
        public IReadOnlyDictionary<Vector2Int, GridMapCell> Cells => cells;

        private void Awake()
        {
            InitializeComponents();
            CacheTileDefinitions();
        }

        private void OnValidate()
        {
            width = Mathf.Max(1, width);
            height = Mathf.Max(1, height);
        }

        public void GenerateMap()
        {
            InitializeComponents();
            CacheTileDefinitions();

            cells.Clear();
            tilemap.ClearAllTiles();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var position = new Vector2Int(x, y);
                    GridTileType tileType = ResolveTileType(position);
                    var cell = new GridMapCell(position, tileType);

                    cells[position] = cell;
                    tilemap.SetTile(cell.TilemapPosition, ResolveTile(tileType));
                }
            }

            tilemap.CompressBounds();
        }

        public bool IsInsideBounds(Vector2Int position)
        {
            return position.x >= 0
                && position.y >= 0
                && position.x < width
                && position.y < height;
        }

        public bool IsPassable(Vector2Int position)
        {
            if (!cells.TryGetValue(position, out GridMapCell cell))
            {
                return false;
            }

            return definitionsByType.TryGetValue(cell.TileType, out GridTileDefinition definition)
                && definition.IsPassable;
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            InitializeComponents();
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);
            return new Vector2Int(cellPosition.x, cellPosition.y);
        }

        public Vector3 CellToWorld(Vector2Int cellPosition)
        {
            InitializeComponents();
            var tilemapPosition = new Vector3Int(cellPosition.x, cellPosition.y, 0);
            return grid.GetCellCenterWorld(tilemapPosition);
        }

        private void InitializeComponents()
        {
            if (grid == null)
            {
                grid = GetComponent<Grid>();
            }

            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            grid.cellSize = Vector3.one;

            if (tilemap == null)
            {
                tilemap = GetComponentInChildren<Tilemap>();
            }

            if (tilemap == null)
            {
                var tilemapObject = new GameObject("Generated Tilemap", typeof(Tilemap), typeof(TilemapRenderer));
                tilemapObject.transform.SetParent(transform, false);
                tilemap = tilemapObject.GetComponent<Tilemap>();
            }
        }

        private void CacheTileDefinitions()
        {
            definitionsByType.Clear();

            if (tileDefinitions == null)
            {
                return;
            }

            foreach (GridTileDefinition definition in tileDefinitions)
            {
                if (definition == null)
                {
                    continue;
                }

                definitionsByType[definition.Type] = definition;
            }
        }

        private GridTileType ResolveTileType(Vector2Int position)
        {
            return GridTileType.Grass;
        }

        private TileBase ResolveTile(GridTileType tileType)
        {
            if (definitionsByType.TryGetValue(tileType, out GridTileDefinition definition) && definition.Tile != null)
            {
                return definition.Tile;
            }

            return GetFallbackGrassTile();
        }

        private TileBase GetFallbackGrassTile()
        {
            if (fallbackGrassTile != null)
            {
                return fallbackGrassTile;
            }

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = "Runtime Grass Tile";
            tile.sprite = GetFallbackGrassSprite();
            tile.color = new Color(0.32f, 0.68f, 0.26f);
            fallbackGrassTile = tile;
            return fallbackGrassTile;
        }

        private Sprite GetFallbackGrassSprite()
        {
            if (fallbackGrassSprite != null)
            {
                return fallbackGrassSprite;
            }

            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            fallbackGrassSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            fallbackGrassSprite.name = "Runtime Grass Sprite";
            return fallbackGrassSprite;
        }
    }
}

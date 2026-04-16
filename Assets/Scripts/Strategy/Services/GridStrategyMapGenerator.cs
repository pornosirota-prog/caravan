using CaravanRoguelite.Strategy.Domain;

namespace CaravanRoguelite.Strategy.Services
{
    public sealed class GridStrategyMapGenerator : IStrategyMapGenerator
    {
        public StrategyMapModel Generate()
        {
            var map = new StrategyMapModel();
            int id = 0;
            int centerRow = StrategyConfig.MapRows / 2;
            int centerCol = StrategyConfig.MapCols / 2;

            for (int row = 0; row < StrategyConfig.MapRows; row++)
            {
                for (int col = 0; col < StrategyConfig.MapCols; col++)
                {
                    var territory = new TerritoryModel
                    {
                        Id = id++,
                        Row = row,
                        Col = col,
                        Type = ResolveType(row, col),
                        Owner = row == centerRow && col == centerCol ? TerritoryOwner.Player : TerritoryOwner.Neutral,
                        RewardGold = 5 + row + col,
                        RewardFood = 2 + ((row + col) % 3)
                    };

                    map.Territories.Add(territory);
                }
            }

            foreach (var territory in map.Territories)
            {
                LinkNeighbors(map, territory);
            }

            return map;
        }

        private static TerritoryType ResolveType(int row, int col)
        {
            int seed = row + col;
            return seed % 4 switch
            {
                0 => TerritoryType.Plains,
                1 => TerritoryType.Farmland,
                2 => TerritoryType.Mine,
                _ => TerritoryType.Fortress
            };
        }

        private static void LinkNeighbors(StrategyMapModel map, TerritoryModel territory)
        {
            foreach (var other in map.Territories)
            {
                if (other.Id == territory.Id)
                {
                    continue;
                }

                bool isNeighbor = (other.Row == territory.Row && System.Math.Abs(other.Col - territory.Col) == 1)
                    || (other.Col == territory.Col && System.Math.Abs(other.Row - territory.Row) == 1);

                if (isNeighbor)
                {
                    territory.Neighbors.Add(other.Id);
                }
            }
        }
    }
}

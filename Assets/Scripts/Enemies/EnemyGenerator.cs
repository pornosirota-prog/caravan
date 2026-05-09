using System;
using System.Collections.Generic;
using CaravanRoguelite.Run;

namespace CaravanRoguelite.Enemies
{
    public sealed class EnemyGenerator
    {
        private readonly List<string> _enemyNames = new()
        {
            "Гоблин",
            "Скелет",
            "Культист",
            "Волк",
            "Бандит",
            "Огненный бес",
            "Ядовитый паук",
            "Бронированный страж"
        };

        private readonly List<string> _bossNames = new()
        {
            "Король гоблинов",
            "Некромант",
            "Огненный демон",
            "Железный рыцарь",
            "Пожиратель карт"
        };

        private readonly Random _random;
        private readonly InfiniteScalingService _scalingService = new();

        public EnemyGenerator(int seed = 0)
        {
            _random = seed == 0 ? new Random() : new Random(seed);
        }

        public EnemyModel Generate(int floor, int act, bool isBoss)
        {
            var baseStats = new EnemyStats
            {
                MaxHealth = (isBoss ? 70 : 30) + _random.Next(0, isBoss ? 35 : 20),
                Damage = (isBoss ? 10 : 5) + _random.Next(0, isBoss ? 8 : 5),
                Armor = _random.Next(0, isBoss ? 5 : 3)
            };

            var scaledStats = _scalingService.Scale(baseStats, floor, act);
            return new EnemyModel
            {
                Name = isBoss ? _bossNames[_random.Next(_bossNames.Count)] : _enemyNames[_random.Next(_enemyNames.Count)],
                Stats = scaledStats,
                Traits = GenerateTraits(floor, act, isBoss),
                IsBoss = isBoss
            };
        }

        private List<EnemyTrait> GenerateTraits(int floor, int act, bool isBoss)
        {
            var traits = new List<EnemyTrait>();
            int budget = isBoss ? 2 : 1;
            if (act >= 5 && isBoss)
            {
                budget++;
            }

            TryAdd(traits, EnemyTrait.Armored, floor >= 5, 0.25, budget);
            TryAdd(traits, EnemyTrait.Regenerating, floor >= 10, 0.20, budget);
            TryAdd(traits, EnemyTrait.Berserker, floor >= 15, 0.15, budget);
            TryAdd(traits, EnemyTrait.FireResistant, isBoss || floor >= 8, 0.20, budget);
            TryAdd(traits, EnemyTrait.CriticalResistant, isBoss || floor >= 12, 0.18, budget);
            TryAdd(traits, EnemyTrait.LimitsCards, isBoss, 0.35, budget);
            TryAdd(traits, EnemyTrait.PunishesHealing, isBoss && act >= 2, 0.25, budget);
            return traits;
        }

        private void TryAdd(List<EnemyTrait> traits, EnemyTrait trait, bool enabled, double chance, int budget)
        {
            if (enabled && traits.Count < budget && _random.NextDouble() < chance)
            {
                traits.Add(trait);
            }
        }
    }
}

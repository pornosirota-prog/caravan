using System.Collections.Generic;

namespace CaravanRoguelite.Combat
{
    public static class EnemyLibrary
    {
        public static List<EnemyDefinition> CreateDefault()
        {
            return new List<EnemyDefinition>
            {
                new() { Name = "Налётчик", MaxHealth = 12, Attack = 4, RewardGold = 10 },
                new() { Name = "Пустынный волк", MaxHealth = 10, Attack = 5, RewardGold = 8 },
                new() { Name = "Бронированный мародёр", MaxHealth = 16, Attack = 5, RewardGold = 14 },
                new() { Name = "Вождь пустоши", MaxHealth = 26, Attack = 7, RewardGold = 30 }
            };
        }
    }
}

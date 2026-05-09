using CaravanRoguelite.Enemies;

namespace CaravanRoguelite.Run
{
    public sealed class InfiniteScalingService
    {
        public EnemyStats Scale(EnemyStats baseStats, int floor, int act)
        {
            float actPressure = 1f + (act - 1) * 0.3f;
            float healthMultiplier = actPressure + floor * 0.12f;
            float damageMultiplier = actPressure + floor * 0.08f;

            return new EnemyStats
            {
                MaxHealth = (int)(baseStats.MaxHealth * healthMultiplier),
                CurrentHealth = (int)(baseStats.MaxHealth * healthMultiplier),
                Damage = (int)(baseStats.Damage * damageMultiplier),
                Armor = baseStats.Armor + floor / 5 + act - 1
            };
        }
    }
}

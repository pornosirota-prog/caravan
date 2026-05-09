using System.Collections.Generic;

namespace CaravanRoguelite.Enemies
{
    public sealed class EnemyModel
    {
        public string Name { get; set; }
        public EnemyStats Stats { get; set; }
        public List<EnemyTrait> Traits { get; set; } = new();
        public bool IsBoss { get; set; }
    }
}

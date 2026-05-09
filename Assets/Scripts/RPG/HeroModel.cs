namespace CaravanRoguelite.RPG
{
    public sealed class HeroModel
    {
        public int Level { get; set; } = 1;
        public int Experience { get; set; }
        public int MaxHealth { get; set; } = 100;
        public int CurrentHealth { get; set; } = 100;
        public int Strength { get; set; }
        public int MagicPower { get; set; }
        public int Armor { get; set; }
        public int CritChance { get; set; } = 5;
        public int CritDamagePercent { get; set; } = 150;
    }
}

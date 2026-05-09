namespace CaravanRoguelite.Cards
{
    public sealed class ComboResult
    {
        public ComboResult(string title, int damage, int block, int healing, StatusEffect statusEffect)
        {
            Title = title;
            Damage = damage;
            Block = block;
            Healing = healing;
            StatusEffect = statusEffect;
        }

        public string Title { get; }
        public int Damage { get; }
        public int Block { get; }
        public int Healing { get; }
        public StatusEffect StatusEffect { get; }
    }
}

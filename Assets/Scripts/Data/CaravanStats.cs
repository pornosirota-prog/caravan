namespace CaravanRoguelite.Data
{
    public class CaravanStats
    {
        public int Gold = 30;
        public int Food = 12;
        public int Morale = 6;
        public int Health = 24;
        public int MaxHealth = 24;
        public int Attack = 5;
        public int Armor = 0;

        public bool IsDefeated => Health <= 0 || Food < 0 || Morale <= 0;
    }
}

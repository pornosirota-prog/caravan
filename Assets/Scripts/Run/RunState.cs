namespace CaravanRoguelite.Run
{
    public sealed class RunState
    {
        public int Floor { get; set; } = 1;
        public int Act { get; set; } = 1;
        public int BossInterval { get; set; } = 5;
        public int ShopsVisited { get; set; }

        public bool IsBossFloor => Floor % BossInterval == 0;

        public void AdvanceAfterCombat(bool defeatedBoss)
        {
            Floor++;
            if (defeatedBoss)
            {
                Act++;
            }
        }
    }
}

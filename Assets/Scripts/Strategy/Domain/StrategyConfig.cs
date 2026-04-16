namespace CaravanRoguelite.Strategy.Domain
{
    public static class StrategyConfig
    {
        public const int MapRows = 3;
        public const int MapCols = 3;
        public const float BattleDurationSeconds = 20f;
        public const float CaptureProgressTarget = 100f;
        public const float BaseCorrectProgress = 16f;
        public const float ComboBonusPerStep = 0.08f;
        public const float WrongAnswerPenalty = 7f;
        public const int MaxHistoryEntries = 8;
        public const int MinTaskNumber = 2;
        public const int MaxTaskNumber = 12;
    }
}

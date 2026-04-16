namespace CaravanRoguelite.Strategy.Domain
{
    public interface IStrategyMapGenerator
    {
        StrategyMapModel Generate();
    }

    public interface IMathTaskGenerator
    {
        MathTask NextTask();
    }

    public interface IBattleProgressionService
    {
        BattleAnswerResult ApplyAnswer(BattleSessionState session, bool isCorrect, float answerTimeSeconds);
    }

    public interface IRewardCalculator
    {
        RewardBundle Calculate(TerritoryModel territory, BattleSessionState session, bool captured);
    }
}

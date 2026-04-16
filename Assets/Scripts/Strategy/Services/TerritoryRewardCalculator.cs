using CaravanRoguelite.Strategy.Domain;
using UnityEngine;

namespace CaravanRoguelite.Strategy.Services
{
    public sealed class TerritoryRewardCalculator : IRewardCalculator
    {
        public RewardBundle Calculate(TerritoryModel territory, BattleSessionState session, bool captured)
        {
            if (!captured)
            {
                return new RewardBundle(0, 0, Mathf.RoundToInt(session.Progress * 0.25f));
            }

            int comboBonus = Mathf.Clamp(session.MaxCombo - 2, 0, 8);
            int gold = territory.RewardGold + comboBonus;
            int food = territory.RewardFood + Mathf.RoundToInt(session.Accuracy * 2f);
            int score = 25 + Mathf.RoundToInt(session.Accuracy * 20f) + comboBonus;
            return new RewardBundle(gold, food, score);
        }
    }
}

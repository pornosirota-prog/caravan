using CaravanRoguelite.Strategy.Domain;
using UnityEngine;

namespace CaravanRoguelite.Strategy.Services
{
    public sealed class ComboBattleProgressionService : IBattleProgressionService
    {
        public BattleAnswerResult ApplyAnswer(BattleSessionState session, bool isCorrect, float answerTimeSeconds)
        {
            session.SolvedTotal++;
            if (isCorrect)
            {
                session.SolvedCorrect++;
                session.Combo++;
                session.MaxCombo = Mathf.Max(session.MaxCombo, session.Combo);

                float speedMultiplier = answerTimeSeconds <= 2.5f ? 1.2f : 1f;
                float comboMultiplier = 1f + (session.Combo - 1) * StrategyConfig.ComboBonusPerStep;
                float gain = StrategyConfig.BaseCorrectProgress * speedMultiplier * comboMultiplier;
                session.Progress = Mathf.Min(StrategyConfig.CaptureProgressTarget, session.Progress + gain);
            }
            else
            {
                session.Combo = 0;
                session.Progress = Mathf.Max(0f, session.Progress - StrategyConfig.WrongAnswerPenalty);
            }

            bool captured = session.Progress >= StrategyConfig.CaptureProgressTarget;
            return new BattleAnswerResult(isCorrect, captured, session.Progress, session.Combo);
        }
    }
}

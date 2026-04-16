using CaravanRoguelite.Strategy.Domain;
using UnityEngine;

namespace CaravanRoguelite.Strategy.Services
{
    public sealed class StrategyGameService
    {
        private readonly IStrategyMapGenerator _mapGenerator;
        private readonly IMathTaskGenerator _mathTaskGenerator;
        private readonly IBattleProgressionService _battleProgression;
        private readonly IRewardCalculator _rewardCalculator;

        public StrategyGameService(
            IStrategyMapGenerator mapGenerator,
            IMathTaskGenerator mathTaskGenerator,
            IBattleProgressionService battleProgression,
            IRewardCalculator rewardCalculator)
        {
            _mapGenerator = mapGenerator;
            _mathTaskGenerator = mathTaskGenerator;
            _battleProgression = battleProgression;
            _rewardCalculator = rewardCalculator;
        }

        public StrategyGameState CreateNewState()
        {
            return new StrategyGameState
            {
                Map = _mapGenerator.Generate(),
                Gold = 10,
                Food = 8,
                Score = 0
            };
        }

        public bool CanAttack(StrategyGameState state, int territoryId)
        {
            var target = FindTerritory(state, territoryId);
            if (target == null || target.Owner == TerritoryOwner.Player)
            {
                return false;
            }

            foreach (int neighborId in target.Neighbors)
            {
                var neighbor = FindTerritory(state, neighborId);
                if (neighbor != null && neighbor.Owner == TerritoryOwner.Player)
                {
                    return true;
                }
            }

            return false;
        }

        public BattleSessionState StartBattle(StrategyGameState state, int territoryId)
        {
            if (!CanAttack(state, territoryId))
            {
                return null;
            }

            var session = new BattleSessionState
            {
                TargetTerritoryId = territoryId,
                Progress = 0f,
                RemainingTime = StrategyConfig.BattleDurationSeconds,
                CurrentTask = _mathTaskGenerator.NextTask()
            };

            state.ActiveBattle = session;
            AppendHistory(state, $"Начат штурм территории #{territoryId}.");
            return session;
        }

        public BattleAnswerResult SubmitAnswer(StrategyGameState state, int answer, float answerTimeSeconds)
        {
            var session = state.ActiveBattle;
            if (session == null)
            {
                return default;
            }

            bool isCorrect = answer == session.CurrentTask.Answer;
            var result = _battleProgression.ApplyAnswer(session, isCorrect, answerTimeSeconds);
            session.CurrentTask = _mathTaskGenerator.NextTask();

            if (result.IsCorrect)
            {
                AppendHistory(state, $"Верный ответ. Прогресс: {Mathf.RoundToInt(result.Progress)}%.");
            }
            else
            {
                AppendHistory(state, $"Ошибка. Прогресс: {Mathf.RoundToInt(result.Progress)}%.");
            }

            return result;
        }

        public BattleOutcome TickBattle(StrategyGameState state, float deltaTime)
        {
            var session = state.ActiveBattle;
            if (session == null)
            {
                return default;
            }

            session.RemainingTime = Mathf.Max(0f, session.RemainingTime - deltaTime);
            if (session.Progress >= StrategyConfig.CaptureProgressTarget || session.RemainingTime <= 0f)
            {
                return CompleteBattle(state);
            }

            return default;
        }

        private BattleOutcome CompleteBattle(StrategyGameState state)
        {
            var session = state.ActiveBattle;
            var target = FindTerritory(state, session.TargetTerritoryId);
            bool captured = session.Progress >= StrategyConfig.CaptureProgressTarget;
            if (captured && target != null)
            {
                target.Owner = TerritoryOwner.Player;
            }

            RewardBundle reward = target == null
                ? new RewardBundle(0, 0, 0)
                : _rewardCalculator.Calculate(target, session, captured);

            state.Gold += reward.Gold;
            state.Food += reward.Food;
            state.Score += reward.Score;
            state.ActiveBattle = null;

            AppendHistory(state, captured
                ? $"Территория #{target.Id} захвачена. +{reward.Gold} золота, +{reward.Food} еды."
                : $"Штурм территории #{target?.Id} провален. Получено {reward.Score} очков опыта.");

            return new BattleOutcome(captured, captured, reward, session.Accuracy, session.SolvedTotal, session.MaxCombo);
        }

        private static TerritoryModel FindTerritory(StrategyGameState state, int territoryId)
        {
            foreach (var territory in state.Map.Territories)
            {
                if (territory.Id == territoryId)
                {
                    return territory;
                }
            }

            return null;
        }

        private static void AppendHistory(StrategyGameState state, string message)
        {
            state.History.Insert(0, message);
            if (state.History.Count > StrategyConfig.MaxHistoryEntries)
            {
                state.History.RemoveAt(state.History.Count - 1);
            }
        }
    }
}

using System.Collections.Generic;

namespace CaravanRoguelite.Strategy.Domain
{
    public enum TerritoryType
    {
        Plains,
        Mine,
        Farmland,
        Fortress
    }

    public enum TerritoryOwner
    {
        Neutral,
        Player
    }

    public sealed class TerritoryModel
    {
        public int Id;
        public int Row;
        public int Col;
        public TerritoryType Type;
        public TerritoryOwner Owner;
        public int RewardGold;
        public int RewardFood;
        public readonly List<int> Neighbors = new();
    }

    public sealed class StrategyMapModel
    {
        public readonly List<TerritoryModel> Territories = new();
    }

    public readonly struct RewardBundle
    {
        public readonly int Gold;
        public readonly int Food;
        public readonly int Score;

        public RewardBundle(int gold, int food, int score)
        {
            Gold = gold;
            Food = food;
            Score = score;
        }
    }

    public readonly struct MathTask
    {
        public readonly string Prompt;
        public readonly int Answer;

        public MathTask(string prompt, int answer)
        {
            Prompt = prompt;
            Answer = answer;
        }
    }

    public sealed class BattleSessionState
    {
        public int TargetTerritoryId;
        public float Progress;
        public int SolvedTotal;
        public int SolvedCorrect;
        public int Combo;
        public int MaxCombo;
        public float RemainingTime;
        public MathTask CurrentTask;

        public float Accuracy => SolvedTotal <= 0 ? 0f : SolvedCorrect / (float)SolvedTotal;
    }

    public readonly struct BattleAnswerResult
    {
        public readonly bool IsCorrect;
        public readonly bool Captured;
        public readonly float Progress;
        public readonly int Combo;

        public BattleAnswerResult(bool isCorrect, bool captured, float progress, int combo)
        {
            IsCorrect = isCorrect;
            Captured = captured;
            Progress = progress;
            Combo = combo;
        }
    }

    public readonly struct BattleOutcome
    {
        public readonly bool Win;
        public readonly bool Captured;
        public readonly RewardBundle Reward;
        public readonly float Accuracy;
        public readonly int Solved;
        public readonly int MaxCombo;

        public BattleOutcome(bool win, bool captured, RewardBundle reward, float accuracy, int solved, int maxCombo)
        {
            Win = win;
            Captured = captured;
            Reward = reward;
            Accuracy = accuracy;
            Solved = solved;
            MaxCombo = maxCombo;
        }
    }

    public sealed class StrategyGameState
    {
        public StrategyMapModel Map;
        public int Gold;
        public int Food;
        public int Score;
        public readonly List<string> History = new();
        public BattleSessionState ActiveBattle;

        public int CapturedCount
        {
            get
            {
                int captured = 0;
                foreach (var territory in Map.Territories)
                {
                    if (territory.Owner == TerritoryOwner.Player)
                    {
                        captured++;
                    }
                }

                return captured;
            }
        }
    }
}

using CaravanRoguelite.Strategy.Domain;
using UnityEngine;

namespace CaravanRoguelite.Strategy.Services
{
    public sealed class AdditionMathTaskGenerator : IMathTaskGenerator
    {
        public MathTask NextTask()
        {
            int left = Random.Range(StrategyConfig.MinTaskNumber, StrategyConfig.MaxTaskNumber + 1);
            int right = Random.Range(StrategyConfig.MinTaskNumber, StrategyConfig.MaxTaskNumber + 1);
            return new MathTask($"{left} + {right}", left + right);
        }
    }
}

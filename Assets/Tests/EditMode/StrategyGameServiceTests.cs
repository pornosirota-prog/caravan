using System.Collections.Generic;
using CaravanRoguelite.Cards;
using CaravanRoguelite.Enemies;
using CaravanRoguelite.Run;
using CaravanRoguelite.Strategy.Domain;
using CaravanRoguelite.Strategy.Services;
using NUnit.Framework;

namespace CaravanRoguelite.Tests.EditMode
{
    public class StrategyGameServiceTests
    {
        [Test]
        public void MapGenerator_GivesSinglePlayerStartTerritory()
        {
            var map = new GridStrategyMapGenerator().Generate();
            int owned = 0;
            foreach (var territory in map.Territories)
            {
                if (territory.Owner == TerritoryOwner.Player)
                {
                    owned++;
                }
            }

            Assert.AreEqual(1, owned);
        }

        [Test]
        public void BattleProgression_CorrectAnswersIncreaseProgress()
        {
            var session = new BattleSessionState();
            var service = new ComboBattleProgressionService();
            var first = service.ApplyAnswer(session, true, 1.5f);
            var second = service.ApplyAnswer(session, true, 1.2f);

            Assert.Greater(first.Progress, 0f);
            Assert.Greater(second.Progress, first.Progress);
            Assert.GreaterOrEqual(session.MaxCombo, 2);
        }

        [Test]
        public void RewardCalculator_CapturedTerritoryGivesResources()
        {
            var territory = new TerritoryModel { RewardGold = 7, RewardFood = 3 };
            var session = new BattleSessionState { Progress = 100f, SolvedTotal = 5, SolvedCorrect = 4, MaxCombo = 4 };
            var reward = new TerritoryRewardCalculator().Calculate(territory, session, true);

            Assert.Greater(reward.Gold, 0);
            Assert.Greater(reward.Food, 0);
            Assert.Greater(reward.Score, 0);
        }

        [Test]
        public void StartBattle_AllowsOnlyNeighborOfOwnedTerritory()
        {
            var game = BuildGameService();
            var state = game.CreateNewState();

            int farCornerId = 0;
            int centerId = 4;
            Assert.IsFalse(game.CanAttack(state, farCornerId));

            foreach (var territory in state.Map.Territories)
            {
                if (territory.Id == centerId)
                {
                    continue;
                }

                if (territory.Neighbors.Contains(centerId))
                {
                    Assert.IsTrue(game.CanAttack(state, territory.Id));
                    return;
                }
            }

            Assert.Fail("Expected at least one attackable neighbor.");
        }

        [Test]
        public void CompletingBattle_CapturesTerritoryAndKeepsCaravanStateIsolated()
        {
            var game = BuildGameService(new FixedMathTaskGenerator(5));
            var state = game.CreateNewState();
            int initialGold = state.Gold;
            int targetId = 1;

            Assert.IsTrue(game.CanAttack(state, targetId));
            game.StartBattle(state, targetId);
            for (int i = 0; i < 8; i++)
            {
                game.SubmitAnswer(state, 5, 1f);
            }

            var outcome = game.TickBattle(state, 0f);
            Assert.IsTrue(outcome.Captured);

            TerritoryModel target = null;
            foreach (var territory in state.Map.Territories)
            {
                if (territory.Id == targetId)
                {
                    target = territory;
                    break;
                }
            }

            Assert.NotNull(target);
            Assert.AreEqual(TerritoryOwner.Player, target.Owner);
            Assert.Greater(state.Gold, initialGold);
        }


        [Test]
        public void ComboResolver_AttackFireCritCreatesSignatureCombo()
        {
            var cards = new List<CardDefinition>
            {
                new CardDefinition("strike", "Удар", "", 5, 1, new[] { CardTag.Attack }),
                new CardDefinition("fire", "Огонь", "", 4, 1, new[] { CardTag.Fire }),
                new CardDefinition("crit", "Крит", "", 4, 1, new[] { CardTag.Crit })
            };

            var result = new ComboResolver().Resolve(cards);

            Assert.AreEqual("Критический огненный удар", result.Title);
            Assert.AreEqual(39, result.Damage);
            Assert.AreEqual(StatusEffect.Burning, result.StatusEffect);
        }

        [Test]
        public void PlayerDeck_RemoveWeakestKeepsThinDeckFloor()
        {
            var deck = new PlayerDeck(CardLibrary.CreateWarriorStarter(), 42);

            Assert.IsTrue(deck.RemoveWeakest());

            for (int i = 0; i < 10; i++)
            {
                deck.RemoveWeakest();
            }

            Assert.AreEqual(5, deck.Count);
        }

        [Test]
        public void InfiniteScaling_IncreasesHealthDamageAndArmorByFloorAndAct()
        {
            var baseStats = new EnemyStats { MaxHealth = 50, CurrentHealth = 50, Damage = 10, Armor = 1 };
            var service = new InfiniteScalingService();

            var early = service.Scale(baseStats, 1, 1);
            var late = service.Scale(baseStats, 10, 3);

            Assert.Greater(late.MaxHealth, early.MaxHealth);
            Assert.Greater(late.Damage, early.Damage);
            Assert.Greater(late.Armor, early.Armor);
        }

        private static StrategyGameService BuildGameService(IMathTaskGenerator generator = null)
        {
            return new StrategyGameService(
                new GridStrategyMapGenerator(),
                generator ?? new FixedMathTaskGenerator(3),
                new ComboBattleProgressionService(),
                new TerritoryRewardCalculator());
        }

        private sealed class FixedMathTaskGenerator : IMathTaskGenerator
        {
            private readonly int _answer;

            public FixedMathTaskGenerator(int answer)
            {
                _answer = answer;
            }

            public MathTask NextTask() => new MathTask("2 + 3", _answer);
        }
    }
}

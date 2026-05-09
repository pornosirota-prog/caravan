using System;
using System.Collections.Generic;
using CaravanRoguelite.Cards;

namespace CaravanRoguelite.Rewards
{
    public sealed class RewardGenerator
    {
        private readonly IReadOnlyList<CardDefinition> _cards;
        private readonly IReadOnlyList<ArtifactDefinition> _artifacts;
        private readonly Random _random;

        public RewardGenerator(IReadOnlyList<CardDefinition> cards, IReadOnlyList<ArtifactDefinition> artifacts, int seed = 0)
        {
            _cards = cards;
            _artifacts = artifacts;
            _random = seed == 0 ? new Random() : new Random(seed);
        }

        public List<RewardChoice> Generate(int floor, bool bossReward)
        {
            var rewards = new List<RewardChoice>();
            rewards.Add(CreateCardReward());
            rewards.Add(bossReward ? CreateArtifactReward() : CreateGoldReward(floor));
            rewards.Add(CreateUtilityReward());
            return rewards;
        }

        private RewardChoice CreateCardReward()
        {
            var card = _cards[_random.Next(_cards.Count)];
            return new RewardChoice
            {
                Type = RewardType.Card,
                Title = "+ карта: " + card.Title,
                Description = card.Description,
                Card = card
            };
        }

        private RewardChoice CreateGoldReward(int floor)
        {
            int gold = 8 + floor + _random.Next(0, 8);
            return new RewardChoice
            {
                Type = RewardType.Gold,
                Title = "+ золото",
                Description = $"Получить {gold} золота для магазина.",
                Gold = gold
            };
        }

        private RewardChoice CreateUtilityReward()
        {
            double roll = _random.NextDouble();
            if (roll < 0.34)
            {
                return CreateUpgradeReward();
            }

            if (roll < 0.67)
            {
                return CreateRemoveReward();
            }

            return CreateHealReward();
        }

        private RewardChoice CreateHealReward()
        {
            return new RewardChoice
            {
                Type = RewardType.Heal,
                Title = "Лечение",
                Description = "Восстановить 10 HP.",
                Heal = 10
            };
        }

        private RewardChoice CreateUpgradeReward()
        {
            return new RewardChoice
            {
                Type = RewardType.UpgradeCard,
                Title = "Улучшить карту",
                Description = "Случайная карта в колоде получает +2 силы."
            };
        }

        private RewardChoice CreateRemoveReward()
        {
            return new RewardChoice
            {
                Type = RewardType.RemoveCard,
                Title = "Удалить карту",
                Description = "Удалить слабейшую карту из колоды."
            };
        }

        private RewardChoice CreateArtifactReward()
        {
            var artifact = _artifacts[_random.Next(_artifacts.Count)];
            return new RewardChoice
            {
                Type = RewardType.Artifact,
                Title = "Артефакт: " + artifact.Title,
                Description = artifact.Description,
                Artifact = artifact
            };
        }

        public static List<ArtifactDefinition> CreateDefaultArtifacts()
        {
            return new List<ArtifactDefinition>
            {
                new("fire_heart", "Огненное сердце", "Каждый Fire-тег даёт +2 урона.", CardTag.Fire, 2),
                new("blood_blade", "Клинок крови", "Каждый Blood-тег даёт +2 урона.", CardTag.Blood, 2),
                new("mirror", "Зеркало повтора", "Каждый Repeat-тег даёт +2 урона.", CardTag.Repeat, 2),
                new("toxic_seal", "Ядовитая печать", "Каждый Poison-тег даёт +2 урона.", CardTag.Poison, 2),
                new("ice_crown", "Ледяная корона", "Каждый Ice-тег даёт +2 урона.", CardTag.Ice, 2),
                new("thunder_drum", "Громовой барабан", "Каждый Lightning-тег даёт +2 урона.", CardTag.Lightning, 2),
                new("martyr_shield", "Щит мученика", "Каждый Shield-тег даёт +1 урона даже защитным ходам.", CardTag.Shield, 1),
                new("gold_tooth", "Золотой зуб", "Награды золотом чаще помогают пережить магазин.")
            };
        }
    }
}

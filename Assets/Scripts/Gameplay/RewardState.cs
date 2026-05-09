using System.Collections.Generic;
using System.Linq;
using CaravanRoguelite.Core;
using CaravanRoguelite.Rewards;
using UnityEngine;

namespace CaravanRoguelite.Gameplay
{
    public sealed class RewardState : IGameState
    {
        private readonly GameContext _context;
        private readonly bool _bossReward;
        private List<RewardChoice> _rewards;

        public RewardState(GameContext context, bool bossReward)
        {
            _context = context;
            _bossReward = bossReward;
        }

        public void Enter()
        {
            _context.Hud.SetStrategyNavigationEnabled(false);
            var generator = new RewardGenerator(_context.CardLibrary, _context.ArtifactLibrary);
            _rewards = generator.Generate(_context.Run.Floor, _bossReward);
            ShowRewards();
        }

        public void Tick() { }

        public void Exit() { }

        private void ShowRewards()
        {
            var choices = _rewards.Select(reward => reward.Title).ToList();
            _context.Panel.Show(
                _bossReward ? "Награда за босса" : "Награда после боя",
                BuildBody(),
                choices,
                PickReward);
        }

        private string BuildBody()
        {
            string prefix = _bossReward
                ? $"Босс повержен. Начинается акт {_context.Run.Act}: враги сильнее, билды должны быть безумнее."
                : "Выберите одну награду из трёх. Тонкая колода часто сильнее большой.";
            return prefix + "\n\n" + string.Join("\n", _rewards.Select((reward, index) => $"{index + 1}. {reward.Title}: {reward.Description}"));
        }

        private void PickReward(int index)
        {
            var reward = _rewards[index];
            if (reward.Type == RewardType.Card && reward.Card != null)
            {
                _context.Deck.Add(reward.Card);
                _context.Hud.Log("В колоду добавлена карта: " + reward.Card.Title);
            }
            else if (reward.Type == RewardType.Artifact && reward.Artifact != null)
            {
                _context.Artifacts.Add(reward.Artifact);
                _context.Hud.Log("Получен артефакт: " + reward.Artifact.Title);
            }
            else if (reward.Type == RewardType.Gold)
            {
                _context.Stats.Gold += reward.Gold;
                _context.Hud.Log("Получено золото: " + reward.Gold);
            }
            else if (reward.Type == RewardType.Heal)
            {
                _context.Stats.Health = Mathf.Min(_context.Stats.MaxHealth, _context.Stats.Health + reward.Heal);
                _context.Hud.Log("Герой восстановил " + reward.Heal + " HP.");
            }
            else if (reward.Type == RewardType.UpgradeCard)
            {
                _context.Hud.Log(_context.Deck.UpgradeRandom() ? "Случайная карта улучшена." : "Некого улучшать.");
            }
            else if (reward.Type == RewardType.RemoveCard)
            {
                _context.Hud.Log(_context.Deck.RemoveWeakest() ? "Слабая карта удалена из колоды." : "Колода слишком мала для удаления.");
            }

            _context.Hud.Refresh(_context.Stats, _context.Day);
            if (_bossReward)
            {
                _context.Graph = new CaravanRoguelite.Generation.MapGenerator().Create(Random.Range(0, 999999));
                _context.CurrentNodeId = 0;
                _context.Hud.Log("Новый акт создан: маршрут начинается заново, бесконечный забег продолжается.");
            }

            _context.StateMachine.ChangeState(new TravelState(_context));
        }
    }
}

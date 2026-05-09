using System.Collections.Generic;
using CaravanRoguelite.Core;
using UnityEngine;

namespace CaravanRoguelite.Gameplay
{
    public class CityState : IGameState
    {
        private readonly GameContext _context;

        public CityState(GameContext context)
        {
            _context = context;
        }

        public void Enter()
        {
            _context.Hud.SetStrategyNavigationEnabled(false);
            _context.Sounds.PlayCity();
            _context.Panel.Show(
                "Магазин рун",
                BuildBody(),
                new List<string>
                {
                    "Купить случайную карту (10 золота)",
                    "Удалить слабую карту (14 золота)",
                    "Улучшить случайную карту (16 золота)",
                    "Лечение (8 золота -> +15 HP)",
                    "Уйти"
                },
                OnChoice);
        }

        public void Tick() { }

        public void Exit() { }

        private string BuildBody()
        {
            return $"Акт {_context.Run.Act}, этаж {_context.Run.Floor}.\n" +
                   $"Золото: {_context.Stats.Gold}. Карт в колоде: {_context.Deck.Count}.\n" +
                   "Главная услуга магазина — удаление мусорных карт: тонкая колода чаще собирает комбо.";
        }

        private void OnChoice(int idx)
        {
            if (idx == 0)
            {
                if (!TryPay(10)) return;
                var card = _context.CardLibrary[Random.Range(0, _context.CardLibrary.Count)];
                _context.Deck.Add(card);
                _context.Hud.Log("Куплена карта: " + card.Title);
                _context.Sounds.PlayOk();
            }
            else if (idx == 1)
            {
                if (!TryPay(14)) return;
                _context.Hud.Log(_context.Deck.RemoveWeakest() ? "Слабейшая карта удалена." : "Колода слишком мала: удаление отменено.");
                _context.Sounds.PlayOk();
            }
            else if (idx == 2)
            {
                if (!TryPay(16)) return;
                _context.Hud.Log(_context.Deck.UpgradeRandom() ? "Случайная карта улучшена." : "Некого улучшать.");
                _context.Sounds.PlayOk();
            }
            else if (idx == 3)
            {
                if (!TryPay(8)) return;
                _context.Stats.Health = Mathf.Clamp(_context.Stats.Health + 15, 0, _context.Stats.MaxHealth);
                _context.Hud.Log("Лекарь восстановил HP.");
                _context.Sounds.PlayOk();
            }
            else
            {
                _context.Hud.Log("Герой покидает магазин.");
                _context.Sounds.PlayClick();
            }

            _context.Hud.Refresh(_context.Stats, _context.Day);
            if (_context.Stats.IsDefeated)
            {
                _context.Defeat?.Invoke();
                return;
            }

            _context.StateMachine.ChangeState(new TravelState(_context));
        }

        private bool TryPay(int gold)
        {
            if (_context.Stats.Gold >= gold)
            {
                _context.Stats.Gold -= gold;
                return true;
            }

            _context.Hud.Log("Недостаточно золота.");
            _context.Sounds.PlayWarn();
            _context.Hud.Refresh(_context.Stats, _context.Day);
            Enter();
            return false;
        }
    }
}

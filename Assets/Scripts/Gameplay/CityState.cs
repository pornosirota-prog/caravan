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
                "Город",
                "Торговая застава предлагает несколько сделок.",
                new List<string>
                {
                    "Купить еду (5 золота -> +4 еды)",
                    "Лекарь (8 золота -> +6 HP)",
                    "Кузнец (12 золота -> +1 атака)",
                    "Уйти"
                },
                OnChoice);
        }

        public void Tick() { }

        public void Exit() { }

        private void OnChoice(int idx)
        {
            if (idx == 0 && _context.Stats.Gold >= 5)
            {
                _context.Stats.Gold -= 5;
                _context.Stats.Food += 4;
                _context.Hud.Log("Запасы пополнены.");
                _context.Sounds.PlayOk();
            }
            else if (idx == 1 && _context.Stats.Gold >= 8)
            {
                _context.Stats.Gold -= 8;
                _context.Stats.Health = Mathf.Clamp(_context.Stats.Health + 6, 0, _context.Stats.MaxHealth);
                _context.Hud.Log("Лекарь укрепил караван.");
                _context.Sounds.PlayOk();
            }
            else if (idx == 2 && _context.Stats.Gold >= 12)
            {
                _context.Stats.Gold -= 12;
                _context.Stats.Attack += 1;
                _context.Hud.Log("Оружие улучшено.");
                _context.Sounds.PlayOk();
            }
            else if (idx == 3)
            {
                _context.Hud.Log("Караван покидает город.");
                _context.Sounds.PlayClick();
            }
            else
            {
                _context.Hud.Log("Недостаточно золота.");
                _context.Sounds.PlayWarn();
                _context.Hud.Refresh(_context.Stats, _context.Day);
                Enter();
                return;
            }

            _context.Hud.Refresh(_context.Stats, _context.Day);
            if (_context.Stats.IsDefeated)
            {
                _context.Defeat?.Invoke();
                return;
            }
            _context.StateMachine.ChangeState(new TravelState(_context));
        }
    }
}

using System.Collections.Generic;
using CaravanRoguelite.Combat;
using CaravanRoguelite.Core;
using UnityEngine;

namespace CaravanRoguelite.Gameplay
{
    public class CombatState : IGameState
    {
        private readonly GameContext _context;
        private readonly bool _isBoss;
        private EnemyDefinition _enemy;
        private int _enemyHealth;
        private bool _guarding;

        public CombatState(GameContext context, bool isBoss)
        {
            _context = context;
            _isBoss = isBoss;
        }

        public void Enter()
        {
            _context.Hud.SetStrategyNavigationEnabled(false);
            _context.Sounds.PlayCombat();
            _enemy = _isBoss ? _context.Enemies[^1] : _context.Enemies[Random.Range(0, _context.Enemies.Count - 1)];
            _enemyHealth = _enemy.MaxHealth;
            ShowChoices($"Бой: {_enemy.Name} (HP {_enemyHealth})");
        }

        public void Tick() { }

        public void Exit() { }

        private void ShowChoices(string body)
        {
            _context.Panel.Show(
                _isBoss ? "Финальная битва" : "Стычка",
                body,
                new List<string> { "Атаковать", "Защититься", "Отступить (-мораль)" },
                OnChoice);
            _context.Hud.Refresh(_context.Stats, _context.Day);
        }

        private void OnChoice(int index)
        {
            if (index == 0)
            {
                _enemyHealth -= _context.Stats.Attack;
                _context.Hud.Log($"Вы нанесли {_context.Stats.Attack} урона.");
                _context.Sounds.PlayHit();
            }
            else if (index == 1)
            {
                _guarding = true;
                _context.Hud.Log("Караван готовится к удару.");
                _context.Sounds.PlayClick();
            }
            else
            {
                _context.Stats.Morale -= 2;
                _context.Hud.Log("Отступление удалось, но дух каравана упал.");
                _context.Sounds.PlayWarn();
                ResolveEnd(false);
                return;
            }

            if (_enemyHealth <= 0)
            {
                _context.Stats.Gold += _enemy.RewardGold;
                _context.Hud.Log($"Победа! Добыто {_enemy.RewardGold} золота.");
                _context.Sounds.PlayWin();
                if (_isBoss)
                {
                    _context.Victory?.Invoke();
                    return;
                }

                _context.StateMachine.ChangeState(new TravelState(_context));
                return;
            }

            int income = Mathf.Max(1, _enemy.Attack - _context.Stats.Armor - (_guarding ? 2 : 0));
            _guarding = false;
            _context.Stats.Health -= income;
            _context.Hud.Log($"{_enemy.Name} наносит {income} урона.");
            _context.Sounds.PlayHit();
            ResolveEnd(true);
        }

        private void ResolveEnd(bool continueFight)
        {
            _context.Hud.Refresh(_context.Stats, _context.Day);
            if (_context.Stats.IsDefeated)
            {
                _context.Defeat?.Invoke();
                return;
            }

            if (continueFight)
            {
                ShowChoices($"Бой: {_enemy.Name} (HP {_enemyHealth})");
            }
            else
            {
                _context.StateMachine.ChangeState(new TravelState(_context));
            }
        }
    }
}

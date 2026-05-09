using System.Collections.Generic;
using System.Linq;
using System.Text;
using CaravanRoguelite.Cards;
using CaravanRoguelite.Core;
using CaravanRoguelite.Enemies;
using CaravanRoguelite.Rewards;
using UnityEngine;

namespace CaravanRoguelite.Gameplay
{
    public class CombatState : IGameState
    {
        private const int HandSize = 5;
        private readonly GameContext _context;
        private readonly bool _isBoss;
        private readonly ComboResolver _comboResolver = new();
        private EnemyModel _enemy;
        private List<CardDefinition> _hand = new();
        private readonly List<int> _selectedIndexes = new();
        private int _lastBlock;
        private int _turn = 1;

        public CombatState(GameContext context, bool isBoss)
        {
            _context = context;
            _isBoss = isBoss;
        }

        public void Enter()
        {
            _context.Hud.SetStrategyNavigationEnabled(false);
            _context.Sounds.PlayCombat();
            _enemy = _context.EnemyGenerator.Generate(_context.Run.Floor, _context.Run.Act, _isBoss || _context.Run.IsBossFloor);
            DrawNewHand();
            ShowChoices("Выберите до " + GetSelectionLimit() + " карт и сыграйте комбо.");
        }

        public void Tick() { }

        public void Exit() { }

        private void DrawNewHand()
        {
            _context.Deck.DiscardHandRemainder();
            _hand = _context.Deck.Draw(HandSize).ToList();
            _selectedIndexes.Clear();
        }

        private void ShowChoices(string message)
        {
            var choices = new List<string>();
            for (int i = 0; i < _hand.Count; i++)
            {
                string marker = _selectedIndexes.Contains(i) ? "✓ " : string.Empty;
                choices.Add(marker + _hand[i].Title + " [" + string.Join(",", _hand[i].Tags) + "]");
            }

            choices.Add("Сыграть выбранное комбо");
            choices.Add("Сбросить руку");
            choices.Add("Отступить (-2 морали)");

            _context.Panel.Show(
                _enemy.IsBoss ? "Босс: " + _enemy.Name : "Бой: " + _enemy.Name,
                BuildBattleBody(message),
                choices,
                OnChoice);
            _context.Hud.Refresh(_context.Stats, _context.Day);
        }

        private string BuildBattleBody(string message)
        {
            var body = new StringBuilder();
            body.AppendLine($"Акт {_context.Run.Act}, этаж {_context.Run.Floor}, ход {_turn}");
            body.AppendLine($"Враг: {_enemy.Name} HP {_enemy.Stats.CurrentHealth}/{_enemy.Stats.MaxHealth}, урон {_enemy.Stats.Damage}, броня {_enemy.Stats.Armor}");
            if (_enemy.Traits.Count > 0)
            {
                body.AppendLine("Свойства: " + string.Join(", ", _enemy.Traits));
            }

            body.AppendLine($"Герой: HP {_context.Stats.Health}/{_context.Stats.MaxHealth}, золото {_context.Stats.Gold}, карт в колоде {_context.Deck.Count}");
            if (_context.Artifacts.Count > 0)
            {
                body.AppendLine("Артефакты: " + string.Join(", ", _context.Artifacts.Select(artifact => artifact.Title)));
            }

            body.AppendLine();
            body.AppendLine(message);
            return body.ToString();
        }

        private void OnChoice(int index)
        {
            if (index < _hand.Count)
            {
                ToggleCard(index);
                return;
            }

            int playIndex = _hand.Count;
            int discardIndex = _hand.Count + 1;
            if (index == playIndex)
            {
                PlaySelectedCards();
            }
            else if (index == discardIndex)
            {
                _context.Hud.Log("Рука сброшена ради поиска лучших синергий.");
                DrawNewHand();
                EnemyTurn();
            }
            else
            {
                _context.Stats.Morale -= 2;
                _context.Hud.Log("Отступление удалось, но мораль падает.");
                _context.Sounds.PlayWarn();
                _context.StateMachine.ChangeState(new TravelState(_context));
            }
        }

        private void ToggleCard(int index)
        {
            if (_selectedIndexes.Contains(index))
            {
                _selectedIndexes.Remove(index);
            }
            else if (_selectedIndexes.Count < GetSelectionLimit())
            {
                _selectedIndexes.Add(index);
            }
            else
            {
                _context.Hud.Log("Этот босс/ход не позволяет выбрать больше карт.");
                _context.Sounds.PlayWarn();
            }

            ShowChoices("Соберите связку тегов: Attack + Fire + Crit, Shield + Heal, Lightning + Repeat и т.д.");
        }

        private int GetSelectionLimit()
        {
            return _enemy != null && _enemy.Traits.Contains(EnemyTrait.LimitsCards) ? 3 : 4;
        }

        private void PlaySelectedCards()
        {
            if (_selectedIndexes.Count == 0)
            {
                ShowChoices("Сначала выберите хотя бы одну карту.");
                return;
            }

            var selectedCards = _selectedIndexes.OrderBy(i => i).Select(i => _hand[i]).ToList();
            var result = _comboResolver.Resolve(selectedCards);
            int damage = ApplyDamageModifiers(result, selectedCards);
            int actualDamage = Mathf.Max(0, damage - _enemy.Stats.Armor);
            _enemy.Stats.CurrentHealth -= actualDamage;
            _lastBlock = result.Block;
            if (result.Healing > 0)
            {
                int heal = result.Healing;
                if (_enemy.Traits.Contains(EnemyTrait.PunishesHealing))
                {
                    _enemy.Stats.CurrentHealth = Mathf.Min(_enemy.Stats.MaxHealth, _enemy.Stats.CurrentHealth + heal);
                    _context.Hud.Log($"{_enemy.Name} искажает лечение и восстанавливает {heal} HP.");
                }
                else
                {
                    _context.Stats.Health = Mathf.Min(_context.Stats.MaxHealth, _context.Stats.Health + heal);
                }
            }

            ApplyStatus(result.StatusEffect);
            _context.Deck.Discard(selectedCards);
            _context.Hud.Log($"Комбо: {result.Title}. Урон {actualDamage}, блок {_lastBlock}, лечение {result.Healing}.");
            _context.Sounds.PlayHit();

            if (_enemy.Stats.CurrentHealth <= 0)
            {
                WinCombat();
                return;
            }

            EnemyTurn();
        }

        private int ApplyDamageModifiers(ComboResult result, IReadOnlyList<CardDefinition> selectedCards)
        {
            int damage = result.Damage + _context.Stats.Attack;
            var tags = selectedCards.SelectMany(card => card.Tags).ToList();
            foreach (var artifact in _context.Artifacts)
            {
                if (artifact.AmplifiedTag.HasValue)
                {
                    damage += tags.Count(tag => tag == artifact.AmplifiedTag.Value) * artifact.BonusDamage;
                }
            }

            if (_enemy.Traits.Contains(EnemyTrait.FireResistant) && tags.Contains(CardTag.Fire))
            {
                damage /= 2;
            }

            if (_enemy.Traits.Contains(EnemyTrait.CriticalResistant) && tags.Contains(CardTag.Crit))
            {
                damage = Mathf.RoundToInt(damage * 0.65f);
            }

            return damage;
        }

        private void ApplyStatus(StatusEffect status)
        {
            if (status == StatusEffect.Burning)
            {
                _enemy.Stats.CurrentHealth -= 4;
            }
            else if (status == StatusEffect.Poisoned)
            {
                _enemy.Stats.CurrentHealth -= 3 + _context.Run.Act;
            }
            else if (status == StatusEffect.Stunned)
            {
                _lastBlock += _enemy.Stats.Damage;
            }
        }

        private void EnemyTurn()
        {
            if (_enemy.Traits.Contains(EnemyTrait.Regenerating))
            {
                _enemy.Stats.CurrentHealth = Mathf.Min(_enemy.Stats.MaxHealth, _enemy.Stats.CurrentHealth + 3 + _context.Run.Act);
            }

            int rawDamage = _enemy.Stats.Damage;
            if (_enemy.Traits.Contains(EnemyTrait.Berserker) && _enemy.Stats.CurrentHealth < _enemy.Stats.MaxHealth / 2)
            {
                rawDamage += 4 + _context.Run.Act;
            }

            int incoming = Mathf.Max(1, rawDamage - _context.Stats.Armor - _lastBlock);
            _context.Stats.Health -= incoming;
            _lastBlock = 0;
            _turn++;
            _context.Hud.Log($"{_enemy.Name} наносит {incoming} урона.");
            _context.Sounds.PlayHit();

            if (_context.Stats.IsDefeated)
            {
                _context.Defeat?.Invoke();
                return;
            }

            DrawNewHand();
            ShowChoices("Новый ход: ищите сломанную синергию.");
        }

        private void WinCombat()
        {
            int gold = _enemy.IsBoss ? 25 + _context.Run.Act * 5 : 8 + _context.Run.Floor;
            _context.Stats.Gold += gold;
            bool defeatedBoss = _enemy.IsBoss;
            _context.Run.AdvanceAfterCombat(defeatedBoss);
            _context.Hud.Log($"Победа! Получено {gold} золота.");
            _context.Sounds.PlayWin();
            _context.StateMachine.ChangeState(new RewardState(_context, defeatedBoss));
        }
    }
}

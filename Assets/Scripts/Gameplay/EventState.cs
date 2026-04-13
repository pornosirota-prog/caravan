using System.Collections.Generic;
using CaravanRoguelite.Core;
using UnityEngine;

namespace CaravanRoguelite.Gameplay
{
    public class EventState : IGameState
    {
        private readonly GameContext _context;

        public EventState(GameContext context)
        {
            _context = context;
        }

        public void Enter()
        {
            var ev = _context.Events[Random.Range(0, _context.Events.Count)];
            var labels = new List<string>();
            foreach (var choice in ev.Choices) labels.Add(choice.Text);

            _context.Panel.Show(ev.Title, ev.Body, labels, idx =>
            {
                var choice = ev.Choices[idx];
                _context.Stats.Gold += choice.Gold;
                _context.Stats.Food += choice.Food;
                _context.Stats.Morale += choice.Morale;
                _context.Stats.Health = Mathf.Clamp(_context.Stats.Health + choice.Health, 0, _context.Stats.MaxHealth);
                _context.Stats.Attack += choice.Attack;
                _context.Hud.Log(choice.Result);
                if (choice.Health < 0 || choice.Morale < 0)
                {
                    _context.Sounds.PlayWarn();
                }
                else
                {
                    _context.Sounds.PlayOk();
                }
                _context.Hud.Refresh(_context.Stats, _context.Day);

                if (_context.Stats.IsDefeated)
                {
                    _context.Defeat?.Invoke();
                    return;
                }

                _context.StateMachine.ChangeState(new TravelState(_context));
            });
        }

        public void Tick() { }

        public void Exit() { }
    }
}

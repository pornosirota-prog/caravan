using System.Collections.Generic;
using CaravanRoguelite.Core;
using CaravanRoguelite.Data;

namespace CaravanRoguelite.Gameplay
{
    public class TravelState : IGameState
    {
        private readonly GameContext _context;

        public TravelState(GameContext context)
        {
            _context = context;
        }

        public void Enter()
        {
            _context.Hud.Log("Ваш узел помечен \"ВЫ ЗДЕСЬ\". Нажимайте только на узлы со стрелкой ▲ ЖМИ.");
            _context.MapView.Render(_context.Graph, _context.CurrentNodeId, OnNodeClicked);
            _context.MapView.SetInteractable(GetAvailable());
            _context.Hud.Refresh(_context.Stats, _context.Day);
            _context.Panel.Hide();
        }

        public void Tick() { }

        public void Exit() { }

        private HashSet<int> GetAvailable()
        {
            var current = _context.Graph.Get(_context.CurrentNodeId);
            return new HashSet<int>(current.Links);
        }

        private void OnNodeClicked(int id)
        {
            var available = GetAvailable();
            if (!available.Contains(id))
            {
                return;
            }

            _context.CurrentNodeId = id;
            _context.Day++;
            _context.Stats.Food--;
            var node = _context.Graph.Get(id);
            node.Visited = true;

            if (_context.Stats.IsDefeated)
            {
                _context.Defeat?.Invoke();
                return;
            }

            if (node.Type == NodeType.Boss)
            {
                _context.StateMachine.ChangeState(new CombatState(_context, true));
                return;
            }

            if (node.Type == NodeType.Combat)
            {
                _context.StateMachine.ChangeState(new CombatState(_context, false));
                return;
            }

            if (node.Type == NodeType.City)
            {
                _context.StateMachine.ChangeState(new CityState(_context));
                return;
            }

            _context.StateMachine.ChangeState(new EventState(_context));
        }
    }
}

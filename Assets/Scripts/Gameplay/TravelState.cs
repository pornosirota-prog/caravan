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
            _context.Hud.Log("Ваш караван отмечен над текущим узлом. Кликните доступный узел, чтобы увидеть прогноз перед переходом.");
            _context.MapView.Render(_context.Graph, _context.CurrentNodeId, OnNodeClicked);
            _context.MapView.SetInteractable(GetAvailable());
            _context.Hud.Refresh(_context.Stats, _context.Day);
            _context.Panel.Hide();
        }

        public void Tick() { }

        public void Exit() { }

        private HashSet<int> GetAvailable()
        {
            var available = new HashSet<int>();
            foreach (var node in _context.Graph.Nodes)
            {
                if (node.Id != _context.CurrentNodeId)
                {
                    available.Add(node.Id);
                }
            }

            return available;
        }

        private void OnNodeClicked(int id)
        {
            var available = GetAvailable();
            if (!available.Contains(id))
            {
                _context.Hud.Log("Этот узел недоступен.");
                _context.Sounds.PlayWarn();
                return;
            }

            var node = _context.Graph.Get(id);
            _context.Panel.Show(
                $"Переход: {NodeName(node.Type)}",
                BuildTravelForecast(node),
                new List<string> { "Поехать", "Отмена" },
                choice =>
                {
                    _context.Panel.Hide();
                    if (choice == 0)
                    {
                        ConfirmTravel(id);
                    }
                    else
                    {
                        _context.Hud.Log("Переход отменён. Выберите другой маршрут.");
                        _context.Sounds.PlayWarn();
                    }
                });
        }

        private void ConfirmTravel(int id)
        {
            _context.CurrentNodeId = id;
            _context.Sounds.PlayTravel();
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

        private string BuildTravelForecast(MapNode node)
        {
            string result = node.Type switch
            {
                NodeType.Combat => "Ожидается бой. Можно получить золото, но есть риск потери HP и морали.",
                NodeType.City => "Город: можно восстановиться и подготовиться к следующим дням.",
                NodeType.Boss => "Опаснейшая битва с боссом. Подготовьтесь по еде, HP и атаке.",
                NodeType.Event => "Случайное событие: выбор с наградой или риском.",
                _ => "Дорога без гарантии, но с шансом на полезную находку."
            };

            return $"{result}\n\nЦена перехода: -1 еда, +1 день.";
        }

        private string NodeName(NodeType type)
        {
            return type switch
            {
                NodeType.Combat => "Бой",
                NodeType.City => "Город",
                NodeType.Boss => "Босс",
                NodeType.Event => "Событие",
                _ => "Маршрут"
            };
        }
    }
}

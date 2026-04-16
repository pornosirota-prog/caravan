using CaravanRoguelite.Core;

namespace CaravanRoguelite.Gameplay
{
    public sealed class StrategyState : IGameState
    {
        private readonly GameContext _context;

        public StrategyState(GameContext context)
        {
            _context = context;
        }

        public void Enter()
        {
            _context.MapView.SetVisible(false);
            _context.Panel.Hide();
            _context.Hud.SetStrategyNavigationEnabled(false);
            _context.Hud.SetVisible(false);
            _context.StrategyScreen.SetVisible(true);
            _context.Hud.Log("Стратегический режим активен. Захватывайте соседние территории.");
        }

        public void Tick() { }

        public void Exit()
        {
            _context.StrategyScreen.SetVisible(false);
            _context.MapView.SetVisible(true);
            _context.Hud.SetVisible(true);
        }
    }
}

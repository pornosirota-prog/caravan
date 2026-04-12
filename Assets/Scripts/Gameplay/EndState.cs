using System.Collections.Generic;
using CaravanRoguelite.Core;

namespace CaravanRoguelite.Gameplay
{
    public class EndState : IGameState
    {
        private readonly GameContext _context;
        private readonly bool _win;

        public EndState(GameContext context, bool win)
        {
            _context = context;
            _win = win;
        }

        public void Enter()
        {
            string title = _win ? "Победа" : "Поражение";
            string body = _win
                ? "Вождь пустоши повержен. Караван нашёл безопасный путь."
                : "Караван распался. Попробуйте новый маршрут и распределение ресурсов.";

            _context.Panel.Show(title, body, new List<string> { "Начать заново" }, _ =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            });
            _context.Hud.Log(_win ? "Кампания завершена успешно." : "Экспедиция потеряна.");
        }

        public void Tick() { }

        public void Exit() { }
    }
}

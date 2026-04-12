using UnityEngine;

namespace CaravanRoguelite.Core
{
    public class GameStateMachine
    {
        private IGameState _current;

        public void ChangeState(IGameState next)
        {
            _current?.Exit();
            _current = next;
            _current?.Enter();
        }

        public void Tick()
        {
            _current?.Tick();
        }
    }
}

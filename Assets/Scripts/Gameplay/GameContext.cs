using System;
using System.Collections.Generic;
using CaravanRoguelite.Combat;
using CaravanRoguelite.Core;
using CaravanRoguelite.Data;
using CaravanRoguelite.Events;
using CaravanRoguelite.Map;
using CaravanRoguelite.UI;

namespace CaravanRoguelite.Gameplay
{
    public class GameContext
    {
        public readonly GameStateMachine StateMachine = new();
        public readonly CaravanStats Stats = new();
        public readonly List<EventDefinition> Events = EventLibrary.CreateDefault();
        public readonly List<EnemyDefinition> Enemies = EnemyLibrary.CreateDefault();

        public MapGraph Graph;
        public MapView MapView;
        public GameHud Hud;
        public ChoicePanel Panel;

        public int Day = 1;
        public int CurrentNodeId;

        public Action Victory;
        public Action Defeat;
    }
}

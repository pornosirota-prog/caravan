using System;
using System.Collections.Generic;
using CaravanRoguelite.Combat;
using CaravanRoguelite.Cards;
using CaravanRoguelite.Enemies;
using CaravanRoguelite.Rewards;
using CaravanRoguelite.RPG;
using CaravanRoguelite.Run;
using CaravanRoguelite.Core;
using CaravanRoguelite.Data;
using CaravanRoguelite.Events;
using CaravanRoguelite.Map;
using CaravanRoguelite.UI;
using CaravanRoguelite.Strategy.UI;

namespace CaravanRoguelite.Gameplay
{
    public class GameContext
    {
        public readonly GameStateMachine StateMachine = new();
        public readonly CaravanStats Stats = new();
        public readonly List<EventDefinition> Events = EventLibrary.CreateDefault();
        public readonly List<EnemyDefinition> Enemies = EnemyLibrary.CreateDefault();
        public readonly List<CardDefinition> CardLibrary = CaravanRoguelite.Cards.CardLibrary.CreateDefault();
        public readonly List<ArtifactDefinition> ArtifactLibrary = RewardGenerator.CreateDefaultArtifacts();
        public readonly List<ArtifactDefinition> Artifacts = new();
        public readonly RunState Run = new();
        public readonly HeroModel Hero = new();
        public readonly EnemyGenerator EnemyGenerator = new();
        public PlayerDeck Deck;

        public MapGraph Graph;
        public MapView MapView;
        public GameHud Hud;
        public ChoicePanel Panel;
        public UiSoundPlayer Sounds;
        public StrategyScreen StrategyScreen;

        public int Day = 1;
        public int CurrentNodeId;

        public Action Victory;
        public Action Defeat;
    }
}

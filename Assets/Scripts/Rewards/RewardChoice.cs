using CaravanRoguelite.Cards;

namespace CaravanRoguelite.Rewards
{
    public sealed class RewardChoice
    {
        public RewardType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Gold { get; set; }
        public int Heal { get; set; }
        public CardDefinition Card { get; set; }
        public ArtifactDefinition Artifact { get; set; }
    }
}

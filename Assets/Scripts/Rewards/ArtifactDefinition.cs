using CaravanRoguelite.Cards;

namespace CaravanRoguelite.Rewards
{
    public sealed class ArtifactDefinition
    {
        public ArtifactDefinition(string id, string title, string description, CardTag? amplifiedTag = null, int bonusDamage = 0)
        {
            Id = id;
            Title = title;
            Description = description;
            AmplifiedTag = amplifiedTag;
            BonusDamage = bonusDamage;
        }

        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public CardTag? AmplifiedTag { get; }
        public int BonusDamage { get; }
    }
}

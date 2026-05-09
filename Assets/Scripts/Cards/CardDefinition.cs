using System.Collections.Generic;
using System.Linq;

namespace CaravanRoguelite.Cards
{
    public sealed class CardDefinition
    {
        public CardDefinition(string id, string title, string description, int basePower, int energyCost, IEnumerable<CardTag> tags, int level = 1)
        {
            Id = id;
            Title = title;
            Description = description;
            BasePower = basePower;
            EnergyCost = energyCost;
            Tags = tags.ToList();
            Level = level;
        }

        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public int BasePower { get; }
        public int EnergyCost { get; }
        public IReadOnlyList<CardTag> Tags { get; }
        public int Level { get; }

        public CardDefinition Upgrade()
        {
            return new CardDefinition(Id, Title + "+", Description, BasePower + 2, EnergyCost, Tags, Level + 1);
        }

        public override string ToString()
        {
            return Level > 1 ? $"{Title} ({BasePower})" : Title;
        }
    }
}

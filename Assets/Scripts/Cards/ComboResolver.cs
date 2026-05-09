using System.Collections.Generic;
using System.Linq;

namespace CaravanRoguelite.Cards
{
    public sealed class ComboResolver
    {
        public ComboResult Resolve(IReadOnlyList<CardDefinition> selectedCards)
        {
            if (selectedCards == null || selectedCards.Count == 0)
            {
                return new ComboResult("Пустой ход", 0, 0, 0, StatusEffect.None);
            }

            var tags = selectedCards.SelectMany(card => card.Tags).ToList();
            int basePower = selectedCards.Sum(card => card.BasePower);
            int fireCount = tags.Count(tag => tag == CardTag.Fire);
            int lightningCount = tags.Count(tag => tag == CardTag.Lightning);

            if (Has(tags, CardTag.Attack, CardTag.Fire, CardTag.Crit))
            {
                return new ComboResult("Критический огненный удар", basePower * 3, 0, 0, StatusEffect.Burning);
            }

            if (Has(tags, CardTag.Attack, CardTag.Fire) && fireCount >= 2)
            {
                return new ComboResult("Большой огненный удар", basePower * 2, 0, 0, StatusEffect.Burning);
            }

            if (Has(tags, CardTag.Lightning, CardTag.Repeat) || lightningCount >= 3)
            {
                return new ComboResult("Цепная молния", basePower * 2, 0, 0, StatusEffect.Stunned);
            }

            if (Has(tags, CardTag.Attack, CardTag.Poison))
            {
                return new ComboResult("Ядовитый удар", basePower, 0, 0, StatusEffect.Poisoned);
            }

            if (Has(tags, CardTag.Shield, CardTag.Heal))
            {
                return new ComboResult("Регенерирующая защита", 0, basePower, basePower / 2, StatusEffect.None);
            }

            if (Has(tags, CardTag.Attack, CardTag.Blood))
            {
                return new ComboResult("Вампирский удар", basePower, 0, basePower / 3, StatusEffect.Bleeding);
            }

            if (Has(tags, CardTag.Attack, CardTag.Crit))
            {
                return new ComboResult("Точный удар", basePower * 2, 0, 0, StatusEffect.None);
            }

            if (Has(tags, CardTag.Shield))
            {
                return new ComboResult("Блок", 0, basePower, 0, StatusEffect.None);
            }

            if (Has(tags, CardTag.Heal))
            {
                return new ComboResult("Лечение", 0, 0, basePower, StatusEffect.None);
            }

            if (Has(tags, CardTag.Attack))
            {
                return new ComboResult("Базовая атака", basePower, 0, 0, StatusEffect.None);
            }

            return new ComboResult("Слабое действие", basePower / 2, 0, 0, StatusEffect.None);
        }

        private static bool Has(List<CardTag> tags, params CardTag[] requiredTags)
        {
            return requiredTags.All(tags.Contains);
        }
    }
}

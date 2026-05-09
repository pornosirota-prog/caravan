using System.Collections.Generic;

namespace CaravanRoguelite.Cards
{
    public static class CardLibrary
    {
        public static List<CardDefinition> CreateDefault()
        {
            return new List<CardDefinition>
            {
                new("strike", "Удар", "Физическая основа почти любого атакующего комбо.", 5, 1, new[] { CardTag.Attack }),
                new("heavy_strike", "Сильный удар", "Медленная, но мощная атака.", 8, 2, new[] { CardTag.Attack }),
                new("fire", "Огонь", "Добавляет горение к атакующим связкам.", 4, 1, new[] { CardTag.Fire }),
                new("ice", "Лёд", "Контроль и подготовка заморозки.", 4, 1, new[] { CardTag.Ice }),
                new("poison", "Яд", "Ставит отравляющие синергии.", 3, 1, new[] { CardTag.Poison }),
                new("shield", "Щит", "Даёт блок и защитные связки.", 6, 1, new[] { CardTag.Shield }),
                new("heal", "Лечение", "Восстанавливает здоровье или усиливает защиту.", 5, 1, new[] { CardTag.Heal }),
                new("crit", "Крит", "Удваивает атакующие комбо и открывает огненный крит.", 4, 1, new[] { CardTag.Crit }),
                new("repeat", "Повтор", "Повторяет молнию и усиливает цепочки.", 3, 1, new[] { CardTag.Repeat }),
                new("blood", "Кровь", "Превращает атаку в вампиризм.", 4, 1, new[] { CardTag.Blood }),
                new("lightning", "Молния", "Бьёт цепями при повторе или тройной молнии.", 5, 1, new[] { CardTag.Lightning }),
                new("curse", "Проклятие", "Рискованная руна для будущих артефактов.", 6, 0, new[] { CardTag.Curse })
            };
        }

        public static List<CardDefinition> CreateWarriorStarter()
        {
            return new List<CardDefinition>
            {
                new("strike", "Удар", "Физическая основа почти любого атакующего комбо.", 5, 1, new[] { CardTag.Attack }),
                new("strike", "Удар", "Физическая основа почти любого атакующего комбо.", 5, 1, new[] { CardTag.Attack }),
                new("heavy_strike", "Сильный удар", "Медленная, но мощная атака.", 8, 2, new[] { CardTag.Attack }),
                new("shield", "Щит", "Даёт блок и защитные связки.", 6, 1, new[] { CardTag.Shield }),
                new("shield", "Щит", "Даёт блок и защитные связки.", 6, 1, new[] { CardTag.Shield }),
                new("heal", "Лечение", "Восстанавливает здоровье или усиливает защиту.", 5, 1, new[] { CardTag.Heal }),
                new("fire", "Огонь", "Добавляет горение к атакующим связкам.", 4, 1, new[] { CardTag.Fire }),
                new("crit", "Крит", "Удваивает атакующие комбо и открывает огненный крит.", 4, 1, new[] { CardTag.Crit }),
                new("poison", "Яд", "Ставит отравляющие синергии.", 3, 1, new[] { CardTag.Poison }),
                new("lightning", "Молния", "Бьёт цепями при повторе или тройной молнии.", 5, 1, new[] { CardTag.Lightning })
            };
        }
    }
}

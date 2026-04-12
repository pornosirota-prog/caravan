using System.Collections.Generic;

namespace CaravanRoguelite.Events
{
    public static class EventLibrary
    {
        public static List<EventDefinition> CreateDefault()
        {
            return new List<EventDefinition>
            {
                new()
                {
                    Title = "Старый колодец",
                    Body = "Вы находите полуразрушенный колодец в пустоши.",
                    Choices =
                    {
                        new EventChoice { Text = "Рискнуть и спуститься", Food = +3, Health = -2, Result = "Вы подняли припасы, но поранились." },
                        new EventChoice { Text = "Разобрать механизм", Gold = +8, Morale = -1, Result = "Детали проданы, но люди устали." }
                    }
                },
                new()
                {
                    Title = "Беглецы",
                    Body = "Группа беженцев просит поделиться едой.",
                    Choices =
                    {
                        new EventChoice { Text = "Поделиться", Food = -3, Morale = +2, Result = "Караван вдохновлён вашим поступком." },
                        new EventChoice { Text = "Отказать", Morale = -2, Gold = +5, Result = "Вы сохранили ресурсы, но люди недовольны." }
                    }
                },
                new()
                {
                    Title = "Заброшенная мастерская",
                    Body = "Среди руин вы находите работающие станки.",
                    Choices =
                    {
                        new EventChoice { Text = "Усилить оружие", Gold = -10, Attack = +1, Result = "Кузнец поднял боевую мощь." },
                        new EventChoice { Text = "Собрать металлолом", Gold = +12, Result = "Лом продан на следующем привале." }
                    }
                }
            };
        }
    }
}

using System.Collections.Generic;

namespace CaravanRoguelite.Events
{
    public class EventDefinition
    {
        public string Title;
        public string Body;
        public readonly List<EventChoice> Choices = new();
    }
}

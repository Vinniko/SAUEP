using System;
using Prism.Events;

namespace SAUEP.WPF.Events
{
    public sealed class DataUpdateEvent : PubSubEvent<DateTime>
    {
    }
}

using Prism.Events;
using System;

namespace SAUEP.WPF.Events
{
    public sealed class ExceptionEvent : PubSubEvent<Exception>
    {
    }
}

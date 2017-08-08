using ProtoBuf;

namespace RiemannLoggerCore.Models
{
    [ProtoContract]
    public class EventAttribute
    {
        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public string Value { get; set; }

    }
}
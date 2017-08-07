using ProtoBuf;

namespace RiemannLoggerCore.Models
{
    [ProtoContract]
    public class Message
    {

        [ProtoMember(2)]
        public bool? Ok { get; set; }

        [ProtoMember(3)]
        public string Error { get; set; }

        //[ProtoMember(4)]
        //public StateEntry[] States { get; set; }

        //[ProtoMember(5)]
        //public Query Query { get; set; }

        [ProtoMember(6)]
        public Event[] Events { get; set; }

    }
}
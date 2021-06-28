using System;

namespace ServiceBusManager.Lib.Messages
{
    [Serializable]
    public class EventMessage
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public string Data { get; set; }
        public int TenantId { get; set; }
    }
}

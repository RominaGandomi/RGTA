using System;

namespace ServiceBusManager.Lib.Messages
{
    public interface IEventMessage : IConvertible
    {
        int Id { get; set; }
        int Value { get; set; }
        string Data { get; set; }
        int TenantId { get; set; }
    }
}

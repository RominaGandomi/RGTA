using System;

namespace ServiceBusManager.Lib.Messages
{
    public interface IEventCompletedMessage : IConvertible
    {
        int Id { get; set; }
        int Value { get; set; }
        string Data { get; set; }
        int TenantId { get; set; }
    }
}

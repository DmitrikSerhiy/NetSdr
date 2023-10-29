namespace NetSdr.Client.Message;

using Interfaces;
using Interfaces.Message;
using Item;

/// <summary>
/// Net sdr nak message
/// </summary>
/// <param name="Item"></param>
/// <param name="Header"></param>
public sealed record NAKMessage(Header Header) : IMessage {
    public IMessageItem Item { get; init; } = new EmptyItem();
}
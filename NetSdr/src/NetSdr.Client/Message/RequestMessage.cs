namespace NetSdr.Client.Message;

using Interfaces;
using Interfaces.Message;

/// <summary>
/// Net sdr response message
/// </summary>
/// <param name="Item"></param>
/// <param name="Header"></param>
public sealed record RequestMessage(IMessageItem Item, Header Header) : IMessage;
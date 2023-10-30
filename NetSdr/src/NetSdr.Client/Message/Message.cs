namespace NetSdr.Client.Message;

using Interfaces;
using Interfaces.Message;

/// <summary>
/// Net sdr response message
/// </summary>
/// <param name="Item"></param>
/// <param name="Header"></param>
public sealed record Message(IMessageItem Item, Header? Header) : IMessage;
namespace NetSdr.Client.Message.Item;

using NetSdr.Client.Interfaces.Message;

/// <summary>
/// Data item for the net sdr message
/// </summary>
/// <param name="Type"></param>
/// <param name="Data">size is limited by the general message size</param>
public sealed record DataItem(DataItemType Type, byte[] Data) : IMessageItem;
namespace NetSdr.Client.Message.Item;

using NetSdr.Client.Interfaces.Message;

/// <summary>
/// Control item contains a dictionary of key value pairs of control data
/// </summary>
/// <param name="Code">up to 16 bit</param>
/// <param name="Parameters"></param>
public sealed record ControlItem(ControlItemCode Code, byte[] Parameters) : IMessageItem;
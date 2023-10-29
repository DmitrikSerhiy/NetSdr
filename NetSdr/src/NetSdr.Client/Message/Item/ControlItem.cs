namespace NetSdr.Client.Message.Item;

using NetSdr.Client.Interfaces.Message;

/// <summary>
/// Control item contains a dictionary of key value pairs of control data
/// </summary>
/// <param name="Data">up to 16 bit</param>
public sealed record ControlItem(IDictionary<string, string> Data) : IMessageItem;
namespace NetSdr.Client.Interfaces;

using Client.Message;
using Message;

/// <summary>
/// General net sdr message contract. Message length range: 0 - 8191
/// </summary>
public interface IMessage { 
    IMessageItem Item { get; init; }
    Header Header { get; init; }
}
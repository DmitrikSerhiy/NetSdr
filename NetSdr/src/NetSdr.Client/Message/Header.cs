namespace NetSdr.Client.Message;

/// <summary>
/// Header of a net sdr message
/// </summary>
/// <param name="MessageType">up to 3 bits</param>
public sealed record Header(MessageType MessageType) {
    public int BlockLength { get; init; }
}
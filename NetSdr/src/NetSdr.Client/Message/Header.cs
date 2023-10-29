namespace NetSdr.Client.Message; 

/// <summary>
/// Header of a net sdr message
/// </summary>
/// <param name="MessageType">up to 3 bits</param>
/// <param name="BlockLength"></param>
public sealed record Header(MessageType MessageType, int BlockLength);
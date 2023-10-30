namespace NetSdr.Client.Interfaces;

using Helpers;


public interface IClient : IDisposable, IAsyncDisposable {
    ConnectionState ConnectionState { get; }
    string Frequency { get; }
    Task ConnectAsync(ITarget target, int port = 50000);
    Task ReceiverOnAsync(CaptureMode captureMode, byte dataChannelTypeSpecifier = 128, byte? fifoSampleCount = null);
    Task ReceiverOffAsync();
    void SetFrequency(int frequency);
    void Disconnect();
}
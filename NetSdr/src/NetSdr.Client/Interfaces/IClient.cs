namespace NetSdr.Client.Interfaces;

using Helpers;


public interface IClient : IDisposable {
    ConnectionState ConnectionState { get; }
    string Frequency { get; }
    void Connect(ITarget target, int port = 50000);
    void ReceiverOn(CaptureMode captureMode, byte dataChannelTypeSpecifier = 0x80, byte? fifoSampleCount = null);
    void ReceiverOff();
    void SetFrequency(int frequency);
    void Disconnect();
}
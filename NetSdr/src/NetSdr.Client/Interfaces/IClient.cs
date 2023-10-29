namespace NetSdr.Client.Interfaces; 

public interface IClient : IDisposable {
    ConnectionState ConnectionState { get; }
    string Frequency { get; }
    void Connect(ITarget target, int port = 50000);
    void ReceiverOn();
    void ReceiverOff();
    void SetFrequency(int frequency);
    void Disconnect();
}
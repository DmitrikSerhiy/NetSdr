namespace NetSdr.Client.Interfaces; 

public interface IClient {
    ConnectionState ConnectionState { get; }
    string Frequency { get; }
    void Connect(ITarget target, int port = 50000);
    void Disconnect();
    void ReceiverOn(int port = 60000);
    void SetFrequency(int frequency);
}
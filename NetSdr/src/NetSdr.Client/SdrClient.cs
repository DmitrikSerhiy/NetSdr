namespace NetSdr.Client;

using Helpers;
using Interfaces;
using Message;
using Message.Item;
using System.Net.Sockets;


internal sealed class SdrClient : IClient {

    public ConnectionState ConnectionState { get; private set; } = ConnectionState.Undefined;
    public string Frequency { get; } = "0";

    private TcpClient? _client;
    
    public void Connect(ITarget target, int port = 50000) {
        if (ConnectionState != ConnectionState.Undefined || _client != null) {
            return;
        }
        
        _client = new TcpClient();
        ConnectionState = ConnectionState.Pending;
        
        _client.Connect(target.Address, port);
        if (_client.Connected) {
            ConnectionState = ConnectionState.Connected;
        }
    }
    
    public void Disconnect() {
        _client?.Close();
        ConnectionState = ConnectionState.Disconnected;
    }
    
    public void ReceiverOn() {
        if (ConnectionState != ConnectionState.Connected) {
            return;
        }

        var message = new RequestMessage(
            new ControlItem(new Dictionary<string, string>()),
            new Header(MessageType.HostSetControlItem, 0)
        );
        
        using var networkStream = _client!.GetStream();
        var writter = new BinaryWriter(networkStream);
        writter.Write(message.GetBytes());
        writter.Flush();
    }
    public void ReceiverOff() {
        if (ConnectionState != ConnectionState.Connected) {
            return;
        }
        
        throw new NotImplementedException();
    }

    public void SetFrequency(int frequency) {
        if (ConnectionState != ConnectionState.Connected) {
            return;
        }
        
        throw new NotImplementedException();
    }
    
    public void Dispose() {
        _client?.Dispose();
    }
}
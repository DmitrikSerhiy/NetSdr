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

    /// <summary>
    /// Start receiver.
    /// MessageType: HostSetControlItem
    /// ControlItemCode: ReceiverState
    /// Parameters:
    ///     Channel/type specifier: 0x80
    ///     Run/stop control: 0x02 (run)
    ///     Capture mode defined by CaptureMode enum: 0x00 (default)
    ///     FIFO samples (default): 0x00
    /// </summary>
    /// <param name="dataChannelTypeSpecifier"></param>
    /// <param name="fifoSampleCount">Number of 4096 16-bit data samples to capture in FIFO mode</param>
    /// <param name="captureMode"></param>
    public void ReceiverOn(CaptureMode captureMode, byte dataChannelTypeSpecifier = 0x80, byte? fifoSampleCount = null) {
        if (ConnectionState != ConnectionState.Connected) {
            return;
        }

        var parameters = new List<byte>
        {
            dataChannelTypeSpecifier, 0x02, (byte)captureMode,
        };
        
        if (captureMode == CaptureMode.Fifo16Bit && fifoSampleCount != null) {
            parameters.Add(fifoSampleCount.Value);
        }
        else {
            parameters.Add(0x00);
        }
        
        var message = new RequestMessage(
            new ControlItem(ControlItemCode.ReceiverState, parameters.ToArray()),
            new Header(MessageType.HostSetControlItem));

        SendMessage(message);
    }

    /// <summary>
    /// Stop receiver.
    /// MessageType: HostSetControlItem
    /// ControlItemCode: ReceiverState
    /// Parameters:
    ///     Channel/type specifier (ignored, used default: 0x00)
    ///     Run/stop control: 0x01 (stop)
    ///     FIFO samples (default): 0x00
    ///     Capture mode (ignored, used default: 0x00)
    /// </summary>
    public void ReceiverOff() {
        if (ConnectionState != ConnectionState.Connected) {
            return;
        }
        
        var message = new RequestMessage(
            new ControlItem(ControlItemCode.ReceiverState, new byte[] { 0x00, 0x01, 0x00, 0x00 }),
            new Header(MessageType.HostSetControlItem));

        SendMessage(message);
    }

    public void SetFrequency(int frequency) {
        if (ConnectionState != ConnectionState.Connected) {
            return;
        }
        
        throw new NotImplementedException();
    }
    
    
    private void SendMessage(IMessage message) {
        using var networkStream = _client!.GetStream();
        var writer = new BinaryWriter(networkStream);
        writer.Write(message.GetBytes()); 
        writer.Flush();
    }
    
    public void Dispose() {
        _client?.Dispose();
    }
}
namespace NetSdr.Client;

using Helpers;
using Interfaces;
using Message;
using Message.Item;
using System.Net.Sockets;

/// <summary>
/// Implementation of the NetSDR protocol
/// </summary>
public sealed class SdrClient : IClient {
    public ConnectionState ConnectionState { get; private set; } = ConnectionState.Undefined;

    private readonly TcpClientAdapter _client;
    
    private UdpClient? _udpClient;
    private ushort _sequenceNumber = 0;

    private readonly IHost _host;

    private Stream? _networkStream;
    // To detect redundant Dispose calls
    private bool _disposed;

    public SdrClient(TcpClientAdapter client, IHost host) {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _host = host ?? throw new ArgumentNullException(nameof(host));
    }
    
    public async Task ConnectAsync(ITarget target, int port = 50000) {
        if (ConnectionState != ConnectionState.Undefined) {
            return;
        }
        
        ConnectionState = ConnectionState.Pending;
        await _client.ConnectAsync(target.Address, port);

        ConnectionState = _client.Connected 
            ? ConnectionState.Connected 
            : ConnectionState.Undefined;
    }
    
    public void Disconnect() {
        _client.Close();
        _udpClient?.Close();
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
    /// <param name="captureMode"></param>
    /// <param name="dataChannelTypeSpecifier"></param>
    /// <param name="fifoSampleCount">Number of 4096 16-bit data samples to capture in FIFO mode</param>
    public async Task ReceiverOnAsync(CaptureMode captureMode, byte dataChannelTypeSpecifier = 0x80, byte? fifoSampleCount = null) {
        if (ConnectionState != ConnectionState.Connected) {
            return;
        }

        var parameters = new List<byte> {
            dataChannelTypeSpecifier, 0x02, (byte)captureMode,
        };
        
        if (captureMode == CaptureMode.Fifo16Bit && fifoSampleCount != null) {
            parameters.Add(fifoSampleCount.Value);
        }
        else {
            parameters.Add(0x00);
        }
        
        var message = new Message.Message(
            new ControlItem(ControlItemCode.ReceiverState, parameters.ToArray()),
            new Header(MessageType.HostSetControlItem));
        
        await SendMessageAsync(message);

        // Part two: send UDP message back to the host
        // uncomment if real data and real host is available
        // OpenUdpSocketAsync();
        // await SendDataToHostAsync(GetHelloWorldMessage(_sequenceNumber)); // don't know what to send. It's not clear neither from documentation nor from the task description 
        // _sequenceNumber = (ushort)((_sequenceNumber + 1) & 0xFFFF);
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
    public async Task ReceiverOffAsync() {
        if (ConnectionState != ConnectionState.Connected) {
            return;
        }
        
        var message = new Message.Message(
            new ControlItem(ControlItemCode.ReceiverState, new byte[] { 0x00, 0x01, 0x00, 0x00 }),
            new Header(MessageType.HostSetControlItem));

        await SendMessageAsync(message);
        _udpClient?.Close();
    }

    /// <summary>
    /// Set frequency for specified (or all) channel.
    /// </summary>
    /// <param name="frequencyInHz">Contains 5 bytes which represents the real frequency</param>
    /// <param name="channelId">Channel to set or set all the same frequency(0xFF - default value)</param>
    public async Task SetFrequencyAsync(ulong frequencyInHz, byte channelId = 0xFF) {
        if (ConnectionState != ConnectionState.Connected) {
            return;
        }

        // Convert frequency to 5-byte array
        var frequencyBytes = new byte[5];
        for (var i = 0; i < 5; i++) {
            frequencyBytes[i] = (byte)(frequencyInHz & 0xFF);
            frequencyInHz >>= 8;
        }

        var parameters = new List<byte>
        {
            channelId
        };
        parameters.AddRange(frequencyBytes);

        var message = new Message.Message(
            new ControlItem(ControlItemCode.ReceiverFrequency, parameters.ToArray()),
            new Header(MessageType.HostSetControlItem));

        await SendMessageAsync(message);
    }
    
    private async Task SendMessageAsync(IMessage message) {
        _networkStream ??= _client.GetStream();
        if (_networkStream is null) {
            return;
        }
        await _networkStream.WriteAsync( (Memory<byte>)message.GetBytesForControlItemMessage());
        await _networkStream.FlushAsync();
    }
    
    private void OpenUdpSocketAsync()
    {
        _udpClient ??= new UdpClient();
        _udpClient.Connect(_host.Address, 60000);
    }

    public async Task SendDataToHostAsync(byte[] data) {
        if (_udpClient is null) {
            return;
        }

        await _udpClient.SendAsync(data);
    }

    private byte[] GetHelloWorldMessage(ushort sequenceNumber) {
        var helloWorld = "Hello World!"u8.ToArray();
        var message = new DataMessage(new DataItem(DataItemType.IQIFData, helloWorld), null);
        return message.GetBytesForDataItemMessage(sequenceNumber);
    }

    #region Dispose

    public void Dispose() {
        Dispose(true);

        // Suppress finalization to prevent the finalizer from running
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// Protected Dispose method that does the actual work of releasing resources
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing) {
        if (_disposed) return;

        if (disposing) {
            _networkStream?.Dispose();
            _client.Dispose();
            _udpClient?.Dispose();
        }
        
        _disposed = true;
    }
    

    public async ValueTask DisposeAsync()
    {
        if (_networkStream != null)
        {
            await _networkStream.DisposeAsync();
            _networkStream = null;
        }

        _client.Dispose();
        _udpClient?.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Destructor (Finalizer)
    /// </summary>
    ~SdrClient() {
        Dispose(false);
    }
    
    #endregion
}
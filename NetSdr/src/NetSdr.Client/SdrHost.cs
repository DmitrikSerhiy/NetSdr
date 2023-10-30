namespace NetSdr.Client;

using Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class SdrHost: IHost {
    public IPAddress Address { get; init; } = null!;
    public List<IClient> Clients { get; init; } = new ();
    
    private UdpClient? _udpListener;
    private readonly int _udpPort; 
    private CancellationTokenSource? _udpCancellation;
    
    private object _locker = new();

    public SdrHost(int port = 60000) {
        _udpPort = port;
    }
    
    public void AttachClient(IClient client) {
        Clients.Add(client);
    }
    
    public async Task StartListeningAsync() {
        _udpListener = new UdpClient(_udpPort);
        _udpCancellation = new CancellationTokenSource();
        await ListenAsync(_udpCancellation.Token);
    }

    private async Task ListenAsync(CancellationToken cancellationToken) {
        if (_udpListener == null) {
            return;
        }
        
        while (!cancellationToken.IsCancellationRequested) {
            var receivedResults = await _udpListener.ReceiveAsync(cancellationToken);
            HandleReceivedData(receivedResults.Buffer, receivedResults.RemoteEndPoint);
        }
    }

    private void HandleReceivedData(byte[] data, IPEndPoint remoteEndPoint) {

        try {
            var receivedText = Encoding.UTF8.GetString(data);
            Console.WriteLine($"Received: {receivedText}. From {remoteEndPoint.Address}:{remoteEndPoint.Port}");

            lock (_locker)
            {
                File.AppendAllText("receivedData.txt", receivedText + Environment.NewLine);
            }
            
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    public void StopListening() {
        _udpCancellation?.Cancel();
        _udpListener?.Close();
    }
    
    public void Dispose() {
        _udpListener?.Dispose();
    }
}
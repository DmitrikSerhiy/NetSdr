namespace NetSdr.Client;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// A class that wraps the TcpClient class for unit testing purposes
/// </summary>
public class TcpClientAdapter : IDisposable {
    private TcpClient? _client;
    
    public virtual  bool Connected => _client?.Connected ?? false;

    public virtual Task ConnectAsync(IPAddress target, int port) {
        _client ??= new TcpClient();
        return _client.ConnectAsync(target, port);
    }

    public virtual Stream? GetStream() => _client?.GetStream();

    public virtual void Close() => _client?.Close();

    public virtual void Dispose() => _client?.Dispose();
}
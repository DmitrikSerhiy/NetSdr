namespace NetSdr.Client.Interfaces;

using System.Net;


public interface IHost: IDisposable {
    IPAddress Address { get; }
    List<IClient> Clients { get; }
    void AttachClient(IClient client);
    Task StartListeningAsync();
    void StopListening();
}
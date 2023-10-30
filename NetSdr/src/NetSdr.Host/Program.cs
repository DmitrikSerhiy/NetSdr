using NetSdr.Client;
using NetSdr.Client.Helpers;
using System.Net;

// just for the demonstration the localhost is used for both sending and receiving data over different ports
// but it wont work in real life.
// In order client.ConnectAsync(target); to work, the target should be the IP address of the real server (which is out of scope of this project)
// todo: configure dockerfile for appropriate ports
var localHost = IPAddress.Parse("127.0.0.1");

var host = new SdrHost
{
    Address = localHost
};

var tcpClient = new TcpClientAdapter();
var client = new SdrClient(tcpClient, host);
host.AttachClient(client);

await host.StartListeningAsync(); // from now, host will receive from udp:60000 and save data to the file

var target = new SdrTarget { Address = localHost };
await client.ConnectAsync(target);
await client.ReceiverOnAsync(CaptureMode.Fifo16Bit); // pass other params here

Console.ReadKey();
host.StopListening();
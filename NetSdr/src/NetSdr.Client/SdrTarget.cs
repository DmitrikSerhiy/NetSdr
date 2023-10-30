namespace NetSdr.Client;

using Interfaces;
using System.Net;


public sealed class SdrTarget : ITarget {
    public IPAddress Address { get; init; } = null!;
}
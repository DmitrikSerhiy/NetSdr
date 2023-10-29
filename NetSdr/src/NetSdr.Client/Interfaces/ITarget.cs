namespace NetSdr.Client.Interfaces;

using System.Net;


public interface ITarget {
    IPAddress Address { get; }
}
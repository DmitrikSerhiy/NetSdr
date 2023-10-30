namespace NetSdr.Tests;

using Client;
using Client.Helpers;
using Client.Interfaces;
using Moq;
using System.Net;
using Xunit;

public sealed class TcpClientTests {
    
    private readonly Mock<TcpClientAdapter> _mockTcpAdapter;
    private readonly IClient _sdrClient;

    private readonly string _targetIp = "127.0.0.1";
    private readonly int _port = 50000; 

    public TcpClientTests() {
        _mockTcpAdapter = new Mock<TcpClientAdapter>();
        Mock<SdrHost> mockSdrHost = new();
        _sdrClient = new SdrClient(_mockTcpAdapter.Object, mockSdrHost.Object);
    }
    
    
    [Fact]
    public async Task ConnectAsync_ShouldInvokeConnectAsyncAndSetConnectionStateToConnectedAsyncTest() {
        // Arrange
        var target = new SdrTarget { Address = IPAddress.Parse(_targetIp) };

        _mockTcpAdapter.Setup(adapter => adapter.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        _mockTcpAdapter.SetupGet(a => a.Connected).Returns(true);

        // Act
        await _sdrClient.ConnectAsync(target, _port);

        // Assert
        _mockTcpAdapter.Verify(adapter => adapter.ConnectAsync(target.Address, _port), Times.Once);
        Assert.Equal(ConnectionState.Connected, _sdrClient.ConnectionState);
    }
    
    [Fact]
    public async Task ConnectAsync_ShouldSetConnectionStateToUndefinedIfConnectionFailedAsyncTest() {
        // Arrange
        var target = new SdrTarget { Address = IPAddress.Parse(_targetIp) };

        _mockTcpAdapter.Setup(adapter => adapter.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        _mockTcpAdapter.SetupGet(a => a.Connected).Returns(false);
        
        // Act
        await _sdrClient.ConnectAsync(target, _port);

        // Assert
        _mockTcpAdapter.Verify(adapter => adapter.ConnectAsync(target.Address, _port), Times.Once);
        Assert.Equal(ConnectionState.Undefined, _sdrClient.ConnectionState);
    }
    
    [Fact]
    public async Task Disconnect_ShouldCloseConnectionAsyncTest() {
        // Arrange
        _mockTcpAdapter.SetupGet(a => a.Connected).Returns(true);
        var target = new SdrTarget { Address = IPAddress.Parse(_targetIp) };

        // Act
        await _sdrClient.ConnectAsync(target, _port);
        _sdrClient.Disconnect();

        // Assert
        _mockTcpAdapter.Verify(adapter => adapter.Close(), Times.Once);
        Assert.Equal(ConnectionState.Disconnected, _sdrClient.ConnectionState);
    }
    
    [Fact]
    public async Task ReceiverOnAsync_ShouldSendMessageAsyncTest() {
        // Arrange
        var captureMode = CaptureMode.Fifo16Bit;  // or any valid value
        byte dataChannelTypeSpecifier = 0x80;  // default value
        byte? fifoSampleCount = 0x00;  // default value
        var target = new SdrTarget { Address = IPAddress.Parse(_targetIp) };
        _mockTcpAdapter.SetupGet(a => a.Connected).Returns(true);
        _mockTcpAdapter.Setup(adapter => adapter.GetStream()).Returns(() => new MemoryStream());

        // Act
        await _sdrClient.ConnectAsync(target, _port);
        await _sdrClient.ReceiverOnAsync(captureMode, dataChannelTypeSpecifier, fifoSampleCount);

        // Assert
        _mockTcpAdapter.Verify(adapter => adapter.GetStream(), Times.Once);
    }
    
    [Fact]
    public async Task ReceiverOnAsync_ShouldNotSendMessageIfConnectionFailedAsyncTest() {
        // Arrange
        var captureMode = CaptureMode.Fifo16Bit;  // or any valid value
        byte dataChannelTypeSpecifier = 0x80;  // default value
        byte? fifoSampleCount = 0x00;  // default value
        var target = new SdrTarget { Address = IPAddress.Parse(_targetIp) };
        _mockTcpAdapter.SetupGet(a => a.Connected).Returns(false);
        _mockTcpAdapter.Setup(adapter => adapter.GetStream()).Returns(() => new MemoryStream());

        // Act
        await _sdrClient.ConnectAsync(target, _port);
        await _sdrClient.ReceiverOnAsync(captureMode, dataChannelTypeSpecifier, fifoSampleCount);

        // Assert
        _mockTcpAdapter.Verify(adapter => adapter.GetStream(), Times.Never);
    }
        
    [Fact]
    public async Task ReceiverOffAsync_ShouldSendMessageAsyncTest() {
        // Arrange
        var target = new SdrTarget { Address = IPAddress.Parse(_targetIp) };
        _mockTcpAdapter.SetupGet(a => a.Connected).Returns(true);
        _mockTcpAdapter.Setup(adapter => adapter.GetStream()).Returns(() => new MemoryStream());

        // Act
        await _sdrClient.ConnectAsync(target, _port);
        await _sdrClient.ReceiverOffAsync();

        // Assert
        _mockTcpAdapter.Verify(adapter => adapter.GetStream(), Times.Once);
    }
    
    [Fact]
    public async Task ReceiverOffAsync_ShouldNotSendMessageIfConnectionIsFailedAsyncTest() {
        // Arrange
        var target = new SdrTarget { Address = IPAddress.Parse(_targetIp) };
        _mockTcpAdapter.SetupGet(a => a.Connected).Returns(false);
        _mockTcpAdapter.Setup(adapter => adapter.GetStream()).Returns(() => new MemoryStream());

        // Act
        await _sdrClient.ConnectAsync(target, _port);
        await _sdrClient.ReceiverOffAsync();

        // Assert
        _mockTcpAdapter.Verify(adapter => adapter.GetStream(), Times.Never);
    }

}
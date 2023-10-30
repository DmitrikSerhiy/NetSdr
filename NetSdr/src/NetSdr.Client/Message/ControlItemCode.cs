namespace NetSdr.Client.Message; 

public enum ControlItemCode : ushort {
    TargetName = 0x0001,
    TargetSerialNumber = 0x0002,
    InterfaceVersion = 0x0003,
    // ...
    ReceiverState = 0x0018,
    ReceiverFrequency = 0x0020
    // ...
}
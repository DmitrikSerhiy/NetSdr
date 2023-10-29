namespace NetSdr.Client.Message; 

public enum MessageType : byte
{
    // Host message types
    HostSetControlItem = 0b0000, // 000 Host
    HostRequestCurrentControlItem = 0b0001, // 001 Host
    HostRequestControlItemRange = 0b0010, // 010 Host
    HostDataItemACK = 0b0011, // 011 Host
    HostDataItem0 = 0b0100, // 100 Host
    HostDataItem1 = 0b0101, // 101 Host
    HostDataItem2 = 0b0110, // 110 Host
    HostDataItem3 = 0b0111, // 111 Host

    // Target message types
    TargetResponseToSetOrRequest = 0b1000, // 000 Target
    TargetUnsolicitedControlItem = 0b1001, // 001 Target
    TargetResponseToControlItemRange = 0b1010, // 010 Target
    TargetDataItemACK = 0b1011, // 011 Target
    TargetDataItem0 = 0b1100, // 100 Target
    TargetDataItem1 = 0b1101, // 101 Target
    TargetDataItem2 = 0b1110, // 110 Target
    TargetDataItem3 = 0b1111  // 111 Target
}
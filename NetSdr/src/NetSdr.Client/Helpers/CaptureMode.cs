namespace NetSdr.Client.Helpers; 

public enum CaptureMode {
    Contiguous16Bit = 0x00,
    Contiguous24Bit = 0x80,
    Fifo16Bit = 0x01,
    HardwareTriggeredPulse24Bit = 0x83,
    HardwareTriggeredPulse16Bit = 0x03
}
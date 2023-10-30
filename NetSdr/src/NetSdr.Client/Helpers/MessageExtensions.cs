namespace NetSdr.Client.Helpers;

using Interfaces;
using Message.Item;


public static class MessageExtensions {
    
    /// <summary>
    /// Took From specification:
    /// A special case for Data Items is that a message length of Zero is used to specify an actual message length of 8194 
    /// bytes(8192 data bytes + 2 header bytes). This allows data blocks of a power of 2 to be used which is useful in dealing 
    /// with FFT data.
    /// HEADER:
    /// 1) A a bitwise AND between blockLength and 0xFF makes any bit in blockLength that aligns with a 0 in 0xFF will be cleared to 0, and any bit that aligns with a 1 in 0xFF will remain unchanged.
    /// Because 0xFF is all ones (1111_1111) for the least significant 8 bits, the operation essentially keeps only the least significant 8 bits of blockLength and discards (sets to zero) all the higher bits.
    /// 2) By casting MessageType to an int and left-shifting by 8 bits, this operation moves the 3 bits of the MessageType to the 9th, 10th, and 11th positions of the header.
    /// 3) The expression blockLength & 0x1F00 extracts bits 8-12 of the blockLength. By left-shifting the result by 3 bit the 5 bits extracted from blockLength
    /// (which represent the most significant bits of the block length) are moved to the 12th to 16th positions in the header.
    /// This allows the header to store the full 13-bit block length split between the least significant 8 bits and the 5 most significant bits.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static byte[] GetBytes(this IMessage message) {

        var byteList = new List<byte>();

        // Write header
        var blockLength = CalculateBlockLength(message);
        if (message.Item is DataItem && blockLength == 8192) // special case
            blockLength = 0;

        var header = (ushort)((blockLength & 0xFF) | ((int)message.Header.MessageType << 8) | ((blockLength & 0x1F00) << 3));
        byteList.Add((byte)(header & 0xFF)); // lsb
        byteList.Add((byte)((header >> 8) & 0xFF)); // msb

        // Write message
        if (message.Item is ControlItem controlItem)
        {
            byteList.Add((byte)((ushort)controlItem.Code & 0xFF)); // lsb
            byteList.Add((byte)(((ushort)controlItem.Code >> 8) & 0xFF)); // msb
            byteList.AddRange(controlItem.Parameters);
        }
        else if (message.Item is DataItem dataItem)
        {
            byteList.AddRange(dataItem.Data);
        }

        return byteList.ToArray();
    }
    
    private static int CalculateBlockLength(IMessage message)
    {
        var length = 2; // Always counting 2 bytes of the header

        if (message.Item is ControlItem controlItem)
        {
            length += 2; // 2 bytes for Control Item code
            length += controlItem.Parameters.Length; // Number of bytes in parameters
        }
        else if (message.Item is DataItem dataItem)
        {
            length += dataItem.Data.Length; // Number of bytes in data
        }

        return length;
    }
}
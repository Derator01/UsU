using System.Linq;
using System.Text;

namespace WordsGame;

public sealed class Packet
{
    public enum PacketType
    {
        HandShake = 0x01,
        Message = 0x02,
        Command = 0x03,
        Terminate = 0xFF
    }

    public const byte PACKET_END = 0xFE;

    public readonly PacketType Type;

    public readonly string Message;

    public Packet(byte[] bytes)
    {
        PacketType = (PacketType)bytes[0];
        Message = Encoding.Unicode.GetString(bytes.Skip(1).ToArray());
    }

    public static byte[] ToBytes(PacketType type, string message) => Encoding.Unicode.GetBytes(message).Prepend((byte)type).Append(PACKET_END).ToArray();
}
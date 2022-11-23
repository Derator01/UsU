using System.Threading;

namespace UsU.Networking;

public class Client
{
    public readonly int ID;
    public readonly TcpClient TcpClient;
    private readonly NetworkingStream stream;
    public bool Connected {get => TcpClient.Connected;}

    public delegate void OnPacketCame(byte[] packet, int  clientID);
    public event OnPacketCame PacketCame; 

    public Client(int id, tcpClient)
    {
        ID = id;
        TcpClient = tcpClient;
        stream TcpClient.GetStream();
        new Thread(ClientLoop).Start();
    }

    private void ClientLoop()
    {
        try
        {
            while(TcpClient.Connected)
            {
                throw new NotImplementedException();
            }
        }
        finally
        {
            TcpClient.Close();
        }
    }
}
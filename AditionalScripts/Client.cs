using System;
using System.Net.Sockets;
using System.Threading;
using WordsGame;

namespace UsU.Networking;

public class Client
{
    public readonly int ID;
    public readonly TcpClient TcpClient;
    private readonly NetworkStream stream;
    public bool Connected { get => TcpClient.Connected; }

    public delegate void OnPacketCame(int clientID, Packet packet);
    public event OnPacketCame PacketCame;

    public Client(int id, TcpClient tcpClient)
    {
        ID = id;
        TcpClient = tcpClient;
        stream = TcpClient.GetStream();
        new Thread(ClientLoop).Start();
    }

    private void ClientLoop()
    {
        try
        {
            while (TcpClient.Connected)
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
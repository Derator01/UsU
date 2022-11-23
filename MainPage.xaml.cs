using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WordsGame.Storage;

namespace WordsGame;

public partial class MainPage : ContentPage
{
    #region AppHelpers
    const int UPDATE_SPEED = 20;

    private readonly Random random = new();
    #endregion

    #region Networking
    private NetworkState _networkState = NetworkState.Offline;

    private bool _networking = false;

    private const int PORT = 13000;
    private readonly IPAddress _localIP = IPAddress.Parse("127.0.0.1");
    private int _clientCount = 0;

    private TcpListener _server;

    private readonly List<Client> _clients = new();

    private enum NetworkState : int
    {
        Offline = 0,
        Client = 1,
        Host = 2
    }
    private enum ClientStates : int
    {
        NotConnected = 0,
        Connecting = 1,
        FailedToConnect = 2
    }
    private enum HostStates : int
    {
        Stopped = 0,
        RunningS = 1,
        RunningM = 2,
        Error = 3
    }
    #endregion

    #region GameData
    private int Score { get { return _localStorage.Score; } set { _localStorage.Score = value; ScoreUpdate(); _localStorage.Save(); } }
    private new int Style { get => _localStorage.Style; set => _localStorage.Style = value; }

    private bool answered = false;

    public string Answer;

    private readonly DataStorage _localStorage = new();

    private List<string> _possibleWords;

    private readonly char[] _styles = new[]
    {
        'F',
        'E',
        'D',
        'C',
        'B',
        'A',
        'S',
        'Z',
        'G',
    };
    private readonly int[] _styleTimes = new[]
    {   0,
        13,
        10,
        8,
        7,
        6,
        5,
        4,
        3
    };
    private string NextWord { get => _possibleWords[random.Next(_possibleWords.Count - 1)]; }
    private string _currentWord;
    #endregion

    public MainPage()
    {
        InitializeComponent();
        //OnOpened();
    }

    private void OnConnectionSwitchBtn(object sender, EventArgs e)
    {
        if (_networking)
            return;

        _networkState++;
        if (!Enum.IsDefined(typeof(NetworkState), _networkState))
            _networkState = 0;

        NetworkStateLbl.Text = Enum.GetName(typeof(NetworkState), _networkState);
    }

    private void OnStartStopGameBtn(object sender, EventArgs e)
    {
        if (_networkState == 0)
            return;
        if (Connectivity.Current.NetworkAccess == NetworkAccess.None || Connectivity.Current.NetworkAccess == NetworkAccess.Unknown)
            return;

        _networking = !_networking;

        StartStopBtn.Text = _networking ? "Stop Game" : "Start Game";

        if (_networkState == NetworkState.Client)
            new Thread(ClientLoop).Start();
        else
            new Thread(ServerLoop).Start();
    }

    private void ClientLoop()
    {

    }

    private void ServerLoop()
    {
        _server = new TcpListener(_localIP, PORT);
        _server.Start();

        try
        {
            while(_networking)
            {
            Dispatcher.Dispatch(() => NDebugLbl.Text = nameof(HostStates.RunningS));
            
            var tcpClient = _server.AcceptTcpClient()

            NetworkStream stream = client.GetStream();

            byte nextByte;
            if((nextByte =stream.ReadNextByte()) != 0x01)
            {
                Dispatcher.Dispatch(() => NDebugLbl.Text = "Failed To Connect!");
                client.Close();
            }
            else
            {   
                Dispatcher.Dispatch(() => NDebugLbl.Text = nameof(HostStates.RunningM));
                Client client =new(_clientCount++, tcpClient)
                _clients.Add(client);
                client.PacketCame += OnPacket;
            }
            
            }
        }
        catch (Exception e)
        {
            Dispatcher.Dispatch(() => NDebugLbl.Text = nameof(HostStates.Error) + " " + e.Message);
        }
        finally
        {
            _server.Close();
            Dispatcher.Dispatch(() => NDebugLbl.Text = nameof(HostStates.Stopped));
        }
    }

    private void OnPacket(Packet packet, int  clientID)
    {
        switch(packet.Type)
        {
            case Packet.PacketType.Message:
                NDebugLbl.Text = packet.Message;
            break;
            case Packet.PacketType.Command:
                Execute(packet.Message);
            break;
        }
    }

    private void Execute(string command)
    {
        string arr = command.Split(' ');
        for(int i = 0; i < arr.length-1; i++)
            

        if(command[1] != ' ')
            return;

        switch(type)
        {
            case 'w':
                PeerAnswer(command);
            break;
            case 'b':
                Broadcast(command);
            break;
        }
    }

    //    public async void OnOpened()
    //    {
    //        await LoadWordsHoldMultiThreaded();
    //        Load();
    //        Thread combobarLoop = new(CombobarLoop);
    //        combobarLoop.Start();
    //        ScoreLbl.Text = _localStorage.Score.ToString();
    //    }

    //    private void Load()
    //    {
    //        if (string.IsNullOrEmpty(_localStorage.ThreeLetters))
    //            PlayNextWord();
    //        else
    //            WordLbl.Text = _localStorage.ThreeLetters;
    //    }

    private async Task LoadWordsHoldMultiThreaded()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("Dict.txt");
        using var reader = new StreamReader(stream);

        _possibleWords = reader.ReadToEnd().Split(' ').ToList();
    }

    private void OnEnterBtn(object sender, EventArgs e)
    {
        //        if (Answer == null)
        //            return;
        //        if (_possibleWords.Contains(Answer.ToLower()) && Answer.ToLower().Contains(_localStorage.ThreeLetters))
        //        {
        //            VictoryMessage.Text = "Congrats!";
        //            if (Style < 6)
        //                Score++;
        //            else if (Style < 8)
        //                Score += 2;
        //            else
        //                Score += 3;

        //            if (Style < 8)
        //                Style++;
        //            answered = true;

        //            InputBox.Text = "";
        //            PlayNextWord();
        //        }
        //        else
        //        {
        //            VictoryMessage.Text = "Go To Hell!";
        //            //Score--;
        //            answered = true;
        //            if (Style > 0)
        //                Style--;
        //        }
    }
    private void OnSkipBtn(object sender, EventArgs e)
    {
        //        Score -= 4;
        //        if (Style > 3)
        //            Style -= 3;
        //        else if (Style > 0)
        //            Style--;

        //        PlayNextWord();
    }
    private void OnHintBtn(object sender, EventArgs e)
    {
        //        if (WordLbl.Text.Length > 14)
        //        {
        //            VictoryMessage.Text = "Max Hint Count";
        //            return;
        //        }

        //        Score--;

        //        List<string> validHints = new();
        //        for (int i = 0; i < _possibleWords.Count; i++)
        //            if (_possibleWords[i].Contains(_localStorage.ThreeLetters))
        //                validHints.Add(_possibleWords[i]);

        //        string hint = validHints[random.Next(validHints.Count)];
        //        int index = random.Next(0, hint.Length - 2);

        //        string part = hint[index].ToString() + hint[++index] + hint[++index];
        //        if (part == _localStorage.ThreeLetters)
        //        {
        //            Score++;
        //            OnHintBtn(null, null);
        //        }
        //        else
        //            WordLbl.Text += $" ({part})";
    }
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        //        if (((Entry)sender).Text == null)
        //            return;
        //        Answer = ((Entry)sender).Text.Trim();
    }
    private void OnCompleted(object sender, EventArgs e)
    {
        //        OnEnterBtn(null, null);
    }

    private void PlayNextWord()
    {
        //        _currentWord = NextWord;
        //        if (_currentWord.Length < 3)
        //        {
        //            PlayNextWord();
        //            return;
        //        }
        //        int firstIndex = random.Next(_currentWord.Length - 2);

        //        _localStorage.ThreeLetters = _currentWord[firstIndex].ToString() + _currentWord[++firstIndex] + _currentWord[++firstIndex];

        //        WordLbl.Text = _localStorage.ThreeLetters;

    }

    private void ScoreUpdate()
    {
        //        if (_localStorage.Score < 0)
        //        {
        //            InputBox.Text = "You Lost!";
        //            Reset();
        //        }
        //        if (_localStorage.Score > 50 && _localStorage.Score < 60)
        //        {
        //            InputBox.Text = "You Won!";
        //            _localStorage.Score = 1000;
        //        }

        //        ScoreLbl.Text = _localStorage.Score.ToString();
    }

    private void Reset()
    {
        //        PlayNextWord();

        //        Score = 5;
    }

    private void CombobarLoop()
    {
        //        while (true)
        //        {
        //            Dispatcher.Dispatch(() => ComboTextLbl.Text = _styles[Style].ToString());
        //            int currentStyle = _styleTimes[Style];
        //            Dispatcher.Dispatch(() => ComboTimeLbl.Text = currentStyle.ToString());
        //            Dispatcher.Dispatch(() => { ComboBar.Progress = 1; });
        //            if (Style == 0)
        //            {
        //                while (Style == 0)
        //                    Thread.Sleep(20);
        //            }
        //            int comboTime = currentStyle * UPDATE_SPEED;
        //            for (int i = 1; i <= comboTime; i++)
        //            {
        //                Thread.Sleep(50);
        //                if (answered || currentStyle != _localStorage.Style)
        //                {
        //                    answered = false;
        //                    break;
        //                }
        //                float progress = 1f - (float)(i / (float)comboTime);
        //                Dispatcher.Dispatch(() => ComboBar.Progress = progress - 0.0001f);
        //                Dispatcher.Dispatch(() => ComboTimeLbl.Text = ((int)(progress * currentStyle)).ToString());

        //                if (i == comboTime)
        //                {
        //                    Style--;
        //                    break;
        //                }
        //            }
        //        }
    }

    private async void OnGoToStatistics(object sender, EventArgs e)
    {
        //        await Navigation.PushAsync(new Statistics(new Dictionary<string, object>() {
        //                                                                                    { nameof(_localStorage.MaxScore), _localStorage.MaxScore },
        //                                                                                    { nameof(_localStorage.MaxStyle), _styles[_localStorage.MaxStyle] } }));
    }
}
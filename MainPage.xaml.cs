using IOSerializer;

namespace WordsGame;

public partial class MainPage : ContentPage
{
    #region GameStatitics




    private Dictionary<string, string> _strings;
    private Dictionary<string, float> _floats;
    #endregion

    #region GameData
    private int Score { get { return (int)_floats["Score"]; } set { _floats["Score"] = value; ScoreUpdate(); } }
    private new int Style { get; set; }

    private bool answered = false;

    public string Answer;

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
    {   -1,
        13,
        11,
        9,
        8,
        7,
        6,
        4,
        3
    };
    private string NextWord { get => _possibleWords[random.Next(_possibleWords.Count - 1)]; }
    private string _currentWord;
    private readonly Random random = new Random();
    #endregion

    public MainPage()
    {
        InitializeComponent();
        OnOpened();
    }

    public async void OnOpened()
    {
        await LoadWordsHold();
        await Load();
        Thread combobarLoop = new(CombobarLoop);
        combobarLoop.Start();
    }

    async Task Save()
    {

        if (!File.Exists(_savePath))
        {
            var file = File.Create(_savePath);
            file.Close();
        }
        Serializer.Serialize(File.OpenWrite(_savePath), _strings, _floats);
    }
    async Task Load()
    {
        if (!File.Exists(_savePath))
        {
            _strings = new() { { "ThreeLetters", "" } };
            _floats = new() { { "Score", 5 } };
            PlayNextWord();
            return;
        }

        Serializer.Deserialize(File.OpenRead(_savePath), out _strings, out _floats);
        Score = (int)_floats["Score"];
        WordLbl.Text = _strings["ThreeLetters"];
    }
    async Task LoadWordsHold()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("Dict.txt");
        using var reader = new StreamReader(stream);

        _possibleWords = reader.ReadToEnd().Split(' ').ToList();
    }

    private void OnEnterBtn(object sender, EventArgs e)
    {
        if (Answer == null)
            return;
        if (_possibleWords.Contains(Answer.ToLower()) && Answer.ToLower().Contains(_strings["ThreeLetters"]))
        {
            VictoryMessage.Text = "Congrats!";
            Score += 1;
            if (Style < 8)
                Style++;
            answered = true;
            if (_floats.ContainsKey("MaxStyle"))
                if (_floats["MaxStyle"] < Style)
                    _floats["MaxStyle"] = Style;
                else
                    _floats.Add("MaxStyle", Style);
            InputBox.Text = "";
            PlayNextWord();
        }
        else
        {
            VictoryMessage.Text = "Go To Hell!";
            Score -= 1;
            answered = true;
            if (Style > 0)
                Style--;
        }
    }
    private void OnSkipBtn(object sender, EventArgs e)
    {
        Score -= 3;
        if (Style > 0)
            Style--;

        PlayNextWord();
    }
    private void OnHintBtn(object sender, EventArgs e)
    {
        if (WordLbl.Text.Length > 14)
        {
            VictoryMessage.Text = "Max Hint Count";
            return;
        }

        Score -= 1;

        List<string> validHints = new();
        for (int i = 0; i < _possibleWords.Count; i++)
            if (_possibleWords[i].Contains(_strings["ThreeLetters"]))
                validHints.Add(_possibleWords[i]);

        string hint = validHints[random.Next(validHints.Count)];
        int index = random.Next(0, hint.Length - 2);

        WordLbl.Text += $" ({hint[index]}{hint[++index]}{hint[++index]})";
    }
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (((Entry)sender).Text == null)
            return;
        Answer = ((Entry)sender).Text.Trim();
    }
    private void OnCompleted(object sender, EventArgs e)
    {
        OnEnterBtn(null, null);
    }

    private void PlayNextWord()
    {
        _currentWord = NextWord;
        if (_currentWord.Length < 3)
        {
            PlayNextWord();
            return;
        }
        int firstIndex = random.Next(_currentWord.Length - 2);
        if (!_strings.ContainsKey("ThreeLetters"))
            _strings.Add("ThreeLetters", "");
        _strings["ThreeLetters"] = _currentWord[firstIndex].ToString() + _currentWord[++firstIndex] + _currentWord[++firstIndex];

        WordLbl.Text = _strings["ThreeLetters"];

        Save();
    }

    private void ScoreUpdate()
    {
        if (_floats["Score"] < 0)
        {
            InputBox.Text = "You Lost!";
            Reset();
        }
        ScoreLbl.Text = _floats["Score"].ToString();
        if (_floats["Score"] > 50)
        {
            InputBox.Text = "You Won!";
            _floats["Score"] = 1000;
        }
    }

    private void Reset()
    {
        PlayNextWord();

        Score = 5;
    }

    private void CombobarLoop()
    {
        while (true)
        {
            Dispatcher.Dispatch(new Action(() => ComboTextLbl.Text = _styles[Style].ToString())).ToString();
            if (Style == 0)
            {
                Dispatcher.Dispatch(new Action(() => { ComboBar.Progress = 1; }));
                Thread.Sleep(5);
                continue;
            }
            for (int i = 1; i <= _styleTimes[Style] * 20; i++)
            {
                Thread.Sleep(50);
                if (answered)
                {
                    answered = false;
                    break;
                }
                float progress = 1f - (float)((float)i / (float)((float)_styleTimes[Style] * 20f));
                Dispatcher.Dispatch(new Action(() =>
                {
                    ComboBar.Progress = progress;
                }));
                Dispatcher.Dispatch(new Action(() => DebugLbl.Text = (progress * 100).ToString()));

                if (i == _styleTimes[Style] * 20)
                {
                    Style--;
                    break;
                }
            }
        }
    }
}
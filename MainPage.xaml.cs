using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UsU;

public partial class MainPage : ContentPage
{
    #region State Bools

    #endregion

    private int Score { get { ScoreUpdate(); return _score; } set { _score = value; ScoreUpdate(); } }
    private int _score = 5;


    public string Answer;

    private List<string> _possibleWords = new();
    private string NextWord { get => _possibleWords[random.Next(_possibleWords.Count - 1)]; }
    private string _currentWord;
    private string _threeLetters;

    private readonly Random random = new Random();
    public MainPage()
    {
        LoadMauiAsset();
        InitializeComponent();
        PlayNextWord();
        Window.Created += OnOpened;
        Window.Stopped += OnStopped;
    }

    private void OnStopped(object sender, EventArgs e)
    {
        SaveScore();
    }
    async Task SaveScore()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("Save.kln");
        using var reader = new StreamReader(stream);

        var contents = reader.ReadToEnd();
    }

    private void OnOpened(object sender, EventArgs eventArgs)
    {
        LoadScore();
    }

    async Task LoadScore()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("Save.kln");
        stream.Position = 0;
        using var writer = new StreamWriter(stream);
        writer.Write(Score);
    }

    async Task LoadMauiAsset()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("Dict.txt");
        using var reader = new StreamReader(stream);

        var contents = reader.ReadToEnd();
        _possibleWords = contents.Replace("\r", "").Split('\n').ToList();
    }

    private void OnEnterBtn(object sender, EventArgs e)
    {
        if (Answer == null)
            return;
        if (_possibleWords.Contains(Answer.ToLower()) && Answer.ToLower().Contains(_threeLetters))
        {
            VictoryMessage.Text = "Congrats";
            Score += 1;
            InputBox.Text = "";
            PlayNextWord();
        }
        else
        {
            VictoryMessage.Text = "Go To Hell";
            Score -= 1;
        }
    }
    private void OnSkipBtn(object sender, EventArgs e)
    {
        Score -= 3;
        
         PlayNextWord();
    }
    private void OnHintBtn(object sender, EventArgs e)
    {
        Score -= 1;
        
        List<string> validHints = new();
        for (int i = 0; i < _possibleWords.Count; i++)
            if (_possibleWords[i].Contains(_threeLetters))
                validHints.Add(_possibleWords[i]);
        
        string hint = validHints[random.Next(validHints.Count)];
        int index = random.Next(0, hint.Length-2);
        
        WordLbl.Text = hint[index].ToString() + hint[++index] + hint[++index];
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
        int firstIndex = random.Next(_currentWord.Length - 2);
        _threeLetters = _currentWord[firstIndex].ToString() + _currentWord[++firstIndex] + _currentWord[++firstIndex];
        WordLbl.Text = _threeLetters;
    }

    private void ScoreUpdate()
    {
        if (_score < 0)
        {
            WordLbl.Text = "You Lost!";
            Reset();
        }
        ScoreLbl.Text = _score.ToString();
    }

    

    private void Reset()
    {
        _score = 5;
        InputBox.Text = "";
        PlayNextWord();
    }
}
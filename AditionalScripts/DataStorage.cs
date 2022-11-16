using IOSerializer;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.IO;

namespace WordsGame.Storage;

public class DataStorage
{
    public string Path { get; } = FileSystem.Current.AppDataDirectory + @"/save.cfg";

    private readonly Dictionary<string, float> floats;
    private readonly Dictionary<string, string> strings;

    public int Score { get { return _score; } set { _score = value; if (MaxScore < Score) MaxScore = _score; } }
    private int _score;

    public int Style { get { return _style; } set { _style = value; if (MaxStyle < Style) MaxStyle = _style; } }
    private int _style = 0;

    public string ThreeLetters { get { return _threeLetters; } set { _threeLetters = value; Save(); } }
    private string _threeLetters;


    public int MaxScore { get; private set; }

    public int MaxStyle { get; private set; }

    public void Save()
    {
        FlushToLists();
        FileStream stream = File.OpenWrite(Path);
        Serializer.Serialize(stream, strings, floats);
        stream.Close();
    }

    private void FlushToLists()
    {
        floats[nameof(Score)] = Score;
        floats[nameof(MaxScore)] = MaxScore;
        floats[nameof(MaxStyle)] = MaxStyle;

        strings[nameof(ThreeLetters)] = ThreeLetters;
    }

    public DataStorage()
    {
        if (!File.Exists(Path))
        {
            var file = File.Create(Path);
            file.Close();
            _score = 5;
            _threeLetters = "";
            MaxScore = 0;
            MaxStyle = 0;

        }

        FileStream stream = File.OpenRead(Path);
        Serializer.Deserialize(stream, out strings, out floats);
        stream.Close();
        if (!floats.ContainsKey(nameof(Score)))
            floats.Add(nameof(Score), 5);
        if (!floats.ContainsKey(nameof(MaxScore)))
            floats.Add(nameof(MaxScore), 5);
        if (!floats.ContainsKey(nameof(MaxStyle)))
            floats.Add(nameof(MaxStyle), 0);
        if (!strings.ContainsKey(nameof(ThreeLetters)))
            strings.Add(nameof(ThreeLetters), "");

        _score = (int)floats[nameof(Score)];
        MaxScore = (int)floats[nameof(MaxStyle)];
        MaxStyle = (int)floats[nameof(MaxStyle)];

        _threeLetters = strings[nameof(ThreeLetters)];
    }

}

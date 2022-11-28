using Microsoft.Maui.Controls;
using System.Collections.Generic;
using UsU.Extended;

namespace WordsGame;

public partial class Statistics : ContentPage
{
    public Statistics(IDictionary<string, object> dictionary)
    {
        InitializeComponent();
        LoadStatistics(dictionary);
    }

    public void LoadStatistics(IDictionary<string, object> dictionary)
    {
        foreach (var obj in dictionary)
            MainLayout.Add(new Label() { Text = $"{obj.Key.SplitByCapitals()} = {obj.Value}" });
    }
}
using Microsoft.Maui.Controls;
using System.Collections.Generic;

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
        foreach(var obj in dictionary)
            MainLayout.Add(new label() {Text = $"{obj.Key.SplitByCapitals()} = {obj.Value}"});
    }
}
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace WordsGame;

public partial class Statistics : ContentPage
{
    public Statistics(IDictionary<string, object> query)
    {
        InitializeComponent();
        ApplyQueryAttributes(query);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        MaxScore.Text = $"Max Score = {query[nameof(MaxScore)]}";
        MaxStyle.Text = $"Max Style = {query[nameof(MaxStyle)]}";
    }
}
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace WordsGame;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new MainPage());
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Created += (s, e) =>
        {

        };

        return window;
    }
}

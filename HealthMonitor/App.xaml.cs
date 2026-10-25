
namespace HealthMonitor;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());

        window.Width = 1000;
        window.Height = 750;

        window.MinimumWidth = 800;
        window.MinimumHeight = 650;

        return window;
    }
}


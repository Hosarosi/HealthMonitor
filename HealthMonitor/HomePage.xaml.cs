
namespace HealthMonitor;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnRegistrarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MedicaoPage));
    }

    private async void OnHistoricoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Historico));
    }

    private async void OnAlertasClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Alertas));
    }

    private async void OnResumoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Resumo));
    }

    private async void OnSairClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}

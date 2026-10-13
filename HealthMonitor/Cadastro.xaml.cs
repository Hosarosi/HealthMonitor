
namespace HealthMonitor;

public partial class CadastroPage : ContentPage
{
    public CadastroPage()
    {
        InitializeComponent();
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        string nome = NomeEntry.Text ?? "";
        string idade = IdadeEntry.Text ?? "";
        string peso = PesoEntry.Text ?? "";
        string altura = AlturaEntry.Text ?? "";
        string email = EmailEntry.Text ?? "";
        string senha = SenhaEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(nome) ||
            string.IsNullOrWhiteSpace(idade) ||
            string.IsNullOrWhiteSpace(peso) ||
            string.IsNullOrWhiteSpace(altura) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlertAsync("Atenção",
                "Preencha todos os campos.",
                "OK");
            return;
        }

        await DisplayAlertAsync("HealthMonitor",
            "Dados preenchidos. O cadastro será salvo no banco posteriormente.",
            "OK");
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}

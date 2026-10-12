namespace HealthMonitor
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnEntrarClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string senha = SenhaEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                await DisplayAlertAsync("Atenção",
                    "Informe o e-mail e a senha.",
                    "OK");
                return;
            }

            await DisplayAlertAsync("HealthMonitor",
                "Dados preenchidos. O acesso ao banco será configurado posteriormente.",
                "OK");
        }

        private async void OnCadastrarClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("HealthMonitor",
                "A tela de cadastro será criada na próxima etapa.",
                "OK");
        }
    }
}
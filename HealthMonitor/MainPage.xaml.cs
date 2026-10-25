
using System.Net.Http.Json;
using System.Text.Json;

namespace HealthMonitor
{
    public partial class MainPage : ContentPage
    {
        private static readonly HttpClient cliente = new HttpClient();
        private const string EnderecoApi = "https://localhost:7231/api/Usuario/";

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnEntrarClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim() ?? "";
            string senha = SenhaEntry.Text ?? "";

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha))
            {
                await DisplayAlertAsync(
                    "Atenção",
                    "Informe o e-mail e a senha.",
                    "OK");
                return;
            }

            try
            {
                var dados = new { email, senha };

                using var resposta = await cliente.PostAsJsonAsync(
                    EnderecoApi + "login", dados);

                string conteudo = await resposta.Content.ReadAsStringAsync();
                string mensagem = "Não foi possível realizar o login.";
                string nome = "";
                int idUsuario = 0;

                try
                {
                    using var json = JsonDocument.Parse(conteudo);

                    if (json.RootElement.TryGetProperty(
                        "mensagem", out var mensagemJson))
                    {
                        mensagem = mensagemJson.GetString() ?? mensagem;
                    }

                    if (json.RootElement.TryGetProperty(
                        "nome", out var nomeJson))
                    {
                        nome = nomeJson.GetString() ?? "";
                    }

                    if (json.RootElement.TryGetProperty(
                        "idUsuario", out var idJson))
                    {
                        idUsuario = idJson.GetInt32();
                    }
                }
                catch (JsonException)
                {
                    mensagem = "A API retornou uma resposta inesperada.";
                }

                if (resposta.IsSuccessStatusCode)
                {
                    if (idUsuario <= 0)
                    {
                        await DisplayAlertAsync(
                            "Erro no login",
                            "A API não retornou um ID de usuário válido.",
                            "OK");
                        return;
                    }

                    Preferences.Set("IdUsuario", idUsuario);

                    await DisplayAlertAsync(
                        "Bem-vinda ao HealthMonitor",
                        string.IsNullOrWhiteSpace(nome)
                            ? mensagem
                            : $"Olá, {nome}! Login realizado com sucesso.",
                        "OK");

                    await Shell.Current.GoToAsync(nameof(HomePage));
                }
                else
                {
                    await DisplayAlertAsync(
                        "Não foi possível entrar",
                        mensagem,
                        "OK");
                }
            }
            catch (HttpRequestException)
            {
                await DisplayAlertAsync(
                    "Erro de conexão",
                    "Não foi possível acessar a API. Confira se ela está em execução.",
                    "OK");
            }
            catch (TaskCanceledException)
            {
                await DisplayAlertAsync(
                    "Tempo esgotado",
                    "A API demorou para responder. Tente novamente.",
                    "OK");
            }
        }

        private async void OnCadastrarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CadastroPage));
        }
    }
}

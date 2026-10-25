
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace HealthMonitor;

public partial class CadastroPage : ContentPage
{
    private static readonly HttpClient cliente = new HttpClient();

    // Endereço HTTPS da API executada no computador.
    private const string EnderecoApi =
        "https://localhost:7231/api/Usuario/cadastrar";

    public CadastroPage()
    {
        InitializeComponent();
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        string nome = NomeEntry.Text?.Trim() ?? "";
        string idadeTexto = IdadeEntry.Text?.Trim() ?? "";
        string pesoTexto = PesoEntry.Text?.Trim() ?? "";
        string alturaTexto = AlturaEntry.Text?.Trim() ?? "";
        string email = EmailEntry.Text?.Trim() ?? "";
        string senha = SenhaEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(nome) ||
            string.IsNullOrWhiteSpace(idadeTexto) ||
            string.IsNullOrWhiteSpace(pesoTexto) ||
            string.IsNullOrWhiteSpace(alturaTexto) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlertAsync(
                "Atenção",
                "Preencha todos os campos.",
                "OK");
            return;
        }

        if (!int.TryParse(idadeTexto, out int idade) || idade <= 0)
        {
            await DisplayAlertAsync(
                "Atenção",
                "Informe uma idade válida.",
                "OK");
            return;
        }

        if (!decimal.TryParse(
                pesoTexto.Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimal peso) || peso <= 0)
        {
            await DisplayAlertAsync(
                "Atenção",
                "Informe um peso válido. Exemplo: 70,5.",
                "OK");
            return;
        }

        if (!decimal.TryParse(
                alturaTexto.Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimal altura) || altura <= 0)
        {
            await DisplayAlertAsync(
                "Atenção",
                "Informe uma altura válida. Exemplo: 1,70.",
                "OK");
            return;
        }

        var dados = new
        {
            nome,
            idade,
            peso,
            altura,
            email,
            senha
        };

        try
        {
            using var resposta = await cliente.PostAsJsonAsync(
                EnderecoApi, dados);

            string conteudo = await resposta.Content.ReadAsStringAsync();

            string mensagem;

            try
            {
                using var json = JsonDocument.Parse(conteudo);
                mensagem = json.RootElement
                    .TryGetProperty("mensagem", out var valor)
                    ? valor.GetString() ?? "Resposta recebida da API."
                    : "Resposta recebida da API.";
            }
            catch (JsonException)
            {
                mensagem = "A API retornou uma resposta inesperada.";
            }

            if (resposta.IsSuccessStatusCode)
            {
                await DisplayAlertAsync(
                    "HealthMonitor",
                    mensagem,
                    "OK");

                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlertAsync(
                    "Não foi possível cadastrar",
                    mensagem,
                    "OK");
            }
        }
        catch (HttpRequestException)
        {
            await DisplayAlertAsync(
                "Erro de conexão",
                "Não foi possível acessar a API. Verifique se ela está em execução no computador.",
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

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}

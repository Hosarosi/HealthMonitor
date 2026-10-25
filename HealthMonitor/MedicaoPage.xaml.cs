

using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace HealthMonitor;

public partial class MedicaoPage : ContentPage
{
    private static readonly HttpClient cliente = new HttpClient();
    private const string EnderecoApi =
        "https://localhost:7231/api/Medicao/salvar";

    public MedicaoPage()
    {
        InitializeComponent();
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SistolicaEntry.Text) ||
            string.IsNullOrWhiteSpace(DiastolicaEntry.Text) ||
            string.IsNullOrWhiteSpace(FrequenciaEntry.Text) ||
            string.IsNullOrWhiteSpace(TemperaturaEntry.Text) ||
            string.IsNullOrWhiteSpace(OxigenacaoEntry.Text))
        {
            await DisplayAlertAsync(
                "Atenção",
                "Preencha todos os campos obrigatórios da medição.",
                "OK");
            return;
        }

        if (!int.TryParse(SistolicaEntry.Text, out int sistolica) ||
            !int.TryParse(DiastolicaEntry.Text, out int diastolica) ||
            !int.TryParse(FrequenciaEntry.Text, out int frequencia) ||
            !int.TryParse(OxigenacaoEntry.Text, out int oxigenacao))
        {
            await DisplayAlertAsync(
                "Atenção",
                "Confira os valores numéricos informados.",
                "OK");
            return;
        }

        string temperaturaTexto = TemperaturaEntry.Text.Trim()
            .Replace(',', '.');

        if (!decimal.TryParse(
                temperaturaTexto,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal temperatura))
        {
            await DisplayAlertAsync(
                "Atenção",
                "Informe uma temperatura válida, por exemplo, 36,5.",
                "OK");
            return;
        }

        if (sistolica <= 0 || diastolica <= 0 ||
            frequencia <= 0 || temperatura <= 0 ||
            oxigenacao <= 0 || oxigenacao > 100)
        {
            await DisplayAlertAsync(
                "Atenção",
                "Confira os valores informados. A oxigenação deve estar entre 1 e 100.",
                "OK");
            return;
        }

        int idUsuario = Preferences.Get("IdUsuario", 0);

        if (idUsuario <= 0)
        {
            await DisplayAlertAsync(
                "Sessão não identificada",
                "Não foi possível identificar o usuário. Saia do aplicativo e entre novamente.",
                "OK");
            return;
        }

        var dados = new
        {
            idUsuario,
            pressaoSistolica = sistolica,
            pressaoDiastolica = diastolica,
            frequenciaCardiaca = frequencia,
            temperatura,
            oxigenacao,
            observacoes = ObservacoesEditor.Text?.Trim()
        };

        try
        {
            using var resposta = await cliente.PostAsJsonAsync(
                EnderecoApi, dados);

            string conteudo = await resposta.Content.ReadAsStringAsync();
            string mensagem = "Não foi possível salvar a medição.";

            try
            {
                using var json = JsonDocument.Parse(conteudo);

                if (json.RootElement.TryGetProperty(
                        "mensagem", out var mensagemJson))
                {
                    mensagem = mensagemJson.GetString() ?? mensagem;
                }
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

                SistolicaEntry.Text = string.Empty;
                DiastolicaEntry.Text = string.Empty;
                FrequenciaEntry.Text = string.Empty;
                TemperaturaEntry.Text = string.Empty;
                OxigenacaoEntry.Text = string.Empty;
                ObservacoesEditor.Text = string.Empty;
            }
            else
            {
                await DisplayAlertAsync(
                    "Erro ao salvar",
                    mensagem,
                    "OK");
            }
        }
        catch (HttpRequestException)
        {
            await DisplayAlertAsync(
                "Erro de conexão",
                "Não foi possível acessar a API. Verifique se ela está em execução.",
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


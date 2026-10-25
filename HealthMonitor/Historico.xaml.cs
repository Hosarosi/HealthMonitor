
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace HealthMonitor;

public partial class Historico : ContentPage
{
    private static readonly HttpClient cliente = new HttpClient();

    private const string EnderecoApi =
        "https://localhost:7231/api/Medicao/historico/";

    private readonly ObservableCollection<MedicaoItem> medicoes = new();

    public Historico()
    {
        InitializeComponent();
        ListaMedicoes.ItemsSource = medicoes;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarHistoricoAsync();
    }

    private async void OnAtualizarClicked(object sender, EventArgs e)
    {
        await CarregarHistoricoAsync();
    }

    private async Task CarregarHistoricoAsync()
    {
        int idUsuario = Preferences.Get("IdUsuario", 0);

        if (idUsuario <= 0)
        {
            await DisplayAlertAsync(
                "Sessão não identificada",
                "Entre novamente no aplicativo para consultar o histórico.",
                "OK");
            return;
        }

        try
        {
            using var resposta = await cliente.GetAsync(
                EnderecoApi + idUsuario);

            if (!resposta.IsSuccessStatusCode)
            {
                await DisplayAlertAsync(
                    "Erro",
                    "Não foi possível carregar o histórico.",
                    "OK");
                return;
            }

            var resultado = await resposta.Content
                .ReadFromJsonAsync<List<MedicaoItem>>();

            medicoes.Clear();

            if (resultado != null)
            {
                foreach (var medicao in resultado)
                    medicoes.Add(medicao);
            }
        }
        catch (HttpRequestException)
        {
            await DisplayAlertAsync(
                "Erro de conexão",
                "Verifique se a API está em execução.",
                "OK");
        }
        catch (System.Text.Json.JsonException)
        {
            await DisplayAlertAsync(
                "Erro",
                "Não foi possível interpretar os dados do histórico.",
                "OK");
        }
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}

public class MedicaoItem
{
    [JsonPropertyName("idSinal")]
    public int IdSinal { get; set; }

    [JsonPropertyName("idUsuario")]
    public int IdUsuario { get; set; }

    [JsonPropertyName("pressaoSistolica")]
    public int PressaoSistolica { get; set; }

    [JsonPropertyName("pressaoDiastolica")]
    public int PressaoDiastolica { get; set; }

    [JsonPropertyName("frequenciaCardiaca")]
    public int FrequenciaCardiaca { get; set; }

    [JsonPropertyName("temperatura")]
    public decimal Temperatura { get; set; }

    [JsonPropertyName("oxigenacao")]
    public int Oxigenacao { get; set; }

    [JsonPropertyName("dataRegistro")]
    public DateTime DataRegistro { get; set; }

    [JsonPropertyName("observacoes")]
    public string? Observacoes { get; set; }

    public string DataFormatada =>
        DataRegistro.ToString("dd/MM/yyyy 'às' HH:mm");

    public string PressaoFormatada =>
        $"{PressaoSistolica}/{PressaoDiastolica} mmHg";

    public string FrequenciaFormatada =>
        $"{FrequenciaCardiaca} bpm";

    public string TemperaturaFormatada =>
        $"{Temperatura.ToString("F1", CultureInfo.GetCultureInfo("pt-BR"))} °C";

    public string OxigenacaoFormatada => $"{Oxigenacao}%";

    public bool TemObservacoes =>
        !string.IsNullOrWhiteSpace(Observacoes);

    public string ObservacoesFormatadas =>
        $"Observações: {Observacoes}";
}

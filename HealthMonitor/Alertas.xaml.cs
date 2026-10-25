
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace HealthMonitor;

public partial class Alertas : ContentPage
{
    private static readonly HttpClient cliente = new HttpClient();

    private const string EnderecoApi =
        "https://localhost:7231/api/Alerta/";

    private readonly ObservableCollection<AlertaItemApp> alertas = new();

    public Alertas()
    {
        InitializeComponent();
        ListaAlertas.ItemsSource = alertas;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAlertasAsync();
    }

    private async void OnAtualizarClicked(object sender, EventArgs e)
    {
        await CarregarAlertasAsync();
    }

    private async Task CarregarAlertasAsync()
    {
        int idUsuario = Preferences.Get("IdUsuario", 0);

        if (idUsuario <= 0)
        {
            await DisplayAlertAsync(
                "Sessão não identificada",
                "Entre novamente no aplicativo para consultar os alertas.",
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
                    "Não foi possível carregar os alertas.",
                    "OK");
                return;
            }

            var resultado = await resposta.Content
                .ReadFromJsonAsync<List<AlertaItemApp>>();

            alertas.Clear();

            if (resultado != null)
            {
                foreach (var alerta in resultado)
                    alertas.Add(alerta);
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
                "Não foi possível interpretar os dados dos alertas.",
                "OK");
        }
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}

public class AlertaItemApp
{
    [JsonPropertyName("idSinal")]
    public int IdSinal { get; set; }

    [JsonPropertyName("dataRegistro")]
    public DateTime DataRegistro { get; set; }

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = "";

    [JsonPropertyName("mensagem")]
    public string Mensagem { get; set; } = "";

    [JsonPropertyName("nivel")]
    public string Nivel { get; set; } = "";

    public string DataFormatada =>
        DataRegistro.ToString("dd/MM/yyyy 'às' HH:mm");
}

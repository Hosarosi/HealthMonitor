
using System.Net.Http.Json;

namespace HealthMonitor
{
    public partial class Resumo : ContentPage
    {
        private static readonly HttpClient cliente = new HttpClient();
        private const string EnderecoApi =
            "https://localhost:7231/api/Medicao/resumo/";

        public Resumo()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarResumoAsync();
        }

        private async void OnAtualizarClicked(object sender, EventArgs e)
        {
            await CarregarResumoAsync();
        }


        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }


        private async Task CarregarResumoAsync()
        {
            int idUsuario = Preferences.Get("IdUsuario", 0);

            if (idUsuario <= 0)
            {
                await DisplayAlertAsync(
                    "Atenção",
                    "Não foi possível identificar o usuário conectado. Faça login novamente.",
                    "OK");
                return;
            }

            try
            {
                var resumo = await cliente.GetFromJsonAsync<ResumoMedicoes>(
                    EnderecoApi + idUsuario);

                if (resumo == null)
                {
                    await DisplayAlertAsync(
                        "Aviso",
                        "Não foi possível carregar o resumo.",
                        "OK");
                    return;
                }

                TotalMedicoesLabel.Text = resumo.TotalMedicoes.ToString();

                DataUltimaLabel.Text = resumo.UltimaData.HasValue
                    ? resumo.UltimaData.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
                    : "Nenhuma medição registrada";

                PressaoLabel.Text =
                    resumo.UltimaSistolica.HasValue && resumo.UltimaDiastolica.HasValue
                    ? $"{resumo.UltimaSistolica}/{resumo.UltimaDiastolica} mmHg"
                    : "--/-- mmHg";

                FrequenciaLabel.Text = resumo.UltimaFrequencia.HasValue
                    ? $"{resumo.UltimaFrequencia} bpm"
                    : "-- bpm";

                TemperaturaLabel.Text = resumo.UltimaTemperatura.HasValue
                    ? $"{resumo.UltimaTemperatura.Value:F1} °C"
                    : "-- °C";

                OxigenacaoLabel.Text = resumo.UltimaOxigenacao.HasValue
                    ? $"{resumo.UltimaOxigenacao}%"
                    : "--%";
            }
            catch (HttpRequestException)
            {
                await DisplayAlertAsync(
                    "Erro de conexão",
                    "Não foi possível acessar a API. Confira se ela está em execução.",
                    "OK");
            }
            catch (System.Text.Json.JsonException)
            {
                await DisplayAlertAsync(
                    "Erro",
                    "Não foi possível interpretar os dados do resumo.",
                    "OK");
            }
        }
    }

    public class ResumoMedicoes
    {
        public int TotalMedicoes { get; set; }
        public DateTime? UltimaData { get; set; }
        public int? UltimaSistolica { get; set; }
        public int? UltimaDiastolica { get; set; }
        public int? UltimaFrequencia { get; set; }
        public decimal? UltimaTemperatura { get; set; }
        public int? UltimaOxigenacao { get; set; }
    }
}

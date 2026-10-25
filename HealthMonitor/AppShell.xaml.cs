
namespace HealthMonitor
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(CadastroPage), typeof(CadastroPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(MedicaoPage), typeof(MedicaoPage));
            Routing.RegisterRoute(nameof(Historico),typeof(Historico));
            Routing.RegisterRoute(nameof(Alertas), typeof(Alertas));
            Routing.RegisterRoute(nameof(Resumo), typeof(Resumo));
        }
    }
}

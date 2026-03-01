using EcoTurismo.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EcoTurismo.Api.BackgroundServices;

/// <summary>
/// Job que reconcilia a ocupação dos atrativos periodicamente
/// para corrigir eventuais inconsistências (drift)
/// </summary>
public class ReconciliacaoOcupacaoJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReconciliacaoOcupacaoJob> _logger;
    private readonly TimeSpan _intervalo = TimeSpan.FromHours(1); // Roda a cada hora

    public ReconciliacaoOcupacaoJob(
        IServiceProvider serviceProvider,
        ILogger<ReconciliacaoOcupacaoJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReconciliacaoOcupacaoJob iniciado");

        // Aguarda 5 minutos após o startup para dar tempo do sistema estabilizar
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Iniciando reconciliação de ocupações");

                using var scope = _serviceProvider.CreateScope();
                var ocupacaoService = scope.ServiceProvider.GetRequiredService<IOcupacaoService>();

                await ocupacaoService.ReconciliarOcupacoesAsync(stoppingToken);

                _logger.LogInformation("Reconciliação concluída com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante reconciliação de ocupações");
            }

            // Aguarda o intervalo antes da próxima execução
            await Task.Delay(_intervalo, stoppingToken);
        }

        _logger.LogInformation("ReconciliacaoOcupacaoJob parado");
    }
}

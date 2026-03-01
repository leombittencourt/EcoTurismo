namespace EcoTurismo.Application.DTOs;
public record DashboardDto(
    int VisitantesHoje,
    string VisitantesTendencia,
    double PermanenciaMedia,
    double OcupacaoMedia,
    string PressaoTuristica,
    List<DataPointDto> VisitantesPorDia,
    List<OcupacaoBalnearioDto> OcupacaoPorBalneario,
    List<OrigemUfDto> OrigemPorUF,
    List<DataPointDto> EvolucaoMensal,
    List<TopAtrativoDto> TopAtrativos
);
public record DataPointDto(string Label, int Valor);
public record OcupacaoBalnearioDto(string Nome, int Ocupacao, int Capacidade);
public record OrigemUfDto(string Uf, int Quantidade);
public record TopAtrativoDto(string Nome, int Visitantes, string Tendencia);


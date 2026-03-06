namespace EcoTurismo.Application.DTOs;

/// <summary>
/// Classe base para requisições paginadas
/// </summary>
public class PagedRequest
{
    private int _page = 1;
    private int _pageSize = 10;
    
    /// <summary>
    /// Número da página (começa em 1)
    /// </summary>
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Quantidade de itens por página (mínimo 1, máximo 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 1,
            > 100 => 100,
            _ => value
        };
    }

    /// <summary>
    /// Calcula quantos registros pular (para Skip no LINQ)
    /// </summary>
    public int Skip => (Page - 1) * PageSize;
}

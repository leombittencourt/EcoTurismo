namespace EcoTurismo.Api.Endpoints.Banners;

public class ReorderBannersRequest
{
    public List<ReorderItem> Itens { get; set; } = [];
}

public class ReorderItem
{
    public Guid Id { get; set; }
    public int Ordem { get; set; }
}

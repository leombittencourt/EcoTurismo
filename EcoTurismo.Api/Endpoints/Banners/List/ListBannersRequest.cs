using FastEndpoints;

namespace EcoTurismo.Api.Endpoints.Banners;

public class ListBannersRequest
{
    [QueryParam]
    public bool? ApenasAtivos { get; set; }
}

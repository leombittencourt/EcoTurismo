using Microsoft.AspNetCore.Authorization;

namespace EcoTurismo.Api.Authorization;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(params string[] permissions)
    {
        Policy = string.Join(",", permissions);
    }
}

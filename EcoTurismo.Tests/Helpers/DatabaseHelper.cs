using EcoTurismo.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Tests.Helpers;

public static class DatabaseHelper
{
    public static EcoTurismoDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EcoTurismoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new EcoTurismoDbContext(options);
    }
}

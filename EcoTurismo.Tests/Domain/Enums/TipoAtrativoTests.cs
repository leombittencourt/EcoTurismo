using EcoTurismo.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace EcoTurismo.Tests.Domain.Enums;

public class TipoAtrativoTests
{
    [Theory]
    [InlineData(TipoAtrativo.Balneario, "balneario")]
    [InlineData(TipoAtrativo.Cachoeira, "cachoeira")]
    [InlineData(TipoAtrativo.Trilha, "trilha")]
    [InlineData(TipoAtrativo.Parque, "parque")]
    [InlineData(TipoAtrativo.FazendaEcoturismo, "fazenda_ecoturismo")]
    public void ToStringValue_Should_Return_Correct_String(TipoAtrativo tipo, string expected)
    {
        // Act
        var result = tipo.ToStringValue();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("balneario", TipoAtrativo.Balneario)]
    [InlineData("balneário", TipoAtrativo.Balneario)]
    [InlineData("BALNEARIO", TipoAtrativo.Balneario)]
    [InlineData("cachoeira", TipoAtrativo.Cachoeira)]
    [InlineData("trilha", TipoAtrativo.Trilha)]
    [InlineData("parque", TipoAtrativo.Parque)]
    [InlineData("fazenda_ecoturismo", TipoAtrativo.FazendaEcoturismo)]
    [InlineData("fazenda ecoturismo", TipoAtrativo.FazendaEcoturismo)]
    [InlineData("fazendaecoturismo", TipoAtrativo.FazendaEcoturismo)]
    public void FromString_Should_Parse_Correctly(string input, TipoAtrativo expected)
    {
        // Act
        var result = TipoAtrativoExtensions.FromString(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(TipoAtrativo.Balneario, "Balneário")]
    [InlineData(TipoAtrativo.Cachoeira, "Cachoeira")]
    [InlineData(TipoAtrativo.Trilha, "Trilha")]
    [InlineData(TipoAtrativo.Parque, "Parque")]
    [InlineData(TipoAtrativo.FazendaEcoturismo, "Fazenda Ecoturismo")]
    public void ToDescricao_Should_Return_Correct_Description(TipoAtrativo tipo, string expected)
    {
        // Act
        var result = tipo.ToDescricao();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void FromString_With_Invalid_String_Should_Return_Default()
    {
        // Arrange
        var invalidInput = "tipo_invalido";

        // Act
        var result = TipoAtrativoExtensions.FromString(invalidInput);

        // Assert
        result.Should().Be(TipoAtrativo.Balneario); // Default
    }
}

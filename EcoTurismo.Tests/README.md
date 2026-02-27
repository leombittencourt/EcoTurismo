# Testes Unitários - EcoTurismo API

## Visão Geral

Suite completa de testes unitários para garantir a qualidade e confiabilidade do sistema.

## Estrutura dos Testes

```
EcoTurismo.Tests/
├── Helpers/
│   ├── DatabaseHelper.cs      # Criação de contexto em memória
│   └── TestDataBuilder.cs     # Builders para entidades de teste
├── Services/
│   ├── UsuarioServiceTests.cs      # 13 testes
│   ├── AuthServiceTests.cs         # 4 testes
│   └── PermissionServiceTests.cs   # 9 testes
└── EcoTurismo.Tests.csproj
```

## Tecnologias Utilizadas

- **xUnit**: Framework de testes
- **FluentAssertions**: Assertions fluentes e legíveis
- **Moq**: Mocking de dependências
- **InMemoryDatabase**: EF Core em memória para testes
- **FastEndpoints.Testing**: Testes de endpoints

## Executar Testes

### Via CLI
```bash
# Executar todos os testes
dotnet test

# Executar com detalhes
dotnet test --logger "console;verbosity=detailed"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~UsuarioServiceTests"

# Gerar relatório de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Via Visual Studio
- **Test Explorer**: View → Test Explorer
- **Run All Tests**: Ctrl+R, A
- **Debug Test**: Clique direito → Debug Test

## Cobertura de Testes

### UsuarioServiceTests (13 testes)

#### ✅ CriarAsync
- `CriarAsync_DeveCrearUsuarioComSucesso`
- `CriarAsync_DeveLancarExcecaoSeEmailJaExiste`
- `CriarAsync_DeveLancarExcecaoSeRoleNaoExiste`

#### ✅ ListarAsync
- `ListarAsync_DeveRetornarApenasUsuariosAtivos`

#### ✅ ObterPorIdAsync
- `ObterPorIdAsync_DeveRetornarUsuarioQuandoExiste`
- `ObterPorIdAsync_DeveRetornarNullQuandoNaoExiste`

#### ✅ AtualizarAsync
- `AtualizarAsync_DeveAtualizarCamposFornecidos`
- `AtualizarAsync_DeveAtualizarSenhaSeForFornecida`
- `AtualizarAsync_DeveLancarExcecaoSeEmailJaExiste`
- `AtualizarAsync_DeveRetornarNullSeUsuarioNaoExiste`

#### ✅ ExcluirAsync
- `ExcluirAsync_DeveExcluirUsuarioQuandoExiste`
- `ExcluirAsync_DeveRetornarFalseQuandoUsuarioNaoExiste`

### AuthServiceTests (4 testes)

- `LoginAsync_DeveRetornarTokenQuandoCredenciaisValidas`
- `LoginAsync_DeveRetornarNullQuandoEmailNaoExiste`
- `LoginAsync_DeveRetornarNullQuandoSenhaIncorreta`
- `LoginAsync_DeveIncluirPermissoesNoToken`

### PermissionServiceTests (9 testes)

- `GetPermissionsByRoleIdAsync_DeveRetornarPermissoesCorretas`
- `GetPermissionsByRoleNameAsync_DeveRetornarPermissoesCorretas`
- `HasPermissionAsync_DeveRetornarTrueQuandoTemPermissao`
- `HasPermissionAsync_DeveRetornarFalseQuandoNaoTemPermissao`
- `HasAnyPermissionAsync_DeveRetornarTrueQuandoTemQualquerUma`
- `GetRoleByNameAsync_DeveRetornarRoleQuandoExiste`
- `GetRoleByIdAsync_DeveRetornarRoleQuandoExiste`
- `GetPermissionsByRoleIdAsync_DeveFazerCacheDasPermissoes`

## Padrão AAA

Todos os testes seguem o padrão **Arrange-Act-Assert**:

```csharp
[Fact]
public async Task MetodoTeste_DeveComportamentoEsperado()
{
    // Arrange (Preparar)
    // Configurar dados, mocks e dependências
    
    // Act (Agir)
    // Executar o método sendo testado
    
    // Assert (Verificar)
    // Validar o resultado esperado
}
```

## Helpers

### DatabaseHelper

Cria contextos do EF Core em memória para testes:

```csharp
using var context = DatabaseHelper.CreateInMemoryContext();
```

### TestDataBuilder

Cria entidades de teste com dados válidos:

```csharp
var role = TestDataBuilder.CreateRole("Admin");
var usuario = TestDataBuilder.CreateUsuario(
    nome: "João Silva",
    email: "joao@example.com",
    roleId: role.Id
);
var municipio = TestDataBuilder.CreateMunicipio("Bonito", "MS");
```

## Exemplos de Uso

### Teste Simples

```csharp
[Fact]
public async Task Exemplo_TesteSimples()
{
    // Arrange
    using var context = DatabaseHelper.CreateInMemoryContext();
    var service = new UsuarioService(context);
    
    // Act
    var result = await service.ListarAsync();
    
    // Assert
    result.Should().BeEmpty();
}
```

### Teste com Mock

```csharp
[Fact]
public async Task Exemplo_ComMock()
{
    // Arrange
    var mockService = new Mock<IPermissionService>();
    mockService
        .Setup(x => x.HasPermissionAsync(It.IsAny<Guid>(), "banners:read"))
        .ReturnsAsync(true);
    
    // Act
    var result = await mockService.Object.HasPermissionAsync(Guid.NewGuid(), "banners:read");
    
    // Assert
    result.Should().BeTrue();
    mockService.Verify(x => x.HasPermissionAsync(It.IsAny<Guid>(), "banners:read"), Times.Once);
}
```

### Teste com Dados

```csharp
[Fact]
public async Task Exemplo_ComDados()
{
    // Arrange
    using var context = DatabaseHelper.CreateInMemoryContext();
    var role = TestDataBuilder.CreateRole();
    await context.Roles.AddAsync(role);
    await context.SaveChangesAsync();
    
    // Act
    var roleNoBanco = await context.Roles.FindAsync(role.Id);
    
    // Assert
    roleNoBanco.Should().NotBeNull();
    roleNoBanco!.Name.Should().Be(role.Name);
}
```

## FluentAssertions

Exemplos de assertions:

```csharp
// Valores
result.Should().NotBeNull();
result.Should().Be(expected);
result.Should().BeGreaterThan(0);

// Strings
result.Should().NotBeNullOrEmpty();
result.Should().Contain("texto");
result.Should().StartWith("prefix");

// Coleções
list.Should().HaveCount(5);
list.Should().Contain(item);
list.Should().BeEmpty();
list.Should().ContainSingle();

// Exceções
await Assert.ThrowsAsync<InvalidOperationException>(
    async () => await service.MetodoQueJogaExcecao());

// Objetos
result.Should().BeEquivalentTo(expected);
result.Should().NotBeNull();
```

## Boas Práticas

### ✅ DO

- Nomear testes de forma descritiva: `MetodoTeste_DeveComportamento_QuandoCondicao`
- Um assert por teste (quando possível)
- Usar `using` com contextos de banco
- Isolar testes (sem dependências entre eles)
- Testar casos de sucesso e falha
- Usar dados realistas

### ❌ DON'T

- Testes que dependem de ordem de execução
- Testes que compartilham estado
- Testes sem assertions
- Testes muito longos ou complexos
- Dados hardcoded sem sentido
- Esquecer de fazer dispose de recursos

## Adicionar Novos Testes

### 1. Criar classe de teste

```csharp
public class NovoServiceTests
{
    [Fact]
    public async Task MetodoTeste_DeveComportamento()
    {
        // Seu teste aqui
    }
}
```

### 2. Adicionar à suite

Os testes são descobertos automaticamente pelo xUnit.

### 3. Executar

```bash
dotnet test
```

## Integração Contínua

### GitHub Actions

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --verbosity normal
```

## Cobertura de Código

### Gerar Relatório

```bash
# Instalar ferramenta
dotnet tool install --global dotnet-reportgenerator-globaltool

# Gerar cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Gerar relatório HTML
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report

# Abrir relatório
start coverage-report/index.html
```

## Troubleshooting

### Erro: Banco em memória não limpa

**Solução:** Use `Guid.NewGuid()` no nome do banco:
```csharp
.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
```

### Erro: Testes falhando aleatoriamente

**Solução:** Verifique se não há estado compartilhado entre testes.

### Erro: Mock não está sendo chamado

**Solução:** Verifique o setup do mock:
```csharp
mockService.Verify(x => x.Metodo(), Times.Once);
```

## Estatísticas

- **Total de Testes**: 26
- **Classes de Teste**: 3
- **Helpers**: 2
- **Cobertura Esperada**: >80%

## Roadmap

### Próximos Testes

- [ ] ReservaServiceTests
- [ ] QuiosqueServiceTests
- [ ] Testes de Integração com FastEndpoints
- [ ] Testes de Controllers
- [ ] Testes de Validators

## Referências

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [EF Core Testing](https://docs.microsoft.com/en-us/ef/core/testing/)

## Checklist

- [x] Projeto de testes criado
- [x] Dependências adicionadas
- [x] Helpers implementados
- [x] UsuarioServiceTests (13 testes)
- [x] AuthServiceTests (4 testes)
- [x] PermissionServiceTests (9 testes)
- [x] README de testes
- [ ] Testes executados com sucesso
- [ ] Cobertura >80%
- [ ] CI/CD configurado

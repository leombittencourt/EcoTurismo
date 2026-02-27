# Inicialização Automática do Sistema

## Como Funciona

O sistema EcoTurismo está configurado para executar automaticamente a configuração inicial do banco de dados sempre que a aplicação inicia.

## Fluxo de Inicialização

### 1. Startup da Aplicação

Quando você executa:
```bash
dotnet run --project EcoTurismo.Api
```

### 2. Processo Automático

O `Program.cs` executa as seguintes etapas em ordem:

#### a) Aplicar Migrations
```
Iniciando configuração do banco de dados...
Aplicando migrations pendentes...
Migrations aplicadas com sucesso!
```

#### b) Executar Seed
```
Executando seed de dados iniciais...
🌱 Iniciando seed de dados iniciais...
   ✅ 4 Roles criadas
   ✅ 26 Permissions criadas
   ✅ 55 RolePermissions associadas
   ✅ Município base criado: Rio Verde de Mato Grosso - MS
   ✅ 4 Usuários default criados (senha: admin123)
🎉 Seed concluído com sucesso!

📋 Credenciais de acesso:
   👨‍💼 Admin:       admin@ecoturismo.com.br / admin123
   🏛️  Prefeitura:  prefeitura@ecoturismo.com.br / admin123
   🏖️  Balneário:   balneario@ecoturismo.com.br / admin123
   👤 Público:     publico@ecoturismo.com.br / admin123

✅ Banco de dados configurado e pronto para uso!
```

#### c) Aplicação Disponível
```
Now listening on: https://localhost:5001
Application started. Press Ctrl+C to shut down.
```

## Comportamento do Seed

### Primeira Execução
- ✅ Cria todas as tabelas (migrations)
- ✅ Popula dados iniciais (roles, permissions, município, usuários)
- ✅ Exibe credenciais de acesso

### Execuções Subsequentes
- ✅ Verifica migrations pendentes
- ℹ️ Pula o seed se os dados já existem
```
ℹ️  Seed ignorado: Dados já existem no banco.
```

## Código do Program.cs

```csharp
var app = builder.Build();

// ── Inicialização do Banco de Dados ──
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Iniciando configuração do banco de dados...");
        
        var db = services.GetRequiredService<EcoTurismoDbContext>();

        // Executar migrations automaticamente
        logger.LogInformation("Aplicando migrations pendentes...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Migrations aplicadas com sucesso!");

        // Executar seed de dados iniciais
        logger.LogInformation("Executando seed de dados iniciais...");
        await EcoTurismo.Infra.Data.Seeds.AuthorizationSeed.SeedAsync(db);
        logger.LogInformation("Seed executado com sucesso!");
        
        logger.LogInformation("✅ Banco de dados configurado e pronto para uso!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Erro ao configurar o banco de dados. Detalhes: {Message}", ex.Message);
        throw; // Re-throw para impedir a inicialização com erro
    }
}
```

## Vantagens

### ✅ Zero Configuração Manual
- Não precisa rodar migrations manualmente
- Não precisa executar scripts SQL
- Dados iniciais criados automaticamente

### ✅ Ambiente Sempre Pronto
- Desenvolvimento: Dados de teste prontos
- Staging: Estrutura criada automaticamente
- Produção: Migrations aplicadas no deploy

### ✅ Segurança
- Se houver erro, a aplicação não inicia
- Logs detalhados de cada etapa
- Rollback automático em caso de falha

## Logs Detalhados

### Sucesso
```
[11:30:45 INF] Iniciando configuração do banco de dados...
[11:30:45 INF] Aplicando migrations pendentes...
[11:30:47 INF] Migrations aplicadas com sucesso!
[11:30:47 INF] Executando seed de dados iniciais...
🌱 Iniciando seed de dados iniciais...
   ✅ 4 Roles criadas
   ✅ 26 Permissions criadas
   ✅ 55 RolePermissions associadas
   ✅ Município base criado: Rio Verde de Mato Grosso - MS
   ✅ 4 Usuários default criados (senha: admin123)
🎉 Seed concluído com sucesso!
[11:30:48 INF] Seed executado com sucesso!
[11:30:48 INF] ✅ Banco de dados configurado e pronto para uso!
```

### Seed Já Executado
```
[11:35:20 INF] Iniciando configuração do banco de dados...
[11:35:20 INF] Aplicando migrations pendentes...
[11:35:20 INF] Migrations aplicadas com sucesso!
[11:35:20 INF] Executando seed de dados iniciais...
ℹ️  Seed ignorado: Dados já existem no banco.
[11:35:20 INF] Seed executado com sucesso!
[11:35:20 INF] ✅ Banco de dados configurado e pronto para uso!
```

### Erro
```
[11:40:15 ERR] ❌ Erro ao configurar o banco de dados. Detalhes: Connection refused
System.Exception: Connection refused
   at Npgsql.NpgsqlConnection.Open()
   ...
```

## Desabilitar Seed Automático

Se você quiser desabilitar o seed automático (ex: produção com dados reais):

### Opção 1: Comentar no Program.cs
```csharp
// await EcoTurismo.Infra.Data.Seeds.AuthorizationSeed.SeedAsync(db);
```

### Opção 2: Variável de Ambiente
Adicione no `Program.cs`:
```csharp
// Só executar seed em desenvolvimento
if (app.Environment.IsDevelopment())
{
    await EcoTurismo.Infra.Data.Seeds.AuthorizationSeed.SeedAsync(db);
}
```

### Opção 3: Configuração
Adicione no `appsettings.json`:
```json
{
  "Database": {
    "RunSeedOnStartup": true
  }
}
```

E use:
```csharp
if (builder.Configuration.GetValue<bool>("Database:RunSeedOnStartup"))
{
    await EcoTurismo.Infra.Data.Seeds.AuthorizationSeed.SeedAsync(db);
}
```

## Troubleshooting

### Seed não executa

**Problema:** Nenhuma mensagem de seed aparece

**Soluções:**
1. Verifique se o banco de dados está acessível
2. Verifique a connection string no `appsettings.json`
3. Verifique os logs de erro

### Erro de conexão

**Problema:** `Connection refused` ou similar

**Soluções:**
1. Verifique se o PostgreSQL está rodando
2. Verifique a connection string
3. Verifique firewall/rede

### Dados duplicados

**Problema:** Erro de chave duplicada

**Solução:**
O seed verifica se já existem dados. Se ainda assim ocorrer erro:
```sql
-- Limpar TUDO (CUIDADO!)
DELETE FROM "RolePermissions";
DELETE FROM "Permissions";
DELETE FROM "Usuarios";
DELETE FROM "Municipios";
DELETE FROM "Roles";
```

### Migration pendente

**Problema:** Estrutura de tabela incorreta

**Solução:**
```bash
# Resetar banco (CUIDADO!)
dotnet ef database drop --project EcoTurismo.Infra --startup-project EcoTurismo.Api
dotnet run --project EcoTurismo.Api
```

## Ordem de Criação

1. ✅ Migrations (estrutura do banco)
2. ✅ Roles
3. ✅ Permissions
4. ✅ RolePermissions
5. ✅ Município Base
6. ✅ Usuários Default

## Verificação Manual

Se quiser verificar se o seed foi executado:

```sql
-- Contar registros
SELECT 
    (SELECT COUNT(*) FROM "Roles") as Roles,
    (SELECT COUNT(*) FROM "Permissions") as Permissions,
    (SELECT COUNT(*) FROM "RolePermissions") as RolePermissions,
    (SELECT COUNT(*) FROM "Municipios") as Municipios,
    (SELECT COUNT(*) FROM "Usuarios") as Usuarios;
```

**Resultado esperado:**
- Roles: 4
- Permissions: 26
- RolePermissions: 55
- Municipios: 1
- Usuarios: 4

## Testando

### 1. Primeira execução (banco vazio)
```bash
dotnet run --project EcoTurismo.Api
```
Deve exibir todas as mensagens de seed.

### 2. Segunda execução (banco populado)
```bash
dotnet run --project EcoTurismo.Api
```
Deve exibir: `ℹ️  Seed ignorado: Dados já existem no banco.`

### 3. Testar login
```bash
POST /api/auth/login
{
  "email": "admin@ecoturismo.com.br",
  "password": "admin123"
}
```

## Checklist

- [x] Migrations automáticas configuradas
- [x] Seed automático configurado
- [x] Logs informativos adicionados
- [x] Tratamento de erros implementado
- [x] Verificação de dados existentes
- [x] Credenciais exibidas após seed
- [x] Compilação bem-sucedida
- [ ] Testado em ambiente local
- [ ] Testado em ambiente de staging
- [ ] Documentação atualizada

## Resumo

✅ **Tudo automático!**
- Migrations aplicadas na inicialização
- Seed executado automaticamente
- Dados de teste prontos
- Zero configuração manual necessária

Basta executar `dotnet run` e começar a usar! 🚀

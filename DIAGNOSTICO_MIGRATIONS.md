# 🔍 Diagnóstico de Migrations

## Problema Reportado

As migrations não estão sendo aplicadas com `await db.Database.MigrateAsync()`.

## 🧪 Testes para Executar

### 1. Verificar Connection String

Execute no terminal:
```bash
# Ver a connection string que está sendo usada
dotnet user-secrets list --project EcoTurismo.Api

# Ou ver no appsettings
cat EcoTurismo.Api/appsettings.Development.json | grep ConnectionStrings -A 2
```

### 2. Testar Conexão com o Banco

```bash
# Se estiver usando Docker local
docker ps | grep postgres

# Testar conexão diretamente
psql -h localhost -U postgres -d ecoturismo -c "SELECT version();"
```

### 3. Listar Migrations Disponíveis

```bash
cd EcoTurismo.Api
dotnet ef migrations list --project ../EcoTurismo.Infra
```

**Resultado esperado:**
```
...
20260228222439_RemoveDuplicateUsuarioColumns (Pending)
```

### 4. Aplicar Migrations Manualmente

```bash
# Forçar aplicação das migrations
dotnet ef database update --project EcoTurismo.Infra --startup-project EcoTurismo.Api --verbose
```

### 5. Verificar no Banco

```sql
-- Ver tabela de controle do EF Core
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";

-- Ver últimas migrations aplicadas
SELECT "MigrationId", "ProductVersion" 
FROM "__EFMigrationsHistory" 
ORDER BY "MigrationId" DESC 
LIMIT 5;
```

## 🔍 Possíveis Causas

### Causa 1: Connection String Incorreta

**Sintoma:** Erro ao conectar ou timeout

**Verificar:**
```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ecoturismo;Username=postgres;Password=postgres"
  }
}
```

**Solução:**
```bash
# Testar conexão
psql "Host=localhost;Port=5432;Database=ecoturismo;Username=postgres;Password=postgres"
```

### Causa 2: PostgreSQL Não Está Rodando

**Verificar:**
```bash
# Docker
docker ps | grep postgres

# Se não estiver rodando
docker-compose up -d postgres

# Ou
docker run -d --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:15
```

### Causa 3: Banco de Dados Não Existe

**Verificar:**
```bash
psql -h localhost -U postgres -c "\l" | grep ecoturismo
```

**Solução:**
```bash
# Criar banco manualmente
psql -h localhost -U postgres -c "CREATE DATABASE ecoturismo;"

# Ou deixar o EF Core criar
dotnet ef database update --project EcoTurismo.Infra --startup-project EcoTurismo.Api
```

### Causa 4: Migrations Não Foram Criadas

**Verificar:**
```bash
ls EcoTurismo.Infra/Migrations/
```

**Solução:**
```bash
# Se não houver migrations, criar uma inicial
dotnet ef migrations add InitialCreate --project EcoTurismo.Infra --startup-project EcoTurismo.Api
```

### Causa 5: Erro Silencioso no Try-Catch

**Logs Atualizados** no `Program.cs` agora mostram:
- ✅ Status da conexão
- ✅ Migrations aplicadas
- ✅ Migrations pendentes
- ✅ Detalhes de exceções

**Ver logs ao iniciar:**
```bash
dotnet run --project EcoTurismo.Api
```

### Causa 6: DbContext Não Configurado

**Verificar arquivo:**
```bash
cat EcoTurismo.Infra/Data/EcoTurismoDbContext.cs | grep OnConfiguring
```

**Não deve ter `OnConfiguring` hardcoded**, deve usar DI do `Program.cs`.

## 🎯 Checklist de Diagnóstico

Execute na ordem:

- [ ] 1. PostgreSQL está rodando? `docker ps | grep postgres`
- [ ] 2. Banco `ecoturismo` existe? `psql -l | grep ecoturismo`
- [ ] 3. Connection string correta? Ver `appsettings.Development.json`
- [ ] 4. Conexão funciona? `psql -h localhost -U postgres -d ecoturismo`
- [ ] 5. Migrations existem? `dotnet ef migrations list`
- [ ] 6. Tabela `__EFMigrationsHistory` existe? `psql -c "\dt"`
- [ ] 7. Aplicar manualmente: `dotnet ef database update --verbose`
- [ ] 8. Ver logs detalhados ao iniciar aplicação

## 📊 Logs Esperados (Após Atualização)

Ao iniciar a aplicação, você deve ver:

```
info: EcoTurismo.Api.Program[0]
      Iniciando configuração do banco de dados...
info: EcoTurismo.Api.Program[0]
      Testando conexão com o banco de dados...
info: EcoTurismo.Api.Program[0]
      Conexão com banco: ✅ OK
info: EcoTurismo.Api.Program[0]
      Migrations aplicadas: 5
info: EcoTurismo.Api.Program[0]
      Migrations pendentes: 1
info: EcoTurismo.Api.Program[0]
      📋 Migrations pendentes:
info: EcoTurismo.Api.Program[0]
        - 20260228222439_RemoveDuplicateUsuarioColumns
info: EcoTurismo.Api.Program[0]
      Aplicando migrations pendentes...
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (50ms) [...]
      ALTER TABLE "Usuarios" DROP COLUMN "MunicipioId1"
info: EcoTurismo.Api.Program[0]
      Migrations aplicadas com sucesso!
info: EcoTurismo.Api.Program[0]
      ✅ Todas as migrations foram aplicadas!
```

## 🆘 Se Nada Funcionar

### Reset Completo do Banco

```bash
# 1. Parar aplicação
Ctrl+C

# 2. Dropar banco (cuidado!)
psql -h localhost -U postgres -c "DROP DATABASE IF EXISTS ecoturismo;"

# 3. Criar banco novamente
psql -h localhost -U postgres -c "CREATE DATABASE ecoturismo;"

# 4. Aplicar todas as migrations
dotnet ef database update --project EcoTurismo.Infra --startup-project EcoTurismo.Api --verbose

# 5. Iniciar aplicação
dotnet run --project EcoTurismo.Api
```

### Aplicar Migrations Via SQL Direto

```bash
# Gerar script SQL das migrations
dotnet ef migrations script --project EcoTurismo.Infra --startup-project EcoTurismo.Api --output migrations.sql

# Aplicar manualmente
psql -h localhost -U postgres -d ecoturismo -f migrations.sql
```

## 📝 Próximo Passo

**REINICIE A APLICAÇÃO** para ver os novos logs:

```bash
dotnet run --project EcoTurismo.Api
```

Os logs detalhados dirão exatamente onde está o problema!

---

**Compartilhe os logs que aparecerem para eu poder ajudar melhor!** 🔍

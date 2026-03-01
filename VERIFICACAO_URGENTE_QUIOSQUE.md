# 🔍 Verificação Urgente - Status do Quiosque

## ⚠️ Problema Atual

O status do quiosque não está sendo modificado, mesmo o código encontrando o quiosque corretamente.

## 🧪 Verificações Imediatas

### 1. Verificar Tipo da Coluna Status no Banco

```sql
-- Ver tipo atual da coluna
SELECT 
    column_name,
    data_type,
    character_maximum_length
FROM information_schema.columns
WHERE table_name = 'Quiosques' 
  AND column_name = 'Status';

-- ESPERADO: data_type = 'integer'
-- SE FOR 'character varying': A migration NÃO foi aplicada!
```

### 2. Verificar Migrations Aplicadas

```sql
-- Ver última migration aplicada
SELECT "MigrationId", "ProductVersion"
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId" DESC
LIMIT 1;

-- DEVE incluir: 20260228224221_ChangeQuiosqueStatusToInteger
```

### 3. Testar UPDATE Manual

```sql
-- Teste 1: Atualizar com inteiro
UPDATE "Quiosques" 
SET "Status" = 2 
WHERE "Numero" = 1
RETURNING "Id", "Numero", "Status";

-- Teste 2: Ver resultado
SELECT "Id", "Numero", "Status", "UpdatedAt"
FROM "Quiosques"
WHERE "Numero" = 1;
```

## 🎯 Logs Adicionados

O código agora imprime logs detalhados ao criar reserva:

```
🔍 Buscando quiosque {guid}...
📍 Quiosque encontrado - ID: {guid}, Número: 1
📊 Status ANTES: Disponivel (1)
📊 Status DEPOIS: Ocupado (2)
🔄 Entity State: Modified
💾 SaveChanges retornou: 1 mudança(s)
✅ Verificação: Status no banco = Ocupado, UpdatedAt = ...
```

## 📋 Checklist Diagnóstico

Execute NA ORDEM:

### Passo 1: Verificar Migration
```bash
dotnet ef migrations list --project EcoTurismo.Infra --startup-project EcoTurismo.Api

# DEVE mostrar (Applied):
# 20260228224221_ChangeQuiosqueStatusToInteger
```

### Passo 2: Aplicar Migration (se não foi aplicada)
```bash
dotnet ef database update --project EcoTurismo.Infra --startup-project EcoTurismo.Api --verbose
```

### Passo 3: Verificar no Banco
```sql
\d "Quiosques"

-- Coluna Status DEVE ser: integer
-- Se for character varying(15): Migration não foi aplicada!
```

### Passo 4: Reiniciar Aplicação
```bash
# Parar aplicação
Ctrl+C

# Limpar e rebuild
dotnet clean
dotnet build

# Iniciar
dotnet run --project EcoTurismo.Api
```

### Passo 5: Criar Reserva e Ver Logs
```bash
POST /api/reservas
{
  "quiosqueId": "guid",
  "data": "2026-03-01"
}

# OBSERVAR OS LOGS NO CONSOLE!
```

## 🚨 Se Status Ainda É VARCHAR

A migration **NÃO foi aplicada**. Aplicar manualmente:

```bash
# Aplicar com logs detalhados
dotnet ef database update --project EcoTurismo.Infra --startup-project EcoTurismo.Api --verbose

# Deve mostrar:
# Applying migration '20260228224221_ChangeQuiosqueStatusToInteger'
# Executed DbCommand (...)
# ALTER TABLE "Quiosques" ADD "StatusTemp" integer
# UPDATE "Quiosques" SET "StatusTemp" = CASE ...
# ALTER TABLE "Quiosques" DROP COLUMN "Status"
# ALTER TABLE "Quiosques" RENAME COLUMN "StatusTemp" TO "Status"
```

## 🔧 Solução Alternativa (Se migration falhar)

```sql
-- Backup
CREATE TABLE "Quiosques_Backup" AS 
SELECT * FROM "Quiosques";

-- Adicionar coluna temporária
ALTER TABLE "Quiosques" ADD COLUMN "StatusTemp" INTEGER DEFAULT 1;

-- Converter valores
UPDATE "Quiosques" 
SET "StatusTemp" = 
    CASE LOWER("Status")
        WHEN 'disponivel' THEN 1
        WHEN 'ocupado' THEN 2
        WHEN 'manutencao' THEN 3
        WHEN 'bloqueado' THEN 4
        WHEN 'inativo' THEN 5
        ELSE 1
    END;

-- Remover coluna antiga
ALTER TABLE "Quiosques" DROP COLUMN "Status";

-- Renomear
ALTER TABLE "Quiosques" RENAME COLUMN "StatusTemp" TO "Status";

-- Verificar
SELECT "Numero", "Status" FROM "Quiosques";
```

## 📊 Interpretação dos Logs

### ✅ Sucesso:
```
📍 Quiosque encontrado
📊 Status ANTES: Disponivel (1)
📊 Status DEPOIS: Ocupado (2)
💾 SaveChanges retornou: 1 mudança(s)
✅ Verificação: Status no banco = Ocupado
```

### ❌ Problema - SaveChanges = 0:
```
💾 SaveChanges retornou: 0 mudança(s)
```
→ **EF Core não detectou mudança** (possível problema de tracking)

### ❌ Problema - Verificação = Disponivel:
```
✅ Verificação: Status no banco = Disponivel
```
→ **Salvou mas reverteu** (trigger? default value?)

## 🆘 Última Opção

Se NADA funcionar, usar SQL direto com inteiro:

```csharp
// No ReservaService
await _db.Database.ExecuteSqlRawAsync(
    @"UPDATE ""Quiosques"" 
      SET ""Status"" = {0}, ""UpdatedAt"" = {1} 
      WHERE ""Id"" = {2}",
    2, // INTEGER para Ocupado
    DateTimeOffset.UtcNow,
    request.QuiosqueId.Value);
```

## 📝 Próximo Passo

**EXECUTE AGORA:**

1. **Verificar tipo da coluna:**
   ```sql
   \d "Quiosques"
   ```

2. **Aplicar migration:**
   ```bash
   dotnet ef database update --verbose
   ```

3. **Reiniciar aplicação:**
   ```bash
   dotnet run --project EcoTurismo.Api
   ```

4. **Criar reserva e OBSERVAR LOGS**

5. **Me mostre os logs completos!**

---

**Os logs dirão EXATAMENTE onde está o problema!** 🔍

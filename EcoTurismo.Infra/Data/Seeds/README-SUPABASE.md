# Scripts SQL para Supabase - Dados de Exemplo

## 📁 Arquivos Disponíveis

### 1. `supabase-insert-exemplo.sql`
Script completo usando **PL/pgSQL** com:
- ✅ Geração automática de UUIDs
- ✅ Verificação se município já existe
- ✅ Logs detalhados (RAISE NOTICE)
- ✅ Consultas de verificação
- ✅ Transação em bloco

**Use este** para inserções automáticas e mais robustas.

### 2. `supabase-insert-simples.sql`
Script simplificado com:
- ✅ INSERTs diretos
- ✅ IDs fixos (fácil de rastrear)
- ✅ Pode executar linha por linha
- ✅ Mais fácil de debugar

**Use este** para testes rápidos ou se tiver problemas com PL/pgSQL.

## 🎯 O que os Scripts Inserem

| Item | Quantidade | Detalhes |
|------|-----------|----------|
| **Município** | 1 | Rio Verde de Mato Grosso - MS |
| **Atrativo** | 1 | Balneário das Águas Claras (cap. 500) |
| **Configuração** | 1 | capacidade_maxima_balneario |
| **Quiosques** | 2 | Ambos com churrasqueira |
| **Reservas** | 3 | 1 day use + 2 com quiosques |

## 🚀 Como Usar no Supabase

### Método 1: Script Completo (Recomendado)

1. Acesse o **SQL Editor** do Supabase
2. Cole todo o conteúdo de `supabase-insert-exemplo.sql`
3. Clique em **Run** (ou F5)
4. Veja os logs de sucesso no output

**Resultado esperado:**
```
NOTICE: Município já existe: ...
NOTICE: Atrativo criado: ... - Balneário das Águas Claras
NOTICE: Quiosque 1 criado: ...
NOTICE: Quiosque 2 criado: ...
NOTICE: Reservas criadas: 3 reservas de exemplo
```

### Método 2: Script Simples (Linha por Linha)

1. Acesse o **SQL Editor** do Supabase
2. Abra `supabase-insert-simples.sql`
3. Execute cada seção separadamente:
   - Primeiro o município
   - Depois o atrativo
   - Depois configuração
   - Depois quiosques
   - Por fim reservas

**Vantagem:** Pode debugar cada etapa

## 🔍 Verificar Inserções

Após executar, use estas queries:

### Ver Atrativo
```sql
SELECT * FROM "Atrativos" 
WHERE "Nome" = 'Balneário das Águas Claras';
```

### Ver Quiosques
```sql
SELECT q.*, a."Nome" as Atrativo
FROM "Quiosques" q
JOIN "Atrativos" a ON q."AtrativoId" = a."Id"
WHERE a."Nome" = 'Balneário das Águas Claras';
```

### Ver Reservas
```sql
SELECT 
    r."NomeVisitante",
    r."Email",
    r."Data",
    r."QuantidadePessoas",
    r."Token",
    CASE 
        WHEN r."QuiosqueId" IS NOT NULL 
        THEN 'Quiosque'
        ELSE 'Day Use'
    END as Tipo
FROM "Reservas" r
JOIN "Atrativos" a ON r."AtrativoId" = a."Id"
WHERE a."Nome" = 'Balneário das Águas Claras';
```

## 📊 Estrutura dos Dados

### Atrativo
```
Nome: Balneário das Águas Claras
Tipo: balneario
Status: ativo
Capacidade: 500 pessoas
Ocupação: 0
```

### Quiosques
```
Quiosque 1:
├─ Número: 1
├─ Churrasqueira: Sim
├─ Status: disponivel
└─ Posição: (10, 20)

Quiosque 2:
├─ Número: 2
├─ Churrasqueira: Sim
├─ Status: disponivel
└─ Posição: (30, 20)
```

### Reservas
```
Reserva 1 (Day Use):
├─ Nome: Maria Silva Santos
├─ Email: maria.silva@email.com
├─ Data: Hoje + 7 dias
├─ Pessoas: 4
└─ Quiosque: Não

Reserva 2 (Quiosque 1):
├─ Nome: João Pedro Oliveira
├─ Email: joao.pedro@email.com
├─ Data: Hoje + 5 dias
├─ Pessoas: 6
└─ Quiosque: 1

Reserva 3 (Quiosque 2):
├─ Nome: Ana Carolina Ferreira
├─ Email: ana.ferreira@email.com
├─ Data: Hoje + 10 dias
├─ Pessoas: 8
└─ Quiosque: 2
```

## ⚠️ Notas Importantes

### 1. IDs do Script Simples
Se usar `supabase-insert-simples.sql`, estes são os IDs fixos:

```
Município: 11111111-1111-1111-1111-111111111111
Atrativo:  22222222-2222-2222-2222-222222222222
Config:    33333333-3333-3333-3333-333333333333
Quiosque1: 44444444-4444-4444-4444-444444444444
Quiosque2: 55555555-5555-5555-5555-555555555555
Reserva1:  66666666-6666-6666-6666-666666666666
Reserva2:  77777777-7777-7777-7777-777777777777
Reserva3:  88888888-8888-8888-8888-888888888888
```

### 2. Município Existente
- Script completo: Verifica e usa existente
- Script simples: Usa `ON CONFLICT DO NOTHING`

### 3. Datas das Reservas
As reservas são criadas para:
- Reserva 1: **Hoje + 7 dias**
- Reserva 2: **Hoje + 5 dias**
- Reserva 3: **Hoje + 10 dias**

Isso garante que as reservas sejam futuras.

### 4. Tokens
- Script completo: Gera aleatórios
- Script simples: Usa fixos (ABC12345, XYZ98765, LMN45678)

## 🧪 Testando via API

Após inserir os dados, teste via API:

### 1. Listar Atrativos
```bash
GET /api/atrativos

# Deve retornar o Balneário das Águas Claras
```

### 2. Listar Quiosques
```bash
GET /api/quiosques?atrativoId=22222222-2222-2222-2222-222222222222

# Deve retornar 2 quiosques
```

### 3. Listar Reservas
```bash
GET /api/reservas?atrativoId=22222222-2222-2222-2222-222222222222

# Deve retornar 3 reservas
```

### 4. Validar Ticket
```bash
POST /api/validacoes
Authorization: Bearer {token-balneario}
{
  "token": "ABC12345"
}

# Deve validar a reserva da Maria Silva
```

## 🔄 Limpar Dados de Teste

Se quiser remover os dados inseridos:

```sql
-- ATENÇÃO: Isso vai deletar TODOS os dados relacionados!
DELETE FROM "Reservas" 
WHERE "AtrativoId" = '22222222-2222-2222-2222-222222222222';

DELETE FROM "Quiosques" 
WHERE "AtrativoId" = '22222222-2222-2222-2222-222222222222';

DELETE FROM "Configuracoes" 
WHERE "Chave" = 'capacidade_maxima_balneario';

DELETE FROM "Atrativos" 
WHERE "Id" = '22222222-2222-2222-2222-222222222222';

-- Opcional: Deletar município
DELETE FROM "Municipios" 
WHERE "Id" = '11111111-1111-1111-1111-111111111111';
```

## 🎯 Casos de Uso

### Desenvolvimento Local
Use `supabase-insert-simples.sql` para popular rapidamente.

### Ambiente de Testes
Use `supabase-insert-exemplo.sql` para dados mais realistas.

### Demonstração
Ambos funcionam! Escolha baseado na familiaridade com SQL.

## ✅ Checklist

- [ ] Escolher qual script usar
- [ ] Abrir SQL Editor do Supabase
- [ ] Copiar e colar o script
- [ ] Executar
- [ ] Verificar com queries de validação
- [ ] Testar via API
- [ ] Conferir se dados aparecem no frontend

## 🆘 Troubleshooting

### Erro: "duplicate key value violates unique constraint"
**Causa:** ID já existe no banco.

**Solução:** 
- Script completo: Não deve acontecer (gera UUIDs)
- Script simples: Altere os IDs no script

### Erro: "relation does not exist"
**Causa:** Tabelas não foram criadas (migrations não rodaram).

**Solução:** Execute as migrations primeiro:
```bash
dotnet ef database update --project EcoTurismo.Infra
```

### Erro: "foreign key violation"
**Causa:** Município não existe.

**Solução:** Execute primeiro a inserção do município.

## 📚 Recursos Adicionais

- [Documentação Supabase SQL Editor](https://supabase.com/docs/guides/database/overview)
- [PostgreSQL UUID Functions](https://www.postgresql.org/docs/current/functions-uuid.html)
- [Entity Framework Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

## 🎉 Pronto!

Agora você tem dados de exemplo completos para testar o sistema EcoTurismo!

Execute o script e comece a testar! 🚀

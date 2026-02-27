# Atrativo Inicial Adicionado ao Seed

## ✅ O que foi adicionado

Um **atrativo inicial** foi criado no seed de dados para facilitar os testes e demonstração do sistema.

## 📋 Dados do Atrativo

```csharp
Nome: "Balneário Municipal"
Tipo: "balneario"
Descrição: "O Balneário Municipal de Rio Verde de Mato Grosso é um dos principais 
            atrativos turísticos da região. Com águas cristalinas e infraestrutura 
            completa, oferece lazer e diversão para toda a família. O local conta 
            com piscinas naturais, áreas de camping, quiosques e playground."
Município: Rio Verde de Mato Grosso - MS
Imagem: https://via.placeholder.com/800x600?text=Balneario+Municipal
Status: "ativo"
Capacidade Máxima: 500 pessoas
Ocupação Atual: 0
```

## 🔗 Vinculações

### Usuário Balneário
O usuário com role **Balneário** foi vinculado ao atrativo criado:

```csharp
Email: balneario@ecoturismo.com.br
Senha: admin123
Role: Balneario
AtrativoId: {ID do Balneário Municipal}
```

Isso significa que esse usuário é o **operador** do Balneário Municipal e pode:
- ✅ Validar tickets de reservas para este atrativo
- ✅ Ver reservas do atrativo
- ✅ Gerenciar quiosques do atrativo (se implementado)

## 🎯 Benefícios

### 1. Testes Facilitados
Agora você pode testar imediatamente:
- Criar reservas para o Balneário Municipal
- Validar tickets com usuário balneario@ecoturismo.com.br
- Listar atrativos (retornará 1 atrativo)
- Ver detalhes do atrativo

### 2. Demonstração
O sistema já inicia com dados de exemplo realistas.

### 3. Desenvolvimento
Não é mais necessário criar manualmente um atrativo para começar a desenvolver/testar.

## 🧪 Como Testar

### 1. Resetar o Banco
```bash
dotnet ef database drop --project EcoTurismo.Infra --startup-project EcoTurismo.Api --force
dotnet run --project EcoTurismo.Api
```

### 2. Verificar Atrativo Criado
```bash
GET /api/atrativos
# Deve retornar 1 atrativo
```

### 3. Criar Reserva para o Atrativo
```bash
POST /api/reservas
{
  "atrativoId": "{id-do-atrativo}",
  "nomeVisitante": "João Silva",
  "email": "joao@example.com",
  "cpf": "123.456.789-00",
  "cidadeOrigem": "Campo Grande",
  "ufOrigem": "MS",
  "dataVisita": "2024-12-25",
  "numeroVisitantes": 4
}
```

### 4. Login como Operador do Balneário
```bash
POST /api/auth/login
{
  "email": "balneario@ecoturismo.com.br",
  "password": "admin123"
}
```

### 5. Validar Ticket
```bash
POST /api/validacoes
Authorization: Bearer {token-do-balneario}
{
  "token": "{token-da-reserva}"
}
```

## 📊 Estrutura do Seed Atualizada

```
1. Roles (4)
   ├─ Admin
   ├─ Prefeitura
   ├─ Balneario
   └─ Publico

2. Permissions (26)
   └─ banners:*, atrativos:*, quiosques:*, etc.

3. RolePermissions (vinculações)

4. Município Base (1)
   └─ Rio Verde de Mato Grosso - MS

4.1. Atrativo Inicial (1) ✅ NOVO!
   └─ Balneário Municipal
      ├─ Capacidade: 500
      ├─ Status: ativo
      └─ Vinculado ao município

5. Usuários Default (4)
   ├─ admin@ecoturismo.com.br (Admin)
   ├─ prefeitura@ecoturismo.com.br (Prefeitura)
   ├─ balneario@ecoturismo.com.br (Balneario) ← Vinculado ao atrativo ✅
   └─ publico@ecoturismo.com.br (Publico)
```

## 🎨 Próximos Passos (Opcional)

### Adicionar mais atrativos
Você pode adicionar mais atrativos ao seed:

```csharp
var cachoeira = new Atrativo
{
    Id = Guid.NewGuid(),
    Nome = "Cachoeira do Rio Verde",
    Tipo = "cachoeira",
    Descricao = "Bela cachoeira com queda d'água de 15 metros...",
    MunicipioId = municipioBase.Id,
    Status = "ativo",
    CapacidadeMaxima = 100,
    OcupacaoAtual = 0,
    CreatedAt = now,
    UpdatedAt = now
};

await context.Atrativos.AddAsync(cachoeira);
```

### Adicionar quiosques ao atrativo
```csharp
var quiosque1 = new Quiosque
{
    Id = Guid.NewGuid(),
    AtrativoId = atrativoInicial.Id,
    Nome = "Quiosque 1",
    Numero = 1,
    Status = "disponivel",
    CreatedAt = now
};

await context.Quiosques.AddAsync(quiosque1);
```

### Adicionar reserva de exemplo
```csharp
var reservaExemplo = new Reserva
{
    Id = Guid.NewGuid(),
    AtrativoId = atrativoInicial.Id,
    NomeVisitante = "Maria Silva",
    Email = "maria@example.com",
    // ... outros campos
};

await context.Reservas.AddAsync(reservaExemplo);
```

## ✅ Checklist

- [x] Atrativo adicionado ao seed
- [x] Usuário Balneário vinculado ao atrativo
- [x] Compilação bem-sucedida
- [ ] Banco resetado
- [ ] Seed executado
- [ ] Atrativo verificado via API
- [ ] Reserva teste criada
- [ ] Validação teste executada

## 🎉 Pronto!

Agora o sistema já inicia com um atrativo completo para testes e demonstrações!

Execute:
```bash
dotnet ef database drop --force
dotnet run --project EcoTurismo.Api
```

E teste:
```bash
GET /api/atrativos
# ✅ Retorna Balneário Municipal
```

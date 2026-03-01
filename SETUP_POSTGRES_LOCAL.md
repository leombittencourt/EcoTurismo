# 🐘 PostgreSQL Local - Docker Setup

## ✅ Configuração Concluída!

O `appsettings.Development.json` foi atualizado para conectar no PostgreSQL local.

## 🚀 Como Iniciar o Banco de Dados

### Opção 1: Docker Compose (Recomendado)

```bash
# Iniciar PostgreSQL + pgAdmin
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar
docker-compose down

# Parar e remover volumes (apaga dados)
docker-compose down -v
```

### Opção 2: Docker Run Simples

```bash
# Iniciar apenas o PostgreSQL
docker run -d \
  --name ecoturismo-postgres \
  -e POSTGRES_DB=ecoturismo \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:15-alpine

# Ver logs
docker logs -f ecoturismo-postgres

# Parar
docker stop ecoturismo-postgres

# Remover
docker rm ecoturismo-postgres
```

## 📊 Connection String Configurada

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ecoturismo;Username=postgres;Password=postgres"
}
```

## 🔧 Credenciais

### PostgreSQL
- **Host:** localhost
- **Port:** 5432
- **Database:** ecoturismo
- **Username:** postgres
- **Password:** postgres

### pgAdmin (opcional)
- **URL:** http://localhost:5050
- **Email:** admin@ecoturismo.com
- **Password:** admin

## 🎯 Testar Conexão

### Via psql (linha de comando)
```bash
# Entrar no container
docker exec -it ecoturismo-postgres psql -U postgres -d ecoturismo

# Ou via psql local
psql -h localhost -U postgres -d ecoturismo

# Comandos úteis
\dt              # Listar tabelas
\d "Quiosques"   # Descrever tabela
\q               # Sair
```

### Via pgAdmin
1. Abra http://localhost:5050
2. Login: admin@ecoturismo.com / admin
3. Add Server:
   - Name: EcoTurismo Local
   - Host: postgres (se dentro do docker) ou host.docker.internal
   - Port: 5432
   - Database: ecoturismo
   - Username: postgres
   - Password: postgres

### Via .NET (já configurado!)
```bash
# Iniciar a aplicação
dotnet run --project EcoTurismo.Api

# As migrations rodarão automaticamente
# O seed criará dados iniciais
```

## 📋 Checklist de Setup

- [x] 1. appsettings.Development.json atualizado
- [ ] 2. Docker Desktop instalado e rodando
- [ ] 3. Executar `docker-compose up -d`
- [ ] 4. Aguardar PostgreSQL iniciar (~10 segundos)
- [ ] 5. Iniciar aplicação: `dotnet run --project EcoTurismo.Api`
- [ ] 6. Verificar logs: "✅ Banco de dados configurado e pronto para uso!"

## 🔍 Troubleshooting

### Erro: "Connection refused"
```bash
# Verificar se o container está rodando
docker ps | grep postgres

# Se não estiver, iniciar
docker-compose up -d postgres
```

### Erro: "Port 5432 already in use"
```bash
# Ver o que está usando a porta
# Windows
netstat -ano | findstr :5432

# Linux/Mac
lsof -i :5432

# Parar o serviço PostgreSQL local ou usar outra porta
# docker-compose.yml: "5433:5432"
# appsettings: "Port=5433"
```

### Resetar Banco Completamente
```bash
# Parar containers
docker-compose down

# Remover volumes (apaga TODOS os dados)
docker-compose down -v

# Iniciar novamente
docker-compose up -d

# As migrations recriarão tudo
```

## 📚 Comandos Úteis

```bash
# Ver todos os containers
docker ps -a

# Ver logs do PostgreSQL
docker logs -f ecoturismo-postgres

# Acessar o shell do container
docker exec -it ecoturismo-postgres bash

# Backup do banco
docker exec ecoturismo-postgres pg_dump -U postgres ecoturismo > backup.sql

# Restore do banco
docker exec -i ecoturismo-postgres psql -U postgres ecoturismo < backup.sql

# Ver tamanho do banco
docker exec ecoturismo-postgres psql -U postgres -d ecoturismo -c "SELECT pg_size_pretty(pg_database_size('ecoturismo'));"
```

## 🎨 Visualizar Dados

### pgAdmin (Interface Gráfica)
- URL: http://localhost:5050
- Melhor para: Consultas complexas, design de tabelas

### DBeaver (Alternativa)
```bash
# Download: https://dbeaver.io/download/
# Connection:
Host: localhost
Port: 5432
Database: ecoturismo
Username: postgres
Password: postgres
```

### TablePlus (macOS)
```bash
# Download: https://tableplus.com/
# Criar nova conexão PostgreSQL com as credenciais acima
```

## ⚠️ Notas Importantes

### Desenvolvimento
- ✅ **Usar** esta configuração para desenvolvimento local
- ✅ **NÃO** commitar senhas reais no git
- ✅ **Usar** docker-compose para simplicidade

### Produção
- ❌ **NÃO** usar estas credenciais em produção
- ✅ **Usar** appsettings.Production.json
- ✅ **Usar** Azure Key Vault ou similar
- ✅ **Usar** connection string do Supabase/Azure

## 🚀 Próximos Passos

1. **Iniciar PostgreSQL:**
   ```bash
   docker-compose up -d
   ```

2. **Iniciar aplicação:**
   ```bash
   dotnet run --project EcoTurismo.Api
   ```

3. **Testar endpoints:**
   ```bash
   curl http://localhost:5000/api/municipios
   ```

4. **Ver banco no pgAdmin:**
   ```
   http://localhost:5050
   ```

## ✅ Pronto!

Seu ambiente de desenvolvimento está configurado com:
- ✅ PostgreSQL rodando no Docker
- ✅ appsettings apontando para localhost
- ✅ pgAdmin para visualizar dados
- ✅ Migrations automáticas ao iniciar
- ✅ Seed de dados iniciais

**Basta rodar `docker-compose up -d` e `dotnet run`!** 🎉

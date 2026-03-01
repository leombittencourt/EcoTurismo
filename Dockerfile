# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia arquivos de projeto e restaura dependências (melhor cache de layers)
COPY ["EcoTurismo.Api/EcoTurismo.Api.csproj", "EcoTurismo.Api/"]
COPY ["EcoTurismo.Application/EcoTurismo.Application.csproj", "EcoTurismo.Application/"]
COPY ["EcoTurismo.Domain/EcoTurismo.Domain.csproj", "EcoTurismo.Domain/"]
COPY ["EcoTurismo.Infra/EcoTurismo.Infra.csproj", "EcoTurismo.Infra/"]

# ✅ CORRIGIDO: Restore do projeto correto
RUN dotnet restore "EcoTurismo.Api/EcoTurismo.Api.csproj"

# Copia todo o código e faz o build
COPY . .

# ✅ CORRIGIDO: WORKDIR correto
WORKDIR "/src/EcoTurismo.Api"

# ✅ CORRIGIDO: Build do projeto correto
RUN dotnet build "EcoTurismo.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish

# ✅ CORRIGIDO: Publish do projeto correto
RUN dotnet publish "EcoTurismo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia os arquivos publicados
COPY --from=publish /app/publish .

# Expõe as portas HTTP e HTTPS
EXPOSE 8080
EXPOSE 8081

# Variáveis de ambiente para produção
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# ✅ CORRIGIDO: DLL correto
ENTRYPOINT ["dotnet", "EcoTurismo.Api.dll"]
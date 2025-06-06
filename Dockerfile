# Use a imagem oficial do .NET 8 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["AutoMLDemo.csproj", "."]
RUN dotnet restore "AutoMLDemo.csproj"

# Copiar todo o código fonte
COPY . .

# Compilar e publicar a aplicação
RUN dotnet publish "AutoMLDemo.csproj" -c Release -o /app/publish

# Use a imagem runtime do .NET 8 para execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Instalar curl para health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copiar arquivos publicados
COPY --from=build /app/publish .

# Criar diretório para dataset
RUN mkdir -p /app/src/AutoMLDemo/dataset

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Expor porta
EXPOSE 5000

# Comando de entrada
ENTRYPOINT ["dotnet", "AutoMLDemo.dll"]
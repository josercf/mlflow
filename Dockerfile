# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/AutoMLDemo/AutoMLDemo.csproj", "src/AutoMLDemo/"]
RUN dotnet restore "src/AutoMLDemo/AutoMLDemo.csproj"

COPY src/AutoMLDemo/ src/AutoMLDemo/
WORKDIR /src/src/AutoMLDemo
RUN dotnet publish "AutoMLDemo.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/src/AutoMLDemo/dataset

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000
ENTRYPOINT ["dotnet", "AutoMLDemo.dll"]

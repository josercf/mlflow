# 🚀 Como usar a API ML.NET

## 1. Executar a aplicação

```bash
# Navegar para o diretório do projeto
cd src/AutoMLDemo

# Restaurar dependências
dotnet restore

# Executar a aplicação
dotnet run
```

## 2. Endpoints disponíveis

### 🔮 Fazer Predição
```bash
# POST - Fazer uma predição
curl -X POST "http://localhost:5000/api/prediction/predict" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "movieId": 10
  }'
```

**Resposta esperada:**
```json
{
  "userId": 1,
  "movieId": 10,
  "predictedRating": 3.2456,
  "ratingRange": "medium",
  "latencyMs": 15.23,
  "totalPredictions": 1
}
```

### ❤️ Health Check
```bash
# GET - Verificar saúde da API
curl "http://localhost:5000/api/prediction/health"
```

**Resposta:**
```json
{
  "status": "healthy",
  "timestamp": "2025-06-06T15:30:00Z",
  "model_loaded": true
}
```

### 📊 Resumo das Métricas
```bash
# GET - Ver resumo das métricas
curl "http://localhost:5000/api/prediction/metrics-summary"
```

**Resposta:**
```json
{
  "total_predictions": 42,
  "model_r2": 0.0826,
  "model_rmse": 0.9901,
  "timestamp": "2025-06-06T15:30:00Z"
}
```

### 📈 Métricas Prometheus
```bash
# GET - Endpoint de métricas para Prometheus
curl "http://localhost:5000/metrics"
```

## 3. Testando com múltiplas predições

### Script Bash para teste de carga
```bash
#!/bin/bash

echo "🧪 Testando API com múltiplas predições..."

for i in {1..10}; do
  echo "Predição $i:"
  curl -X POST "http://localhost:5000/api/prediction/predict" \
    -H "Content-Type: application/json" \
    -d "{\"userId\": $i, \"movieId\": $((i * 10))}" \
    -w "\nTempo total: %{time_total}s\n\n"
  
  sleep 1
done

echo "✅ Teste concluído!"
```

### Script PowerShell para Windows
```powershell
# Testar predições múltiplas
for ($i = 1; $i -le 10; $i++) {
    Write-Host "Predição $i:"
    
    $body = @{
        userId = $i
        movieId = $i * 10
    } | ConvertTo-Json
    
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/prediction/predict" `
                                  -Method Post `
                                  -Body $body `
                                  -ContentType "application/json"
    
    Write-Host "Rating predito: $($response.predictedRating)" -ForegroundColor Green
    Write-Host "Faixa: $($response.ratingRange)" -ForegroundColor Yellow
    Write-Host "Latência: $($response.latencyMs)ms`n" -ForegroundColor Cyan
    
    Start-Sleep -Seconds 1
}
```

## 4. Validação de entrada

A API valida automaticamente os dados de entrada:

### ❌ Requisição inválida
```bash
curl -X POST "http://localhost:5000/api/prediction/predict" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": -1,
    "movieId": 0
  }'
```

**Resposta de erro:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "UserId": ["UserId deve ser maior que 0"],
    "MovieId": ["MovieId deve ser maior que 0"]
  }
}
```

## 5. Monitoramento com Prometheus

### Métricas disponíveis:
- **`prediction_latency_seconds`**: Histograma de latência das predições
- **`predictions_total`**: Contador total de predições
- **`accuracy_by_rating_range`**: Acurácia por faixa de rating
- **`model_r2`**: R-squared do modelo
- **`model_rmse`**: RMSE do modelo

### Query examples para Grafana:
```promql
# Taxa de predições por segundo
rate(predictions_total[5m])

# Latência média das predições
rate(prediction_latency_seconds_sum[5m]) / rate(prediction_latency_seconds_count[5m])

# Percentil 95 de latência
histogram_quantile(0.95, rate(prediction_latency_seconds_bucket[5m]))

# Acurácia por faixa de rating
accuracy_by_rating_range
```

## 6. Swagger UI

Acesse http://localhost:5000/swagger para uma interface interativa onde você pode:
- Ver toda a documentação da API
- Testar endpoints diretamente no navegador
- Ver exemplos de requisições e respostas

## 7. Integração com Docker

Para integrar com seu docker-compose, adicione:

```yaml
services:
  mlnet-api:
    build: .
    ports:
      - "5000:5000"
    volumes:
      - ./src/AutoMLDemo/dataset:/app/src/AutoMLDemo/dataset
    environment:
      - ASPNETCORE_URLS=http://+:5000
```

## 8. Monitoramento da API

### Logs estruturados
A API gera logs detalhados para cada predição:
```
[15:30:45] info: Predição - User: 1, Movie: 10, Rating: 3.25, Latência: 15.23ms
```

### Métricas HTTP automáticas
Além das métricas customizadas, a API expõe automaticamente:
- `http_requests_total`: Total de requests HTTP
- `http_request_duration_seconds`: Duração dos requests
- `http_requests_in_progress`: Requests em andamento

/
├── .devcontainer/
│   ├── devcontainer.json          # Configuração do Dev Container
│   └── Dockerfile                 # Dockerfile base para Dev Container
├── docker-compose.yml             # Compose para Prometheus e Grafana
├── prometheus.yml                 # Configuração do Prometheus
├── src/
│   └── AutoMLDemo/
│       ├── AutoMLDemo.csproj      # Projeto .NET
│       ├── Program.cs             # Código principal de AutoML e métricas
│       ├── MetricServer.cs        # Servidor Prometheus (exposição de métricas)
│       └── dataset/
│           └── movies.csv         # Exemplo de dataset (recomendação de filmes)
└── README.md

## Como usar

1. Abra o repositório no GitHub Codespaces (ou em VS Code com Remote – Containers).
2. Aguarde a criação do Dev Container (instala .NET 8, Python, R e pacotes MLflow, prometheus-client).
3. Coloque o arquivo `movies.csv` em `src/AutoMLDemo/dataset/`.
4. No terminal integrado do Codespace, execute:
   ```bash
   dotnet run --project src/AutoMLDemo/AutoMLDemo.csproj
   ```
   Isso iniciará o experimento AutoML, exibirá métricas no console e exporá métricas Prometheus em http://localhost:5000/metrics.
5. Em outro terminal, inicie Prometheus e Grafana:
   ```bash
   docker-compose up -d
   ```
6. Acesse o Prometheus em http://localhost:9090 para verificar alvos.
7. Acesse o Grafana em http://localhost:3000 (usuário padrão: admin / admin).
   - Adicione um Data Source do tipo Prometheus apontando para http://prometheus:9090.
   - Crie dashboards para as métricas model_r2 e model_rmse.

Dependências
- .NET 8 SDK
- Microsoft.ML (>= 2.0.0)
- Microsoft.ML.AutoML (preview)
- prometheus-net (>= 6.0.0)
- Python 3.x + pacotes: mlflow, prometheus-client
- R 4.x + pacote: mlflow
- Docker (para Prometheus e Grafana)

Referências
- Tutoriais ML.NET AutoML em .NET (Microsoft Docs)
- Exemplo de recomendação de filmes com Model Builder
- Prometheus .NET Client
- Grafana Documentation

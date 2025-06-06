#!/bin/bash

set -e

command_exists() {
    command -v "$1" >/dev/null 2>&1
}

echo "📋 Verificando dependências..."

if ! command_exists docker; then
    echo "❌ Docker não encontrado. Instale o Docker primeiro."; exit 1; fi
if ! command_exists docker-compose; then
    echo "❌ Docker Compose não encontrado. Instale o Docker Compose primeiro."; exit 1; fi
if ! command_exists dotnet; then
    echo "❌ .NET SDK não encontrado. Instale o .NET 8 SDK primeiro."; exit 1; fi

echo "✅ Todas as dependências encontradas!"

wait_for_service() {
    local url=$1
    local name=$2
    local max_attempts=30
    local attempt=1
    echo "⏳ Aguardando $name em $url..."
    while [ $attempt -le $max_attempts ]; do
        if curl -f -s "$url" >/dev/null 2>&1; then
            echo "✅ $name disponível"; return 0; fi
        echo "   Tentativa $attempt/$max_attempts..."; sleep 2; attempt=$((attempt+1))
    done
    echo "❌ $name não ficou disponível"; return 1
}

docker-compose down --remove-orphans 2>/dev/null || true

docker-compose build mlnet-api

docker-compose up -d

wait_for_service "http://localhost:5000/api/prediction/health" "API ML.NET"
wait_for_service "http://localhost:9090/-/ready" "Prometheus"
wait_for_service "http://localhost:3000/api/health" "Grafana"

echo "🎉 Setup concluído!"
echo "📊 Serviços:"; echo "   • API ML.NET: http://localhost:5000"; echo "   • Swagger UI: http://localhost:5000/swagger"; echo "   • Prometheus: http://localhost:9090"; echo "   • Grafana: http://localhost:3000 (admin/admin123)"; echo "   • Métricas: http://localhost:5000/metrics";

echo "🧪 Testando predição..."

curl -X POST "http://localhost:5000/api/prediction/predict" -H "Content-Type: application/json" -d '{"userId":1,"movieId":10}' -w "\n" | jq . 2>/dev/null || echo "(instale jq para formatação)"

echo "🔥 Executando teste de carga..."
for i in {1..10}; do
  curl -s -X POST "http://localhost:5000/api/prediction/predict" -H "Content-Type: application/json" -d "{\"userId\": $i, \"movieId\": $((i*10))}" >/dev/null
  sleep 0.5
done

echo "📈 Total de predições: $(curl -s http://localhost:5000/metrics | grep '^predictions_total' | head -1 | awk '{print $2}')"

echo "Para parar serviços: docker-compose down"

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

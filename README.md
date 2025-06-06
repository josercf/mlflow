/
├── .devcontainer/
│   ├── devcontainer.json          # Configuração do Dev Container
│   └── Dockerfile                 # Dockerfile base para Dev Container
├── docker-compose.yml             # Compose para API, Prometheus e Grafana
├── Dockerfile                     # Dockerfile da API ML.NET
├── prometheus.yml                 # Configuração do Prometheus
├── test.sh                        # Script de build e testes
├── src/
│   └── AutoMLDemo/
│       ├── AutoMLDemo.csproj      # Projeto Web API .NET
│       ├── Program.cs             # Host da API e métricas
│       ├── Controllers/           # Endpoints da API
│       ├── Services/              # Serviço de ML.NET
│       ├── DTOs/                  # Modelos de requisição/resposta
│       ├── Models/                # Tipos de dados do ML.NET
│       └── dataset/
│           └── ratings.csv        # Dataset de exemplo
└── README.md

## Como usar

1. Abra o repositório no GitHub Codespaces ou VS Code com Remote Containers.
2. Aguarde a criação do Dev Container (instala .NET 8, Python, R e pacotes necessários).
3. Coloque o arquivo `ratings.csv` em `src/AutoMLDemo/dataset/`.
4. Execute o script de teste para subir a API e os serviços de monitoramento:
   ```bash
   ./test.sh
   ```
5. Acesse:
   - API ML.NET: http://localhost:5000
   - Swagger: http://localhost:5000/swagger
   - Prometheus: http://localhost:9090
   - Grafana: http://localhost:3000 (admin/admin123)

Dependências
- Docker e Docker Compose
- .NET 8 SDK
- Python 3.x (instalado no Dev Container)
- R 4.x (instalado no Dev Container)

Referências
- Documentação do ML.NET AutoML
- Projeto prometheus-net
- Grafana Documentation


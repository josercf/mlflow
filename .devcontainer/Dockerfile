# Use a imagem oficial de Dev Container para .NET 8 + Python 3.11 + R no Debian

FROM mcr.microsoft.com/dotnet/sdk:8.0-noble

# 1. Atualizar repositório e instalar Python 3, pip e R-base
# Atualiza repos e instala dependências mínimas + Docker CLI + plugin docker-compose
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
         ca-certificates \
         curl \
         lsb-release \
         gnupg \
    && \
    # Instalação do repositório oficial do Docker
    mkdir -m 0755 -p /etc/apt/keyrings && \
    curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg && \
    echo \
      "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
      https://download.docker.com/linux/debian \
      $(lsb_release -cs) stable" \
      > /etc/apt/sources.list.d/docker.list && \
    apt-get update \
    && apt-get install -y --no-install-recommends \
         docker-ce-cli \
         docker-compose-plugin \
    && rm -rf /var/lib/apt/lists/*



# Use a imagem oficial de Dev Container para .NET 8 + Python 3.11 + R no Debian

FROM mcr.microsoft.com/dotnet/sdk:8.0-noble

# 2. Atualiza repositórios e instala dependências mínimas e chaves GPG
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
         ca-certificates \
         curl \
         lsb-release \
         gnupg \
    && rm -rf /var/lib/apt/lists/*

# 3. Adiciona o repositório do Docker para Ubuntu Jammy (22.04)
RUN mkdir -p /etc/apt/keyrings \
    && curl -fsSL https://download.docker.com/linux/ubuntu/gpg \
       | gpg --dearmor -o /etc/apt/keyrings/docker.gpg \
    && echo \
         "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
         https://download.docker.com/linux/ubuntu jammy stable" \
       > /etc/apt/sources.list.d/docker.list

# 4. Atualiza novamente e instala docker-ce-cli + docker-compose-plugin
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
         docker-ce-cli \
         docker-compose-plugin \
    && rm -rf /var/lib/apt/lists/*
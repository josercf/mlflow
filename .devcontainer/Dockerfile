# Use a imagem oficial de Dev Container para .NET 8 + Python 3.11 + R no Debian

FROM mcr.microsoft.com/devcontainers/dotnet:8-bullseye


# Instalar Python 3 e pip (caso não esteja incluído)
RUN apt-get update \
    && apt-get install -y python3 python3-pip r-base \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*

# Instalar MLflow para Python
RUN pip3 install mlflow prometheus-client

# Instalar o pacote mlflow no R
RUN R -e "install.packages('mlflow', repos='https://cloud.r-project.org/')"

# Instalar cliente Prometheus para .NET (no momento do build do projeto .NET, será feito dotnet add package)
# Apenas garanta que o ambiente .NET esteja pronto

# Configurar usuário vscode como padrão (imagem já define isso)

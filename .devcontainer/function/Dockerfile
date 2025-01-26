FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base

RUN curl -sL https://github.com/Azure/azure-functions-core-tools/releases/download/4.0.6821/Azure.Functions.Cli.linux-x64.4.0.6821.zip -o func-tools.zip && \
    mkdir -p /azure-functions && \
    unzip func-tools.zip -d /azure-functions && \
    rm func-tools.zip && \
    chmod +x /azure-functions/func && \
    ln -s /azure-functions/func /usr/bin/func

FROM base AS final

RUN apt-get update && \
    apt-get install -y --no-install-recommends openssl curl htop procps unzip && \
    rm -rf /var/lib/apt/lists/*

COPY .devcontainer/function/certs/azurite-ca.crt /usr/local/share/ca-certificates/azurite-ca.crt
RUN update-ca-certificates

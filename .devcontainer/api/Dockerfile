FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base

EXPOSE 80

COPY .devcontainer/api/certs/azurite-ca.crt /usr/local/share/ca-certificates/azurite-ca.crt
RUN update-ca-certificates

RUN apt-get update && \
    apt-get install -y --no-install-recommends openssl curl htop procps && \
    rm -rf /var/lib/apt/lists/*

COPY .devcontainer/api/run-container.sh /run-container.sh
RUN chmod +x /run-container.sh
RUN dotnet dev-certs https
RUN dotnet dev-certs https --trust

ENTRYPOINT ["/run-container.sh"]

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["NuGet.config", "."]
COPY ["backend/", "backend/"]
COPY ["shared/", "shared/"]
RUN dotnet restore "backend/MinimalApi.csproj"
WORKDIR "/src/backend"  

FROM build AS publish
RUN dotnet publish "MinimalApi.csproj" -o /app

FROM base AS final

COPY ./certs/azurite-ca.crt /usr/local/share/ca-certificates/azurite-ca.crt
RUN update-ca-certificates

COPY ./run-container.sh /app/run-container.sh

WORKDIR /app
COPY --from=publish /app .
ENV ASPNETCORE_URLS=http://+:80;
ENTRYPOINT ["./run-container.sh", "MinimalApi.dll"]
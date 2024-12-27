#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

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
WORKDIR /app
COPY --from=publish /app .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "MinimalApi.dll"]
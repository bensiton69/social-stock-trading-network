# Build context must be the repository root:
#   docker build -f SocialStockTradingNetwork.Api/Dockerfile -t <tag> .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore against just the project files first for better layer caching.
COPY SocialStockTradingNetwork.Api/SocialStockTradingNetwork.Api.csproj SocialStockTradingNetwork.Api/
COPY SocialStockTradingNetwork.ServiceDefaults/SocialStockTradingNetwork.ServiceDefaults.csproj SocialStockTradingNetwork.ServiceDefaults/
RUN dotnet restore SocialStockTradingNetwork.Api/SocialStockTradingNetwork.Api.csproj

COPY SocialStockTradingNetwork.Api/ SocialStockTradingNetwork.Api/
COPY SocialStockTradingNetwork.ServiceDefaults/ SocialStockTradingNetwork.ServiceDefaults/
RUN dotnet publish SocialStockTradingNetwork.Api/SocialStockTradingNetwork.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Run as the non-root user shipped in the base image.
USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    ENABLE_HTTPS_REDIRECTION=false

EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SocialStockTradingNetwork.Api.dll"]

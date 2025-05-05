# Базовый рантайм-образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY DT.Common/DT.Common.csproj .
COPY DT.Payments/DT.Payments.csproj .
RUN dotnet restore DT.Payments.csproj

COPY . .
RUN dotnet build DT.Payments.csproj -c $BUILD_CONFIGURATION -o /app/build

# Публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet build DT.Saga.csproj -c $BUILD_CONFIGURATION -o /app/build --no-restore
RUN dotnet publish DT.Saga.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DT.Payments.dll"]

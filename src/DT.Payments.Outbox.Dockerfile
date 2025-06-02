# Базовый рантайм-образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY DT.Shared/DT.Shared.csproj DT.Shared/
COPY DT.Payments.Outbox/DT.Payments.Outbox.csproj DT.Payments.Outbox/
RUN dotnet restore DT.Payments.Outbox/DT.Payments.Outbox.csproj

COPY DT.Shared DT.Shared
COPY DT.Payments.Outbox DT.Payments.Outbox
RUN dotnet build DT.Payments.Outbox/DT.Payments.Outbox.csproj -c $BUILD_CONFIGURATION -o /app/build

# Публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish DT.Payments.Outbox/DT.Payments.Outbox.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DT.Payments.Outbox.dll"]

# 1. Базовый образ рантайма
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# 2. Сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY DT.Shared/DT.Shared.csproj DT.Shared/
COPY DT.Inventories.Outbox/DT.Inventories.Outbox.csproj DT.Inventories.Outbox/
RUN dotnet restore DT.Inventories.Outbox/DT.Inventories.Outbox.csproj

COPY DT.Shared DT.Shared
COPY DT.Inventories.Outbox DT.Inventories.Outbox 
RUN dotnet build DT.Inventories.Outbox/DT.Inventories.Outbox.csproj -c $BUILD_CONFIGURATION -o /app/build
    
# 3. Публикация
FROM build AS publish
RUN dotnet publish DT.Inventories.Outbox/DT.Inventories.Outbox.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DT.Inventories.Outbox.dll"]

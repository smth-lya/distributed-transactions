# 1. Базовый образ рантайма
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY DT.Common/DT.Common.csproj DT.Common/
COPY DT.Inventories/DT.Inventories.csproj DT.Inventories/
RUN dotnet restore DT.Inventories/DT.Inventories.csproj

COPY DT.Common DT.Common
COPY DT.Inventories DT.Inventories 
RUN dotnet build DT.Inventories/DT.Inventories.csproj -c $BUILD_CONFIGURATION -o /app/build
    
# 3. Публикация
FROM build AS publish
RUN dotnet publish DT.Inventories/DT.Inventories.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DT.Inventories.dll"]

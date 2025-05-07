# Базовый рантайм образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY DT.Common/DT.Common.csproj DT.Common/
COPY DT.Orders/DT.Orders.csproj DT.Orders/
RUN dotnet restore DT.Orders/DT.Orders.csproj

COPY DT.Common DT.Common
COPY DT.Orders DT.Orders                      
RUN dotnet build DT.Orders/DT.Orders.csproj -c $BUILD_CONFIGURATION -o /app/build

# Публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish DT.Orders/DT.Orders.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DT.Orders.dll"]

# Базовый рантайм-образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY DT.Common/DT.Common.csproj DT.Common/
COPY DT.Payments/DT.Payments.csproj DT.Payments/
RUN dotnet restore DT.Payments/DT.Payments.csproj

COPY DT.Common DT.Common
COPY DT.Payments DT.Payments
RUN dotnet build DT.Payments/DT.Payments.csproj -c $BUILD_CONFIGURATION -o /app/build

# Публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish DT.Payments/DT.Payments.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DT.Payments.dll"]

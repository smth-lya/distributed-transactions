﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY DT.Shared/DT.Shared.csproj DT.Shared/
COPY DT.Saga.Outbox/DT.Saga.Outbox.csproj DT.Saga.Outbox/
RUN dotnet restore DT.Saga.Outbox/DT.Saga.Outbox.csproj

COPY DT.Shared DT.Shared
COPY DT.Saga.Outbox DT.Saga.Outbox
RUN dotnet build DT.Saga.Outbox/DT.Saga.Outbox.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish DT.Saga.Outbox/DT.Saga.Outbox.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DT.Saga.Outbox.dll"]
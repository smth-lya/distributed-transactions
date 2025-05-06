FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY DT.Common/DT.Common.csproj DT.Common/
COPY DT.Saga/DT.Saga.csproj DT.Saga/
RUN dotnet restore DT.Saga/DT.Saga.csproj

COPY DT.Common DT.Common
COPY DT.Saga DT.Saga
RUN dotnet build DT.Saga/DT.Saga.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish DT.Saga/DT.Saga.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DT.Saga.dll"]
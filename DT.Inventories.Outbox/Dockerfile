FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DT.Inventories.Outbox/DT.Inventories.Outbox.csproj", "DT.Inventories.Outbox/"]
RUN dotnet restore "DT.Inventories.Outbox/DT.Inventories.Outbox.csproj"
COPY . .
WORKDIR "/src/DT.Inventories.Outbox"
RUN dotnet build "DT.Inventories.Outbox.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DT.Inventories.Outbox.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DT.Inventories.Outbox.dll"]

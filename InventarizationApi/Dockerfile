FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["InventarizationApi/InventarizationApi.csproj", "InventarizationApi/"]
RUN dotnet restore "InventarizationApi/InventarizationApi.csproj"
COPY . .
WORKDIR "/src/InventarizationApi"
RUN dotnet build "InventarizationApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventarizationApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventarizationApi.dll"]

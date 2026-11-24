FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/SIGMA.API/SIGMA.API.csproj", "src/SIGMA.API/"]
RUN dotnet restore "src/SIGMA.API/SIGMA.API.csproj"
COPY src/SIGMA.API/ src/SIGMA.API/
RUN dotnet publish "src/SIGMA.API/SIGMA.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SIGMA.API.dll"]

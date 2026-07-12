FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/SIGMA.API/SIGMA.API.csproj", "src/SIGMA.API/"]
COPY ["src/SIGMA.Application/SIGMA.Application.csproj", "src/SIGMA.Application/"]
COPY ["src/SIGMA.Infrastructure/SIGMA.Infrastructure.csproj", "src/SIGMA.Infrastructure/"]
COPY ["src/SIGMA.Domain/SIGMA.Domain.csproj", "src/SIGMA.Domain/"]
RUN dotnet restore "src/SIGMA.API/SIGMA.API.csproj"

COPY . .
WORKDIR "/src/src/SIGMA.API"
RUN dotnet build "SIGMA.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SIGMA.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SIGMA.API.dll"]

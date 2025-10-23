# Stage 1: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
# Install PostgreSQL client
RUN apt-get update && apt-get install -y postgresql-client

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files và restore
COPY AuthService.Api/AuthService.Api.csproj AuthService.Api/
COPY AuthService.Application/AuthService.Application.csproj AuthService.Application/
COPY AuthService.Infrastructure/AuthService.Infrastructure.csproj AuthService.Infrastructure/
COPY AuthService.Domain/AuthService.Domain.csproj AuthService.Domain/

RUN dotnet restore "AuthService.Api/AuthService.Api.csproj"

# Copy all source code
COPY . .

WORKDIR /src/AuthService.Api
RUN dotnet build "AuthService.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "AuthService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.Api.dll"]

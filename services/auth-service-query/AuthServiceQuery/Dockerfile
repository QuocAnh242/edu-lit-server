FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["AuthServiceQuery/AuthServiceQuery.Api.csproj", "AuthServiceQuery/"]
COPY ["AuthServiceQuery.Application/AuthServiceQuery.Application.csproj", "AuthServiceQuery.Application/"]
COPY ["AuthServiceQuery.Domain/AuthServiceQuery.Domain.csproj", "AuthServiceQuery.Domain/"]
COPY ["AuthServiceQuery.Infrastructure/AuthServiceQuery.Infrastructure.csproj", "AuthServiceQuery.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "AuthServiceQuery/AuthServiceQuery.Api.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/AuthServiceQuery"
RUN dotnet build "AuthServiceQuery.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthServiceQuery.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthServiceQuery.Api.dll"]

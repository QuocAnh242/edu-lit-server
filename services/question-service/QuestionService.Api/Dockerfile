# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Cài gói cần thiết bằng quyền root
USER root
RUN apt-get update && apt-get install -y --no-install-recommends postgresql-client && rm -rf /var/lib/apt/lists/*

# Tạo user non-root để chạy app (an toàn hơn)
RUN adduser --disabled-password --home /app appuser
USER appuser

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["QuestionService.Api/QuestionService.Api.csproj", "QuestionService.Api/"]
COPY ["QuestionService.Application/QuestionService.Application.csproj", "QuestionService.Application/"]
COPY ["QuestionService.Domain/QuestionService.Domain.csproj", "QuestionService.Domain/"]
COPY ["QuestionService.Infrastructure/QuestionService.Infrastructure.csproj", "QuestionService.Infrastructure/"]
RUN dotnet restore "./QuestionService.Api/QuestionService.Api.csproj"
COPY . .
WORKDIR "/src/QuestionService.Api"
RUN dotnet build "./QuestionService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./QuestionService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuestionService.Api.dll"]

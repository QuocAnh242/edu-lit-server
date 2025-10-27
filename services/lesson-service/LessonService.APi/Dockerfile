# ===================================================
# Stage 1: Base - Môi trường chạy cuối cùng
# Chứa .NET Runtime và các công cụ cần thiết như psql client
# ===================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:8080"
EXPOSE 8080

# Cài đặt postgresql-client để có thể dùng pg_isready nếu cần
RUN apt-get update && apt-get install -y --no-install-recommends postgresql-client && rm -rf /var/lib/apt/lists/*

# ===================================================
# Stage 2: Build - Môi trường để biên dịch code
# Chứa .NET SDK đầy đủ
# ===================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src 

COPY LessonService.Api/LessonService.Api.csproj LessonService.Api/
COPY LessonService.Application/LessonService.Application.csproj LessonService.Application/
COPY LessonService.Domain/LessonService.Domain.csproj LessonService.Domain/
COPY LessonService.Infrastructure/LessonService.Infrastructure.csproj LessonService.Infrastructure/

RUN dotnet restore "LessonService.Api/LessonService.Api.csproj"

# Copy toàn bộ mã nguồn
COPY . .

# Build project
WORKDIR "/src/LessonService.Api"
RUN dotnet build "LessonService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===================================================
# Stage 3: Publish - Tách biệt bước publish cho rõ ràng
# ===================================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/LessonService.Api"
RUN dotnet publish "LessonService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===================================================
# Stage 4: Final - Tạo image cuối cùng để chạy
# Kế thừa từ stage 'base' đã được chuẩn bị
# ===================================================
FROM base AS final
WORKDIR /app

# Copy publish output into /app (explicit destination directory)
COPY --from=publish /app/publish /app

# Run the published assembly by absolute path to avoid ambiguity
ENTRYPOINT ["dotnet", "/app/LessonService.Api.dll"]

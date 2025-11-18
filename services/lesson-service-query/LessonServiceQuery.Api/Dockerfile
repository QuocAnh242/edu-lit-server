# ===================================================
# Stage 1: Base - Môi trường chạy cuối cùng
# Chứa .NET Runtime và các công cụ cần thiết như psql client
# ===================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:8080"
EXPOSE 8080

# ===================================================
# Stage 2: Build - Môi trường để biên dịch code
# Chứa .NET SDK đầy đủ
# ===================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src 

COPY LessonServiceQuery.Api/LessonServiceQuery.Api.csproj LessonServiceQuery.Api/
COPY LessonServiceQuery.Application/LessonServiceQuery.Application.csproj LessonServiceQuery.Application/
COPY LessonServiceQuery.Domain/LessonServiceQuery.Domain.csproj LessonServiceQuery.Domain/
COPY LessonServiceQuery.Infrastructure/LessonServiceQuery.Infrastructure.csproj LessonServiceQuery.Infrastructure/

RUN dotnet restore "LessonServiceQuery.Api/LessonServiceQuery.Api.csproj"

# Copy toàn bộ mã nguồn
COPY . .

# Build project
WORKDIR "/src/LessonServiceQuery.Api"
RUN dotnet build "LessonServiceQuery.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===================================================
# Stage 3: Publish - Tách biệt bước publish cho rõ ràng
# ===================================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/LessonServiceQuery.Api"
RUN dotnet publish "LessonServiceQuery.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===================================================
# Stage 4: Final - Tạo image cuối cùng để chạy
# Kế thừa từ stage 'base' đã được chuẩn bị
# ===================================================
FROM base AS final
WORKDIR /app

# Copy publish output into /app (explicit destination directory)
COPY --from=publish /app/publish /app

# Run the published assembly by absolute path to avoid ambiguity
ENTRYPOINT ["dotnet", "/app/LessonServiceQuery.Api.dll"]

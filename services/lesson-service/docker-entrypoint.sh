#!/bin/bash
set -e

echo "==================================="
echo "LessonService Startup Script"
echo "==================================="

# Wait for PostgreSQL
echo "Waiting for PostgreSQL to be ready..."
until PGPASSWORD="$DB_PASSWORD" psql -h "$DB_HOST" -U "$DB_USER" -d "$DB_NAME" -c '\q' 2>/dev/null; do
  echo "PostgreSQL is unavailable - sleeping"
  sleep 2
done

echo "✅ PostgreSQL is ready!"

# Note: Migrations are applied automatically by the application on startup

# Wait for RabbitMQ
echo "Waiting for RabbitMQ to be ready..."
timeout=60
counter=0
until nc -z rabbitmq 5672 2>/dev/null; do
  counter=$((counter + 1))
  if [ $counter -gt $timeout ]; then
    echo "⚠️  RabbitMQ timeout, continuing anyway..."
    break
  fi
  echo "RabbitMQ is unavailable - sleeping"
  sleep 2
done

echo "✅ RabbitMQ is ready!"

# Start the application
echo "Starting LessonService API..."
exec dotnet LessonService.Api.dll

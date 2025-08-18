#!/bin/bash

# ========================================
# Start .NET API Service
# ========================================

set -e

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_status "Starting .NET API..."

# Check if PostgreSQL is running
if ! pgrep -x "postgres" > /dev/null; then
    print_status "Starting PostgreSQL..."
    brew services start postgresql@16 2>/dev/null || brew services start postgresql
    sleep 2
fi

# Check if Redis is running
if ! pgrep -x "redis-server" > /dev/null; then
    print_status "Starting Redis..."
    brew services start redis
    sleep 2
fi

# Kill any existing process on port 5000
if lsof -Pi :5000 -sTCP:LISTEN -t >/dev/null 2>&1; then
    print_status "Stopping existing API process..."
    lsof -ti:5000 | xargs kill -9 2>/dev/null || true
    sleep 1
fi

# Start the API
cd QueueManagement.Api
print_status "Building API..."
dotnet build --configuration Release

print_status "Running API..."
dotnet run --configuration Release

# Note: This will run in the foreground. 
# Press Ctrl+C to stop the API.
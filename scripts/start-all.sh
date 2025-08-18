#!/bin/bash

# ========================================
# Queue Management Platform - Start All Services
# ========================================

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if a port is in use
check_port() {
    if lsof -Pi :$1 -sTCP:LISTEN -t >/dev/null 2>&1; then
        return 0
    else
        return 1
    fi
}

# Function to wait for a service to be ready
wait_for_service() {
    local port=$1
    local service=$2
    local max_attempts=30
    local attempt=0
    
    print_status "Waiting for $service to be ready on port $port..."
    
    while [ $attempt -lt $max_attempts ]; do
        if check_port $port; then
            print_success "$service is ready!"
            return 0
        fi
        sleep 1
        attempt=$((attempt + 1))
    done
    
    print_error "$service failed to start on port $port"
    return 1
}

# Kill any existing processes on our ports
print_status "Checking for existing processes..."
for port in 5000 5173 3000; do
    if check_port $port; then
        print_status "Killing process on port $port..."
        lsof -ti:$port | xargs kill -9 2>/dev/null || true
        sleep 1
    fi
done

# Start PostgreSQL if not running
if ! pgrep -x "postgres" > /dev/null; then
    print_status "Starting PostgreSQL..."
    if [[ $(uname -m) == 'arm64' ]]; then
        brew services start postgresql@16
    else
        brew services start postgresql
    fi
    sleep 2
else
    print_success "PostgreSQL is already running"
fi

# Start Redis if not running
if ! pgrep -x "redis-server" > /dev/null; then
    print_status "Starting Redis..."
    brew services start redis
    sleep 2
else
    print_success "Redis is already running"
fi

# Create log directory
mkdir -p logs

# Start .NET API
print_status "Starting .NET API..."
cd QueueManagement.Api
dotnet run --configuration Release > ../logs/api.log 2>&1 &
API_PID=$!
cd ..

# Wait for API to be ready
wait_for_service 5000 "API"

# Start Admin Frontend
print_status "Starting Admin Frontend..."
cd queuemanagement-admin
npm run dev > ../logs/admin.log 2>&1 &
ADMIN_PID=$!
cd ..

# Wait for Admin to be ready
wait_for_service 5173 "Admin Frontend"

# Start Platform Frontend
print_status "Starting Platform Frontend..."
cd queue-management-platform
npm run dev > ../logs/platform.log 2>&1 &
PLATFORM_PID=$!
cd ..

# Wait for Platform to be ready
wait_for_service 3000 "Platform Frontend"

# Save PIDs to file for stop script
echo "API_PID=$API_PID" > .pids
echo "ADMIN_PID=$ADMIN_PID" >> .pids
echo "PLATFORM_PID=$PLATFORM_PID" >> .pids

# Display status
echo ""
print_success "🚀 All services started successfully! 🚀"
echo ""
echo "========================================="
echo "Services are running at:"
echo "========================================="
echo ""
echo "  API Documentation: http://localhost:5000/swagger"
echo "  Admin Dashboard:   http://localhost:5173"
echo "  Platform:          http://localhost:3000"
echo ""
echo "========================================="
echo "Logs are available in:"
echo "========================================="
echo ""
echo "  API:      logs/api.log"
echo "  Admin:    logs/admin.log"
echo "  Platform: logs/platform.log"
echo ""
echo "========================================="
echo "To stop all services, run:"
echo "========================================="
echo ""
echo "  ./scripts/stop-all.sh"
echo ""
echo "Press Ctrl+C to stop all services"
echo "========================================="

# Function to cleanup on exit
cleanup() {
    print_status "Stopping all services..."
    kill $API_PID 2>/dev/null || true
    kill $ADMIN_PID 2>/dev/null || true
    kill $PLATFORM_PID 2>/dev/null || true
    rm -f .pids
    print_success "All services stopped"
    exit 0
}

# Set trap to cleanup on script exit
trap cleanup EXIT INT TERM

# Keep script running
while true; do
    sleep 1
done
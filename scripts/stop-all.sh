#!/bin/bash

# ========================================
# Queue Management Platform - Stop All Services
# ========================================

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
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

print_status "Stopping all Queue Management services..."

# Stop services using saved PIDs
if [ -f .pids ]; then
    source .pids
    
    if [ ! -z "$API_PID" ]; then
        print_status "Stopping API (PID: $API_PID)..."
        kill $API_PID 2>/dev/null || true
    fi
    
    if [ ! -z "$ADMIN_PID" ]; then
        print_status "Stopping Admin Frontend (PID: $ADMIN_PID)..."
        kill $ADMIN_PID 2>/dev/null || true
    fi
    
    if [ ! -z "$PLATFORM_PID" ]; then
        print_status "Stopping Platform Frontend (PID: $PLATFORM_PID)..."
        kill $PLATFORM_PID 2>/dev/null || true
    fi
    
    rm -f .pids
fi

# Kill any remaining processes on our ports
print_status "Cleaning up any remaining processes..."
for port in 5000 5173 3000; do
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1; then
        print_status "Killing process on port $port..."
        lsof -ti:$port | xargs kill -9 2>/dev/null || true
    fi
done

# Optionally stop PostgreSQL and Redis
read -p "Do you want to stop PostgreSQL and Redis? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    print_status "Stopping PostgreSQL..."
    brew services stop postgresql@16 2>/dev/null || brew services stop postgresql 2>/dev/null || true
    
    print_status "Stopping Redis..."
    brew services stop redis 2>/dev/null || true
fi

print_success "All services stopped successfully!"
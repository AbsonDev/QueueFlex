#!/bin/bash

# ========================================
# Queue Management Platform - Health Check
# ========================================

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${BLUE}[CHECK]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[✓]${NC} $1"
}

print_error() {
    echo -e "${RED}[✗]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[!]${NC} $1"
}

echo "========================================="
echo "Queue Management Platform - Health Check"
echo "========================================="
echo ""

# Check system dependencies
print_status "Checking system dependencies..."

# Check Node.js
if command -v node &> /dev/null; then
    print_success "Node.js: $(node --version)"
else
    print_error "Node.js: Not installed"
fi

# Check npm
if command -v npm &> /dev/null; then
    print_success "npm: $(npm --version)"
else
    print_error "npm: Not installed"
fi

# Check .NET
if command -v dotnet &> /dev/null; then
    print_success ".NET SDK: $(dotnet --version)"
else
    print_error ".NET SDK: Not installed"
fi

# Check PostgreSQL
if command -v psql &> /dev/null; then
    print_success "PostgreSQL: $(psql --version | head -n1)"
else
    print_error "PostgreSQL: Not installed"
fi

# Check Redis
if command -v redis-cli &> /dev/null; then
    print_success "Redis: $(redis-cli --version)"
else
    print_error "Redis: Not installed"
fi

echo ""
print_status "Checking services..."

# Function to check if a port is in use
check_service() {
    local port=$1
    local service=$2
    
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1; then
        print_success "$service is running on port $port"
        return 0
    else
        print_error "$service is not running on port $port"
        return 1
    fi
}

# Check PostgreSQL service
if pgrep -x "postgres" > /dev/null; then
    print_success "PostgreSQL service is running"
    
    # Try to connect to database
    if PGPASSWORD=password psql -U postgres -d QueueManagement -c '\q' 2>/dev/null; then
        print_success "Can connect to QueueManagement database"
    else
        print_warning "Cannot connect to QueueManagement database (may need setup)"
    fi
else
    print_error "PostgreSQL service is not running"
fi

# Check Redis service
if pgrep -x "redis-server" > /dev/null; then
    print_success "Redis service is running"
    
    # Try to ping Redis
    if redis-cli ping > /dev/null 2>&1; then
        print_success "Redis is responding to ping"
    else
        print_warning "Redis is not responding"
    fi
else
    print_error "Redis service is not running"
fi

echo ""
print_status "Checking application services..."

# Check API
check_service 5000 "API (.NET)"

# Check Admin Frontend
check_service 5173 "Admin Frontend (Vite)"

# Check Platform Frontend
check_service 3000 "Platform Frontend (Next.js)"

echo ""
print_status "Checking project files..."

# Check if node_modules exist
if [ -d "queuemanagement-admin/node_modules" ]; then
    print_success "Admin Frontend dependencies installed"
else
    print_warning "Admin Frontend dependencies not installed (run npm install)"
fi

if [ -d "queue-management-platform/node_modules" ]; then
    print_success "Platform Frontend dependencies installed"
else
    print_warning "Platform Frontend dependencies not installed (run npm install)"
fi

# Check if .NET packages are restored
if [ -d "QueueManagement.Api/obj" ]; then
    print_success ".NET packages restored"
else
    print_warning ".NET packages not restored (run dotnet restore)"
fi

# Check environment files
if [ -f ".env" ]; then
    print_success "Root .env file exists"
else
    print_warning "Root .env file missing (copy from .env.example)"
fi

if [ -f "queuemanagement-admin/.env" ]; then
    print_success "Admin Frontend .env file exists"
else
    print_warning "Admin Frontend .env file missing"
fi

if [ -f "queue-management-platform/.env.local" ]; then
    print_success "Platform Frontend .env.local file exists"
else
    print_warning "Platform Frontend .env.local file missing"
fi

echo ""
echo "========================================="
echo "Health check complete!"
echo "========================================="
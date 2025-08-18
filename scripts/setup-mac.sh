#!/bin/bash

# ========================================
# Queue Management Platform - Mac Setup Script
# For Mac Mini M4 and other Apple Silicon Macs
# ========================================

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if running on Mac
if [[ "$OSTYPE" != "darwin"* ]]; then
    print_error "This script is designed for macOS only"
    exit 1
fi

print_status "Starting Queue Management Platform setup for Mac..."

# ========================================
# Check and Install Homebrew
# ========================================
if ! command -v brew &> /dev/null; then
    print_status "Installing Homebrew..."
    /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
    
    # Add Homebrew to PATH for Apple Silicon
    if [[ $(uname -m) == 'arm64' ]]; then
        echo 'eval "$(/opt/homebrew/bin/brew shellenv)"' >> ~/.zprofile
        eval "$(/opt/homebrew/bin/brew shellenv)"
    fi
else
    print_success "Homebrew is already installed"
fi

# Update Homebrew
print_status "Updating Homebrew..."
brew update

# ========================================
# Install System Dependencies
# ========================================
print_status "Installing system dependencies..."

# Install Node.js (LTS version)
if ! command -v node &> /dev/null; then
    print_status "Installing Node.js..."
    brew install node@20
    brew link node@20
else
    print_success "Node.js is already installed ($(node --version))"
fi

# Install .NET SDK
if ! command -v dotnet &> /dev/null; then
    print_status "Installing .NET SDK 8.0..."
    brew install --cask dotnet-sdk
else
    print_success ".NET SDK is already installed ($(dotnet --version))"
fi

# Install PostgreSQL
if ! command -v psql &> /dev/null; then
    print_status "Installing PostgreSQL..."
    brew install postgresql@16
    brew services start postgresql@16
    echo 'export PATH="/opt/homebrew/opt/postgresql@16/bin:$PATH"' >> ~/.zshrc
else
    print_success "PostgreSQL is already installed"
fi

# Install Redis
if ! command -v redis-cli &> /dev/null; then
    print_status "Installing Redis..."
    brew install redis
    brew services start redis
else
    print_success "Redis is already installed"
fi

# Install Git (if not already installed)
if ! command -v git &> /dev/null; then
    print_status "Installing Git..."
    brew install git
else
    print_success "Git is already installed"
fi

# ========================================
# Setup PostgreSQL Database
# ========================================
print_status "Setting up PostgreSQL database..."

# Wait for PostgreSQL to be ready
sleep 2

# Create database and user
print_status "Creating database and user..."
createuser -s postgres 2>/dev/null || true
psql -U postgres -c "ALTER USER postgres PASSWORD 'password';" 2>/dev/null || true
createdb -U postgres QueueManagement 2>/dev/null || print_warning "Database QueueManagement might already exist"

print_success "PostgreSQL setup complete"

# ========================================
# Setup Environment Files
# ========================================
print_status "Setting up environment files..."

# Copy .env.example to .env if it doesn't exist
if [ ! -f .env ]; then
    if [ -f .env.example ]; then
        cp .env.example .env
        print_success "Created .env file from .env.example"
    fi
fi

# ========================================
# Install Project Dependencies
# ========================================
print_status "Installing project dependencies..."

# Install .NET dependencies
print_status "Restoring .NET packages..."
dotnet restore

# Install Node.js dependencies for admin frontend
print_status "Installing dependencies for admin frontend..."
cd queuemanagement-admin
npm install
cd ..

# Install Node.js dependencies for platform frontend
print_status "Installing dependencies for platform frontend..."
cd queue-management-platform
npm install
cd ..

print_success "All dependencies installed successfully"

# ========================================
# Build Projects
# ========================================
print_status "Building projects..."

# Build .NET API
print_status "Building .NET API..."
dotnet build --configuration Release

# Build admin frontend
print_status "Building admin frontend..."
cd queuemanagement-admin
npm run build
cd ..

# Build platform frontend
print_status "Building platform frontend..."
cd queue-management-platform
npm run build
cd ..

print_success "All projects built successfully"

# ========================================
# Database Migrations
# ========================================
print_status "Running database migrations..."
cd QueueManagement.Api
dotnet ef database update 2>/dev/null || print_warning "No migrations to apply or EF Core tools not installed"
cd ..

# ========================================
# Final Instructions
# ========================================
echo ""
print_success "🎉 Setup complete! 🎉"
echo ""
echo "========================================="
echo "To start the application, run:"
echo "========================================="
echo ""
echo "  ./scripts/start-all.sh"
echo ""
echo "Or start services individually:"
echo ""
echo "  ./scripts/start-api.sh      # Start .NET API"
echo "  ./scripts/start-admin.sh    # Start Admin Frontend"
echo "  ./scripts/start-platform.sh # Start Platform Frontend"
echo ""
echo "========================================="
echo "Access the applications at:"
echo "========================================="
echo ""
echo "  API:      http://localhost:5000"
echo "  Admin:    http://localhost:5173"
echo "  Platform: http://localhost:3000"
echo ""
echo "========================================="
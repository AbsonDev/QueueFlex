#!/bin/bash

# ========================================
# Queue Management Platform - Test Setup
# ========================================

set -e

# Colors for output
RED="\033[0;31m"
GREEN="\033[0;32m"
YELLOW="\033[1;33m"
BLUE="\033[0;34m"
NC="\033[0m" # No Color

print_status() {
    echo -e "${BLUE}[TEST]${NC} $1"
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
echo "Testing Queue Management Platform Setup"
echo "========================================="
echo ""

ERRORS=0
WARNINGS=0

# Test system dependencies
print_status "Testing system dependencies..."

# Node.js
if command -v node &> /dev/null; then
    NODE_VERSION=$(node --version)
    print_success "Node.js $NODE_VERSION"
else
    print_error "Node.js not installed"
    ((ERRORS++))
fi

# .NET SDK
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    print_success ".NET SDK $DOTNET_VERSION"
else
    print_error ".NET SDK not installed"
    ((ERRORS++))
fi

echo ""
echo "========================================="
echo "Test Results"
echo "========================================="

if [ $ERRORS -eq 0 ]; then
    print_success "Everything is ready to go!"
    echo ""
    echo "Run ./scripts/start-all.sh to start all services"
else
    print_error "Found $ERRORS critical errors"
    echo ""
    echo "Please run ./scripts/setup-mac.sh to fix the issues"
    exit 1
fi

#!/bin/bash

# ========================================
# Start Platform Frontend Service
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

print_status "Starting Platform Frontend..."

# Kill any existing process on port 3000
if lsof -Pi :3000 -sTCP:LISTEN -t >/dev/null 2>&1; then
    print_status "Stopping existing Platform Frontend process..."
    lsof -ti:3000 | xargs kill -9 2>/dev/null || true
    sleep 1
fi

# Check if node_modules exists
cd queue-management-platform
if [ ! -d "node_modules" ]; then
    print_status "Installing dependencies..."
    npm install
fi

# Start the development server
print_status "Starting Next.js development server..."
npm run dev

# Note: This will run in the foreground. 
# Press Ctrl+C to stop the server.
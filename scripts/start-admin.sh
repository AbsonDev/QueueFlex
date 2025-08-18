#!/bin/bash

# ========================================
# Start Admin Frontend Service
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

print_status "Starting Admin Frontend..."

# Kill any existing process on port 5173
if lsof -Pi :5173 -sTCP:LISTEN -t >/dev/null 2>&1; then
    print_status "Stopping existing Admin Frontend process..."
    lsof -ti:5173 | xargs kill -9 2>/dev/null || true
    sleep 1
fi

# Check if node_modules exists
cd queuemanagement-admin
if [ ! -d "node_modules" ]; then
    print_status "Installing dependencies..."
    npm install
fi

# Start the development server
print_status "Starting development server..."
npm run dev

# Note: This will run in the foreground. 
# Press Ctrl+C to stop the server.
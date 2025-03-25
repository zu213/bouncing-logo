#!/bin/bash

# Check if the project file exists
if [ ! -f "YourProjectName.csproj" ]; then
  echo "Project file not found!"
  exit 1
fi

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore

# Build the project in Release mode
echo "Building the project..."
dotnet build --configuration Release

# Publish the project
echo "Publishing the project..."
dotnet publish -c Release -o ./out

echo "Build and publish complete. Output directory: ./out"
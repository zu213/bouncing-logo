# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the C# project files to the container
COPY . ./

# Restore dependencies (if needed)
RUN dotnet restore

# Build the C# project
RUN dotnet build --configuration Release

# Publish the application to a folder named "out"
RUN dotnet publish -c Release -o /app/out

# Use the .NET runtime image to run the application (if you need to run it)
FROM mcr.microsoft.com/dotnet/runtime:6.0

WORKDIR /app

# Copy the published application from the build container to the runtime container
COPY --from=build /app/out .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "YourAppName.dll"]

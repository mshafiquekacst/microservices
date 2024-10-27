# Use the official .NET Core 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET Core 8 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build /app/out .

# Expose the port the application runs on
# EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "PlatformService.dll"]
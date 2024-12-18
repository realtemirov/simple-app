# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project file and restore the dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . ./



# Add EF Tools for migration
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Run EF migration command
RUN dotnet ef database update

# Build the application
RUN dotnet build -c Release -o /app/build

# Publish the application to a folder
RUN dotnet publish -c Release -o /app/publish

# Use the runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory in the container
WORKDIR /app

# Copy the published application from the build image
COPY --from=build /app/publish .

# Expose the port your app will listen on
EXPOSE 5000

# Set the entry point for the application
ENTRYPOINT ["dotnet", "simple-app.dll"]

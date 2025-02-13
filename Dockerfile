# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application and build it
COPY . ./
RUN dotnet publish -c Release -o /app/out

# List the contents of the output directory
RUN ls -l /app/out

# Use the official ASP.NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose the ports the application runs on
EXPOSE 3016

# Copy the HTTPS certificates
#COPY /path/to/https/certificate.pfx /https/certificate.pfx

# Set the entry point for the container
ENTRYPOINT ["dotnet", "ProductManagement.dll"]

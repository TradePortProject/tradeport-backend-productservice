# ----- build -----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ProductManagement/ProductManagement.csproj ProductManagement/
RUN dotnet restore ProductManagement/ProductManagement.csproj

# Copy only the API project source
COPY ProductManagement/ ProductManagement/
WORKDIR /src/ProductManagement
RUN dotnet publish ProductManagement.csproj -c Release -o /app/publish --no-restore

# ----- runtime -----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "ProductManagement.dll"]

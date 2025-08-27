# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy individual projects
COPY StockService/ ./StockService/
COPY OrderService/ ./OrderService/

# Restore dependency of OrderService
RUN dotnet restore ./OrderService/OrderService.csproj

# Publish both services in separate folders
RUN dotnet publish ./StockService/StockService.csproj -c Release -o /app/out/StockService
RUN dotnet publish ./OrderService/OrderService.csproj -c Release -o /app/out/OrderService

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published outputs from the build stage
COPY --from=build /app/out/StockService ./StockService
COPY --from=build /app/out/OrderService ./OrderService

# Define the input arguments for the OrderService
WORKDIR /app/OrderService
ENTRYPOINT ["dotnet", "OrderService.dll"]

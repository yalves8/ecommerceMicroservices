# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia os projetos individuais
COPY StockService/ ./StockService/
COPY OrderService/ ./OrderService/

# Restaura dependências do OrderService (com ProjectReference para StockService)
RUN dotnet restore ./OrderService/OrderService.csproj

# Publica cada projeto em pasta separada
RUN dotnet publish ./StockService/StockService.csproj -c Release -o /app/out/StockService
RUN dotnet publish ./OrderService/OrderService.csproj -c Release -o /app/out/OrderService

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copia as saídas publicadas
COPY --from=build /app/out/StockService ./StockService
COPY --from=build /app/out/OrderService ./OrderService

# Define o ponto de entrada
WORKDIR /app/OrderService
ENTRYPOINT ["dotnet", "OrderService.dll"]

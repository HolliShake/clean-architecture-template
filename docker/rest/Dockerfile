# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY ../../rest/API/API.csproj API/
COPY ../../rest/APPLICATION/APPLICATION.csproj APPLICATION/
COPY ../../rest/DOMAIN/DOMAIN.csproj DOMAIN/
COPY ../../rest/INFRASTRUCTURE/INFRASTRUCTURE.csproj INFRASTRUCTURE/

RUN dotnet restore "API/API.csproj"

# Copy all source
COPY ../../rest .

# Build
WORKDIR /src/API
RUN dotnet build "API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]

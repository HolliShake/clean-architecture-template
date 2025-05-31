# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY ../../api/API/API.csproj API/
COPY ../../api/APPLICATION/APPLICATION.csproj APPLICATION/
COPY ../../api/DOMAIN/DOMAIN.csproj DOMAIN/
COPY ../../api/INFRASTRUCTURE/INFRASTRUCTURE.csproj INFRASTRUCTURE/

RUN dotnet restore "API/API.csproj"

# Copy all source
COPY ../../api .

# Build
WORKDIR /src/API
RUN dotnet build "API.csproj" -c Release -o /app/build

# Install EF tools in the build stage
RUN dotnet tool install --global dotnet-ef --version 8.*

# Publish
FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Copy management scripts with correct path syntax for Docker
COPY ../../api/manage.sh  .
COPY ../../api/manage.bat .

# Install Python
RUN apt-get update && apt-get install -y python3

# Install dotnet-ef and make it available in PATH
RUN dotnet tool install --global dotnet-ef --version 8.*
ENV PATH="${PATH}:/root/.dotnet/tools"

# Create a symbolic link to make it accessible as a direct command
RUN ln -s /root/.dotnet/tools/dotnet-ef /usr/local/bin/dotnet-ef

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]

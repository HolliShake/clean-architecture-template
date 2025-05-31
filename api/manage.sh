#!/bin/bash

# ===== Define Constants =====
API_PROJECT="API"
INFRASTRUCTURE_PROJECT="INFRASTRUCTURE"
BUILD_CMD="dotnet build -c release"
MIGRATION_ADD_CMD="dotnet-ef migrations add"
MIGRATION_UPDATE_CMD="dotnet-ef database update"
MIGRATION_REMOVE_CMD="dotnet-ef migrations remove"
PROJECT_ARG="--project ./src/$INFRASTRUCTURE_PROJECT --startup-project ./src/$API_PROJECT"

# ===== Build Command =====
if [ "$1" = "--build" ]; then
    echo "Building project in Release mode..."
    $BUILD_CMD $API_PROJECT
    exit 0
fi

# ===== Migration Commands =====
if [ "$1" = "--add-migration" ]; then
    if [ "$2" = "--name" ]; then
        if [ -n "$3" ]; then
            echo "Creating migration '$3'..."
            $MIGRATION_ADD_CMD "$3" $PROJECT_ARG
        else
            echo "ERROR: Migration name not provided"
            echo "USAGE: ./manage.bash --add-migration --name <MigrationName>"
        fi
    else
        echo "ERROR: Missing --name parameter"
        echo "USAGE: ./manage.bash --add-migration --name <MigrationName>"
    fi
    exit 0
fi

if [ "$1" = "--migrate" ]; then
    echo "Applying pending migrations to database..."
    $MIGRATION_UPDATE_CMD $PROJECT_ARG
    exit 0
fi

if [ "$1" = "--remove-migration" ]; then
    echo "Removing the last migration..."
    $MIGRATION_REMOVE_CMD $PROJECT_ARG
    exit 0
fi

# ===== Help Section =====
if [ -z "$1" ]; then
    echo "USAGE:"
    echo "  ./manage.bash --build               : Build the project in Release mode"
    echo "  ./manage.bash --add-migration --name <MigrationName> : Add a new migration"
    echo "  ./manage.bash --migrate             : Update database with pending migrations"
    echo "  ./manage.bash --remove-migration    : Remove the last migration"
fi

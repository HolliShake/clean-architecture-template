@echo off

rem ===== Define Constants =====
set "API_PROJECT=API"
set "INFRASTRUCTURE_PROJECT=INFRASTRUCTURE"
set "BUILD_CMD=dotnet build -c release"
set "MIGRATION_ADD_CMD=dotnet-ef migrations add"
set "MIGRATION_UPDATE_CMD=dotnet-ef database update"
set "MIGRATION_REMOVE_CMD=dotnet-ef migrations remove"
set "PROJECT_ARG=--project %INFRASTRUCTURE_PROJECT% --startup-project %API_PROJECT%"

rem ===== Build Command =====
if "%1"=="--build" (
    echo Building project in Release mode...
    %BUILD_CMD% %API_PROJECT%
    goto :eof
)

rem ===== Migration Commands =====
if "%1"=="--add-migration" (
    if "%2"=="--name" (
        if not "%3"=="" (
            echo Creating migration '%3'...
            %MIGRATION_ADD_CMD% %3 %PROJECT_ARG%
        ) else (
            echo ERROR: Migration name not provided
            echo USAGE: manage.bat --add-migration --name ^<MigrationName^>
        )
    ) else (
        echo ERROR: Missing --name parameter
        echo USAGE: manage.bat --add-migration --name ^<MigrationName^>
    )
    goto :eof
)

if "%1"=="--migrate" (
    echo Applying pending migrations to database...
    %MIGRATION_UPDATE_CMD% %PROJECT_ARG%
    goto :eof
)

if "%1"=="--remove-migration" (
    echo Removing the last migration...
    %MIGRATION_REMOVE_CMD% %PROJECT_ARG%
    goto :eof
)

rem ===== Help Section =====
if "%1"=="" (
    echo USAGE:
    echo   manage.bat --build               : Build the project in Release mode
    echo   manage.bat --add-migration --name ^<MigrationName^> : Add a new migration
    echo   manage.bat --migrate             : Update database with pending migrations
    echo   manage.bat --remove-migration    : Remove the last migration
)

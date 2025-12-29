# Use a build argument for the PAT
ARG NUGET_PAT

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY BudgetTracker.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

COPY Src/BudgetTracker.Warehouse/ ./BudgetTracker.Warehouse/
COPY Src/BudgetTracker.Shared/ ./BudgetTracker.Shared/

# Re-declare the build argument to make it available in this stage
ARG NUGET_PAT

# Add the private package source using the build argument
RUN dotnet nuget add source --name "github" "https://nuget.pkg.github.com/Abhiram13/index.json" --username Abhiram13 --password "${NUGET_PAT}" --store-password-in-clear-text

# Copy csproj and restore as distinct layers
# COPY *.csproj ./
# RUN dotnet restore

RUN dotnet restore BudgetTracker.Warehouse/BudgetTracker.Warehouse.csproj
RUN dotnet publish BudgetTracker.Warehouse/BudgetTracker.Warehouse.csproj -c Release -o /out

# Copy everything else and build
# COPY . .
# RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
# COPY --from=build-env /app/out .
COPY --from=build-env /out ./

ENTRYPOINT [ "dotnet", "/app/BudgetTracker.Warehouse.dll" ]
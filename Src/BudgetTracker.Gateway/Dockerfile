ARG NUGET_PAT
ARG DOTNET_VERSION=10.0-alpine
ARG APP_ENV

# Build Base Image with SDK
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS base
ARG APP_ENV
ARG NUGET_PAT

# Create and Set working directory inside docker container
WORKDIR /budget-tracker

# Run restore command inside container
RUN dotnet nuget add source --name "github" "https://nuget.pkg.github.com/Abhiram13/index.json" --username Abhiram13 --password "${NUGET_PAT}" --store-password-in-clear-text

FROM base AS restore

COPY BudgetTracker.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY Src/BudgetTracker.Gateway/ ./Src/BudgetTracker.Gateway/
COPY Src/BudgetTracker.Shared/ ./Src/BudgetTracker.Shared/
COPY Src/sharedsettings.json ./Src/sharedsettings.json
COPY Src/sharedsettings.${APP_ENV}.json ./Src/sharedsettings.${APP_ENV}.json

RUN dotnet restore Src/BudgetTracker.Gateway/BudgetTracker.Gateway.csproj

FROM restore AS dev

#COPY . .

WORKDIR /budget-tracker/Src/BudgetTracker.Gateway

CMD ["dotnet", "watch", "run"]

FROM restore AS build

RUN dotnet build Src/BudgetTracker.Gateway/BudgetTracker.Gateway.csproj -c Release --no-restore

FROM build AS publish

ARG APP_ENV

#ENV ASPNETCORE_ENVIRONMENT=$APP_ENV

COPY Src/sharedsettings.json ./Src/sharedsettings.json
COPY Src/sharedsettings.${APP_ENV}.json ./Src/sharedsettings.${APP_ENV}.json

RUN dotnet publish Src/BudgetTracker.Gateway/BudgetTracker.Gateway.csproj -c Release -o /out --no-build

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime

#ARG APP_ENV

#ENV ASPNETCORE_ENVIRONMENT=$APP_ENV

WORKDIR /budget-tracker

COPY --from=publish /out ./

ENTRYPOINT [ "dotnet", "BudgetTracker.Gateway.dll" ]
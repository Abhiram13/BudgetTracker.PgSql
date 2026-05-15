# Use a build argument for the PAT
#ARG NUGET_PAT
#ARG DOTNET_VERSION=10.0
#ARG APP_ENV
#
#FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build-env
#
#ARG APP_ENV
#
#RUN echo "$APP_ENV"
#
#RUN if [ -z "$APP_ENV" ]; then \
#    echo "ERROR: APP_ENV build argument is required (e.g., --build-arg APP_ENV=Production)"; \
#    exit 1; \
#fi
#
#WORKDIR /app
#
#COPY BudgetTracker.sln ./
#COPY Directory.Build.props ./
#COPY Directory.Packages.props ./
#COPY Src/BudgetTracker.Gateway/ ./BudgetTracker.Gateway/
#COPY Src/BudgetTracker.Shared/ ./BudgetTracker.Shared/
#COPY Src/sharedsettings.json ./sharedsettings.json
#COPY Src/sharedsettings.${APP_ENV}.json ./sharedsettings.${APP_ENV}.json
#
## Re-declare the build argument to make it available in this stage
#ARG NUGET_PAT
#ARG DOTNET_VERSION=10.0
#
## Add the private package source using the build argument
#RUN dotnet nuget add source --name "github" "https://nuget.pkg.github.com/Abhiram13/index.json" --username Abhiram13 --password "${NUGET_PAT}" --store-password-in-clear-text
#
## Copy csproj and restore as distinct layers
## COPY *.csproj ./
## RUN dotnet restore
#
#RUN dotnet restore BudgetTracker.Gateway/BudgetTracker.Gateway.csproj
#RUN dotnet publish BudgetTracker.Gateway/BudgetTracker.Gateway.csproj -c Release -o /out
#
## Copy everything else and build
## COPY . .
## RUN dotnet publish -c Release -o out
#
## Build runtime image
#FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}
#WORKDIR /app
## COPY --from=build-env /app/out .
#COPY --from=build-env /out ./
#
#ENTRYPOINT [ "dotnet", "/app/BudgetTracker.Gateway.dll" ]

ARG NUGET_PAT
ARG DOTNET_VERSION=10.0
ARG APP_ENV

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS base

ARG APP_ENV
ARG NUGET_PAT

WORKDIR /budget-tracker

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
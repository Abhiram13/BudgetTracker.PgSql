ARG NUGET_PAT
ARG DOTNET_VERSION=10.0-alpine
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
COPY Src/BudgetTracker.Finance/ ./Src/BudgetTracker.Finance/
COPY Src/BudgetTracker.Shared/ ./Src/BudgetTracker.Shared/
COPY Src/sharedsettings.json ./Src/sharedsettings.json
COPY Src/sharedsettings.${APP_ENV}.json ./Src/sharedsettings.${APP_ENV}.json
RUN dotnet restore Src/BudgetTracker.Finance/BudgetTracker.Finance.csproj

FROM restore AS dev

WORKDIR /budget-tracker/Src/BudgetTracker.Finance
CMD ["dotnet", "watch", "run"]

FROM restore AS build

RUN dotnet build Src/BudgetTracker.Finance/BudgetTracker.Finance.csproj -c Release --no-restore

FROM build AS publish

ARG APP_ENV
COPY Src/sharedsettings.json ./Src/sharedsettings.json
COPY Src/sharedsettings.${APP_ENV}.json ./Src/sharedsettings.${APP_ENV}.json
RUN dotnet publish Src/BudgetTracker.Finance/BudgetTracker.Finance.csproj -c Release -o /out --no-build

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime

WORKDIR /budget-tracker
COPY --from=publish /out ./
ENTRYPOINT [ "dotnet", "BudgetTracker.Finance.dll" ]
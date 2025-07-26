# Specifies the base image for running the ASP.NET Core app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Specifies the build environment image, which contains the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/CanteenAutomation.Api/CanteenAutomation.Api.csproj", "src/CanteenAutomation.Api/"]
COPY ["src/CanteenAutomation.Domain/CanteenAutomation.Domain.csproj", "src/CanteenAutomation.Domain/"]
COPY ["src/CanteenAutomation.Infrastructure/CanteenAutomation.Infrastructure.csproj", "src/CanteenAutomation.Infrastructure/"]
RUN dotnet restore "src/CanteenAutomation.Api/CanteenAutomation.Api.csproj"
COPY . .
WORKDIR "/src/src/CanteenAutomation.Api"
RUN dotnet build "CanteenAutomation.Api.csproj" -c Release -o /app/build

# Publishes the application
FROM build AS publish
RUN dotnet publish "CanteenAutomation.Api.csproj" -c Release -o /app/publish

# Creates the final, smaller image for deployment
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CanteenAutomation.Api.dll"]

# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8180
EXPOSE 8181


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Operon.Api/Operon.Api.csproj", "src/Operon.Api/"]
COPY ["src/Operon.Application/Operon.Application.csproj", "src/Operon.Application/"]
COPY ["src/Operon.Domain/Operon.Domain.csproj", "src/Operon.Domain/"]
COPY ["src/Operon.Infrastructure/Operon.Infrastructure.csproj", "src/Operon.Infrastructure/"]
RUN dotnet restore "./src/Operon.Api/Operon.Api.csproj"
COPY . .
WORKDIR "/src/src/Operon.Api"
RUN dotnet build "./Operon.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Operon.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Operon.Api.dll"]
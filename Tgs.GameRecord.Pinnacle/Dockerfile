#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Pinnacle/Tgs.GameRecord.Pinnacle.csproj", "Tgs.GameRecord.Pinnacle/"]
COPY ["Tgs.GameRecord.Pinnacle.Core/Tgs.GameRecord.Pinnacle.Core.csproj", "Tgs.GameRecord.Pinnacle.Core/"]
COPY ["Tgs.GameRecord.Pinnacle.Common/Tgs.GameRecord.Pinnacle.Common.csproj", "Tgs.GameRecord.Pinnacle.Common/"]
RUN dotnet restore "./Tgs.GameRecord.Pinnacle/./Tgs.GameRecord.Pinnacle.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Pinnacle"
RUN dotnet build "./Tgs.GameRecord.Pinnacle.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.Pinnacle.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Pinnacle.dll"]
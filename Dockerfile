# syntax=docker/dockerfile:1.4
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Kopieer solution file
COPY ["dotNIES.API.sln", "./"]

# Kopieer project files uit huidige directory
COPY ["dotNIES.API/dotNIES.API.csproj", "dotNIES.API/"]
COPY ["dotNIES.API.Core/dotNIES.API.Core.csproj", "dotNIES.API.Core/"]

# Kopieer externe project files
COPY --from=extern ["dotNIES.Data.Db/dotNIES.Data.Db.csproj", "dotNIES.Data.Db/"]
COPY --from=extern ["dotNIES.Data.Dto/dotNIES.Data.Dto.csproj", "dotNIES.Data.Dto/"]
COPY --from=extern ["dotNIES.Data.Logging/dotNIES.Data.Logging.csproj", "dotNIES.Data.Logging/"]

# Restore packages
RUN dotnet restore

# Kopieer alle bestanden van huidige directory
COPY . ./

# Kopieer externe project bestanden
COPY --from=extern ["/dotNIES.Data.Db/", "./dotNIES.Data.Db/"]
COPY --from=extern ["/dotNIES.Data.Dto/", "./dotNIES.Data.Dto/"]
COPY --from=extern ["/dotNIES.Data.Logging/", "./dotNIES.Data.Logging/"]

# Build en publish in één stap
WORKDIR "/src/dotNIES.API"
RUN dotnet publish "dotNIES.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image - gebruik kleinere runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Gezondheidscheck voor Railway
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:80/ || exit 1

# Configuratie voor Railway
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 80
ENTRYPOINT ["dotnet", "dotNIES.API.dll"]
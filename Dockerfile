﻿# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopieer csproj en restore
COPY dotNIES.API/*.csproj ./dotNIES.API/
RUN dotnet restore ./dotNIES.API/dotNIES.API.csproj

# Kopieer de rest van de app
COPY dotNIES.API/. ./dotNIES.API/
WORKDIR /src/dotNIES.API
RUN dotnet publish -c Release -o /app/publish

# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "dotNIES.API.dll"]

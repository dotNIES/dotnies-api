FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Kopieer csproj en restore packages
COPY *.csproj ./
RUN dotnet restore

# Kopieer rest van de code en build
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "dotNIES.API.dll"]
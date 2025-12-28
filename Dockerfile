FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install SSL certificates
RUN apt-get update && apt-get install -y ca-certificates curl && update-ca-certificates

COPY ["src/NflPlayoffPool.Web/NflPlayoffPool.Web.csproj", "NflPlayoffPool.Web/"]
COPY ["src/NflPlayoffPool.Data/NflPlayoffPool.Data.csproj", "NflPlayoffPool.Data/"]
RUN dotnet restore "NflPlayoffPool.Web/NflPlayoffPool.Web.csproj"

COPY src/ .
RUN dotnet build "NflPlayoffPool.Web/NflPlayoffPool.Web.csproj" -c Release -o /app/build --no-cache
RUN dotnet publish "NflPlayoffPool.Web/NflPlayoffPool.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install SSL certificates in the final image
RUN apt-get update && apt-get install -y ca-certificates curl && update-ca-certificates

COPY --from=build /app/publish .
EXPOSE 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "NflPlayoffPool.Web.dll"]

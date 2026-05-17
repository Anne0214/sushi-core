# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Restaurant.API/Restaurant.API.csproj", "Restaurant.API/"]
RUN dotnet restore "Restaurant.API/Restaurant.API.csproj"

COPY . .
RUN dotnet publish "Restaurant.API/Restaurant.API.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Restaurant.API.dll"]

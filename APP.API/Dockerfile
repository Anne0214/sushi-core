# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["APP.API/APP.API.csproj", "APP.API/"]
COPY ["ApiGateway.Application/ApiGateway.Application.csproj", "ApiGateway.Application/"]
COPY ["ApiGateway.Infrasturcture/ApiGateway.Infrastructure.csproj", "ApiGateway.Infrasturcture/"]
COPY ["Sushi.All.Infrastructure.Web/Sushi.All.Infrastructure.Web.csproj", "Sushi.All.Infrastructure.Web/"]
COPY ["Sushi.All.Core/Sushi.All.Core.csproj", "Sushi.All.Core/"]
COPY ["Sushi.All.Infrastructure/Sushi.All.Infrastructure.csproj", "Sushi.All.Infrastructure/"]
COPY ["ApiGateway.Domain/ApiGateway.Domain.csproj", "ApiGateway.Domain/"]

RUN dotnet restore "APP.API/APP.API.csproj"

COPY . .

RUN dotnet publish "APP.API/APP.API.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5005

ENTRYPOINT ["dotnet", "APP.API.dll"]

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Member.API/Member.API.csproj", "Member.API/"]
RUN dotnet restore "Member.API/Member.API.csproj"

COPY . .
RUN dotnet publish "Member.API/Member.API.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Member.API.dll"]

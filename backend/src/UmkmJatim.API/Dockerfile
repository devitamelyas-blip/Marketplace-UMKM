# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["UmkmJatim.Domain/UmkmJatim.Domain.csproj", "UmkmJatim.Domain/"]
COPY ["UmkmJatim.Application/UmkmJatim.Application.csproj", "UmkmJatim.Application/"]
COPY ["UmkmJatim.Infrastructure/UmkmJatim.Infrastructure.csproj", "UmkmJatim.Infrastructure/"]
COPY ["UmkmJatim.API/UmkmJatim.API.csproj", "UmkmJatim.API/"]

RUN dotnet restore "UmkmJatim.API/UmkmJatim.API.csproj"

# Copy full source and publish
COPY . .
WORKDIR "/src/UmkmJatim.API"
RUN dotnet publish "UmkmJatim.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
RUN mkdir -p /app/uploads

ENTRYPOINT ["dotnet", "UmkmJatim.API.dll"]

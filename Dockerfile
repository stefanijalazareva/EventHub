# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy solution and project files
COPY EventHub.sln .
COPY src/EventHub.Domain/EventHub.Domain.csproj src/EventHub.Domain/
COPY src/EventHub.Repository/EventHub.Repository.csproj src/EventHub.Repository/
COPY src/EventHub.Service/EventHub.Service.csproj src/EventHub.Service/
COPY src/EventHub.Web/EventHub.Web.csproj src/EventHub.Web/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build the Web project
WORKDIR /source/src/EventHub.Web
RUN dotnet publish -c Release -o /app

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

# Create directory for SQLite database
RUN mkdir -p /var/data

# Expose port
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "EventHub.Web.dll"]

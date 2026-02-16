# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AlertHub.sln ./
COPY AlertHub/AlertHub.csproj AlertHub/
COPY AlertHub.Tests/AlertHub.Tests.csproj AlertHub.Tests/
RUN dotnet restore AlertHub.sln

COPY . .
RUN dotnet publish AlertHub/AlertHub.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "AlertHub.dll"]

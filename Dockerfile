FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["TeamTaskTracking.Web.csproj", "./"]
RUN dotnet restore "TeamTaskTracking.Web.csproj"

COPY . .
RUN dotnet publish "TeamTaskTracking.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["sh", "-c", "dotnet TeamTaskTracking.Web.dll --urls http://0.0.0.0:${PORT:-8080}"]

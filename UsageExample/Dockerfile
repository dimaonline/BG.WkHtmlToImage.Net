FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UsageExample/UsageExample.csproj", "UsageExample/"]
RUN dotnet restore "UsageExample/UsageExample.csproj"
COPY . .
WORKDIR "/src/UsageExample"
RUN dotnet build "UsageExample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UsageExample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsageExample.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Backend.API/Backend.API.csproj", "Backend.API/"]
RUN dotnet restore "Backend.API/Backend.API.csproj"
COPY . .
WORKDIR "/src/Backend.API"
RUN dotnet build "Backend.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Backend.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.API.dll"]

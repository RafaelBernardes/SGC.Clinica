FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SGC.Clinica.API.csproj", "."]

RUN dotnet restore "SGC.Clinica.API.csproj"

COPY . .

RUN dotnet build "SGC.Clinica.API.csproj" -c Release -o /app/build
RUN dotnet publish "SGC.Clinica.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5000 5001

ENTRYPOINT ["dotnet", "SGC.Clinica.API.dll"]

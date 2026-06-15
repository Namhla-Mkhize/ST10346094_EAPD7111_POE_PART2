FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TechMove.API/TechMove.API.csproj", "TechMove.API/"]
COPY ["TechMove.Web/TechMove.Web.csproj", "TechMove.Web/"]
RUN dotnet restore "TechMove.API/TechMove.API.csproj"
COPY . .
WORKDIR "/src/TechMove.API"
RUN dotnet publish "TechMove.API.csproj" -c Release -o /app/publish /p:ErrorOnDuplicatePublishOutputFiles=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechMove.API.dll"]
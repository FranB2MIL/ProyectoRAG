FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ProyectoRAG.WebApi/ProyectoRAG.WebApi.csproj", "ProyectoRAG.WebApi/"]
COPY ["ProyectoRAG.Application/ProyectoRAG.Application.csproj", "ProyectoRAG.Application/"]
COPY ["ProyectoRAG.Infrastructure/ProyectoRAG.Infrastructure.csproj", "ProyectoRAG.Infrastructure/"]
COPY ["ProyectoRAG.Domain/ProyectoRAG.Domain.csproj", "ProyectoRAG.Domain/"]
RUN dotnet restore "ProyectoRAG.WebApi/ProyectoRAG.WebApi.csproj"

COPY . .
WORKDIR "/src/ProyectoRAG.WebApi"
RUN dotnet build "ProyectoRAG.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProyectoRAG.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProyectoRAG.WebApi.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


ENV ASPNETCORE_URLS=http://+:8080
ENV DISABLE_HTTPS_REDIRECT=true
ENV ASPNETCORE_ENVIRONMENT=Production


EXPOSE 8080



FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Restaura primeiro para aproveitar cache
COPY ["LixoZero.csproj", "./"]
RUN dotnet restore "./LixoZero.csproj"

# Copia tudo e compila
COPY . .
RUN dotnet build "./LixoZero.csproj" -c $BUILD_CONFIGURATION -o /app/build



FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LixoZero.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false



FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "LixoZero.dll"]

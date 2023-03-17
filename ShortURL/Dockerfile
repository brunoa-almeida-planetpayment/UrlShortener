FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ShortURL/ShortURL.csproj", "ShortURL/"]
RUN dotnet restore "ShortURL/ShortURL.csproj"
COPY . .
WORKDIR "/src/ShortURL"
RUN dotnet build "ShortURL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShortURL.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShortURL.dll"]

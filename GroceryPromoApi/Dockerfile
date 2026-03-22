FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["GroceryPromoApi/GroceryPromoApi.csproj", "GroceryPromoApi/"]
COPY ["GroceryPromoApi.Application/GroceryPromoApi.Application.csproj", "GroceryPromoApi.Application/"]
COPY ["GroceryPromoApi.Domain/GroceryPromoApi.Domain.csproj", "GroceryPromoApi.Domain/"]
COPY ["GroceryPromoApi.Infrastructure/GroceryPromoApi.Infrastructure.csproj", "GroceryPromoApi.Infrastructure/"]

RUN dotnet restore "GroceryPromoApi/GroceryPromoApi.csproj"

COPY . .

WORKDIR "/src/GroceryPromoApi"
RUN dotnet build "GroceryPromoApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GroceryPromoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GroceryPromoApi.dll"]

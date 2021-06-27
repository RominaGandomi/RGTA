#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Product.WebApi/Product.WebApi.csproj", "Product.WebApi/"]
RUN dotnet restore "Product.WebApi/Product.WebApi.csproj"
COPY . .
WORKDIR "/src/Product.WebApi"
RUN dotnet build "Product.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Product.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.WebApi.dll"]
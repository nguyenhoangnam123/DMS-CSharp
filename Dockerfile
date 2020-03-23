#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["DMS.BE.csproj", ""]
RUN dotnet restore "DMS.BE.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "DMS.BE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DMS.BE.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=consul:latest /bin/consul /usr/local/bin/consul
COPY --from=envoyproxy/envoy-alpine /usr/local/bin/envoy /usr/local/bin/envoy

COPY --from=publish /app/publish .

COPY ./docker-entrypoint.sh ./
RUN  chmod a+x docker-entrypoint.sh

#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base
WORKDIR /app

RUN apt-get update && apt-get install -y net-tools curl iputils-ping telnetd nano vim libc6-dev libgdiplus

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["DMS.csproj", "./"]
RUN dotnet restore "DMS.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "DMS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DMS.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app

COPY --from=truesight.asia:8443/consul:stable /bin/consul /usr/local/bin/consul
COPY --from=truesight.asia:8443/envoy:stable /usr/local/bin/envoy /usr/local/bin/envoy

COPY /consul /consul/config 

COPY --from=publish /app/publish .

COPY ["docker-entrypoint.sh", "."]

RUN chmod a+x docker-entrypoint.sh

CMD ["./docker-entrypoint.sh"]

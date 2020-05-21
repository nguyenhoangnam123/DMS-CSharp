#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM truesight.asia:8443/aspnet:3.1-buster-slim AS base
WORKDIR /app

RUN apt-get install -y net-tools
RUN apt-get install -y curl
RUN apt-get install -y iputils-ping
RUN apt-get install -y telnetd
RUN apt-get install -y nano
RUN apt-get install -y vim
RUN apt-get install -y libc6-dev libgdiplus

FROM truesight.asia:8443/dotnet-sdk:3.1-buster AS build
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

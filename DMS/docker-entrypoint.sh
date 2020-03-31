#!/bin/bash

PROJECT_NAME=DMS

consul agent -config-dir /consul/config &
sleep 5
consul connect envoy -sidecar-for rd-auth &
sleep 5
dotnet $PROJECT_NAME.dll --environment Development

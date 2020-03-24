#!/bin/bash

PROJECT_NAME=DMS.BE

consul agent -config-dir /consul/config &
sleep 5
consul connect envoy -sidecar-for rd-auth &
sleep 5
dotnet $PROJECT_NAME.dll --urls http://0.0.0.0:5003 --environment Development --launch-profile $PROJECT_NAME

#!/bin/bash
sed -i "s/{SECRET_KEY}/${SECRET_KEY}/g" appsettings.json
sed -i "s/{SQL_IP}/${SQL_IP}/g" appsettings.json
sed -i "s/{SQL_DB}/${SQL_DB}/g" appsettings.json
sed -i "s/{SQL_USER}/${SQL_USER}/g" appsettings.json
sed -i "s/{SQL_PASS}/${SQL_PASS}/g" appsettings.json
sed -i "s/{PUBLIC_RSA_KEY}/${PUBLIC_RSA_KEY}/g" appsettings.json

PROJECT_NAME=DMS
dotnet ${PROJECT_NAME}.dll --urls http://0.0.0.0:8080 --launch-profile ${MODE}


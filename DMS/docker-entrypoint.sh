#!/bin/bash
sed -i "s/{SECRET_KEY}/${SECRET_KEY}/g" appsettings.json
sed -i "s/{SQL_IP}/${SQL_IP}/g" appsettings.json
sed -i "s/{SQL_DB}/${SQL_DB}/g" appsettings.json
sed -i "s/{SQL_USER}/${SQL_USER}/g" appsettings.json
sed -i "s/{SQL_PASS}/${SQL_PASS}/g" appsettings.json
sed -i "s/{RB_USER}/${RB_USER}/g" appsettings.json
sed -i "s/{RB_PASS}/${RB_PASS}/g" appsettings.json

dotnet ${PROJECT_NAME}.dll --urls http://0.0.0.0:80 --launch-profile ${MODE}


#!/bin/bash
if [ -z ${NODE} ]; then
	    NODE="dms-backend_${HOSTNAME}"
fi

PROJECT_NAME="DMS"

dotnet ${PROJECT_NAME}.dll --urls http://localhost:10002 --environment Development --launch-profile ${PROJECT_NAME} &
consul agent -config-dir /consul/config -node ${NODE}&
sleep 5
consul connect envoy -sidecar-for dms-backend


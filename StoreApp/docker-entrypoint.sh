#!/bin/bash
sed -i "s/{SECRET_KEY}/${SECRET_KEY}/g" appsettings.json
sed -i "s/{SQL_DB}/${SQL_DB}/g" appsettings.json
sed -i "s/{SQL_USER}/${SQL_USER}/g" appsettings.json
sed -i "s/{SQL_PASS}/${SQL_PASS}/g" appsettings.json
sed -i "s/{RB_USER}/${RB_USER}/g" appsettings.json
sed -i "s/{RB_PASS}/${RB_PASS}/g" appsettings.json

if [ -z ${NODE} ]; then
	    NODE="store-app-backend_${HOSTNAME}"
fi

# setting up SIGTERM handler for consul agent
CONSUL_PID=0
term_handler () {
    if [ ${CONSUL_PID} -ne 0 ]; then
        consul force-leave ${NODE}
        kill "${CONSUL_PID}"
        wait "${CONSUL_PID}"
    fi
    exit 143;
}
trap term_handler TERM

PROJECT_NAME="StoreApp"

getStatus () {
    netstat -ant | awk '/LISTEN/ { print $4 }' | rev | cut -f1 -d: | rev | grep -w $1 && return 0
    return 1
}

consul agent -config-dir /consul/config -node ${NODE} &
CONSUL_PID="$!"
sleep 5
consul connect envoy -sidecar-for dms-backend &
sleep 10

while [ 1 ]; do
    getStatus 80
    if [ $? -eq 0 ]; then
        break
    fi
    dotnet ${PROJECT_NAME}.dll --urls http://0.0.0.0:80 --launch-profile ${MODE} &
    sleep 5
done

wait "${CONSUL_PID}"


#!/bin/bash
if [ -z ${NODE} ]; then
	    NODE="dms-backend_${HOSTNAME}"
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

PROJECT_NAME="DMS"

consul agent -config-dir /consul/config -node ${NODE} &
CONSUL_PID="$!"
sleep 5
dotnet ${PROJECT_NAME}.dll --urls http://localhost:80 --environment Production &
sleep 5
consul connect envoy -sidecar-for dms-backend &
wait "${CONSUL_PID}"


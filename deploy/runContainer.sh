#!/bin/bash

#
# Start the API container for a release. On retry, reuse the container if its
# image matches and connect it to any missing deployment networks.
#
# Usage:
#   bash runContainer.sh "$containerName" "$imageRepository" "$uniqueifier" \
#       "$environment" "$rootPath" "$appsettingsDir" "$port"
#

set -eu

containerName=$1
image="$2:$3"
uniqueifier=$3
environment=$4
rootPath=$5
appsettingsDir=$6
port=$7

export MSYS_NO_PATHCONV=1

# Include stopped and partially configured containers from an earlier attempt.
containerId=$(docker container ls --all --quiet --no-trunc --filter "name=^/${containerName}$")
if [[ -z "$containerId" ]]; then
    containerId=$(docker container create \
        --name "$containerName" \
        --restart always \
        --log-driver fluentd \
        --log-opt fluentd-address=localhost:24224 \
        --log-opt fluentd-sub-second-precision=true \
        --log-opt fluentd-async=true \
        --log-opt "tag=pisearch.api.$environment" \
        -v "$rootPath:/var/www/pi_digits:ro" \
        -v "$appsettingsDir/appsettings.$environment.json:/app/appsettings.$environment.json" \
        -e "ASPNETCORE_ENVIRONMENT=$environment" \
        -e "ASPNETCORE_URLS=http://*:$port" \
        -e "UNIQUEIFIER=$uniqueifier" \
        "$image")
else
    existingImage=$(docker container inspect --format '{{.Config.Image}}' "$containerId")
    if [[ "$existingImage" != "$image" ]]; then
        echo "Container $containerName uses $existingImage. Deployment requires $image." >&2
        exit 1
    fi
fi

for network in reverseProxy mysql; do
    connected=$(docker container inspect --format "{{if index .NetworkSettings.Networks \"$network\"}}yes{{end}}" "$containerId")
    if [[ "$connected" != yes ]]; then
        docker network connect "$network" "$containerId"
    fi
done

docker start "$containerId"

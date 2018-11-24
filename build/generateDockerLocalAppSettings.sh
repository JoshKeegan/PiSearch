#!/bin/bash

#
# Generates the appsettings json for use when running in Docker locally.
#	Gets outputted into the artefacts/local dir	
#

cp ../StringSearch.Api/appsettings.DockerLocal.json ../artefacts/local/appsettings.DockerLocal.json

# On some platforms, host.docker.internal must be replaced with an actual IP
hostIp=""

# Linux
if [ "`uname`" == "Linux" ];
then
	# Use IPv4 of docker0 iface which must exist
	hostIp="`ip -4 addr show docker0 | grep -oP '(?<=inet\s)\d+(\.\d+){3}'`"
# Windows if running Docker Toolbox
elif [ "$DOCKER_TOOLBOX_INSTALL_PATH" != "" ]
then
	# Use the first IPv4 result of ipconfig
	hostIp="`ipconfig | grep IPv4 | awk 'NR==1 {split($$0, a, ": "); print a[2];}'`"
fi

# If we've evaluated a host IP, do the replacement
if [ "$hostIp" != "" ];
then
	echo "Using Host IP $hostIp for docker to connect to the host machine"

	sed s/host.docker.internal/$hostIp/g -i ../artefacts/local/appsettings.DockerLocal.json
fi
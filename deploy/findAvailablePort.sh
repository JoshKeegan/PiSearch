#!/bin/bash

#
# Finds an available port to use.
#	Note that in the time between finding a free port and going to use it, it could get 
#	taken, so wherever possible use port 0 so that the kernel assigns you one.
#	This script is for when you absolutely must know in advance, or so that it can be persisted between
#	restarts etc...
#
# Usage: Run script. Will print port number to use to stdout
#		 Can be used programatically by capturing the output as a variable, e.g. freePort=`/bin/bash findAvailablePort.sh`
#		 Can debug by passing the -v flag. This will print more to stdout. If using programatically the final line will be the free port found.
#

verbose="false"
if [ "$1" = "-v" ]; then
	verbose="true"
fi

verboseEcho()
{
	if [ "$verbose" = "true" ]; then
		echo "$1"
	fi
}

read minPort maxPort < /proc/sys/net/ipv4/ip_local_port_range

verboseEcho "Searching for an available port in the range $minPort-$maxPort"

while :; do
	for (( port = minPort ; port <= maxPort ; port++ )); do
		verboseEcho "Trying port $port . . ."

		portTest=`netstat -lt | grep ":$port "`
		if [ -z "$portTest" ]; then
			verboseEcho "Found free port $port"
			echo $port
			exit 0
		fi
	done
done
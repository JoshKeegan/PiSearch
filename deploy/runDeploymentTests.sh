#!/bin/bash

#
# Run Dpeloyment Tests
#
# Usage: ./runDeploymentTests.sh hostname port

# Exit on error
set -e

readonly hostname=$1
readonly port=$2

if [ -z "$hostname" ] || [ -z "$port" ]; then
	echo "Missing required params" >&2
	echo 'Usage: ./runDeploymentTests.sh hostname port' >&2
	exit 1
fi

readonly uri="http://$hostname:$port"
# Note: If running on Windows via Mingw, this curl will give a non-zero exit code. It doesn't like
#	--output /dev/null. If ever this is needed to work on Windows, that will need looking at.
readonly curl="curl -L --silent --fail --output /dev/null --write-out '%{http_code}: %{url_effective}\n'"

# Health check
eval $curl $uri/api/Health

# Quick happy path
eval $curl $uri/api/Lookup?find=230893
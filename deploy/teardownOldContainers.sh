#!/bin/bash

#
# Tear down old containers for an environment
#
# Usage:
#	./teardownOldContainers.sh $environment $currentUniqueifier

# Exit on error
set -e

readonly environment="$1"
readonly currentUniqueifier="$2"

if [ -z "$environment" ] || [ -z "$currentUniqueifier" ]; then
	echo "Missing required params" >&2
	echo 'Usage: ./teardownOldContainers.sh $environment $currentUniqueifier' >&2
	exit 1
fi

# Find containers to delete (as newline delimited string)
containerNames=`docker container ls --format {{.Names}} | grep pisearch-$environment- | { grep -v $currentUniqueifier || true; }`

if [ -z "$containerNames" ]; then
	echo "No existing containers for environment $environment"
	exit 0
fi

# Delete them one by one
while read -r containerName; do
	echo "Deleting container $containerName"
	docker container rm -f $containerName
done <<< "$containerNames"

echo "All old containers for environment $environment deleted"
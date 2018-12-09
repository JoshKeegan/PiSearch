#!/bin/bash

#
# Tear down old containers for an environment
#
# Usage:
#	./teardownOldContainers.sh $environment $currentUniqueifier

readonly environment="$1"
readonly currentUniqueifier="$2"

if [ -z "$environment" ] || [ -z "$currentUniqueifier" ]; then
	echo "Missing required params" >&2
	echo 'Usage: ./teardownOldContainers.sh $environment $currentUniqueifier' >&2
	exit 1
fi

# Find containers to delete (as newline delimited string)
containerNames=`docker container ls --format {{.Names}} | grep PiSearch-$environment- | grep -v $currentUniqueifier`

# Delete them one by one
while read -r containerName; do
	echo "Deleting container $containerName"
	docker container rm -f $containerName
done <<< "$containerName"

echo "All old containers for environment $environment deleted"
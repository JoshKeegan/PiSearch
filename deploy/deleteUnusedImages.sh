#!/bin/bash

#
# Delete unused versions of an image
#
# Usage:
#       ./deleteUnusedImages.sh $image
#

# Exit on error
set -e

readonly image="$1"

if [ -z "$image" ]; then
        echo "Missing required params" >&2
        echo 'Usage: ./deleteUnusedImages.sh $image' >&2
        exit 1
fi

# Find images that are currently being used in running containers in format ":repo1/image1:tag1:|:repo2/image2:tag2:", (ready for egrep)
inUse=`docker container ls --format "{{ .Image }}" | \
	grep $image: | \
	awk 'NR != 1 { printf "|" } { printf ":" $$0 ":" }'`

numInUse=`echo $inUse | wc -w`
echo "Found $numInUse running containers using versions of this image, these will be kept"

# Find images to delete (as newline delimited string)
toDelete=`docker image ls --format ":{{ .Repository }}:{{ .Tag }}:{{ .ID }}:" | \
		grep :$image: | \
		grep -v -E "$inUse" | \
		awk '{ split($$0, a, ":"); print a[4] }'`

if [ -z "$toDelete" ]; 
then
	echo "No unused versions of this image to delete"
else
	numImages=`echo $toDelete | wc -w`
	echo "Deleting $numImages unused versions of this image . . ."
	docker rmi -f $toDelete
fi

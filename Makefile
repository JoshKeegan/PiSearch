#
# PiSearch Makefile
#

#
# Constants
#

UNIQUEIFIER_PATH = artefacts/uniqueifier#
IMAGE_API = pi-search-api#
IMAGE_API_URL = joshkeegan/$(IMAGE_API)#
LOCAL_API_DOCKER_PORT = 5002#

clean:
	rm -r */out || true
	rm -r */bin || true
	rm -r */obj || true
	rm -r artefacts || true
	
	mkdir -p \
		artefacts/testResults \
		artefacts/local

#
# Build
#

build: clean
	dotnet build -c Release

unit-tests:
# If not on windows, set a filter to exclude the windows-specific tests
ifeq ($(shell uname), Linux)
	$(eval testFilter = --filter TestCategory!=windows)
endif

	dotnet test \
		-c Release \
		--no-build \
		$(testFilter) \
		--logger:trx\;logfilename=../../artefacts/testResults/UnitTests.trx \
		UnitTests

publish-api: build
	dotnet publish -c Release --no-build -o out StringSearch.Api

#
# Uniqueifier
#

# Args:
#	- buildId (remote only) (gitlab env var CI_JOB_ID)
generate-uniqueifier:
# If running locally
ifeq ($(buildId),)
	date +%Y-%m-%d-%H-%M-%S > $(UNIQUEIFIER_PATH)
else
	echo -n $(buildId)-$(shell git rev-parse --short HEAD) > $(UNIQUEIFIER_PATH)
endif
	
#
# Docker Images
#

build-api-image:
	docker build --pull -t $(IMAGE_API):latest StringSearch.Api

# Args:
#	- buildId (remote only)
tag-api-image: build-api-image generate-uniqueifier
	docker tag $(IMAGE_API):latest $(IMAGE_API_URL):$(shell cat $(UNIQUEIFIER_PATH))

# Args:
#	- buildId (remote only)
publish-api-image: tag-api-image
	docker push $(IMAGE_API_URL):$(shell cat $(UNIQUEIFIER_PATH))

#
# Run locally
#

# Args
#	- rootPath
run-local-api-docker: publish-api build-api-image
ifeq ($(rootPath),)
	$(error rootPath must be specified)
endif
	
	cd build; \
		/bin/bash generateDockerLocalAppSettings.sh

	export MSYS_NO_PATHCONV=1; \
		docker run \
			-it \
			--rm \
			-v "$(rootPath)":/var/www/pi_digits:rw \
			-e ASPNETCORE_ENVIRONMENT="DockerLocal" \
			-e ASPNETCORE_URLS="http://*:$(LOCAL_API_DOCKER_PORT)" \
			-p $(LOCAL_API_DOCKER_PORT):$(LOCAL_API_DOCKER_PORT) \
			-v "${PWD}/artefacts/local/appsettings.DockerLocal.json":/app/appsettings.DockerLocal.json \
			$(IMAGE_API):latest
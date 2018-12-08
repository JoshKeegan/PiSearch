#
# PiSearch Makefile
#

#
# Constants
#

UNIQUEIFIER_PATH = artefacts/uniqueifier#
CONTAINER_REGISTRY_HOSTNAME = registry.gitlab.com#
CONTAINER_REGISTRY = $(CONTAINER_REGISTRY_HOSTNAME)/joshkeegan/pisearch
IMAGE_API = $(CONTAINER_REGISTRY)/pi-search-api#
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

unit-tests: publish-unit-tests
	cd UnitTests; \
		make run

publish-api: build
	dotnet publish -c Release --no-build -o out StringSearch.Api

publish-unit-tests: build
	dotnet publish -c Release --no-build -o out UnitTests

publish-all: publish-api publish-unit-tests

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

# Args:
#	- buildId (remote only)
build-api-image: generate-uniqueifier
	docker build --pull -t $(IMAGE_API):$(shell cat $(UNIQUEIFIER_PATH)) StringSearch.Api

# Args:
#	- crUsername
#	- crPassword
#	- buildId (remote only)
publish-api-image: build-api-image docker-login
	docker push $(IMAGE_API):$(shell cat $(UNIQUEIFIER_PATH))

# Args:
#	- crUsername
#	- crPassword
docker-login:
	@docker login $(CONTAINER_REGISTRY_HOSTNAME) -u $(crUsername) -p $(crPassword)

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
			$(IMAGE_API):$(shell cat $(UNIQUEIFIER_PATH))
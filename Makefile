#
# Solution Makefile
#

#
# Constants
#

UNIQUEIFIER_PATH = artefacts/uniqueifier#
LOCAL_API_DOCKER_PORT = 5002#

include sharedScripts/make/dockerImages.mk

clean:
	rm -r */out || true
	rm -r */bin || true
	rm -r */obj || true
	rm -r artefacts || true
	
	mkdir -p \
		artefacts/testResults \
		artefacts/local

	cp -r deploy artefacts/

#
# Build
#

build: clean
	dotnet build -c Release

unit-tests:
	cd test/StringSearch.Tests.Unit; \
		make run

ci: build unit-tests

publish-api: build
	cd src/StringSearch.Api.Host && \
		dotnet publish -c Release --no-build -o out

publish-all: publish-api

#
# Uniqueifier
#

# Args:
#	- buildId (remote only)
#	- commitHash (remote only - optional)
generate-uniqueifier:
# If running locally
ifeq ($(buildId),)
	date +%Y-%m-%d-%H-%M-%S > $(UNIQUEIFIER_PATH)
else
# If we haven't been supplied with a commit hash, get it from git
ifeq ($(commitHash),)
	$(eval commitHash = $(shell git rev-parse --short HEAD))
endif
	echo -n $(buildId)-$(commitHash) > $(UNIQUEIFIER_PATH)
endif
	
#
# Docker Images
#

# Args:
#	- buildId (remote only)
#	- commitHash (remote only - optional)
build-api-image: generate-uniqueifier
	docker build --pull -t $(IMAGE_API):$(shell cat $(UNIQUEIFIER_PATH)) src/StringSearch.Api.Host

# Args:
#	- crUsername
#	- crPassword
#	- buildId (remote only)
#	- commitHash (remote only - optional)
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
			--rm \
			-v "$(rootPath)":/var/www/pi_digits:ro \
			-e ASPNETCORE_ENVIRONMENT="DockerLocal" \
			-e ASPNETCORE_URLS="http://*:$(LOCAL_API_DOCKER_PORT)" \
			-e UNIQUEIFIER="$(shell cat $(UNIQUEIFIER_PATH))" \
			-p $(LOCAL_API_DOCKER_PORT):$(LOCAL_API_DOCKER_PORT) \
			-v "${PWD}/artefacts/local/appsettings.DockerLocal.json":/app/appsettings.DockerLocal.json:ro \
			$(IMAGE_API):$(shell cat $(UNIQUEIFIER_PATH))
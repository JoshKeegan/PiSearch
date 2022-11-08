#
# Container Testing Makefile
#	The intention is for this to be run in dind in the CI pipeline, so no dependencies
#	should be taken on dotnet etc... docker images should be used for any external dependencies here.
#

###############################################################################
# Constants
###############################################################################
RELATIVE_ARTEFACTS_DIR_PATH = ../../artefacts#
ABSOLUTE_ARTEFACTS_DIR_PATH = $(PWD)/$(RELATIVE_ARTEFACTS_DIR_PATH)#
UNIQUEIFIER_PATH = $(RELATIVE_ARTEFACTS_DIR_PATH)/uniqueifier#

SUT_PORT_NUMBER = 5002#

include ../../sharedScripts/make/dockerImages.mk

###############################################################################
# Cleanup
###############################################################################
clean:
	rm -rf $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting
	rm -rf $(RELATIVE_ARTEFACTS_DIR_PATH)/testResults/ContainerTests.trx

	mkdir -p $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting/digits

###############################################################################
# Run end-to-end
###############################################################################
run-e2e: clean dependency-prep docker-compose-generate sut-start container-tests-run sut-stop

###############################################################################
# Build Docker Images
###############################################################################
build-container-test-image:
	docker build -t $(IMAGE_CONTAINER_TESTS):$(shell cat $(UNIQUEIFIER_PATH)) .

###############################################################################
# Dependency Preparation
###############################################################################
dependency-prep: dependency-prep-schema dependency-prep-digits dependency-prep-config

dependency-prep-schema:
	echo "USE PiSearchContainerTesting;" > $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting/schema.sql
	cat ../../src/StringSearch.Api/schema.sql >> $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting/schema.sql

# TODO: slightly odd to run this interacftively - would probably be clearer cut to have an image
#	built in the main makefile, so the process is the same for this & the API image.
dependency-prep-digits:
	export MSYS_NO_PATHCONV=1; \
	docker run --rm \
		-e INPUT_DIGITS_DIR=/input \
		-e OUTPUT_DIR=/output \
		-v $(PWD)/../../src/StringSearch.IndexGenerator/out:/app \
		-v $(PWD)/digits:/input \
		-v $(ABSOLUTE_ARTEFACTS_DIR_PATH)/containerTesting/digits:/output \
		-w="/app" \
		--entrypoint dotnet \
		mcr.microsoft.com/dotnet/runtime:7.0.0-alpine3.16 \
		StringSearch.IndexGenerator.dll

dependency-prep-config:
	cp appsettings.prod.template.json $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting/appsettings.prod.json

	# TODO: Should we DRY the mysql creds between the app config & docker-compose yaml

###############################################################################
# Run SUT
###############################################################################
sut-start:
	cd $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting && \
		docker-compose up -d

	# TODO: wait for the system to come up..? - or maybe do that in Specflow...

sut-stop:
	cd $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting && \
		docker-compose down

docker-compose-generate:
	cp docker-compose.template.yaml $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting/docker-compose.yaml

	sed -i s!{{IMAGE_API}}!$(IMAGE_API):$(shell cat $(UNIQUEIFIER_PATH))!g $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting/docker-compose.yaml
	sed -i s/{{SUT_PORT_NUMBER}}/$(SUT_PORT_NUMBER)/g $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting/docker-compose.yaml
	sed -i s/{{UNIQUEIFIER}}/$(shell cat $(UNIQUEIFIER_PATH))/g $(RELATIVE_ARTEFACTS_DIR_PATH)/containerTesting/docker-compose.yaml

###############################################################################
# Run Tests
###############################################################################
container-tests-run: build-container-test-image
	docker run --rm \
		--net=host \
		-e API_BASE_URI=http://localhost:$(SUT_PORT_NUMBER) \
		-v $(ABSOLUTE_ARTEFACTS_DIR_PATH)/testResults:/artefacts/testResults \
		$(IMAGE_CONTAINER_TESTS):$(shell cat $(UNIQUEIFIER_PATH))